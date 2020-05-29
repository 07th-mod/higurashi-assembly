using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	internal interface IWrappedCollection : IList, IEnumerable, ICollection
	{
		object UnderlyingCollection
		{
			get;
		}
	}
}
