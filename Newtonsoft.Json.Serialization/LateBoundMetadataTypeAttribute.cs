using Newtonsoft.Json.Utilities;
using System;
using System.Reflection;

namespace Newtonsoft.Json.Serialization
{
	internal class LateBoundMetadataTypeAttribute : IMetadataTypeAttribute
	{
		private static PropertyInfo _metadataClassTypeProperty;

		private readonly object _attribute;

		public Type MetadataClassType
		{
			get
			{
				if (_metadataClassTypeProperty == null)
				{
					_metadataClassTypeProperty = _attribute.GetType().GetProperty("MetadataClassType");
				}
				return (Type)ReflectionUtils.GetMemberValue(_metadataClassTypeProperty, _attribute);
			}
		}

		public LateBoundMetadataTypeAttribute(object attribute)
		{
			_attribute = attribute;
		}
	}
}
