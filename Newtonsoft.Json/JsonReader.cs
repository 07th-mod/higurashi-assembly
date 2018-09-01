using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json
{
	public abstract class JsonReader : IDisposable
	{
		protected enum State
		{
			Start,
			Complete,
			Property,
			ObjectStart,
			Object,
			ArrayStart,
			Array,
			Closed,
			PostValue,
			ConstructorStart,
			Constructor,
			Error,
			Finished
		}

		private JsonToken _token;

		private object _value;

		private Type _valueType;

		private char _quoteChar;

		private State _currentState;

		private JTokenType _currentTypeContext;

		private int _top;

		private readonly List<JTokenType> _stack;

		protected State CurrentState => _currentState;

		public bool CloseInput
		{
			get;
			set;
		}

		public virtual char QuoteChar
		{
			get
			{
				return _quoteChar;
			}
			protected internal set
			{
				_quoteChar = value;
			}
		}

		public virtual JsonToken TokenType => _token;

		public virtual object Value => _value;

		public virtual Type ValueType => _valueType;

		public virtual int Depth
		{
			get
			{
				int num = _top - 1;
				if (IsStartToken(TokenType))
				{
					return num - 1;
				}
				return num;
			}
		}

		protected JsonReader()
		{
			_currentState = State.Start;
			_stack = new List<JTokenType>();
			CloseInput = true;
			Push(JTokenType.None);
		}

		private void Push(JTokenType value)
		{
			_stack.Add(value);
			_top++;
			_currentTypeContext = value;
		}

		private JTokenType Pop()
		{
			JTokenType result = Peek();
			_stack.RemoveAt(_stack.Count - 1);
			_top--;
			_currentTypeContext = _stack[_top - 1];
			return result;
		}

		private JTokenType Peek()
		{
			return _currentTypeContext;
		}

		public abstract bool Read();

		public abstract byte[] ReadAsBytes();

		public abstract decimal? ReadAsDecimal();

		public abstract DateTimeOffset? ReadAsDateTimeOffset();

		public void Skip()
		{
			if (IsStartToken(TokenType))
			{
				int depth = Depth;
				while (Read() && depth < Depth)
				{
				}
			}
		}

		protected void SetToken(JsonToken newToken)
		{
			SetToken(newToken, null);
		}

		protected virtual void SetToken(JsonToken newToken, object value)
		{
			_token = newToken;
			switch (newToken)
			{
			case JsonToken.StartObject:
				_currentState = State.ObjectStart;
				Push(JTokenType.Object);
				break;
			case JsonToken.StartArray:
				_currentState = State.ArrayStart;
				Push(JTokenType.Array);
				break;
			case JsonToken.StartConstructor:
				_currentState = State.ConstructorStart;
				Push(JTokenType.Constructor);
				break;
			case JsonToken.EndObject:
				ValidateEnd(JsonToken.EndObject);
				_currentState = State.PostValue;
				break;
			case JsonToken.EndArray:
				ValidateEnd(JsonToken.EndArray);
				_currentState = State.PostValue;
				break;
			case JsonToken.EndConstructor:
				ValidateEnd(JsonToken.EndConstructor);
				_currentState = State.PostValue;
				break;
			case JsonToken.PropertyName:
				_currentState = State.Property;
				Push(JTokenType.Property);
				break;
			case JsonToken.Raw:
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				_currentState = State.PostValue;
				break;
			}
			JTokenType jTokenType = Peek();
			if (jTokenType == JTokenType.Property && _currentState == State.PostValue)
			{
				Pop();
			}
			if (value != null)
			{
				_value = value;
				_valueType = value.GetType();
			}
			else
			{
				_value = null;
				_valueType = null;
			}
		}

		private void ValidateEnd(JsonToken endToken)
		{
			JTokenType jTokenType = Pop();
			if (GetTypeForCloseToken(endToken) != jTokenType)
			{
				throw new JsonReaderException("JsonToken {0} is not valid for closing JsonType {1}.".FormatWith(CultureInfo.InvariantCulture, endToken, jTokenType));
			}
		}

		protected void SetStateBasedOnCurrent()
		{
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
				_currentState = State.Constructor;
				break;
			case JTokenType.None:
				_currentState = State.Finished;
				break;
			default:
				throw new JsonReaderException("While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith(CultureInfo.InvariantCulture, jTokenType));
			}
		}

		internal static bool IsPrimitiveToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsStartToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.StartObject:
			case JsonToken.StartArray:
			case JsonToken.StartConstructor:
			case JsonToken.PropertyName:
				return true;
			case JsonToken.None:
			case JsonToken.Comment:
			case JsonToken.Raw:
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.EndObject:
			case JsonToken.EndArray:
			case JsonToken.EndConstructor:
			case JsonToken.Date:
			case JsonToken.Bytes:
				return false;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("token", token, "Unexpected JsonToken value.");
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
				throw new JsonReaderException("Not a valid close JsonToken: {0}".FormatWith(CultureInfo.InvariantCulture, token));
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(disposing: true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_currentState != State.Closed && disposing)
			{
				Close();
			}
		}

		public virtual void Close()
		{
			_currentState = State.Closed;
			_token = JsonToken.None;
			_value = null;
			_valueType = null;
		}
	}
}
