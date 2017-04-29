using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using Softopoulos.Crestron.Core.Diagnostics;

namespace Softopoulos.Crestron.PhilipsHue
{
	public class Scene : IdentifiableHueObject
	{
		internal const string Api = "scenes";

		internal override string ApiName
		{
			get { return Api; }
		}

		private string _name;

		private Scene()
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
				},

				// NOTE: Purposely left out Lights property here; We handle that manually in our override of the UpdateFrom method.

				{
					"Owner", new FieldGetterSetterPair<string>()
					{
						Getter = () => Owner,
						Setter = newValue => Owner = newValue
					}
				},
				{
					"AppData", new FieldGetterSetterPair<SceneAppData>()
					{
						Getter = () => AppData,
						Setter = newValue => AppData = newValue
					}
				},
				{
					"Picture", new FieldGetterSetterPair<string>()
					{
						Getter = () => Picture,
						Setter = newValue => Picture = newValue
					}
				},
				{
					"Recycle", new FieldGetterSetterPair<bool?>()
					{
						Getter = () => Recycle,
						Setter = newValue => Recycle = newValue
					}
				},
				{
					"Locked", new FieldGetterSetterPair<bool?>()
					{
						Getter = () => Locked,
						Setter = newValue => Locked = newValue
					}
				},
				{
					"Version", new FieldGetterSetterPair<int?>()
					{
						Getter = () => Version,
						Setter = newValue => Version = newValue
					}
				},
				{
					"LastUpdated", new FieldGetterSetterPair<DateTime?>()
					{
						Getter = () => LastUpdated,
						Setter = newValue => LastUpdated = newValue
					}
				},
			};
		}

		internal override bool UpdateFrom(HueObject hueObject)
		{
			Scene scene = hueObject as Scene;
			if (scene == null)
				return false;

			bool anyChanged = base.UpdateFrom(hueObject);

			if (!Lights.SequenceEqual(scene.Lights))
			{
				if (IsDeserialized)
					Log(DebugLevel.Debug, "Update Scene.Lights to {0}", string.Join(",", scene.Lights));

				Lights = scene.Lights.ToArray();
				NotifyPropertyChanged("Lights");
				anyChanged = true;
			}

			return anyChanged;
		}

		#endregion Property Updating

		[JsonProperty("name")]
		public string Name
		{
			get { return _name; }
			set { SetHuePropertyRequest("Name", "name", ref _name, value); }
		}

		[JsonProperty("lights")]
		public string[] Lights { get; private set; }

		/// <summary>
		/// Whitelist user that created or modified the content of the scene. Note that changing name does not change the owner.
		/// </summary>
		[JsonProperty("owner")]
		public string Owner { get; private set; }

		/// <summary>
		/// App specific data linked to the scene.  Each individual application should take responsibility for the data written in this field.
		/// </summary>
		[JsonProperty("appdata")]
		public SceneAppData AppData { get; private set; }

		/// <summary>
		/// Only available on a GET of an individual scene resource (/api/[username]/scenes/[id]). Not available for scenes created via a PUT in version 1. . Reserved for future use.
		/// </summary>
		[JsonProperty("picture")]
		public string Picture { get; private set; }

		/// <summary>
		/// Indicates whether the scene can be automatically deleted by the bridge. Only available by POSTSet to 'false' when omitted. Legacy scenes created by PUT are defaulted to true. When set to 'false' the bridge keeps the scene until deleted by an application.
		/// </summary>
		[JsonProperty("recycle")]
		public bool? Recycle { get; private set; }

		/// <summary>
		/// Indicates that the scene is locked by a rule or a schedule and cannot be deleted until all resources requiring or that reference the scene are deleted.
		/// </summary>
		[JsonProperty("locked")]
		public bool? Locked { get; private set; }

		[JsonProperty("version")]
		public int? Version { get; private set; }

		[JsonProperty("lastupdated")]
		public DateTime? LastUpdated { get; private set; }
	}
}