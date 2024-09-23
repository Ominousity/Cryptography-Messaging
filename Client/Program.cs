using System.Net;
using Socketwrapper;

var socketWrapper = new SocketWrapper();

// Use the host machine's IP address or hostname
IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

await socketWrapper.Connect(ip);

while (true)
{
    string? data = Console.ReadLine();
    await socketWrapper.Send(data!);
}
