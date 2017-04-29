using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Softopoulos.Crestron.PhilipsHue
{
	/// <summary>
	/// State of software updates available to the bridge;  See <see cref="BridgeConfiguration.SoftwareUpdate"/> for more details.
	/// </summary>
	public class SoftwareUpdate : HueObject
	{
		internal const string Api = "config/swupdate";

		internal override string ApiName
		{
			get { return Api; }
		}

		private SoftwareUpdate()
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
					"UpdateState", new FieldGetterSetterPair<SoftwareUpdateState>()
					{
						Getter = () => UpdateState,
						Setter = newValue => PrivateUpdateState = (int)newValue
					}
				},
				{
					"Url", new FieldGetterSetterPair<string>()
					{
						Getter = () => Url,
						Setter = newValue => Url = newValue
					}
				},
				{
					"Text", new FieldGetterSetterPair<string>()
					{
						Getter = () => Text,
						Setter = newValue => Text = newValue
					}
				},
				{
					"Notify", new FieldGetterSetterPair<bool>()
					{
						Getter = () => Notify,
						Setter = newValue => Notify = newValue
					}
				},
				{
					"DeviceTypes", new FieldGetterSetterPair<SoftwareUpdateDeviceTypes>()
					{
						Getter = () => DeviceTypes,
						Setter = newValue => DeviceTypes = newValue
					}
				},
				{
					"CheckForUpdate", new FieldGetterSetterPair<bool>()
					{
						Getter = () => CheckForUpdate,
						Setter = newValue => CheckForUpdate = newValue
					}
				},
			};
		}

		#endregion Property Updating

		[JsonProperty("updatestate")]
		private int PrivateUpdateState { get; set; }

		/// <summary>
		/// Current state of a software update;
		/// The update can be started only when in state <see cref="SoftwareUpdateState.UpdateAvailableToApply"/>. 
		/// All other transitions are invalid and will return an error. 
		/// Performing an update should only be done with the consent of the end-user in the current application.
		/// </summary>
		public SoftwareUpdateState UpdateState
		{
			get { return (SoftwareUpdateState)PrivateUpdateState; }
		}

		/// <summary>
		/// URL to release notes
		/// </summary>
		[JsonProperty("url")]
		public string Url { get; private set; }

		/// <summary>
		/// Brief release note
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }

		/// <summary>
		/// The notify flag is set to true when software has been successfully installed. 
		/// This can be used to notify the user upon installation and should be reset to false when the user has seen this information.
		/// <para>
		/// NOTE: This API does not support this flag directly.
		/// Instead, use the method on the <see cref="BridgeClient"/> class <see cref="BridgeClient.BeginApplySoftwareUpdates"/> and listen for 
		/// the event <see cref="BridgeClient.ApplySoftwareUpdatesCompleted"/> to know when the update is complete.
		/// </para>
		/// </summary>
		[JsonProperty("notify")]
		internal bool Notify { get; set; }

		/// <summary>
		/// Details types of updates available; 
		/// Only present in update state <see cref="SoftwareUpdateState.UpdateAvailableToApply"/> and <see cref="SoftwareUpdateState.ApplyingUpdate"/>.
		/// </summary>
		[JsonProperty("devicetypes")]
		public SoftwareUpdateDeviceTypes DeviceTypes { get; private set; }

		/// <summary>
		/// Setting this flag to true lets the bridge search for software updates in the portal. 
		/// After the search attempt, this flag is set back to false. 
		/// Requires <see cref="BridgeConfiguration.PortalState"/> to be <see cref="PortalState.SignedOn"/>.
		/// <para>
		/// NOTE: This API does not support this flag directly.
		/// Instead, use the method on the <see cref="BridgeClient"/> class <see cref="BridgeClient.BeginCheckForSoftwareUpdates"/> and listen for 
		/// the event <see cref="BridgeClient.CheckForSoftwareUpdatesCompleted"/> to know when the check for updates is complete.
		/// </para>
		/// </summary>
		[JsonProperty("checkforupdate")]
		internal bool CheckForUpdate { get; set; }
	}
}