using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.Serialization
{
	public class JsonArrayContract : JsonContract
	{
		private readonly bool _isCollectionItemTypeNullableType;

		private readonly Type _genericCollectionDefinitionType;

		private Type _genericWrapperType;

		private MethodCall<object, object> _genericWrapperCreator;

		internal Type CollectionItemType
		{
			get;
			private set;
		}

		public JsonArrayContract(Type underlyingType)
			: base(underlyingType)
		{
			if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out _genericCollectionDefinitionType))
			{
				CollectionItemType = _genericCollectionDefinitionType.GetGenericArguments()[0];
			}
			else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				_genericCollectionDefinitionType = typeof(IEnumerable<>);
				CollectionItemType = underlyingType.GetGenericArguments()[0];
			}
			else
			{
				CollectionItemType = ReflectionUtils.GetCollectionItemType(base.UnderlyingType);
			}
			if (CollectionItemType != null)
			{
				_isCollectionItemTypeNullableType = ReflectionUtils.IsNullableType(CollectionItemType);
			}
			if (IsTypeGenericCollectionInterface(base.UnderlyingType))
			{
				base.CreatedType = ReflectionUtils.MakeGenericType(typeof(List<>), CollectionItemType);
			}
			else if (typeof(HashSet<>).IsAssignableFrom(base.UnderlyingType))
			{
				base.CreatedType = ReflectionUtils.MakeGenericType(typeof(HashSet<>), CollectionItemType);
			}
		}

		internal IWrappedCollection CreateWrapper(object list)
		{
			if ((list is IList && (CollectionItemType == null || !_isCollectionItemTypeNullableType)) || base.UnderlyingType.IsArray)
			{
				return new CollectionWrapper<object>((IList)list);
			}
			if (_genericCollectionDefinitionType != null)
			{
				EnsureGenericWrapperCreator();
				return (IWrappedCollection)_genericWrapperCreator(null, list);
			}
			IList list2 = ((IEnumerable)list).Cast<object>().ToList();
			if (CollectionItemType != null)
			{
				Array array = Array.CreateInstance(CollectionItemType, list2.Count);
				for (int i = 0; i < list2.Count; i++)
				{
					array.SetValue(list2[i], i);
				}
				list2 = array;
			}
			return new CollectionWrapper<object>(list2);
		}

		private void EnsureGenericWrapperCreator()
		{
			if (_genericWrapperType == null)
			{
				_genericWrapperType = ReflectionUtils.MakeGenericType(typeof(CollectionWrapper<>), CollectionItemType);
				Type type = (!ReflectionUtils.InheritsGenericDefinition(_genericCollectionDefinitionType, typeof(List<>)) && _genericCollectionDefinitionType.GetGenericTypeDefinition() != typeof(IEnumerable<>)) ? _genericCollectionDefinitionType : ReflectionUtils.MakeGenericType(typeof(ICollection<>), CollectionItemType);
				ConstructorInfo constructor = _genericWrapperType.GetConstructor(new Type[1]
				{
					type
				});
				_genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(constructor);
			}
		}

		private bool IsTypeGenericCollectionInterface(Type type)
		{
			if (!type.IsGenericType)
			{
				return false;
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(IEnumerable<>);
		}
	}
}
