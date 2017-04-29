using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Bool = System.Int16;
using Softopoulos.Crestron.Core;
using Softopoulos.Crestron.Core.Diagnostics;

namespace Softopoulos.Crestron.PhilipsHue
{
	/// <summary>
	/// Contains information about a Philips Hue Bridge;  See more information about its usage in <see cref="BridgeClient.BridgeConfiguration"/>.
	/// </summary>
	public class BridgeConfiguration : HueObject
	{
		internal const string Api = "config";

		internal override string ApiName
		{
			get { return Api; }
		}

		private bool _linkButton;
		private bool _dhcp;
		private bool _portalServices;
		private bool _factoryNew;

		private BridgeConfiguration()
		{
		}

		#region Property Updating

		protected override Dictionary<string, Action<object>> GetHuePropertySetters()
		{
			return new Dictionary<string, Action<object>>();
		}

		protected override Dictionary<string, FieldGetterSetterPair> GetFieldGetterSetterPairs()
		{
			return new Dictionary<string, FieldGetterSetterPair>()
			{
				{
					"Name", new FieldGetterSetterPair<string>()
					{
						Getter = () => Name,
						Setter = newValue => Name = newValue
					}
				},
				{
					"SoftwareUpdate", new FieldGetterSetterPair<SoftwareUpdate>()
					{
						Getter = () => SoftwareUpdate,
						Setter = newValue => SoftwareUpdate = newValue
					}
				},
				{
					"ApiVersion", new FieldGetterSetterPair<string>()
					{
						Getter = () => ApiVersion,
						Setter = newValue => ApiVersion = newValue
					}
				},
				{
					"SoftwareVersion", new FieldGetterSetterPair<string>()
					{
						Getter = () => SoftwareVersion,
						Setter = newValue => SoftwareVersion = newValue
					}
				},
				{
					"ProxyAddress", new FieldGetterSetterPair<string>()
					{
						Getter = () => ProxyAddress,
						Setter = newValue => ProxyAddress = newValue
					}
				},
				{
					"ProxyPort", new FieldGetterSetterPair<ushort>()
					{
						Getter = () => ProxyPort,
						Setter = newValue => ProxyPort = newValue
					}
				},
				{
					"LinkButton", new FieldGetterSetterPair<Bool>()
					{
						Getter = () => LinkButton,
						Setter = newValue => LinkButton = newValue
					}
				},
				{
					"IpAddress", new FieldGetterSetterPair<string>()
					{
						Getter = () => IpAddress,
						Setter = newValue => IpAddress = newValue
					}
				},
				{
					"MacAddress", new FieldGetterSetterPair<string>()
					{
						Getter = () => MacAddress,
						Setter = newValue => MacAddress = newValue
					}
				},
				{
					"NetMask", new FieldGetterSetterPair<string>()
					{
						Getter = () => NetMask,
						Setter = newValue => NetMask = newValue
					}
				},
				{
					"Gateway", new FieldGetterSetterPair<string>()
					{
						Getter = () => Gateway,
						Setter = newValue => Gateway = newValue
					}
				},
				{
					"Dhcp", new FieldGetterSetterPair<Bool>()
					{
						Getter = () => Dhcp,
						Setter = newValue => Dhcp = newValue
					}
				},
				{
					"PortalConnection", new FieldGetterSetterPair<string>()
					{
						Getter = () => PortalConnection,
						Setter = newValue => PortalConnection = newValue
					}
				},
				{
					"PortalState", new FieldGetterSetterPair<PortalState>()
					{
						Getter = () => PortalState,
						Setter = newValue => PortalState = newValue
					}
				},
				{
					"Utc", new FieldGetterSetterPair<string>()
					{
						Getter = () => Utc,
						Setter = newValue => Utc = newValue
					}
				},
				{
					"LocalTime", new FieldGetterSetterPair<string>()
					{
						Getter = () => LocalTime,
						Setter = newValue => LocalTime = newValue
					}
				},
				{
					"TimeZone", new FieldGetterSetterPair<string>()
					{
						Getter = () => TimeZone,
						Setter = newValue => TimeZone = newValue
					}
				},
				{
					"ZigbeeChannel", new FieldGetterSetterPair<ushort>()
					{
						Getter = () => ZigbeeChannel,
						Setter = newValue => ZigbeeChannel = newValue
					}
				},
				{
					"ModelId", new FieldGetterSetterPair<string>()
					{
						Getter = () => ModelId,
						Setter = newValue => ModelId = newValue
					}
				},
				{
					"BridgeId", new FieldGetterSetterPair<string>()
					{
						Getter = () => BridgeId,
						Setter = newValue => BridgeId = newValue
					}
				},
				{
					"FactoryNew", new FieldGetterSetterPair<Bool>()
					{
						Getter = () => FactoryNew,
						Setter = newValue => FactoryNew = newValue
					}
				},
				{
					"ReplacesBridgeId", new FieldGetterSetterPair<string>()
					{
						Getter = () => ReplacesBridgeId,
						Setter = newValue => ReplacesBridgeId = newValue
					}
				},			
				//{
				//    "TouchLink", new FieldGetterSetterPair<bool>()
				//    {
				//        Getter = () => TouchLink,
				//        Setter = newValue => TouchLink = newValue
				//    }
				//},			
			};
		}

		internal override bool UpdateFrom(HueObject hueObject)
		{
			BridgeConfiguration bridgeConfiguration = hueObject as BridgeConfiguration;
			if (bridgeConfiguration == null)
				return false;

			bool anyChanged = base.UpdateFrom(hueObject);

			bool whitelistReplaced = false;
			foreach (var kv in bridgeConfiguration.WhiteList)
			{
				if (!WhiteList.ContainsKey(kv.Key))
				{
					if (IsDeserialized)
						Log(DebugLevel.Debug, "Update WhiteList");

					WhiteList = bridgeConfiguration.WhiteList;
					anyChanged = true;
					whitelistReplaced = true;
					break;
				}
			}

			if (whitelistReplaced)
			{
				NotifyPropertyChanged("WhiteList");
			}
			else
			{
				foreach (var kv in bridgeConfiguration.WhiteList)
				{
					WhiteList whitelist;
					if (WhiteList.TryGetValue(kv.Key, out whitelist))
					{
						if (whitelist.UpdateFrom(kv.Value))
							anyChanged = true;
					}
				}
			}

			return anyChanged;
		}

		#endregion Property Updating

		#region Public Properties

		/// <summary>
		/// Name of the bridge.  This is also its uPnP name, so will reflect the actual uPnP name after any conflicts have been resolved.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }

		/// <summary>
		/// Contains information related to software updates. 
		/// </summary>
		[JsonProperty("swupdate")]
		public SoftwareUpdate SoftwareUpdate { get; private set; }

		/// <summary>
		/// A list of whitelisted users.
		/// </summary>
		[JsonProperty("whitelist")]
		public IDictionary<string, WhiteList> WhiteList { get; private set; }

		/// <summary>
		/// The version of the hue API in the format major.minor.patch, for example 1.2.1
		/// </summary>
		[JsonProperty("apiversion")]
		public string ApiVersion { get; private set; }

		/// <summary>
		/// Software version of the bridge.
		/// </summary>
		[JsonProperty("swversion")]
		public string SoftwareVersion { get; private set; }

		/// <summary>
		/// IP Address of the proxy server being used. A value of “none” indicates no proxy.
		/// </summary>
		[JsonProperty("proxyaddress")]
		public string ProxyAddress { get; private set; }

		/// <summary>
		/// Port of the proxy being used by the bridge. If set to 0 then a proxy is not being used.
		/// </summary>
		[JsonProperty("proxyport")]
		public ushort ProxyPort { get; private set; }

		/// <summary>
		/// Indicates whether the link button has been pressed within the last 30 seconds.
		/// </summary>
		[JsonProperty("linkbutton")]
		public Bool LinkButton
		{
			get { return PlatformConverter.ToPlatformBool(_linkButton); }
			private set { _linkButton = PlatformConverter.ToBool(value); }
		}

		/// <summary>
		/// IP address of the bridge.
		/// </summary>
		[JsonProperty("ipaddress")]
		public string IpAddress { get; private set; }

		/// <summary>
		/// MAC address of the bridge.
		/// </summary>
		[JsonProperty("mac")]
		public string MacAddress { get; private set; }

		/// <summary>
		/// Network mask of the bridge.
		/// </summary>
		[JsonProperty("netmask")]
		public string NetMask { get; private set; }

		/// <summary>
		/// Gateway IP address of the bridge. 
		/// </summary>
		[JsonProperty("gateway")]
		public string Gateway { get; private set; }

		/// <summary>
		/// Whether the IP address of the bridge is obtained with DHCP.
		/// </summary>
		[JsonProperty("dhcp")]
		public Bool Dhcp
		{
			get { return PlatformConverter.ToPlatformBool(_dhcp); }
			private set { _dhcp = PlatformConverter.ToBool(value); }
		}

		/// <summary>
		/// This indicates whether the bridge is registered to synchronize data with a portal account. 
		/// </summary>
		[JsonProperty("portalservices")]
		public Bool PortalServices
		{
			get { return PlatformConverter.ToPlatformBool(_portalServices); }
			private set { _portalServices = PlatformConverter.ToBool(value); }
		}

		/// <summary>
		/// Not documented by Philips at this time
		/// </summary>
		[JsonProperty("portalconnection")]
		public string PortalConnection { get; private set; }

		/// <summary>
		/// Current state of the bridge's connection to the Hue portal (where software updates for the Hue bridge/devices come from)
		/// </summary>
		[JsonProperty("portalstate")]
		public PortalState PortalState { get; private set; }

		/// <summary>
		/// Current time stored on the bridge. 
		/// </summary>
		[JsonProperty("UTC")]
		public string Utc { get; private set; }

		/// <summary>
		/// The local time of the bridge. "none" if not available. 
		/// </summary>
		[JsonProperty("localtime")]
		public string LocalTime { get; private set; }

		/// <summary>
		/// Timezone of the bridge as OlsenIDs, like "Europe/Amsterdam" or "none" when not configured
		/// </summary>
		[JsonProperty("timezone")]
		public string TimeZone { get; private set; }

		/// <summary>
		/// The current wireless frequency channel used by the bridge.It can take values of 11, 15, 20,25 or 0 if undefined (factory new). 
		/// </summary>
		[JsonProperty("zigbeechannel")]
		public ushort ZigbeeChannel { get; private set; }

		/// <summary>
		/// This parameter uniquely identifies the hardware model of the bridge (BSB001, BSB002).
		/// </summary>
		[JsonProperty("modelid")]
		public string ModelId { get; private set; }

		/// <summary>
		/// The unique bridge id. This is currently generated from the bridge Ethernet mac address.
		/// </summary>
		[JsonProperty("bridgeid")]
		public string BridgeId { get; private set; }

		/// <summary>
		/// Indicates if bridge settings are factory new.
		/// </summary>
		[JsonProperty("factorynew")]
		public Bool FactoryNew
		{
			get { return PlatformConverter.ToPlatformBool(_factoryNew); }
			private set { _factoryNew = PlatformConverter.ToBool(value); }
		}

		/// <summary>
		/// If a bridge backup file has been restored on this bridge from a bridge with a different bridgeid, it will indicate that bridge id, otherwise it will be null.
		/// </summary>
		[JsonProperty("replacesbridgeid")]
		public string ReplacesBridgeId { get; private set; }

		///// <summary>
		///// Perform a touchlink action if set to true, setting to false is ignored. When set to true a touchlink procedure starts which adds the closest lamp (within range) to the ZigBee network.  You can then search for new lights and lamp will show up in the bridge.
		///// </summary>
		//[JsonProperty("touchlink")]
		//public bool TouchLink { get; private set; }

		#endregion Public Properties
	}
}