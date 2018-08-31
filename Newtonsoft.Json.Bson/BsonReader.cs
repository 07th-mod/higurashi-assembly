using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Newtonsoft.Json.Bson
{
	public class BsonReader : JsonReader
	{
		private enum BsonReaderState
		{
			Normal,
			ReferenceStart,
			ReferenceRef,
			ReferenceId,
			CodeWScopeStart,
			CodeWScopeCode,
			CodeWScopeScope,
			CodeWScopeScopeObject,
			CodeWScopeScopeEnd
		}

		private class ContainerContext
		{
			public readonly BsonType Type;

			public int Length;

			public int Position;

			public ContainerContext(BsonType type)
			{
				Type = type;
			}
		}

		private const int MaxCharBytesSize = 128;

		private static readonly byte[] _seqRange1 = new byte[2]
		{
			0,
			127
		};

		private static readonly byte[] _seqRange2 = new byte[2]
		{
			194,
			223
		};

		private static readonly byte[] _seqRange3 = new byte[2]
		{
			224,
			239
		};

		private static readonly byte[] _seqRange4 = new byte[2]
		{
			240,
			244
		};

		private readonly BinaryReader _reader;

		private readonly List<ContainerContext> _stack;

		private byte[] _byteBuffer;

		private char[] _charBuffer;

		private BsonType _currentElementType;

		private BsonReaderState _bsonReaderState;

		private ContainerContext _currentContext;

		private bool _readRootValueAsArray;

		private bool _jsonNet35BinaryCompatibility;

		private DateTimeKind _dateTimeKindHandling;

		public bool JsonNet35BinaryCompatibility
		{
			get
			{
				return _jsonNet35BinaryCompatibility;
			}
			set
			{
				_jsonNet35BinaryCompatibility = value;
			}
		}

		public bool ReadRootValueAsArray
		{
			get
			{
				return _readRootValueAsArray;
			}
			set
			{
				_readRootValueAsArray = value;
			}
		}

		public DateTimeKind DateTimeKindHandling
		{
			get
			{
				return _dateTimeKindHandling;
			}
			set
			{
				_dateTimeKindHandling = value;
			}
		}

		public BsonReader(Stream stream)
			: this(stream, /*readRootValueAsArray:*/ false, DateTimeKind.Local)
		{
		}

		public BsonReader(Stream stream, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
		{
			ValidationUtils.ArgumentNotNull(stream, "stream");
			_reader = new BinaryReader(stream);
			_stack = new List<ContainerContext>();
			_readRootValueAsArray = readRootValueAsArray;
			_dateTimeKindHandling = dateTimeKindHandling;
		}

		private string ReadElement()
		{
			_currentElementType = ReadType();
			return ReadString();
		}

		public override byte[] ReadAsBytes()
		{
			Read();
			if (TokenType == JsonToken.Null)
			{
				return null;
			}
			if (TokenType == JsonToken.Bytes)
			{
				return (byte[])Value;
			}
			throw new JsonReaderException("Error reading bytes. Expected bytes but got {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}

		public override decimal? ReadAsDecimal()
		{
			Read();
			if (TokenType == JsonToken.Null)
			{
				return null;
			}
			if (TokenType == JsonToken.Integer || TokenType == JsonToken.Float)
			{
				SetToken(JsonToken.Float, Convert.ToDecimal(Value, CultureInfo.InvariantCulture));
				return (decimal)Value;
			}
			throw new JsonReaderException("Error reading decimal. Expected a number but got {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			Read();
			if (TokenType == JsonToken.Null)
			{
				return null;
			}
			if (TokenType == JsonToken.Date)
			{
				SetToken(JsonToken.Date, new DateTimeOffset((DateTime)Value));
				return (DateTimeOffset)Value;
			}
			throw new JsonReaderException("Error reading date. Expected bytes but got {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
		}

		public override bool Read()
		{
			try
			{
				switch (_bsonReaderState)
				{
				case BsonReaderState.Normal:
					return ReadNormal();
				case BsonReaderState.ReferenceStart:
				case BsonReaderState.ReferenceRef:
				case BsonReaderState.ReferenceId:
					return ReadReference();
				case BsonReaderState.CodeWScopeStart:
				case BsonReaderState.CodeWScopeCode:
				case BsonReaderState.CodeWScopeScope:
				case BsonReaderState.CodeWScopeScopeObject:
				case BsonReaderState.CodeWScopeScopeEnd:
					return ReadCodeWScope();
				default:
					throw new JsonReaderException("Unexpected state: {0}".FormatWith(CultureInfo.InvariantCulture, _bsonReaderState));
				}
				IL_0083:
				bool result;
				return result;
			}
			catch (EndOfStreamException)
			{
				return false;
				IL_0090:
				bool result;
				return result;
			}
		}

		public override void Close()
		{
			base.Close();
			if (base.CloseInput && _reader != null)
			{
				_reader.Close();
			}
		}

		private bool ReadCodeWScope()
		{
			switch (_bsonReaderState)
			{
			case BsonReaderState.CodeWScopeStart:
				SetToken(JsonToken.PropertyName, "$code");
				_bsonReaderState = BsonReaderState.CodeWScopeCode;
				return true;
			case BsonReaderState.CodeWScopeCode:
				ReadInt32();
				SetToken(JsonToken.String, ReadLengthString());
				_bsonReaderState = BsonReaderState.CodeWScopeScope;
				return true;
			case BsonReaderState.CodeWScopeScope:
			{
				if (base.CurrentState == State.PostValue)
				{
					SetToken(JsonToken.PropertyName, "$scope");
					return true;
				}
				SetToken(JsonToken.StartObject);
				_bsonReaderState = BsonReaderState.CodeWScopeScopeObject;
				ContainerContext containerContext = new ContainerContext(BsonType.Object);
				PushContext(containerContext);
				containerContext.Length = ReadInt32();
				return true;
			}
			case BsonReaderState.CodeWScopeScopeObject:
			{
				bool flag = ReadNormal();
				if (flag && TokenType == JsonToken.EndObject)
				{
					_bsonReaderState = BsonReaderState.CodeWScopeScopeEnd;
				}
				return flag;
			}
			case BsonReaderState.CodeWScopeScopeEnd:
				SetToken(JsonToken.EndObject);
				_bsonReaderState = BsonReaderState.Normal;
				return true;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private bool ReadReference()
		{
			switch (base.CurrentState)
			{
			case State.ObjectStart:
				SetToken(JsonToken.PropertyName, "$ref");
				_bsonReaderState = BsonReaderState.ReferenceRef;
				return true;
			case State.Property:
				if (_bsonReaderState == BsonReaderState.ReferenceRef)
				{
					SetToken(JsonToken.String, ReadLengthString());
					return true;
				}
				if (_bsonReaderState == BsonReaderState.ReferenceId)
				{
					SetToken(JsonToken.Bytes, ReadBytes(12));
					return true;
				}
				throw new JsonReaderException("Unexpected state when reading BSON reference: " + _bsonReaderState);
			case State.PostValue:
				if (_bsonReaderState == BsonReaderState.ReferenceRef)
				{
					SetToken(JsonToken.PropertyName, "$id");
					_bsonReaderState = BsonReaderState.ReferenceId;
					return true;
				}
				if (_bsonReaderState == BsonReaderState.ReferenceId)
				{
					SetToken(JsonToken.EndObject);
					_bsonReaderState = BsonReaderState.Normal;
					return true;
				}
				throw new JsonReaderException("Unexpected state when reading BSON reference: " + _bsonReaderState);
			default:
				throw new JsonReaderException("Unexpected state when reading BSON reference: " + base.CurrentState);
			}
		}

		private bool ReadNormal()
		{
			switch (base.CurrentState)
			{
			case State.Start:
			{
				JsonToken token2 = (!_readRootValueAsArray) ? JsonToken.StartObject : JsonToken.StartArray;
				BsonType type = _readRootValueAsArray ? BsonType.Array : BsonType.Object;
				SetToken(token2);
				ContainerContext containerContext = new ContainerContext(type);
				PushContext(containerContext);
				containerContext.Length = ReadInt32();
				return true;
			}
			case State.Complete:
			case State.Closed:
				return false;
			case State.Property:
				ReadType(_currentElementType);
				return true;
			case State.ObjectStart:
			case State.ArrayStart:
			case State.PostValue:
			{
				ContainerContext currentContext = _currentContext;
				if (currentContext == null)
				{
					return false;
				}
				int num = currentContext.Length - 1;
				if (currentContext.Position < num)
				{
					if (currentContext.Type == BsonType.Array)
					{
						ReadElement();
						ReadType(_currentElementType);
						return true;
					}
					SetToken(JsonToken.PropertyName, ReadElement());
					return true;
				}
				if (currentContext.Position == num)
				{
					if (ReadByte() != 0)
					{
						throw new JsonReaderException("Unexpected end of object byte value.");
					}
					PopContext();
					if (_currentContext != null)
					{
						MovePosition(currentContext.Length);
					}
					JsonToken token = (currentContext.Type != BsonType.Object) ? JsonToken.EndArray : JsonToken.EndObject;
					SetToken(token);
					return true;
				}
				throw new JsonReaderException("Read past end of current container context.");
			}
			default:
				throw new ArgumentOutOfRangeException();
			case State.ConstructorStart:
			case State.Constructor:
			case State.Error:
			case State.Finished:
				return false;
			}
		}

		private void PopContext()
		{
			_stack.RemoveAt(_stack.Count - 1);
			if (_stack.Count == 0)
			{
				_currentContext = null;
			}
			else
			{
				_currentContext = _stack[_stack.Count - 1];
			}
		}

		private void PushContext(ContainerContext newContext)
		{
			_stack.Add(newContext);
			_currentContext = newContext;
		}

		private byte ReadByte()
		{
			MovePosition(1);
			return _reader.ReadByte();
		}

		private void ReadType(BsonType type)
		{
			switch (type)
			{
			case BsonType.Number:
				SetToken(JsonToken.Float, ReadDouble());
				break;
			case BsonType.String:
			case BsonType.Symbol:
				SetToken(JsonToken.String, ReadLengthString());
				break;
			case BsonType.Object:
			{
				SetToken(JsonToken.StartObject);
				ContainerContext containerContext2 = new ContainerContext(BsonType.Object);
				PushContext(containerContext2);
				containerContext2.Length = ReadInt32();
				break;
			}
			case BsonType.Array:
			{
				SetToken(JsonToken.StartArray);
				ContainerContext containerContext = new ContainerContext(BsonType.Array);
				PushContext(containerContext);
				containerContext.Length = ReadInt32();
				break;
			}
			case BsonType.Binary:
				SetToken(JsonToken.Bytes, ReadBinary());
				break;
			case BsonType.Undefined:
				SetToken(JsonToken.Undefined);
				break;
			case BsonType.Oid:
			{
				byte[] value2 = ReadBytes(12);
				SetToken(JsonToken.Bytes, value2);
				break;
			}
			case BsonType.Boolean:
			{
				bool flag = Convert.ToBoolean(ReadByte());
				SetToken(JsonToken.Boolean, flag);
				break;
			}
			case BsonType.Date:
			{
				long javaScriptTicks = ReadInt64();
				DateTime dateTime = JsonConvert.ConvertJavaScriptTicksToDateTime(javaScriptTicks);
				DateTime dateTime2;
				switch (DateTimeKindHandling)
				{
				case DateTimeKind.Unspecified:
					dateTime2 = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
					break;
				case DateTimeKind.Local:
					dateTime2 = dateTime.ToLocalTime();
					break;
				default:
					dateTime2 = dateTime;
					break;
				}
				SetToken(JsonToken.Date, dateTime2);
				break;
			}
			case BsonType.Null:
				SetToken(JsonToken.Null);
				break;
			case BsonType.Regex:
			{
				string str = ReadString();
				string str2 = ReadString();
				string value = "/" + str + "/" + str2;
				SetToken(JsonToken.String, value);
				break;
			}
			case BsonType.Reference:
				SetToken(JsonToken.StartObject);
				_bsonReaderState = BsonReaderState.ReferenceStart;
				break;
			case BsonType.Code:
				SetToken(JsonToken.String, ReadLengthString());
				break;
			case BsonType.CodeWScope:
				SetToken(JsonToken.StartObject);
				_bsonReaderState = BsonReaderState.CodeWScopeStart;
				break;
			case BsonType.Integer:
				SetToken(JsonToken.Integer, (long)ReadInt32());
				break;
			case BsonType.TimeStamp:
			case BsonType.Long:
				SetToken(JsonToken.Integer, ReadInt64());
				break;
			default:
				throw new ArgumentOutOfRangeException("type", "Unexpected BsonType value: " + type);
			}
		}

		private byte[] ReadBinary()
		{
			int count = ReadInt32();
			BsonBinaryType bsonBinaryType = (BsonBinaryType)ReadByte();
			if (bsonBinaryType == BsonBinaryType.Data && !_jsonNet35BinaryCompatibility)
			{
				count = ReadInt32();
			}
			return ReadBytes(count);
		}

		private string ReadString()
		{
			EnsureBuffers();
			StringBuilder stringBuilder = null;
			int num = 0;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				byte b;
				while (num3 < 128 && (b = _reader.ReadByte()) > 0)
				{
					_byteBuffer[num3++] = b;
				}
				int num5 = num3 - num2;
				num += num5;
				if (num3 < 128 && stringBuilder == null)
				{
					int chars = Encoding.UTF8.GetChars(_byteBuffer, 0, num5, _charBuffer, 0);
					MovePosition(num + 1);
					return new string(_charBuffer, 0, chars);
				}
				int lastFullCharStop = GetLastFullCharStop(num3 - 1);
				int chars2 = Encoding.UTF8.GetChars(_byteBuffer, 0, lastFullCharStop + 1, _charBuffer, 0);
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(256);
				}
				stringBuilder.Append(_charBuffer, 0, chars2);
				if (lastFullCharStop < num5 - 1)
				{
					num2 = num5 - lastFullCharStop - 1;
					Array.Copy(_byteBuffer, lastFullCharStop + 1, _byteBuffer, 0, num2);
				}
				else
				{
					if (num3 < 128)
					{
						break;
					}
					num2 = 0;
				}
			}
			MovePosition(num + 1);
			return stringBuilder.ToString();
		}

		private string ReadLengthString()
		{
			int num = ReadInt32();
			MovePosition(num);
			string @string = GetString(num - 1);
			_reader.ReadByte();
			return @string;
		}

		private string GetString(int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			EnsureBuffers();
			StringBuilder stringBuilder = null;
			int num = 0;
			int num2 = 0;
			do
			{
				int count = (length - num <= 128 - num2) ? (length - num) : (128 - num2);
				int num3 = _reader.BaseStream.Read(_byteBuffer, num2, count);
				if (num3 == 0)
				{
					throw new EndOfStreamException("Unable to read beyond the end of the stream.");
				}
				num += num3;
				num3 += num2;
				if (num3 == length)
				{
					int chars = Encoding.UTF8.GetChars(_byteBuffer, 0, num3, _charBuffer, 0);
					return new string(_charBuffer, 0, chars);
				}
				int lastFullCharStop = GetLastFullCharStop(num3 - 1);
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(length);
				}
				int chars2 = Encoding.UTF8.GetChars(_byteBuffer, 0, lastFullCharStop + 1, _charBuffer, 0);
				stringBuilder.Append(_charBuffer, 0, chars2);
				if (lastFullCharStop < num3 - 1)
				{
					num2 = num3 - lastFullCharStop - 1;
					Array.Copy(_byteBuffer, lastFullCharStop + 1, _byteBuffer, 0, num2);
				}
				else
				{
					num2 = 0;
				}
			}
			while (num < length);
			return stringBuilder.ToString();
		}

		private int GetLastFullCharStop(int start)
		{
			int num = start;
			int num2 = 0;
			while (num >= 0)
			{
				num2 = BytesInSequence(_byteBuffer[num]);
				switch (num2)
				{
				case 0:
					break;
				default:
					num--;
					goto end_IL_003c;
				case 1:
					goto end_IL_003c;
				}
				num--;
				continue;
				continue;
				end_IL_003c:
				break;
			}
			if (num2 == start - num)
			{
				return start;
			}
			return num;
		}

		private int BytesInSequence(byte b)
		{
			if (b <= _seqRange1[1])
			{
				return 1;
			}
			if (b >= _seqRange2[0] && b <= _seqRange2[1])
			{
				return 2;
			}
			if (b >= _seqRange3[0] && b <= _seqRange3[1])
			{
				return 3;
			}
			if (b >= _seqRange4[0] && b <= _seqRange4[1])
			{
				return 4;
			}
			return 0;
		}

		private void EnsureBuffers()
		{
			if (_byteBuffer == null)
			{
				_byteBuffer = new byte[128];
			}
			if (_charBuffer == null)
			{
				int maxCharCount = Encoding.UTF8.GetMaxCharCount(128);
				_charBuffer = new char[maxCharCount];
			}
		}

		private double ReadDouble()
		{
			MovePosition(8);
			return _reader.ReadDouble();
		}

		private int ReadInt32()
		{
			MovePosition(4);
			return _reader.ReadInt32();
		}

		private long ReadInt64()
		{
			MovePosition(8);
			return _reader.ReadInt64();
		}

		private BsonType ReadType()
		{
			MovePosition(1);
			return (BsonType)_reader.ReadSByte();
		}

		private void MovePosition(int count)
		{
			_currentContext.Position += count;
		}

		private byte[] ReadBytes(int count)
		{
			MovePosition(count);
			return _reader.ReadBytes(count);
		}
	}
}
