using System;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using static Pillbox.Utils;
using File = System.IO.File;
using System.Net;
using System.Text.Json;
using Microsoft.Win32;
using Pillbox.Services;



namespace Pillbox.Managers
{
    // Enum to represent the source using the channel.
    public enum ChannelOwner
    {
        None,
        VoiceAssistant,
        TextChat,
        Internal,
        Event
    }

    // Custom event arguments for when a reply is received.
    public class ReplyReceivedEventArgs : EventArgs
    {
        public string Reply { get; }
        public ChannelOwner Owner { get; }

        public ReplyReceivedEventArgs(string reply, ChannelOwner owner)
        {
            Reply = reply;
            Owner = owner;
        }
    }

    public class CommunicationManager
    {
        private string serverIp;
        private int port;
        private TcpClient client;
        private NetworkStream stream;

        // Boolean flag indicating that a message has been sent.
        private bool _hasSentMessage = false;

        // Field to track which source is using the channel.
        private ChannelOwner _channelOwner = ChannelOwner.None;

        // Cancellation token to stop the asynchronous receive loop.
        private CancellationTokenSource _listeningCancellationTokenSource;

        // Public properties to check the channel status.
        public bool IsChannelInUse => _channelOwner != ChannelOwner.None;
        public ChannelOwner CurrentOwner => _channelOwner;

        public bool UseRag { get; set; }
        public bool UsePubmed { get; set; }

        public bool UseBiggerGranite { get; set; }

        public bool DebugMode { get; set; }

        public bool IsAIConnected => client?.Connected ?? false;


        // Event that is raised when a reply is received.
        public event EventHandler<ReplyReceivedEventArgs> ReplyReceivedForVoice;

        public event EventHandler<ReplyReceivedEventArgs> ReplyReceivedForText;

        public event EventHandler<ReplyReceivedEventArgs> ReplyReceivedForInternal;

        public event EventHandler<ReplyReceivedEventArgs> ReplyReceivedForEvent;

        public CommunicationManager(string serverIp, int port)
        {
            this.serverIp = serverIp;
            this.port = port;
            UseRag = true;
            UsePubmed = true;
            DebugMode = false;
            UseBiggerGranite = false;
        }

        public void startServers(string relativeDir, string exe, string? optionalFlag = null)
        {
            try
            {
                // Construct the absolute path to the executable.
                string exePath = Path.Combine(Environment.CurrentDirectory, relativeDir, exe);

                // Verify that the executable exists at the specified location.
                if (!File.Exists(exePath))
                {
                    MessageBox.Show("Executable not found at: " + exePath, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create and configure the ProcessStartInfo object.
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = Path.GetDirectoryName(exePath),
                    UseShellExecute = false,
                    CreateNoWindow = !DebugMode,
                };
                if (!string.IsNullOrWhiteSpace(optionalFlag))
                {
                    startInfo.Arguments = optionalFlag;
                }

                Process process = Process.Start(startInfo);
                if (process == null)
                {
                    MessageBox.Show("Process could not be started.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error launching application: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool Connect()
        {
            _channelOwner = ChannelOwner.Internal;

            try
            {
                // 1) Launch helper processes if not already running
                if (!IsAppRunning("RAGSocket"))
                {
                    if (UseBiggerGranite)
                        startServers("granite", "GraniteSocket.exe", "big");
                    else
                        startServers("granite", "GraniteSocket.exe");

                    startServers("RAGSocket", "RAGSocket.exe", $"--pubmed {UsePubmed}");
                }
                else
                {
                    Debug.WriteLine("RAGSocket is already running.");
                }

                // 2) Start the TCP listener right away
                var server = new TcpListener(IPAddress.Parse(serverIp), port);
                server.Start();
                Debug.WriteLine($"Server listening on {serverIp}:{port}. Waiting up to 60s for client…");

                // 3) Kick off the accept task
                var acceptTask = server.AcceptTcpClientAsync();
                // 4) Wait either for the client to connect or our 120s timeout
                if (!acceptTask.Wait(TimeSpan.FromSeconds(120)))
                {
                    Debug.WriteLine("Timeout waiting for RAGSocket to connect (120s elapsed).");
                    server.Stop();
                    _channelOwner = ChannelOwner.None;
                    return false;
                }

                // 5) Success → grab the client & stream
                client = acceptTask.Result;
                stream = client.GetStream();
                Debug.WriteLine("Client connected.");

                _channelOwner = ChannelOwner.None;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Connect() failed: " + ex.Message);
                _channelOwner = ChannelOwner.None;
                return false;
            }
        }


        /// <summary>
        /// Attempts to send a message on the channel from the specified source.
        /// Returns true if the message was sent, or false if the channel is in use by another source.
        /// </summary>
        public bool SendMessage(string message, ChannelOwner source)
        {
            SummaryPDF.GenerateSummaryPDF(IsInternal: true);
            if (!IsAIConnected)
            {
                MessageBox.Show("Not connected to the server. Please use reconnect in Settings");
                return false;
            }
            // Check if the channel is already in use by a different source.
            if (_channelOwner != ChannelOwner.None)
            {
                MessageBox.Show($"Channel is currently in use by {_channelOwner}. Please wait for its reply before trying again");
                return false;
            }

            try
            {
                if (stream == null)
                    throw new Exception("Not connected to the server.");

                // Set (or retain) the channel owner.
                _channelOwner = source;
                bool isInternal = ChannelOwner.Internal == source;
                bool rag = UseRag;
                var json = ChannelOwner.Event == source ? 
                    new { question = message, RAGBool = rag, Internal = true, Event = true}
                    : new { question = message, RAGBool = rag, Internal = isInternal, Event = false};

                // Serialize the object to a JSON string.
                string json_message = JsonSerializer.Serialize(json);
                Debug.WriteLine(message + "  "+ json_message);
                byte[] data = Encoding.UTF8.GetBytes(json_message);
                byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lengthPrefix); // Convert to big-endian

                // Write data to the network stream.
                stream.Write(data, 0, data.Length);

                // Mark that a message has been sent.
                _hasSentMessage = true;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Send failed: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Starts an asynchronous loop that continuously listens for incoming data
        /// without blocking the main thread.
        /// </summary>
        public async Task StartListeningForMessagesAsync(CancellationToken cancellationToken)
        {
            // A buffer to hold received data.
            byte[] buffer = new byte[4096];
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (stream != null)
                    {
                        // Await an asynchronous read. This call yields control until data is available.
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (bytesRead > 0)
                        {
                            string reply = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            // Fire the event if a message was previously sent.
                            if (_hasSentMessage)
                            {
                                OnReplyReceived(reply);
                                _hasSentMessage = false;
                                _channelOwner = ChannelOwner.None;
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // The cancellation token was triggered; exit the loop.
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Receive failed: " + ex.Message);
                    // Optionally you can break here or continue trying depending on your error-handling strategy.
                    break;
                }
            }
        }

        /// <summary>
        /// Starts the asynchronous listening for messages on a background Task.
        /// </summary>
        public void StartListening()
        {
            _listeningCancellationTokenSource = new CancellationTokenSource();
            // Launch the async receive loop in a fire-and-forget task.
            _ = StartListeningForMessagesAsync(_listeningCancellationTokenSource.Token);
        }

        /// <summary>
        /// Stops the asynchronous listening for messages.
        /// </summary>
        public void StopListening()
        {
            _listeningCancellationTokenSource?.Cancel();
        }

        public void Disconnect()
        {
            try
            {
                StopListening();
                stream?.Close();
                client?.Close();
                Debug.WriteLine("Disconnected from Python Server.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Disconnection failed: " + e.Message);
            }
        }

        // Helper method to raise the ReplyReceived event.
        protected virtual void OnReplyReceived(string reply)
        {
            if (_channelOwner == ChannelOwner.VoiceAssistant)
            {
                ReplyReceivedForVoice?.Invoke(this, new ReplyReceivedEventArgs(reply, ChannelOwner.VoiceAssistant));
            }
            else if (_channelOwner == ChannelOwner.TextChat)
            {
                ReplyReceivedForText?.Invoke(this, new ReplyReceivedEventArgs(reply, ChannelOwner.TextChat));
            }
            else if (_channelOwner == ChannelOwner.Event) 
            {
                ReplyReceivedForEvent?.Invoke(this, new ReplyReceivedEventArgs(reply, ChannelOwner.Internal));

            }
            else
            {
                ReplyReceivedForInternal?.Invoke(this, new ReplyReceivedEventArgs(reply, ChannelOwner.Internal));
            }
                // Reset the state after handling the reply.
            _hasSentMessage = false;
            _channelOwner = ChannelOwner.None;

        }
    }
}
