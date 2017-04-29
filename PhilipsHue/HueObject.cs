using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using Softopoulos.Crestron.Core.ComponentModel;
using Softopoulos.Crestron.Core.Diagnostics;
using Crestron.SimplSharp;

namespace Softopoulos.Crestron.PhilipsHue
{
	public abstract class HueObject : ObservableObject
	{
		internal abstract string ApiName { get; }

		protected bool IsDeserialized { get; set; }

		[JsonIgnore]
		public ushort Index { get; internal set; }

		#region Locks

		protected CCriticalSection _classLock = new CCriticalSection();

		#endregion

		#region Logging

		internal LogAction LogAction { get; set; }

		protected void Log(DebugLevel debugLevel, string message, params object[] parameters)
		{
			LogAction(debugLevel, message, parameters);
		}

		#endregion

		internal void OnDeserialized()
		{
			IsDeserialized = true;
		}

		#region Property Updating

		internal bool SetHuePropertyRequestInProgress { get; set; }
		internal HueObjectSetPropertyRequestAction SetHuePropertyRequestAction { get; set; }
		protected void SetHuePropertyRequest<T>(string propertyName, string huePropertyName, ref T field, T newValue)
		{
			if (SetHuePropertyRequestAction == null ||
			    SetHuePropertyRequestAction(huePropertyName, newValue))
			{
				field = newValue;
			}

			NotifyPropertyChanged(propertyName);
		}

		private Dictionary<string, Action<object>> _huePropertySetters;
		protected abstract Dictionary<string, Action<object>> GetHuePropertySetters();
		internal void UpdateHueProperty(string huePropertyName, object value)
		{
			if (_huePropertySetters == null)
				_huePropertySetters = GetHuePropertySetters();

			Action<object> huePropertySetter;
			if (_huePropertySetters.TryGetValue(huePropertyName, out huePropertySetter))
			{
				huePropertySetter(value);
			}
		}

		protected T ConvertHueValue<T>(object valueFromHue)
		{
			if (valueFromHue is T)
				return (T)valueFromHue;

			return (T)Convert.ChangeType(valueFromHue, typeof(T), null);
		}

		protected override void SetField<T>(string propertyName, ref T field, T newValue)
		{
			if (IsDeserialized)
				Log(DebugLevel.Debug, "Update " + propertyName + " to " + (newValue != null ? newValue.ToString() : "null"));
			base.SetField(propertyName, ref field, newValue);
		}

		protected abstract class FieldGetterSetterPair
		{
			public abstract object Get();
			public abstract void Set(object newValue);
		}

		protected class FieldGetterSetterPair<T> : FieldGetterSetterPair
		{
			public override object Get() { return Getter(); }
			public override void Set(object newValue) { Setter((T)newValue); }

			public Func<T> Getter;
			public Action<T> Setter;
		}

		private Dictionary<string, FieldGetterSetterPair> _fieldGetterSetterPairs;
		protected abstract Dictionary<string, FieldGetterSetterPair> GetFieldGetterSetterPairs();

		internal virtual bool UpdateFrom(HueObject hueObject)
		{
			if (_fieldGetterSetterPairs == null)
				_fieldGetterSetterPairs = GetFieldGetterSetterPairs();

			if (hueObject._fieldGetterSetterPairs == null)
				hueObject._fieldGetterSetterPairs = hueObject.GetFieldGetterSetterPairs();

			bool anyChanged = false;

			foreach (var kv in _fieldGetterSetterPairs)
			{
				string propertyName = kv.Key;
				FieldGetterSetterPair getterSetter = kv.Value;

				object fromValue = hueObject._fieldGetterSetterPairs[propertyName].Get();
				if (fromValue is HueObject == false ||
				    getterSetter.Get() == null)
				{
					if (!Equals(getterSetter.Get(), fromValue))
					{
						if (IsDeserialized)
							Log(DebugLevel.Debug, "Update " + propertyName + " to " + (fromValue != null ? fromValue.ToString() : "null"));
						getterSetter.Set(fromValue);
						NotifyPropertyChanged(propertyName);
						anyChanged = true;
					}
				}
				else
				{
					if (((HueObject)getterSetter.Get()).UpdateFrom((HueObject)fromValue))
						anyChanged = true;
				}
			}

			return anyChanged;
		}

		#endregion Property Updating
	}
}