using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json
{
	[Serializable]
	public class JsonReaderException : Exception
	{
		public int LineNumber
		{
			get;
			private set;
		}

		public int LinePosition
		{
			get;
			private set;
		}

		public JsonReaderException()
		{
		}

		public JsonReaderException(string message)
			: base(message)
		{
		}

		public JsonReaderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public JsonReaderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal JsonReaderException(string message, Exception innerException, int lineNumber, int linePosition)
			: base(message, innerException)
		{
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}
	}
}
