using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.Utilities
{
	internal static class CollectionUtils
	{
		public static IEnumerable<T> CastValid<T>(this IEnumerable enumerable)
		{
			ValidationUtils.ArgumentNotNull(enumerable, "enumerable");
			return (from object o in enumerable
			where o is T
			select o).Cast<T>();
		}

		public static List<T> CreateList<T>(params T[] values)
		{
			return new List<T>(values);
		}

		public static bool IsNullOrEmpty(ICollection collection)
		{
			if (collection != null)
			{
				return collection.Count == 0;
			}
			return true;
		}

		public static bool IsNullOrEmpty<T>(ICollection<T> collection)
		{
			if (collection != null)
			{
				return collection.Count == 0;
			}
			return true;
		}

		public static bool IsNullOrEmptyOrDefault<T>(IList<T> list)
		{
			if (IsNullOrEmpty(list))
			{
				return true;
			}
			return ReflectionUtils.ItemsUnitializedValue(list);
		}

		public static IList<T> Slice<T>(IList<T> list, int? start, int? end)
		{
			return Slice(list, start, end, null);
		}

		public static IList<T> Slice<T>(IList<T> list, int? start, int? end, int? step)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (step.GetValueOrDefault() == 0 && step.HasValue)
			{
				throw new ArgumentException("Step cannot be zero.", "step");
			}
			List<T> list2 = new List<T>();
			if (list.Count == 0)
			{
				return list2;
			}
			int num = (!step.HasValue) ? 1 : step.Value;
			int num2 = start.HasValue ? start.Value : 0;
			int num3 = (!end.HasValue) ? list.Count : end.Value;
			num2 = ((num2 >= 0) ? num2 : (list.Count + num2));
			num3 = ((num3 >= 0) ? num3 : (list.Count + num3));
			num2 = Math.Max(num2, 0);
			num3 = Math.Min(num3, list.Count - 1);
			for (int i = num2; i < num3; i += num)
			{
				list2.Add(list[i]);
			}
			return list2;
		}

		public static Dictionary<K, List<V>> GroupBy<K, V>(ICollection<V> source, Func<V, K> keySelector)
		{
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			Dictionary<K, List<V>> dictionary = new Dictionary<K, List<V>>();
			foreach (V item in source)
			{
				K key = keySelector(item);
				if (!dictionary.TryGetValue(key, out List<V> value))
				{
					value = new List<V>();
					dictionary.Add(key, value);
				}
				value.Add(item);
			}
			return dictionary;
		}

		public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection)
		{
			if (initial == null)
			{
				throw new ArgumentNullException("initial");
			}
			if (collection != null)
			{
				foreach (T item in collection)
				{
					initial.Add(item);
				}
			}
		}

		public static void AddRange(this IList initial, IEnumerable collection)
		{
			ValidationUtils.ArgumentNotNull(initial, "initial");
			ListWrapper<object> initial2 = new ListWrapper<object>(initial);
			initial2.AddRange(collection.Cast<object>());
		}

		public static List<T> Distinct<T>(List<T> collection)
		{
			List<T> list = new List<T>();
			foreach (T item in collection)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static List<List<T>> Flatten<T>(params IList<T>[] lists)
		{
			List<List<T>> list = new List<List<T>>();
			Dictionary<int, T> currentSet = new Dictionary<int, T>();
			Recurse(new List<IList<T>>(lists), 0, currentSet, list);
			return list;
		}

		private static void Recurse<T>(IList<IList<T>> global, int current, Dictionary<int, T> currentSet, List<List<T>> flattenedResult)
		{
			IList<T> list = global[current];
			for (int i = 0; i < list.Count; i++)
			{
				currentSet[current] = list[i];
				if (current == global.Count - 1)
				{
					List<T> list2 = new List<T>();
					for (int j = 0; j < currentSet.Count; j++)
					{
						list2.Add(currentSet[j]);
					}
					flattenedResult.Add(list2);
				}
				else
				{
					Recurse(global, current + 1, currentSet, flattenedResult);
				}
			}
		}

		public static List<T> CreateList<T>(ICollection collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			T[] array = new T[collection.Count];
			collection.CopyTo(array, 0);
			return new List<T>(array);
		}

		public static bool ListEquals<T>(IList<T> a, IList<T> b)
		{
			if (a == null || b == null)
			{
				return a == null && b == null;
			}
			if (a.Count != b.Count)
			{
				return false;
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < a.Count; i++)
			{
				if (!@default.Equals(a[i], b[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool TryGetSingleItem<T>(IList<T> list, out T value)
		{
			return TryGetSingleItem(list, /*returnDefaultIfEmpty:*/ false, out value);
		}

		public static bool TryGetSingleItem<T>(IList<T> list, bool returnDefaultIfEmpty, out T value)
		{
			return MiscellaneousUtils.TryAction(() => GetSingleItem(list, returnDefaultIfEmpty), out value);
		}

		public static T GetSingleItem<T>(IList<T> list)
		{
			return GetSingleItem(list, /*returnDefaultIfEmpty:*/ false);
		}

		public static T GetSingleItem<T>(IList<T> list, bool returnDefaultIfEmpty)
		{
			if (list.Count == 1)
			{
				return list[0];
			}
			if (returnDefaultIfEmpty && list.Count == 0)
			{
				return default(T);
			}
			throw new Exception("Expected single {0} in list but got {1}.".FormatWith(CultureInfo.InvariantCulture, typeof(T), list.Count));
		}

		public static IList<T> Minus<T>(IList<T> list, IList<T> minus)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			List<T> list2 = new List<T>(list.Count);
			foreach (T item in list)
			{
				if (minus == null || !minus.Contains(item))
				{
					list2.Add(item);
				}
			}
			return list2;
		}

		public static IList CreateGenericList(Type listType)
		{
			ValidationUtils.ArgumentNotNull(listType, "listType");
			return (IList)ReflectionUtils.CreateGeneric(typeof(List<>), listType);
		}

		public static IDictionary CreateGenericDictionary(Type keyType, Type valueType)
		{
			ValidationUtils.ArgumentNotNull(keyType, "keyType");
			ValidationUtils.ArgumentNotNull(valueType, "valueType");
			return (IDictionary)ReflectionUtils.CreateGeneric(typeof(Dictionary<, >), keyType, valueType);
		}

		public static bool IsListType(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (type.IsArray)
			{
				return true;
			}
			if (typeof(IList).IsAssignableFrom(type))
			{
				return true;
			}
			if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IList<>)))
			{
				return true;
			}
			return false;
		}

		public static bool IsCollectionType(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (type.IsArray)
			{
				return true;
			}
			if (typeof(ICollection).IsAssignableFrom(type))
			{
				return true;
			}
			if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(ICollection<>)))
			{
				return true;
			}
			return false;
		}

		public static bool IsDictionaryType(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				return true;
			}
			if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IDictionary<, >)))
			{
				return true;
			}
			return false;
		}

		public static IWrappedCollection CreateCollectionWrapper(object list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			if (ReflectionUtils.ImplementsGenericDefinition(list.GetType(), typeof(ICollection<>), out Type collectionDefinition))
			{
				Type collectionItemType = ReflectionUtils.GetCollectionItemType(collectionDefinition);
				Func<Type, IList<object>, object> instanceCreator = delegate(Type t, IList<object> a)
				{
					ConstructorInfo constructor = t.GetConstructor(new Type[1]
					{
						collectionDefinition
					});
					return constructor.Invoke(new object[1]
					{
						list
					});
				};
				return (IWrappedCollection)ReflectionUtils.CreateGeneric(typeof(CollectionWrapper<>), new Type[1]
				{
					collectionItemType
				}, instanceCreator, list);
			}
			if (list is IList)
			{
				return new CollectionWrapper<object>((IList)list);
			}
			throw new Exception("Can not create ListWrapper for type {0}.".FormatWith(CultureInfo.InvariantCulture, list.GetType()));
		}

		public static IWrappedList CreateListWrapper(object list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			if (ReflectionUtils.ImplementsGenericDefinition(list.GetType(), typeof(IList<>), out Type listDefinition))
			{
				Type collectionItemType = ReflectionUtils.GetCollectionItemType(listDefinition);
				Func<Type, IList<object>, object> instanceCreator = delegate(Type t, IList<object> a)
				{
					ConstructorInfo constructor = t.GetConstructor(new Type[1]
					{
						listDefinition
					});
					return constructor.Invoke(new object[1]
					{
						list
					});
				};
				return (IWrappedList)ReflectionUtils.CreateGeneric(typeof(ListWrapper<>), new Type[1]
				{
					collectionItemType
				}, instanceCreator, list);
			}
			if (list is IList)
			{
				return new ListWrapper<object>((IList)list);
			}
			throw new Exception("Can not create ListWrapper for type {0}.".FormatWith(CultureInfo.InvariantCulture, list.GetType()));
		}

		public static IWrappedDictionary CreateDictionaryWrapper(object dictionary)
		{
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
			if (ReflectionUtils.ImplementsGenericDefinition(dictionary.GetType(), typeof(IDictionary<, >), out Type dictionaryDefinition))
			{
				Type dictionaryKeyType = ReflectionUtils.GetDictionaryKeyType(dictionaryDefinition);
				Type dictionaryValueType = ReflectionUtils.GetDictionaryValueType(dictionaryDefinition);
				Func<Type, IList<object>, object> instanceCreator = delegate(Type t, IList<object> a)
				{
					ConstructorInfo constructor = t.GetConstructor(new Type[1]
					{
						dictionaryDefinition
					});
					return constructor.Invoke(new object[1]
					{
						dictionary
					});
				};
				return (IWrappedDictionary)ReflectionUtils.CreateGeneric(typeof(DictionaryWrapper<, >), new Type[2]
				{
					dictionaryKeyType,
					dictionaryValueType
				}, instanceCreator, dictionary);
			}
			if (dictionary is IDictionary)
			{
				return new DictionaryWrapper<object, object>((IDictionary)dictionary);
			}
			throw new Exception("Can not create DictionaryWrapper for type {0}.".FormatWith(CultureInfo.InvariantCulture, dictionary.GetType()));
		}

		public static object CreateAndPopulateList(Type listType, Action<IList, bool> populateList)
		{
			ValidationUtils.ArgumentNotNull(listType, "listType");
			ValidationUtils.ArgumentNotNull(populateList, "populateList");
			bool flag = false;
			IList list;
			Type implementingType;
			if (listType.IsArray)
			{
				list = new List<object>();
				flag = true;
			}
			else if (!ReflectionUtils.InheritsGenericDefinition(listType, typeof(ReadOnlyCollection<>), out implementingType))
			{
				list = (typeof(IList).IsAssignableFrom(listType) ? (ReflectionUtils.IsInstantiatableType(listType) ? ((IList)Activator.CreateInstance(listType)) : ((listType != typeof(IList)) ? null : new List<object>())) : ((!ReflectionUtils.ImplementsGenericDefinition(listType, typeof(ICollection<>))) ? null : ((!ReflectionUtils.IsInstantiatableType(listType)) ? null : CreateCollectionWrapper(Activator.CreateInstance(listType)))));
			}
			else
			{
				Type type = implementingType.GetGenericArguments()[0];
				Type type2 = ReflectionUtils.MakeGenericType(typeof(IEnumerable<>), type);
				bool flag2 = false;
				ConstructorInfo[] constructors = listType.GetConstructors();
				foreach (ConstructorInfo constructorInfo in constructors)
				{
					IList<ParameterInfo> parameters = constructorInfo.GetParameters();
					if (parameters.Count == 1 && type2.IsAssignableFrom(parameters[0].ParameterType))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					throw new Exception("Read-only type {0} does not have a public constructor that takes a type that implements {1}.".FormatWith(CultureInfo.InvariantCulture, listType, type2));
				}
				list = CreateGenericList(type);
				flag = true;
			}
			if (list == null)
			{
				throw new Exception("Cannot create and populate list type {0}.".FormatWith(CultureInfo.InvariantCulture, listType));
			}
			populateList(list, flag);
			if (flag)
			{
				if (listType.IsArray)
				{
					list = ToArray(((List<object>)list).ToArray(), ReflectionUtils.GetCollectionItemType(listType));
				}
				else if (ReflectionUtils.InheritsGenericDefinition(listType, typeof(ReadOnlyCollection<>)))
				{
					list = (IList)ReflectionUtils.CreateInstance(listType, list);
				}
			}
			else if (list is IWrappedCollection)
			{
				return ((IWrappedCollection)list).UnderlyingCollection;
			}
			return list;
		}

		public static Array ToArray(Array initial, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Array array = Array.CreateInstance(type, initial.Length);
			Array.Copy(initial, 0, array, 0, initial.Length);
			return array;
		}

		public static bool AddDistinct<T>(this IList<T> list, T value)
		{
			return list.AddDistinct(value, EqualityComparer<T>.Default);
		}

		public static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
		{
			if (list.ContainsValue(value, comparer))
			{
				return false;
			}
			list.Add(value);
			return true;
		}

		public static bool ContainsValue<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			foreach (TSource item in source)
			{
				if (comparer.Equals(item, value))
				{
					return true;
				}
			}
			return false;
		}

		public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values)
		{
			return list.AddRangeDistinct(values, EqualityComparer<T>.Default);
		}

		public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer)
		{
			bool result = true;
			foreach (T value in values)
			{
				if (!list.AddDistinct(value, comparer))
				{
					result = false;
				}
			}
			return result;
		}

		public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			int num = 0;
			foreach (T item in collection)
			{
				if (predicate(item))
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value) where TSource : IEquatable<TSource>
		{
			return list.IndexOf(value, EqualityComparer<TSource>.Default);
		}

		public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value, IEqualityComparer<TSource> comparer)
		{
			int num = 0;
			foreach (TSource item in list)
			{
				if (comparer.Equals(item, value))
				{
					return num;
				}
				num++;
			}
			return -1;
		}
	}
}
