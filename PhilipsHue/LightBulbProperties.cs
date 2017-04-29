using System;
using System.Linq;

using Bool = System.Int16;
using Softopoulos.Crestron.Core;

namespace Softopoulos.Crestron.PhilipsHue
{
	public class LightBulbProperties
	{
		public LightBulbProperties()
		{
			TransitionTimeToUse = -1;
		}

		internal LightBulbProperties(LightBulb lightBulbFromWhichToSetAll)
		{
			Bool trueBool = PlatformConverter.ToPlatformBool(true);
			SetBrightness = trueBool;
			SetBrightnessPercentage = trueBool;
			SetHue = trueBool;
			SetSaturation = trueBool;
			SetSaturationPercentage = trueBool;
			SetXY = trueBool;
			SetColorTemperature = trueBool;

			Brightness = lightBulbFromWhichToSetAll.Brightness;
			BrightnessPercentage = lightBulbFromWhichToSetAll.BrightnessPercentage;
			Hue = lightBulbFromWhichToSetAll.Hue;
			Saturation = lightBulbFromWhichToSetAll.Saturation;
			SaturationPercentage = lightBulbFromWhichToSetAll.SaturationPercentage;
			XY = lightBulbFromWhichToSetAll.XY.ToArray();
			ColorTemperature = lightBulbFromWhichToSetAll.ColorTemperature;
		}

		public Bool SetBrightness { get; set; }
		public ushort Brightness { get; set; }
		public Bool SetBrightnessPercentage { get; set; }
		public ushort BrightnessPercentage { get; set; }
		public Bool SetHue { get; set; }
		public ushort Hue { get; set; }
		public Bool SetSaturation { get; set; }
		public ushort Saturation { get; set; }
		public Bool SetSaturationPercentage { get; set; }
		public ushort SaturationPercentage { get; set; }
		public Bool SetXY { get; set; }
		public float[] XY { get; set; }
		public Bool SetColorTemperature { get; set; }
		public ushort ColorTemperature { get; set; }
		public string ColorNameOrHexRgb { get; set; }

		public Bool SetBrightnessOffset { get; set; }
		public short BrightnessOffset { get; set; }
		public Bool SetHueOffset { get; set; }
		public int HueOffset { get; set; }
		public Bool SetSaturationOffset { get; set; }
		public short SaturationOffset { get; set; }
		public Bool SetXYOffset { get; set; }
		public float[] XYOffset { get; set; }
		public Bool SetColorTemperatureOffset { get; set; }
		public short ColorTemperatureOffset { get; set; }

		public short TransitionTimeToUse { get; set; }
	}
}