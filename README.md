[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_System.Net.Sockets.UdpClient&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_System.Net.Sockets.UdpClient) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_System.Net.Sockets.UdpClient&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_System.Net.Sockets.UdpClient) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.System.Net.Sockets.UdpClient.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Net.Sockets.UdpClient/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

# System.Net.Sockets.UdpClient

This API implements the UdpClient class with a pattern similar to the official .NET equivalent [System.Net.Sockets.UdpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient). Beside the lack of the asynchronous methods the receive functions don't use an internal buffer like the .NET API but instead requires a buffer to be passed as part of the call.

***Note:*** *For the `Receive` methods if the buffer is smaller than the packet to receive it will be truncated to the buffer size without warning. Indeed "lwIP", the TCP/IP stack used commonly by RTOS, doesn't support the `MSG_TRUNC` socket option in calls to `recvfrom` to return the real length of the datagram when it is longer than the passed buffer opposite to common Un\*x implementations.*

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.System.Net.Sockets.UdpClient | [![Build Status](https://dev.azure.com/nanoframework/System.Net.Sockets.UdpClient/_apis/build/status/nanoframework.System.Net.Sockets.UdpClient?repoName=nanoframework%2FSystem.Net.Sockets.UdpClient&branchName=main)](https://dev.azure.com/nanoframework/System.Net.Sockets.UdpClient/_build/latest?definitionId=92&repoName=nanoframework%2FSystem.Net.Sockets.UdpClient&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.System.Net.Sockets.UdpClient.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Net.Sockets.UdpClient/) |

## Usage

**Important:** Obviously UdpClient requires a working network connection with a valid IP address. Please check the examples with the Network Helpers on how to connect to a network.

The UdpClient class provides simple methods for sending and receiving UDP datagrams on an IP network. The current implementation supports only IPv4. IPv6 isn't supported on nanoFramework currently.

### Samples

Samples for `UdpClient` are present in the [nanoFramework Sample repository](https://github.com/nanoframework/Samples).

### Remote host

Because UDP is a connectionless protocol you don't need to establish a remote host connection before sending and receiving data. But you can define a default remote host that will be used for subsequent `Send` method calls. If you establish a default remote host, you cannot specify a different host when sending datagrams. You can define a default remote host with one of the following methods:

- Create your client using the `UdpClient(string hostname,string remoteport)` constructor.
- Create an instance and then call the `Connect` method.

### Client usage

Using `UdpClient` in client mode is pretty easy. You create an UdpClient with one of the constructor, establish or not a default remote host (see above) then you can send and receive message from the network. 

The following code show a typical client server exchange where you first send a message to the server and wait for the server answer:

```C#
// establish defaut host and port
UdpClient udpClient = new UdpClient("1.2.3.4", 5000);
udpClient.Send(Encoding.UTF8.GetBytes("Hello server"));

// receive message
byte[] buffer = new byte[1024];
IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Any, 0);
int length = udpClient.Receive(buffer, ref ipEndpoint);
Debug.WriteLine(Encoding.UTF8.GetString(buffer, 0, length));
```

### Server usage

Working as server you bind your `UdpClient` on a local port then you wait for messages from clients. As a server you don't know beforehand the IP address of your clients so you shouldn't define any remote host.

The following code show a simple echo server:

```C#
// Run echo protocol on port 5000
UdpClient udpClient = new UdpClient(5000); 

// We limit ourself to a 1024 bytes buffer
byte[] buffer = new byte[1024];
IPEndPoint endpointClient = new IPEndPoint(IPAddress.Any, 0);

// We send back every request we get
while (true)
{
    int length = udpClient.Receive(buffer, ref endpointClient);
    udpClient.Send(buffer,endpointClient);
}
```

### Multicast

If you want to use multicast ensure that you bind your `UdpClient` on the `0.0.0.0` (wilcard) address. If you bind your UdpClient to a specific `IPAddress` you won't receive the multicast datagrams.

Basically for a functional multicast client/server you need to:

- Create your UdpClient without binding it to a specific address.
- Join a Multicast group by a call to the JoinMulticastGroup method.
- Receive datagram sent to that group address with call to `Receive`
- Send message to the group multicast using `Send`
- Leave the Multicast group by calling the `DropMulticastGroup` method

The following sample illustrate that basic workflow:

```C#
// Create your UdpClient without binding it to a specific address
IPEndPoint iPEndpoint = new IPEndPoint(IPAddress.Any, 5000);
UdpClient client = new UdpClient(iPEndpoint);

// Join a Multicast group
IPAddress ipGroupMulticast = IPAddress.Parse("239.255.255.250");
client.JoinMulticastGroup(ipGroupMulticast);

bool StopListener = false;
byte[] buffer = new byte[2048];
while (!StopListener)
{
    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
    int length = client.Receive(buffer, ref remote);
    string result = Encoding.UTF8.GetString(buffer, 0, length);
    if (result == "Ping")
    {
        buffer = Encoding.UTF8.GetBytes("Present");
        client.Send(buffer,ipGroupMulticast);
    }
    StopListener = (result == "Exit");
}

// Leave the Multicast group
client.DropMulticastGroup(ipGroupMulticast);
```

If you want to receive your own messages you can enable this by setting the `UdpClient.MulticastLoopback` property to `true`.

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
