using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
	internal class BidirectionalDictionary<TFirst, TSecond>
	{
		private readonly IDictionary<TFirst, TSecond> _firstToSecond;

		private readonly IDictionary<TSecond, TFirst> _secondToFirst;

		public BidirectionalDictionary()
			: this((IEqualityComparer<TFirst>)EqualityComparer<TFirst>.Default, (IEqualityComparer<TSecond>)EqualityComparer<TSecond>.Default)
		{
		}

		public BidirectionalDictionary(IEqualityComparer<TFirst> firstEqualityComparer, IEqualityComparer<TSecond> secondEqualityComparer)
		{
			_firstToSecond = new Dictionary<TFirst, TSecond>(firstEqualityComparer);
			_secondToFirst = new Dictionary<TSecond, TFirst>(secondEqualityComparer);
		}

		public void Add(TFirst first, TSecond second)
		{
			if (_firstToSecond.ContainsKey(first) || _secondToFirst.ContainsKey(second))
			{
				throw new ArgumentException("Duplicate first or second");
			}
			_firstToSecond.Add(first, second);
			_secondToFirst.Add(second, first);
		}

		public bool TryGetByFirst(TFirst first, out TSecond second)
		{
			return _firstToSecond.TryGetValue(first, out second);
		}

		public bool TryGetBySecond(TSecond second, out TFirst first)
		{
			return _secondToFirst.TryGetValue(second, out first);
		}
	}
}
