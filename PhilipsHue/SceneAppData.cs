using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Softopoulos.Crestron.PhilipsHue
{
	public class SceneAppData : HueObject
	{
		internal const string Api = "scenes/appdata";

		internal override string ApiName
		{
			get { return Api; }
		}

		private SceneAppData()
		{
		}

		#region Property Updating

		protected override Dictionary<string, Action<object>> GetHuePropertySetters()
		{
			return new Dictionary<string, Action<object>>()
			{
				{ "version", newValue => Version = ConvertHueValue<int?>(newValue) },
				{ "data", newValue => Data = ConvertHueValue<string>(newValue) },
			};
		}

		protected override Dictionary<string, FieldGetterSetterPair> GetFieldGetterSetterPairs()
		{
			return new Dictionary<string, FieldGetterSetterPair>()
			{
				{
					"Version", new FieldGetterSetterPair<int?>()
					{
						Getter = () => Version,
						Setter = newValue => Version = newValue
					}
				},
				{
					"Data", new FieldGetterSetterPair<string>()
					{
						Getter = () => Data,
						Setter = newValue => Data = newValue
					}
				},
			};
		}

		#endregion Property Updating

		[JsonProperty("version")]
		public int? Version { get; set; }
		[JsonProperty("data")]
		public string Data { get; set; }
	}
}