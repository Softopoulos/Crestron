using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Softopoulos.Crestron.Core.ComponentModel
{
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool UpdateProperty<T>(string propertyName, ref T field, T newValue)
		{
			if (!Equals(field, newValue))
			{
				SetField(propertyName, ref field, newValue);
				NotifyPropertyChanged(propertyName);
				return true;
			}

			return false;
		}

		protected bool UpdateAutomaticProperty<T>(string propertyName, Func<T> getter, Action<T> setter, T newValue)
		{
			if (!Equals(getter(), newValue))
			{
				setter(newValue);
				NotifyPropertyChanged(propertyName);
				return true;
			}

			return false;
		}

		protected virtual void SetField<T>(string propertyName, ref T field, T newValue)
		{
			field = newValue;
		}
	}

}