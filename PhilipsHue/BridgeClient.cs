using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Softopoulos.Crestron.Core;
using Softopoulos.Crestron.Core.ComponentModel;
using Softopoulos.Crestron.Core.Diagnostics;
using Softopoulos.Crestron.Core.Net;
using Softopoulos.Crestron.Core.Threading;
using Bool = System.Int16;

namespace Softopoulos.Crestron.PhilipsHue
{
	public class BridgeClient : ObservableObject
	{
		#region Statics

		private static Dictionary<string, Color> _colorsByName = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
		{
			{ "AliceBlue", Color.AliceBlue },
			{ "AntiqueWhite", Color.AntiqueWhite },
			{ "Aqua", Color.Aqua },
			{ "Aquamarine", Color.Aquamarine },
			{ "Azure", Color.Azure },
			{ "Beige", Color.Beige },
			{ "Bisque", Color.Bisque },
			{ "Black", Color.Black },
			{ "BlanchedAlmond", Color.BlanchedAlmond },
			{ "Blue", Color.Blue },
			{ "BlueViolet", Color.BlueViolet },
			{ "Brown", Color.Brown },
			{ "BurlyWood", Color.BurlyWood },
			{ "CadetBlue", Color.CadetBlue },
			{ "Chartreuse", Color.Chartreuse },
			{ "Chocolate", Color.Chocolate },
			{ "Coral", Color.Coral },
			{ "CornflowerBlue", Color.CornflowerBlue },
			{ "Cornsilk", Color.Cornsilk },
			{ "Crimson", Color.Crimson },
			{ "Cyan", Color.Cyan },
			{ "DarkBlue", Color.DarkBlue },
			{ "DarkCyan", Color.DarkCyan },
			{ "DarkGoldenrod", Color.DarkGoldenrod },
			{ "DarkGray", Color.DarkGray },
			{ "DarkGreen", Color.DarkGreen },
			{ "DarkKhaki", Color.DarkKhaki },
			{ "DarkMagenta", Color.DarkMagenta },
			{ "DarkOliveGreen", Color.DarkOliveGreen },
			{ "DarkOrange", Color.DarkOrange },
			{ "DarkOrchid", Color.DarkOrchid },
			{ "DarkRed", Color.DarkRed },
			{ "DarkSalmon", Color.DarkSalmon },
			{ "DarkSeaGreen", Color.DarkSeaGreen },
			{ "DarkSlateBlue", Color.DarkSlateBlue },
			{ "DarkSlateGray", Color.DarkSlateGray },
			{ "DarkTurquoise", Color.DarkTurquoise },
			{ "DarkViolet", Color.DarkViolet },
			{ "DeepPink", Color.DeepPink },
			{ "DeepSkyBlue", Color.DeepSkyBlue },
			{ "DimGray", Color.DimGray },
			{ "DodgerBlue", Color.DodgerBlue },
			{ "Firebrick", Color.Firebrick },
			{ "FloralWhite", Color.FloralWhite },
			{ "ForestGreen", Color.ForestGreen },
			{ "Fuchsia", Color.Fuchsia },
			{ "Gainsboro", Color.Gainsboro },
			{ "GhostWhite", Color.GhostWhite },
			{ "Gold", Color.Gold },
			{ "Goldenrod", Color.Goldenrod },
			{ "Gray", Color.Gray },
			{ "Green", Color.Green },
			{ "GreenYellow", Color.GreenYellow },
			{ "Honeydew", Color.Honeydew },
			{ "HotPink", Color.HotPink },
			{ "IndianRed", Color.IndianRed },
			{ "Indigo", Color.Indigo },
			{ "Ivory", Color.Ivory },
			{ "Khaki", Color.Khaki },
			{ "Lavender", Color.Lavender },
			{ "LavenderBlush", Color.LavenderBlush },
			{ "LawnGreen", Color.LawnGreen },
			{ "LemonChiffon", Color.LemonChiffon },
			{ "LightBlue", Color.LightBlue },
			{ "LightCoral", Color.LightCoral },
			{ "LightCyan", Color.LightCyan },
			{ "LightGoldenrodYellow", Color.LightGoldenrodYellow },
			{ "LightGray", Color.LightGray },
			{ "LightGreen", Color.LightGreen },
			{ "LightPink", Color.LightPink },
			{ "LightSalmon", Color.LightSalmon },
			{ "LightSeaGreen", Color.LightSeaGreen },
			{ "LightSkyBlue", Color.LightSkyBlue },
			{ "LightSlateGray", Color.LightSlateGray },
			{ "LightSteelBlue", Color.LightSteelBlue },
			{ "LightYellow", Color.LightYellow },
			{ "Lime", Color.Lime },
			{ "LimeGreen", Color.LimeGreen },
			{ "Linen", Color.Linen },
			{ "Magenta", Color.Magenta },
			{ "Maroon", Color.Maroon },
			{ "MediumAquamarine", Color.MediumAquamarine },
			{ "MediumBlue", Color.MediumBlue },
			{ "MediumOrchid", Color.MediumOrchid },
			{ "MediumPurple", Color.MediumPurple },
			{ "MediumSeaGreen", Color.MediumSeaGreen },
			{ "MediumSlateBlue", Color.MediumSlateBlue },
			{ "MediumSpringGreen", Color.MediumSpringGreen },
			{ "MediumTurquoise", Color.MediumTurquoise },
			{ "MediumVioletRed", Color.MediumVioletRed },
			{ "MidnightBlue", Color.MidnightBlue },
			{ "MintCream", Color.MintCream },
			{ "MistyRose", Color.MistyRose },
			{ "Moccasin", Color.Moccasin },
			{ "NavajoWhite", Color.NavajoWhite },
			{ "Navy", Color.Navy },
			{ "OldLace", Color.OldLace },
			{ "Olive", Color.Olive },
			{ "OliveDrab", Color.OliveDrab },
			{ "Orange", Color.Orange },
			{ "OrangeRed", Color.OrangeRed },
			{ "Orchid", Color.Orchid },
			{ "PaleGoldenrod", Color.PaleGoldenrod },
			{ "PaleGreen", Color.PaleGreen },
			{ "PaleTurquoise", Color.PaleTurquoise },
			{ "PaleVioletRed", Color.PaleVioletRed },
			{ "PapayaWhip", Color.PapayaWhip },
			{ "PeachPuff", Color.PeachPuff },
			{ "Peru", Color.Peru },
			{ "Pink", Color.Pink },
			{ "Plum", Color.Plum },
			{ "PowderBlue", Color.PowderBlue },
			{ "Purple", Color.Purple },
			{ "Red", Color.Red },
			{ "RosyBrown", Color.RosyBrown },
			{ "RoyalBlue", Color.RoyalBlue },
			{ "SaddleBrown", Color.SaddleBrown },
			{ "Salmon", Color.Salmon },
			{ "SandyBrown", Color.SandyBrown },
			{ "SeaGreen", Color.SeaGreen },
			{ "SeaShell", Color.SeaShell },
			{ "Sienna", Color.Sienna },
			{ "Silver", Color.Silver },
			{ "SkyBlue", Color.SkyBlue },
			{ "SlateBlue", Color.SlateBlue },
			{ "SlateGray", Color.SlateGray },
			{ "Snow", Color.Snow },
			{ "SpringGreen", Color.SpringGreen },
			{ "SteelBlue", Color.SteelBlue },
			{ "Tan", Color.Tan },
			{ "Teal", Color.Teal },
			{ "Thistle", Color.Thistle },
			{ "Tomato", Color.Tomato },
			{ "Transparent", Color.Transparent },
			{ "Turquoise", Color.Turquoise },
			{ "Violet", Color.Violet },
			{ "Wheat", Color.Wheat },
			{ "White", Color.White },
			{ "WhiteSmoke", Color.WhiteSmoke },
			{ "Yellow", Color.Yellow },
			{ "YellowGreen", Color.YellowGreen },
		};

		public const ushort MinLightBulbBrightness = 1;
		public const ushort MaxLightBulbBrightness = 254;

		public const ushort MinLightBulbHue = 0;
		public const ushort MaxLightBulbHue = 65535;

		public const ushort MinLightBulbSaturation = 1;
		public const ushort MaxLightBulbSaturation = 254;

		public const ushort MinLightBulbColorTemperature = 153;
		public const ushort MaxLightBulbColorTemperature = 500;

		/// <summary>
		/// Lock to protect initializing/uninitializing class members
		/// (the <see cref="_refreshLock"/> will be used to protect the Hue collections and Hue objects)
		/// </summary>
		private static CCriticalSection StaticLockObject = new CCriticalSection();

		private static Dictionary<string, BridgeClient> InitializedClientsByIpAddress = new Dictionary<string, BridgeClient>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Retrieves a client previous initialized via the <see cref="Initialize"/> method;
		/// This is needed for multiple SIMPL+ wrappers to share the same client.
		/// </summary>
		public static Bool TryGetExistingClient(string ipAddressOrHostName, ref BridgeClient client)
		{
			using (new LockScope(StaticLockObject))
			{
				if (InitializedClientsByIpAddress.TryGetValue(ipAddressOrHostName, out client))
					return PlatformConverter.ToPlatformBool(true);
			}

			return PlatformConverter.ToPlatformBool(false);
		}

		#endregion Statics

		#region Fields

		private DebugLevel _debugLevel = DebugLevel.None;

		private bool _isBridgeAvailable;
		private bool _isAuthenticated;

		private string _ipAddressOrHostName;
		private string _username;

		/// <summary>
		/// Lock to protect initializing/uninitializing class members
		/// (the <see cref="_refreshLock"/> will be used to protect the Hue collections and Hue objects)
		/// </summary>
		private CCriticalSection _classLock = new CCriticalSection();

		private CCriticalSection _raiseErrorLock = new CCriticalSection();

		/// <summary>
		/// Lock used to protect te Hue object collections and Hue objects when they are changing
		/// </summary>
		private CCriticalSection _refreshLock = new CCriticalSection();

		private CTimer _refreshTimer;

		private int _refreshInterval;
		private RefreshIntervalParts _refreshIntervalParts = RefreshIntervalParts.Lights | RefreshIntervalParts.Groups | RefreshIntervalParts.Scenes;

		private BridgeConfiguration _bridgeConfiguration;
		private ushort _bridgeRefreshTime = 3 * 60; // 3 am
		private DateTime? _lastBridgeRefreshTime;
		private CTimer _softwareUpdatesTimer;

		// Light members
		private short _lightBulbsDefaultTransitionTime = -1;
		private List<LightBulb> _lightBulbs;
		private ReadOnlyCollection<LightBulb> _readOnlyLightBulbsCollection;
		private Dictionary<string, LightBulb> _lightBulbsById;
		private Dictionary<string, LightBulb> _lightBulbsByUniqueId;
		private LightBulbsSortOrder _lightBulbsSortOrder = LightBulbsSortOrder.ByName;
		private CTimer _searchForNewLightBulbsTimer;

		// Group members
		private List<Group> _groups;
		private Dictionary<string, Group> _groupsById;

		// Scene members
		private List<Scene> _scenes;
		private ReadOnlyCollection<Scene> _readOnlyScenesCollection;
		private Dictionary<string, Scene> _scenesById;

		#endregion

		public BridgeClient()
		{
		}

		#region Events

		/// <summary>
		/// Raised when an error occurs during the current method or property setter call
		/// <para>
		/// This even can occur from different threads when the automatic refresh feature (<see cref="RefreshInterval"/>)
		/// is being used (or if the current application is making calls into this class from different threads).
		/// </para>
		/// </summary>
		public event ErrorOccurredEventHandler ErrorOccurred;

		public event CheckForSoftwareUpdatesCompletedEventHandler CheckForSoftwareUpdatesCompleted;
		public event ApplySoftwareUpdatesCompletedEventHandler ApplySoftwareUpdatesCompleted;
		public event FoundLightBulbEventHandler FoundLightBulb;
		public event SearchForNewLightBulbsCompletedEventHandler SearchForNewLightBulbsCompleted;

		/// <summary>
		/// Occurs when one or more properties on the <see cref="BridgeConfiguration"/> object changes (or any of its child objects)
		/// <para>
		/// This will occur during the call to <see cref="Initialize"/>, <see cref="Refresh"/>, <see cref="RefreshBridgeConfiguration"/>
		/// or during any automatic refresh started by setting the <see cref="RefreshInterval"/>
		/// </para>
		/// </summary>
		public event BridgeConfigurationPropertiesChangedEventHandler BridgeConfigurationPropertiesChanged;

		/// <summary>
		/// Occurs when one or more properties on a <see cref="LightBulb"/> has changed
		/// <para>
		/// This will occur during the call to <see cref="Initialize"/>, <see cref="Refresh"/>, <see cref="RefreshLightBulbs"/>
		/// or during any automatic refresh started by setting the <see cref="RefreshInterval"/>
		/// This will occur for new light bulbs just discovered as well.
		/// </para>
		/// </summary>
		public event LightBulbPropertiesChangedEventHandler LightBulbPropertiesChanged;

		public event ScenePropertiesChangedEventHandler ScenePropertiesChanged;

		#endregion

		#region Error Handling

		private void Execute(Action action)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				RaiseError(ex);
			}
		}
		private T Execute<T>(Func<T> func)
		{
			return Execute(func, default(T));
		}

		private T Execute<T>(Func<T> func, T failedReturnValue)
		{
			try
			{
				return func();
			}
			catch (Exception ex)
			{
				RaiseError(ex);
				return failedReturnValue;
			}
		}

		private void RaiseError(Exception ex)
		{
			Log(ex);
			RaiseError(new Error() { Description = ex.Message });
		}

		private void RaiseError(string error)
		{
			RaiseError(new Error() { Description = error });
		}

		private void RaiseError(Error error)
		{
			LogError(error);

			bool isLastErrorUserError = false;
			if (error.Type != 0)
			{
				switch (error.Type)
				{
					case 101:
						isLastErrorUserError = true;
						error.Description = "The button on the Hue Bridge was not pressed within 30 seconds of attempting authentication.";
						break;
				}
			}

			if (ErrorOccurred != null)
			{
				ErrorOccurred(this, new ErrorOccurredEventArgs(error.Description, isLastErrorUserError));
			}
		}

		#endregion

		#region Logging / Debug

		/// <summary>
		/// Debug/logging level of this class
		/// </summary>
		public DebugLevel DebugLevel
		{
			get { return _debugLevel; }
			set { UpdateProperty("DebugLevel", ref _debugLevel, value); }
		}

		public ushort DebugLevelValue
		{
			get { return (ushort)DebugLevel; }
			set { DebugLevel = (DebugLevel)value; }
		}

		private void Log(DebugLevel debugLevel, string message, params object[] parameters)
		{
			if (debugLevel == DebugLevel.None)
				return;

			if (debugLevel > DebugLevel)
				return;

			if (parameters != null && parameters.Length > 0)
				message = string.Format(message, parameters);
			int index = 0;
			while (index < message.Length)
			{
				string line = message.Substring(index, Math.Min(80, message.Length - index));
				CrestronConsole.PrintLine(line);
				index += line.Length;
			}
		}

		private void LogError(string message, params object[] parameters)
		{
			if (parameters != null && parameters.Length > 0)
				message = string.Format(message, parameters);

			LogError(new Error() { Description = message });
		}

		private void LogError(Error error)
		{
			if (error.Type != 0)
			{
				if (string.IsNullOrEmpty(error.Address))
					Log(DebugLevel.Debug, "{0}: {1}", error.Type, error.Description);
				else
					Log(DebugLevel.Debug, "{0}: {1}: {2}", error.Type, error.Address, error.Description);
			}

			string message = "Error: " + error.Description;
			CrestronConsole.PrintLine(message);
		}

		private void Log(Exception ex)
		{
			CrestronConsole.PrintLine(ex.ToString());
		}

		#endregion

		#region Discovery

		/// <summary>
		/// After calling <see cref="CheckBridgeAvailability"/> on application startup,
		/// this will be true if the bridge was available at the specified IP address
		/// (which was previously determined by a call to <see cref="DiscoverBridges"/>)
		/// </summary>
		public Bool IsBridgeAvailable
		{
			get { return PlatformConverter.ToPlatformBool(_isBridgeAvailable); }
			private set { UpdateProperty("IsBridgeAvailable", ref _isBridgeAvailable, PlatformConverter.ToBool(value)); }
		}

		/// <summary>
		/// Checks if the bridge is available at the specified IP address
		/// (which presumably was determined the first time this application was run
		/// by calling to <see cref="DiscoverBridges"/>)
		/// <para>
		/// If the bridge is available the <see cref="IsBridgeAvailable"/> property will be set to true.
		/// </para>
		/// </summary>
		/// <returns>true if the bridge was available at the specified IP address</returns>
		public Bool CheckBridgeAvailability(string ipAddress)
		{
			if (string.IsNullOrEmpty(ipAddress))
			{
				RaiseError("CheckBridgeAvailability: ipAddress parameter cannot be null or empty");
				return PlatformConverter.ToPlatformBool(false);
			}

			Execute(() =>
			{
				Log(DebugLevel.Info, "Check bridge availability at: {0}", ipAddress);

				DiscoveredHueBridge bridge = GetDiscoveredBridge("http://" + ipAddress + "/description.xml");
				IsBridgeAvailable = PlatformConverter.ToPlatformBool(bridge != null);

				Log(DebugLevel.Normal, "Bridge availability is {0}", PlatformConverter.ToBool(IsBridgeAvailable) ? "true" : "false");
			});

			return IsBridgeAvailable;
		}

		private DiscoveredHueBridge[] _discoveredBridges;

		/// <summary>
		/// Gets the number of bridges discovered after call <see cref="DiscoverBridges"/>
		/// </summary>
		public short DiscoveredBridgesCount { get { return (short)(_discoveredBridges != null ? _discoveredBridges.Length : 0); } }

		/// <summary>
		/// Gets information about the discovered bridge at the specified index;
		/// The method <see cref="DiscoverBridges"/> must be called first and then the property
		/// <see cref="DiscoveredBridgesCount"/> can be used to determine how many bridges were discovered.
		/// </summary>
		public void GetDiscoveredBridge(int index, ref DiscoveredHueBridge bridge)
		{
			if (_discoveredBridges == null)
			{
				RaiseError("DiscoveredBridges not called");
				bridge = null;
			}
			else if (index < 0 || index >= _discoveredBridges.Length)
			{
				RaiseError("Discovered bridge index out of range: " + index);
				bridge = null;
			}
			else
			{
				bridge = _discoveredBridges[index];
			}
		}

		/// <summary>
		/// Discovers Hue Bridges on the current network;  This is a synchronous call and may take up to 10 seconds.
		/// <para>
		/// This will return the number of bridges discovered and set that number to the <see cref="DiscoveredBridgesCount"/> property.
		/// Use the <see cref="GetDiscoveredBridge"/> method to retrieve information about each discovered bridge.
		/// </para>
		/// </summary>
		public short DiscoverBridges()
		{
			DiscoveredHueBridge[] discoveredBridges = null;
			Execute(() =>
			{
				Log(DebugLevel.Info, "Discover bridges");

				discoveredBridges = ExecuteDiscoverBridges();
				_discoveredBridges = discoveredBridges;
				Log(DebugLevel.Normal, "Discovered bridges: {0}", discoveredBridges != null ? discoveredBridges.Length : 0);
			});

			return DiscoveredBridgesCount;
		}

		private DiscoveredHueBridge[] ExecuteDiscoverBridges()
		{
			// Perform the UPNP and NUPNP requests simultaneously
			string upnpResponse = null;

			Action upnpRequestAction = () =>
			{
				Execute(() =>
				{
					var upnpRequest = new StringBuilder();
					upnpRequest.AppendLine("M-SEARCH * HTTP/1.1");
					upnpRequest.AppendLine("HOST: 239.255.255.250:1900");
					upnpRequest.AppendLine("MAN: ssdp:discover");
					upnpRequest.AppendLine("MX: 5");
					upnpRequest.AppendLine("ST: ssdp:all");

					byte[] sendBytes = Encoding.ASCII.GetBytes(upnpRequest.ToString());
					Log(DebugLevel.Normal, "Broadcast bridge discovery message");
					UDPServer udpServer = new UDPServer();
					udpServer.SendData(sendBytes, sendBytes.Length, IPAddress.Broadcast.ToString(), 1900);
					udpServer.SocketSendOrReceiveTimeOutInMs = 5000;
					Log(DebugLevel.Debug, "Wait to receive data");
					udpServer.ReceiveData();
					Log(DebugLevel.Debug, "Received response length: {0}", udpServer.IncomingDataBuffer.Length);
					if (udpServer.IncomingDataBuffer.Length > 0)
					{
						upnpResponse = Encoding.ASCII.GetString(udpServer.IncomingDataBuffer, 0, udpServer.IncomingDataBuffer.Length);
					}

					//upnpRequestComplete = true;
				});
			};

			//upnpRequestAction();

			const string nupnpUrl = "https://www.meethue.com/api/nupnp";
			string nupnpResponse = null;
			Log(DebugLevel.Normal, "Retrieve bridge information from Hue website");
			nupnpResponse = Http.Get(nupnpUrl);

			// Parse the UPNP response
			if (!string.IsNullOrEmpty(upnpResponse))
			{
				Log(DebugLevel.Debug, "Parse response from bridge discovery");
				Log(DebugLevel.Verbose, "Response from upnp: {0}", upnpResponse);

				//HTTP/1.1 200 OK
				//HOST: 239.255.255.250:1900
				//EXT:
				//CACHE-CONTROL: max-age=100
				//LOCATION: http://192.168.1.111:80/description.xml
				//SERVER: Linux/3.14.0 UPnP/1.0 IpBridge/1.14.0
				//hue-bridgeid: 001788FFFE20EE37
				//ST: upnp:rootdevice
				//USN: uuid:2f402f80-da50-11e1-9b23-00178820ee37::upnp:rootdevice

				int index = upnpResponse.IndexOf("LOCATION:", StringComparison.OrdinalIgnoreCase);
				if (index >= 0)
				{
					index += "LOCATION:".Length;
					int endIndex = upnpResponse.IndexOf("description.xml", index, StringComparison.OrdinalIgnoreCase);
					if (endIndex >= 0)
					{
						string url = upnpResponse.Substring(index, endIndex + "description.xml".Length - index).Trim();
						Log(DebugLevel.Info, "Found bridge URL: {0}", url);
						DiscoveredHueBridge bridge = GetDiscoveredBridge(url);
						if (bridge != null)
							return new[] { bridge };
					}
				}
			}

			Log(DebugLevel.Verbose, "Response from nupnp: {0}", nupnpResponse);

			DiscoveredHueBridge[] discoveredHueBridges = null;
			if (!string.IsNullOrEmpty(nupnpResponse))
			{
				discoveredHueBridges = JsonConvert.DeserializeObject<DiscoveredHueBridge[]>(nupnpResponse, JsonSerializerSettings);
				foreach (DiscoveredHueBridge hueBridge in discoveredHueBridges)
				{
					if (string.IsNullOrEmpty(hueBridge.Name))
						hueBridge.Name = "Philips Hue Bridge";
				}
			}

			return discoveredHueBridges;
		}

		private DiscoveredHueBridge GetDiscoveredBridge(string urlToDescriptionXml)
		{
			Log(DebugLevel.Normal, "Retrieve discovered bridge information");
			string descriptionXml = Http.Get(urlToDescriptionXml);
			Log(DebugLevel.Verbose, "Description.xml: {0}", descriptionXml);
			string modelName = ReadXmlValue(descriptionXml, "modelName");
			if (modelName.IndexOf("Philips hue bridge", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				string url = ReadXmlValue(descriptionXml, "URLBase");
				if (!string.IsNullOrEmpty(url))
				{
					url = url.Replace(":80", "").Replace("http://", "");
					url = url.TrimEnd('/');
					//string id = ReadXmlValue(descriptionXml, "serialNumber");
					//if (!string.IsNullOrEmpty(id))
					{
						Log(DebugLevel.Debug, "Discovered bridge URL: {0}", url);
						var bridge = new DiscoveredHueBridge();
						bridge.LanIpAddress = url;
						//bridge.Id = id;
						bridge.Name = modelName;
						{
							return bridge;
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Reads XML as a string (for compatibility reasons with other platforms this code is compiled against)
		/// </summary>
		private static string ReadXmlValue(string xml, string tag)
		{
			int index;
			int endIndex;
			string startTag = "<" + tag + ">";
			string endTag = "</" + tag + ">";
			index = xml.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
			if (index >= 0)
			{
				index += startTag.Length;
				endIndex = xml.IndexOf(endTag, index, StringComparison.OrdinalIgnoreCase);
				if (endIndex > index)
				{
					return xml.Substring(index, endIndex - index);
				}
			}

			return null;
		}

		#endregion Discovery

		#region Authentication

		/// <summary>
		/// True if <see cref="Initialize"/> was called and the specified username is already authenticated for the bridge at the specified IP address;
		/// If the user was not authenticated for that bridge, the <see cref="Authenticate"/> method should be called before using any other
		/// functionality in this class.
		/// </summary>
		public Bool IsAuthenticated
		{
			get { return PlatformConverter.ToPlatformBool(_isAuthenticated); }
			private set { UpdateProperty("IsAuthenticated", ref _isAuthenticated, PlatformConverter.ToBool(value)); }
		}

		/// <summary>
		/// Authenticates a new user so this class can communicate with the Hue Bridge.
		/// The <see cref="Initialize"/> method should be called with the username returned
		/// by this method in order for other methods in this class to being working.
		/// </summary>
		/// <returns>
		/// The username to be stored by the current application in order to avoid having to 
		/// authenticate again.  The next time the current application starts, it can call
		/// the <see cref="Initialize"/> method with this username.
		/// </returns>
		public string Authenticate(string ipAddressOrHostName, string applicationName, string deviceName)
		{
			if (string.IsNullOrEmpty(ipAddressOrHostName))
			{
				RaiseError("Authenticate: ipAddressOrHostName parameter cannot be null or empty");
				return null;
			}
			if (string.IsNullOrEmpty(applicationName))
			{
				RaiseError("Authenticate: applicationName parameter cannot be null or empty");
				return null;
			}
			if (string.IsNullOrEmpty(deviceName))
			{
				RaiseError("Authenticate: deviceName parameter cannot be null or empty");
				return null;
			}

			Log(DebugLevel.Info, "Authenticate: {0}; {1}; {2}", ipAddressOrHostName, applicationName, deviceName);
			string username = null;
			Execute(() =>
			{
				using (new LockScope(_refreshLock))
					username = ExecuteAuthenticate(ipAddressOrHostName, applicationName, deviceName);
			});

			Log(DebugLevel.Normal, "Authentication result: {0}", (string.IsNullOrEmpty(username) ? "new user not created" : "new user created"));
			return username;
		}

		private string ExecuteAuthenticate(string ipAddressOrHostName, string applicationName, string deviceName)
		{
			Log(DebugLevel.Debug, "Make authentication URL and body");
			string url = string.Format("http://{0}/api", ipAddressOrHostName);
			Log(DebugLevel.Verbose, "URL: " + url);

			List<object[]> bodyParams = new List<object[]>();
			bodyParams.Add(new object[]
			{
				"devicetype",
				applicationName.Substring(0, Math.Min(20, applicationName.Length)) + "#" +
				deviceName.Substring(0, Math.Min(19, deviceName.Length))
			});
			string body = MakeBody(bodyParams.ToArray());
			Log(DebugLevel.Verbose, "Body: " + body);

			Log(DebugLevel.Debug, "Post authentication request");
			string response = Post(url, body, true);
			Results<Result> results;
			ParseResultsAndReportErrors(response, out results);
			foreach (Result result in results)
			{
				if (result.Error == null && result.Success != null)
				{
					object value;
					if (result.Success.TryGetValue("username", out value))
					{
						return (string)value;
					}
				}
			}

			return "";
		}

		#endregion

		#region Initialization

		/// <summary>
		/// IP address specified when <see cref="Initialize"/> method was called
		/// </summary>
		public string IpAddressOrHostName
		{
			get { return _ipAddressOrHostName; }
			private set { UpdateProperty("IpAddressOrHostName", ref _ipAddressOrHostName, value); }
		}

		/// <summary>
		/// Username specified when <see cref="Initialize"/> method was called
		/// </summary>
		public string Username
		{
			get { return _username; }
			private set { UpdateProperty("Username", ref _username, value); }
		}

		/// <summary>
		/// Initializes this class using an existing user ID previously obtained from the 
		/// <see cref="Authenticate"/> method of this class
		/// <para>
		/// If the <param name="username" /> is authenticated already, the <see cref="IsAuthenticated"/>
		/// property will be set to true and <see cref="Refresh"/> will be called.
		/// </para>
		/// <para>
		/// Otherwise, <see cref="IsAuthenticated"/> will be set to false and nothing else will 
		/// be initialized.  In that case, the <see cref="Authenticate"/> method should be called.
		/// </para>
		/// </summary>
		public void Initialize(string ipAddressOrHostName, string username)
		{
			Execute(() =>
			{
				if (!string.IsNullOrEmpty(IpAddressOrHostName))
				{
					using (new LockScope(StaticLockObject))
					{
						InitializedClientsByIpAddress.Remove(IpAddressOrHostName);
					}
				}

				using (new LockScope(_classLock))
				using (new LockScope(_refreshLock))
				{

					Log(DebugLevel.Info, "Initialize");
					ExecuteInitialize(ipAddressOrHostName, username);
					Log(DebugLevel.Normal, "Initialized");
				}

				using (new LockScope(StaticLockObject))
				{
					InitializedClientsByIpAddress[ipAddressOrHostName] = this;
				}
			});
		}

		private void ExecuteInitialize(string ipAddressOrHostName, string username)
		{
			IpAddressOrHostName = ipAddressOrHostName;
			Username = username;

			// Retrieve the configuration
			// (doubles as a way to check the authentication)
			ExecuteRefreshBridgeConfiguration();
			IsAuthenticated = PlatformConverter.ToPlatformBool(
				BridgeConfiguration != null &&
				BridgeConfiguration.WhiteList != null &&
				BridgeConfiguration.WhiteList.ContainsKey(Username));
			Log(DebugLevel.Debug, "Initialize: IsAuthenticated={0}", PlatformConverter.ToBool(IsAuthenticated));
			if (PlatformConverter.ToBool(IsAuthenticated))
				ExecuteRefresh(RefreshIntervalParts.Lights | RefreshIntervalParts.Groups | RefreshIntervalParts.Scenes);

			// Restart the refresh timer (if it was set)
			if (_refreshInterval > 0)
				RefreshInterval = _refreshInterval;
		}

		/// <summary>
		/// Resets the state of this class back to before the <see cref="Initialize"/> method was called
		/// </summary>
		public void Uninitialize()
		{
			if (!string.IsNullOrEmpty(IpAddressOrHostName))
			{
				using (new LockScope(StaticLockObject))
				{
					InitializedClientsByIpAddress.Remove(IpAddressOrHostName);
				}
			}

			using (new LockScope(_classLock))
			using (new LockScope(_refreshLock))
			{
				Log(DebugLevel.Info, "Uninitialize");
				Execute(ExecuteUninitialize);
				Log(DebugLevel.Normal, "Uninitialized");
			}
		}

		private void ExecuteUninitialize()
		{
			if (_refreshTimer != null)
			{
				_refreshTimer.Dispose();
				_refreshTimer = null;
			}

			if (_softwareUpdatesTimer != null)
			{
				_softwareUpdatesTimer.Dispose();
				_softwareUpdatesTimer = null;
			}

			if (_searchForNewLightBulbsTimer != null)
			{
				_searchForNewLightBulbsTimer.Dispose();
				_searchForNewLightBulbsTimer = null;
			}

			LightBulbs = null;
			_lightBulbs = null;
			_lightBulbsById = null;
			_lightBulbsByUniqueId = null;

			Groups = null;
			NotifyPropertyChanged("Groups");
			_groups = null;
			_groupsById = null;

			Scenes = null;
			_scenes = null;
			_scenesById = null;

			IsAuthenticated = PlatformConverter.ToPlatformBool(false);
			IsBridgeAvailable = PlatformConverter.ToPlatformBool(false);
			BridgeConfiguration = null;
			IpAddressOrHostName = null;
			Username = null;
		}

		#endregion

		#region Refresh

		/// <summary>
		/// Interval (in seconds) in which information from the Hue Bridge will be polled;  Defaults to 0 (off)
		/// <para>
		/// Also by default, this does not include refreshing information about the bridge itself.
		/// To adjust what is refreshed, use the <see cref="RefreshIntervalParts"/> property.
		/// </para>
		/// </summary>
		public int RefreshInterval
		{
			get { return _refreshInterval; }
			set
			{
				_refreshInterval = value;

				using (new LockScope(_refreshLock))
				{
					if (_lightBulbs == null)
						return;

					if (_refreshInterval > 0)
					{
						if (_refreshTimer == null)
						{
							Log(DebugLevel.Verbose, "Begin automatic refresh timer");
							_refreshTimer = new CTimer(data =>
							{
								if (PlatformConverter.ToBool(IsAuthenticated))
								{
									Log(DebugLevel.Verbose, "Automatic refresh");
									using (new LockScope(_refreshLock))
										Execute(ExecuteRefresh);
								}
							},
							5000 /*dummy value until Reset is called below since CTimer doesn't seem to respect the ctor parameter*/);
						}

						_refreshTimer.Reset(_refreshInterval * 1000, _refreshInterval * 1000);

					}
					else
					{
						if (_refreshTimer != null)
						{
							Log(DebugLevel.Verbose, "Stop automatic refresh timer");
							_refreshTimer.Dispose();
							_refreshTimer = null;
						}
					}
				}
			}
		}

		/// <summary>
		/// Determines what is refreshed when the <see cref="RefreshInterval"/> property has been enabled.
		/// </summary>
		public RefreshIntervalParts RefreshIntervalParts
		{
			get { return _refreshIntervalParts; }
			set { UpdateProperty("RefreshIntervalParts", ref _refreshIntervalParts, value); }
		}

		public ushort RefreshIntervalPartsValue
		{
			get { return (ushort)RefreshIntervalParts; }
			set { RefreshIntervalParts = (RefreshIntervalParts)value; }
		}

		/// <summary>
		/// Refreshes all Hue objects managed by this class; 
		/// Existing object references are maintained and only their property values are refreshed.
		/// </summary>
		public void Refresh()
		{
			if (!PlatformConverter.ToBool(IsAuthenticated))
			{
				RaiseError("Not authenticated");
				return;
			}

			Log(DebugLevel.Info, "Refresh");
			using (new LockScope(_refreshLock))
				Execute(ExecuteRefresh);
			Log(DebugLevel.Normal, "Refreshed");
		}

		private void ExecuteRefresh()
		{
			ExecuteRefresh(_refreshIntervalParts);
		}

		private void ExecuteRefresh(RefreshIntervalParts refreshIntervalParts)
		{
			if ((refreshIntervalParts & RefreshIntervalParts.Bridge) == RefreshIntervalParts.Bridge)
			{
				DateTime now = DateTime.Now;
				if ((_lastBridgeRefreshTime == null || now.Day != _lastBridgeRefreshTime.Value.Day) &&
					now.TimeOfDay.TotalMinutes >= _bridgeRefreshTime)
				{
					Log(DebugLevel.Normal, "Automatically refresh bridge configuration");
					ExecuteRefreshBridgeConfiguration();
					_lastBridgeRefreshTime = now;
				}
			}

			if ((refreshIntervalParts & RefreshIntervalParts.Lights) == RefreshIntervalParts.Lights)
				ExecuteRefreshLightBulbs();
			if ((refreshIntervalParts & RefreshIntervalParts.Groups) == RefreshIntervalParts.Groups)
				RefreshGroups();
			if ((refreshIntervalParts & RefreshIntervalParts.Scenes) == RefreshIntervalParts.Scenes)
				RefreshScenes();
		}

		#endregion

		#region Bridge Configuration

		/// <summary>
		/// Information about the bridge at the specified address when <see cref="Initialize"/> was called;
		/// If user was not authenticated, this will only contain a minimal amount of information about the bridge.
		/// <para>
		/// This information can also be refreshed by called <see cref="RefreshBridgeConfiguration"/>
		/// or automatically using the <see cref="RefreshInterval"/> and the <see cref="RefreshIntervalParts"/> properties.
		/// </para>
		/// </summary>
		public BridgeConfiguration BridgeConfiguration
		{
			get
			{
				using (new LockScope(_refreshLock))
					return _bridgeConfiguration;
			}
			private set { UpdateProperty("BridgeConfiguration", ref _bridgeConfiguration, value); }
		}

		/// <summary>
		/// Refreshes the information in the <see cref="BridgeConfiguration"/> property.
		/// A previous call to <see cref="Initialize"/> should have already been made prior to calling this
		/// in order for the IP address of the bridge to be known by this class.
		/// </summary>
		public void RefreshBridgeConfiguration()
		{
			Log(DebugLevel.Info, "RefreshBridgeConfiguration");
			using (new LockScope(_refreshLock))
			{
				if (_bridgeConfiguration == null)
				{
					RaiseError("Initialize not called");
					return;
				}

				Execute(ExecuteRefreshBridgeConfiguration);
			}
			Log(DebugLevel.Normal, "RefreshBridgeConfiguration: done");
		}

		private void ExecuteRefreshBridgeConfiguration()
		{
			ExecuteRefreshBridgeConfiguration(true);
		}

		private void ExecuteRefreshBridgeConfiguration(bool raiseChangedEvent)
		{
			string response = Get(MakeApi(BridgeConfiguration.Api));
			var bridgeConfiguration = JsonConvert.DeserializeObject<BridgeConfiguration>(response, JsonSerializerSettings);
			if (BridgeConfiguration == null)
			{
				InitializeBridgeConfigurationObject(bridgeConfiguration);
				BridgeConfiguration = bridgeConfiguration;
				if (BridgeConfigurationPropertiesChanged != null)
					BridgeConfigurationPropertiesChanged(this, EventArgs.Empty);
			}
			else
			{
				if (BridgeConfiguration.UpdateFrom(bridgeConfiguration))
				{
					if (raiseChangedEvent)
					{
						if (BridgeConfigurationPropertiesChanged != null)
							BridgeConfigurationPropertiesChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Minutes since midnight, at which the <see cref="BridgeConfiguration"/> property will be refreshed
		/// (requires properties <see cref="RefreshInterval"/> and <see cref="RefreshIntervalParts"/> to also
		/// have been set correctly)
		/// </summary>
		public ushort BridgeRefreshTime
		{
			get { return _bridgeRefreshTime; }
			private set { UpdateProperty("BridgeRefreshTime", ref _bridgeRefreshTime, value); }
		}

		private void InitializeBridgeConfigurationObject(BridgeConfiguration bridgeConfiguration)
		{
		}

		public void BeginCheckForSoftwareUpdates(ref BeginCheckForSoftwareUpdatesResult result)
		{
			Log(DebugLevel.Info, "Begin check for software updates");
			using (new LockScope(_classLock)) // lock to protect timer object initialized in ExecuteBeginCheckForSoftwareUpdates
			using (new LockScope(_refreshLock)) // lock to refresh and protect BridgeConfiguration from changing while we check its properties
			{
				if (_bridgeConfiguration == null)
				{
					RaiseError("Initialize not called");
					result = BeginCheckForSoftwareUpdatesResult.Failed;
					return;
				}

				BeginCheckForSoftwareUpdatesResult localResult;
				localResult = Execute(
					() => ExecuteBeginCheckForSoftwareUpdates(),
					BeginCheckForSoftwareUpdatesResult.Failed);

				Log(DebugLevel.Normal, "Begin check for software updates done: {0}", localResult.ToString());
				result = localResult;
			}
		}

		private BeginCheckForSoftwareUpdatesResult ExecuteBeginCheckForSoftwareUpdates()
		{
			// NOTE: Caller of this private method must lock "this" to protect this thread/timer
			if (_softwareUpdatesTimer != null)
				return BeginCheckForSoftwareUpdatesResult.CheckOrUpdateAlreadyInProgress;

			Log(DebugLevel.Debug, "Get latest bridge info");

			// Get the latest info
			ExecuteRefreshBridgeConfiguration();

			Log(DebugLevel.Debug, "Check software update state");

			// If the update is already determined to be available, we are done
			if (BridgeConfiguration.SoftwareUpdate.UpdateState == SoftwareUpdateState.UpdateAvailableToApply)
				return BeginCheckForSoftwareUpdatesResult.UpdateAlreadyAvailable;

			// Check if someone else is checking for updates (we know it is someone else because our thread/timer was null)
			if (BridgeConfiguration.SoftwareUpdate.CheckForUpdate)
				return BeginCheckForSoftwareUpdatesResult.BridgeIsBusyAndCannotCheckForUpdates;

			// Check for older bridge software that requires the Hue app only to upgrade
			Version bridgeApiVersion = new Version(BridgeConfiguration.ApiVersion);
			if (bridgeApiVersion < new Version(1, 4))
				return BeginCheckForSoftwareUpdatesResult.OldBridgeMustUseOfficialHueApp;

			// Check for a portal connection
			if (!PlatformConverter.ToBool(BridgeConfiguration.PortalState.SignedOn))
				return BeginCheckForSoftwareUpdatesResult.NotConnectedToHuePortal;

			// Check if we are in a state where we can check for upgrades
			if (BridgeConfiguration.SoftwareUpdate.UpdateState == SoftwareUpdateState.UpdateDownloading)
				return BeginCheckForSoftwareUpdatesResult.UpdateAlreadyDownloading;
			if (BridgeConfiguration.SoftwareUpdate.UpdateState != SoftwareUpdateState.NoUpdate)
				return BeginCheckForSoftwareUpdatesResult.BridgeIsBusyAndCannotCheckForUpdates;

			// Tell the bridge to begin checking for updates
			string checkForUpdateResponse = Put(BridgeConfiguration.Api, "{\"swupdate\": {\"checkforupdate\":true}}");
			Results<Result> checkForUpdateResults;
			bool checkForUpdatesSucceeded = ParseResultsAndReportErrors(checkForUpdateResponse, out checkForUpdateResults);
			if (!checkForUpdatesSucceeded)
				return BeginCheckForSoftwareUpdatesResult.Failed;

			BridgeConfiguration.SoftwareUpdate.CheckForUpdate = true;

			// Start a timer/thread to wait for the check to complete
			bool inProgress = false;
			int counter = 0;
			const int interval = 5000;
			_softwareUpdatesTimer = new CTimer(data =>
			{
				if (inProgress) // Prevent overlapping timer calls (mostly during debugging)
					return;

				inProgress = true;
				try
				{
					using (var refreshLockScope = new LockScope(_refreshLock)) // lock to refresh and protect BridgeConfiguration from changing while we check its properties
					{
						if (_softwareUpdatesTimer == null) // A previous timer elapsed already determined we were finished
							return;

						if (!Execute(() =>
						{
							// Check if the "check for updates" is complete
							CheckForSoftwareUpdatesResult? result = ExecuteCompleteCheckForSoftwareUpdate();

							// Check for an approximate 5 minute time out (as suggested by Philips)
							bool timedOut = ++counter * interval >= (5 * 60 * 1000);

							Log(DebugLevel.Verbose, "Check for software updates completion status: {0}",
								result != null ? result.ToString() :
									(timedOut ? "timed out" : "waiting..."));

							if (timedOut ||
								(result != null && result.Value != CheckForSoftwareUpdatesResult.LostConnectingToHuePortal))
							{
								// Ensure our flag for check for update is false (in case we timed out)
								BridgeConfiguration.SoftwareUpdate.CheckForUpdate = false;

								// Raise one changed event since our thread/timer has been refreshing the bridge configuration secretly
								// so the code using this client is updated with the latest information
								if (BridgeConfigurationPropertiesChanged != null)
									BridgeConfigurationPropertiesChanged(this, EventArgs.Empty);
								Log(DebugLevel.Verbose, "BridgeConfigurationPropertiesChanged event returned");

								Log(DebugLevel.Verbose, "Dispose refresh lock");
								// Release the refresh lock (since we are done and to avoid out-of-order locking)
								// ReSharper disable once AccessToDisposedClosure
								refreshLockScope.Dispose();

								Log(DebugLevel.Verbose, "Lock client");
								using (new LockScope(_classLock)) // lock to protect timer object
								{
									Log(DebugLevel.Verbose, "Locked client");
									_softwareUpdatesTimer.Stop();
									// ReSharper disable once AccessToDisposedClosure
									_softwareUpdatesTimer.Dispose();
									_softwareUpdatesTimer = null;
								}

								if (result == null /* && implied timedOut == true*/)
									result = CheckForSoftwareUpdatesResult.TimedOut;
							}

							// Finally, raise the completed event for this async operation
							if (result != null && CheckForSoftwareUpdatesCompleted != null)
							{
								Log(DebugLevel.Verbose, "Raise completed event: {0}", result.Value.ToString());
								CheckForSoftwareUpdatesCompleted(this, new CheckForSoftwareUpdatesCompletedEventArgs(result.Value));
							}

							return true; // Execute succeeded
						}, false))
						{
							// Execute failed.
							// Exception occurred while checking - hopefully this never happens

							// Ensure our flag for check for update is false (for next time we check)
							BridgeConfiguration.SoftwareUpdate.CheckForUpdate = false;

							// Release the refresh lock (since we are done and to avoid out-of-order locking)
							refreshLockScope.Dispose();

							using (new LockScope(_classLock)) // lock to protect timer object
							{
								_softwareUpdatesTimer.Stop();
								_softwareUpdatesTimer.Dispose();
								_softwareUpdatesTimer = null;
							}

							// Raise the completed event for this async operation to notify about the failure
							if (CheckForSoftwareUpdatesCompleted != null)
								CheckForSoftwareUpdatesCompleted(this, new CheckForSoftwareUpdatesCompletedEventArgs(CheckForSoftwareUpdatesResult.Failed));
						}
					}
				}
				finally
				{
					inProgress = false;
				}

			},
			 5000 /*dummy value until Reset is called below since CTimer doesn't seem to respect the ctor parameter*/);

			_softwareUpdatesTimer.Reset(interval, interval);

			return BeginCheckForSoftwareUpdatesResult.Started;
		}

		private CheckForSoftwareUpdatesResult? ExecuteCompleteCheckForSoftwareUpdate()
		{
			Log(DebugLevel.Verbose, "Check if check for software update is complete");

			// Get the latest info, but don't raise the changed event since we may be doing this a lot while waiting for the check to complete
			ExecuteRefreshBridgeConfiguration(false);
			if (!BridgeConfiguration.SoftwareUpdate.CheckForUpdate)
			{
				// Verify our connection to the portal again (as suggested by the Philips documentation's flow chart for checking for updates)
				if (!PlatformConverter.ToBool(BridgeConfiguration.PortalState.SignedOn))
					return CheckForSoftwareUpdatesResult.LostConnectingToHuePortal;

				Log(DebugLevel.Verbose, "Check for software update is complete: {0}", BridgeConfiguration.SoftwareUpdate.UpdateState.ToString());

				return CheckForSoftwareUpdatesResult.Completed;
			}

			return null;
		}

		public void BeginApplySoftwareUpdates(ref BeginApplySoftwareUpdatesResult result)
		{
			Log(DebugLevel.Info, "Begin applying software updates");
			using (new LockScope(_classLock)) // lock to protect timer object initialized in ExecuteBeginApplySoftwareUpdates
			using (new LockScope(_refreshLock)) // lock to refresh and protect BridgeConfiguration from changing while we check its properties
			{
				if (_bridgeConfiguration == null)
				{
					RaiseError("Initialize not called");
					result = BeginApplySoftwareUpdatesResult.Failed;
					return;
				}

				BeginApplySoftwareUpdatesResult localResult;
				localResult = Execute(
					() => ExecuteBeginApplySoftwareUpdates(),
					BeginApplySoftwareUpdatesResult.Failed);

				Log(DebugLevel.Normal, "Begin applying software updates done: {0}", localResult.ToString());
				result = localResult;
			}
		}

		private BeginApplySoftwareUpdatesResult ExecuteBeginApplySoftwareUpdates()
		{
			// NOTE: Caller of this private method must lock "this" to protect this thread/timer
			if (_softwareUpdatesTimer != null)
				return BeginApplySoftwareUpdatesResult.CheckOrUpdateAlreadyInProgress;

			Log(DebugLevel.Debug, "Get latest bridge info");

			// Get the latest info
			ExecuteRefreshBridgeConfiguration();

			Log(DebugLevel.Debug, "Check software update state");

			// Check if we are in the right state to apply the update
			if (BridgeConfiguration.SoftwareUpdate.UpdateState == SoftwareUpdateState.UpdateDownloading)
				return BeginApplySoftwareUpdatesResult.UpdateStillDownloading;
			if (BridgeConfiguration.SoftwareUpdate.UpdateState == SoftwareUpdateState.ApplyingUpdate)
				return BeginApplySoftwareUpdatesResult.UpdateAlreadyBeingApplied;
			if (BridgeConfiguration.SoftwareUpdate.UpdateState == SoftwareUpdateState.NoUpdate)
				return BeginApplySoftwareUpdatesResult.NoUpdateAvailable;

			// Begin applying the update
			string startUpdateResponse = Put(BridgeConfiguration.Api, "{\"swupdate\": {\"updatestate\":3}}");
			Results<Result> startUpdateResults;
			bool startUpdateSucceeded = ParseResultsAndReportErrors(startUpdateResponse, out startUpdateResults);
			if (!startUpdateSucceeded)
				return BeginApplySoftwareUpdatesResult.Failed;

			// Start a timer/thread to wait for the update to complete
			bool inProgress = false;
			int counter = 0;
			int interval = 1000;
			_softwareUpdatesTimer = new CTimer(data =>
			{
				if (inProgress) // Prevent overlapping timer calls (mostly during debugging)
					return;

				inProgress = true;
				try
				{
					using (LockScope refreshLockScope = new LockScope(_refreshLock)) // lock to refresh and protect BridgeConfiguration from changing while we check its properties
					{
						if (!Execute(() =>
						{
							// Check if the update is complete
							ApplySoftwareUpdatesResult? result = ExecuteCompleteApplySoftwareUpdate();

							// Check for an approximate 15 minute time out
							bool timedOut = ++counter * interval >= (15 * 60 * 1000);

							Log(DebugLevel.Verbose, "Software updates completion status: {0}",
								result != null ? result.ToString() :
									(timedOut ? "timed out" : "waiting..."));

							if (timedOut ||
								(result != null && result.Value == ApplySoftwareUpdatesResult.Completed))
							{
								// Set the notify flag back to false (as recommended by Philips so other apps can use this flag reliably)
								string response = Put(BridgeConfiguration.Api, "{\"swupdate\": {\"notify\":false}}");
								Results<Result> results;
								bool success = ParseResultsAndReportErrors(response, out results);
								if (success)
									BridgeConfiguration.SoftwareUpdate.Notify = false;

								// Raise one changed event since our thread/timer has been refreshing the bridge configuration secretly
								// so the code using this client is updated with the latest information
								if (BridgeConfigurationPropertiesChanged != null)
									BridgeConfigurationPropertiesChanged(this, EventArgs.Empty);

								Log(DebugLevel.Verbose, "Release refresh lock");

								// Release the refresh lock (since we are done and to avoid out-of-order locking)
								// ReSharper disable once AccessToDisposedClosure
								refreshLockScope.Dispose();

								Log(DebugLevel.Verbose, "Lock client");
								using (new LockScope(_classLock)) // lock to protect timer object
								{
									Log(DebugLevel.Verbose, "Stop timer");

									_softwareUpdatesTimer.Stop();
									// ReSharper disable once AccessToDisposedClosure
									_softwareUpdatesTimer.Dispose();
									_softwareUpdatesTimer = null;
								}

								if (result == null /*&& implied timedOut == true*/)
									result = ApplySoftwareUpdatesResult.TimedOutWaitingForUpdateToComplete;
							}

							// Finally, raise the completed event for this async operation
							if (result != null && ApplySoftwareUpdatesCompleted != null)
								ApplySoftwareUpdatesCompleted(this, new ApplySoftwareUpdatesCompletedEventArgs(result.Value));

							return true; // Execute succeeded
						}, false))
						{
							// Ensure our flag for check for update is false (for next time we check)
							BridgeConfiguration.SoftwareUpdate.CheckForUpdate = false;

							// Release the refresh lock (since we are done and to avoid out-of-order locking)
							refreshLockScope.Dispose();

							// Execute failed.
							// Exception occurred while checking - hopefully this never happens
							using (new LockScope(_classLock)) // lock to protect timer object
							{
								_softwareUpdatesTimer.Stop();
								_softwareUpdatesTimer.Dispose();
								_softwareUpdatesTimer = null;
							}

							if (ApplySoftwareUpdatesCompleted != null)
								ApplySoftwareUpdatesCompleted(this, new ApplySoftwareUpdatesCompletedEventArgs(ApplySoftwareUpdatesResult.Failed));
						}
					}
				}
				finally
				{
					inProgress = false;
				}
			},
			5000 /*dummy value until Reset is called below since CTimer doesn't seem to respect the ctor parameter*/);

			_softwareUpdatesTimer.Reset(interval, interval);
			return BeginApplySoftwareUpdatesResult.Started;
		}

		private ApplySoftwareUpdatesResult? ExecuteCompleteApplySoftwareUpdate()
		{
			Log(DebugLevel.Verbose, "Check if software update is complete");

			// Get the latest info, but don't raise the changed event since we may be doing this a lot while waiting for the update to complete
			ExecuteRefreshBridgeConfiguration(false);
			if (BridgeConfiguration.SoftwareUpdate.Notify)
			{
				Log(DebugLevel.Verbose, "Software update is complete: {0}", BridgeConfiguration.SoftwareUpdate.UpdateState.ToString());

				return ApplySoftwareUpdatesResult.Completed;
			}

			return null;
		}

		#endregion Bridge Configuration

		#region Lights

		/// <summary>
		/// Refreshes the <see cref="LightBulbs"/> list; 
		/// Existing object references are maintained and only their property values are refreshed.
		/// </summary>
		public void RefreshLightBulbs()
		{
			Log(DebugLevel.Info, "RefreshLightBulbs");
			using (new LockScope(_refreshLock))
				Execute(ExecuteRefreshLightBulbs);
			Log(DebugLevel.Normal, "RefreshLightBulbs: done");
		}

		private void ExecuteRefreshLightBulbs()
		{
			if (_lightBulbsByUniqueId == null)
				_lightBulbsByUniqueId = new Dictionary<string, LightBulb>();

			// Get the latest list of light bulbs
			List<LightBulb> lightBulbsWithChanges =
				RefreshHueObjects(LightBulb.Api, ref _lightBulbs, ref _lightBulbsById);

			// Refresh our unique ID dictionary and (re)initialize our light bulb objects 
			// (some might have been still existing and already initialized, but we just initialize all of them again anyway)
			_lightBulbsByUniqueId = new Dictionary<string, LightBulb>();
			for (int i = 0; i < _lightBulbs.Count; i++)
			{
				LightBulb lightBulb = _lightBulbs[i];
				_lightBulbsByUniqueId.Add(lightBulb.UniqueId, lightBulb);
				InitializeLightBulbObject(lightBulb);
			}

			if (_lightBulbsSortOrder != LightBulbsSortOrder.ByHueId)
			{
				_lightBulbs = SortLightBulbs(_lightBulbs);
				for (int i = 0; i < _lightBulbs.Count; i++)
					_lightBulbs[i].Index = (ushort)i;
			}

			LightBulbs = new ReadOnlyCollection<LightBulb>(_lightBulbs);

			if (LightBulbPropertiesChanged != null)
			{
				foreach (LightBulb lightBulb in lightBulbsWithChanges)
				{
					LightBulbPropertiesChanged(this, new LightBulbPropertiesChangedEventArgs(lightBulb));
				}
			}
		}

		private void InitializeLightBulbObject(LightBulb lightBulb)
		{
			lightBulb.RefreshRequestAction = () =>
				Execute(() =>
				{
					using (new LockScope(_refreshLock))
					{
						return ExecuteRefreshLightBulb(lightBulb.Id, false);
					}
				});

			lightBulb.SetHuePropertiesRequestAction = lightBulbProperties =>
				Execute(() =>
				{
					using (new LockScope(_refreshLock))
					{
						return ExecuteSetHueProperties(lightBulb, lightBulbProperties);
					}
				});

			lightBulb.State.SetHuePropertyRequestAction = (huePropertyName, value, turnOnLightBulb, newColorMode) =>
				Execute(() =>
				{
					using (new LockScope(_refreshLock))
					{
						return ExecuteSetHueProperty(lightBulb, huePropertyName, value, turnOnLightBulb, newColorMode);
					}
				});
		}

		private bool ExecuteRefreshLightBulb(string id, bool useUniqueId)
		{
			Log(DebugLevel.Verbose, "Refresh light bulb {0}", id);

			LightBulb existingLightBulb;
			var primaryIdDictionary = useUniqueId ? _lightBulbsByUniqueId : _lightBulbsById;
			var secondaryIdDictionary = !useUniqueId ? _lightBulbsByUniqueId : _lightBulbsById;
			if (primaryIdDictionary.TryGetValue(id, out existingLightBulb))
				secondaryIdDictionary.Remove(existingLightBulb.UniqueId);

			bool anyChanges = RefreshHueObject(LightBulb.Api, id, _lightBulbs, primaryIdDictionary);
			LightBulb lightBulb;
			if (primaryIdDictionary.TryGetValue(id, out lightBulb))
			{
				if (!string.IsNullOrEmpty(lightBulb.UniqueId))
					secondaryIdDictionary[lightBulb.UniqueId] = lightBulb;

				InitializeLightBulbObject(lightBulb);
			}

			if (anyChanges)
			{
				if (LightBulbPropertiesChanged != null)
					LightBulbPropertiesChanged(this, new LightBulbPropertiesChangedEventArgs(lightBulb));
			}

			return anyChanges;
		}

		private bool ExecuteSetHueProperties(LightBulb lightBulb, LightBulbProperties lightBulbPropertiesToSet)
		{
			if (lightBulb.State.SetHuePropertyRequestInProgress)
				return true;

			lightBulb.State.SetHuePropertyRequestInProgress = true;
			try
			{
				List<object[]> bodyParams = new List<object[]>();

				LightBulbColorMode? newColorMode = null;
				if (!lightBulb.State.On)
				{
					Log(DebugLevel.Debug, "Set light bulb \"{0}\" to on", lightBulb.Name);
					bodyParams.Add(new object[] { "on", true });
				}

				if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetBrightness))
				{
					if (lightBulbPropertiesToSet.Brightness != lightBulb.State.Brightness)
					{
						Log(DebugLevel.Debug, "Set light bulb \"{0}\" to brightness {1}%", lightBulb.Name, lightBulbPropertiesToSet.Brightness / MaxLightBulbBrightness * 100.0);
						if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetBrightnessPercentage))
							bodyParams.Add(new object[] { "bri", lightBulbPropertiesToSet.BrightnessPercentage / 100.0 * MaxLightBulbBrightness});
						else
							bodyParams.Add(new object[] { "bri", lightBulbPropertiesToSet.Brightness });
					}
				}
				else if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetBrightnessOffset))
				{
					Log(DebugLevel.Debug, "Adjust light bulb \"{0}\" brightness by {1} ({2}%)", lightBulb.Name, lightBulbPropertiesToSet.BrightnessOffset, lightBulbPropertiesToSet.BrightnessOffset / MaxLightBulbBrightness * 100.0);
					bodyParams.Add(new object[] { "bri_inc", lightBulbPropertiesToSet.BrightnessOffset });
				}

				// NOTE: These are in priority order as documented by Philips.
				// When all properties are specified, the precedence for the color mode is xy > ct > hs.

				if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetXY))
				{
					newColorMode = LightBulbColorMode.XY;
					bool isInProperColorMode = lightBulb.State.ColorMode == newColorMode.Value;
					if (lightBulbPropertiesToSet.XY[0] != lightBulb.State.XY[0] ||
						lightBulbPropertiesToSet.XY[1] != lightBulb.State.XY[1] ||
						!isInProperColorMode)
					{
						Log(DebugLevel.Debug, "Set light bulb \"{0}\" to XY ({1},{2})", lightBulb.Name, lightBulbPropertiesToSet.XY[0], lightBulbPropertiesToSet.XY[1]);
						bodyParams.Add(new object[] { "xy", lightBulbPropertiesToSet.XY });
					}
				}
				else if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetXYOffset))
				{
					newColorMode = LightBulbColorMode.XY;
					Log(DebugLevel.Debug, "Adjust light bulb \"{0}\" XY by ({1},{2})", lightBulb.Name, lightBulbPropertiesToSet.XYOffset[0], lightBulbPropertiesToSet.XYOffset[1]);
					bodyParams.Add(new object[] { "xy_inc", lightBulbPropertiesToSet.XYOffset });
				}

				if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetColorTemperature))
				{
					if (newColorMode == null)
						newColorMode = LightBulbColorMode.ColorTemperature;
					bool isInProperColorMode = lightBulb.State.ColorMode == newColorMode.Value;
					if (lightBulbPropertiesToSet.ColorTemperature != lightBulb.State.ColorTemperature || !isInProperColorMode)
					{
						Log(DebugLevel.Debug, "Set light bulb \"{0}\" to color temperature {1}", lightBulb.Name, lightBulbPropertiesToSet.ColorTemperature);
						bodyParams.Add(new object[] { "ct", lightBulbPropertiesToSet.ColorTemperature });
					}
				}
				else if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetColorTemperatureOffset))
				{
					if (newColorMode == null)
						newColorMode = LightBulbColorMode.ColorTemperature;
					Log(DebugLevel.Debug, "Adjust light bulb \"{0}\" color temperature by {1})", lightBulb.Name, lightBulbPropertiesToSet.ColorTemperatureOffset);
					bodyParams.Add(new object[] { "ct_inc", lightBulbPropertiesToSet.ColorTemperatureOffset });
				}

				if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetHue) ||
					PlatformConverter.ToBool(lightBulbPropertiesToSet.SetSaturation))
				{
					if (newColorMode == null)
						newColorMode = LightBulbColorMode.HueSaturation;
					bool isInProperColorMode = lightBulb.State.ColorMode == newColorMode.Value;

					// NOTE: When we are not in the right color mode, we always send hue and saturation
					// to get the desired result.  For example, if you are in color temperature mode and 
					// try to go to hue-saturation mode, sending only the hue property ends up with
					// wrong satuaration (even though the light bulb claims the correct saturation).

					if (!isInProperColorMode
						||
						(PlatformConverter.ToBool(lightBulbPropertiesToSet.SetHue) &&
						lightBulbPropertiesToSet.Hue != lightBulb.State.Hue))
					{
						Log(DebugLevel.Debug, "Set light bulb \"{0}\" to hue {1}", lightBulb.Name, lightBulbPropertiesToSet.Hue);
						bodyParams.Add(new object[] { "hue", lightBulbPropertiesToSet.Hue });
					}

					if (!isInProperColorMode
						||
						(PlatformConverter.ToBool(lightBulbPropertiesToSet.SetSaturation) &&
						lightBulbPropertiesToSet.Saturation != lightBulb.State.Saturation))
					{
						Log(DebugLevel.Debug, "Set light bulb \"{0}\" to saturation {1}", lightBulb.Name, lightBulbPropertiesToSet.Saturation);
						if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetSaturationPercentage))
							bodyParams.Add(new object[] { "sat", lightBulbPropertiesToSet.SaturationPercentage / 100.0 * MaxLightBulbSaturation});
						else
							bodyParams.Add(new object[] { "sat", lightBulbPropertiesToSet.Saturation });
					}
				}
				else if (!string.IsNullOrEmpty(lightBulbPropertiesToSet.ColorNameOrHexRgb))
				{
					if (newColorMode == null)
						newColorMode = LightBulbColorMode.XY;
					bool isInProperColorMode = lightBulb.State.ColorMode == newColorMode.Value;

					Log(DebugLevel.Debug, "Set light bulb \"{0}\" to color {1}", lightBulb.Name, lightBulbPropertiesToSet.ColorNameOrHexRgb);

					RGBColor? color;
					string colorNameOrHexRgb = lightBulbPropertiesToSet.ColorNameOrHexRgb;
					if (colorNameOrHexRgb[0] == '#')
					{
						//int a = 0;
						int r = 0;
						int g = 0;
						int b = 0;
						colorNameOrHexRgb = colorNameOrHexRgb.TrimStart('#');
						if (colorNameOrHexRgb.Length == 3)
						{
							r = int.Parse(colorNameOrHexRgb[0].ToString(), NumberStyles.HexNumber);
							g = int.Parse(colorNameOrHexRgb[1].ToString(), NumberStyles.HexNumber);
							b = int.Parse(colorNameOrHexRgb[2].ToString(), NumberStyles.HexNumber);
						}
						else if (colorNameOrHexRgb.Length == 4)
						{
							//a = int.Parse(colorNameOrHexRgb[0].ToString(), NumberStyles.HexNumber);
							r = int.Parse(colorNameOrHexRgb[1].ToString(), NumberStyles.HexNumber);
							g = int.Parse(colorNameOrHexRgb[2].ToString(), NumberStyles.HexNumber);
							b = int.Parse(colorNameOrHexRgb[3].ToString(), NumberStyles.HexNumber);
						}
						else if (colorNameOrHexRgb.Length == 6)
						{
							r = int.Parse(colorNameOrHexRgb.Substring(0, 2), NumberStyles.HexNumber);
							g = int.Parse(colorNameOrHexRgb.Substring(2, 2), NumberStyles.HexNumber);
							b = int.Parse(colorNameOrHexRgb.Substring(4, 2), NumberStyles.HexNumber);
						}
						else if (colorNameOrHexRgb.Length == 8)
						{
							//a = int.Parse(colorNameOrHexRgb.Substring(0, 2), NumberStyles.HexNumber);
							r = int.Parse(colorNameOrHexRgb.Substring(2, 2), NumberStyles.HexNumber);
							g = int.Parse(colorNameOrHexRgb.Substring(4, 2), NumberStyles.HexNumber);
							b = int.Parse(colorNameOrHexRgb.Substring(6, 2), NumberStyles.HexNumber);
						}

						color = new RGBColor(r, g, b);
					}
					else
					{
						string colorName = colorNameOrHexRgb;
						Color systemColor = Color.Empty;
						_colorsByName.TryGetValue(colorName, out systemColor);
						//PropertyInfo propertyInfo = typeof(Color).GetProperty(colorName, BindingFlags.Static | BindingFlags.Public);
						//if (propertyInfo != null)
						//    systemColor = (Color)propertyInfo.GetValue(null, null);
						if (systemColor == Color.Empty)
						{
							RaiseError("Invalid color: " + colorName);
							return false;
						}

						color = new RGBColor(systemColor.R, systemColor.G, systemColor.B);
					}

					//float[] xy = ColorToXY(color.Value);
					CIE1931Point point = HueColorConverter.RgbToXY(color.Value, lightBulb.ModelId);
					if (point.x != lightBulb.State.XY[0] ||
						point.y != lightBulb.State.XY[1] ||
						!isInProperColorMode)
					{
						Log(DebugLevel.Debug, "Set light bulb \"{0}\" to {1} translated to XY ({2},{3})", lightBulb.Name, lightBulbPropertiesToSet.ColorNameOrHexRgb, point.x, point.y);
						bodyParams.Add(new object[] { "xy", new[] { (float)point.x, (float)point.y } });
					}
				}
				else
				{
					if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetHueOffset))
					{
						if (newColorMode == null)
							newColorMode = LightBulbColorMode.HueSaturation;
						Log(DebugLevel.Debug, "Adjust light bulb \"{0}\" hue by {1})", lightBulb.Name, lightBulbPropertiesToSet.HueOffset);
						bodyParams.Add(new object[] { "hue_inc", lightBulbPropertiesToSet.HueOffset });
					}

					if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetSaturationOffset))
					{
						if (newColorMode == null)
							newColorMode = LightBulbColorMode.HueSaturation;
						Log(DebugLevel.Debug, "Adjust light bulb \"{0}\" saturation by {1})", lightBulb.Name, lightBulbPropertiesToSet.SaturationOffset);
						bodyParams.Add(new object[] { "sat_inc", lightBulbPropertiesToSet.SaturationOffset });
					}
				}

				bool addTransitionTime = true;
				if (lightBulbPropertiesToSet.TransitionTimeToUse >= 0)
				{
					AddTransitionTime(lightBulb, bodyParams, lightBulbPropertiesToSet.TransitionTimeToUse);
					addTransitionTime = false;
				}

				return SendStateUpdateAndRaiseChangedEvent(lightBulb, bodyParams, addTransitionTime, newColorMode);
			}
			finally
			{
				lightBulb.State.SetHuePropertyRequestInProgress = false;
			}
		}

		private bool ExecuteSetHueProperty(LightBulb lightBulb, string huePropertyName, object value, bool turnOnLightBulb, LightBulbColorMode? newColorMode)
		{
			if (lightBulb.State.SetHuePropertyRequestInProgress)
				return true;

			lightBulb.State.SetHuePropertyRequestInProgress = true;
			try
			{
				Log(DebugLevel.Debug, "Set light bulb \"{0}\" property \"{1}\" to {2}", lightBulb.Name, huePropertyName, value != null ? value.ToString() : "null");
				List<object[]> bodyParams = new List<object[]>();
				if (turnOnLightBulb && !lightBulb.State.On)
					bodyParams.Add(new object[] { "on", true });
				bodyParams.Add(new[] { huePropertyName, value });
				return SendStateUpdateAndRaiseChangedEvent(lightBulb, bodyParams, true, newColorMode);
			}
			finally
			{
				lightBulb.State.SetHuePropertyRequestInProgress = false;
			}
		}

		public void BeginSearchForNewLightBulbs(ref BeginSearchForNewLightBulbsResult result)
		{
			Log(DebugLevel.Info, "Begin search for new light bulbs");
			using (new LockScope(_classLock)) // lock to protect timer object initialized in ExecuteBeginSearchForNewLightBulbs
			using (new LockScope(_refreshLock)) // lock to access light bulbs collections
			{
				if (_bridgeConfiguration == null)
				{
					RaiseError("Initialize not called");
					result = BeginSearchForNewLightBulbsResult.Failed;
					return;
				}

				BeginSearchForNewLightBulbsResult localResult;
				localResult = Execute(
					() => ExecuteBeginSearchForNewLightBulbs(),
					BeginSearchForNewLightBulbsResult.Failed);

				Log(DebugLevel.Normal, "Begin search for new light bulbs done: {0}", localResult.ToString());
				result = localResult;
			}
		}

		private BeginSearchForNewLightBulbsResult ExecuteBeginSearchForNewLightBulbs()
		{
			string response = Post(MakeApi(LightBulb.Api), "");
			Results<Result> results;
			bool success = ParseResultsAndReportErrors(response, out results);
			if (!success)
				return BeginSearchForNewLightBulbsResult.Failed;

			foreach (Result result in results)
			{
				if (result.Error == null && result.Success != null)
				{
					object value;
					if (!result.Success.TryGetValue("/" + LightBulb.Api, out value))
					{
						RaiseError("Failed to begin search for new lights");
						return BeginSearchForNewLightBulbsResult.Failed;
					}
				}
			}

			Log(DebugLevel.Verbose, "Begin waiting for search for new light bulbs to complete");

			bool inProgress = false;
			int counter = 0;
			const int interval = 1000;
			List<string> cumulativeNewLightBulbIds = new List<string>();
			_searchForNewLightBulbsTimer = new CTimer(data =>
			{
				if (inProgress) // Prevent overlapping timer calls (mostly during debugging)
					return;

				inProgress = true;
				try
				{
					using (var refreshLockScope = new LockScope(_refreshLock)) // lock to access light bulbs collections
					{
						if (_searchForNewLightBulbsTimer == null) // A previous timer elapsed already determined we were finished
							return;

						if (!Execute(() =>
						{
							// Check if the "check for updates" is complete
							SearchForNewLightBulbsResult? result = ExecuteCompleteSearchForNewLightBulbs(cumulativeNewLightBulbIds);

							// Check for an approximate 65 second time out (as suggested by Philips)
							bool done = ++counter * interval >= (65 * 1000);

							Log(DebugLevel.Verbose, "Search for new light bulbs completion status: {0}",
								result != null ? result.ToString() :
									(done ? "done" : "searching..."));

							if (done || result != null)
							{
								Log(DebugLevel.Verbose, "Dispose refresh lock");
								// Release the refresh lock (since we are done and to avoid out-of-order locking)
								// ReSharper disable once AccessToDisposedClosure
								refreshLockScope.Dispose();

								Log(DebugLevel.Verbose, "Lock client");
								using (new LockScope(_classLock)) // lock to protect timer object
								{
									Log(DebugLevel.Verbose, "Locked client");
									_searchForNewLightBulbsTimer.Stop();
									// ReSharper disable once AccessToDisposedClosure
									_searchForNewLightBulbsTimer.Dispose();
									_searchForNewLightBulbsTimer = null;
								}

								if (result == null /* && implied done == true*/)
									result = SearchForNewLightBulbsResult.Completed;
							}

							// Finally, raise the completed event for this async operation
							if (result != null && SearchForNewLightBulbsCompleted != null)
							{
								Log(DebugLevel.Verbose, "Raise completed event: {0}", result.Value.ToString());
								SearchForNewLightBulbsCompleted(this, new SearchForNewLightBulbsCompletedEventArgs(result.Value));
							}

							return true; // Execute succeeded
						}, false))
						{
							// Execute failed.
							// Exception occurred while checking - hopefully this never happens

							// Release the refresh lock (since we are done and to avoid out-of-order locking)
							refreshLockScope.Dispose();

							using (new LockScope(_classLock)) // lock to protect timer object
							{
								_searchForNewLightBulbsTimer.Stop();
								_searchForNewLightBulbsTimer.Dispose();
								_searchForNewLightBulbsTimer = null;
							}

							// Raise the completed event for this async operation to notify about the failure
							if (SearchForNewLightBulbsCompleted != null)
								SearchForNewLightBulbsCompleted(this, new SearchForNewLightBulbsCompletedEventArgs(SearchForNewLightBulbsResult.Failed));
						}
					}
				}
				finally
				{
					inProgress = false;
				}

			},
			5000 /*dummy value until Reset is called below since CTimer doesn't seem to respect the ctor parameter*/);

			_searchForNewLightBulbsTimer.Reset(interval, interval);

			return BeginSearchForNewLightBulbsResult.Started;
		}

		private SearchForNewLightBulbsResult? ExecuteCompleteSearchForNewLightBulbs(List<string> cumulativeNewLightBulbIds)
		{
			string newLightsResponse = Get(MakeApi(LightBulb.Api, "new"));
			if (CheckForAndReportErrors(newLightsResponse))
				return SearchForNewLightBulbsResult.Failed;

			var newLightsResponseObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(newLightsResponse);

			SearchForNewLightBulbsResult? result = null;
			foreach (KeyValuePair<string, object> kv in newLightsResponseObj)
			{
				if (kv.Key == "lastscan")
				{
					if (kv.Value is DateTime)
						break;

					string lastScan = kv.Value as string;
					if (lastScan == null)
					{
						RaiseError("Unexpected response while searching for new lights: " + newLightsResponse);
						result = SearchForNewLightBulbsResult.Failed;
						continue;
					}

					if (lastScan == "none")
					{
						RaiseError("Searching for new lights did not start or complete; Make sure the bridge is stays powered on");
						result = SearchForNewLightBulbsResult.Failed;
						continue;
					}

					if (lastScan == "active")
						continue;

					// Completed search
					//DateTime lastScanDateTime;
					//DateTime.TryParse(lastScan, out lastScanDateTime);
					result = SearchForNewLightBulbsResult.Completed;
					continue;
				}

				cumulativeNewLightBulbIds.Add(kv.Key);

				Dictionary<string, string> nameAndValue = ((JObject)kv.Value).ToObject<Dictionary<string, string>>();
				string name;
				if (nameAndValue.TryGetValue("name", out name))
				{
					if (FoundLightBulb != null)
					{
						FoundLightBulb(this, new FoundLightBulbEventArgs(kv.Key, name));
					}
				}
			}

			if (result == SearchForNewLightBulbsResult.Completed)
			{
				if (cumulativeNewLightBulbIds.Count > 0)
				{
					foreach (string id in cumulativeNewLightBulbIds)
						ExecuteRefreshLightBulb(id, false);

					ReSortLightBulbs();
				}
			}

			return result;
		}

		public Bool DeleteLightBulb(int index)
		{
			return Execute(() =>
			{
				using (new LockScope(_refreshLock))
				{
					LightBulb lightBulb = GetHueObject(index, _lightBulbs);
					if (lightBulb == null)
						return PlatformConverter.ToPlatformBool(false);

					return PlatformConverter.ToPlatformBool(DeleteLightBulb(lightBulb.Id, false));
				}
			});
		}

		public Bool DeleteLightBulbById(string id)
		{
			return PlatformConverter.ToPlatformBool(DeleteLightBulb(id, false));
		}

		public Bool DeleteLightBulbByUniqueId(string uniqueId)
		{
			return PlatformConverter.ToPlatformBool(DeleteLightBulb(uniqueId, true));
		}

		private bool DeleteLightBulb(string id, bool useUniqueId)
		{
			Log(DebugLevel.Info, "DeleteLightBulb: {0}", id);
			bool success = Execute(() =>
			{
				using (new LockScope(_refreshLock))
					return ExecuteDeleteLightBulb(id, useUniqueId);
			});
			Log(DebugLevel.Info, "DeleteLightBulb: {0}: done", id);
			return success;
		}

		private bool ExecuteDeleteLightBulb(string id, bool useUniqueId)
		{
			LightBulb lightBulb = TryGetLightBulb(id, useUniqueId);
			if (lightBulb == null)
				return false;

			string response = Delete(MakeApi(LightBulb.Api, lightBulb.Id));
			Results<Result> results;
			return ParseResultsAndReportErrors(response, out results);
		}

		public ReadOnlyCollection<LightBulb> LightBulbs
		{
			get { return _readOnlyLightBulbsCollection; }
			private set { UpdateProperty("LightBulbs", ref _readOnlyLightBulbsCollection, value); }
		}

		public short LightBulbCount
		{
			get
			{
				using (new LockScope(_refreshLock))
					return LightBulbs != null ? (short)LightBulbs.Count : (short)0;
			}
		}

		private void ReSortLightBulbs()
		{
			_lightBulbs = SortLightBulbs(_lightBulbs);
			for (int i = 0; i < _lightBulbs.Count; i++)
				_lightBulbs[i].Index = (ushort)i;
			LightBulbs = new ReadOnlyCollection<LightBulb>(_lightBulbs);
			if (LightBulbPropertiesChanged != null)
			{
				for (int i = 0; i < _lightBulbs.Count; i++)
				{
					LightBulbPropertiesChanged(this, new LightBulbPropertiesChangedEventArgs(_lightBulbs[i]));
				}
			}
		}

		private List<LightBulb> SortLightBulbs(List<LightBulb> lightBulbs)
		{
			if (_lightBulbsSortOrder == LightBulbsSortOrder.ByName)
				return lightBulbs.OrderBy(x => x.Name).ToList();
			return lightBulbs.OrderBy(x => x.Id).ToList();
		}

		public Bool TryGetLightBulb(int index, ref LightBulb lightBulb)
		{
			using (new LockScope(_refreshLock))
			{
				lightBulb = GetHueObject(index, _lightBulbs);
				return PlatformConverter.ToPlatformBool(lightBulb != null);
			}
		}

		public Bool TryGetLightBulbById(string id, ref LightBulb lightBulb)
		{
			using (new LockScope(_refreshLock))
			{
				lightBulb = TryGetLightBulb(id, false);
				return PlatformConverter.ToPlatformBool(lightBulb != null);
			}
		}

		public Bool TryGetLightBulbByUniqueId(string uniqueId, ref LightBulb lightBulb)
		{
			using (new LockScope(_refreshLock))
			{
				lightBulb = TryGetLightBulb(uniqueId, true);
				return PlatformConverter.ToPlatformBool(lightBulb != null);
			}
		}

		private LightBulb TryGetLightBulb(string id, bool useUniqueId)
		{
			if (_lightBulbs == null)
			{
				RaiseError("Initialize not called");
				return null;
			}

			LightBulb lightBulb;
			if ((useUniqueId ? _lightBulbsByUniqueId : _lightBulbsById).TryGetValue(id, out lightBulb))
				return lightBulb;
			RaiseError("Light bulb with ID " + id + " not found");
			return null;
		}

		private void AddTransitionTime(LightBulb lightBulb, List<object[]> bodyParams)
		{
			AddTransitionTime(lightBulb, bodyParams, null);
		}

		private void AddTransitionTime(LightBulb lightBulb, List<object[]> bodyParams, short? customTransitionTime)
		{
			short transitionTime;
			if (customTransitionTime != null && customTransitionTime.Value >= 0)
				transitionTime = customTransitionTime.Value;
			else if (lightBulb.TransitionTime >= 0)
				transitionTime = lightBulb.TransitionTime;
			else
				transitionTime = LightBulbsDefaultTransitionTime;

			if (transitionTime >= 0)
			{
				Log(DebugLevel.Debug, "Add transition time: {0} seconds", transitionTime / 10.0f);
				bodyParams.Add(new object[] { "transitiontime", transitionTime });
			}
		}

		/// <summary>
		/// Time of the transition from a light’s current state to the new state (in 100ms multiples)
		/// (e.g. 10 = 1 second)
		/// <para>
		/// If this is less than 0, the Philips Hue default is used, which is 400 ms or 0.4 seconds.
		/// </para>
		/// <para>
		/// Each <see cref="LightBulb"/> can also be configured to have different transition times
		/// using the <see cref="LightBulb.TransitionTime"/> property
		/// </para>
		/// </summary>
		public short LightBulbsDefaultTransitionTime
		{
			get { return _lightBulbsDefaultTransitionTime; }
			set { UpdateProperty("LightBulbsDefaultTransitionTime", ref _lightBulbsDefaultTransitionTime, value); }
		}

		private bool SendStateUpdateAndRaiseChangedEvent(
			LightBulb lightBulb,
			List<object[]> bodyParams,
			bool addTransitionTime,
			LightBulbColorMode? newColorMode)
		{
#if SIMPL_SHARP
			bool anyPropertiesUpdated = false;
			PropertyChangedEventHandler propertyChangedHandler = (sender, e) =>
			{
				anyPropertiesUpdated = true;
			};
			lightBulb.PropertyChanged += propertyChangedHandler;

			try
			{
				bool allSuccess = SendStateUpdate(lightBulb, bodyParams, true, newColorMode);

				if (anyPropertiesUpdated)
				{
					if (LightBulbPropertiesChanged != null)
						LightBulbPropertiesChanged(this, new LightBulbPropertiesChangedEventArgs(lightBulb));
				}

				return allSuccess;
			}
			finally
			{
				lightBulb.PropertyChanged -= propertyChangedHandler;
			}
#else
			bool allSuccess = SendStateUpdate(lightBulb, bodyParams, addTransitionTime, newColorMode);
			return allSuccess;
#endif
		}

		private bool SendStateUpdate(
			LightBulb lightBulb,
			List<object[]> bodyParams,
			bool addTransitionTime,
			LightBulbColorMode? newColorMode)
		{
			if (bodyParams.Count == 0)
				return true;

			if (addTransitionTime)
				AddTransitionTime(lightBulb, bodyParams);

			string response = Put(
				MakeApi(lightBulb.ApiName, lightBulb.Id, lightBulb.State.ApiName),
				MakeBody(bodyParams.ToArray()));

			Results<Result> results;
			bool allSuccess = ParseResultsAndReportErrors(response, out results);
			foreach (Result result in results)
			{
				if (result.Error == null && result.Success != null)
				{
					foreach (object[] param in bodyParams)
					{
						string property = (string)param[0];
						string propertyWithoutInc = property.Replace("_inc", "");
						string key = "/" + lightBulb.ApiName + "/" + lightBulb.Id + "/" + lightBulb.State.ApiName + "/" + propertyWithoutInc;
						object value;
						if (result.Success.TryGetValue(key, out value))
						{
							lightBulb.State.UpdateHueProperty(propertyWithoutInc, value);
						}
					}
				}
			}

			if (allSuccess)
			{
				if (newColorMode != null)
					lightBulb.State.ColorMode = newColorMode.Value;
			}
			else
			{
				if (newColorMode != null)
				{
					// Since there were errors, we don't really know what color mode we are in now.
					// Refresh the info for this bulb to we are back to a known state.
					lightBulb.Refresh();
				}
			}

			return allSuccess;
		}

		/// <summary>
		/// Checks for error-only responses, where upon success, the response is the actual content
		/// rather than a "success" message.
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		private bool CheckForAndReportErrors(string response)
		{
			// Preliminary check for an error to avoid deserializing a successful response below for nothing
			if (!response.Contains("\"error\""))
				return false;

			JArray jArray = JsonConvert.DeserializeObject<object>(response) as JArray;
			if (jArray != null)
			{
				bool anyErrors = false;
				foreach (JToken jArrayToken in jArray)
				{
					var jObject = jArrayToken as JObject;
					if (jObject != null)
					{
						JToken jToken;
						if (jObject.TryGetValue("error", out jToken))
						{
							anyErrors = true;
							Error error = new Error
							{
								Type = (int)jToken["type"],
								Address = (string)jToken["address"],
								Description = (string)jToken["description"]
							};
							RaiseError(error);
						}
					}
				}

				return anyErrors;
			}

			return false;
		}

		private bool ParseResultsAndReportErrors<T>(string response, out Results<T> results)
			where T : ResultBase
		{
			Log(DebugLevel.Verbose, "Response: {0}", response);

			results = JsonConvert.DeserializeObject<Results<T>>(response, JsonSerializerSettings);
			bool allSuccess = true;
			foreach (T result in results)
			{
				if (result.Error != null)
				{
					allSuccess = false;
					RaiseError(result.Error);
				}
			}

			Log(DebugLevel.Verbose, "Response: Results count={0}; all success={1}",
				results != null ? results.Count.ToString() : "null",
				allSuccess);

			return allSuccess;
		}

		#endregion Lights

		#region Groups

		public ReadOnlyCollection<Group> Groups { get; private set; }

		public void RefreshGroups()
		{
			using (new LockScope(_refreshLock))
			{
				RefreshHueObjects(Group.Api, ref _groups, ref _groupsById);
				Groups = new ReadOnlyCollection<Group>(_groups);
			}
		}

		public bool RefreshGroup(string groupId)
		{
			return RefreshHueObject(Group.Api, groupId, _groups, _groupsById);
		}

		#endregion Groups

		#region Scenes

		public ReadOnlyCollection<Scene> Scenes
		{
			get { return _readOnlyScenesCollection; }
			private set { UpdateProperty("Scenes", ref _readOnlyScenesCollection, value); }
		}

		public void RefreshScenes()
		{
			Log(DebugLevel.Info, "Refresh Scenes");
			using (new LockScope(_refreshLock))
				Execute(ExecuteRefreshScenes);
			Log(DebugLevel.Normal, "Refresh Scenes: done");
		}

		private void ExecuteRefreshScenes()
		{
			List<Scene> scenesWithChanges =
				RefreshHueObjects(Scene.Api, ref _scenes, ref _scenesById);

			for (int i = 0; i < _scenes.Count; i++)
			{
				Scene scene = _scenes[i];
				InitializeSceneObject(scene);
			}

			Scenes = new ReadOnlyCollection<Scene>(_scenes);

			if (ScenePropertiesChanged != null)
			{
				foreach (Scene scene in scenesWithChanges)
				{
					ScenePropertiesChanged(this, new ScenePropertiesChangedEventArgs(scene));
				}
			}
		}

		private bool ExecuteRefreshScene(string sceneId)
		{
			Log(DebugLevel.Verbose, "Refresh scene {0}", sceneId);

			bool anyChanged = RefreshHueObject(Scene.Api, sceneId, _scenes, _scenesById);
			Scene scene;
			if (_scenesById.TryGetValue(sceneId, out scene))
			{
				InitializeSceneObject(scene);
			}

			if (anyChanged)
			{
				if (ScenePropertiesChanged != null)
					ScenePropertiesChanged(this, new ScenePropertiesChangedEventArgs(scene));
			}

			return anyChanged;
		}

		private void InitializeSceneObject(Scene scene)
		{
		}

		public short SceneCount
		{
			get
			{
				using (new LockScope(_refreshLock))
					return Scenes != null ? (short)Scenes.Count : (short)0;
			}
		}

		private void ReSortScenes()
		{
			_scenes = SortScenes(_scenes);
			for (int i = 0; i < _scenes.Count; i++)
				_scenes[i].Index = (ushort)i;
			Scenes = new ReadOnlyCollection<Scene>(_scenes);
			if (ScenePropertiesChanged != null)
			{
				for (int i = 0; i < _scenes.Count; i++)
				{
					ScenePropertiesChanged(this, new ScenePropertiesChangedEventArgs(_scenes[i]));
				}
			}
		}

		private List<Scene> SortScenes(List<Scene> scenes)
		{
			return scenes.OrderBy(x => x.Name).ToList();
		}

		public Bool TryScene(int index, ref Scene scene)
		{
			using (new LockScope(_refreshLock))
			{
				scene = GetHueObject(index, _scenes);
				return PlatformConverter.ToPlatformBool(scene != null);
			}
		}

		public Bool TryGetSceneId(string id, ref Scene scene)
		{
			using (new LockScope(_refreshLock))
			{
				scene = TryGetScene(id);
				return PlatformConverter.ToPlatformBool(scene != null);
			}
		}

		private Scene TryGetScene(string id)
		{
			if (_scenes == null)
			{
				RaiseError("Initialize not called");
				return null;
			}

			Scene scene;
			if (_scenesById.TryGetValue(id, out scene))
				return scene;
			RaiseError("Scene with ID " + id + " not found");
			return null;
		}

		public void SetScene(string sceneId)
		{
			Put(
				MakeApi("groups", "0", "action"),
				MakeBody(new object[] { "scene", sceneId }));
		}

		public bool CreateScene(string sceneName, params string[] lightIds)
		{
			List<object[]> bodyParams = new List<object[]>();
			bodyParams.Add(new object[] { "name", sceneName });
			bodyParams.Add(new object[] { "lights", lightIds });
			bodyParams.Add(new object[] { "recycle", false });

			string response = Post(
				MakeApi(Scene.Api),
				MakeBody(bodyParams.ToArray()));
			Results<Result> results;
			bool allSuccess = ParseResultsAndReportErrors(response, out results);
			foreach (Result result in results)
			{
				if (result.Error == null && result.Success != null)
				{
					object value;
					if (result.Success.TryGetValue("id", out value))
					{
						ExecuteRefreshScene((string)value);
					}
				}
			}

			return allSuccess;
		}

		public bool UpdateScene(string sceneId)
		{
			List<object[]> bodyParams = new List<object[]>();
			bodyParams.Add(new object[] { "storelightstate", true });

			string response = Put(
				MakeApi(Scene.Api, sceneId),
				MakeBody(bodyParams.ToArray()));
			Results<Result> results;
			bool allSuccess = ParseResultsAndReportErrors(response, out results);
			foreach (Result result in results)
			{
				if (result.Error == null && result.Success != null)
				{
					object value;
					if (result.Success.TryGetValue("/" + Scene.Api + "/" + sceneId + "/storelightstate", out value))
					{
						return true;
					}
				}
			}

			return allSuccess;
		}

		public bool DeleteScene(string sceneId)
		{
			string response = Delete(MakeApi(Scene.Api, sceneId));
			Results<BasicResult> results;
			bool allSuccess = ParseResultsAndReportErrors(response, out results);
			foreach (BasicResult result in results)
			{
				if (result.Error == null && result.Success != null)
				{
					if (result.Success == "/" + Scene.Api + "/" + sceneId + " deleted")
					{
						Scene scene;
						if (_scenesById.TryGetValue(sceneId, out scene))
						{
							_scenesById.Remove(sceneId);
							_scenes.Remove(scene);
						}
					}
				}
			}

			return allSuccess;
		}

		#endregion Scenes

		#region Bridge Communication

		private string Get(string api)
		{
			Log(DebugLevel.Verbose, "Get: {0}", api);
			string url = MakeUrl(api);
			string response = Http.Get(url);
			Log(DebugLevel.Verbose, "Get: response {0}", response);
			return response;
		}

		private string Put(string api, string body)
		{
			string url = MakeUrl(api);
			Log(DebugLevel.Verbose, "Put: {0}: {1}", api, body);
			string response = Http.Put(url, body);
			Log(DebugLevel.Verbose, "Put: response {0}", response);
			return response;
		}

		private string Post(string api, string body)
		{
			return Post(api, body, false);
		}

		private string Post(string api, string body, bool apiIsFullUrl)
		{
			Log(DebugLevel.Verbose, "Post: {0}: {1}", api, body);

			string url = apiIsFullUrl ? api : MakeUrl(api);
			string response = Http.Post(url, body);
			Log(DebugLevel.Verbose, "Post: response {0}", response);
			return response;
		}

		private string Delete(string api)
		{
			Log(DebugLevel.Verbose, "Delete: {0}: {1}", api);
			string url = MakeUrl(api);
			string response = Http.Delete(url);
			Log(DebugLevel.Verbose, "Delete: response {0}", response);
			return response;
		}

		private string MakeUrl(string api)
		{
			return string.Format("http://{0}/api/{1}/{2}",
				IpAddressOrHostName,
				Username,
				api);
		}

		private string MakeApi(params string[] parts)
		{
			return parts.Length > 0 ? string.Join("/", parts) : "";
		}

		private string MakeBody(params object[][] nameAndValues)
		{
			var jObject = new JObject();
			foreach (object[] nameAndValue in nameAndValues)
			{
				jObject.Add((string)nameAndValue[0],
					JToken.FromObject(nameAndValue[1] is Enum == false ? nameAndValue[1] : nameAndValue[1].ToString().ToLower()));
			}

			return jObject.ToString();
		}

		#endregion

		#region Generic HueObject Methods

		private T GetHueObject<T>(int index, IList<T> hueObjects) where T : HueObject
		{
			if (hueObjects == null)
			{
				RaiseError("Initialize not called");
				return null;
			}

			if (index < 0 || index >= hueObjects.Count)
			{
				RaiseError("Index out of range: " + index);
				return null;
			}

			return hueObjects[index];
		}

		private List<T> RefreshHueObjects<T>(
			string apiName,
			ref List<T> objects,
			ref Dictionary<string, T> objectsById)
			where T : IdentifiableHueObject
		{
			Log(DebugLevel.Debug, "RefreshHueObjects: {0}", apiName);

			if (objects == null)
				objects = new List<T>();
			if (objectsById == null)
				objectsById = new Dictionary<string, T>();

			// Get the latest list of objects
			string response = Get(MakeApi(apiName));
			if (CheckForAndReportErrors(response))
				return null;

			Dictionary<string, T> latestObjectsById;
			if (response != null)
				latestObjectsById = JsonConvert.DeserializeObject<Dictionary<string, T>>(response, JsonSerializerSettings);
			else
				latestObjectsById = new Dictionary<string, T>();

			// Enumerate the existing objects
			Dictionary<string, T> existingObjectsById = new Dictionary<string, T>();
			foreach (KeyValuePair<string, T> kv in objectsById)
			{
				existingObjectsById.Add(kv.Key, kv.Value);
			}

			// Remove the objects that no longer exist
			foreach (KeyValuePair<string, T> kv in existingObjectsById)
			{
				if (!latestObjectsById.ContainsKey(kv.Key))
				{
					objectsById.Remove(kv.Key);
					objects.Remove(kv.Value);
				}
			}

			// Add or update the new or still existing objects
			List<T> objectsWithChanges = new List<T>();
			foreach (KeyValuePair<string, T> kv in latestObjectsById)
			{
				kv.Value.OnDeserialized();
				if (AddOrUpdateHueObject(kv, objects, objectsById))
					objectsWithChanges.Add(kv.Value);
			}

			Log(DebugLevel.Debug, "RefreshHueObjects: {0}: done", apiName);

			return objectsWithChanges;
		}

		private bool RefreshHueObject<T>(
			string apiName,
			string objectId,
			List<T> objects,
			Dictionary<string, T> objectsById)
			where T : IdentifiableHueObject
		{
			Log(DebugLevel.Debug, "RefreshHueObject: {0}/{1}", apiName, objectId);

			string response = Get(MakeApi(apiName, objectId));
			if (CheckForAndReportErrors(response))
				return false;

			var hueObject = JsonConvert.DeserializeObject<T>(response, JsonSerializerSettings);
			if (hueObject != null)
			{
				hueObject.OnDeserialized();
				return AddOrUpdateHueObject(new KeyValuePair<string, T>(objectId, hueObject), objects, objectsById);
			}

			return false;
		}

		private bool AddOrUpdateHueObject<T>(
			KeyValuePair<string, T> kv,
			List<T> objects,
			Dictionary<string, T> objectsById)
			where T : IdentifiableHueObject
		{
			string id = kv.Key;
			T hueObject = kv.Value;

			Log(DebugLevel.Verbose, "AddOrUpdateHueObject: {0} ID={1}", typeof(T).Name, id);

			T existingHueObject;
			objectsById.TryGetValue(id, out existingHueObject);
			if (existingHueObject != null)
			{
				Log(DebugLevel.Verbose, "AddOrUpdateHueObject: Update existing");
				hueObject.Id = kv.Key;
				return existingHueObject.UpdateFrom(hueObject);
			}

			Log(DebugLevel.Verbose, "AddOrUpdateHueObject: Initialize new");
			InitializeHueObject(hueObject, kv);
			objects.Add(hueObject);
			hueObject.Index = (ushort)(objects.Count - 1);
			objectsById[kv.Value.Id] = hueObject;
			return true;
		}

		private void InitializeHueObject<T>(T hueObject, KeyValuePair<string, T> kv)
			where T : IdentifiableHueObject
		{
			hueObject.Id = kv.Key;

			hueObject.LogAction = (debugLevel, message, parameters) =>
			{
				Log(debugLevel, message, parameters);
			};

			hueObject.SetHuePropertyRequestAction = (name, value) =>
			{
				using (new LockScope(_refreshLock))
				{
					if (hueObject.SetHuePropertyRequestInProgress)
						return true;

					hueObject.SetHuePropertyRequestInProgress = true;
					try
					{
						List<object[]> bodyParams = new List<object[]>();
						bodyParams.Add(new[] { name, value });
						SetProperty(hueObject, bodyParams);
						return true;
					}
					finally
					{
						hueObject.SetHuePropertyRequestInProgress = false;
					}
				}
			};
		}

		private bool SetProperty<T>(
			T hueObject,
			List<object[]> bodyParams)
			where T : IdentifiableHueObject
		{
			if (bodyParams.Count == 0)
				return true;

			string response = Put(
				MakeApi(hueObject.ApiName, hueObject.Id),
				MakeBody(bodyParams.ToArray()));

			Results<Result> results;
			bool allSuccess = ParseResultsAndReportErrors(response, out results);
			foreach (Result result in results)
			{
				if (result.Error == null && result.Success != null)
				{
					foreach (object[] param in bodyParams)
					{
						string property = (string)param[0];
						string key = "/" + hueObject.ApiName + "/" + hueObject.Id + "/" + property;
						object value;
						if (result.Success.TryGetValue(key, out value))
						{
							hueObject.UpdateHueProperty(property, value);
						}
					}
				}
			}

			return allSuccess;
		}

		#endregion

		#region Serialization

		internal JsonSerializerSettings JsonSerializerSettings
		{
			get
			{
				return new JsonSerializerSettings()
				{
					ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
				};
			}
		}

		#endregion
	}
}
