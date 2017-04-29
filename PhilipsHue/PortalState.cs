using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using Bool = System.Int16;
using Softopoulos.Crestron.Core;


namespace Softopoulos.Crestron.PhilipsHue
{
	/// <summary>
	/// Represets the state of the bridge's connection with the Hue portal (used to download software updates);  See it used here: <see cref="BridgeConfiguration.PortalState"/>
	/// </summary>
	public class PortalState : HueObject
	{
		internal const string Api = "config/portalstate";

		internal override string ApiName
		{
			get { return Api; }
		}

		private bool _signedOn;
		private bool _incoming;
		private bool _outgoing;

		private PortalState()
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
					"SignedOn", new FieldGetterSetterPair<Bool>()
					{
						Getter = () => SignedOn,
						Setter = newValue => SignedOn = newValue
					}
				},
				{
					"Incoming", new FieldGetterSetterPair<Bool>()
					{
						Getter = () => Incoming,
						Setter = newValue => Incoming = newValue
					}
				},
				{
					"Outgoing", new FieldGetterSetterPair<Bool>()
					{
						Getter = () => Outgoing,
						Setter = newValue => Outgoing = newValue
					}
				},
				{
					"Communication", new FieldGetterSetterPair<string>()
					{
						Getter = () => Communication,
						Setter = newValue => Communication = newValue
					}
				},
			};
		}

		#endregion Property Updating

		/// <summary>
		/// Indicates whether the bridge is signed on to the Hue portal;
		/// Only when this is true can an update be started.
		/// </summary>
		[JsonProperty("signedon")]
		public Bool SignedOn
		{
			get { return PlatformConverter.ToPlatformBool(_signedOn); }
			private set { _signedOn = PlatformConverter.ToBool(value); }
		}

		/// <summary>
		/// Not documented by Philips at this time
		/// </summary>
		[JsonProperty("incoming")]
		public Bool Incoming
		{
			get { return PlatformConverter.ToPlatformBool(_incoming); }
			private set { _incoming = PlatformConverter.ToBool(value); }
		}

		/// <summary>
		/// Not documented by Philips at this time
		/// </summary>
		[JsonProperty("outgoing")]
		public Bool Outgoing
		{
			get { return PlatformConverter.ToPlatformBool(_outgoing); }
			private set { _outgoing = PlatformConverter.ToBool(value); }
		}

		/// <summary>
		/// Not documented by Philips at this time
		/// </summary>
		[JsonProperty("communication")]
		public string Communication { get; private set; }
	}
}