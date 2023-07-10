using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Kadmium_sACN.MulticastAddressProvider;

namespace Kadmium_sACN.SacnReceiver
{
	public class SacnReceiver : IDisposable, ISacnReceiver
	{
		protected Socket Socket { get; private set; }
		public event EventHandler<DataPacket> OnDataPacketReceived;
		public event EventHandler<SynchronizationPacket> OnSynchronizationPacketReceived;
		public event EventHandler<UniverseDiscoveryPacket> OnUniverseDiscoveryPacketReceived;

		public IPEndPoint HostEndPoint => Socket?.LocalEndPoint as IPEndPoint;
		private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

		public SacnReceiver() : this(new SacnMulticastAddressProviderIPV4(), new SacnMulticastAddressProviderIPV6())
		{
		}

		protected SacnReceiver(ISacnMulticastAddressProvider ipv4Provider, ISacnMulticastAddressProvider ipv6Provider)
		{
			IPv4MulticastAddressProvider = ipv4Provider;
			IPv6MulticastAddressProvider = ipv6Provider;
		}

		public void Listen(IPAddress address)
		{
			var endpoint = new IPEndPoint(address, Constants.RemotePort);

			Pipe pipe = new Pipe();
			Socket = new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			Socket.Bind(endpoint);

			Task.Run(async () =>
			{
				while (!CancellationTokenSource.IsCancellationRequested)
				{
					var listenTask = ListenAsync(pipe.Writer, endpoint);
					var processTask = ProcessPackets(pipe.Reader);

					await Task.WhenAll(listenTask, processTask);
				}
			});

		}

		private async Task ListenAsync(PipeWriter writer, IPEndPoint endPoint)
		{
			while (true)
			{
				var memory = writer.GetMemory(DataPacket.MAX_LENGTH);
				try
				{
					var bytesRead = await Socket.ReceiveAsync(memory, SocketFlags.None);
					if (bytesRead == 0)
					{
						break;
					}
					writer.Advance(bytesRead);
				}
				catch (Exception e)
				{
					await Console.Error.WriteLineAsync(e.ToString());
					break;
				}

				var result = await writer.FlushAsync();

				if (result.IsCompleted)
				{
					break;
				}
			}
			await writer.CompleteAsync();
		}

		private async Task ProcessPackets(PipeReader reader)
		{
			var token = CancellationTokenSource.Token;
			while (true)
			{
				var result = await reader.ReadAsync(token);

				var buffer = result.Buffer;
				var packet = SacnPacket.Parse(buffer.FirstSpan);
				switch (packet)
				{
					case DataPacket dataPacket:
						OnDataPacketReceived?.Invoke(this, dataPacket);
						break;
					case SynchronizationPacket syncPacket:
						OnSynchronizationPacketReceived?.Invoke(this, syncPacket);
						break;
					case UniverseDiscoveryPacket discoveryPacket:
						OnUniverseDiscoveryPacketReceived?.Invoke(this, discoveryPacket);
						break;
				}

				buffer = buffer.Slice(0, packet.Length);

				reader.AdvanceTo(buffer.Start, buffer.End);

				if (result.IsCompleted)
				{
					break;
				}
			}
			reader.Complete();
		}

		public void Dispose()
		{
			CancellationTokenSource.Cancel();
			Socket?.Dispose();
		}

		private ISacnMulticastAddressProvider IPv4MulticastAddressProvider { get; }
		private ISacnMulticastAddressProvider IPv6MulticastAddressProvider { get; }



		public void JoinMulticastGroups(IEnumerable<UInt16> universes, bool ipv6 = false)
		{
			foreach (var universe in universes)
			{
				JoinMulticastGroup(universe, ipv6);
			}
		}

		public void JoinMulticastGroup(UInt16 universe, bool ipv6 = false)
		{
			if (universe < Constants.Universe_MinValue || universe > Constants.Universe_MaxValue)
			{
				throw new ArgumentOutOfRangeException($"Universe must be between {Constants.Universe_MinValue} and {Constants.Universe_MaxValue} inclusive");
			}

			if (ipv6)
			{
				var multicastAddress = IPv6MulticastAddressProvider.GetMulticastAddress(universe);
				var level = SocketOptionLevel.IPv6;
				var option = new IPv6MulticastOption(multicastAddress);
				Socket.SetSocketOption(level, SocketOptionName.AddMembership, option);
			}
			else
			{
				var multicastAddress = IPv4MulticastAddressProvider.GetMulticastAddress(universe);
				var level = SocketOptionLevel.IP;
				var option = new MulticastOption(multicastAddress, this.HostEndPoint.Address);
				Socket.SetSocketOption(level, SocketOptionName.AddMembership, option);
			}
		}

		public void DropMulticastGroups(IEnumerable<ushort> universes, bool ipv6 = false)
		{
			foreach (var universe in universes)
			{
				DropMulticastGroup(universe, ipv6);
			}
		}

		public void DropMulticastGroup(ushort universe, bool ipv6 = false)
		{
			if (universe < Constants.Universe_MinValue || universe > Constants.Universe_MaxValue)
			{
				throw new ArgumentOutOfRangeException($"Universe must be between {Constants.Universe_MinValue} and {Constants.Universe_MaxValue} inclusive");
			}

			if (ipv6)
			{
				var multicastAddress = IPv6MulticastAddressProvider.GetMulticastAddress(universe);
				var level = SocketOptionLevel.IPv6;
				var option = new IPv6MulticastOption(multicastAddress);
				Socket.SetSocketOption(level, SocketOptionName.DropMembership, option);
			}
			else
			{
				var multicastAddress = IPv4MulticastAddressProvider.GetMulticastAddress(universe);
				var level = SocketOptionLevel.IP;
				var option = new MulticastOption(multicastAddress, this.HostEndPoint.Address);
				Socket.SetSocketOption(level, SocketOptionName.DropMembership, option);
			}

		}
	}

}
