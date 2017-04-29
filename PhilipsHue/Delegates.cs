using System;
using Softopoulos.Crestron.Core.Diagnostics;

namespace Softopoulos.Crestron.PhilipsHue
{
	public delegate void ErrorOccurredEventHandler(object sender, ErrorOccurredEventArgs e);
	public delegate void CheckForSoftwareUpdatesCompletedEventHandler(object sender, CheckForSoftwareUpdatesCompletedEventArgs e);
	public delegate void ApplySoftwareUpdatesCompletedEventHandler(object sender, ApplySoftwareUpdatesCompletedEventArgs e);
	public delegate void FoundLightBulbEventHandler(object sender, FoundLightBulbEventArgs e);
	public delegate void SearchForNewLightBulbsCompletedEventHandler(object sender, SearchForNewLightBulbsCompletedEventArgs e);

	public delegate void BridgeConfigurationPropertiesChangedEventHandler(object sender, EventArgs e);
	public delegate void LightBulbPropertiesChangedEventHandler(object sender, LightBulbPropertiesChangedEventArgs e);
	public delegate void ScenePropertiesChangedEventHandler(object sender, ScenePropertiesChangedEventArgs e);

	internal delegate void LogAction(DebugLevel debugLevel, string message, object[] parameters);
	internal delegate bool HueObjectSetPropertyRequestAction(string huePropertyName, object value);

	internal delegate bool LightBulbSetPropertiesRequestAction(LightBulbProperties lightBulbStatePropertiesToSet);

	internal delegate bool LightBulbSetStatePropertyRequestAction(string huePropertyName, object value, bool turnOnLightBulb, LightBulbColorMode? newColorMode);
}
