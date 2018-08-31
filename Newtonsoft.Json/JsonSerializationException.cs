using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json
{
	[Serializable]
	public class JsonSerializationException : Exception
	{
		public JsonSerializationException()
		{
		}

		public JsonSerializationException(string message)
			: base(message)
		{
		}

		public JsonSerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public JsonSerializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
