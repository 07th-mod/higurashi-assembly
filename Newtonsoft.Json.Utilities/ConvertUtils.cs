using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	internal static class ConvertUtils
	{
		internal struct TypeConvertKey : IEquatable<TypeConvertKey>
		{
			private readonly Type _initialType;

			private readonly Type _targetType;

			public Type InitialType => _initialType;

			public Type TargetType => _targetType;

			public TypeConvertKey(Type initialType, Type targetType)
			{
				_initialType = initialType;
				_targetType = targetType;
			}

			public override int GetHashCode()
			{
				return _initialType.GetHashCode() ^ _targetType.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (!(obj is TypeConvertKey))
				{
					return false;
				}
				return Equals((TypeConvertKey)obj);
			}

			public bool Equals(TypeConvertKey other)
			{
				return _initialType == other._initialType && _targetType == other._targetType;
			}
		}

		private static readonly ThreadSafeStore<TypeConvertKey, Func<object, object>> CastConverters = new ThreadSafeStore<TypeConvertKey, Func<object, object>>(CreateCastConverter);

		[CompilerGenerated]
		private static Func<TypeConvertKey, Func<object, object>> _003C_003Ef__mg_0024cache0;

		private static Func<object, object> CreateCastConverter(TypeConvertKey t)
		{
			MethodInfo method = t.TargetType.GetMethod("op_Implicit", new Type[1]
			{
				t.InitialType
			});
			if (method == null)
			{
				method = t.TargetType.GetMethod("op_Explicit", new Type[1]
				{
					t.InitialType
				});
			}
			if (method == null)
			{
				return null;
			}
			MethodCall<object, object> call = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object o) => call(null, o);
		}

		public static bool CanConvertType(Type initialType, Type targetType, bool allowTypeNameToString)
		{
			ValidationUtils.ArgumentNotNull(initialType, "initialType");
			ValidationUtils.ArgumentNotNull(targetType, "targetType");
			if (ReflectionUtils.IsNullableType(targetType))
			{
				targetType = Nullable.GetUnderlyingType(targetType);
			}
			if (targetType == initialType)
			{
				return true;
			}
			if (typeof(IConvertible).IsAssignableFrom(initialType) && typeof(IConvertible).IsAssignableFrom(targetType))
			{
				return true;
			}
			if (initialType == typeof(DateTime) && targetType == typeof(DateTimeOffset))
			{
				return true;
			}
			if (initialType == typeof(Guid) && (targetType == typeof(Guid) || targetType == typeof(string)))
			{
				return true;
			}
			if (initialType == typeof(Type) && targetType == typeof(string))
			{
				return true;
			}
			TypeConverter converter = GetConverter(initialType);
			if (converter != null && !IsComponentConverter(converter) && converter.CanConvertTo(targetType) && (allowTypeNameToString || converter.GetType() != typeof(TypeConverter)))
			{
				return true;
			}
			TypeConverter converter2 = GetConverter(targetType);
			if (converter2 != null && !IsComponentConverter(converter2) && converter2.CanConvertFrom(initialType))
			{
				return true;
			}
			if (initialType == typeof(DBNull) && ReflectionUtils.IsNullable(targetType))
			{
				return true;
			}
			return false;
		}

		private static bool IsComponentConverter(TypeConverter converter)
		{
			return converter is ComponentConverter;
		}

		public static T Convert<T>(object initialValue)
		{
			return Convert<T>(initialValue, CultureInfo.CurrentCulture);
		}

		public static T Convert<T>(object initialValue, CultureInfo culture)
		{
			return (T)Convert(initialValue, culture, typeof(T));
		}

		public static object Convert(object initialValue, CultureInfo culture, Type targetType)
		{
			if (initialValue == null)
			{
				throw new ArgumentNullException("initialValue");
			}
			if (ReflectionUtils.IsNullableType(targetType))
			{
				targetType = Nullable.GetUnderlyingType(targetType);
			}
			Type type = initialValue.GetType();
			if (targetType == type)
			{
				return initialValue;
			}
			if (initialValue is string && typeof(Type).IsAssignableFrom(targetType))
			{
				return Type.GetType((string)initialValue, throwOnError: true);
			}
			if (targetType.IsInterface || targetType.IsGenericTypeDefinition || targetType.IsAbstract)
			{
				throw new ArgumentException("Target type {0} is not a value type or a non-abstract class.".FormatWith(CultureInfo.InvariantCulture, targetType), "targetType");
			}
			if (initialValue is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
			{
				if (targetType.IsEnum)
				{
					if (initialValue is string)
					{
						return Enum.Parse(targetType, initialValue.ToString(), ignoreCase: true);
					}
					if (IsInteger(initialValue))
					{
						return Enum.ToObject(targetType, initialValue);
					}
				}
				return System.Convert.ChangeType(initialValue, targetType, culture);
			}
			if (initialValue is DateTime && targetType == typeof(DateTimeOffset))
			{
				return new DateTimeOffset((DateTime)initialValue);
			}
			if (initialValue is string)
			{
				if (targetType == typeof(Guid))
				{
					return new Guid((string)initialValue);
				}
				if (targetType == typeof(Uri))
				{
					return new Uri((string)initialValue);
				}
				if (targetType == typeof(TimeSpan))
				{
					return TimeSpan.Parse((string)initialValue);
				}
			}
			TypeConverter converter = GetConverter(type);
			if (converter != null && converter.CanConvertTo(targetType))
			{
				return converter.ConvertTo(null, culture, initialValue, targetType);
			}
			TypeConverter converter2 = GetConverter(targetType);
			if (converter2 != null && converter2.CanConvertFrom(type))
			{
				return converter2.ConvertFrom(null, culture, initialValue);
			}
			if (initialValue == DBNull.Value)
			{
				if (ReflectionUtils.IsNullable(targetType))
				{
					return EnsureTypeAssignable(null, type, targetType);
				}
				throw new Exception("Can not convert null {0} into non-nullable {1}.".FormatWith(CultureInfo.InvariantCulture, type, targetType));
			}
			throw new Exception("Can not convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, type, targetType));
		}

		public static bool TryConvert<T>(object initialValue, out T convertedValue)
		{
			return TryConvert(initialValue, CultureInfo.CurrentCulture, out convertedValue);
		}

		public static bool TryConvert<T>(object initialValue, CultureInfo culture, out T convertedValue)
		{
			return MiscellaneousUtils.TryAction(delegate
			{
				TryConvert(initialValue, CultureInfo.CurrentCulture, typeof(T), out object convertedValue2);
				return (T)convertedValue2;
			}, out convertedValue);
		}

		public static bool TryConvert(object initialValue, CultureInfo culture, Type targetType, out object convertedValue)
		{
			return MiscellaneousUtils.TryAction(() => Convert(initialValue, culture, targetType), out convertedValue);
		}

		public static T ConvertOrCast<T>(object initialValue)
		{
			return ConvertOrCast<T>(initialValue, CultureInfo.CurrentCulture);
		}

		public static T ConvertOrCast<T>(object initialValue, CultureInfo culture)
		{
			return (T)ConvertOrCast(initialValue, culture, typeof(T));
		}

		public static object ConvertOrCast(object initialValue, CultureInfo culture, Type targetType)
		{
			if (targetType == typeof(object))
			{
				return initialValue;
			}
			if (initialValue == null && ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			if (TryConvert(initialValue, culture, targetType, out object convertedValue))
			{
				return convertedValue;
			}
			return EnsureTypeAssignable(initialValue, ReflectionUtils.GetObjectType(initialValue), targetType);
		}

		public static bool TryConvertOrCast<T>(object initialValue, out T convertedValue)
		{
			return TryConvertOrCast(initialValue, CultureInfo.CurrentCulture, out convertedValue);
		}

		public static bool TryConvertOrCast<T>(object initialValue, CultureInfo culture, out T convertedValue)
		{
			return MiscellaneousUtils.TryAction(delegate
			{
				TryConvertOrCast(initialValue, CultureInfo.CurrentCulture, typeof(T), out object convertedValue2);
				return (T)convertedValue2;
			}, out convertedValue);
		}

		public static bool TryConvertOrCast(object initialValue, CultureInfo culture, Type targetType, out object convertedValue)
		{
			return MiscellaneousUtils.TryAction(() => ConvertOrCast(initialValue, culture, targetType), out convertedValue);
		}

		private static object EnsureTypeAssignable(object value, Type initialType, Type targetType)
		{
			Type type = value?.GetType();
			if (value != null)
			{
				if (targetType.IsAssignableFrom(type))
				{
					return value;
				}
				Func<object, object> func = CastConverters.Get(new TypeConvertKey(type, targetType));
				if (func != null)
				{
					return func(value);
				}
			}
			else if (ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			throw new Exception("Could not cast or convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, (initialType == null) ? "{null}" : initialType.ToString(), targetType));
		}

		internal static TypeConverter GetConverter(Type t)
		{
			return JsonTypeReflector.GetTypeConverter(t);
		}

		public static bool IsInteger(object value)
		{
			switch (System.Convert.GetTypeCode(value))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return true;
			default:
				return false;
			}
		}
	}
}
