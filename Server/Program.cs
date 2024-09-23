using System.Net;
using Socketwrapper;

var socketWrapper = new SocketWrapper();

IPEndPoint ip = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 5000); // Listen on all network interfaces

await socketWrapper.Listen(ip);

while (true)
{
    await socketWrapper.Receive();
}
