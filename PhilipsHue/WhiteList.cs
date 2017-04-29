using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Softopoulos.Crestron.PhilipsHue
{
	/// <summary>
	/// Represents a user authenticated to communicate with bridge;  See it used here: <see cref="BridgeConfiguration.WhiteList"/>
	/// </summary>
	public class WhiteList : HueObject
	{
		internal const string Api = "config/whitelist";

		internal override string ApiName
		{
			get { return Api; }
		}

		private WhiteList()
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
					"Id", new FieldGetterSetterPair<string>()
					{
						Getter = () => Id,
						Setter = newValue => Id = newValue
					}
				},
				{
					"LastUsedDate", new FieldGetterSetterPair<string>()
					{
						Getter = () => LastUsedDate,
						Setter = newValue => LastUsedDate = newValue
					}
				},
				{
					"CreateDate", new FieldGetterSetterPair<string>()
					{
						Getter = () => CreateDate,
						Setter = newValue => CreateDate = newValue
					}
				},
				{
					"Name", new FieldGetterSetterPair<string>()
					{
						Getter = () => Name,
						Setter = newValue => Name = newValue
					}
				},
			};
		}

		#endregion Property Updating

		[JsonProperty("id")]
		public string Id { get; private set; }

		[JsonProperty("last use date")]
		public string LastUsedDate { get; private set; }

		[JsonProperty("create date")]
		public string CreateDate { get; private set; }

		[JsonProperty("name")]
		public string Name { get; private set; }
	}
}