using System;

namespace Newtonsoft.Json.ObservableSupport
{
	public class PropertyChangingEventArgs : EventArgs
	{
		public virtual string PropertyName
		{
			get;
			set;
		}

		public PropertyChangingEventArgs(string propertyName)
		{
			PropertyName = propertyName;
		}
	}
}
