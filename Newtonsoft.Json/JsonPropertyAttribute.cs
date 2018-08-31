using System;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class JsonPropertyAttribute : Attribute
	{
		internal NullValueHandling? _nullValueHandling;

		internal DefaultValueHandling? _defaultValueHandling;

		internal ReferenceLoopHandling? _referenceLoopHandling;

		internal ObjectCreationHandling? _objectCreationHandling;

		internal TypeNameHandling? _typeNameHandling;

		internal bool? _isReference;

		internal int? _order;

		public NullValueHandling NullValueHandling
		{
			get
			{
				NullValueHandling? nullValueHandling = _nullValueHandling;
				return nullValueHandling.HasValue ? nullValueHandling.Value : NullValueHandling.Include;
			}
			set
			{
				_nullValueHandling = value;
			}
		}

		public DefaultValueHandling DefaultValueHandling
		{
			get
			{
				DefaultValueHandling? defaultValueHandling = _defaultValueHandling;
				return defaultValueHandling.HasValue ? defaultValueHandling.Value : DefaultValueHandling.Include;
			}
			set
			{
				_defaultValueHandling = value;
			}
		}

		public ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				ReferenceLoopHandling? referenceLoopHandling = _referenceLoopHandling;
				return referenceLoopHandling.HasValue ? referenceLoopHandling.Value : ReferenceLoopHandling.Error;
			}
			set
			{
				_referenceLoopHandling = value;
			}
		}

		public ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				ObjectCreationHandling? objectCreationHandling = _objectCreationHandling;
				return objectCreationHandling.HasValue ? objectCreationHandling.Value : ObjectCreationHandling.Auto;
			}
			set
			{
				_objectCreationHandling = value;
			}
		}

		public TypeNameHandling TypeNameHandling
		{
			get
			{
				TypeNameHandling? typeNameHandling = _typeNameHandling;
				return typeNameHandling.HasValue ? typeNameHandling.Value : TypeNameHandling.None;
			}
			set
			{
				_typeNameHandling = value;
			}
		}

		public bool IsReference
		{
			get
			{
				bool? isReference = _isReference;
				return isReference.HasValue && isReference.Value;
			}
			set
			{
				_isReference = value;
			}
		}

		public int Order
		{
			get
			{
				int? order = _order;
				return order.HasValue ? order.Value : 0;
			}
			set
			{
				_order = value;
			}
		}

		public string PropertyName
		{
			get;
			set;
		}

		public Required Required
		{
			get;
			set;
		}

		public JsonPropertyAttribute()
		{
		}

		public JsonPropertyAttribute(string propertyName)
		{
			PropertyName = propertyName;
		}
	}
}
