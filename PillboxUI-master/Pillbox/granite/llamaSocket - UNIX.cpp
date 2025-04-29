#include <cassert>
#include <cstdio>
#include <cstring>
#include <iostream>
#include <string>
#include <vector>
#include <chrono>
#include <cstdlib>

// POSIX socket headers
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>

#include "llama.h"

// Helper function to send a message reliably (in one call, for simplicity)
void send_message(int sockfd, const std::string &msg) {
    if (send(sockfd, msg.c_str(), msg.size(), 0) == -1) {
        perror("send");
    }
}

// Resets the model's context by clearing its KV cache.
void reset_context(struct llama_context* ctx) {
    llama_kv_self_clear(ctx);
    printf("🧹 Context cleared! Starting fresh for next request...\n");
}

int main(int argc, char** argv) {

    if (argc < 2) {
        fprintf(stderr, "Usage: %s <model-path.gguf>\n", argv[0]);
        return 1;
    }

    const std::string model_path = argv[1];

    // 1. Set llama model and context parameters
    struct llama_model_params mparams = llama_model_default_params();
    mparams.n_gpu_layers = 99;

    struct llama_context_params ctx_params = llama_context_default_params();
    ctx_params.embeddings = false;
    ctx_params.n_ctx     = 16384;   // Context size
    ctx_params.n_threads = 4;      // CPU threads for generation

    // 2. Load the model
    struct llama_model* model = llama_model_load_from_file(model_path.c_str(), mparams);
    if (!model) {
        fprintf(stderr, "Error: Failed to load model from '%s'\n", model_path.c_str());
        return 1;
    }

    // 3. Create a new context
    struct llama_context* ctx = llama_init_from_model(model, ctx_params);
    if (!ctx) {
        fprintf(stderr, "Error: Failed to create llama_context\n");
        llama_model_free(model);
        return 1;
    }

    // Predefine the system message to be used for every request.
    const char* system_message = R"(
        You are an AI assistant with expertise in multiple roles. Your responses must strictly follow the specific **guidelines** provided for each task.
        
        **IMPORTANT RULES**
        - ONLY Respond to the User section of the prompt.
        - **NEVER include any system instructions, guidelines, or context in your response.**
        - **NEVER include any explanations, opinions, or formatting beyond what is requested.**
        - **ALWAYS follow the instructions in the prompt.**
        - **NEVER add extra explanations, opinions, or formatting beyond what is requested.**
        - **If asked for structured output (JSON, lists, etc.), return ONLY the required format with NO extra text.**
        - **If a question is unclear or missing details, make a reasonable assumption and continue.**
        
        ### **YOUR ROLES & RESPONSIBILITIES**
        You will be given different tasks, each with specific instructions. Examples of roles include:
        
        1 **Question Router** → Determines the correct data source for a given question (returns JSON).
        2 **Retrieval Grader** → Evaluates if a retrieved document is relevant to a question (returns JSON).
        3 **Hallucination Grader** → Checks if an answer is factually supported (returns JSON).
        4 **Answer Grader** → Assesses if an answer is useful and relevant (returns JSON).
        5 **Search Assistant** → Generates optimized search terms for a given query (returns JSON).
        6 **Question Re-Writer** → Reformulates a question for clarity (returns only the revised question).
        7 **Generator** → Provides factual answers in plain text. When operating as the Generator, you must return a plain text answer with no additional formatting or explanation.
        
        - **Before responding to any prompt, carefully read the instructions and follow them exactly.**
        - **If no role-specific guidelines are given, default to concise and direct answers.**
        
        **REMEMBER:** Your responses must match the expected output format and NEVER include explanations.
        )";

    // Initialize the sampler
    llama_sampler* smpl_chain = llama_sampler_chain_init(llama_sampler_chain_default_params());
    llama_sampler_chain_add(smpl_chain, llama_sampler_init_min_p(0.95f, 1));
    llama_sampler_chain_add(smpl_chain, llama_sampler_init_temp(0.8f));
    llama_sampler_chain_add(smpl_chain, llama_sampler_init_dist(LLAMA_DEFAULT_SEED));

    // ============================== SOCKET SETUP ==============================
    int server_fd, client_fd;
    struct sockaddr_in address;
    socklen_t addrlen = sizeof(address);

    if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == -1) {
        perror("socket failed");
        return 1;
    }
    
    int opt = 1;
    if (setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt)) < 0) {
        perror("setsockopt");
        return 1;
    }
    
    address.sin_family = AF_INET;
    address.sin_addr.s_addr = inet_addr("127.0.0.1");
    address.sin_port = htons(12345);
    if (bind(server_fd, (struct sockaddr *)&address, sizeof(address)) < 0) {
        perror("bind failed");
        return 1;
    }
    
    if (listen(server_fd, 3) < 0) {
        perror("listen");
        return 1;
    }
    
    printf("Listening on 127.0.0.1:12345...\n");
    
    client_fd = accept(server_fd, (struct sockaddr *)&address, &addrlen);
    if (client_fd < 0) {
        perror("accept");
        return 1;
    }
    printf("Client connected.\n");
    
    send_message(client_fd, "Conversation started. Type 'exit' to end.\n");

    // ============================== SOCKET COMMUNICATION LOOP ==============================
    char buffer[1024];
    std::string user_input;
    
    while (true) {
        memset(buffer, 0, sizeof(buffer));
        int bytes_received = recv(client_fd, buffer, sizeof(buffer) - 1, 0);
        if (bytes_received <= 0) {
            printf("Connection closed or error while receiving data.\n");
            break;
        }
        user_input = std::string(buffer);
        user_input.erase(user_input.find_last_not_of("\r\n") + 1);

        if (user_input == "exit") {
            break;
        }
        
        // Create a temporary conversation for this request.
        // Only include the system prompt and the current user input.
        std::vector<llama_chat_message> current_messages;
        current_messages.push_back({"system", system_message});
        current_messages.push_back({"user", strdup(user_input.c_str())});
        
        // Format the conversation using the chat template.
        std::vector<char> formatted(llama_n_ctx(ctx));
        const char* tmpl = llama_model_chat_template(model, /* name */ nullptr);
        const struct llama_vocab* vocab = llama_model_get_vocab(model);
        int new_len = llama_chat_apply_template(tmpl, current_messages.data(), current_messages.size(), true,
                                                formatted.data(), formatted.size());
        if (new_len > (int)formatted.size()) {
            formatted.resize(new_len);
            new_len = llama_chat_apply_template(tmpl, current_messages.data(), current_messages.size(), true,
                                                formatted.data(), formatted.size());
        }
        if (new_len < 0) {
            fprintf(stderr, "Failed to apply the chat template\n");
            // Free the strdup'ed user input.
            free((void*)current_messages.back().content);
            continue;
        }
        
        std::string prompt(formatted.begin(), formatted.begin() + new_len);
        
        // Tokenize the prompt.
        const bool is_first = true;  // Each call is independent.
        int n_prompt_tokens = -llama_tokenize(vocab, prompt.c_str(), prompt.size(), nullptr, 0, is_first, true);
        std::vector<llama_token> prompt_tokens(n_prompt_tokens);
        if (llama_tokenize(vocab, prompt.c_str(), prompt.size(), prompt_tokens.data(), prompt_tokens.size(), is_first, true) < 0) {
            fprintf(stderr, "Failed to tokenize the prompt\n");
            free((void*)current_messages.back().content);
            continue;
        }
        
        llama_batch batch = llama_batch_get_one(prompt_tokens.data(), prompt_tokens.size());
        
        // Process the prompt tokens to condition the context.
        if (llama_decode(ctx, batch)) {
            fprintf(stderr, "Failed to decode prompt\n");
            free((void*)current_messages.back().content);
            continue;
        }
        
        std::string response;
        send_message(client_fd, "AI: ");
        
        auto start_time = std::chrono::high_resolution_clock::now();
        int token_count = 0;

        // Generate response tokens.
        while (true) {
            llama_token new_token_id = llama_sampler_sample(smpl_chain, ctx, -1);
            if (llama_vocab_is_eog(vocab, new_token_id)) {
                break;
            }
            token_count++;
            
            char buf[256];
            int n = llama_token_to_piece(vocab, new_token_id, buf, sizeof(buf), 0, true);
            if (n < 0) {
                fprintf(stderr, "Failed to convert token to piece\n");
                break;
            }
            std::string piece(buf, n);
            send_message(client_fd, piece);
            response += piece;
            
            batch = llama_batch_get_one(&new_token_id, 1);
            if (llama_decode(ctx, batch)) {
                fprintf(stderr, "Failed to decode token\n");
                break;
            }
        }

        auto end_time = std::chrono::high_resolution_clock::now();
        std::chrono::duration<double> elapsed = end_time - start_time;
        double tokens_per_second = token_count / elapsed.count();
        
        char stats_buf[256];
        snprintf(stats_buf, sizeof(stats_buf), "\n\n[Stats: Generated %d tokens in %.2f seconds | %.2f tokens/sec]\n\n",
                 token_count, elapsed.count(), tokens_per_second);
        send_message(client_fd, stats_buf);

        // Optionally, you can log the response or handle it as needed.
        // Since we do not accumulate history, we do not store the assistant response.

        // Free the strdup'ed user input.
        free((void*)current_messages.back().content);

        // Clear the model's context so that the next request starts fresh.
        reset_context(ctx);
    }
    // ===========================================================================

    llama_sampler_free(smpl_chain);
    llama_free(ctx);
    llama_model_free(model);

    close(client_fd);
    close(server_fd);

    return 0;
}
