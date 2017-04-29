using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Softopoulos.Crestron.PhilipsHue
{
	public class Group : IdentifiableHueObject
	{
		internal const string Api = "groups";

		internal override string ApiName
		{
			get { return Api; }
		}

		private string _name;

		private Group()
		{
		}

		#region Property Updating

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
				"Name", new FieldGetterSetterPair<string>()
				{
					Getter = () => _name,
					Setter = newValue => _name = newValue
				}
			},          };
		}

		#endregion Property Updating

		[JsonProperty("name")]
		public string Name
		{
			get { return _name; }
			set { SetHuePropertyRequest("Name", "name", ref _name, value); }
		}

		///// <summary>
		///// Luminaire / Lightsource / LightGroup
		///// </summary>
		//[JsonConverter(typeof(StringEnumConverter))]
		//[JsonProperty("type")]
		//public GroupType Type { get; set; }

		///// <summary>
		///// Category of the Room type. Default is "Other".
		///// </summary>
		//[JsonConverter(typeof(StringEnumConverter))]
		//[JsonProperty("class")]
		//public RoomClass? Class { get; set; }

		///// <summary>
		///// As of 1.4. Uniquely identifies the hardware model of the luminaire. Only present for automatically created Luminaires.
		///// </summary>
		//[JsonProperty("modelid")]
		//public string ModelId { get; set; }

		///// <summary>
		///// The IDs of the lights that are in the group.
		///// </summary>
		//[JsonProperty("lights")]
		//public List<string> Lights { get; set; }

		///// <summary>
		///// The light state of one of the lamps in the group.
		///// </summary>
		//[JsonProperty("action")]
		//public State Action { get; set; }

		//[JsonProperty("state")]
		//public GroupState State { get; set; }
	}
}