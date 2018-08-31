using System;

namespace Newtonsoft.Json.Serialization
{
	internal interface IMetadataTypeAttribute
	{
		Type MetadataClassType
		{
			get;
		}
	}
}
