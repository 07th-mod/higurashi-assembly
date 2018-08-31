using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	public class DefaultContractResolver : IContractResolver
	{
		private static readonly IContractResolver _instance = new DefaultContractResolver(shareCache: true);

		private static readonly IList<JsonConverter> BuiltInConverters = new List<JsonConverter>
		{
			new KeyValuePairConverter(),
			new XmlNodeConverter(),
			new BsonObjectIdConverter()
		};

		private static Dictionary<ResolverContractKey, JsonContract> _sharedContractCache;

		private static readonly object _typeContractCacheLock = new object();

		private Dictionary<ResolverContractKey, JsonContract> _instanceContractCache;

		private readonly bool _sharedCache;

		internal static IContractResolver Instance => _instance;

		public bool DynamicCodeGeneration => JsonTypeReflector.DynamicCodeGeneration;

		public BindingFlags DefaultMembersSearchFlags
		{
			get;
			set;
		}

		public bool SerializeCompilerGeneratedMembers
		{
			get;
			set;
		}

		public DefaultContractResolver()
			: this(shareCache: false)
		{
		}

		public DefaultContractResolver(bool shareCache)
		{
			DefaultMembersSearchFlags = (BindingFlags.Instance | BindingFlags.Public);
			_sharedCache = shareCache;
		}

		private Dictionary<ResolverContractKey, JsonContract> GetCache()
		{
			if (_sharedCache)
			{
				return _sharedContractCache;
			}
			return _instanceContractCache;
		}

		private void UpdateCache(Dictionary<ResolverContractKey, JsonContract> cache)
		{
			if (_sharedCache)
			{
				_sharedContractCache = cache;
			}
			else
			{
				_instanceContractCache = cache;
			}
		}

		public virtual JsonContract ResolveContract(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ResolverContractKey key = new ResolverContractKey(GetType(), type);
			Dictionary<ResolverContractKey, JsonContract> cache = GetCache();
			if (cache == null || !cache.TryGetValue(key, out JsonContract value))
			{
				value = CreateContract(type);
				lock (_typeContractCacheLock)
				{
					cache = GetCache();
					Dictionary<ResolverContractKey, JsonContract> dictionary = (cache == null) ? new Dictionary<ResolverContractKey, JsonContract>() : new Dictionary<ResolverContractKey, JsonContract>(cache);
					dictionary[key] = value;
					UpdateCache(dictionary);
					return value;
				}
			}
			return value;
		}

		protected virtual List<MemberInfo> GetSerializableMembers(Type objectType)
		{
			DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(objectType);
			List<MemberInfo> list = (from m in ReflectionUtils.GetFieldsAndProperties(objectType, DefaultMembersSearchFlags)
			where !ReflectionUtils.IsIndexedProperty(m)
			select m).ToList();
			List<MemberInfo> list2 = (from m in ReflectionUtils.GetFieldsAndProperties(objectType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
			where !ReflectionUtils.IsIndexedProperty(m)
			select m).ToList();
			List<MemberInfo> list3 = new List<MemberInfo>();
			foreach (MemberInfo item in list2)
			{
				if (SerializeCompilerGeneratedMembers || !item.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
				{
					if (list.Contains(item))
					{
						list3.Add(item);
					}
					else if (JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(item) != null)
					{
						list3.Add(item);
					}
					else if (dataContractAttribute != null && JsonTypeReflector.GetAttribute<DataMemberAttribute>(item) != null)
					{
						list3.Add(item);
					}
				}
			}
			if (objectType.AssignableToTypeName("System.Data.Objects.DataClasses.EntityObject", out Type _))
			{
				list3 = list3.Where(ShouldSerializeEntityMember).ToList();
			}
			return list3;
		}

		private bool ShouldSerializeEntityMember(MemberInfo memberInfo)
		{
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null && propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "System.Data.Objects.DataClasses.EntityReference`1")
			{
				return false;
			}
			return true;
		}

		protected virtual JsonObjectContract CreateObjectContract(Type objectType)
		{
			JsonObjectContract jsonObjectContract = new JsonObjectContract(objectType);
			InitializeContract(jsonObjectContract);
			jsonObjectContract.MemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(objectType);
			jsonObjectContract.Properties.AddRange(CreateProperties(jsonObjectContract.UnderlyingType, jsonObjectContract.MemberSerialization));
			if (objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any((ConstructorInfo c) => c.IsDefined(typeof(JsonConstructorAttribute), inherit: true)))
			{
				ConstructorInfo attributeConstructor = GetAttributeConstructor(objectType);
				if (attributeConstructor != null)
				{
					jsonObjectContract.OverrideConstructor = attributeConstructor;
					jsonObjectContract.ConstructorParameters.AddRange(CreateConstructorParameters(attributeConstructor, jsonObjectContract.Properties));
				}
			}
			else if (jsonObjectContract.DefaultCreator == null || jsonObjectContract.DefaultCreatorNonPublic)
			{
				ConstructorInfo parametrizedConstructor = GetParametrizedConstructor(objectType);
				if (parametrizedConstructor != null)
				{
					jsonObjectContract.ParametrizedConstructor = parametrizedConstructor;
					jsonObjectContract.ConstructorParameters.AddRange(CreateConstructorParameters(parametrizedConstructor, jsonObjectContract.Properties));
				}
			}
			return jsonObjectContract;
		}

		private ConstructorInfo GetAttributeConstructor(Type objectType)
		{
			IList<ConstructorInfo> list = (from c in objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where c.IsDefined(typeof(JsonConstructorAttribute), inherit: true)
			select c).ToList();
			if (list.Count > 1)
			{
				throw new Exception("Multiple constructors with the JsonConstructorAttribute.");
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			return null;
		}

		private ConstructorInfo GetParametrizedConstructor(Type objectType)
		{
			IList<ConstructorInfo> constructors = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
			if (constructors.Count == 1)
			{
				return constructors[0];
			}
			return null;
		}

		protected virtual IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
		{
			ParameterInfo[] parameters = constructor.GetParameters();
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(constructor.DeclaringType);
			ParameterInfo[] array = parameters;
			foreach (ParameterInfo parameterInfo in array)
			{
				JsonProperty jsonProperty = memberProperties.GetClosestMatchProperty(parameterInfo.Name);
				if (jsonProperty != null && jsonProperty.PropertyType != parameterInfo.ParameterType)
				{
					jsonProperty = null;
				}
				JsonProperty jsonProperty2 = CreatePropertyFromConstructorParameter(jsonProperty, parameterInfo);
				if (jsonProperty2 != null)
				{
					jsonPropertyCollection.AddProperty(jsonProperty2);
				}
			}
			return jsonPropertyCollection;
		}

		protected virtual JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
		{
			JsonProperty jsonProperty = new JsonProperty();
			jsonProperty.PropertyType = parameterInfo.ParameterType;
			SetPropertySettingsFromAttributes(jsonProperty, parameterInfo, parameterInfo.Name, parameterInfo.Member.DeclaringType, MemberSerialization.OptOut, out bool _, out bool _);
			jsonProperty.Readable = false;
			jsonProperty.Writable = true;
			if (matchingMemberProperty != null)
			{
				jsonProperty.PropertyName = ((!(jsonProperty.PropertyName != parameterInfo.Name)) ? matchingMemberProperty.PropertyName : jsonProperty.PropertyName);
				jsonProperty.Converter = (jsonProperty.Converter ?? matchingMemberProperty.Converter);
				jsonProperty.MemberConverter = (jsonProperty.MemberConverter ?? matchingMemberProperty.MemberConverter);
				jsonProperty.DefaultValue = (jsonProperty.DefaultValue ?? matchingMemberProperty.DefaultValue);
				jsonProperty.Required = ((jsonProperty.Required == Required.Default) ? matchingMemberProperty.Required : jsonProperty.Required);
				jsonProperty.IsReference = ((!jsonProperty.IsReference.HasValue) ? matchingMemberProperty.IsReference : jsonProperty.IsReference);
				jsonProperty.NullValueHandling = ((!jsonProperty.NullValueHandling.HasValue) ? matchingMemberProperty.NullValueHandling : jsonProperty.NullValueHandling);
				jsonProperty.DefaultValueHandling = ((!jsonProperty.DefaultValueHandling.HasValue) ? matchingMemberProperty.DefaultValueHandling : jsonProperty.DefaultValueHandling);
				jsonProperty.ReferenceLoopHandling = ((!jsonProperty.ReferenceLoopHandling.HasValue) ? matchingMemberProperty.ReferenceLoopHandling : jsonProperty.ReferenceLoopHandling);
				jsonProperty.ObjectCreationHandling = ((!jsonProperty.ObjectCreationHandling.HasValue) ? matchingMemberProperty.ObjectCreationHandling : jsonProperty.ObjectCreationHandling);
				jsonProperty.TypeNameHandling = ((!jsonProperty.TypeNameHandling.HasValue) ? matchingMemberProperty.TypeNameHandling : jsonProperty.TypeNameHandling);
			}
			return jsonProperty;
		}

		protected virtual JsonConverter ResolveContractConverter(Type objectType)
		{
			return JsonTypeReflector.GetJsonConverter(objectType, objectType);
		}

		private Func<object> GetDefaultCreator(Type createdType)
		{
			return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(createdType);
		}

		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Runtime.Serialization.DataContractAttribute.#get_IsReference()")]
		private void InitializeContract(JsonContract contract)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(contract.UnderlyingType);
			if (jsonContainerAttribute != null)
			{
				contract.IsReference = jsonContainerAttribute._isReference;
			}
			else
			{
				DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(contract.UnderlyingType);
				if (dataContractAttribute != null && dataContractAttribute.IsReference)
				{
					contract.IsReference = true;
				}
			}
			contract.Converter = ResolveContractConverter(contract.UnderlyingType);
			contract.InternalConverter = JsonSerializer.GetMatchingConverter(BuiltInConverters, contract.UnderlyingType);
			if (ReflectionUtils.HasDefaultConstructor(contract.CreatedType, nonPublic: true) || contract.CreatedType.IsValueType)
			{
				contract.DefaultCreator = GetDefaultCreator(contract.CreatedType);
				contract.DefaultCreatorNonPublic = (!contract.CreatedType.IsValueType && ReflectionUtils.GetDefaultConstructor(contract.CreatedType) == null);
			}
			ResolveCallbackMethods(contract, contract.UnderlyingType);
		}

		private void ResolveCallbackMethods(JsonContract contract, Type t)
		{
			if (t.BaseType != null)
			{
				ResolveCallbackMethods(contract, t.BaseType);
			}
			GetCallbackMethodsForType(t, out MethodInfo onSerializing, out MethodInfo onSerialized, out MethodInfo onDeserializing, out MethodInfo onDeserialized, out MethodInfo onError);
			if (onSerializing != null)
			{
				contract.OnSerializing = onSerializing;
			}
			if (onSerialized != null)
			{
				contract.OnSerialized = onSerialized;
			}
			if (onDeserializing != null)
			{
				contract.OnDeserializing = onDeserializing;
			}
			if (onDeserialized != null)
			{
				contract.OnDeserialized = onDeserialized;
			}
			if (onError != null)
			{
				contract.OnError = onError;
			}
		}

		private void GetCallbackMethodsForType(Type type, out MethodInfo onSerializing, out MethodInfo onSerialized, out MethodInfo onDeserializing, out MethodInfo onDeserialized, out MethodInfo onError)
		{
			onSerializing = null;
			onSerialized = null;
			onDeserializing = null;
			onDeserialized = null;
			onError = null;
			MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (!methodInfo.ContainsGenericParameters)
				{
					Type prevAttributeType = null;
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (IsValidCallback(methodInfo, parameters, typeof(OnSerializingAttribute), onSerializing, ref prevAttributeType))
					{
						onSerializing = methodInfo;
					}
					if (IsValidCallback(methodInfo, parameters, typeof(OnSerializedAttribute), onSerialized, ref prevAttributeType))
					{
						onSerialized = methodInfo;
					}
					if (IsValidCallback(methodInfo, parameters, typeof(OnDeserializingAttribute), onDeserializing, ref prevAttributeType))
					{
						onDeserializing = methodInfo;
					}
					if (IsValidCallback(methodInfo, parameters, typeof(OnDeserializedAttribute), onDeserialized, ref prevAttributeType))
					{
						onDeserialized = methodInfo;
					}
					if (IsValidCallback(methodInfo, parameters, typeof(OnErrorAttribute), onError, ref prevAttributeType))
					{
						onError = methodInfo;
					}
				}
			}
		}

		protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
		{
			JsonDictionaryContract jsonDictionaryContract = new JsonDictionaryContract(objectType);
			InitializeContract(jsonDictionaryContract);
			jsonDictionaryContract.PropertyNameResolver = ResolvePropertyName;
			return jsonDictionaryContract;
		}

		protected virtual JsonArrayContract CreateArrayContract(Type objectType)
		{
			JsonArrayContract jsonArrayContract = new JsonArrayContract(objectType);
			InitializeContract(jsonArrayContract);
			return jsonArrayContract;
		}

		protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
		{
			JsonPrimitiveContract jsonPrimitiveContract = new JsonPrimitiveContract(objectType);
			InitializeContract(jsonPrimitiveContract);
			return jsonPrimitiveContract;
		}

		protected virtual JsonLinqContract CreateLinqContract(Type objectType)
		{
			JsonLinqContract jsonLinqContract = new JsonLinqContract(objectType);
			InitializeContract(jsonLinqContract);
			return jsonLinqContract;
		}

		protected virtual JsonISerializableContract CreateISerializableContract(Type objectType)
		{
			JsonISerializableContract jsonISerializableContract = new JsonISerializableContract(objectType);
			InitializeContract(jsonISerializableContract);
			ConstructorInfo constructor = objectType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
			{
				typeof(SerializationInfo),
				typeof(StreamingContext)
			}, null);
			if (constructor != null)
			{
				MethodCall<object, object> methodCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(constructor);
				jsonISerializableContract.ISerializableCreator = ((object[] args) => methodCall(null, args));
			}
			return jsonISerializableContract;
		}

		protected virtual JsonStringContract CreateStringContract(Type objectType)
		{
			JsonStringContract jsonStringContract = new JsonStringContract(objectType);
			InitializeContract(jsonStringContract);
			return jsonStringContract;
		}

		protected virtual JsonContract CreateContract(Type objectType)
		{
			Type type = ReflectionUtils.EnsureNotNullableType(objectType);
			if (JsonConvert.IsJsonPrimitiveType(type))
			{
				return CreatePrimitiveContract(type);
			}
			if (JsonTypeReflector.GetJsonObjectAttribute(type) != null)
			{
				return CreateObjectContract(type);
			}
			if (JsonTypeReflector.GetJsonArrayAttribute(type) != null)
			{
				return CreateArrayContract(type);
			}
			if (type == typeof(JToken) || type.IsSubclassOf(typeof(JToken)))
			{
				return CreateLinqContract(type);
			}
			if (CollectionUtils.IsDictionaryType(type))
			{
				return CreateDictionaryContract(type);
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				return CreateArrayContract(type);
			}
			if (CanConvertToString(type))
			{
				return CreateStringContract(type);
			}
			if (typeof(ISerializable).IsAssignableFrom(type))
			{
				return CreateISerializableContract(type);
			}
			return CreateObjectContract(type);
		}

		internal static bool CanConvertToString(Type type)
		{
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && !(converter is ComponentConverter) && !(converter is ReferenceConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
			{
				return true;
			}
			if (type == typeof(Type) || type.IsSubclassOf(typeof(Type)))
			{
				return true;
			}
			return false;
		}

		private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType, MethodInfo currentCallback, ref Type prevAttributeType)
		{
			if (!method.IsDefined(attributeType, inherit: false))
			{
				return false;
			}
			if (currentCallback != null)
			{
				throw new Exception("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.".FormatWith(CultureInfo.InvariantCulture, method, currentCallback, GetClrTypeFullName(method.DeclaringType), attributeType));
			}
			if (prevAttributeType != null)
			{
				throw new Exception("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.".FormatWith(CultureInfo.InvariantCulture, prevAttributeType, attributeType, GetClrTypeFullName(method.DeclaringType), method));
			}
			if (method.IsVirtual)
			{
				throw new Exception("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.".FormatWith(CultureInfo.InvariantCulture, method, GetClrTypeFullName(method.DeclaringType), attributeType));
			}
			if (method.ReturnType != typeof(void))
			{
				throw new Exception("Serialization Callback '{1}' in type '{0}' must return void.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method));
			}
			if (attributeType == typeof(OnErrorAttribute))
			{
				if (parameters == null || parameters.Length != 2 || parameters[0].ParameterType != typeof(StreamingContext) || parameters[1].ParameterType != typeof(ErrorContext))
				{
					throw new Exception("Serialization Error Callback '{1}' in type '{0}' must have two parameters of type '{2}' and '{3}'.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext), typeof(ErrorContext)));
				}
			}
			else if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != typeof(StreamingContext))
			{
				throw new Exception("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext)));
			}
			prevAttributeType = attributeType;
			return true;
		}

		internal static string GetClrTypeFullName(Type type)
		{
			if (type.IsGenericTypeDefinition || !type.ContainsGenericParameters)
			{
				return type.FullName;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", type.Namespace, type.Name);
		}

		protected virtual IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			List<MemberInfo> serializableMembers = GetSerializableMembers(type);
			if (serializableMembers == null)
			{
				throw new JsonSerializationException("Null collection of seralizable members returned.");
			}
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(type);
			foreach (MemberInfo item in serializableMembers)
			{
				JsonProperty jsonProperty = CreateProperty(item, memberSerialization);
				if (jsonProperty != null)
				{
					jsonPropertyCollection.AddProperty(jsonProperty);
				}
			}
			return jsonPropertyCollection.OrderBy(delegate(JsonProperty p)
			{
				int? order = p.Order;
				return (!order.HasValue) ? (-1) : order.Value;
			}).ToList();
		}

		protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
		{
			if (DynamicCodeGeneration)
			{
				return new DynamicValueProvider(member);
			}
			return new ReflectionValueProvider(member);
		}

		protected virtual JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty jsonProperty = new JsonProperty();
			jsonProperty.PropertyType = ReflectionUtils.GetMemberUnderlyingType(member);
			jsonProperty.ValueProvider = CreateMemberValueProvider(member);
			SetPropertySettingsFromAttributes(jsonProperty, member, member.Name, member.DeclaringType, memberSerialization, out bool allowNonPublicAccess, out bool hasExplicitAttribute);
			jsonProperty.Readable = ReflectionUtils.CanReadMemberValue(member, allowNonPublicAccess);
			jsonProperty.Writable = ReflectionUtils.CanSetMemberValue(member, allowNonPublicAccess, hasExplicitAttribute);
			jsonProperty.ShouldSerialize = CreateShouldSerializeTest(member);
			SetIsSpecifiedActions(jsonProperty, member, allowNonPublicAccess);
			return jsonProperty;
		}

		private void SetPropertySettingsFromAttributes(JsonProperty property, ICustomAttributeProvider attributeProvider, string name, Type declaringType, MemberSerialization memberSerialization, out bool allowNonPublicAccess, out bool hasExplicitAttribute)
		{
			hasExplicitAttribute = false;
			DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(declaringType);
			DataMemberAttribute dataMemberAttribute = (dataContractAttribute == null || !(attributeProvider is MemberInfo)) ? null : JsonTypeReflector.GetDataMemberAttribute((MemberInfo)attributeProvider);
			JsonPropertyAttribute attribute = JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(attributeProvider);
			if (attribute != null)
			{
				hasExplicitAttribute = true;
			}
			bool flag = JsonTypeReflector.GetAttribute<JsonIgnoreAttribute>(attributeProvider) != null;
			string propertyName = (attribute != null && attribute.PropertyName != null) ? attribute.PropertyName : ((dataMemberAttribute == null || dataMemberAttribute.Name == null) ? name : dataMemberAttribute.Name);
			property.PropertyName = ResolvePropertyName(propertyName);
			property.UnderlyingName = name;
			if (attribute != null)
			{
				property.Required = attribute.Required;
				property.Order = attribute._order;
			}
			else if (dataMemberAttribute != null)
			{
				property.Required = (dataMemberAttribute.IsRequired ? Required.AllowNull : Required.Default);
				property.Order = ((dataMemberAttribute.Order == -1) ? null : new int?(dataMemberAttribute.Order));
			}
			else
			{
				property.Required = Required.Default;
			}
			property.Ignored = (flag || (memberSerialization == MemberSerialization.OptIn && attribute == null && dataMemberAttribute == null));
			property.Converter = JsonTypeReflector.GetJsonConverter(attributeProvider, property.PropertyType);
			property.MemberConverter = JsonTypeReflector.GetJsonConverter(attributeProvider, property.PropertyType);
			property.DefaultValue = JsonTypeReflector.GetAttribute<DefaultValueAttribute>(attributeProvider)?.Value;
			property.NullValueHandling = attribute?._nullValueHandling;
			property.DefaultValueHandling = attribute?._defaultValueHandling;
			property.ReferenceLoopHandling = attribute?._referenceLoopHandling;
			property.ObjectCreationHandling = attribute?._objectCreationHandling;
			property.TypeNameHandling = attribute?._typeNameHandling;
			property.IsReference = attribute?._isReference;
			allowNonPublicAccess = false;
			if ((DefaultMembersSearchFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
			{
				allowNonPublicAccess = true;
			}
			if (attribute != null)
			{
				allowNonPublicAccess = true;
			}
			if (dataMemberAttribute != null)
			{
				allowNonPublicAccess = true;
				hasExplicitAttribute = true;
			}
		}

		private Predicate<object> CreateShouldSerializeTest(MemberInfo member)
		{
			MethodInfo method = member.DeclaringType.GetMethod("ShouldSerialize" + member.Name, new Type[0]);
			if (method == null || method.ReturnType != typeof(bool))
			{
				return null;
			}
			MethodCall<object, object> shouldSerializeCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object o) => (bool)shouldSerializeCall(o);
		}

		private void SetIsSpecifiedActions(JsonProperty property, MemberInfo member, bool allowNonPublicAccess)
		{
			MemberInfo memberInfo = member.DeclaringType.GetProperty(member.Name + "Specified");
			if (memberInfo == null)
			{
				memberInfo = member.DeclaringType.GetField(member.Name + "Specified");
			}
			if (memberInfo != null && ReflectionUtils.GetMemberUnderlyingType(memberInfo) == typeof(bool))
			{
				Func<object, object> specifiedPropertyGet = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(memberInfo);
				property.GetIsSpecified = ((object o) => (bool)specifiedPropertyGet(o));
				if (ReflectionUtils.CanSetMemberValue(memberInfo, allowNonPublicAccess, canSetReadOnly: false))
				{
					property.SetIsSpecified = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(memberInfo);
				}
			}
		}

		protected internal virtual string ResolvePropertyName(string propertyName)
		{
			return propertyName;
		}
	}
}
