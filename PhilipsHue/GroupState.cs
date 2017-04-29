using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Softopoulos.Crestron.PhilipsHue
{
	public class GroupState : HueObject
	{
		internal const string Api = "groups/state";

		internal override string ApiName
		{
			get { return Api; }
		}

		private GroupState()
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
					"AnyOn", new FieldGetterSetterPair<bool?>()
					{
						Getter = () => AnyOn,
						Setter = newValue => AnyOn = newValue
					}
				},
				{
					"AllOn", new FieldGetterSetterPair<bool?>()
					{
						Getter = () => AllOn,
						Setter = newValue => AllOn = newValue
					}
				},
			};
		}

		#endregion Property Updating

		[JsonProperty("any_on")]
		public bool? AnyOn { get; private set; }

		[JsonProperty("all_on")]
		public bool? AllOn { get; private set; }
	}
}