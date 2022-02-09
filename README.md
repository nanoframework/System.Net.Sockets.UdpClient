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
| nanoFramework.System.Net.Sockets.UdpClient (preview) | [![Build Status](https://dev.azure.com/nanoframework/System.Net.Sockets.UdpClient/_apis/build/status/nanoframework.System.Net.Sockets.UdpClient?repoName=nanoframework%2FSystem.Net.Sockets.UdpClient&branchName=develop)](https://dev.azure.com/nanoframework/System.Net.Sockets.UdpClient/_build/latest?definitionId=92&repoName=nanoframework%2FSystem.Net.Sockets.UdpClient&branchName=develop) | [![NuGet](https://img.shields.io/nuget/vpre/nanoFramework.System.Net.Sockets.UdpClient.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Net.Sockets.UdpClient/) |

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

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
