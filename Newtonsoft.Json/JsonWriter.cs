using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json
{
	public abstract class JsonWriter : IDisposable
	{
		private enum State
		{
			Start,
			Property,
			ObjectStart,
			Object,
			ArrayStart,
			Array,
			ConstructorStart,
			Constructor,
			Bytes,
			Closed,
			Error
		}

		private static readonly State[][] stateArray = new State[8][]
		{
			new State[10]
			{
				State.Error,
				State.Error,
				State.Error,
				State.Error,
				State.Error,
				State.Error,
				State.Error,
				State.Error,
				State.Error,
				State.Error
			},
			new State[10]
			{
				State.ObjectStart,
				State.ObjectStart,
				State.Error,
				State.Error,
				State.ObjectStart,
				State.ObjectStart,
				State.ObjectStart,
				State.ObjectStart,
				State.Error,
				State.Error
			},
			new State[10]
			{
				State.ArrayStart,
				State.ArrayStart,
				State.Error,
				State.Error,
				State.ArrayStart,
				State.ArrayStart,
				State.ArrayStart,
				State.ArrayStart,
				State.Error,
				State.Error
			},
			new State[10]
			{
				State.ConstructorStart,
				State.ConstructorStart,
				State.Error,
				State.Error,
				State.ConstructorStart,
				State.ConstructorStart,
				State.ConstructorStart,
				State.ConstructorStart,
				State.Error,
				State.Error
			},
			new State[10]
			{
				State.Property,
				State.Error,
				State.Property,
				State.Property,
				State.Error,
				State.Error,
				State.Error,
				State.Error,
				State.Error,
				State.Error
			},
			new State[10]
			{
				State.Start,
				State.Property,
				State.ObjectStart,
				State.Object,
				State.ArrayStart,
				State.Array,
				State.Constructor,
				State.Constructor,
				State.Error,
				State.Error
			},
			new State[10]
			{
				State.Start,
				State.Property,
				State.ObjectStart,
				State.Object,
				State.ArrayStart,
				State.Array,
				State.Constructor,
				State.Constructor,
				State.Error,
				State.Error
			},
			new State[10]
			{
				State.Start,
				State.Object,
				State.Error,
				State.Error,
				State.Array,
				State.Array,
				State.Constructor,
				State.Constructor,
				State.Error,
				State.Error
			}
		};

		private int _top;

		private readonly List<JTokenType> _stack;

		private State _currentState;

		private Formatting _formatting;

		public bool CloseOutput
		{
			get;
			set;
		}

		protected internal int Top => _top;

		public WriteState WriteState
		{
			get
			{
				switch (_currentState)
				{
				case State.Error:
					return WriteState.Error;
				case State.Closed:
					return WriteState.Closed;
				case State.ObjectStart:
				case State.Object:
					return WriteState.Object;
				case State.ArrayStart:
				case State.Array:
					return WriteState.Array;
				case State.ConstructorStart:
				case State.Constructor:
					return WriteState.Constructor;
				case State.Property:
					return WriteState.Property;
				case State.Start:
					return WriteState.Start;
				default:
					throw new JsonWriterException("Invalid state: " + _currentState);
				}
			}
		}

		public Formatting Formatting
		{
			get
			{
				return _formatting;
			}
			set
			{
				_formatting = value;
			}
		}

		protected JsonWriter()
		{
			_stack = new List<JTokenType>(8);
			_stack.Add(JTokenType.None);
			_currentState = State.Start;
			_formatting = Formatting.None;
			CloseOutput = true;
		}

		private void Push(JTokenType value)
		{
			_top++;
			if (_stack.Count <= _top)
			{
				_stack.Add(value);
			}
			else
			{
				_stack[_top] = value;
			}
		}

		private JTokenType Pop()
		{
			JTokenType result = Peek();
			_top--;
			return result;
		}

		private JTokenType Peek()
		{
			return _stack[_top];
		}

		public abstract void Flush();

		public virtual void Close()
		{
			AutoCompleteAll();
		}

		public virtual void WriteStartObject()
		{
			AutoComplete(JsonToken.StartObject);
			Push(JTokenType.Object);
		}

		public virtual void WriteEndObject()
		{
			AutoCompleteClose(JsonToken.EndObject);
		}

		public virtual void WriteStartArray()
		{
			AutoComplete(JsonToken.StartArray);
			Push(JTokenType.Array);
		}

		public virtual void WriteEndArray()
		{
			AutoCompleteClose(JsonToken.EndArray);
		}

		public virtual void WriteStartConstructor(string name)
		{
			AutoComplete(JsonToken.StartConstructor);
			Push(JTokenType.Constructor);
		}

		public virtual void WriteEndConstructor()
		{
			AutoCompleteClose(JsonToken.EndConstructor);
		}

		public virtual void WritePropertyName(string name)
		{
			AutoComplete(JsonToken.PropertyName);
		}

		public virtual void WriteEnd()
		{
			WriteEnd(Peek());
		}

		public void WriteToken(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			int initialDepth = (reader.TokenType == JsonToken.None) ? (-1) : (IsStartToken(reader.TokenType) ? reader.Depth : (reader.Depth + 1));
			WriteToken(reader, initialDepth);
		}

		internal void WriteToken(JsonReader reader, int initialDepth)
		{
			do
			{
				switch (reader.TokenType)
				{
				case JsonToken.StartObject:
					WriteStartObject();
					break;
				case JsonToken.StartArray:
					WriteStartArray();
					break;
				case JsonToken.StartConstructor:
				{
					string strA = reader.Value.ToString();
					if (string.Compare(strA, "Date", StringComparison.Ordinal) == 0)
					{
						WriteConstructorDate(reader);
					}
					else
					{
						WriteStartConstructor(reader.Value.ToString());
					}
					break;
				}
				case JsonToken.PropertyName:
					WritePropertyName(reader.Value.ToString());
					break;
				case JsonToken.Comment:
					WriteComment(reader.Value.ToString());
					break;
				case JsonToken.Integer:
					WriteValue(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
					break;
				case JsonToken.Float:
					WriteValue(Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
					break;
				case JsonToken.String:
					WriteValue(reader.Value.ToString());
					break;
				case JsonToken.Boolean:
					WriteValue(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
					break;
				case JsonToken.Null:
					WriteNull();
					break;
				case JsonToken.Undefined:
					WriteUndefined();
					break;
				case JsonToken.EndObject:
					WriteEndObject();
					break;
				case JsonToken.EndArray:
					WriteEndArray();
					break;
				case JsonToken.EndConstructor:
					WriteEndConstructor();
					break;
				case JsonToken.Date:
					WriteValue((DateTime)reader.Value);
					break;
				case JsonToken.Raw:
					WriteRawValue((string)reader.Value);
					break;
				case JsonToken.Bytes:
					WriteValue((byte[])reader.Value);
					break;
				default:
					throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", reader.TokenType, "Unexpected token type.");
				case JsonToken.None:
					break;
				}
			}
			while (initialDepth - 1 < reader.Depth - (IsEndToken(reader.TokenType) ? 1 : 0) && reader.Read());
		}

		private void WriteConstructorDate(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw new Exception("Unexpected end while reading date constructor.");
			}
			if (reader.TokenType != JsonToken.Integer)
			{
				throw new Exception("Unexpected token while reading date constructor. Expected Integer, got " + reader.TokenType);
			}
			long javaScriptTicks = (long)reader.Value;
			DateTime value = JsonConvert.ConvertJavaScriptTicksToDateTime(javaScriptTicks);
			if (!reader.Read())
			{
				throw new Exception("Unexpected end while reading date constructor.");
			}
			if (reader.TokenType != JsonToken.EndConstructor)
			{
				throw new Exception("Unexpected token while reading date constructor. Expected EndConstructor, got " + reader.TokenType);
			}
			WriteValue(value);
		}

		private bool IsEndToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.EndObject:
			case JsonToken.EndArray:
			case JsonToken.EndConstructor:
				return true;
			default:
				return false;
			}
		}

		private bool IsStartToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.StartObject:
			case JsonToken.StartArray:
			case JsonToken.StartConstructor:
				return true;
			default:
				return false;
			}
		}

		private void WriteEnd(JTokenType type)
		{
			switch (type)
			{
			case JTokenType.Object:
				WriteEndObject();
				break;
			case JTokenType.Array:
				WriteEndArray();
				break;
			case JTokenType.Constructor:
				WriteEndConstructor();
				break;
			default:
				throw new JsonWriterException("Unexpected type when writing end: " + type);
			}
		}

		private void AutoCompleteAll()
		{
			while (_top > 0)
			{
				WriteEnd();
			}
		}

		private JTokenType GetTypeForCloseToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.EndObject:
				return JTokenType.Object;
			case JsonToken.EndArray:
				return JTokenType.Array;
			case JsonToken.EndConstructor:
				return JTokenType.Constructor;
			default:
				throw new JsonWriterException("No type for token: " + token);
			}
		}

		private JsonToken GetCloseTokenForType(JTokenType type)
		{
			switch (type)
			{
			case JTokenType.Object:
				return JsonToken.EndObject;
			case JTokenType.Array:
				return JsonToken.EndArray;
			case JTokenType.Constructor:
				return JsonToken.EndConstructor;
			default:
				throw new JsonWriterException("No close token for type: " + type);
			}
		}

		private void AutoCompleteClose(JsonToken tokenBeingClosed)
		{
			int num = 0;
			for (int i = 0; i < _top; i++)
			{
				int index = _top - i;
				if (_stack[index] == GetTypeForCloseToken(tokenBeingClosed))
				{
					num = i + 1;
					break;
				}
			}
			if (num == 0)
			{
				throw new JsonWriterException("No token to close.");
			}
			for (int j = 0; j < num; j++)
			{
				JsonToken closeTokenForType = GetCloseTokenForType(Pop());
				if (_currentState != State.ObjectStart && _currentState != State.ArrayStart)
				{
					WriteIndent();
				}
				WriteEnd(closeTokenForType);
			}
			JTokenType jTokenType = Peek();
			switch (jTokenType)
			{
			case JTokenType.Object:
				_currentState = State.Object;
				break;
			case JTokenType.Array:
				_currentState = State.Array;
				break;
			case JTokenType.Constructor:
				_currentState = State.Array;
				break;
			case JTokenType.None:
				_currentState = State.Start;
				break;
			default:
				throw new JsonWriterException("Unknown JsonType: " + jTokenType);
			}
		}

		protected virtual void WriteEnd(JsonToken token)
		{
		}

		protected virtual void WriteIndent()
		{
		}

		protected virtual void WriteValueDelimiter()
		{
		}

		protected virtual void WriteIndentSpace()
		{
		}

		internal void AutoComplete(JsonToken tokenBeingWritten)
		{
			int num;
			switch (tokenBeingWritten)
			{
			default:
				num = (int)tokenBeingWritten;
				break;
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				num = 7;
				break;
			}
			State state = stateArray[num][(int)_currentState];
			if (state == State.Error)
			{
				throw new JsonWriterException("Token {0} in state {1} would result in an invalid JavaScript object.".FormatWith(CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), _currentState.ToString()));
			}
			if ((_currentState == State.Object || _currentState == State.Array || _currentState == State.Constructor) && tokenBeingWritten != JsonToken.Comment)
			{
				WriteValueDelimiter();
			}
			else if (_currentState == State.Property && _formatting == Formatting.Indented)
			{
				WriteIndentSpace();
			}
			WriteState writeState = WriteState;
			if ((tokenBeingWritten == JsonToken.PropertyName && writeState != WriteState.Start) || writeState == WriteState.Array || writeState == WriteState.Constructor)
			{
				WriteIndent();
			}
			_currentState = state;
		}

		public virtual void WriteNull()
		{
			AutoComplete(JsonToken.Null);
		}

		public virtual void WriteUndefined()
		{
			AutoComplete(JsonToken.Undefined);
		}

		public virtual void WriteRaw(string json)
		{
		}

		public virtual void WriteRawValue(string json)
		{
			AutoComplete(JsonToken.Undefined);
			WriteRaw(json);
		}

		public virtual void WriteValue(string value)
		{
			AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(int value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(uint value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(long value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(ulong value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(float value)
		{
			AutoComplete(JsonToken.Float);
		}

		public virtual void WriteValue(double value)
		{
			AutoComplete(JsonToken.Float);
		}

		public virtual void WriteValue(bool value)
		{
			AutoComplete(JsonToken.Boolean);
		}

		public virtual void WriteValue(short value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(ushort value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(char value)
		{
			AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(byte value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(sbyte value)
		{
			AutoComplete(JsonToken.Integer);
		}

		public virtual void WriteValue(decimal value)
		{
			AutoComplete(JsonToken.Float);
		}

		public virtual void WriteValue(DateTime value)
		{
			AutoComplete(JsonToken.Date);
		}

		public virtual void WriteValue(DateTimeOffset value)
		{
			AutoComplete(JsonToken.Date);
		}

		public virtual void WriteValue(Guid value)
		{
			AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(TimeSpan value)
		{
			AutoComplete(JsonToken.String);
		}

		public virtual void WriteValue(int? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(uint? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(long? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(ulong? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(float? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(double? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(bool? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(short? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(ushort? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(char? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(byte? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(sbyte? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(decimal? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(DateTime? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(DateTimeOffset? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(Guid? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(TimeSpan? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(byte[] value)
		{
			if (value == null)
			{
				WriteNull();
			}
			else
			{
				AutoComplete(JsonToken.Bytes);
			}
		}

		public virtual void WriteValue(Uri value)
		{
			if (value == null)
			{
				WriteNull();
			}
			else
			{
				AutoComplete(JsonToken.String);
			}
		}

		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				WriteNull();
				return;
			}
			if (value is IConvertible)
			{
				IConvertible convertible = value as IConvertible;
				switch (convertible.GetTypeCode())
				{
				case TypeCode.String:
					WriteValue(convertible.ToString(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Char:
					WriteValue(convertible.ToChar(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Boolean:
					WriteValue(convertible.ToBoolean(CultureInfo.InvariantCulture));
					return;
				case TypeCode.SByte:
					WriteValue(convertible.ToSByte(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Int16:
					WriteValue(convertible.ToInt16(CultureInfo.InvariantCulture));
					return;
				case TypeCode.UInt16:
					WriteValue(convertible.ToUInt16(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Int32:
					WriteValue(convertible.ToInt32(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Byte:
					WriteValue(convertible.ToByte(CultureInfo.InvariantCulture));
					return;
				case TypeCode.UInt32:
					WriteValue(convertible.ToUInt32(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Int64:
					WriteValue(convertible.ToInt64(CultureInfo.InvariantCulture));
					return;
				case TypeCode.UInt64:
					WriteValue(convertible.ToUInt64(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Single:
					WriteValue(convertible.ToSingle(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Double:
					WriteValue(convertible.ToDouble(CultureInfo.InvariantCulture));
					return;
				case TypeCode.DateTime:
					WriteValue(convertible.ToDateTime(CultureInfo.InvariantCulture));
					return;
				case TypeCode.Decimal:
					WriteValue(convertible.ToDecimal(CultureInfo.InvariantCulture));
					return;
				case TypeCode.DBNull:
					WriteNull();
					return;
				}
			}
			else
			{
				if (value is DateTimeOffset)
				{
					WriteValue((DateTimeOffset)value);
					return;
				}
				if (value is byte[])
				{
					WriteValue((byte[])value);
					return;
				}
				if (value is Guid)
				{
					WriteValue((Guid)value);
					return;
				}
				if (value is Uri)
				{
					WriteValue((Uri)value);
					return;
				}
				if (value is TimeSpan)
				{
					WriteValue((TimeSpan)value);
					return;
				}
			}
			throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
		}

		public virtual void WriteComment(string text)
		{
			AutoComplete(JsonToken.Comment);
		}

		public virtual void WriteWhitespace(string ws)
		{
			if (ws != null && !StringUtils.IsWhiteSpace(ws))
			{
				throw new JsonWriterException("Only white space characters should be used.");
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
			if (WriteState != WriteState.Closed)
			{
				Close();
			}
		}
	}
}
