using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Softopoulos.Crestron.Core.Diagnostics;

namespace Softopoulos.Crestron.PhilipsHue
{
	/// <summary>
	/// Represents the state of a <see cref="LightBulb"/> registered with the Hue Bridge;  
	/// See it used here: <see cref="LightBulb.State"/>
	/// </summary>
	internal class State : HueObject
	{
		internal const string Api = "state";

		internal override string ApiName
		{
			get { return Api; }
		}

		// Public property field
		private bool _on;
		private ushort _brightness;
		private ushort _hue;
		private ushort _saturation;
		private float[] _xy;
		private ushort _colorTemperature;
		private LightBulbAlert _alert;
		private LightBulbEffect _effect;

		// Internal set fields
		private string _colorModeKey;
		private LightBulbColorMode _colorMode;
		private bool _isOnline;

		private State()
		{
		}

		#region Property Updating

		internal new LightBulbSetStatePropertyRequestAction SetHuePropertyRequestAction { get; set; }

		protected override Dictionary<string, Action<object>> GetHuePropertySetters()
		{
			return new Dictionary<string, Action<object>>()
			{
				{ "on", newValue => On = ConvertHueValue<bool>(newValue) },
				{ "bri", newValue => Brightness = ConvertHueValue<ushort>(newValue) },
				{ "hue", newValue => Hue = ConvertHueValue<ushort>(newValue) },
				{ "sat", newValue => Saturation = ConvertHueValue<ushort>(newValue) },
				{ "xy", newValue =>
				{
					var jArray = (JArray)newValue;
					JToken[] tokenArray = jArray.ToArray();
					XY = new[]
					{
						(float)Convert.ChangeType(((JValue)tokenArray[0]).Value, typeof(float), null),
						(float)Convert.ChangeType(((JValue)tokenArray[1]).Value, typeof(float), null)
					};
				}},
				{ "ct", newValue => ColorTemperature = ConvertHueValue<ushort>(newValue) },
				{ "alert", newValue => Alert = (LightBulbAlert)Enum.Parse(typeof(LightBulbAlert), (string)newValue, true) },
				{ "effect", newValue => Effect = (LightBulbEffect)Enum.Parse(typeof(LightBulbEffect), (string)newValue, true) },
				{ "colormode", newValue => ColorModeKey = ConvertHueValue<string>(newValue) },
				{ "reachable", newValue => IsOnline = ConvertHueValue<bool>(newValue) },
			};
		}

		protected override Dictionary<string, FieldGetterSetterPair> GetFieldGetterSetterPairs()
		{
			return new Dictionary<string, FieldGetterSetterPair>()
			{
				{
					"On", new FieldGetterSetterPair<bool>()
					{
						Getter = () => _on,
						Setter = newValue => _on = newValue
					}
				},
				{
					"Brightness", new FieldGetterSetterPair<ushort>()
					{
						Getter = () => _brightness,
						Setter = newValue => _brightness = newValue
					}
				},
				{
					"Hue", new FieldGetterSetterPair<ushort>()
					{
						Getter = () => _hue,
						Setter = newValue => _hue = newValue
					}
				},
				{
					"Saturation", new FieldGetterSetterPair<ushort>()
					{
						Getter = () => _saturation,
						Setter = newValue => _saturation = newValue
					}
				},

				// NOTE: Purposely left out XY here; We handle that manually in our override of the UpdateFrom method.

				{
					"ColorTemperature", new FieldGetterSetterPair<ushort>()
					{
						Getter = () => _colorTemperature,
						Setter = newValue => _colorTemperature = newValue
					}
				},
				{
					"Alert", new FieldGetterSetterPair<LightBulbAlert>()
					{
						Getter = () => _alert,
						Setter = newValue => _alert = newValue
					}
				},
				{
					"Effect", new FieldGetterSetterPair<LightBulbEffect>()
					{
						Getter = () => _effect,
						Setter = newValue => _effect = newValue
					}
				},
				{
					"ColorModeKey", new FieldGetterSetterPair<string>()
					{
						Getter = () => _colorModeKey,
						Setter = newValue => _colorModeKey = newValue
					}
				},
				{
					"ColorMode", new FieldGetterSetterPair<LightBulbColorMode>()
					{
						Getter = () => _colorMode,
						Setter = newValue => _colorMode = newValue
					}
				},
				{
					"IsOnline", new FieldGetterSetterPair<bool>()
					{
						Getter = () => _isOnline,
						Setter = newValue => _isOnline = newValue
					}
				},
			};
		}


		protected new void SetHuePropertyRequest<T>(string propertyName, string huePropertyName, ref T field, T newValue)
		{
			SetHuePropertyRequest(propertyName, huePropertyName, ref field, newValue, false, null);
		}

		protected void SetHuePropertyRequest<T>(string propertyName, string huePropertyName, ref T field, T newValue, bool turnOn)
		{
			SetHuePropertyRequest(propertyName, huePropertyName, ref field, newValue, turnOn, null);
		}

		protected void SetHuePropertyRequest<T>(string propertyName, string huePropertyName, ref T field, T newValue, bool turnOn, LightBulbColorMode? newColorMode)
		{
			if (SetHuePropertyRequestAction == null || SetHuePropertyRequestAction(huePropertyName, newValue, turnOn, newColorMode))
				field = newValue;

			NotifyPropertyChanged(propertyName);
		}

		internal override bool UpdateFrom(HueObject hueObject)
		{
			State state = hueObject as State;
			if (state == null)
				return false;

			bool anyChanged = base.UpdateFrom(hueObject);

			if (_xy[0] != state.XY[0] || _xy[1] != state.XY[1])
			{
				if (IsDeserialized)
					Log(DebugLevel.Debug, "Update XY to ({0},{1})", state.XY[0], state.XY[1]);

				_xy = state.XY;
				NotifyPropertyChanged("XY");
				anyChanged = true;
			}

			return anyChanged;
		}

		#endregion Property Updating

		/// <summary>
		/// See <see cref="LightBulb.IsOnline"/>
		/// </summary>
		[JsonProperty("reachable")]
		public bool IsOnline
		{
			get { return _isOnline; }
			internal set { UpdateProperty("IsOnline", ref _isOnline, value); }
		}

		/// <summary>
		/// See <see cref="LightBulb.On"/>
		/// </summary>
		[JsonProperty("on")]
		public bool On
		{
			get { return _on; }
			set { SetHuePropertyRequest("On", "on", ref _on, value); }
		}

		/// <summary>
		/// See <see cref="LightBulb.Brightness"/>
		/// </summary>
		[JsonProperty("bri")]
		public ushort Brightness
		{
			get { return _brightness; }
			set { SetHuePropertyRequest("Brightness", "bri", ref _brightness, Math.Min(value, BridgeClient.MaxLightBulbBrightness), true); }
		}

		/// <summary>
		/// See <see cref="LightBulb.Hue"/>
		/// </summary>
		[JsonProperty("hue")]
		public ushort Hue
		{
			get { return _hue; }
			set { SetHuePropertyRequest("Hue", "hue", ref _hue, value, true, LightBulbColorMode.HueSaturation); }
		}

		/// <summary>
		/// See <see cref="LightBulb.Saturation"/>
		/// </summary>
		[JsonProperty("sat")]
		public ushort Saturation
		{
			get { return _saturation; }
			set { SetHuePropertyRequest("Saturation", "sat", ref _saturation, Math.Min(value, BridgeClient.MaxLightBulbSaturation), true, LightBulbColorMode.HueSaturation); }
		}


		/// <summary>
		/// See <see cref="LightBulb.XY"/>
		/// </summary>
		[JsonProperty("xy")]
		// ReSharper disable once InconsistentNaming
		public float[] XY
		{
			get { return _xy; }
			set { SetHuePropertyRequest("XY", "xy", ref _xy, value, true, LightBulbColorMode.XY); }
		}

		/// <summary>
		/// See <see cref="LightBulb.ColorTemperature"/>
		/// </summary>
		[JsonProperty("ct")]
		public ushort ColorTemperature
		{
			get { return _colorTemperature; }
			set { SetHuePropertyRequest("ColorTemperature", "ct", ref _colorTemperature, Math.Max(BridgeClient.MinLightBulbColorTemperature, Math.Min(value, BridgeClient.MaxLightBulbColorTemperature)), true, LightBulbColorMode.ColorTemperature); }
		}

		/// <summary>
		/// See <see cref="LightBulb.Alert"/>
		/// </summary>
		[JsonProperty("alert")]
		[JsonConverter(typeof(StringEnumConverter))]
		public LightBulbAlert Alert
		{
			get { return _alert; }
			set { SetHuePropertyRequest("Alert", "alert", ref _alert, value); }
		}

		/// <summary>
		/// See <see cref="LightBulb.Effect"/>
		/// </summary>
		[JsonProperty("effect")]
		[JsonConverter(typeof(StringEnumConverter))]
		public LightBulbEffect Effect
		{
			get { return _effect; }
			set { SetHuePropertyRequest("Effect", "effect", ref _effect, value); }
		}

		/// <summary>
		/// See <see cref="LightBulb.ColorMode"/>
		/// </summary>
		[JsonIgnore]
		public LightBulbColorMode ColorMode
		{
			get { return _colorMode; }
			internal set
			{
				if (UpdateProperty("ColorMode", ref _colorMode, value))
				{
					if (_colorMode == LightBulbColorMode.NotSupported)
					{
						ColorModeKey = null;
					}
					else
					{
						switch (_colorMode)
						{
							case LightBulbColorMode.HueSaturation:
								ColorModeKey = "hs";
								break;

							case LightBulbColorMode.ColorTemperature:
								ColorModeKey = "ct";
								break;

							case LightBulbColorMode.XY:
								ColorModeKey = "xy";
								break;

							//case LightBulbColorMode.None:
							default:
								ColorModeKey = "";
								break;
						}
					}
				}
			}
		}

		/// <summary>
		/// See <see cref="LightBulb.ColorModeKey"/>
		/// </summary>
		[JsonProperty("colormode")]
		public string ColorModeKey
		{
			get { return _colorModeKey; }
			internal set
			{
				if (UpdateProperty("ColorModeKey", ref _colorModeKey, value))
				{
					if (_colorModeKey == null)
					{
						ColorMode = LightBulbColorMode.NotSupported;
					}
					else
					{
						switch (_colorModeKey.ToLower())
						{
							case "hs":
								ColorMode = LightBulbColorMode.HueSaturation;
								break;

							case "ct":
								ColorMode = LightBulbColorMode.ColorTemperature;
								break;

							case "xy":
								ColorMode = LightBulbColorMode.XY;
								break;

							default:
								ColorMode = LightBulbColorMode.None;
								break;
						}
					}
				}
			}
		}
	}
}