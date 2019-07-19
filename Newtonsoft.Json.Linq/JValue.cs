using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json.Linq
{
	public class JValue : JToken, IEquatable<JValue>, IFormattable, IComparable, IComparable<JValue>
	{
		private JTokenType _valueType;

		private object _value;

		public override bool HasValues => false;

		public override JTokenType Type => _valueType;

		public new object Value
		{
			get
			{
				return _value;
			}
			set
			{
				Type type = (_value == null) ? null : _value.GetType();
				Type type2 = value?.GetType();
				if (type != type2)
				{
					_valueType = GetValueType(_valueType, value);
				}
				_value = value;
			}
		}

		internal JValue(object value, JTokenType type)
		{
			_value = value;
			_valueType = type;
		}

		public JValue(JValue other)
			: this(other.Value, other.Type)
		{
		}

		public JValue(long value)
			: this(value, JTokenType.Integer)
		{
		}

		public JValue(ulong value)
			: this(value, JTokenType.Integer)
		{
		}

		public JValue(double value)
			: this(value, JTokenType.Float)
		{
		}

		public JValue(DateTime value)
			: this(value, JTokenType.Date)
		{
		}

		public JValue(bool value)
			: this(value, JTokenType.Boolean)
		{
		}

		public JValue(string value)
			: this(value, JTokenType.String)
		{
		}

		public JValue(Guid value)
			: this(value, JTokenType.String)
		{
		}

		public JValue(Uri value)
			: this(value, JTokenType.String)
		{
		}

		public JValue(TimeSpan value)
			: this(value, JTokenType.String)
		{
		}

		public JValue(object value)
			: this(value, GetValueType(null, value))
		{
		}

		internal override bool DeepEquals(JToken node)
		{
			JValue jValue = node as JValue;
			if (jValue == null)
			{
				return false;
			}
			return ValuesEquals(this, jValue);
		}

		private static int Compare(JTokenType valueType, object objA, object objB)
		{
			if (objA == null && objB == null)
			{
				return 0;
			}
			if (objA != null && objB == null)
			{
				return 1;
			}
			if (objA == null && objB != null)
			{
				return -1;
			}
			switch (valueType)
			{
			case JTokenType.Integer:
				if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
				{
					return Convert.ToDecimal(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(objB, CultureInfo.InvariantCulture));
				}
				if (objA is float || objB is float || objA is double || objB is double)
				{
					return CompareFloat(objA, objB);
				}
				return Convert.ToInt64(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToInt64(objB, CultureInfo.InvariantCulture));
			case JTokenType.Float:
				return CompareFloat(objA, objB);
			case JTokenType.Comment:
			case JTokenType.String:
			case JTokenType.Raw:
			{
				string text = Convert.ToString(objA, CultureInfo.InvariantCulture);
				string strB = Convert.ToString(objB, CultureInfo.InvariantCulture);
				return text.CompareTo(strB);
			}
			case JTokenType.Boolean:
			{
				bool flag = Convert.ToBoolean(objA, CultureInfo.InvariantCulture);
				bool value3 = Convert.ToBoolean(objB, CultureInfo.InvariantCulture);
				return flag.CompareTo(value3);
			}
			case JTokenType.Date:
			{
				if (objA is DateTime)
				{
					DateTime dateTime = Convert.ToDateTime(objA, CultureInfo.InvariantCulture);
					DateTime value2 = Convert.ToDateTime(objB, CultureInfo.InvariantCulture);
					return dateTime.CompareTo(value2);
				}
				if (!(objB is DateTimeOffset))
				{
					throw new ArgumentException("Object must be of type DateTimeOffset.");
				}
				DateTimeOffset dateTimeOffset = (DateTimeOffset)objA;
				DateTimeOffset other = (DateTimeOffset)objB;
				return dateTimeOffset.CompareTo(other);
			}
			case JTokenType.Bytes:
			{
				if (!(objB is byte[]))
				{
					throw new ArgumentException("Object must be of type byte[].");
				}
				byte[] array = objA as byte[];
				byte[] array2 = objB as byte[];
				if (array == null)
				{
					return -1;
				}
				if (array2 == null)
				{
					return 1;
				}
				return MiscellaneousUtils.ByteArrayCompare(array, array2);
			}
			case JTokenType.Guid:
			{
				if (!(objB is Guid))
				{
					throw new ArgumentException("Object must be of type Guid.");
				}
				Guid guid = (Guid)objA;
				Guid value4 = (Guid)objB;
				return guid.CompareTo(value4);
			}
			case JTokenType.Uri:
			{
				if (!(objB is Uri))
				{
					throw new ArgumentException("Object must be of type Uri.");
				}
				Uri uri = (Uri)objA;
				Uri uri2 = (Uri)objB;
				return Comparer<string>.Default.Compare(uri.ToString(), uri2.ToString());
			}
			case JTokenType.TimeSpan:
			{
				if (!(objB is TimeSpan))
				{
					throw new ArgumentException("Object must be of type TimeSpan.");
				}
				TimeSpan timeSpan = (TimeSpan)objA;
				TimeSpan value = (TimeSpan)objB;
				return timeSpan.CompareTo(value);
			}
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("valueType", valueType, "Unexpected value type: {0}".FormatWith(CultureInfo.InvariantCulture, valueType));
			}
		}

		private static int CompareFloat(object objA, object objB)
		{
			double d = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
			double num = Convert.ToDouble(objB, CultureInfo.InvariantCulture);
			if (MathUtils.ApproxEquals(d, num))
			{
				return 0;
			}
			return d.CompareTo(num);
		}

		internal override JToken CloneToken()
		{
			return new JValue(this);
		}

		public static JValue CreateComment(string value)
		{
			return new JValue(value, JTokenType.Comment);
		}

		public static JValue CreateString(string value)
		{
			return new JValue(value, JTokenType.String);
		}

		private static JTokenType GetValueType(JTokenType? current, object value)
		{
			if (value == null)
			{
				return JTokenType.Null;
			}
			if (value == DBNull.Value)
			{
				return JTokenType.Null;
			}
			if (value is string)
			{
				return GetStringValueType(current);
			}
			if (value is long || value is int || value is short || value is sbyte || value is ulong || value is uint || value is ushort || value is byte)
			{
				return JTokenType.Integer;
			}
			if (value is Enum)
			{
				return JTokenType.Integer;
			}
			if (value is double || value is float || value is decimal)
			{
				return JTokenType.Float;
			}
			if (value is DateTime)
			{
				return JTokenType.Date;
			}
			if (value is DateTimeOffset)
			{
				return JTokenType.Date;
			}
			if (value is byte[])
			{
				return JTokenType.Bytes;
			}
			if (value is bool)
			{
				return JTokenType.Boolean;
			}
			if (value is Guid)
			{
				return JTokenType.Guid;
			}
			if (value is Uri)
			{
				return JTokenType.Uri;
			}
			if (value is TimeSpan)
			{
				return JTokenType.TimeSpan;
			}
			throw new ArgumentException("Could not determine JSON object type for type {0}.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
		}

		private static JTokenType GetStringValueType(JTokenType? current)
		{
			if (!current.HasValue)
			{
				return JTokenType.String;
			}
			switch (current.Value)
			{
			case JTokenType.Comment:
			case JTokenType.String:
			case JTokenType.Raw:
				return current.Value;
			default:
				return JTokenType.String;
			}
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			switch (_valueType)
			{
			case JTokenType.Comment:
				writer.WriteComment(_value.ToString());
				return;
			case JTokenType.Raw:
				writer.WriteRawValue((_value == null) ? null : _value.ToString());
				return;
			case JTokenType.Null:
				writer.WriteNull();
				return;
			case JTokenType.Undefined:
				writer.WriteUndefined();
				return;
			}
			JsonConverter matchingConverter;
			if (_value != null && (matchingConverter = JsonSerializer.GetMatchingConverter(converters, _value.GetType())) != null)
			{
				matchingConverter.WriteJson(writer, _value, new JsonSerializer());
				return;
			}
			switch (_valueType)
			{
			case JTokenType.Integer:
				writer.WriteValue(Convert.ToInt64(_value, CultureInfo.InvariantCulture));
				break;
			case JTokenType.Float:
				writer.WriteValue(Convert.ToDouble(_value, CultureInfo.InvariantCulture));
				break;
			case JTokenType.String:
				writer.WriteValue((_value == null) ? null : _value.ToString());
				break;
			case JTokenType.Boolean:
				writer.WriteValue(Convert.ToBoolean(_value, CultureInfo.InvariantCulture));
				break;
			case JTokenType.Date:
				if (_value is DateTimeOffset)
				{
					writer.WriteValue((DateTimeOffset)_value);
				}
				else
				{
					writer.WriteValue(Convert.ToDateTime(_value, CultureInfo.InvariantCulture));
				}
				break;
			case JTokenType.Bytes:
				writer.WriteValue((byte[])_value);
				break;
			case JTokenType.Guid:
			case JTokenType.Uri:
			case JTokenType.TimeSpan:
				writer.WriteValue((_value == null) ? null : _value.ToString());
				break;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", _valueType, "Unexpected token type.");
			}
		}

		internal override int GetDeepHashCode()
		{
			int num = (_value != null) ? _value.GetHashCode() : 0;
			return _valueType.GetHashCode() ^ num;
		}

		private static bool ValuesEquals(JValue v1, JValue v2)
		{
			return v1 == v2 || (v1._valueType == v2._valueType && Compare(v1._valueType, v1._value, v2._value) == 0);
		}

		public bool Equals(JValue other)
		{
			if (other == null)
			{
				return false;
			}
			return ValuesEquals(this, other);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			JValue jValue = obj as JValue;
			if (jValue != null)
			{
				return Equals(jValue);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			if (_value == null)
			{
				return 0;
			}
			return _value.GetHashCode();
		}

		public override string ToString()
		{
			if (_value == null)
			{
				return string.Empty;
			}
			return _value.ToString();
		}

		public string ToString(string format)
		{
			return ToString(format, CultureInfo.CurrentCulture);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return ToString(null, formatProvider);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (_value == null)
			{
				return string.Empty;
			}
			IFormattable formattable = _value as IFormattable;
			if (formattable != null)
			{
				return formattable.ToString(format, formatProvider);
			}
			return _value.ToString();
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			object objB = (!(obj is JValue)) ? obj : ((JValue)obj).Value;
			return Compare(_valueType, _value, objB);
		}

		public int CompareTo(JValue obj)
		{
			if (obj == null)
			{
				return 1;
			}
			return Compare(_valueType, _value, obj._value);
		}
	}
}
