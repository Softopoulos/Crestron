using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Softopoulos.Crestron.PhilipsHue
{
	[Flags]
	public enum RefreshIntervalParts
	{
		None = 0,

		/// <summary>
		/// Refreshes the information from the bridge (see property <see cref="BridgeConfiguration"/>, including checking for software updates
		/// <para>
		/// Unlike the other objects, this only refreshed once a day.
		/// The desired refresh time can be set via the <see cref="BridgeClient.BridgeRefreshTime"/> property (total minutes from midnight)
		/// </para>
		/// </summary>
		Bridge = 0x01,

		Lights = 0x02,
		Groups = 0x04,
		Scenes = 0x08,
	}

	public enum BeginSearchForNewLightBulbsResult
	{
		Failed,
		Started
	}

	public enum SearchForNewLightBulbsResult
	{
		Failed,
		Completed
	}

	public enum BeginCheckForSoftwareUpdatesResult
	{
		Failed = 0,
		CheckOrUpdateAlreadyInProgress,
		OldBridgeMustUseOfficialHueApp,
		NotConnectedToHuePortal,
		BridgeIsBusyAndCannotCheckForUpdates,
		Started,
		UpdateAlreadyDownloading,
		UpdateAlreadyAvailable
	}

	public enum CheckForSoftwareUpdatesResult
	{
		Failed = 0,
		TimedOut,
		LostConnectingToHuePortal,
		Completed
	}

	public enum BeginApplySoftwareUpdatesResult
	{
		Failed = 0,
		CheckOrUpdateAlreadyInProgress,
		NoUpdateAvailable,
		UpdateStillDownloading,
		UpdateAlreadyBeingApplied,
		Started
	}

	public enum ApplySoftwareUpdatesResult
	{
		Failed = 0,
		TimedOutWaitingForUpdateToComplete,
		Completed
	}

	/// <summary>
	/// State of software updates available to the bridge; See it used here: <see cref="SoftwareUpdate.UpdateState"/>
	/// </summary>
	public enum SoftwareUpdateState
	{
		NoUpdate = 0,
		UpdateDownloading = 1,
		UpdateAvailableToApply = 2,
		ApplyingUpdate = 3
	}

	#region Light Bulb enums

	public enum LightBulbEffect
	{
		None,
		ColorLoop
	}

	public enum LightBulbAlert
	{
		/// <summary>
		/// The light is not performing an alert effect
		/// </summary>
		None,

		/// <summary>
		/// The light is performing one breathe cycle.
		/// </summary>
		Select,

		/// <summary>
		/// The light is performing breathe cycles for 15 seconds or until canceled
		/// </summary>
		LSelect
	}

	public enum LightBulbColorMode
	{
		NotSupported = 0,
		None = 1,
		HueSaturation = 2,
		ColorTemperature = 3,
		// ReSharper disable once InconsistentNaming
		XY = 4
	}

	[Flags]
	public enum LightBulbCapabilities
	{
		None = 0,
		OnOff = 0x01,
		Dimmable = 0x02,
		ColorTemperature = 0x04,
		Color = 0x08
	}

	public enum LightBulbsSortOrder
	{
		ByHueId = 0,
		ByName
	}

	public enum LightBulbColorTemperature
	{
		Custom = 0,
		Warm = 300,
		Daylight = 153
	}

	#endregion

	#region Group enums

	/// <summary>
	/// Possible group types
	/// </summary>
	public enum GroupType
	{
		LightGroup,
		Room,
		Luminaire,
		LightSource
	}

	/// <summary>
	/// Possible room types
	/// </summary>
	public enum RoomClass
	{
		Other,
		[JsonProperty("Living room")]
		LivingRoom,
		Kitchen,
		Dining,
		Bedroom,
		[JsonProperty("Kids bedroom")]
		KidsBedroom,
		Bathroom,
		Nursery,
		Recreation,
		Office,
		Gym,
		Hallway,
		Toilet,
		[JsonProperty("Front door")]
		FrontDoor,
		Garage,
		Terrace,
		Garden,
		Driveway,
		Carport
	}

	#endregion

}
