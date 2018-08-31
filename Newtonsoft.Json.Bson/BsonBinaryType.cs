using System;

namespace Newtonsoft.Json.Bson
{
	internal enum BsonBinaryType : byte
	{
		Binary = 0,
		Function = 1,
		[Obsolete("This type has been deprecated in the BSON specification. Use Binary instead.")]
		Data = 2,
		Uuid = 3,
		Md5 = 5,
		UserDefined = 0x80
	}
}
