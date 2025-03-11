using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;


namespace Pillbox
{
   
public class CommunicationManager
{
    private string serverIp;
    private int port;
    private TcpClient client;
    private NetworkStream stream;

    public CommunicationManager(string serverIp, int port)
    {
        this.serverIp = serverIp;
        this.port = port;
    }

    public bool Connect()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIp, port);
            stream = client.GetStream();
            Console.WriteLine("Connected to Python Server.");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Connection failed: " + e.Message);
            return false;
        }
    }

    public void SendMessage(string message)
    {
        try
        {
            if (stream == null) throw new Exception("Not connected to the server.");
            Debug.WriteLine(message);
            byte[] data = Encoding.UTF8.GetBytes(message);
            byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthPrefix); // Convert to big-endian

            //stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine("Send failed: " + e.Message);
        }
    }

    public string ReceiveMessage()
    {
        try
        {
            if (stream == null) throw new Exception("Not connected to the server.");

                //byte[] lengthPrefix = new byte[4];
                //stream.Read(lengthPrefix, 0, 4);

                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(lengthPrefix); // Convert back to big-endian

                //int messageLength = BitConverter.ToInt32(lengthPrefix, 0);
                //byte[] data = new byte[messageLength];

                //int totalBytesRead = 0;
                //while (totalBytesRead < messageLength)
                //{
                //    int bytesRead = stream.Read(data, totalBytesRead, messageLength - totalBytesRead);
                //    if (bytesRead == 0) break;
                //    totalBytesRead += bytesRead;
                //}

            byte[] data = Utils.ReceiveAll(stream);
                     
            return Encoding.UTF8.GetString(data);
        }
        catch (Exception e)
        {
            Console.WriteLine("Receive failed: " + e.Message);
            return string.Empty;
        }
    }

    public void Disconnect()
    {
        try
        {
            stream?.Close();
            client?.Close();
            Console.WriteLine("Disconnected from Python Server.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Disconnection failed: " + e.Message);
        }
    }
}

}
