using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Newtonsoft.Json
{
	public class JsonValidatingReader : JsonReader, IJsonLineInfo
	{
		private class SchemaScope
		{
			private readonly JTokenType _tokenType;

			private readonly IList<JsonSchemaModel> _schemas;

			private readonly Dictionary<string, bool> _requiredProperties;

			public string CurrentPropertyName
			{
				get;
				set;
			}

			public int ArrayItemCount
			{
				get;
				set;
			}

			public IList<JsonSchemaModel> Schemas => _schemas;

			public Dictionary<string, bool> RequiredProperties => _requiredProperties;

			public JTokenType TokenType => _tokenType;

			public SchemaScope(JTokenType tokenType, IList<JsonSchemaModel> schemas)
			{
				_tokenType = tokenType;
				_schemas = schemas;
				_requiredProperties = schemas.SelectMany(GetRequiredProperties).Distinct().ToDictionary((string p) => p, (string p) => false);
			}

			private IEnumerable<string> GetRequiredProperties(JsonSchemaModel schema)
			{
				if (schema == null || schema.Properties == null)
				{
					return Enumerable.Empty<string>();
				}
				return from p in schema.Properties
				where p.Value.Required
				select p.Key;
			}
		}

		private readonly JsonReader _reader;

		private readonly Stack<SchemaScope> _stack;

		private JsonSchema _schema;

		private JsonSchemaModel _model;

		private SchemaScope _currentScope;

		int IJsonLineInfo.LineNumber
		{
			get
			{
				return (_reader as IJsonLineInfo)?.LineNumber ?? 0;
			}
		}

		int IJsonLineInfo.LinePosition
		{
			get
			{
				return (_reader as IJsonLineInfo)?.LinePosition ?? 0;
			}
		}

		public override object Value => _reader.Value;

		public override int Depth => _reader.Depth;

		public override char QuoteChar
		{
			get
			{
				return _reader.QuoteChar;
			}
			protected internal set
			{
			}
		}

		public override JsonToken TokenType => _reader.TokenType;

		public override Type ValueType => _reader.ValueType;

		private IEnumerable<JsonSchemaModel> CurrentSchemas => _currentScope.Schemas;

		private IEnumerable<JsonSchemaModel> CurrentMemberSchemas
		{
			get
			{
				if (_currentScope == null)
				{
					return new List<JsonSchemaModel>(new JsonSchemaModel[1]
					{
						_model
					});
				}
				if (_currentScope.Schemas == null || _currentScope.Schemas.Count == 0)
				{
					return Enumerable.Empty<JsonSchemaModel>();
				}
				switch (_currentScope.TokenType)
				{
				case JTokenType.None:
					return _currentScope.Schemas;
				case JTokenType.Object:
				{
					if (_currentScope.CurrentPropertyName == null)
					{
						throw new Exception("CurrentPropertyName has not been set on scope.");
					}
					IList<JsonSchemaModel> list2 = new List<JsonSchemaModel>();
					{
						foreach (JsonSchemaModel currentSchema in CurrentSchemas)
						{
							if (currentSchema.Properties != null && currentSchema.Properties.TryGetValue(_currentScope.CurrentPropertyName, out JsonSchemaModel value))
							{
								list2.Add(value);
							}
							if (currentSchema.PatternProperties != null)
							{
								foreach (KeyValuePair<string, JsonSchemaModel> patternProperty in currentSchema.PatternProperties)
								{
									if (Regex.IsMatch(_currentScope.CurrentPropertyName, patternProperty.Key))
									{
										list2.Add(patternProperty.Value);
									}
								}
							}
							if (list2.Count == 0 && currentSchema.AllowAdditionalProperties && currentSchema.AdditionalProperties != null)
							{
								list2.Add(currentSchema.AdditionalProperties);
							}
						}
						return list2;
					}
				}
				case JTokenType.Array:
				{
					IList<JsonSchemaModel> list = new List<JsonSchemaModel>();
					{
						foreach (JsonSchemaModel currentSchema2 in CurrentSchemas)
						{
							if (!CollectionUtils.IsNullOrEmpty(currentSchema2.Items))
							{
								if (currentSchema2.Items.Count == 1)
								{
									list.Add(currentSchema2.Items[0]);
								}
								if (currentSchema2.Items.Count > _currentScope.ArrayItemCount - 1)
								{
									list.Add(currentSchema2.Items[_currentScope.ArrayItemCount - 1]);
								}
							}
							if (currentSchema2.AllowAdditionalProperties && currentSchema2.AdditionalProperties != null)
							{
								list.Add(currentSchema2.AdditionalProperties);
							}
						}
						return list;
					}
				}
				case JTokenType.Constructor:
					return Enumerable.Empty<JsonSchemaModel>();
				default:
					throw new ArgumentOutOfRangeException("TokenType", "Unexpected token type: {0}".FormatWith(CultureInfo.InvariantCulture, _currentScope.TokenType));
				}
			}
		}

		public JsonSchema Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				if (TokenType != 0)
				{
					throw new Exception("Cannot change schema while validating JSON.");
				}
				_schema = value;
				_model = null;
			}
		}

		public JsonReader Reader => _reader;

		public event ValidationEventHandler ValidationEventHandler;

		public JsonValidatingReader(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			_reader = reader;
			_stack = new Stack<SchemaScope>();
		}

		private void Push(SchemaScope scope)
		{
			_stack.Push(scope);
			_currentScope = scope;
		}

		private SchemaScope Pop()
		{
			SchemaScope result = _stack.Pop();
			_currentScope = ((_stack.Count == 0) ? null : _stack.Peek());
			return result;
		}

		private void RaiseError(string message, JsonSchemaModel schema)
		{
			string message2 = (!((IJsonLineInfo)this).HasLineInfo()) ? message : (message + " Line {0}, position {1}.".FormatWith(CultureInfo.InvariantCulture, ((IJsonLineInfo)this).LineNumber, ((IJsonLineInfo)this).LinePosition));
			OnValidationEvent(new JsonSchemaException(message2, null, ((IJsonLineInfo)this).LineNumber, ((IJsonLineInfo)this).LinePosition));
		}

		private void OnValidationEvent(JsonSchemaException exception)
		{
			ValidationEventHandler validationEventHandler = this.ValidationEventHandler;
			if (validationEventHandler != null)
			{
				validationEventHandler(this, new ValidationEventArgs(exception));
				return;
			}
			throw exception;
		}

		private void ValidateInEnumAndNotDisallowed(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			JToken jToken = new JValue(_reader.Value);
			if (schema.Enum != null)
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				jToken.WriteTo(new JsonTextWriter(stringWriter));
				if (!schema.Enum.ContainsValue(jToken, new JTokenEqualityComparer()))
				{
					RaiseError("Value {0} is not defined in enum.".FormatWith(CultureInfo.InvariantCulture, stringWriter.ToString()), schema);
				}
			}
			JsonSchemaType? currentNodeSchemaType = GetCurrentNodeSchemaType();
			if (currentNodeSchemaType.HasValue && JsonSchemaGenerator.HasFlag(schema.Disallow, currentNodeSchemaType.Value))
			{
				RaiseError("Type {0} is disallowed.".FormatWith(CultureInfo.InvariantCulture, currentNodeSchemaType), schema);
			}
		}

		private JsonSchemaType? GetCurrentNodeSchemaType()
		{
			switch (_reader.TokenType)
			{
			case JsonToken.StartObject:
				return JsonSchemaType.Object;
			case JsonToken.StartArray:
				return JsonSchemaType.Array;
			case JsonToken.Integer:
				return JsonSchemaType.Integer;
			case JsonToken.Float:
				return JsonSchemaType.Float;
			case JsonToken.String:
				return JsonSchemaType.String;
			case JsonToken.Boolean:
				return JsonSchemaType.Boolean;
			case JsonToken.Null:
				return JsonSchemaType.Null;
			default:
				return null;
			}
		}

		public override byte[] ReadAsBytes()
		{
			byte[] result = _reader.ReadAsBytes();
			ValidateCurrentToken();
			return result;
		}

		public override decimal? ReadAsDecimal()
		{
			decimal? result = _reader.ReadAsDecimal();
			ValidateCurrentToken();
			return result;
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			DateTimeOffset? result = _reader.ReadAsDateTimeOffset();
			ValidateCurrentToken();
			return result;
		}

		public override bool Read()
		{
			if (!_reader.Read())
			{
				return false;
			}
			if (_reader.TokenType == JsonToken.Comment)
			{
				return true;
			}
			ValidateCurrentToken();
			return true;
		}

		private void ValidateCurrentToken()
		{
			if (_model == null)
			{
				JsonSchemaModelBuilder jsonSchemaModelBuilder = new JsonSchemaModelBuilder();
				_model = jsonSchemaModelBuilder.Build(_schema);
			}
			switch (_reader.TokenType)
			{
			case JsonToken.Raw:
				break;
			case JsonToken.Undefined:
				break;
			case JsonToken.Date:
				break;
			case JsonToken.StartObject:
			{
				ProcessValue();
				IList<JsonSchemaModel> schemas2 = CurrentMemberSchemas.Where(ValidateObject).ToList();
				Push(new SchemaScope(JTokenType.Object, schemas2));
				break;
			}
			case JsonToken.StartArray:
			{
				ProcessValue();
				IList<JsonSchemaModel> schemas = CurrentMemberSchemas.Where(ValidateArray).ToList();
				Push(new SchemaScope(JTokenType.Array, schemas));
				break;
			}
			case JsonToken.StartConstructor:
				Push(new SchemaScope(JTokenType.Constructor, null));
				break;
			case JsonToken.PropertyName:
				foreach (JsonSchemaModel currentSchema in CurrentSchemas)
				{
					ValidatePropertyName(currentSchema);
				}
				break;
			case JsonToken.Integer:
				ProcessValue();
				foreach (JsonSchemaModel currentMemberSchema in CurrentMemberSchemas)
				{
					ValidateInteger(currentMemberSchema);
				}
				break;
			case JsonToken.Float:
				ProcessValue();
				foreach (JsonSchemaModel currentMemberSchema2 in CurrentMemberSchemas)
				{
					ValidateFloat(currentMemberSchema2);
				}
				break;
			case JsonToken.String:
				ProcessValue();
				foreach (JsonSchemaModel currentMemberSchema3 in CurrentMemberSchemas)
				{
					ValidateString(currentMemberSchema3);
				}
				break;
			case JsonToken.Boolean:
				ProcessValue();
				foreach (JsonSchemaModel currentMemberSchema4 in CurrentMemberSchemas)
				{
					ValidateBoolean(currentMemberSchema4);
				}
				break;
			case JsonToken.Null:
				ProcessValue();
				foreach (JsonSchemaModel currentMemberSchema5 in CurrentMemberSchemas)
				{
					ValidateNull(currentMemberSchema5);
				}
				break;
			case JsonToken.EndObject:
				foreach (JsonSchemaModel currentSchema2 in CurrentSchemas)
				{
					ValidateEndObject(currentSchema2);
				}
				Pop();
				break;
			case JsonToken.EndArray:
				foreach (JsonSchemaModel currentSchema3 in CurrentSchemas)
				{
					ValidateEndArray(currentSchema3);
				}
				Pop();
				break;
			case JsonToken.EndConstructor:
				Pop();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void ValidateEndObject(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			Dictionary<string, bool> requiredProperties = _currentScope.RequiredProperties;
			if (requiredProperties != null)
			{
				List<string> list = (from kv in requiredProperties
				where !kv.Value
				select kv.Key).ToList();
				if (list.Count > 0)
				{
					RaiseError("Required properties are missing from object: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", list.ToArray())), schema);
				}
			}
		}

		private void ValidateEndArray(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			int arrayItemCount = _currentScope.ArrayItemCount;
			if (schema.MaximumItems.HasValue)
			{
				int? maximumItems = schema.MaximumItems;
				if (maximumItems.HasValue && arrayItemCount > maximumItems.GetValueOrDefault())
				{
					RaiseError("Array item count {0} exceeds maximum count of {1}.".FormatWith(CultureInfo.InvariantCulture, arrayItemCount, schema.MaximumItems), schema);
				}
			}
			if (schema.MinimumItems.HasValue)
			{
				int? minimumItems = schema.MinimumItems;
				if (minimumItems.HasValue && arrayItemCount < minimumItems.GetValueOrDefault())
				{
					RaiseError("Array item count {0} is less than minimum count of {1}.".FormatWith(CultureInfo.InvariantCulture, arrayItemCount, schema.MinimumItems), schema);
				}
			}
		}

		private void ValidateNull(JsonSchemaModel schema)
		{
			if (schema != null && TestType(schema, JsonSchemaType.Null))
			{
				ValidateInEnumAndNotDisallowed(schema);
			}
		}

		private void ValidateBoolean(JsonSchemaModel schema)
		{
			if (schema != null && TestType(schema, JsonSchemaType.Boolean))
			{
				ValidateInEnumAndNotDisallowed(schema);
			}
		}

		private void ValidateString(JsonSchemaModel schema)
		{
			if (schema == null || !TestType(schema, JsonSchemaType.String))
			{
				return;
			}
			ValidateInEnumAndNotDisallowed(schema);
			string text = _reader.Value.ToString();
			if (schema.MaximumLength.HasValue)
			{
				int? maximumLength = schema.MaximumLength;
				if (maximumLength.HasValue && text.Length > maximumLength.GetValueOrDefault())
				{
					RaiseError("String '{0}' exceeds maximum length of {1}.".FormatWith(CultureInfo.InvariantCulture, text, schema.MaximumLength), schema);
				}
			}
			if (schema.MinimumLength.HasValue)
			{
				int? minimumLength = schema.MinimumLength;
				if (minimumLength.HasValue && text.Length < minimumLength.GetValueOrDefault())
				{
					RaiseError("String '{0}' is less than minimum length of {1}.".FormatWith(CultureInfo.InvariantCulture, text, schema.MinimumLength), schema);
				}
			}
			if (schema.Patterns != null)
			{
				foreach (string pattern in schema.Patterns)
				{
					if (!Regex.IsMatch(text, pattern))
					{
						RaiseError("String '{0}' does not match regex pattern '{1}'.".FormatWith(CultureInfo.InvariantCulture, text, pattern), schema);
					}
				}
			}
		}

		private void ValidateInteger(JsonSchemaModel schema)
		{
			if (schema == null || !TestType(schema, JsonSchemaType.Integer))
			{
				return;
			}
			ValidateInEnumAndNotDisallowed(schema);
			long num = Convert.ToInt64(_reader.Value, CultureInfo.InvariantCulture);
			if (schema.Maximum.HasValue)
			{
				double? maximum = schema.Maximum;
				if (maximum.HasValue && (double)num > maximum.GetValueOrDefault())
				{
					RaiseError("Integer {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, num, schema.Maximum), schema);
				}
				if (schema.ExclusiveMaximum && (double)num == schema.Maximum)
				{
					RaiseError("Integer {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, num, schema.Maximum), schema);
				}
			}
			if (schema.Minimum.HasValue)
			{
				double? minimum = schema.Minimum;
				if (minimum.HasValue && (double)num < minimum.GetValueOrDefault())
				{
					RaiseError("Integer {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, num, schema.Minimum), schema);
				}
				if (schema.ExclusiveMinimum && (double)num == schema.Minimum)
				{
					RaiseError("Integer {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, num, schema.Minimum), schema);
				}
			}
			if (schema.DivisibleBy.HasValue && !IsZero((double)num % schema.DivisibleBy.Value))
			{
				RaiseError("Integer {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.DivisibleBy), schema);
			}
		}

		private void ProcessValue()
		{
			if (_currentScope != null && _currentScope.TokenType == JTokenType.Array)
			{
				_currentScope.ArrayItemCount++;
				foreach (JsonSchemaModel currentSchema in CurrentSchemas)
				{
					if (currentSchema != null && currentSchema.Items != null && currentSchema.Items.Count > 1 && _currentScope.ArrayItemCount >= currentSchema.Items.Count)
					{
						RaiseError("Index {0} has not been defined and the schema does not allow additional items.".FormatWith(CultureInfo.InvariantCulture, _currentScope.ArrayItemCount), currentSchema);
					}
				}
			}
		}

		private void ValidateFloat(JsonSchemaModel schema)
		{
			if (schema == null || !TestType(schema, JsonSchemaType.Float))
			{
				return;
			}
			ValidateInEnumAndNotDisallowed(schema);
			double num = Convert.ToDouble(_reader.Value, CultureInfo.InvariantCulture);
			if (schema.Maximum.HasValue)
			{
				double? maximum = schema.Maximum;
				if (maximum.HasValue && num > maximum.GetValueOrDefault())
				{
					RaiseError("Float {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Maximum), schema);
				}
				if (schema.ExclusiveMaximum && num == schema.Maximum)
				{
					RaiseError("Float {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Maximum), schema);
				}
			}
			if (schema.Minimum.HasValue)
			{
				double? minimum = schema.Minimum;
				if (minimum.HasValue && num < minimum.GetValueOrDefault())
				{
					RaiseError("Float {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Minimum), schema);
				}
				if (schema.ExclusiveMinimum && num == schema.Minimum)
				{
					RaiseError("Float {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Minimum), schema);
				}
			}
			if (schema.DivisibleBy.HasValue && !IsZero(num % schema.DivisibleBy.Value))
			{
				RaiseError("Float {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.DivisibleBy), schema);
			}
		}

		private static bool IsZero(double value)
		{
			double num = 2.2204460492503131E-16;
			return Math.Abs(value) < 10.0 * num;
		}

		private void ValidatePropertyName(JsonSchemaModel schema)
		{
			if (schema != null)
			{
				string text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
				if (_currentScope.RequiredProperties.ContainsKey(text))
				{
					_currentScope.RequiredProperties[text] = true;
				}
				if (!schema.AllowAdditionalProperties && !IsPropertyDefinied(schema, text))
				{
					RaiseError("Property '{0}' has not been defined and the schema does not allow additional properties.".FormatWith(CultureInfo.InvariantCulture, text), schema);
				}
				_currentScope.CurrentPropertyName = text;
			}
		}

		private bool IsPropertyDefinied(JsonSchemaModel schema, string propertyName)
		{
			if (schema.Properties != null && schema.Properties.ContainsKey(propertyName))
			{
				return true;
			}
			if (schema.PatternProperties != null)
			{
				foreach (string key in schema.PatternProperties.Keys)
				{
					if (Regex.IsMatch(propertyName, key))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool ValidateArray(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return true;
			}
			return TestType(schema, JsonSchemaType.Array);
		}

		private bool ValidateObject(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return true;
			}
			return TestType(schema, JsonSchemaType.Object);
		}

		private bool TestType(JsonSchemaModel currentSchema, JsonSchemaType currentType)
		{
			if (!JsonSchemaGenerator.HasFlag(currentSchema.Type, currentType))
			{
				RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith(CultureInfo.InvariantCulture, currentSchema.Type, currentType), currentSchema);
				return false;
			}
			return true;
		}

		bool IJsonLineInfo.HasLineInfo()
		{
			return (_reader as IJsonLineInfo)?.HasLineInfo() ?? false;
		}
	}
}
