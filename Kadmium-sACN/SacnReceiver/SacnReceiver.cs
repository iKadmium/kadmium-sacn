using Kadmium_Udp;
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

namespace Kadmium_sACN.SacnReceiver
{
	public abstract class SacnReceiver : IDisposable, ISacnReceiver
	{
		protected IUdpPipeline UdpPipeline { get; }

		public event EventHandler<DataPacket> OnDataPacketReceived;
		public event EventHandler<SynchronizationPacket> OnSynchronizationPacketReceived;
		public event EventHandler<UniverseDiscoveryPacket> OnUniverseDiscoveryPacketReceived;

		public IPEndPoint HostEndPoint => UdpPipeline.LocalEndPoint;

		private CancellationTokenSource CancellationTokenSource { get; }

		private Task ProcessTask { get; set; }
		private Task ListenTask { get; set; }

		protected SacnReceiver(IUdpPipeline pipeline)
		{
			UdpPipeline = pipeline;
			CancellationTokenSource = new CancellationTokenSource();
		}

		public abstract void Listen(IPAddress address);

		protected void ListenInternal(IPAddress address)
		{
			var endpoint = new IPEndPoint(address, Constants.Port);

			Pipe pipe = new Pipe();
			ListenTask = UdpPipeline.ListenAsync(pipe.Writer, endpoint);
			ProcessTask = ProcessPackets(pipe.Reader);
		}

		private async Task ProcessPackets(PipeReader reader)
		{
			var token = CancellationTokenSource.Token;
			while (!token.IsCancellationRequested)
			{
				var result = await reader.ReadAsync(token);

				if (result.Buffer.IsSingleSegment)
				{
					var packet = SacnPacket.Parse(result.Buffer.FirstSpan);
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
				}
			}
			reader.Complete();
		}

		public void Dispose()
		{
			CancellationTokenSource.Cancel();
			UdpPipeline?.Dispose();
		}
	}
}
