using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Schema
{
	[Serializable]
	public class JsonSchemaException : Exception
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

		public JsonSchemaException()
		{
		}

		public JsonSchemaException(string message)
			: base(message)
		{
		}

		public JsonSchemaException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public JsonSchemaException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal JsonSchemaException(string message, Exception innerException, int lineNumber, int linePosition)
			: base(message, innerException)
		{
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}
	}
}
