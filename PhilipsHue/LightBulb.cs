using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json;

using Bool = System.Int16;
using Softopoulos.Crestron.Core;
using Softopoulos.Crestron.Core.Diagnostics;
using Softopoulos.Crestron.Core.Threading;
using Crestron.SimplSharp;

namespace Softopoulos.Crestron.PhilipsHue
{
	/// <summary>
	/// Represents a light bulb registered with the Hue Bridge
	/// </summary>
	public class LightBulb : IdentifiableHueObject
	{
		internal const string Api = "lights";

		internal override string ApiName
		{
			get { return Api; }
		}

		private string _name;
		private string _type;
		private State _state;
		private short _transitionTime = -1;

		private LightBulbProperties _beforeTemporaryChangeProperties;

		public LightBulb()
		{
		}

		#region Property Updating

		internal LightBulbSetPropertiesRequestAction SetHuePropertiesRequestAction { get; set; }

		protected override Dictionary<string, Action<object>> GetHuePropertySetters()
		{
			return new Dictionary<string, Action<object>>()
			{
				{ "name", newValue => Name = ConvertHueValue<string>(newValue) },
			};
		}

		protected override Dictionary<string, FieldGetterSetterPair> GetFieldGetterSetterPairs()
		{
			return new Dictionary<string, FieldGetterSetterPair>()
			{
				{
					"UniqueId", new FieldGetterSetterPair<string>()
					{
						Getter = () => UniqueId,
						Setter = newValue => UniqueId = newValue
					}
				},
				{
					"Type", new FieldGetterSetterPair<string>()
					{
						Getter = () => Type,
						Setter = newValue => Type = newValue
					}
				},
				{
					"Name", new FieldGetterSetterPair<string>()
					{
						Getter = () => _name,
						Setter = newValue => _name = newValue
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
					"SwVersion", new FieldGetterSetterPair<string>()
					{
						Getter = () => SwVersion,
						Setter = newValue => SwVersion = newValue
					}
				},
				{
					"Manufacturer", new FieldGetterSetterPair<string>()
					{
						Getter = () => Manufacturer,
						Setter = newValue => Manufacturer = newValue
					}
				},
				{
					"LuminaireUniqueId", new FieldGetterSetterPair<string>()
					{
						Getter = () => LuminaireUniqueId,
						Setter = newValue => LuminaireUniqueId = newValue
					}
				},
				{
					"State", new FieldGetterSetterPair<State>()
					{
						Getter = () => State,
						Setter = newValue => State = newValue
					}
				},
			};
		}

		#endregion Property Updating

		#region Public Properties

		/// <summary>
		/// Unique id of the device. The MAC address of the device with a unique endpoint id in the form: AA:BB:CC:DD:EE:FF:00:11-XX
		/// </summary>
		[JsonProperty("uniqueid")]
		public string UniqueId { get; private set; }

		/// <summary>
		/// A fixed name describing the type of light e.g. “Extended color light”.
		/// <para>
		/// It is recommended that the <see cref="Capabilities"/> property be used to determine the properties on this light bulb that are valid.
		/// </para>
		/// </summary>
		[JsonProperty("type")]
		public string Type
		{
			get { return _type; }
			private set
			{
				_type = value;
				LightBulbCapabilities capabilities = LightBulbCapabilities.None;
				switch (_type.ToLower())
				{
					case "dimmable light":
						capabilities = LightBulbCapabilities.OnOff | LightBulbCapabilities.Dimmable;
						break;
					case "color temperature light":
						capabilities = LightBulbCapabilities.OnOff | LightBulbCapabilities.ColorTemperature;
						break;
					case "color light":
						capabilities = LightBulbCapabilities.OnOff | LightBulbCapabilities.Dimmable | LightBulbCapabilities.Color;
						break;
					case "extended color light":
						capabilities = LightBulbCapabilities.OnOff | LightBulbCapabilities.Dimmable | LightBulbCapabilities.ColorTemperature | LightBulbCapabilities.Color;
						break;
				}

				Capabilities = capabilities;
			}
		}

		[JsonIgnore]
		public LightBulbCapabilities Capabilities { get; private set; }

		[JsonIgnore]
		public Bool SupportsOnOff
		{
			get
			{
				return PlatformConverter.ToPlatformBool((Capabilities & LightBulbCapabilities.OnOff) == LightBulbCapabilities.OnOff);
			}
		}

		[JsonIgnore]
		public Bool SupportsDimming
		{
			get
			{
				return PlatformConverter.ToPlatformBool((Capabilities & LightBulbCapabilities.Dimmable) == LightBulbCapabilities.Dimmable);
			}
		}

		[JsonIgnore]
		public Bool SupportsColorTemperature
		{
			get
			{
				return PlatformConverter.ToPlatformBool((Capabilities & LightBulbCapabilities.ColorTemperature) == LightBulbCapabilities.ColorTemperature);
			}
		}

		[JsonIgnore]
		public Bool SupportsColor
		{
			get
			{
				return PlatformConverter.ToPlatformBool((Capabilities & LightBulbCapabilities.Color) == LightBulbCapabilities.Color);
			}
		}

		/// <summary>
		/// A unique, editable name given to the light.
		/// </summary>
		[JsonProperty("name")]
		public string Name
		{
			get { return _name; }
			set { SetHuePropertyRequest("Name", "name", ref _name, value); }
		}

		/// <summary>
		/// The hardware model of the light.
		/// </summary>
		[JsonProperty("modelid")]
		public string ModelId { get; private set; }

		/// <summary>
		/// An identifier for the software version running on the light.
		/// </summary>
		[JsonProperty("swversion")]
		public string SwVersion { get; private set; }

		/// <summary>
		/// The manufacturer name.
		/// </summary>
		[JsonProperty("manufacturername")]
		public string Manufacturer { get; private set; }

		/// <summary>
		/// Unique ID of the luminaire the light is a part of in the format: AA:BB:CC:DD-XX-YY.
		/// AA:BB:, ... represents the hex of the luminaireid, XX the lightsource position (incremental but may contain gaps) 
		/// and YY the lightpoint position (index of light in luminaire group).   A gap in the lightpoint position indicates an 
		/// incomplete luminaire (light search required to discover missing light points in this case).
		/// </summary>
		[JsonProperty("luminaireuniqueid")]
		public string LuminaireUniqueId { get; private set; }

		/// <summary>
		/// Details the state of the light; See <see cref="State"/> for more details
		/// </summary>
		[JsonProperty("state")]
		internal State State
		{
			get { return _state; }
			private set
			{
				if (_state != null)
					_state.PropertyChanged -= State_PropertyChanged;
				_state = value;
				if (_state != null)
					_state.PropertyChanged += State_PropertyChanged;
			}
		}

		private void State_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged(e.PropertyName);
			if (e.PropertyName == "Brightness")
				NotifyPropertyChanged("BrightnessPercentage");
			else if (e.PropertyName == "Saturation")
				NotifyPropertyChanged("SaturationPercentage");
			else if (e.PropertyName == "ColorTemperature")
				NotifyPropertyChanged("KnownColorTemperature");
		}

		#region State Properties

		/// <summary>
		/// Indicates if a light can be reached by the bridge
		/// </summary>
		[JsonIgnore]
		public Bool IsOnline
		{
			get { return PlatformConverter.ToPlatformBool(State.IsOnline); }
			private set { State.IsOnline = PlatformConverter.ToBool(value); }
		}

		/// <summary>
		/// On/Off state of the light. On=true, Off=false
		/// </summary>
		[JsonIgnore]
		public Bool On
		{
			get { return PlatformConverter.ToPlatformBool(State.On); }
			set 
			{
				if (!PlatformConverter.ToBool(value))
					StopCycles();

				State.On = PlatformConverter.ToBool(value); 
			}
		}

		/// <summary>
		/// Brightness of the light. This is a scale from the minimum brightness the light is capable of, 1, to the maximum capable brightness, 254.
		/// <para>
		/// NOTE: A brightness of 1 is not off.
		/// </para>
		/// </summary>
		[JsonIgnore]
		public ushort Brightness
		{
			get { return State.Brightness; }
			set 
			{
				if (PlatformConverter.ToBool(RaiseInProgress))
					StopRaise();
				if (PlatformConverter.ToBool(LowerInProgress))
					StopLower();
				if (PlatformConverter.ToBool(CycleDimInProgress))
					StopCycleDim();

				State.Brightness = value; 
			}
		}

		[JsonIgnore]
		public ushort BrightnessPercentage
		{
			get { return (ushort)((double)State.Brightness / BridgeClient.MaxLightBulbBrightness * 100f); }
			set 
			{
				if (PlatformConverter.ToBool(RaiseInProgress))
					StopRaise();
				if (PlatformConverter.ToBool(LowerInProgress))
					StopLower();
				if (PlatformConverter.ToBool(CycleDimInProgress))
					StopCycleDim();

				State.Brightness = (ushort)(value / 100f * BridgeClient.MaxLightBulbBrightness); 
			}
		}


		/// <summary>
		/// Hue of the light. This is a wrapping value between 0 and 65535. Both 0 and 65535 are red, 25500 is green and 46920 is blue.
		/// </summary>
		[JsonIgnore]
		public ushort Hue
		{
			get { return State.Hue; }
			set 
			{
				if (PlatformConverter.ToBool(CycleHueInProgress))
					StopCycleHue();

				State.Hue = value; 
			}
		}

		/// <summary>
		/// Saturation of the light. 254 is the most saturated (colored) and 0 is the least saturated (white).
		/// </summary>
		[JsonIgnore]
		public ushort Saturation
		{
			get { return State.Saturation; }
			set 
			{
				if (PlatformConverter.ToBool(CycleSaturationInProgress))
					StopCycleSaturation();

				State.Saturation = value; 
			}
		}

		[JsonIgnore]
		public ushort SaturationPercentage
		{
			get { return (ushort)((double)State.Saturation / BridgeClient.MaxLightBulbSaturation * 100f); }
			set 
			{
				if (PlatformConverter.ToBool(CycleSaturationInProgress))
					StopCycleSaturation();
 
				State.Saturation = (ushort)(value / 100f * BridgeClient.MaxLightBulbSaturation); 
			}
		}

		/// <summary>
		/// The x and y coordinates of a color in CIE color space (http://www.developers.meethue.com/documentation/core-concepts#color_gets_more_complicated)
		/// <para>
		/// The first entry is the x coordinate and the second entry is the y coordinate. Both x and y are between 0 and 1.
		/// </para>
		/// </summary>
		[JsonIgnore]
		// ReSharper disable once InconsistentNaming
		public float[] XY
		{
			get { return State.XY; }
			set { State.XY = value; }
		}

		/// <summary>
		/// The Mired Color temperature of the light (https://en.wikipedia.org/wiki/Mired); 
		/// 2012 connected lights are capable of 153 (6500K) to 500 (2000K).
		/// <para>
		/// A value of 370 is the default (a typical soft white bulb color)
		/// </para>
		/// </summary>
		[JsonIgnore]
		public ushort ColorTemperature
		{
			get { return State.ColorTemperature; }
			set 
			{
				if (PlatformConverter.ToBool(CycleColorTemperatureInProgress))
					StopCycleColorTemperature();

				State.ColorTemperature = value; 
			}
		}
		
		[JsonIgnore]
		public LightBulbColorTemperature KnownColorTemperature
		{
			get 
			{
				if (State.ColorTemperature == (ushort)LightBulbColorTemperature.Warm)
					return LightBulbColorTemperature.Warm;
				if (State.ColorTemperature == (ushort)LightBulbColorTemperature.Daylight)
					return LightBulbColorTemperature.Daylight;
				return LightBulbColorTemperature.Custom;
			}
			set 
			{
				if (PlatformConverter.ToBool(CycleColorTemperatureInProgress))
					StopCycleColorTemperature();

				switch (value)
				{
					case LightBulbColorTemperature.Warm:
					case LightBulbColorTemperature.Daylight:
						State.ColorTemperature = (ushort)value;
						break;
				}				
			}
		}

		/// <summary>
		/// The alert effect, which is a temporary change to the bulb’s state.
		/// <para>
		/// Note that this contains the last alert sent to the light and not its current state
		/// (i.e. After the breathe cycle has finished the bridge does not reset the alert to "none")
		/// </para>
		/// </summary>
		[JsonIgnore]
		public LightBulbAlert Alert
		{
			get { return State.Alert; }
			set 
			{
				StopCycles();
				State.Alert = value; 
			}
		}

		/// <summary>
		/// The dynamic effect of the light
		/// <para>
		/// If set to <see cref="LightBulbEffect.ColorLoop"/>, the light will cycle through all hues using the current brightness and saturation settings
		/// </para>
		/// </summary>
		[JsonIgnore]
		public LightBulbEffect Effect
		{
			get { return State.Effect; }
			set 
			{
				StopCycles();
				State.Effect = value; 
			}
		}

		/// <summary>
		/// Indicates the color mode in which the light is working;  This is the last command type it received. 
		/// </summary>
		[JsonIgnore]
		public LightBulbColorMode ColorMode
		{
			get { return State.ColorMode; }
		}

		/// <summary>
		/// Indicates the color mode in which the light is working (string version of the property <see cref="ColorMode"/>); 
		/// This is the last command type it received. 
		/// <para>
		/// Values are “hs” for Hue and Saturation, “xy” for XY and “ct” for Color Temperature. 
		/// </para>
		/// </summary>
		[JsonIgnore]
		public string ColorModeKey
		{
			get { return State.ColorModeKey; }
		}

		#endregion State Properties

		/// <summary>
		/// Time of the transition from the light’s current state to the new state (in 100ms multiples) 
		/// (e.g. 10 = 1 second)
		/// <para>
		/// If this is less than 0, the client's property <see cref="BridgeClient.LightBulbsDefaultTransitionTime"/> is used or, if that is not set either, 
		/// the Philips Hue default is used, which is 400ms or 0.4 seconds.
		/// </para>
		/// </summary>
		[JsonIgnore]
		public short TransitionTime
		{
			get { return _transitionTime; }
			set { UpdateProperty("TransitionTime", ref _transitionTime, value); }
		}

		#endregion Public Properties

		#region Public Methods

		internal Action RefreshRequestAction { get; set; }

		public void Refresh()
		{
			Log(DebugLevel.Info, "Request refresh {0}", Name);
			RefreshRequestAction();
		}

		public void SetProperties(LightBulbProperties lightBulbPropertiesToSet)
		{
			if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetBrightness) && PlatformConverter.ToBool(RaiseInProgress))
				StopRaise();
			if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetBrightness) && PlatformConverter.ToBool(LowerInProgress))
				StopLower();
			if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetBrightness) && PlatformConverter.ToBool(CycleDimInProgress))
				StopCycleDim();
			if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetHue) && PlatformConverter.ToBool(CycleHueInProgress))
				StopCycleHue();
			if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetSaturation) && PlatformConverter.ToBool(CycleSaturationInProgress))
				StopCycleSaturation();
			if (PlatformConverter.ToBool(lightBulbPropertiesToSet.SetColorTemperature) && PlatformConverter.ToBool(CycleColorTemperatureInProgress))
				StopCycleColorTemperature();

			SetHuePropertiesRequestAction(lightBulbPropertiesToSet);
		}

		public void Toggle()
		{
			State.On = !State.On;
		}

		public void InstantOn()
		{
			short savedTime = _transitionTime;
			try
			{
				_transitionTime = 0;

				// See InstantOff method as to why we have to set the brightness.
				// In addition, sending Brightness at the same time as On with a 0 transition time
				// seems to not set the Brightness.  The response only says it set the bulb to On.
				// So, we will send the Brightness here only, which gets the result we want.
				//State.On = true;
				State.Brightness = BridgeClient.MaxLightBulbBrightness;
			}
			finally
			{
				_transitionTime = savedTime;
			}
		}

		public void InstantOff()
		{
			short savedTime = _transitionTime;
			try
			{
				_transitionTime = 0;
				State.On = false;

				// For some reason, the Brightness changes to 1 when we do this with 0 transition time.
				// It doesn't happen when you just toggle the bulb with the default transition time.
				// Refresh to ensure we have the latest values so when you call InstantOn, we know the brightness needs to change.
				Refresh();
			}
			finally
			{
				_transitionTime = savedTime;
			}
		}

		public void Raise(ushort brightnessChange)
		{
			SetProperties(new LightBulbProperties()
			{
				SetBrightnessOffset = PlatformConverter.ToPlatformBool(true),
				BrightnessOffset = (short)Math.Max(
					(ushort)(BridgeClient.MaxLightBulbBrightness / 100.0), 
					Math.Min(
						brightnessChange, 
						(ushort)short.MaxValue))
			});
		}

		public void RaisePercentage(ushort brightnessPercentageChange)
		{
			Raise((ushort)(Math.Max((ushort)1, brightnessPercentageChange) / 100.0 * BridgeClient.MaxLightBulbBrightness));
		}

		public void Lower(ushort brightnessChange)
		{
			SetProperties(new LightBulbProperties()
			{
				SetBrightnessOffset = PlatformConverter.ToPlatformBool(true),
				BrightnessOffset = (short)(-1 * (short)Math.Max(
					                           (ushort)(BridgeClient.MaxLightBulbBrightness / 100.0), 
					                           Math.Min(
						                           brightnessChange, 
						                           (ushort)short.MaxValue)))
			});
		}

		public void LowerPercentage(ushort brightnessPercentageChange)
		{
			Lower((ushort)(Math.Max((ushort)1, brightnessPercentageChange) / 100.0 * BridgeClient.MaxLightBulbBrightness));
		}

		#region Raise Start/Stop

		private bool _raiseInProgress;
		[JsonIgnore]
		public Bool RaiseInProgress
		{
			get { return PlatformConverter.ToPlatformBool(_raiseInProgress); }
			private set { UpdateProperty("RaiseInProgress", ref _raiseInProgress, PlatformConverter.ToBool(value)); }
		}

		private ushort _raiseTime = 2000;
		[JsonIgnore]
		public ushort RaiseTime
		{
			get { return _raiseTime; }
			set { UpdateProperty("RaiseTime", ref _raiseTime, value); }
		}

		private CTimer _raiseTimer;

		public void StartRaise()
		{
			if (PlatformConverter.ToBool(RaiseInProgress))
				return;

			if (PlatformConverter.ToBool(LowerInProgress))
				StopLower();
			if (PlatformConverter.ToBool(CycleDimInProgress))
				StopCycleDim();

			if (!State.On)
				State.Brightness = BridgeClient.MinLightBulbBrightness;

			StartCycleProperty(
				() => _raiseTimer,
				timer => _raiseTimer = timer,
				_raiseTime,
				() => true,
				direction => { },
				inProgress => RaiseInProgress = inProgress,
				() => Brightness,
				brightness => State.Brightness = brightness,
				BridgeClient.MinLightBulbBrightness,
				BridgeClient.MaxLightBulbBrightness);

		}

		public void StopRaise()
		{
			if (!PlatformConverter.ToBool(RaiseInProgress))
				return;

			bool? raiseDirection = true;
			StopCycleProperty(
				ref _raiseTimer,
				ref raiseDirection,
				inProgress => RaiseInProgress = inProgress);
		}

		#endregion Raise Start/Stop

		#region Lower Start/Stop

		private bool _lowerInProgress;
		[JsonIgnore]
		public Bool LowerInProgress
		{
			get { return PlatformConverter.ToPlatformBool(_lowerInProgress); }
			private set { UpdateProperty("LowerInProgress", ref _lowerInProgress, PlatformConverter.ToBool(value)); }
		}

		private ushort _lowerTime = 2000;
		[JsonIgnore]
		public ushort LowerTime
		{
			get { return _lowerTime; }
			set { UpdateProperty("LowerTime", ref _lowerTime, value); }
		}

		private CTimer _lowerTimer;

		public void StartLower()
		{
			if (PlatformConverter.ToBool(LowerInProgress))
				return;

			if (PlatformConverter.ToBool(RaiseInProgress))
				StopRaise();
			if (PlatformConverter.ToBool(CycleDimInProgress))
				StopCycleDim();

			StartCycleProperty(
				() => _lowerTimer,
				timer => _lowerTimer = timer,
				_lowerTime,
				() => false,
				direction =>
				{
					if (State.Brightness <= BridgeClient.MinLightBulbBrightness)
						State.On = false;
				},
				inProgress => LowerInProgress = inProgress,
				() => Brightness,
				brightness => State.Brightness = brightness,
				BridgeClient.MinLightBulbBrightness,
				BridgeClient.MaxLightBulbBrightness);

		}

		public void StopLower()
		{
			if (!PlatformConverter.ToBool(LowerInProgress))
				return;

			bool? lowerDirection = true;
			StopCycleProperty(
				ref _lowerTimer,
				ref lowerDirection,
				inProgress => LowerInProgress = inProgress);
		}

		#endregion Lower Start/Stop

		#region Cycle Dim

		private bool _cycleDimInProgress;
		[JsonIgnore]
		public Bool CycleDimInProgress
		{
			get { return PlatformConverter.ToPlatformBool(_cycleDimInProgress); }
			private set { UpdateProperty("CycleDimInProgress", ref _cycleDimInProgress, PlatformConverter.ToBool(value)); }
		}

		private ushort _cycleDimTime = 2000;
		[JsonIgnore]
		public ushort CycleDimTime
		{
			get { return _cycleDimTime; }
			set { UpdateProperty("CycleDimTime", ref _cycleDimTime, value); }
		}

		private bool? _cycleDimDirection;
		private CTimer _cycleDimTimer;

		public void StartCycleDim()
		{
			if (PlatformConverter.ToBool(CycleDimInProgress))
				return;

			if (PlatformConverter.ToBool(RaiseInProgress))
				StopRaise();
			if (PlatformConverter.ToBool(LowerInProgress))
				StopLower();

			if (!State.On && _cycleDimDirection != true)
				State.Brightness = BridgeClient.MinLightBulbBrightness;

			StartCycleProperty(
				() => _cycleDimTimer,
				timer => _cycleDimTimer = timer,
				_cycleDimTime,
				() => _cycleDimDirection,
				direction => _cycleDimDirection = direction,
				inProgress => CycleDimInProgress = inProgress,
				() => Brightness,
				brightness => State.Brightness = brightness,
				BridgeClient.MinLightBulbBrightness,
				BridgeClient.MaxLightBulbBrightness);

		}

		public void StopCycleDim()
		{
			if (!PlatformConverter.ToBool(CycleDimInProgress))
				return;

			StopCycleProperty(
				ref _cycleDimTimer,
				ref _cycleDimDirection,
				inProgress => CycleDimInProgress = inProgress);
		}

		#endregion Cycle Dim

		#region Cycle Color Temperature

		private bool _cycleColorTemperatureInProgress;
		[JsonIgnore]
		public Bool CycleColorTemperatureInProgress
		{
			get { return PlatformConverter.ToPlatformBool(_cycleColorTemperatureInProgress); }
			private set { UpdateProperty("CycleColorTemperatureInProgress", ref _cycleColorTemperatureInProgress, PlatformConverter.ToBool(value)); }
		}

		private ushort _cycleColorTemperatureTime = 2000;
		[JsonIgnore]
		public ushort CycleColorTemperatureTime
		{
			get { return _cycleColorTemperatureTime; }
			set { UpdateProperty("CycleColorTemperatureTime", ref _cycleColorTemperatureTime, value); }
		}

		private bool? _cycleColorTemperatureDirection;
		private CTimer _cycleColorTemperatureTimer;

		public void StartCycleColorTemperature()
		{
			if (PlatformConverter.ToBool(CycleColorTemperatureInProgress))
				return;

			StartCycleProperty(
				() => _cycleColorTemperatureTimer,
				timer => _cycleColorTemperatureTimer = timer,
				_cycleColorTemperatureTime,
				() => _cycleColorTemperatureDirection,
				direction => _cycleColorTemperatureDirection = direction,
				inProgress => CycleColorTemperatureInProgress = inProgress,
				() => ColorTemperature,
				colorTemperature => State.ColorTemperature = colorTemperature,
				BridgeClient.MinLightBulbColorTemperature,
				BridgeClient.MaxLightBulbColorTemperature);

		}

		public void StopCycleColorTemperature()
		{
			if (!PlatformConverter.ToBool(CycleColorTemperatureInProgress))
				return;

			StopCycleProperty(
				ref _cycleColorTemperatureTimer,
				ref _cycleColorTemperatureDirection,
				inProgress => CycleColorTemperatureInProgress = inProgress);
		}

		#endregion Cycle Color Temperature

		#region Cycle Hue

		private bool _cycleHueInProgress;
		[JsonIgnore]
		public Bool CycleHueInProgress
		{
			get { return PlatformConverter.ToPlatformBool(_cycleHueInProgress); }
			private set { UpdateProperty("CycleHueInProgress", ref _cycleHueInProgress, PlatformConverter.ToBool(value)); }
		}

		private ushort _cycleHueTime = 10000;
		[JsonIgnore]
		public ushort CycleHueTime
		{
			get { return _cycleHueTime; }
			set { UpdateProperty("CycleHueTime", ref _cycleHueTime, value); }
		}

		private bool? _cycleHueDirection;
		private CTimer _cycleHueTimer;

		public void StartCycleHue()
		{
			if (PlatformConverter.ToBool(CycleHueInProgress))
				return;

			StartCycleProperty(
				() => _cycleHueTimer,
				timer => _cycleHueTimer = timer,
				_cycleHueTime,
				() => _cycleHueDirection,
				direction => _cycleHueDirection = direction,
				inProgress => CycleHueInProgress = inProgress,
				() => Hue,
				hue => State.Hue = hue,
				BridgeClient.MinLightBulbHue,
				BridgeClient.MaxLightBulbHue);

		}

		public void StopCycleHue()
		{
			if (!PlatformConverter.ToBool(CycleHueInProgress))
				return;

			StopCycleProperty(
				ref _cycleHueTimer,
				ref _cycleHueDirection,
				inProgress => CycleHueInProgress = inProgress);
		}

		#endregion Cycle Hue

		#region Cycle Saturation

		private bool _cycleSaturationInProgress;
		[JsonIgnore]
		public Bool CycleSaturationInProgress
		{
			get { return PlatformConverter.ToPlatformBool(_cycleSaturationInProgress); }
			private set { UpdateProperty("CycleSaturationInProgress", ref _cycleSaturationInProgress, PlatformConverter.ToBool(value)); }
		}

		private ushort _cycleSaturationTime = 2000;
		[JsonIgnore]
		public ushort CycleSaturationTime
		{
			get { return _cycleSaturationTime; }
			set { UpdateProperty("CycleSaturationTime", ref _cycleSaturationTime, value); }
		}

		private bool? _cycleSaturationDirection;
		private CTimer _cycleSaturationTimer;

		public void StartCycleSaturation()
		{
			if (PlatformConverter.ToBool(CycleSaturationInProgress))
				return;

			StartCycleProperty(
				() => _cycleSaturationTimer,
				timer => _cycleSaturationTimer = timer,
				_cycleSaturationTime,
				() => _cycleSaturationDirection,
				direction => _cycleSaturationDirection = direction,
				inProgress => CycleSaturationInProgress = inProgress,
				() => Saturation,
				saturation => State.Saturation = saturation,
				BridgeClient.MinLightBulbSaturation,
				BridgeClient.MaxLightBulbSaturation);

		}

		public void StopCycleSaturation()
		{
			if (!PlatformConverter.ToBool(CycleSaturationInProgress))
				return;

			StopCycleProperty(
				ref _cycleSaturationTimer,
				ref _cycleSaturationDirection,
				inProgress => CycleSaturationInProgress = inProgress);
		}

		#endregion Cycle Saturation

		public void BeginTemporaryChange()
		{
			using (new LockScope(_classLock))
			{
				if (_beforeTemporaryChangeProperties != null)
					return;

				_beforeTemporaryChangeProperties = new LightBulbProperties();
				_beforeTemporaryChangeProperties.Brightness = Brightness;
				switch (ColorMode)
				{
					case LightBulbColorMode.HueSaturation:
						_beforeTemporaryChangeProperties.Hue = Hue;
						_beforeTemporaryChangeProperties.Saturation = Saturation;
						_beforeTemporaryChangeProperties.SetHue = PlatformConverter.ToPlatformBool(true);
						_beforeTemporaryChangeProperties.SetSaturation = PlatformConverter.ToPlatformBool(true);
						break;

					case LightBulbColorMode.ColorTemperature:
						_beforeTemporaryChangeProperties.ColorTemperature = ColorTemperature;
						_beforeTemporaryChangeProperties.SetColorTemperature = PlatformConverter.ToPlatformBool(true);
						break;

					case LightBulbColorMode.XY:
						_beforeTemporaryChangeProperties.XY = XY;
						_beforeTemporaryChangeProperties.SetXY = PlatformConverter.ToPlatformBool(true);
						break;
				}
			}
		}

		public void EndTemporaryChange()
		{
			LightBulbProperties beforeTemporaryChangeProperties = null;

			using (new LockScope(_classLock))
			{
				if (_beforeTemporaryChangeProperties == null)
					return;

				beforeTemporaryChangeProperties = _beforeTemporaryChangeProperties;
				_beforeTemporaryChangeProperties = null;
			}

			SetProperties(beforeTemporaryChangeProperties);
		}

		#endregion Public Methods

		#region Cycle Timers

		private void StopCycles()
		{
			if (PlatformConverter.ToBool(RaiseInProgress))
				StopRaise();
			if (PlatformConverter.ToBool(LowerInProgress))
				StopLower();
			if (PlatformConverter.ToBool(CycleDimInProgress))
				StopCycleDim();
			if (PlatformConverter.ToBool(CycleHueInProgress))
				StopCycleHue();
			if (PlatformConverter.ToBool(CycleSaturationInProgress))
				StopCycleSaturation();
			if (PlatformConverter.ToBool(CycleColorTemperatureInProgress))
				StopCycleColorTemperature();
		}

		private void StartCycleProperty(
			Func<CTimer> cycleTimerGetter,
			Action<CTimer> cycleTimerSetter,
			ushort cyclePropertyTime,
			Func<bool?> cycleDirectionGetter,
			Action<bool?> cycleDirectionSetter,
			Action<Bool> cyclePropertyInProgressSetter,
			Func<ushort> propertyGetter,
			Action<ushort> propertySetter,
			ushort minPropertyValue,
			ushort maxPropertyValue)
		{
			using (new LockScope(_classLock))
			{
				if (cycleTimerGetter() != null)
				{
					cycleTimerGetter().Dispose();
					cycleTimerSetter(null);
				}

				if (propertyGetter().CompareTo(minPropertyValue) < 0 || !PlatformConverter.ToBool(On))
					cycleDirectionSetter(true);
				else if (propertyGetter().CompareTo(maxPropertyValue) >= 0)
					cycleDirectionSetter(false);
				else if (cycleDirectionGetter() == null)
					cycleDirectionSetter(true);

				int repeatTime = (int)Math.Max(100.0f, (float)cyclePropertyTime / maxPropertyValue);
				// NOTE: Max 100 because Philips recommends no more than 10 commands per second be sent to a bulb

				ushort offset = (ushort)Math.Max(1.0f, maxPropertyValue / (cyclePropertyTime / repeatTime));

				cyclePropertyInProgressSetter(PlatformConverter.ToPlatformBool(true));

				CCriticalSection inProgressLock = new CCriticalSection();

				cycleTimerSetter(
				new CTimer(data =>
						{
							using (new LockScope(inProgressLock))
							{
								if (cycleTimerGetter() == null)
									return;

								bool direction = cycleDirectionGetter() == true;
								if (direction)
								{
									if (propertyGetter() >= maxPropertyValue)
									{
										using (new LockScope(_classLock))
										{
											if (cycleTimerGetter() != null)
											{
												cycleDirectionSetter(false);
												cycleTimerGetter().Dispose();
												cycleTimerSetter(null);
											}

											cyclePropertyInProgressSetter(PlatformConverter.ToPlatformBool(false));
										}
									}
									else
									{
										ushort adjustedOffset = (ushort) Math.Min(offset, maxPropertyValue - propertyGetter());
										propertySetter((ushort) (propertyGetter() + adjustedOffset));
									}
								}
								else
								{
									if (propertyGetter() <= minPropertyValue)
									{
										using (new LockScope(_classLock))
										{
											if (cycleTimerGetter() != null)
											{
												cycleDirectionSetter(true);
												cycleTimerGetter().Dispose();
												cycleTimerSetter(null);
											}

											cyclePropertyInProgressSetter(PlatformConverter.ToPlatformBool(false));
										}
									}
									else
									{
										ushort adjustedOffset = (ushort) Math.Min(offset, propertyGetter() - minPropertyValue);
										propertySetter((ushort) (propertyGetter() - adjustedOffset));
									}
								}
							}
						},
				5000 /*dummy value until Reset is called below since CTimer doesn't seem to respect the ctor parameter*/));

				cycleTimerGetter().Reset(0, (int)repeatTime);
			}
		}

		private void StopCycleProperty(
			ref CTimer cycleTimer,
			ref bool? cycleDirection,
			Action<Bool> cyclePropertyInProgressSetter)
		{
			using (new LockScope(_classLock))
			{
				if (cycleTimer != null)
				{
					cycleTimer.Dispose();
					cycleTimer = null;
				}

				if (cycleDirection != null)
					cycleDirection = !cycleDirection.Value;

				cyclePropertyInProgressSetter(PlatformConverter.ToPlatformBool(false));
			}
		}

		#endregion
	}
}