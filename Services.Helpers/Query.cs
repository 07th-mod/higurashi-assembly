using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Services.Helpers
{
	internal class Query : IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		public class KeyCollection : IEnumerable<string>, ICollection<string>, IEnumerable
		{
			private readonly Query query;

			public int Count => query.Count;

			public bool IsReadOnly => true;

			public KeyCollection(Query query)
			{
				this.query = query;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void Add(string item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(string item)
			{
				for (int i = 0; i < query.Count; i++)
				{
					KeyValuePair<string, string> keyValuePair = query.Pairs[i];
					if (item == keyValuePair.Key)
					{
						return true;
					}
				}
				return false;
			}

			public void CopyTo(string[] array, int arrayIndex)
			{
				for (int i = 0; i < query.Count; i++)
				{
					array[arrayIndex++] = query.Pairs[i].Key;
				}
			}

			public IEnumerator<string> GetEnumerator()
			{
				for (int i = 0; i < query.Count; i++)
				{
					yield return query.Pairs[i].Key;
				}
			}

			public bool Remove(string item)
			{
				throw new NotSupportedException();
			}
		}

		public class ValueCollection : IEnumerable<string>, ICollection<string>, IEnumerable
		{
			private readonly Query query;

			public int Count => query.Count;

			public bool IsReadOnly => true;

			public ValueCollection(Query query)
			{
				this.query = query;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void Add(string item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(string item)
			{
				for (int i = 0; i < query.Count; i++)
				{
					KeyValuePair<string, string> keyValuePair = query.Pairs[i];
					if (item == keyValuePair.Value)
					{
						return true;
					}
				}
				return false;
			}

			public void CopyTo(string[] array, int arrayIndex)
			{
				for (int i = 0; i < query.Count; i++)
				{
					array[arrayIndex++] = query.Pairs[i].Value;
				}
			}

			public IEnumerator<string> GetEnumerator()
			{
				for (int i = 0; i < query.Count; i++)
				{
					yield return query.Pairs[i].Value;
				}
			}

			public bool Remove(string item)
			{
				throw new NotSupportedException();
			}
		}

		private int _count;

		private readonly string _baseUri;

		private KeyValuePair<string, string>[] _pairs;

		ICollection<string> IDictionary<string, string>.Keys
		{
			get
			{
				List<string> list = new List<string>();
				KeyValuePair<string, string>[] pairs = _pairs;
				foreach (KeyValuePair<string, string> keyValuePair in pairs)
				{
					list.Add(keyValuePair.Key);
				}
				return list;
			}
		}

		ICollection<string> IDictionary<string, string>.Values
		{
			get
			{
				List<string> list = new List<string>();
				KeyValuePair<string, string>[] pairs = _pairs;
				foreach (KeyValuePair<string, string> keyValuePair in pairs)
				{
					list.Add(keyValuePair.Value);
				}
				return list;
			}
		}

		public string this[string key]
		{
			get
			{
				for (int i = 0; i < _count; i++)
				{
					KeyValuePair<string, string> keyValuePair = _pairs[i];
					if (keyValuePair.Key == key)
					{
						return keyValuePair.Value;
					}
				}
				throw new KeyNotFoundException();
			}
			set
			{
				for (int i = 0; i < _count; i++)
				{
					KeyValuePair<string, string> keyValuePair = _pairs[i];
					if (keyValuePair.Key == key)
					{
						_pairs[i] = new KeyValuePair<string, string>(key, value);
						return;
					}
				}
				throw new KeyNotFoundException();
			}
		}

		public string BaseUri => _baseUri;

		public int Count => _count;

		public bool IsReadOnly => false;

		public KeyCollection Keys => new KeyCollection(this);

		public KeyValuePair<string, string>[] Pairs => _pairs;

		public ValueCollection Values => new ValueCollection(this);

		public Query(string uri)
		{
			int num = uri.IndexOf('?');
			if (num == -1)
			{
				int num2 = uri.IndexOf('&');
				if (num2 == -1)
				{
					_baseUri = uri;
					return;
				}
				_baseUri = null;
			}
			else
			{
				_baseUri = uri.Substring(0, num);
				uri = uri.Substring(num + 1);
			}
			string[] array = uri.Split('&');
			_pairs = new KeyValuePair<string, string>[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				int num3 = text.IndexOf('=');
				string empty = string.Empty;
				string empty2 = string.Empty;
				if (num3 != text.LastIndexOf('='))
				{
					empty = text.Substring(0, num3);
					empty2 = string.Empty;
				}
				else
				{
					empty = text.Substring(0, num3);
					empty2 = text.Substring(num3 + 1);
				}
				_pairs[i] = new KeyValuePair<string, string>(empty, empty2);
			}
			_count = array.Length;
		}

		void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
		{
			for (int i = 0; i < _count; i++)
			{
				KeyValuePair<string, string> keyValuePair = _pairs[i];
				if (item.Key == keyValuePair.Key && item.Value == keyValuePair.Value)
				{
					return true;
				}
			}
			return false;
		}

		void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			Array.Copy(_pairs, 0, array, arrayIndex, _count);
		}

		bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
		{
			return Remove(item.Key);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(string key, string value)
		{
			EnsureCapacity(_count + 1);
			_pairs[_count++] = new KeyValuePair<string, string>(key, value);
		}

		public void Clear()
		{
			if (_count != 0)
			{
				Array.Clear(_pairs, 0, _count);
				_count = 0;
			}
		}

		public bool ContainsKey(string key)
		{
			for (int i = 0; i < _count; i++)
			{
				if (key == _pairs[i].Key)
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			for (int i = 0; i < _count; i++)
			{
				yield return _pairs[i];
			}
		}

		public bool Remove(string key)
		{
			for (int i = 0; i < _count; i++)
			{
				KeyValuePair<string, string> keyValuePair = _pairs[i];
				if (keyValuePair.Key == key)
				{
					if (i != _count--)
					{
						Array.Copy(_pairs, i + 1, _pairs, i, _count - i);
					}
					_pairs[_count] = default(KeyValuePair<string, string>);
					return true;
				}
			}
			return false;
		}

		public bool TryGetValue(string key, out string value)
		{
			for (int i = 0; i < _count; i++)
			{
				KeyValuePair<string, string> keyValuePair = _pairs[i];
				if (key == keyValuePair.Key)
				{
					value = keyValuePair.Value;
					return true;
				}
			}
			value = null;
			return false;
		}

		public override string ToString()
		{
			if (_count == 0)
			{
				return _baseUri;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (_baseUri != null)
			{
				stringBuilder.Append(_baseUri).Append('?');
			}
			KeyValuePair<string, string> keyValuePair = _pairs[0];
			stringBuilder.Append(keyValuePair.Key).Append('=').Append(keyValuePair.Value);
			for (int i = 1; i < _count; i++)
			{
				keyValuePair = _pairs[i];
				stringBuilder.Append('&').Append(keyValuePair.Key).Append('=')
					.Append(keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		private void EnsureCapacity(int capacity)
		{
			if (capacity > _pairs.Length)
			{
				capacity = Math.Max(capacity, _pairs.Length * 2);
				Array.Resize(ref _pairs, capacity);
			}
		}
	}
}
