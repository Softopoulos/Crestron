using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Bool = System.Int16;
using Softopoulos.Crestron.Core;
using Softopoulos.Crestron.Core.Diagnostics;


namespace Softopoulos.Crestron.PhilipsHue
{
	/// <summary>
	/// Software updates available to the bridge/devices;  See it used here: <see cref="SoftwareUpdate.DeviceTypes"/>
	/// </summary>
	public class SoftwareUpdateDeviceTypes : HueObject
	{
		internal const string Api = "config/swupdate/devicetypes";

		internal override string ApiName
		{
			get { return Api; }
		}

		private bool _bridge;

		private SoftwareUpdateDeviceTypes()
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
					"Bridge", new FieldGetterSetterPair<Bool>()
					{
						Getter = () => Bridge,
						Setter = newValue => Bridge = newValue
					}
				},
			};
		}

		internal override bool UpdateFrom(HueObject hueObject)
		{
			SoftwareUpdateDeviceTypes swUpdateDeviceTypes = hueObject as SoftwareUpdateDeviceTypes;
			if (swUpdateDeviceTypes == null)
				return false;

			bool anyChanged = base.UpdateFrom(hueObject);

			if (!Lights.SequenceEqual(swUpdateDeviceTypes.Lights))
			{
				if (IsDeserialized)
					Log(DebugLevel.Debug, "Update SoftwareUpdateDeviceTypes.Lights to {0}", string.Join(",", swUpdateDeviceTypes.Lights));

				Lights = swUpdateDeviceTypes.Lights;
				NotifyPropertyChanged("Lights");
				anyChanged = true;
			}

			if (!Sensors.SequenceEqual(swUpdateDeviceTypes.Sensors))
			{
				if (IsDeserialized)
					Log(DebugLevel.Debug, "Update SoftwareUpdateDeviceTypes.Sensors to {0}", string.Join(",", swUpdateDeviceTypes.Sensors));

				Sensors = swUpdateDeviceTypes.Sensors;
				NotifyPropertyChanged("Sensors");
				anyChanged = true;
			}

			return anyChanged;
		}

		#endregion Property Updating

		/// <summary>
		/// Update of bridge available;  No special user action is required.
		/// </summary>
		[JsonProperty("bridge")]
		public Bool Bridge
		{
			get { return PlatformConverter.ToPlatformBool(_bridge); }
			private set { _bridge = PlatformConverter.ToBool(value); }
		}

		/// <summary>
		/// List of lights to be updated or empty if no update available
		/// </summary>
		[JsonProperty("lights")]
		public string[] Lights { get; private set; }

		/// <summary>
		/// List of sensors to be updated or empty if no update available
		/// </summary>
		[JsonProperty("sensors")]
		public string[] Sensors { get; private set; }
	}
}