import socket
from typing import List, Optional, Any, Dict
from langchain.chat_models.base import BaseChatModel
from langchain.schema import (
    AIMessage,
    SystemMessage,
    HumanMessage,
    BaseMessage,
    ChatGeneration,
    ChatResult,
)
from pydantic import Field
from socketprompts import ASSISTANT_PROMPT

class ChatSocket(BaseChatModel):
    """
    ChatSocket implements a socket-based LLM interface that mimics the behavior of ChatOllama.
    It connects to a server running on localhost (using the provided host and port) and sends 
    complete chat prompts constructed from the conversation history. The response from the 
    server is captured and returned as a ChatResult.

    Example usage:
        llm = ChatSocket(port=12345)
        # Then you can add it into a chain like:
        # question_router = QUESTION_ROUTER_PROMPT | llm | parser_runnable
    """

    # Pydantic model fields – these appear as arguments in the constructor.
    host: str = Field("127.0.0.1", description="Host for the socket connection.")
    port: int = Field(..., description="Port on localhost where the server is listening.")
    read_timeout: float = Field(600.0, description="Timeout (in seconds) for reading from the socket.")

    # Private runtime attributes (not part of the Pydantic model)
    _sock: Optional[socket.socket] = None
    _greeting: str = ""

    class Config:
        extra = "ignore"

    @property
    def _llm_type(self) -> str:
        """Identifying string for this LLM type."""
        return "chat_socket"

    @property
    def _identifying_params(self) -> Dict[str, Any]:
        """
        Returns a dictionary of parameters that are used to uniquely identify this instance.
        Mimicking ChatOllama, we include the host, port, and read_timeout.
        """
        return {"host": self.host, "port": self.port, "read_timeout": self.read_timeout}

    def __init__(self, **kwargs: Any):
        """
        Initializes a ChatSocket instance. Expects host, port, and read_timeout as keyword arguments.
        Immediately attempts to establish a socket connection to the Llama server.
        """
        super().__init__(**kwargs)
        self._connect()

    def _connect(self) -> None:
        """
        Establishes a TCP socket connection to the server. The socket is configured with
        the specified timeout, and an initial greeting (if any) is read from the server.
        """
        self._sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        try:
            self._sock.connect((self.host, self.port))
        except Exception as e:
            raise ConnectionError(f"Could not connect to {self.host}:{self.port} - {e}")
        self._sock.settimeout(self.read_timeout)
        try:
            greeting = self._sock.recv(1024).decode("utf-8")
        except socket.timeout:
            greeting = ""
        self._greeting = greeting

    def _get_chat_prompt(self, messages: List[BaseMessage]) -> str:
        """
        Constructs the prompt by joining all messages from the conversation history.
        Each message is prefixed by its role (System, User, AI) in a manner similar to ChatOllama.
        """
        lines = []
        for message in messages:
            if isinstance(message, SystemMessage):
                lines.append("System: " + message.content)
            elif isinstance(message, HumanMessage):
                lines.append("User: " + message.content)
            elif isinstance(message, AIMessage):
                lines.append("AI: " + message.content)
            else:
                lines.append(message.content)
        return "\n".join(lines)

    def _generate_chat(
        self,
        messages: List[BaseMessage],
        stop: Optional[List[str]] = None,
        **kwargs: Any,
    ) -> ChatResult:
        """
        Constructs the complete prompt from the chat messages, sends it over the socket,
        and listens for the generated response. The response is assumed to contain an end marker 
        (e.g. "[Stats:") that indicates completion.
        
        The method prints the raw data (if desired for debugging) and returns the final response 
        as a ChatResult containing a single ChatGeneration.
        """
        # print("================================================")
        # print(messages)
        #messages = [SystemMessage(content=ASSISTANT_PROMPT.format(question=""))] + messages
        # print(messages)
        # print("================================================")
        prompt = self._get_chat_prompt(messages)
        #prompt = messages
        if not prompt.endswith("\n"):
            prompt += "\n"
        print("Sending prompt:", prompt)
        try:
            self._sock.sendall(prompt.encode("utf-8"))
        except Exception as e:
            raise RuntimeError(f"Failed to send prompt over socket: {e}")

        response_data = ""
        end_marker = "[Stats:"  # The server is expected to include this marker when finished.
        while True:
            try:
                chunk = self._sock.recv(1024)
                if not chunk:
                    break
                response_data += chunk.decode("utf-8")
                if end_marker in response_data:
                    break
            except socket.timeout:
                break

        # You can print the raw response for debugging:
        print("Raw response received:", response_data)

        if end_marker in response_data:
            response_text = response_data.split(end_marker)[0]
        else:
            response_text = response_data

        response_text = response_text.strip()
        if response_text.startswith("AI:"):
            # Remove the "AI:" prefix if present.
            response_text = response_text[3:].strip()

        ai_message = AIMessage(content=response_text)
        generation = ChatGeneration(message=ai_message, text=response_text)
        return ChatResult(generations=[generation])

    def _generate(
        self,
        messages: List[BaseMessage],
        stop: Optional[List[str]] = None,
        **kwargs: Any,
    ) -> ChatResult:
        """
        Implements the abstract _generate method (required by BaseChatModel) by deferring to _generate_chat.
        """
        return self._generate_chat(messages, stop=stop, **kwargs)

    def _combine_llm_outputs(self, llm_outputs: List[dict]) -> dict:
        """
        Combines multiple LLM outputs if necessary. In this implementation,
        we simply join the "output" fields from non-None outputs.
        """
        outputs = [o.get("output", "") for o in llm_outputs if o is not None]
        return {"output": "\n".join(outputs)}

    def __del__(self) -> None:
        try:
            if self._sock:
                self._sock.close()
        except Exception:
            pass
