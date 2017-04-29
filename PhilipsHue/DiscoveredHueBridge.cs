using Newtonsoft.Json;

namespace Softopoulos.Crestron.PhilipsHue
{
	/// <summary>
	/// Contains information about a Hue Bridge discovered via a call to <see cref="BridgeClient.DiscoverBridges"/>
	/// </summary>
	public class DiscoveredHueBridge
	{
		internal DiscoveredHueBridge()
		{

		}

		[JsonProperty("id")]
		public string Id { get; internal set; }
		[JsonProperty("internalipaddress")]
		public string LanIpAddress { get; internal set; }
		[JsonProperty("macaddress")]
		// ReSharper disable once UnassignedGetOnlyAutoProperty
		public string MacAddress { get; private set; }
		[JsonProperty("name")]
		public string Name { get; internal set; }
	}
}