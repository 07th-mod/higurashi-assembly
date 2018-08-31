using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
	internal class DictionaryWrapper<TKey, TValue> : IEnumerable, IWrappedDictionary, ICollection, IDictionary, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private struct DictionaryEnumerator<TEnumeratorKey, TEnumeratorValue> : IEnumerator, IDictionaryEnumerator
		{
			private readonly IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;

			public DictionaryEntry Entry => (DictionaryEntry)Current;

			public object Key => Entry.Key;

			public object Value => Entry.Value;

			public object Current => new DictionaryEntry(_e.Current.Key, _e.Current.Value);

			public DictionaryEnumerator(IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
			{
				ValidationUtils.ArgumentNotNull(e, "e");
				_e = e;
			}

			public bool MoveNext()
			{
				return _e.MoveNext();
			}

			public void Reset()
			{
				_e.Reset();
			}
		}

		private readonly IDictionary _dictionary;

		private readonly IDictionary<TKey, TValue> _genericDictionary;

		private object _syncRoot;

		bool IDictionary.IsFixedSize
		{
			get
			{
				if (_genericDictionary != null)
				{
					return false;
				}
				return _dictionary.IsFixedSize;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				if (_genericDictionary != null)
				{
					return _genericDictionary.Keys.ToList();
				}
				return _dictionary.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				if (_genericDictionary != null)
				{
					return _genericDictionary.Values.ToList();
				}
				return _dictionary.Values;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				if (_genericDictionary != null)
				{
					return _genericDictionary[(TKey)key];
				}
				return _dictionary[key];
			}
			set
			{
				if (_genericDictionary != null)
				{
					_genericDictionary[(TKey)key] = (TValue)value;
				}
				else
				{
					_dictionary[key] = value;
				}
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				if (_genericDictionary != null)
				{
					return false;
				}
				return _dictionary.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (_syncRoot == null)
				{
					Interlocked.CompareExchange(ref _syncRoot, new object(), null);
				}
				return _syncRoot;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				if (_genericDictionary != null)
				{
					return _genericDictionary.Keys;
				}
				return _dictionary.Keys.Cast<TKey>().ToList();
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				if (_genericDictionary != null)
				{
					return _genericDictionary.Values;
				}
				return _dictionary.Values.Cast<TValue>().ToList();
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (_genericDictionary != null)
				{
					return _genericDictionary[key];
				}
				return (TValue)_dictionary[key];
			}
			set
			{
				if (_genericDictionary != null)
				{
					_genericDictionary[key] = value;
				}
				else
				{
					_dictionary[key] = value;
				}
			}
		}

		public int Count
		{
			get
			{
				if (_genericDictionary != null)
				{
					return _genericDictionary.Count;
				}
				return _dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				if (_genericDictionary != null)
				{
					return _genericDictionary.IsReadOnly;
				}
				return _dictionary.IsReadOnly;
			}
		}

		public object UnderlyingDictionary
		{
			get
			{
				if (_genericDictionary != null)
				{
					return _genericDictionary;
				}
				return _dictionary;
			}
		}

		public DictionaryWrapper(IDictionary dictionary)
		{
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
			_dictionary = dictionary;
		}

		public DictionaryWrapper(IDictionary<TKey, TValue> dictionary)
		{
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
			_genericDictionary = dictionary;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void IDictionary.Add(object key, object value)
		{
			if (_genericDictionary != null)
			{
				_genericDictionary.Add((TKey)key, (TValue)value);
			}
			else
			{
				_dictionary.Add(key, value);
			}
		}

		bool IDictionary.Contains(object key)
		{
			if (_genericDictionary != null)
			{
				return _genericDictionary.ContainsKey((TKey)key);
			}
			return _dictionary.Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			if (_genericDictionary != null)
			{
				return new DictionaryEnumerator<TKey, TValue>(_genericDictionary.GetEnumerator());
			}
			return _dictionary.GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (_genericDictionary != null)
			{
				_genericDictionary.CopyTo((KeyValuePair<TKey, TValue>[])array, index);
			}
			else
			{
				_dictionary.CopyTo(array, index);
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (_genericDictionary != null)
			{
				_genericDictionary.Add(key, value);
			}
			else
			{
				_dictionary.Add(key, value);
			}
		}

		public bool ContainsKey(TKey key)
		{
			if (_genericDictionary != null)
			{
				return _genericDictionary.ContainsKey(key);
			}
			return _dictionary.Contains(key);
		}

		public bool Remove(TKey key)
		{
			if (_genericDictionary != null)
			{
				return _genericDictionary.Remove(key);
			}
			if (_dictionary.Contains(key))
			{
				_dictionary.Remove(key);
				return true;
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (_genericDictionary != null)
			{
				return _genericDictionary.TryGetValue(key, out value);
			}
			if (!_dictionary.Contains(key))
			{
				value = default(TValue);
				return false;
			}
			value = (TValue)_dictionary[key];
			return true;
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			if (_genericDictionary != null)
			{
				_genericDictionary.Add(item);
			}
			else
			{
				((IList)_dictionary).Add(item);
			}
		}

		public void Clear()
		{
			if (_genericDictionary != null)
			{
				_genericDictionary.Clear();
			}
			else
			{
				_dictionary.Clear();
			}
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if (_genericDictionary != null)
			{
				return _genericDictionary.Contains(item);
			}
			return ((IList)_dictionary).Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (_genericDictionary != null)
			{
				_genericDictionary.CopyTo(array, arrayIndex);
			}
			else
			{
				IDictionaryEnumerator enumerator = _dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						array[arrayIndex++] = new KeyValuePair<TKey, TValue>((TKey)dictionaryEntry.Key, (TValue)dictionaryEntry.Value);
					}
				}
				finally
				{
					(enumerator as IDisposable)?.Dispose();
				}
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (_genericDictionary != null)
			{
				return _genericDictionary.Remove(item);
			}
			if (_dictionary.Contains(item.Key))
			{
				object objA = _dictionary[item.Key];
				if (object.Equals(objA, item.Value))
				{
					_dictionary.Remove(item.Key);
					return true;
				}
				return false;
			}
			return true;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			if (_genericDictionary != null)
			{
				return _genericDictionary.GetEnumerator();
			}
			return (from DictionaryEntry de in _dictionary
			select new KeyValuePair<TKey, TValue>((TKey)de.Key, (TValue)de.Value)).GetEnumerator();
		}

		public void Remove(object key)
		{
			if (_genericDictionary != null)
			{
				_genericDictionary.Remove((TKey)key);
			}
			else
			{
				_dictionary.Remove(key);
			}
		}
	}
}
