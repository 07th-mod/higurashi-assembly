using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq
{
	public interface IJEnumerable<T> : IEnumerable, IEnumerable<T> where T : JToken
	{
		IJEnumerable<JToken> this[object key]
		{
			get;
		}
	}
}
