using Newtonsoft.Json.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Linq
{
	public struct JEnumerable<T> : IJEnumerable<T>, IEnumerable<T>, IEnumerable where T : JToken
	{
		public static readonly JEnumerable<T> Empty = new JEnumerable<T>(Enumerable.Empty<T>());

		private IEnumerable<T> _enumerable;

		public IJEnumerable<JToken> this[object key]
		{
			get
			{
				return new JEnumerable<JToken>(_enumerable.Values<T, JToken>(key));
			}
		}

		public JEnumerable(IEnumerable<T> enumerable)
		{
			ValidationUtils.ArgumentNotNull(enumerable, "enumerable");
			_enumerable = enumerable;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _enumerable.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override bool Equals(object obj)
		{
			if (obj is JEnumerable<T>)
			{
				IEnumerable<T> enumerable = _enumerable;
				JEnumerable<T> jEnumerable = (JEnumerable<T>)obj;
				return enumerable.Equals(jEnumerable._enumerable);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _enumerable.GetHashCode();
		}
	}
}
