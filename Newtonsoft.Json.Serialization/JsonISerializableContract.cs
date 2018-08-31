using System;

namespace Newtonsoft.Json.Serialization
{
	public class JsonISerializableContract : JsonContract
	{
		public ObjectConstructor<object> ISerializableCreator
		{
			get;
			set;
		}

		public JsonISerializableContract(Type underlyingType)
			: base(underlyingType)
		{
		}
	}
}
