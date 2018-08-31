using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	internal interface IWrappedDictionary : IEnumerable, ICollection, IDictionary
	{
		object UnderlyingDictionary
		{
			get;
		}
	}
}
