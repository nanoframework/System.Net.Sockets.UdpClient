//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Net;

namespace System.Net.Sockets
{
    /// <summary>
    /// Provides User Datagram Protocol (UDP) network services. Can be used for both client and server roles.
    /// </summary>
    /// <remarks>
    /// For the Receive methods if the buffer is smaller than the packet to receive it will be truncated to
    /// the buffer size without warning. Indeed "lwIP", the TCP/IP stack used commonly by RTOS, doesn't
    /// support the MSG_TRUNC socket option in calls to recvfrom to return the real length of the datagram
    /// when it is longer than the passed buffer opposite to common Un*x implementations.
    /// </remarks>
    public class UdpClient : IDisposable
    {
        private readonly Socket _clientSocket = null!; // initialized by helper called from ctor
        private const AddressFamily _family = AddressFamily.InterNetwork;
        private const string _IPv4OnlySupport = "UDPClient is only supported on IPv4";

        /// <summary>
        /// Create a new <see cref="UdpClient"/> that is not bind to any local port at this time. 
        /// Requires a call to connect to be bind locally on some udp port.
        /// </summary>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public UdpClient()
        {
            _clientSocket = new Socket(_family, SocketType.Dgram, ProtocolType.Udp);
        }

        /// <summary>
        /// Create a new <see cref="UdpClient"/> and bind it to the local port number provided on
        /// all IP addresses (<see cref="IPAddress.Any"/>) 
        /// </summary>
        /// <param name="port">The local port number from which you intend to communicate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The port parameter is not between 0 and 65535.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public UdpClient(int port)
        {
            // Validate input parameters.
            if (port < 0 || port > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(port));

            IPEndPoint localEP;
            localEP = new IPEndPoint(IPAddress.Any, port);

            _clientSocket = new Socket(_family, SocketType.Dgram, ProtocolType.Udp);
            _clientSocket.Bind(localEP);
        }

        /// <summary>
        /// Create a new <see cref="UdpClient"/> and bind it to the specified local endpoint.
        /// </summary>
        /// <param name="localEP">An <see cref="IPEndPoint"/> that represents the local endpoint to which bind the UDP connection.</param>
        /// <exception cref="ArgumentNullException">locelEP is null.</exception>
        /// <exception cref="ArgumentException">localEP isn't an IPv4 Address</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public UdpClient(IPEndPoint localEP)
        {
            // Validate input parameters.
            if (localEP is null)
                throw new ArgumentNullException(nameof(localEP));

            if (localEP.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException(_IPv4OnlySupport);

            _clientSocket = new Socket(_family, SocketType.Dgram, ProtocolType.Udp);
            _clientSocket.Bind(localEP);
        }

        /// <summary>
        /// Gets or sets a value indicating whether a default remote host has been established.
        /// </summary>
        protected bool Active { get; private set; }

        /// <summary>
        /// Amount of data received from the network that is available to read.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public int Available => _clientSocket.Available;

        /// <summary>
        /// Get underlying <see cref="Socket"/>
        /// </summary>
        public Socket Client => _clientSocket;

        /// <summary>
        /// Time To Live (TTL) value of the IP packets sent through the <see cref="UdpClient"/>
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public short Ttl
        {
            get => (short)(int)_clientSocket.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive)!;
            set
            {
                if (value < 0 || value > 255)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _clientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, value);
            }
        }

        /// <summary>
        /// Specify if multicast packets delivered to the sending application.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public bool MulticastLoopback
        {
            get => (int)_clientSocket.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback) != 0;
            set => _clientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, value ? 1 : 0);
        }

        /// <summary>
        /// Specify if the <see cref="UdpClient"/> can send broadcast packet.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public bool EnableBroadcast
        {
            get => (int)_clientSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast) != 0;
            set => _clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, value ? 1 : 0);
        }

        /// <summary>
        /// Specify if the <see cref="UdpClient"/> allows only one client per port.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public bool ExclusiveAddressUse
        {
            get => (int)_clientSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse)! != 0;
            set => _clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, value ? 1 : 0);
        }

        /// <summary>
        /// Allows fragmentation of IP datagram send through this <see cref="UdpClient"/>. This controls the IP DF flag.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public bool DontFragment
        {
            get => (int)_clientSocket.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment)! != 0;
            set => _clientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment, value ? 1 : 0);
        }

        /// <summary>
        /// Establishes a default remote host using the specified IP address and port number.
        /// </summary>
        /// <param name="addr">The <see cref="IPAddress"/> of the remote host to which you intend to send data.</param>
        /// <param name="port">The port number to which you intend send data.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is not between 0 and 65535</exception>
        /// <exception cref="ArgumentNullException"><paramref name="addr"/> is null</exception>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public void Connect(IPAddress addr, int port)
        {
            ThrowIfDisposed();

            if (port < 0 || port > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            IPEndPoint endPoint = new IPEndPoint(addr ?? throw new ArgumentNullException(nameof(addr)), port);
            Connect(endPoint);
        }

        /// <summary>
        /// Establishes a default remote host using the specified IP address and port number.
        /// </summary>
        /// <param name="endPoint">An <see cref="IPEndPoint"/> that specifies the network endpoint to which you intend to send data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="endPoint"/> is null</exception>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public void Connect(IPEndPoint endPoint)
        {
            ThrowIfDisposed();

            Client.Connect(endPoint ?? throw new ArgumentNullException(nameof(endPoint)));
            Active = true;
        }

        /// <summary>
        /// Receive an UDP datagram that was sent by a remote host and store it in buffer.
        /// </summary>
        /// <param name="buffer">A byte array to store the datagram data</param>
        /// <param name="remoteEP">An <see cref="IPEndPoint"/> that represents the remote host from which the data was sent.</param>
        /// <returns>Length of the datagram received. Will be equal to the size of <paramref name="buffer"/> if the datagram size was equal or bigger than the <paramref name="buffer"/> size.</returns>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        /// <remarks>
        /// If the buffer is smaller than the packet to receive it will be truncated to
        /// the buffer size without warning. Indeed "lwIP", the TCP/IP stack used commonly by RTOS, doesn't
        /// support the MSG_TRUNC socket option in calls to recvfrom to return the real length of the datagram
        /// when it is longer than the passed buffer opposite to common Un*x implementations.
        /// </remarks>
        public int Receive(byte[] buffer, ref IPEndPoint remoteEP) => Receive(buffer, 0, buffer.Length, ref remoteEP);

        /// <summary>
        /// Receive an UDP datagram that was sent by a remote host and store it in buffer.
        /// </summary>
        /// <param name="buffer">A byte array to store the datagram data.</param>
        /// <param name="offset">Offset where the datagram received must be stored in <paramref name="buffer"/>.</param>
        /// <param name="size">Maximum size of the datagram to receive.</param>
        /// <param name="remoteEP">An <see cref="IPEndPoint"/> that represents the remote host from which the data was sent.</param>
        /// <returns>Length of the datagram received. Will be equal to the size of <paramref name="buffer"/> if the datagram size was equal or bigger than the <paramref name="buffer"/> size.</returns>
        /// <exception cref="ArgumentException"><paramref name="buffer"/> length is too short compared to <paramref name="offset"/> and <paramref name="size"/></exception>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        /// <remarks>
        /// If <paramref name="size"/> is smaller than the packet to receive it will be truncated to
        /// a length of <paramref name="size"/> without warning. Indeed "lwIP", the TCP/IP stack used commonly by RTOS, doesn't
        /// support the MSG_TRUNC socket option in calls to recvfrom to return the real length of the datagram
        /// when it is longer than the passed buffer opposite to common Un*x implementations.
        /// </remarks>
        public int Receive(byte[] buffer, int offset, int size, ref IPEndPoint remoteEP)
        {
            ThrowIfDisposed();

            if (buffer.Length < offset + size)
            {
                throw new ArgumentException("Buffer is too short for offset and size", nameof(buffer));
            }

            EndPoint tempRemoteEP = new IPEndPoint(0, 0);

            int received = _clientSocket.ReceiveFrom(buffer, offset, size, 0, ref tempRemoteEP);
            remoteEP = (IPEndPoint)tempRemoteEP;
            return received;
        }

        /// <summary>
        /// Sends a UDP datagram to the specified remote host.
        /// </summary>
        /// <param name="dgram">ByteArray containing the datagram to send.</param>
        /// <param name="offset">Offset where the datagram start in <paramref name="dgram"/>.</param>
        /// <param name="size">Length of the datagram.</param>
        /// <param name="endPoint">Remote endpoint to send the datagram to. Must be <code>null</code> if the <see cref="UdpClient"/> is connected to a remote endpoint.</param>
        /// <returns>Number of byte sent</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dgram"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="dgram"/> length is too short compared to <paramref name="offset"/> and <paramref name="size"/></exception>
        /// <exception cref="InvalidOperationException">A remote <paramref name="endPoint"/> is specified but the <see cref="UdpClient"/> is already connected.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public int Send(byte[] dgram, int offset, int size, IPEndPoint endPoint)
        {
            ThrowIfDisposed();

            if (dgram is null)
            {
                throw new ArgumentNullException(nameof(dgram));
            }

            if (dgram.Length < offset + size)
            {
                throw new ArgumentException("Buffer is too short for offset and size", nameof(dgram));
            }

            if (!Active) // not connected client
            {
                if (endPoint is null)
                {
                    // Require remote host when not connected
                    throw new InvalidOperationException("UdpClient not connected");
                }

                return _clientSocket.SendTo(dgram, offset, size, SocketFlags.None, endPoint);
            }

            if (endPoint is not null)
            {
                // Do not allow sending packets to arbitrary host when connected
                throw new InvalidOperationException("UdpClient already connected");
            }

            return _clientSocket.Send(dgram, offset, size, SocketFlags.None);
        }

        /// <summary>
        /// Sends a UDP datagram to the specified remote host.
        /// </summary>
        /// <param name="dgram">ByteArray containing the datagram to send</param>
        /// <param name="endPoint">Remote endpoint to send the datagram to. Must be <code>null</code> if the <see cref="UdpClient"/> is connected to a remote endpoint.</param>
        /// <returns>Number of byte sent</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dgram"/> is null.</exception>
        /// <exception cref="InvalidOperationException">A remote <paramref name="endPoint"/> is specified but the <see cref="UdpClient"/> is already connected.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public int Send(byte[] dgram, IPEndPoint endPoint) => Send(dgram, 0, dgram.Length, endPoint);

        /// <summary>
        /// Send an UDP datagram to the connected host.
        /// </summary>
        /// <param name="dgram">ByteArray containing the datagram to send.</param>
        /// <param name="offset">Offset where the datagram start in <paramref name="dgram"/>.</param>
        /// <param name="size">Length of the datagram.</param>
        /// <returns>Number of byte sent</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dgram"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="dgram"/> length is too short compared to <paramref name="offset"/> and <paramref name="size"/>.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="UdpClient"/> is not connected.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public int Send(byte[] dgram, int offset, int size) => Send(dgram, offset, size, null);

        /// <summary>
        /// Send an UDP datagram to the connected host.
        /// </summary>
        /// <param name="dgram">ByteArray containing the datagram to send</param>
        /// <returns>Number of byte sent</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dgram"/> is null</exception>
        /// <exception cref="InvalidOperationException">The <see cref="UdpClient"/> is not connected.</exception>
        /// <exception cref="ObjectDisposedException"><see cref="UdpClient"/> is already disposed.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public int Send(byte[] dgram) => Send(dgram, 0, dgram.Length, null);

        /// <summary>
        /// Adds the <see cref="UdpClient"/> to a multicast group.
        /// </summary>
        /// <param name="multicastAddr"><see cref="IPAddress"/> of the multicast group to join.</param>
        /// <exception cref="ArgumentNullException"><paramref name="multicastAddr"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="multicastAddr"/> isn't an IPv4 address.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public void JoinMulticastGroup(IPAddress multicastAddr) => JoinMulticastGroup(multicastAddr, IPAddress.Any);

        /// <summary>
        /// Adds the <see cref="UdpClient"/> to a multicast group.
        /// </summary>
        /// <param name="multicastAddr"><see cref="IPAddress"/> of the multicast group to join.</param>
        /// <param name="localAddress">Local <see cref="IPAddress"/> for joining the group.</param>
        /// <exception cref="ArgumentNullException">One of the arguments is null.</exception>
        /// <exception cref="ArgumentException">One of the argument isn't an IPv4 address.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public void JoinMulticastGroup(IPAddress multicastAddr, IPAddress localAddress)
        {
            ThrowIfDisposed();

            // Validate input parameters.
            if (multicastAddr is null)
            {
                throw new ArgumentNullException(nameof(multicastAddr));
            }

            if (localAddress is null)
            {
                throw new ArgumentNullException(nameof(localAddress));
            }

            if (multicastAddr.AddressFamily != AddressFamily.InterNetwork || localAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException(_IPv4OnlySupport);
            }

            MulticastOption mcOpt = new MulticastOption(multicastAddr, localAddress);
            _clientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, mcOpt.GetBytes());
        }

        /// <summary>
        /// Leaves a multicast group address
        /// </summary>
        /// <param name="multicastAddr"><see cref="IPAddress"/> of the multicast group to leave.</param>
        /// <exception cref="ArgumentNullException"><paramref name="multicastAddr"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="multicastAddr"/> isn't an IPv4 address.</exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public void DropMulticastGroup(IPAddress multicastAddr)
        {
            ThrowIfDisposed();

            if (multicastAddr is null)
            {
                throw new ArgumentNullException(nameof(multicastAddr));
            }

            if (multicastAddr.AddressFamily != _family)
            {
                throw new ArgumentException(_IPv4OnlySupport, nameof(multicastAddr));
            }

            MulticastOption mcOpt = new MulticastOption(multicastAddr);
            _clientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, mcOpt.GetBytes());
        }

        /// <summary>
        /// Define the local address (and thus interface) used to send multicast packets from this UdpClient
        /// This is only recommended on multi-homed systems
        /// </summary>
        /// <param name="localAddress">IP address of the interface used to send multicast packets</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SocketException">Error on the underlying socket.</exception>
        public void SetMulticastInterface(IPAddress localAddress)
        {
            ThrowIfDisposed();

            if (localAddress is null)
            {
                throw new ArgumentNullException(nameof(localAddress));
            }
            if (localAddress.AddressFamily != _family)
            {
                throw new ArgumentException(_IPv4OnlySupport, nameof(localAddress));
            }

            _clientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, localAddress.GetAddressBytes());
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public void Close() => Dispose();

        private bool _disposed = false;
        /// <inheritdoc/>
        public void Dispose()
        {
            _disposed = true;
            ((IDisposable)_clientSocket)?.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(UdpClient));
            }
        }
    }
}
