# kadmium-sacn
An sACN library for .NET Standard 2.0

[![codecov](https://codecov.io/gh/iKadmium/kadmium-sacn/branch/master/graph/badge.svg?token=ZSK119NBC5)](https://codecov.io/gh/iKadmium/kadmium-sacn)
![build status](https://github.com/iKadmium/kadmium-sacn/actions/workflows/publish.yml/badge.svg)

## Installation
```
dotnet add package kadmium-sacn
```

## Usage
### Sending sACN packets
```c#
byte[] cid = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
UInt16 universe = 1;
string sourceName = "My lovely source";
var factory = new SacnPacketFactory(cid, sourceName);
using var sacnSender = new MulticastSacnSenderIPV4(); // IPv6 is also supported

byte[] values = new byte[512] { 0, 1, 2, 3, 4, 5 };
var packet = factory.CreateDataPacket(universe, values);
await sacnSender.Send(packet);
```

It's also good manners to send Universe Discovery packets every 10 seconds.

```c#
using Timer timer = new Timer(10000);
timer.Elapsed += async (sender, e) =>
{
	var packets = factory.CreateUniverseDiscoveryPackets(new UInt16[] { universe });
	foreach (var packet in packets)
	{
		await sacnSender.Send(packet);
	}
};
timer.Start();
```

### Listening for sACN Packets
```c#
using var receiver = new MulticastSacnReceiverIPV4(); // IPv6 is also supported
receiver.OnDataPacketReceived += (sender, packet) =>
{
	Console.WriteLine(packet.FramingLayer.Universe + ": ");
	Console.WriteLine(string.Join(", ", packet.DMPLayer.PropertyValues));
};
receiver.OnSynchronizationPacketReceived += (sender, packet) =>
{
	Console.WriteLine("Sync!");
};
receiver.OnUniverseDiscoveryPacketReceived += (sender, packet) =>
{
	Console.WriteLine("Discovery!");
};
receiver.Listen(IPAddress.Any);
receiver.JoinMulticastGroup(1);
```