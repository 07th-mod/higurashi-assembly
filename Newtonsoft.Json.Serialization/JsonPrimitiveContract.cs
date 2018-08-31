using System;

namespace Newtonsoft.Json.Serialization
{
	public class JsonPrimitiveContract : JsonContract
	{
		public JsonPrimitiveContract(Type underlyingType)
			: base(underlyingType)
		{
		}
	}
}
