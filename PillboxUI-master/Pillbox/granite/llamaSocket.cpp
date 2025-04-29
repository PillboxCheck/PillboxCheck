#include <cassert>
#include <cstdio>
#include <cstring>
#include <iostream>
#include <string>
#include <vector>
#include <chrono>
#include <cstdlib>

#include <winsock2.h>
#include <ws2tcpip.h>
#pragma comment(lib, "ws2_32.lib")

#include "llama.h"

// Helper function to send a message reliably (in one call, for simplicity)
void send_message(SOCKET sockfd, const std::string &msg) {
    int sent = send(sockfd, msg.c_str(), static_cast<int>(msg.size()), 0);
    if (sent == SOCKET_ERROR) {
        fprintf(stderr, "send failed: %d\n", WSAGetLastError());
    }
}

// Resets the model's context by clearing its KV cache.
void reset_context(struct llama_context* ctx) {
    llama_kv_self_clear(ctx);
    printf("Context cleared! Starting fresh for next request...\n");
}

int main(int argc, char** argv) {
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        fprintf(stderr, "WSAStartup failed: %d\n", WSAGetLastError());
        return 1;
    }
    bool bigModel = false;
    if (argc > 1) {
        if (strcmp(argv[1], "big") == 0) {
            bigModel = true;
        } else {
            fprintf(stderr, "Usage: %s [big]\n", argv[0]);
            return 1;
        }
    }
    //model\granite-3.2-8b-instruct-Q3_K_L.gguf
    std::string model_path = "model\\granite-3.2-8b-instruct-Q3_K_L.gguf";
    if (bigModel)
    {model_path = "model\\granite-3.2-8b-instruct-Q6_K.gguf";} 
    

    struct llama_model_params mparams = llama_model_default_params();
    mparams.n_gpu_layers = 99;

    struct llama_context_params ctx_params = llama_context_default_params();
    ctx_params.embeddings = false;
    ctx_params.n_ctx     = 16384;
    ctx_params.n_threads = 4;

    struct llama_model* model = llama_model_load_from_file(model_path.c_str(), mparams);
    if (!model) {
        fprintf(stderr, "Error: Failed to load model from '%s'\n", model_path.c_str());
        return 1;
    }

    struct llama_context* ctx = llama_init_from_model(model, ctx_params);
    if (!ctx) {
        fprintf(stderr, "Error: Failed to create llama_context\n");
        llama_model_free(model);
        return 1;
    }

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

    llama_sampler* smpl_chain = llama_sampler_chain_init(llama_sampler_chain_default_params());
    // llama_sampler_chain_add(smpl_chain, llama_sampler_init_min_p(0.05f, 1));
    llama_sampler_chain_add(smpl_chain, llama_sampler_init_temp(0.5f));
    llama_sampler_chain_add(smpl_chain, llama_sampler_init_dist(LLAMA_DEFAULT_SEED));
    llama_sampler_chain_add(smpl_chain, llama_sampler_init_top_k(40));

    //llama_sampler_chain_add(smpl_chain, llama_sampler_init_top_p(0.5f, 1));
    // llama_sampler_chain_add(smpl_chain, llama_sampler_init_penalties(
    //     -1,   // last n tokens to penalize (0 = disable penalty, -1 = context size)
    //     1.5f,   // 1.0 = disabled
    //     0.0f,     // 0.0 = disabled
    //     0.0f     // 0.0 = disabled
    // ));

    SOCKET server_fd, client_fd;
    struct sockaddr_in address;
    int addrlen = sizeof(address);

    if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == INVALID_SOCKET) {
        fprintf(stderr, "socket failed: %d\n", WSAGetLastError());
        return 1;
    }

    int opt = 1;
    if (setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR, (const char*)&opt, sizeof(opt)) < 0) {
        fprintf(stderr, "setsockopt failed: %d\n", WSAGetLastError());
        return 1;
    }

    address.sin_family = AF_INET;
    address.sin_port = htons(12345);
    address.sin_addr.s_addr = inet_addr("127.0.0.1");

    if (bind(server_fd, (struct sockaddr *)&address, sizeof(address)) == SOCKET_ERROR) {
        fprintf(stderr, "bind failed: %d\n", WSAGetLastError());
        return 1;
    }

    if (listen(server_fd, 3) == SOCKET_ERROR) {
        fprintf(stderr, "listen failed: %d\n", WSAGetLastError());
        return 1;
    }

    printf("Listening on 127.0.0.1:12345...\n");

    client_fd = accept(server_fd, (struct sockaddr *)&address, &addrlen);
    if (client_fd == INVALID_SOCKET) {
        fprintf(stderr, "accept failed: %d\n", WSAGetLastError());
        return 1;
    }

    printf("Client connected.\n");
    send_message(client_fd, "Conversation started. Type 'exit' to end.\n");

    char buffer[16384];
    std::string user_input;

    while (true) {
        memset(buffer, 0, sizeof(buffer));
        int bytes_received = recv(client_fd, buffer, sizeof(buffer) - 1, 0);
        if (bytes_received <= 0) {
            printf("Connection closed or error while receiving data.\n");
            break;
        }
        user_input = std::string(buffer);
        size_t pos = user_input.find_last_not_of("\r\n");
        if (pos != std::string::npos) {
            user_input.erase(pos + 1);
        } else {
            user_input.clear();
        }

        if (user_input == "exit") {
            break;
        }

        // Build the conversation messages.
        std::vector<llama_chat_message> current_messages;
        current_messages.push_back({"system", system_message});
        char* user_msg = _strdup(user_input.c_str());
        if (!user_msg) {
            fprintf(stderr, "Memory allocation failed for user message.\n");
            continue;
        }
        current_messages.push_back({"user", user_msg});

        std::vector<char> formatted(llama_n_ctx(ctx));
        const char* tmpl = llama_model_chat_template(model, nullptr);
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
            for (auto &msg : current_messages) {
                if (std::string(msg.role) == "user") {
                    free((void*)msg.content);
                }
            }
            continue;
        }

        std::string prompt(formatted.begin(), formatted.begin() + new_len);
        const bool is_first = true;

        int n_prompt_tokens = -llama_tokenize(vocab, prompt.c_str(), prompt.size(), nullptr, 0, is_first, true);
        if (n_prompt_tokens <= 0) {
            fprintf(stderr, "Tokenization returned non-positive token count\n");
            for (auto &msg : current_messages) {
                if (std::string(msg.role) == "user") {
                    free((void*)msg.content);
                }
            }
            continue;
        }
        std::vector<llama_token> prompt_tokens(n_prompt_tokens);
        if (llama_tokenize(vocab, prompt.c_str(), prompt.size(), prompt_tokens.data(), prompt_tokens.size(), is_first, true) < 0) {
            fprintf(stderr, "Failed to tokenize the prompt\n");
            for (auto &msg : current_messages) {
                if (std::string(msg.role) == "user") {
                    free((void*)msg.content);
                }
            }
            continue;
        }

        llama_batch batch = llama_batch_get_one(prompt_tokens.data(), prompt_tokens.size());
        if (llama_decode(ctx, batch)) {
            fprintf(stderr, "Failed to decode prompt\n");
            for (auto &msg : current_messages) {
                if (std::string(msg.role) == "user") {
                    free((void*)msg.content);
                }
            }
            continue;
        }

        std::string response;
        send_message(client_fd, "");
        printf("Generating response...\n");
        auto start_time = std::chrono::high_resolution_clock::now();
        int token_count = 0;

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
            //send_message(client_fd, piece);
            response += piece;

            batch = llama_batch_get_one(&new_token_id, 1);
            if (llama_decode(ctx, batch)) {
                fprintf(stderr, "Failed to decode token\n");
                break;
            }
        }
        send_message(client_fd, response);
        auto end_time = std::chrono::high_resolution_clock::now();
        std::chrono::duration<double> elapsed = end_time - start_time;
        double tokens_per_second = (elapsed.count() > 0) ? token_count / elapsed.count() : 0;

        char stats_buf[256];
        snprintf(stats_buf, sizeof(stats_buf),
                 "\n\n[Stats: Generated %d tokens in %.2f seconds | %.2f tokens/sec]\n\n",
                 token_count, elapsed.count(), tokens_per_second);
        send_message(client_fd, stats_buf);
        printf("Response sent to client.\n");
        // Free dynamically allocated user message.
        // for (auto &msg : current_messages) {
        //     // if (std::string(msg.role) == "user") {
        //     //     free((void*)msg.content);
        //     // }
        //     free((void*)msg.content);
        // }
        // current_messages.clear();
        // printf("Message cleared.\n");

        // Reset context using the helper instead of a full reinitialization.
        reset_context(ctx);
        printf("Context reset.\n");
    }

    llama_sampler_free(smpl_chain);
    llama_free(ctx);
    llama_model_free(model);

    closesocket(client_fd);
    closesocket(server_fd);
    WSACleanup();

    return 0;
}
