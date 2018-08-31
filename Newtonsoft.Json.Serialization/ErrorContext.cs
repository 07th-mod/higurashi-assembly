using System;

namespace Newtonsoft.Json.Serialization
{
	public class ErrorContext
	{
		public Exception Error
		{
			get;
			private set;
		}

		public object OriginalObject
		{
			get;
			private set;
		}

		public object Member
		{
			get;
			private set;
		}

		public bool Handled
		{
			get;
			set;
		}

		internal ErrorContext(object originalObject, object member, Exception error)
		{
			OriginalObject = originalObject;
			Member = member;
			Error = error;
		}
	}
}
