using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;

#if STANDALONE_SCRIPT_COMPILER
using BGICompiler.Compiler.Logger;
#else
using UnityEngine;
#endif

namespace Newtonsoft.Json.Serialization
{
	internal class JsonSerializerInternalWriter : JsonSerializerInternalBase
	{
		private JsonSerializerProxy _internalSerializer;

		private List<object> _serializeStack;

		private List<object> SerializeStack
		{
			get
			{
				if (_serializeStack == null)
				{
					_serializeStack = new List<object>();
				}
				return _serializeStack;
			}
		}

		public JsonSerializerInternalWriter(JsonSerializer serializer)
			: base(serializer)
		{
		}

		public void Serialize(JsonWriter jsonWriter, object value)
		{
			if (jsonWriter == null)
			{
				throw new ArgumentNullException("jsonWriter");
			}
			SerializeValue(jsonWriter, value, GetContractSafe(value), null, null);
		}

		private JsonSerializerProxy GetInternalSerializer()
		{
			if (_internalSerializer == null)
			{
				_internalSerializer = new JsonSerializerProxy(this);
			}
			return _internalSerializer;
		}

		private JsonContract GetContractSafe(object value)
		{
			if (value == null)
			{
				return null;
			}
			return base.Serializer.ContractResolver.ResolveContract(value.GetType());
		}

		private void SerializePrimitive(JsonWriter writer, object value, JsonPrimitiveContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			if (contract.UnderlyingType == typeof(byte[]) && ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				writer.WriteStartObject();
				WriteTypeProperty(writer, contract.CreatedType);
				writer.WritePropertyName("$value");
				writer.WriteValue(value);
				writer.WriteEndObject();
			}
			else
			{
				writer.WriteValue(value);
			}
		}

		private void SerializeValue(JsonWriter writer, object value, JsonContract valueContract, JsonProperty member, JsonContract collectionValueContract)
		{
			JsonConverter jsonConverter = member?.Converter;
			if (value == null)
			{
				writer.WriteNull();
			}
			else if ((jsonConverter != null || (jsonConverter = valueContract.Converter) != null || (jsonConverter = base.Serializer.GetMatchingConverter(valueContract.UnderlyingType)) != null || (jsonConverter = valueContract.InternalConverter) != null) && jsonConverter.CanWrite)
			{
				SerializeConvertable(writer, jsonConverter, value, valueContract);
			}
			else if (valueContract is JsonPrimitiveContract)
			{
				SerializePrimitive(writer, value, (JsonPrimitiveContract)valueContract, member, collectionValueContract);
			}
			else if (valueContract is JsonStringContract)
			{
				SerializeString(writer, value, (JsonStringContract)valueContract);
			}
			else if (valueContract is JsonObjectContract)
			{
				SerializeObject(writer, value, (JsonObjectContract)valueContract, member, collectionValueContract);
			}
			else if (valueContract is JsonDictionaryContract)
			{
				JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)valueContract;
				SerializeDictionary(writer, jsonDictionaryContract.CreateWrapper(value), jsonDictionaryContract, member, collectionValueContract);
			}
			else if (valueContract is JsonArrayContract)
			{
				JsonArrayContract jsonArrayContract = (JsonArrayContract)valueContract;
				SerializeList(writer, jsonArrayContract.CreateWrapper(value), jsonArrayContract, member, collectionValueContract);
			}
			else if (valueContract is JsonLinqContract)
			{
				((JToken)value).WriteTo(writer, (base.Serializer.Converters == null) ? null : base.Serializer.Converters.ToArray());
			}
			else if (valueContract is JsonISerializableContract)
			{
				SerializeISerializable(writer, (ISerializable)value, (JsonISerializableContract)valueContract);
			}
		}

		private bool ShouldWriteReference(object value, JsonProperty property, JsonContract contract)
		{
			if (value == null)
			{
				return false;
			}
			if (contract is JsonPrimitiveContract)
			{
				return false;
			}
			bool? flag = null;
			if (property != null)
			{
				flag = property.IsReference;
			}
			if (!flag.HasValue)
			{
				flag = contract.IsReference;
			}
			if (!flag.HasValue)
			{
				flag = ((!(contract is JsonArrayContract)) ? new bool?(HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects)) : new bool?(HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Arrays)));
			}
			if (!flag.Value)
			{
				return false;
			}
			return base.Serializer.ReferenceResolver.IsReferenced(this, value);
		}

		private void WriteMemberInfoProperty(JsonWriter writer, object memberValue, JsonProperty property, JsonContract contract)
		{
			string propertyName = property.PropertyName;
			object defaultValue = property.DefaultValue;
			if ((property.NullValueHandling.GetValueOrDefault(base.Serializer.NullValueHandling) != NullValueHandling.Ignore || memberValue != null) && (!HasFlag(property.DefaultValueHandling.GetValueOrDefault(base.Serializer.DefaultValueHandling), DefaultValueHandling.Ignore) || !MiscellaneousUtils.ValueEquals(memberValue, defaultValue)))
			{
				if (ShouldWriteReference(memberValue, property, contract))
				{
					writer.WritePropertyName(propertyName);
					WriteReference(writer, memberValue);
				}
				else if (CheckForCircularReference(memberValue, property.ReferenceLoopHandling, contract))
				{
					if (memberValue == null && property.Required == Required.Always)
					{
						throw new JsonSerializationException("Cannot write a null value for property '{0}'. Property requires a value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
					}
					writer.WritePropertyName(propertyName);
					SerializeValue(writer, memberValue, contract, property, null);
				}
			}
		}

		private bool CheckForCircularReference(object value, ReferenceLoopHandling? referenceLoopHandling, JsonContract contract)
		{
			if (value == null || contract is JsonPrimitiveContract)
			{
				return true;
			}
			if (SerializeStack.IndexOf(value) != -1)
			{
#if STANDALONE_SCRIPT_COMPILER
				Debug.LogWarning("CheckForCircularReference() is not fully implemented in NewtonSoft.Json");
				return false;
#else
				switch ((value is Vector2 || value is Vector3 || value is Vector4 || value is Color || value is Color32) ? ReferenceLoopHandling.Ignore : referenceLoopHandling.GetValueOrDefault(base.Serializer.ReferenceLoopHandling))
				{
				case ReferenceLoopHandling.Error:
					throw new JsonSerializationException("Self referencing loop detected for type '{0}'.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
				case ReferenceLoopHandling.Ignore:
					return false;
				case ReferenceLoopHandling.Serialize:
					return true;
				default:
					throw new InvalidOperationException("Unexpected ReferenceLoopHandling value: '{0}'".FormatWith(CultureInfo.InvariantCulture, base.Serializer.ReferenceLoopHandling));
				}
#endif
			}
			return true;
		}

		private void WriteReference(JsonWriter writer, object value)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("$ref");
			writer.WriteValue(base.Serializer.ReferenceResolver.GetReference(this, value));
			writer.WriteEndObject();
		}

		internal static bool TryConvertToString(object value, Type type, out string s)
		{
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && !(converter is ComponentConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
			{
				s = converter.ConvertToInvariantString(value);
				return true;
			}
			if (value is Type)
			{
				s = ((Type)value).AssemblyQualifiedName;
				return true;
			}
			s = null;
			return false;
		}

		private void SerializeString(JsonWriter writer, object value, JsonStringContract contract)
		{
			contract.InvokeOnSerializing(value, base.Serializer.Context);
			TryConvertToString(value, contract.UnderlyingType, out string s);
			writer.WriteValue(s);
			contract.InvokeOnSerialized(value, base.Serializer.Context);
		}

		private void SerializeObject(JsonWriter writer, object value, JsonObjectContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			contract.InvokeOnSerializing(value, base.Serializer.Context);
			SerializeStack.Add(value);
			writer.WriteStartObject();
			bool? isReference = contract.IsReference;
			if ((!isReference.HasValue) ? HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects) : isReference.Value)
			{
				writer.WritePropertyName("$id");
				writer.WriteValue(base.Serializer.ReferenceResolver.GetReference(this, value));
			}
			if (ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				WriteTypeProperty(writer, contract.UnderlyingType);
			}
			int top = writer.Top;
			foreach (JsonProperty property in contract.Properties)
			{
				try
				{
					if (!property.Ignored && property.Readable && ShouldSerialize(property, value) && IsSpecified(property, value))
					{
						object value2 = property.ValueProvider.GetValue(value);
						JsonContract contractSafe = GetContractSafe(value2);
						WriteMemberInfoProperty(writer, value2, property, contractSafe);
					}
				}
				catch (Exception ex)
				{
					if (!IsErrorHandled(value, contract, property.PropertyName, ex))
					{
						throw;
					}
					HandleError(writer, top);
				}
			}
			writer.WriteEndObject();
			SerializeStack.RemoveAt(SerializeStack.Count - 1);
			contract.InvokeOnSerialized(value, base.Serializer.Context);
		}

		private void WriteTypeProperty(JsonWriter writer, Type type)
		{
			writer.WritePropertyName("$type");
			writer.WriteValue(ReflectionUtils.GetTypeName(type, base.Serializer.TypeNameAssemblyFormat, base.Serializer.Binder));
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool HasFlag(PreserveReferencesHandling value, PreserveReferencesHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool HasFlag(TypeNameHandling value, TypeNameHandling flag)
		{
			return (value & flag) == flag;
		}

		private void SerializeConvertable(JsonWriter writer, JsonConverter converter, object value, JsonContract contract)
		{
			if (ShouldWriteReference(value, null, contract))
			{
				WriteReference(writer, value);
			}
			else if (CheckForCircularReference(value, null, contract))
			{
				SerializeStack.Add(value);
				converter.WriteJson(writer, value, GetInternalSerializer());
				SerializeStack.RemoveAt(SerializeStack.Count - 1);
			}
		}

		private void SerializeList(JsonWriter writer, IWrappedCollection values, JsonArrayContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			contract.InvokeOnSerializing(values.UnderlyingCollection, base.Serializer.Context);
			SerializeStack.Add(values.UnderlyingCollection);
			bool? isReference = contract.IsReference;
			bool flag = (!isReference.HasValue) ? HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Arrays) : isReference.Value;
			bool flag2 = ShouldWriteType(TypeNameHandling.Arrays, contract, member, collectionValueContract);
			if (flag || flag2)
			{
				writer.WriteStartObject();
				if (flag)
				{
					writer.WritePropertyName("$id");
					writer.WriteValue(base.Serializer.ReferenceResolver.GetReference(this, values.UnderlyingCollection));
				}
				if (flag2)
				{
					WriteTypeProperty(writer, values.UnderlyingCollection.GetType());
				}
				writer.WritePropertyName("$values");
			}
			JsonContract collectionValueContract2 = base.Serializer.ContractResolver.ResolveContract(contract.CollectionItemType ?? typeof(object));
			writer.WriteStartArray();
			int top = writer.Top;
			int num = 0;
			foreach (object value in values)
			{
				try
				{
					JsonContract contractSafe = GetContractSafe(value);
					if (ShouldWriteReference(value, null, contractSafe))
					{
						WriteReference(writer, value);
					}
					else if (CheckForCircularReference(value, null, contract))
					{
						SerializeValue(writer, value, contractSafe, null, collectionValueContract2);
					}
				}
				catch (Exception ex)
				{
					if (!IsErrorHandled(values.UnderlyingCollection, contract, num, ex))
					{
						throw;
					}
					HandleError(writer, top);
				}
				finally
				{
					num++;
				}
			}
			writer.WriteEndArray();
			if (flag || flag2)
			{
				writer.WriteEndObject();
			}
			SerializeStack.RemoveAt(SerializeStack.Count - 1);
			contract.InvokeOnSerialized(values.UnderlyingCollection, base.Serializer.Context);
		}

		[SecuritySafeCritical]
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Security.SecuritySafeCriticalAttribute")]
		private void SerializeISerializable(JsonWriter writer, ISerializable value, JsonISerializableContract contract)
		{
			contract.InvokeOnSerializing(value, base.Serializer.Context);
			SerializeStack.Add(value);
			writer.WriteStartObject();
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, new FormatterConverter());
			value.GetObjectData(serializationInfo, base.Serializer.Context);
			SerializationInfoEnumerator enumerator = serializationInfo.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SerializationEntry current = enumerator.Current;
				writer.WritePropertyName(current.Name);
				SerializeValue(writer, current.Value, GetContractSafe(current.Value), null, null);
			}
			writer.WriteEndObject();
			SerializeStack.RemoveAt(SerializeStack.Count - 1);
			contract.InvokeOnSerialized(value, base.Serializer.Context);
		}

		private bool ShouldWriteType(TypeNameHandling typeNameHandlingFlag, JsonContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			TypeNameHandling? typeNameHandling = member?.TypeNameHandling;
			if (HasFlag((!typeNameHandling.HasValue) ? base.Serializer.TypeNameHandling : typeNameHandling.Value, typeNameHandlingFlag))
			{
				return true;
			}
			if (member != null)
			{
				TypeNameHandling? typeNameHandling2 = member.TypeNameHandling;
				if (((!typeNameHandling2.HasValue) ? base.Serializer.TypeNameHandling : typeNameHandling2.Value) == TypeNameHandling.Auto && contract.UnderlyingType != member.PropertyType)
				{
					JsonContract jsonContract = base.Serializer.ContractResolver.ResolveContract(member.PropertyType);
					if (contract.UnderlyingType != jsonContract.CreatedType)
					{
						return true;
					}
				}
			}
			else if (collectionValueContract != null && base.Serializer.TypeNameHandling == TypeNameHandling.Auto && contract.UnderlyingType != collectionValueContract.UnderlyingType)
			{
				return true;
			}
			return false;
		}

		private void SerializeDictionary(JsonWriter writer, IWrappedDictionary values, JsonDictionaryContract contract, JsonProperty member, JsonContract collectionValueContract)
		{
			contract.InvokeOnSerializing(values.UnderlyingDictionary, base.Serializer.Context);
			SerializeStack.Add(values.UnderlyingDictionary);
			writer.WriteStartObject();
			bool? isReference = contract.IsReference;
			if ((!isReference.HasValue) ? HasFlag(base.Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects) : isReference.Value)
			{
				writer.WritePropertyName("$id");
				writer.WriteValue(base.Serializer.ReferenceResolver.GetReference(this, values.UnderlyingDictionary));
			}
			if (ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
			{
				WriteTypeProperty(writer, values.UnderlyingDictionary.GetType());
			}
			JsonContract collectionValueContract2 = base.Serializer.ContractResolver.ResolveContract(contract.DictionaryValueType ?? typeof(object));
			int top = writer.Top;
			IDictionaryEnumerator enumerator = values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry entry = (DictionaryEntry)enumerator.Current;
					string propertyName = GetPropertyName(entry);
					propertyName = ((contract.PropertyNameResolver == null) ? propertyName : contract.PropertyNameResolver(propertyName));
					try
					{
						object value = entry.Value;
						JsonContract contractSafe = GetContractSafe(value);
						if (ShouldWriteReference(value, null, contractSafe))
						{
							writer.WritePropertyName(propertyName);
							WriteReference(writer, value);
						}
						else if (CheckForCircularReference(value, null, contract))
						{
							writer.WritePropertyName(propertyName);
							SerializeValue(writer, value, contractSafe, null, collectionValueContract2);
						}
					}
					catch (Exception ex)
					{
						if (!IsErrorHandled(values.UnderlyingDictionary, contract, propertyName, ex))
						{
							throw;
						}
						HandleError(writer, top);
					}
				}
			}
			finally
			{
				(enumerator as IDisposable)?.Dispose();
			}
			writer.WriteEndObject();
			SerializeStack.RemoveAt(SerializeStack.Count - 1);
			contract.InvokeOnSerialized(values.UnderlyingDictionary, base.Serializer.Context);
		}

		private string GetPropertyName(DictionaryEntry entry)
		{
			if (entry.Key is IConvertible)
			{
				return Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
			}
			if (TryConvertToString(entry.Key, entry.Key.GetType(), out string s))
			{
				return s;
			}
			return entry.Key.ToString();
		}

		private void HandleError(JsonWriter writer, int initialDepth)
		{
			ClearErrorContext();
			while (writer.Top > initialDepth)
			{
				writer.WriteEnd();
			}
		}

		private bool ShouldSerialize(JsonProperty property, object target)
		{
			if (property.ShouldSerialize == null)
			{
				return true;
			}
			return property.ShouldSerialize(target);
		}

		private bool IsSpecified(JsonProperty property, object target)
		{
			if (property.GetIsSpecified == null)
			{
				return true;
			}
			return property.GetIsSpecified(target);
		}
	}
}
