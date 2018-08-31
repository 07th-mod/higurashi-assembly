using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonSerializerInternalReader : JsonSerializerInternalBase
	{
		internal enum PropertyPresence
		{
			None,
			Null,
			Value
		}

		private JsonSerializerProxy _internalSerializer;

		private JsonFormatterConverter _formatterConverter;

		public JsonSerializerInternalReader(JsonSerializer serializer)
			: base(serializer)
		{
		}

		public void Populate(JsonReader reader, object target)
		{
			ValidationUtils.ArgumentNotNull(target, "target");
			Type type = target.GetType();
			JsonContract jsonContract = base.Serializer.ContractResolver.ResolveContract(type);
			if (reader.TokenType == JsonToken.None)
			{
				reader.Read();
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				if (!(jsonContract is JsonArrayContract))
				{
					throw new JsonSerializationException("Cannot populate JSON array onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
				}
				PopulateList(CollectionUtils.CreateCollectionWrapper(target), reader, null, (JsonArrayContract)jsonContract);
			}
			else
			{
				if (reader.TokenType != JsonToken.StartObject)
				{
					throw new JsonSerializationException("Unexpected initial token '{0}' when populating object. Expected JSON object or array.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
				CheckedRead(reader);
				string id = null;
				if (reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$id", StringComparison.Ordinal))
				{
					CheckedRead(reader);
					id = ((reader.Value == null) ? null : reader.Value.ToString());
					CheckedRead(reader);
				}
				if (jsonContract is JsonDictionaryContract)
				{
					PopulateDictionary(CollectionUtils.CreateDictionaryWrapper(target), reader, (JsonDictionaryContract)jsonContract, id);
				}
				else
				{
					if (!(jsonContract is JsonObjectContract))
					{
						throw new JsonSerializationException("Cannot populate JSON object onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
					}
					PopulateObject(target, reader, (JsonObjectContract)jsonContract, id);
				}
			}
		}

		private JsonContract GetContractSafe(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return base.Serializer.ContractResolver.ResolveContract(type);
		}

		private JsonContract GetContractSafe(Type type, object value)
		{
			if (value == null)
			{
				return GetContractSafe(type);
			}
			return base.Serializer.ContractResolver.ResolveContract(value.GetType());
		}

		public object Deserialize(JsonReader reader, Type objectType)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.TokenType == JsonToken.None && !ReadForType(reader, objectType, null))
			{
				return null;
			}
			return CreateValueNonProperty(reader, objectType, GetContractSafe(objectType));
		}

		private JsonSerializerProxy GetInternalSerializer()
		{
			if (_internalSerializer == null)
			{
				_internalSerializer = new JsonSerializerProxy(this);
			}
			return _internalSerializer;
		}

		private JsonFormatterConverter GetFormatterConverter()
		{
			if (_formatterConverter == null)
			{
				_formatterConverter = new JsonFormatterConverter(GetInternalSerializer());
			}
			return _formatterConverter;
		}

		private JToken CreateJToken(JsonReader reader, JsonContract contract)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (contract != null && contract.UnderlyingType == typeof(JRaw))
			{
				return JRaw.Create(reader);
			}
			using (JTokenWriter jTokenWriter = new JTokenWriter())
			{
				jTokenWriter.WriteToken(reader);
				return jTokenWriter.Token;
			}
		}

		private JToken CreateJObject(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			using (JTokenWriter jTokenWriter = new JTokenWriter())
			{
				jTokenWriter.WriteStartObject();
				if (reader.TokenType == JsonToken.PropertyName)
				{
					jTokenWriter.WriteToken(reader, reader.Depth - 1);
				}
				else
				{
					jTokenWriter.WriteEndObject();
				}
				return jTokenWriter.Token;
			}
		}

		private object CreateValueProperty(JsonReader reader, JsonProperty property, object target, bool gottenCurrentValue, object currentValue)
		{
			JsonContract contractSafe = GetContractSafe(property.PropertyType, currentValue);
			Type propertyType = property.PropertyType;
			JsonConverter converter = GetConverter(contractSafe, property.MemberConverter);
			if (converter != null && converter.CanRead)
			{
				if (!gottenCurrentValue && target != null && property.Readable)
				{
					currentValue = property.ValueProvider.GetValue(target);
				}
				return converter.ReadJson(reader, propertyType, currentValue, GetInternalSerializer());
			}
			return CreateValueInternal(reader, propertyType, contractSafe, property, currentValue);
		}

		private object CreateValueNonProperty(JsonReader reader, Type objectType, JsonContract contract)
		{
			JsonConverter converter = GetConverter(contract, null);
			if (converter != null && converter.CanRead)
			{
				return converter.ReadJson(reader, objectType, null, GetInternalSerializer());
			}
			return CreateValueInternal(reader, objectType, contract, null, null);
		}

		private object CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue)
		{
			if (contract is JsonLinqContract)
			{
				return CreateJToken(reader, contract);
			}
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.StartObject:
					return CreateObject(reader, objectType, contract, member, existingValue);
				case JsonToken.StartArray:
					return CreateList(reader, objectType, contract, member, existingValue, null);
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.Boolean:
				case JsonToken.Date:
				case JsonToken.Bytes:
					return EnsureType(reader.Value, CultureInfo.InvariantCulture, objectType);
				case JsonToken.String:
					if (string.IsNullOrEmpty((string)reader.Value) && objectType != null && ReflectionUtils.IsNullableType(objectType))
					{
						return null;
					}
					if (objectType == typeof(byte[]))
					{
						return Convert.FromBase64String((string)reader.Value);
					}
					return EnsureType(reader.Value, CultureInfo.InvariantCulture, objectType);
				case JsonToken.StartConstructor:
				case JsonToken.EndConstructor:
					return reader.Value.ToString();
				case JsonToken.Null:
				case JsonToken.Undefined:
					if (objectType == typeof(DBNull))
					{
						return DBNull.Value;
					}
					return EnsureType(reader.Value, CultureInfo.InvariantCulture, objectType);
				case JsonToken.Raw:
					return new JRaw((string)reader.Value);
				default:
					throw new JsonSerializationException("Unexpected token while deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (reader.Read());
			throw new JsonSerializationException("Unexpected end when deserializing object.");
		}

		private JsonConverter GetConverter(JsonContract contract, JsonConverter memberConverter)
		{
			JsonConverter result = null;
			if (memberConverter != null)
			{
				result = memberConverter;
			}
			else if (contract != null)
			{
				JsonConverter matchingConverter;
				if (contract.Converter != null)
				{
					result = contract.Converter;
				}
				else if ((matchingConverter = base.Serializer.GetMatchingConverter(contract.UnderlyingType)) != null)
				{
					result = matchingConverter;
				}
				else if (contract.InternalConverter != null)
				{
					result = contract.InternalConverter;
				}
			}
			return result;
		}

		private object CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue)
		{
			CheckedRead(reader);
			string text = null;
			if (reader.TokenType == JsonToken.PropertyName)
			{
				bool flag;
				do
				{
					string a = reader.Value.ToString();
					if (string.Equals(a, "$ref", StringComparison.Ordinal))
					{
						CheckedRead(reader);
						if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null)
						{
							throw new JsonSerializationException("JSON reference {0} property must have a string or null value.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
						}
						string text2 = (reader.Value == null) ? null : reader.Value.ToString();
						CheckedRead(reader);
						if (text2 != null)
						{
							if (reader.TokenType == JsonToken.PropertyName)
							{
								throw new JsonSerializationException("Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
							}
							return base.Serializer.ReferenceResolver.ResolveReference(this, text2);
						}
						flag = true;
					}
					else if (string.Equals(a, "$type", StringComparison.Ordinal))
					{
						CheckedRead(reader);
						string text3 = reader.Value.ToString();
						CheckedRead(reader);
						TypeNameHandling? typeNameHandling = member?.TypeNameHandling;
						if (((!typeNameHandling.HasValue) ? base.Serializer.TypeNameHandling : typeNameHandling.Value) != 0)
						{
							ReflectionUtils.SplitFullyQualifiedTypeName(text3, out string typeName, out string assemblyName);
							Type type;
							try
							{
								type = base.Serializer.Binder.BindToType(assemblyName, typeName);
							}
							catch (Exception innerException)
							{
								throw new JsonSerializationException("Error resolving type specified in JSON '{0}'.".FormatWith(CultureInfo.InvariantCulture, text3), innerException);
								IL_01a6:;
							}
							if (type == null)
							{
								throw new JsonSerializationException("Type specified in JSON '{0}' was not resolved.".FormatWith(CultureInfo.InvariantCulture, text3));
							}
							if (objectType != null && !objectType.IsAssignableFrom(type))
							{
								throw new JsonSerializationException("Type specified in JSON '{0}' is not compatible with '{1}'.".FormatWith(CultureInfo.InvariantCulture, type.AssemblyQualifiedName, objectType.AssemblyQualifiedName));
							}
							objectType = type;
							contract = GetContractSafe(type);
						}
						flag = true;
					}
					else if (string.Equals(a, "$id", StringComparison.Ordinal))
					{
						CheckedRead(reader);
						text = ((reader.Value == null) ? null : reader.Value.ToString());
						CheckedRead(reader);
						flag = true;
					}
					else
					{
						if (string.Equals(a, "$values", StringComparison.Ordinal))
						{
							CheckedRead(reader);
							object result = CreateList(reader, objectType, contract, member, existingValue, text);
							CheckedRead(reader);
							return result;
						}
						flag = false;
					}
				}
				while (flag && reader.TokenType == JsonToken.PropertyName);
			}
			if (!HasDefinedType(objectType))
			{
				return CreateJObject(reader);
			}
			if (contract == null)
			{
				throw new JsonSerializationException("Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			JsonDictionaryContract jsonDictionaryContract = contract as JsonDictionaryContract;
			if (jsonDictionaryContract != null)
			{
				if (existingValue == null)
				{
					return CreateAndPopulateDictionary(reader, jsonDictionaryContract, text);
				}
				return PopulateDictionary(jsonDictionaryContract.CreateWrapper(existingValue), reader, jsonDictionaryContract, text);
			}
			JsonObjectContract jsonObjectContract = contract as JsonObjectContract;
			if (jsonObjectContract != null)
			{
				if (existingValue == null)
				{
					return CreateAndPopulateObject(reader, jsonObjectContract, text);
				}
				return PopulateObject(existingValue, reader, jsonObjectContract, text);
			}
			JsonPrimitiveContract jsonPrimitiveContract = contract as JsonPrimitiveContract;
			if (jsonPrimitiveContract != null && reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$value", StringComparison.Ordinal))
			{
				CheckedRead(reader);
				object result2 = CreateValueInternal(reader, objectType, jsonPrimitiveContract, member, existingValue);
				CheckedRead(reader);
				return result2;
			}
			JsonISerializableContract jsonISerializableContract = contract as JsonISerializableContract;
			if (jsonISerializableContract != null)
			{
				return CreateISerializable(reader, jsonISerializableContract, text);
			}
			throw new JsonSerializationException("Cannot deserialize JSON object into type '{0}'.".FormatWith(CultureInfo.InvariantCulture, objectType));
		}

		private JsonArrayContract EnsureArrayContract(Type objectType, JsonContract contract)
		{
			if (contract == null)
			{
				throw new JsonSerializationException("Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			JsonArrayContract jsonArrayContract = contract as JsonArrayContract;
			if (jsonArrayContract == null)
			{
				throw new JsonSerializationException("Cannot deserialize JSON array into type '{0}'.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			return jsonArrayContract;
		}

		private void CheckedRead(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw new JsonSerializationException("Unexpected end when deserializing object.");
			}
		}

		private object CreateList(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue, string reference)
		{
			if (HasDefinedType(objectType))
			{
				JsonArrayContract jsonArrayContract = EnsureArrayContract(objectType, contract);
				if (existingValue == null)
				{
					return CreateAndPopulateList(reader, reference, jsonArrayContract);
				}
				return PopulateList(jsonArrayContract.CreateWrapper(existingValue), reader, reference, jsonArrayContract);
			}
			return CreateJToken(reader, contract);
		}

		private bool HasDefinedType(Type type)
		{
			return type != null && type != typeof(object) && !typeof(JToken).IsAssignableFrom(type);
		}

		private object EnsureType(object value, CultureInfo culture, Type targetType)
		{
			if (targetType == null)
			{
				return value;
			}
			Type objectType = ReflectionUtils.GetObjectType(value);
			if (objectType != targetType)
			{
				try
				{
					return ConvertUtils.ConvertOrCast(value, culture, targetType);
					IL_0024:
					return value;
				}
				catch (Exception innerException)
				{
					throw new JsonSerializationException("Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, FormatValueForPrint(value), targetType), innerException);
					IL_0054:
					return value;
				}
			}
			return value;
		}

		private string FormatValueForPrint(object value)
		{
			if (value == null)
			{
				return "{null}";
			}
			if (value is string)
			{
				return "\"" + value + "\"";
			}
			return value.ToString();
		}

		private void SetPropertyValue(JsonProperty property, JsonReader reader, object target)
		{
			if (property.Ignored)
			{
				reader.Skip();
			}
			else
			{
				object obj = null;
				bool flag = false;
				bool gottenCurrentValue = false;
				ObjectCreationHandling valueOrDefault = property.ObjectCreationHandling.GetValueOrDefault(base.Serializer.ObjectCreationHandling);
				if ((valueOrDefault == ObjectCreationHandling.Auto || valueOrDefault == ObjectCreationHandling.Reuse) && (reader.TokenType == JsonToken.StartArray || reader.TokenType == JsonToken.StartObject) && property.Readable)
				{
					obj = property.ValueProvider.GetValue(target);
					gottenCurrentValue = true;
					flag = (obj != null && !property.PropertyType.IsArray && !ReflectionUtils.InheritsGenericDefinition(property.PropertyType, typeof(ReadOnlyCollection<>)) && !property.PropertyType.IsValueType);
				}
				if (!property.Writable && !flag)
				{
					reader.Skip();
				}
				else if (property.NullValueHandling.GetValueOrDefault(base.Serializer.NullValueHandling) == NullValueHandling.Ignore && reader.TokenType == JsonToken.Null)
				{
					reader.Skip();
				}
				else if (HasFlag(property.DefaultValueHandling.GetValueOrDefault(base.Serializer.DefaultValueHandling), DefaultValueHandling.Ignore) && JsonReader.IsPrimitiveToken(reader.TokenType) && MiscellaneousUtils.ValueEquals(reader.Value, property.DefaultValue))
				{
					reader.Skip();
				}
				else
				{
					object currentValue = (!flag) ? null : obj;
					object obj2 = CreateValueProperty(reader, property, target, gottenCurrentValue, currentValue);
					if ((!flag || obj2 != obj) && ShouldSetPropertyValue(property, obj2))
					{
						property.ValueProvider.SetValue(target, obj2);
						if (property.SetIsSpecified != null)
						{
							property.SetIsSpecified(target, true);
						}
					}
				}
			}
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool ShouldSetPropertyValue(JsonProperty property, object value)
		{
			if (property.NullValueHandling.GetValueOrDefault(base.Serializer.NullValueHandling) == NullValueHandling.Ignore && value == null)
			{
				return false;
			}
			if (HasFlag(property.DefaultValueHandling.GetValueOrDefault(base.Serializer.DefaultValueHandling), DefaultValueHandling.Ignore) && MiscellaneousUtils.ValueEquals(value, property.DefaultValue))
			{
				return false;
			}
			if (!property.Writable)
			{
				return false;
			}
			return true;
		}

		private object CreateAndPopulateDictionary(JsonReader reader, JsonDictionaryContract contract, string id)
		{
			if (contract.DefaultCreator == null || (contract.DefaultCreatorNonPublic && base.Serializer.ConstructorHandling != ConstructorHandling.AllowNonPublicDefaultConstructor))
			{
				throw new JsonSerializationException("Unable to find a default constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
			object dictionary = contract.DefaultCreator();
			IWrappedDictionary wrappedDictionary = contract.CreateWrapper(dictionary);
			PopulateDictionary(wrappedDictionary, reader, contract, id);
			return wrappedDictionary.UnderlyingDictionary;
		}

		private object PopulateDictionary(IWrappedDictionary dictionary, JsonReader reader, JsonDictionaryContract contract, string id)
		{
			if (id != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, id, dictionary.UnderlyingDictionary);
			}
			contract.InvokeOnDeserializing(dictionary.UnderlyingDictionary, base.Serializer.Context);
			int depth = reader.Depth;
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.PropertyName:
				{
					object obj = reader.Value;
					try
					{
						try
						{
							obj = EnsureType(obj, CultureInfo.InvariantCulture, contract.DictionaryKeyType);
						}
						catch (Exception innerException)
						{
							throw new JsonSerializationException("Could not convert string '{0}' to dictionary key type '{1}'. Create a TypeConverter to convert from the string to the key type object.".FormatWith(CultureInfo.InvariantCulture, reader.Value, contract.DictionaryKeyType), innerException);
							IL_00b2:;
						}
						if (!ReadForType(reader, contract.DictionaryValueType, null))
						{
							throw new JsonSerializationException("Unexpected end when deserializing object.");
						}
						dictionary[obj] = CreateValueNonProperty(reader, contract.DictionaryValueType, GetContractSafe(contract.DictionaryValueType));
					}
					catch (Exception ex)
					{
						if (!IsErrorHandled(dictionary, contract, obj, ex))
						{
							throw;
						}
						HandleError(reader, depth);
					}
					break;
				}
				case JsonToken.EndObject:
					contract.InvokeOnDeserialized(dictionary.UnderlyingDictionary, base.Serializer.Context);
					return dictionary.UnderlyingDictionary;
				default:
					throw new JsonSerializationException("Unexpected token when deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (reader.Read());
			throw new JsonSerializationException("Unexpected end when deserializing object.");
		}

		private object CreateAndPopulateList(JsonReader reader, string reference, JsonArrayContract contract)
		{
			return CollectionUtils.CreateAndPopulateList(contract.CreatedType, delegate(IList l, bool isTemporaryListReference)
			{
				if (reference != null && isTemporaryListReference)
				{
					throw new JsonSerializationException("Cannot preserve reference to array or readonly list: {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				if (contract.OnSerializing != null && isTemporaryListReference)
				{
					throw new JsonSerializationException("Cannot call OnSerializing on an array or readonly list: {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				if (contract.OnError != null && isTemporaryListReference)
				{
					throw new JsonSerializationException("Cannot call OnError on an array or readonly list: {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				PopulateList(contract.CreateWrapper(l), reader, reference, contract);
			});
		}

		private bool ReadForTypeArrayHack(JsonReader reader, Type t)
		{
			try
			{
				return ReadForType(reader, t, null);
				IL_000f:
				bool result;
				return result;
			}
			catch (JsonReaderException)
			{
				if (reader.TokenType != JsonToken.EndArray)
				{
					throw;
				}
				return true;
				IL_002b:
				bool result;
				return result;
			}
		}

		private object PopulateList(IWrappedCollection wrappedList, JsonReader reader, string reference, JsonArrayContract contract)
		{
			object underlyingCollection = wrappedList.UnderlyingCollection;
			if (wrappedList.IsFixedSize)
			{
				reader.Skip();
				return wrappedList.UnderlyingCollection;
			}
			if (reference != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, reference, underlyingCollection);
			}
			contract.InvokeOnDeserializing(underlyingCollection, base.Serializer.Context);
			int depth = reader.Depth;
			while (ReadForTypeArrayHack(reader, contract.CollectionItemType))
			{
				switch (reader.TokenType)
				{
				case JsonToken.EndArray:
					contract.InvokeOnDeserialized(underlyingCollection, base.Serializer.Context);
					return wrappedList.UnderlyingCollection;
				default:
					try
					{
						object value = CreateValueNonProperty(reader, contract.CollectionItemType, GetContractSafe(contract.CollectionItemType));
						wrappedList.Add(value);
					}
					catch (Exception ex)
					{
						if (!IsErrorHandled(underlyingCollection, contract, wrappedList.Count, ex))
						{
							throw;
						}
						HandleError(reader, depth);
					}
					break;
				case JsonToken.Comment:
					break;
				}
			}
			throw new JsonSerializationException("Unexpected end when deserializing array.");
		}

		private object CreateISerializable(JsonReader reader, JsonISerializableContract contract, string id)
		{
			Type underlyingType = contract.UnderlyingType;
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, GetFormatterConverter());
			bool flag = false;
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.PropertyName:
				{
					string text = reader.Value.ToString();
					if (!reader.Read())
					{
						throw new JsonSerializationException("Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
					}
					serializationInfo.AddValue(text, JToken.ReadFrom(reader));
					break;
				}
				case JsonToken.EndObject:
					flag = true;
					break;
				default:
					throw new JsonSerializationException("Unexpected token when deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (!flag && reader.Read());
			if (contract.ISerializableCreator == null)
			{
				throw new JsonSerializationException("ISerializable type '{0}' does not have a valid constructor. To correctly implement ISerializable a constructor that takes SerializationInfo and StreamingContext parameters should be present.".FormatWith(CultureInfo.InvariantCulture, underlyingType));
			}
			object obj = contract.ISerializableCreator(serializationInfo, base.Serializer.Context);
			if (id != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, id, obj);
			}
			contract.InvokeOnDeserializing(obj, base.Serializer.Context);
			contract.InvokeOnDeserialized(obj, base.Serializer.Context);
			return obj;
		}

		private object CreateAndPopulateObject(JsonReader reader, JsonObjectContract contract, string id)
		{
			object obj = null;
			if (contract.UnderlyingType.IsInterface || contract.UnderlyingType.IsAbstract)
			{
				throw new JsonSerializationException("Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantated.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
			if (contract.OverrideConstructor != null)
			{
				if (contract.OverrideConstructor.GetParameters().Length > 0)
				{
					return CreateObjectFromNonDefaultConstructor(reader, contract, contract.OverrideConstructor, id);
				}
				obj = contract.OverrideConstructor.Invoke(null);
			}
			else if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || base.Serializer.ConstructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
			{
				obj = contract.DefaultCreator();
			}
			else if (contract.ParametrizedConstructor != null)
			{
				return CreateObjectFromNonDefaultConstructor(reader, contract, contract.ParametrizedConstructor, id);
			}
			if (obj == null)
			{
				throw new JsonSerializationException("Unable to find a constructor to use for type {0}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
			PopulateObject(obj, reader, contract, id);
			return obj;
		}

		private object CreateObjectFromNonDefaultConstructor(JsonReader reader, JsonObjectContract contract, ConstructorInfo constructorInfo, string id)
		{
			ValidationUtils.ArgumentNotNull(constructorInfo, "constructorInfo");
			Type underlyingType = contract.UnderlyingType;
			IDictionary<JsonProperty, object> dictionary = ResolvePropertyAndConstructorValues(contract, reader, underlyingType);
			IDictionary<ParameterInfo, object> dictionary2 = ((IEnumerable<ParameterInfo>)constructorInfo.GetParameters()).ToDictionary((Func<ParameterInfo, ParameterInfo>)((ParameterInfo p) => p), (Func<ParameterInfo, object>)((ParameterInfo p) => null));
			IDictionary<JsonProperty, object> dictionary3 = new Dictionary<JsonProperty, object>();
			foreach (KeyValuePair<JsonProperty, object> item in dictionary)
			{
				ParameterInfo key = dictionary2.ForgivingCaseSensitiveFind((KeyValuePair<ParameterInfo, object> kv) => kv.Key.Name, item.Key.UnderlyingName).Key;
				if (key != null)
				{
					dictionary2[key] = item.Value;
				}
				else
				{
					dictionary3.Add(item);
				}
			}
			object obj = constructorInfo.Invoke(dictionary2.Values.ToArray());
			if (id != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, id, obj);
			}
			contract.InvokeOnDeserializing(obj, base.Serializer.Context);
			foreach (KeyValuePair<JsonProperty, object> item2 in dictionary3)
			{
				JsonProperty key2 = item2.Key;
				object value = item2.Value;
				if (ShouldSetPropertyValue(item2.Key, item2.Value))
				{
					key2.ValueProvider.SetValue(obj, value);
				}
				else if (!key2.Writable && value != null)
				{
					JsonContract jsonContract = base.Serializer.ContractResolver.ResolveContract(key2.PropertyType);
					if (jsonContract is JsonArrayContract)
					{
						JsonArrayContract jsonArrayContract = jsonContract as JsonArrayContract;
						object value2 = key2.ValueProvider.GetValue(obj);
						if (value2 != null)
						{
							IWrappedCollection wrappedCollection = jsonArrayContract.CreateWrapper(value2);
							IWrappedCollection wrappedCollection2 = jsonArrayContract.CreateWrapper(value);
							foreach (object item3 in wrappedCollection2)
							{
								wrappedCollection.Add(item3);
							}
						}
					}
					else if (jsonContract is JsonDictionaryContract)
					{
						JsonDictionaryContract jsonDictionaryContract = jsonContract as JsonDictionaryContract;
						object value3 = key2.ValueProvider.GetValue(obj);
						if (value3 != null)
						{
							IWrappedDictionary wrappedDictionary = jsonDictionaryContract.CreateWrapper(value3);
							IWrappedDictionary wrappedDictionary2 = jsonDictionaryContract.CreateWrapper(value);
							foreach (DictionaryEntry item4 in (IEnumerable)wrappedDictionary2)
							{
								wrappedDictionary.Add(item4.Key, item4.Value);
							}
						}
					}
				}
			}
			contract.InvokeOnDeserialized(obj, base.Serializer.Context);
			return obj;
		}

		private IDictionary<JsonProperty, object> ResolvePropertyAndConstructorValues(JsonObjectContract contract, JsonReader reader, Type objectType)
		{
			IDictionary<JsonProperty, object> dictionary = new Dictionary<JsonProperty, object>();
			bool flag = false;
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.PropertyName:
				{
					string text = reader.Value.ToString();
					JsonProperty jsonProperty = contract.ConstructorParameters.GetClosestMatchProperty(text) ?? contract.Properties.GetClosestMatchProperty(text);
					if (jsonProperty != null)
					{
						if (!ReadForType(reader, jsonProperty.PropertyType, jsonProperty.Converter))
						{
							throw new JsonSerializationException("Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
						}
						if (!jsonProperty.Ignored)
						{
							dictionary[jsonProperty] = CreateValueProperty(reader, jsonProperty, null, /*gottenCurrentValue:*/ true, null);
						}
						else
						{
							reader.Skip();
						}
					}
					else
					{
						if (!reader.Read())
						{
							throw new JsonSerializationException("Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
						}
						if (base.Serializer.MissingMemberHandling == MissingMemberHandling.Error)
						{
							throw new JsonSerializationException("Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, text, objectType.Name));
						}
						reader.Skip();
					}
					break;
				}
				case JsonToken.EndObject:
					flag = true;
					break;
				default:
					throw new JsonSerializationException("Unexpected token when deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (!flag && reader.Read());
			return dictionary;
		}

		private bool ReadForType(JsonReader reader, Type t, JsonConverter propertyConverter)
		{
			if (GetConverter(GetContractSafe(t), propertyConverter) != null)
			{
				return reader.Read();
			}
			if (t == typeof(byte[]))
			{
				reader.ReadAsBytes();
				return true;
			}
			if (t == typeof(decimal) || t == typeof(decimal?))
			{
				reader.ReadAsDecimal();
				return true;
			}
			if (t == typeof(DateTimeOffset) || t == typeof(DateTimeOffset?))
			{
				reader.ReadAsDateTimeOffset();
				return true;
			}
			do
			{
				if (!reader.Read())
				{
					return false;
				}
			}
			while (reader.TokenType == JsonToken.Comment);
			return true;
		}

		private object PopulateObject(object newObject, JsonReader reader, JsonObjectContract contract, string id)
		{
			contract.InvokeOnDeserializing(newObject, base.Serializer.Context);
			Dictionary<JsonProperty, PropertyPresence> dictionary = contract.Properties.ToDictionary((JsonProperty m) => m, (JsonProperty m) => PropertyPresence.None);
			if (id != null)
			{
				base.Serializer.ReferenceResolver.AddReference(this, id, newObject);
			}
			int depth = reader.Depth;
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.PropertyName:
				{
					string text = reader.Value.ToString();
					try
					{
						JsonProperty closestMatchProperty = contract.Properties.GetClosestMatchProperty(text);
						if (closestMatchProperty == null)
						{
							if (base.Serializer.MissingMemberHandling == MissingMemberHandling.Error)
							{
								throw new JsonSerializationException("Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, text, contract.UnderlyingType.Name));
							}
							reader.Skip();
						}
						else
						{
							if (!ReadForType(reader, closestMatchProperty.PropertyType, closestMatchProperty.Converter))
							{
								throw new JsonSerializationException("Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
							}
							SetPropertyPresence(reader, closestMatchProperty, dictionary);
							SetPropertyValue(closestMatchProperty, reader, newObject);
						}
					}
					catch (Exception ex)
					{
						if (!IsErrorHandled(newObject, contract, text, ex))
						{
							throw;
						}
						HandleError(reader, depth);
					}
					break;
				}
				case JsonToken.EndObject:
					foreach (KeyValuePair<JsonProperty, PropertyPresence> item in dictionary)
					{
						JsonProperty key = item.Key;
						switch (item.Value)
						{
						case PropertyPresence.None:
							if (key.Required == Required.AllowNull || key.Required == Required.Always)
							{
								throw new JsonSerializationException("Required property '{0}' not found in JSON.".FormatWith(CultureInfo.InvariantCulture, key.PropertyName));
							}
							if (HasFlag(key.DefaultValueHandling.GetValueOrDefault(base.Serializer.DefaultValueHandling), DefaultValueHandling.Populate) && key.Writable)
							{
								key.ValueProvider.SetValue(newObject, EnsureType(key.DefaultValue, CultureInfo.InvariantCulture, key.PropertyType));
							}
							break;
						case PropertyPresence.Null:
							if (key.Required == Required.Always)
							{
								throw new JsonSerializationException("Required property '{0}' expects a value but got null.".FormatWith(CultureInfo.InvariantCulture, key.PropertyName));
							}
							break;
						}
					}
					contract.InvokeOnDeserialized(newObject, base.Serializer.Context);
					return newObject;
				default:
					throw new JsonSerializationException("Unexpected token when deserializing object: " + reader.TokenType);
				case JsonToken.Comment:
					break;
				}
			}
			while (reader.Read());
			throw new JsonSerializationException("Unexpected end when deserializing object.");
		}

		private void SetPropertyPresence(JsonReader reader, JsonProperty property, Dictionary<JsonProperty, PropertyPresence> requiredProperties)
		{
			if (property != null)
			{
				requiredProperties[property] = ((reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.Undefined) ? PropertyPresence.Null : PropertyPresence.Value);
			}
		}

		private void HandleError(JsonReader reader, int initialDepth)
		{
			ClearErrorContext();
			reader.Skip();
			while (reader.Depth > initialDepth + 1)
			{
				reader.Read();
			}
		}
	}
}
