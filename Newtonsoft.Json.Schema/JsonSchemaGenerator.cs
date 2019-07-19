using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
	public class JsonSchemaGenerator
	{
		private class TypeSchema
		{
			public Type Type
			{
				get;
				private set;
			}

			public JsonSchema Schema
			{
				get;
				private set;
			}

			public TypeSchema(Type type, JsonSchema schema)
			{
				ValidationUtils.ArgumentNotNull(type, "type");
				ValidationUtils.ArgumentNotNull(schema, "schema");
				Type = type;
				Schema = schema;
			}
		}

		private IContractResolver _contractResolver;

		private JsonSchemaResolver _resolver;

		private IList<TypeSchema> _stack = new List<TypeSchema>();

		private JsonSchema _currentSchema;

		public UndefinedSchemaIdHandling UndefinedSchemaIdHandling
		{
			get;
			set;
		}

		public IContractResolver ContractResolver
		{
			get
			{
				if (_contractResolver == null)
				{
					return DefaultContractResolver.Instance;
				}
				return _contractResolver;
			}
			set
			{
				_contractResolver = value;
			}
		}

		private JsonSchema CurrentSchema => _currentSchema;

		private void Push(TypeSchema typeSchema)
		{
			_currentSchema = typeSchema.Schema;
			_stack.Add(typeSchema);
			_resolver.LoadedSchemas.Add(typeSchema.Schema);
		}

		private TypeSchema Pop()
		{
			TypeSchema result = _stack[_stack.Count - 1];
			_stack.RemoveAt(_stack.Count - 1);
			TypeSchema typeSchema = _stack.LastOrDefault();
			if (typeSchema != null)
			{
				_currentSchema = typeSchema.Schema;
			}
			else
			{
				_currentSchema = null;
			}
			return result;
		}

		public JsonSchema Generate(Type type)
		{
			return Generate(type, new JsonSchemaResolver(), rootSchemaNullable: false);
		}

		public JsonSchema Generate(Type type, JsonSchemaResolver resolver)
		{
			return Generate(type, resolver, rootSchemaNullable: false);
		}

		public JsonSchema Generate(Type type, bool rootSchemaNullable)
		{
			return Generate(type, new JsonSchemaResolver(), rootSchemaNullable);
		}

		public JsonSchema Generate(Type type, JsonSchemaResolver resolver, bool rootSchemaNullable)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			ValidationUtils.ArgumentNotNull(resolver, "resolver");
			_resolver = resolver;
			return GenerateInternal(type, (!rootSchemaNullable) ? Required.Always : Required.Default, required: false);
		}

		private string GetTitle(Type type)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(type);
			if (jsonContainerAttribute != null && !string.IsNullOrEmpty(jsonContainerAttribute.Title))
			{
				return jsonContainerAttribute.Title;
			}
			return null;
		}

		private string GetDescription(Type type)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(type);
			if (jsonContainerAttribute != null && !string.IsNullOrEmpty(jsonContainerAttribute.Description))
			{
				return jsonContainerAttribute.Description;
			}
			return ReflectionUtils.GetAttribute<DescriptionAttribute>(type)?.Description;
		}

		private string GetTypeId(Type type, bool explicitOnly)
		{
			JsonContainerAttribute jsonContainerAttribute = JsonTypeReflector.GetJsonContainerAttribute(type);
			if (jsonContainerAttribute != null && !string.IsNullOrEmpty(jsonContainerAttribute.Id))
			{
				return jsonContainerAttribute.Id;
			}
			if (explicitOnly)
			{
				return null;
			}
			switch (UndefinedSchemaIdHandling)
			{
			case UndefinedSchemaIdHandling.UseTypeName:
				return type.FullName;
			case UndefinedSchemaIdHandling.UseAssemblyQualifiedName:
				return type.AssemblyQualifiedName;
			default:
				return null;
			}
		}

		private JsonSchema GenerateInternal(Type type, Required valueRequired, bool required)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			string typeId = GetTypeId(type, explicitOnly: false);
			string typeId2 = GetTypeId(type, explicitOnly: true);
			if (!string.IsNullOrEmpty(typeId))
			{
				JsonSchema schema = _resolver.GetSchema(typeId);
				if (schema != null)
				{
					if (valueRequired != Required.Always && !HasFlag(schema.Type, JsonSchemaType.Null))
					{
						schema.Type |= JsonSchemaType.Null;
					}
					if (required && schema.Required != true)
					{
						schema.Required = true;
					}
					return schema;
				}
			}
			if (_stack.Any((TypeSchema tc) => tc.Type == type))
			{
				throw new Exception("Unresolved circular reference for type '{0}'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property.".FormatWith(CultureInfo.InvariantCulture, type));
			}
			JsonContract jsonContract = ContractResolver.ResolveContract(type);
			JsonConverter jsonConverter;
			if ((jsonConverter = jsonContract.Converter) != null || (jsonConverter = jsonContract.InternalConverter) != null)
			{
				JsonSchema schema2 = jsonConverter.GetSchema();
				if (schema2 != null)
				{
					return schema2;
				}
			}
			Push(new TypeSchema(type, new JsonSchema()));
			if (typeId2 != null)
			{
				CurrentSchema.Id = typeId2;
			}
			if (required)
			{
				CurrentSchema.Required = true;
			}
			CurrentSchema.Title = GetTitle(type);
			CurrentSchema.Description = GetDescription(type);
			if (jsonConverter != null)
			{
				CurrentSchema.Type = JsonSchemaType.Any;
			}
			else if (jsonContract is JsonDictionaryContract)
			{
				CurrentSchema.Type = AddNullType(JsonSchemaType.Object, valueRequired);
				ReflectionUtils.GetDictionaryKeyValueTypes(type, out Type keyType, out Type valueType);
				if (keyType != null && typeof(IConvertible).IsAssignableFrom(keyType))
				{
					CurrentSchema.AdditionalProperties = GenerateInternal(valueType, Required.Default, required: false);
				}
			}
			else if (jsonContract is JsonArrayContract)
			{
				CurrentSchema.Type = AddNullType(JsonSchemaType.Array, valueRequired);
				CurrentSchema.Id = GetTypeId(type, explicitOnly: false);
				bool flag = (JsonTypeReflector.GetJsonContainerAttribute(type) as JsonArrayAttribute)?.AllowNullItems ?? true;
				Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
				if (collectionItemType != null)
				{
					CurrentSchema.Items = new List<JsonSchema>();
					CurrentSchema.Items.Add(GenerateInternal(collectionItemType, (!flag) ? Required.Always : Required.Default, required: false));
				}
			}
			else if (jsonContract is JsonPrimitiveContract)
			{
				CurrentSchema.Type = GetJsonSchemaType(type, valueRequired);
				if (CurrentSchema.Type == JsonSchemaType.Integer && type.IsEnum && !type.IsDefined(typeof(FlagsAttribute), inherit: true))
				{
					CurrentSchema.Enum = new List<JToken>();
					CurrentSchema.Options = new Dictionary<JToken, string>();
					EnumValues<long> namesAndValues = EnumUtils.GetNamesAndValues<long>(type);
					foreach (EnumValue<long> item in namesAndValues)
					{
						JToken jToken = JToken.FromObject(item.Value);
						CurrentSchema.Enum.Add(jToken);
						CurrentSchema.Options.Add(jToken, item.Name);
					}
				}
			}
			else if (jsonContract is JsonObjectContract)
			{
				CurrentSchema.Type = AddNullType(JsonSchemaType.Object, valueRequired);
				CurrentSchema.Id = GetTypeId(type, explicitOnly: false);
				GenerateObjectSchema(type, (JsonObjectContract)jsonContract);
			}
			else if (jsonContract is JsonISerializableContract)
			{
				CurrentSchema.Type = AddNullType(JsonSchemaType.Object, valueRequired);
				CurrentSchema.Id = GetTypeId(type, explicitOnly: false);
				GenerateISerializableContract(type, (JsonISerializableContract)jsonContract);
			}
			else if (jsonContract is JsonStringContract)
			{
				JsonSchemaType value = (!ReflectionUtils.IsNullable(jsonContract.UnderlyingType)) ? JsonSchemaType.String : AddNullType(JsonSchemaType.String, valueRequired);
				CurrentSchema.Type = value;
			}
			else
			{
				if (!(jsonContract is JsonLinqContract))
				{
					throw new Exception("Unexpected contract type: {0}".FormatWith(CultureInfo.InvariantCulture, jsonContract));
				}
				CurrentSchema.Type = JsonSchemaType.Any;
			}
			return Pop().Schema;
		}

		private JsonSchemaType AddNullType(JsonSchemaType type, Required valueRequired)
		{
			if (valueRequired != Required.Always)
			{
				return type | JsonSchemaType.Null;
			}
			return type;
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		private void GenerateObjectSchema(Type type, JsonObjectContract contract)
		{
			CurrentSchema.Properties = new Dictionary<string, JsonSchema>();
			foreach (JsonProperty property in contract.Properties)
			{
				if (!property.Ignored)
				{
					bool flag = property.NullValueHandling == NullValueHandling.Ignore || HasFlag(property.DefaultValueHandling.GetValueOrDefault(), DefaultValueHandling.Ignore) || property.ShouldSerialize != null || property.GetIsSpecified != null;
					JsonSchema jsonSchema = GenerateInternal(property.PropertyType, property.Required, !flag);
					if (property.DefaultValue != null)
					{
						jsonSchema.Default = JToken.FromObject(property.DefaultValue);
					}
					CurrentSchema.Properties.Add(property.PropertyName, jsonSchema);
				}
			}
			if (type.IsSealed)
			{
				CurrentSchema.AllowAdditionalProperties = false;
			}
		}

		private void GenerateISerializableContract(Type type, JsonISerializableContract contract)
		{
			CurrentSchema.AllowAdditionalProperties = true;
		}

		internal static bool HasFlag(JsonSchemaType? value, JsonSchemaType flag)
		{
			if (!value.HasValue)
			{
				return true;
			}
			return (value & flag) == flag;
		}

		private JsonSchemaType GetJsonSchemaType(Type type, Required valueRequired)
		{
			JsonSchemaType jsonSchemaType = JsonSchemaType.None;
			if (valueRequired != Required.Always && ReflectionUtils.IsNullable(type))
			{
				jsonSchemaType = JsonSchemaType.Null;
				if (ReflectionUtils.IsNullableType(type))
				{
					type = Nullable.GetUnderlyingType(type);
				}
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode)
			{
			case TypeCode.Empty:
			case TypeCode.Object:
				return jsonSchemaType | JsonSchemaType.String;
			case TypeCode.DBNull:
				return jsonSchemaType | JsonSchemaType.Null;
			case TypeCode.Boolean:
				return jsonSchemaType | JsonSchemaType.Boolean;
			case TypeCode.Char:
				return jsonSchemaType | JsonSchemaType.String;
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return jsonSchemaType | JsonSchemaType.Integer;
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return jsonSchemaType | JsonSchemaType.Float;
			case TypeCode.DateTime:
				return jsonSchemaType | JsonSchemaType.String;
			case TypeCode.String:
				return jsonSchemaType | JsonSchemaType.String;
			default:
				throw new Exception("Unexpected type code '{0}' for type '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeCode, type));
			}
		}
	}
}
