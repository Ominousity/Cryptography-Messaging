using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Socketwrapper
{
    public class SocketWrapper : IDisposable
    {
        const int BUFFER_SIZE = 1_024;

        private Socket? _listener;
        private Socket? _socket;

        public void Dispose()
        {
            _listener?.Dispose();
            _socket?.Dispose();
        }

        public async Task Connect(IPEndPoint ipEndPoint)
        {
            _socket = new Socket(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            Console.WriteLine("Connecting...");

            await _socket.ConnectAsync(ipEndPoint);
        }

        public void Disconnect()
        {
            _socket?.Shutdown(SocketShutdown.Both);
        }

        public async Task Listen(IPEndPoint ipEndPoint)
        {
            _listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listener.Bind(ipEndPoint);
            _listener.Listen(100);

            Console.WriteLine("Listening...");

            _socket = await _listener.AcceptAsync();
        }

        public async Task Send(string message)
        {
            var encryptedMessage = AesGcmEncryption.Encrypt(message);
            Console.WriteLine($"Encrypted message: {Convert.ToBase64String(encryptedMessage)}");
            await SendBytes(encryptedMessage);
            Console.WriteLine($"Socket sent encrypted message.");
        }

        public async Task SendBytes(byte[] message)
        {
            _ = await _socket!.SendAsync(message, SocketFlags.None);
        }

        public async Task<string> Receive()
        {
            var encryptedMessage = await ReceiveBytes();
            Console.WriteLine($"Received encrypted message: {Convert.ToBase64String(encryptedMessage)}");
            var decryptedMessage = AesGcmEncryption.Decrypt(encryptedMessage);
            Console.WriteLine($"Socket received decrypted message: \"{decryptedMessage}\"");
            return decryptedMessage;
        }

        public async Task<byte[]> ReceiveBytes()
        {
            var buffer = new byte[BUFFER_SIZE];
            if (_socket == null)
            {
                throw new InvalidOperationException("Socket is not connected.");
            }
            var received = await _socket.ReceiveAsync(buffer, SocketFlags.None);
            return buffer.Take(received).ToArray();
        }
    }
}
