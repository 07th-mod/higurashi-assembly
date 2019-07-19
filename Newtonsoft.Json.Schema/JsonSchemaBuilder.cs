using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaBuilder
	{
		private JsonReader _reader;

		private readonly IList<JsonSchema> _stack;

		private readonly JsonSchemaResolver _resolver;

		private JsonSchema _currentSchema;

		private JsonSchema CurrentSchema => _currentSchema;

		public JsonSchemaBuilder(JsonSchemaResolver resolver)
		{
			_stack = new List<JsonSchema>();
			_resolver = resolver;
		}

		private void Push(JsonSchema value)
		{
			_currentSchema = value;
			_stack.Add(value);
			_resolver.LoadedSchemas.Add(value);
		}

		private JsonSchema Pop()
		{
			JsonSchema currentSchema = _currentSchema;
			_stack.RemoveAt(_stack.Count - 1);
			_currentSchema = _stack.LastOrDefault();
			return currentSchema;
		}

		internal JsonSchema Parse(JsonReader reader)
		{
			_reader = reader;
			if (reader.TokenType == JsonToken.None)
			{
				_reader.Read();
			}
			return BuildSchema();
		}

		private JsonSchema BuildSchema()
		{
			if (_reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Expected StartObject while parsing schema object, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
			_reader.Read();
			if (_reader.TokenType == JsonToken.EndObject)
			{
				Push(new JsonSchema());
				return Pop();
			}
			string text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
			_reader.Read();
			if (text == "$ref")
			{
				string text2 = (string)_reader.Value;
				while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
				{
					if (_reader.TokenType == JsonToken.StartObject)
					{
						throw new Exception("Found StartObject within the schema reference with the Id '{0}'".FormatWith(CultureInfo.InvariantCulture, text2));
					}
				}
				JsonSchema schema = _resolver.GetSchema(text2);
				if (schema == null)
				{
					throw new Exception("Could not resolve schema reference for Id '{0}'.".FormatWith(CultureInfo.InvariantCulture, text2));
				}
				return schema;
			}
			Push(new JsonSchema());
			ProcessSchemaProperty(text);
			while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
			{
				text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
				_reader.Read();
				ProcessSchemaProperty(text);
			}
			return Pop();
		}

		private void ProcessSchemaProperty(string propertyName)
		{
			switch (propertyName)
			{
			case "type":
				CurrentSchema.Type = ProcessType();
				break;
			case "id":
				CurrentSchema.Id = (string)_reader.Value;
				break;
			case "title":
				CurrentSchema.Title = (string)_reader.Value;
				break;
			case "description":
				CurrentSchema.Description = (string)_reader.Value;
				break;
			case "properties":
				ProcessProperties();
				break;
			case "items":
				ProcessItems();
				break;
			case "additionalProperties":
				ProcessAdditionalProperties();
				break;
			case "patternProperties":
				ProcessPatternProperties();
				break;
			case "required":
				CurrentSchema.Required = (bool)_reader.Value;
				break;
			case "requires":
				CurrentSchema.Requires = (string)_reader.Value;
				break;
			case "identity":
				ProcessIdentity();
				break;
			case "minimum":
				CurrentSchema.Minimum = Convert.ToDouble(_reader.Value, CultureInfo.InvariantCulture);
				break;
			case "maximum":
				CurrentSchema.Maximum = Convert.ToDouble(_reader.Value, CultureInfo.InvariantCulture);
				break;
			case "exclusiveMinimum":
				CurrentSchema.ExclusiveMinimum = (bool)_reader.Value;
				break;
			case "exclusiveMaximum":
				CurrentSchema.ExclusiveMaximum = (bool)_reader.Value;
				break;
			case "maxLength":
				CurrentSchema.MaximumLength = Convert.ToInt32(_reader.Value, CultureInfo.InvariantCulture);
				break;
			case "minLength":
				CurrentSchema.MinimumLength = Convert.ToInt32(_reader.Value, CultureInfo.InvariantCulture);
				break;
			case "maxItems":
				CurrentSchema.MaximumItems = Convert.ToInt32(_reader.Value, CultureInfo.InvariantCulture);
				break;
			case "minItems":
				CurrentSchema.MinimumItems = Convert.ToInt32(_reader.Value, CultureInfo.InvariantCulture);
				break;
			case "divisibleBy":
				CurrentSchema.DivisibleBy = Convert.ToDouble(_reader.Value, CultureInfo.InvariantCulture);
				break;
			case "disallow":
				CurrentSchema.Disallow = ProcessType();
				break;
			case "default":
				ProcessDefault();
				break;
			case "hidden":
				CurrentSchema.Hidden = (bool)_reader.Value;
				break;
			case "readonly":
				CurrentSchema.ReadOnly = (bool)_reader.Value;
				break;
			case "format":
				CurrentSchema.Format = (string)_reader.Value;
				break;
			case "pattern":
				CurrentSchema.Pattern = (string)_reader.Value;
				break;
			case "options":
				ProcessOptions();
				break;
			case "enum":
				ProcessEnum();
				break;
			case "extends":
				ProcessExtends();
				break;
			default:
				_reader.Skip();
				break;
			}
		}

		private void ProcessExtends()
		{
			CurrentSchema.Extends = BuildSchema();
		}

		private void ProcessEnum()
		{
			if (_reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception("Expected StartArray token while parsing enum values, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
			CurrentSchema.Enum = new List<JToken>();
			while (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
			{
				JToken item = JToken.ReadFrom(_reader);
				CurrentSchema.Enum.Add(item);
			}
		}

		private void ProcessOptions()
		{
			CurrentSchema.Options = new Dictionary<JToken, string>(new JTokenEqualityComparer());
			JsonToken tokenType = _reader.TokenType;
			if (tokenType == JsonToken.StartArray)
			{
				JToken jToken;
				while (true)
				{
					if (!_reader.Read() || _reader.TokenType == JsonToken.EndArray)
					{
						return;
					}
					if (_reader.TokenType != JsonToken.StartObject)
					{
						throw new Exception("Expect object token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
					}
					string value = null;
					jToken = null;
					while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
					{
						string text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
						_reader.Read();
						switch (text)
						{
						case "value":
							jToken = JToken.ReadFrom(_reader);
							break;
						case "label":
							value = (string)_reader.Value;
							break;
						default:
							throw new Exception("Unexpected property in JSON schema option: {0}.".FormatWith(CultureInfo.InvariantCulture, text));
						}
					}
					if (jToken == null)
					{
						throw new Exception("No value specified for JSON schema option.");
					}
					if (CurrentSchema.Options.ContainsKey(jToken))
					{
						break;
					}
					CurrentSchema.Options.Add(jToken, value);
				}
				throw new Exception("Duplicate value in JSON schema option collection: {0}".FormatWith(CultureInfo.InvariantCulture, jToken));
			}
			throw new Exception("Expected array token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
		}

		private void ProcessDefault()
		{
			CurrentSchema.Default = JToken.ReadFrom(_reader);
		}

		private void ProcessIdentity()
		{
			CurrentSchema.Identity = new List<string>();
			switch (_reader.TokenType)
			{
			case JsonToken.String:
				CurrentSchema.Identity.Add(_reader.Value.ToString());
				break;
			case JsonToken.StartArray:
				while (true)
				{
					if (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
					{
						if (_reader.TokenType != JsonToken.String)
						{
							break;
						}
						CurrentSchema.Identity.Add(_reader.Value.ToString());
						continue;
					}
					return;
				}
				throw new Exception("Exception JSON property name string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			default:
				throw new Exception("Expected array or JSON property name string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
		}

		private void ProcessAdditionalProperties()
		{
			if (_reader.TokenType == JsonToken.Boolean)
			{
				CurrentSchema.AllowAdditionalProperties = (bool)_reader.Value;
			}
			else
			{
				CurrentSchema.AdditionalProperties = BuildSchema();
			}
		}

		private void ProcessPatternProperties()
		{
			Dictionary<string, JsonSchema> dictionary = new Dictionary<string, JsonSchema>();
			if (_reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Expected start object token.");
			}
			while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
			{
				string text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
				_reader.Read();
				if (dictionary.ContainsKey(text))
				{
					throw new Exception("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, text));
				}
				dictionary.Add(text, BuildSchema());
			}
			CurrentSchema.PatternProperties = dictionary;
		}

		private void ProcessItems()
		{
			CurrentSchema.Items = new List<JsonSchema>();
			switch (_reader.TokenType)
			{
			case JsonToken.StartObject:
				CurrentSchema.Items.Add(BuildSchema());
				break;
			case JsonToken.StartArray:
				while (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
				{
					CurrentSchema.Items.Add(BuildSchema());
				}
				break;
			default:
				throw new Exception("Expected array or JSON schema object token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
		}

		private void ProcessProperties()
		{
			IDictionary<string, JsonSchema> dictionary = new Dictionary<string, JsonSchema>();
			if (_reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Expected StartObject token while parsing schema properties, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
			while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
			{
				string text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
				_reader.Read();
				if (dictionary.ContainsKey(text))
				{
					throw new Exception("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, text));
				}
				dictionary.Add(text, BuildSchema());
			}
			CurrentSchema.Properties = dictionary;
		}

		private JsonSchemaType? ProcessType()
		{
			switch (_reader.TokenType)
			{
			case JsonToken.String:
				return MapType(_reader.Value.ToString());
			case JsonToken.StartArray:
			{
				JsonSchemaType? result = JsonSchemaType.None;
				while (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
				{
					if (_reader.TokenType != JsonToken.String)
					{
						throw new Exception("Exception JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
					}
					result = ((!result.HasValue) ? null : new JsonSchemaType?(result.GetValueOrDefault() | MapType(_reader.Value.ToString())));
				}
				return result;
			}
			default:
				throw new Exception("Expected array or JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
		}

		internal static JsonSchemaType MapType(string type)
		{
			if (!JsonSchemaConstants.JsonSchemaTypeMapping.TryGetValue(type, out JsonSchemaType value))
			{
				throw new Exception("Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, type));
			}
			return value;
		}

		internal static string MapType(JsonSchemaType type)
		{
			return JsonSchemaConstants.JsonSchemaTypeMapping.Single((KeyValuePair<string, JsonSchemaType> kv) => kv.Value == type).Key;
		}
	}
}
