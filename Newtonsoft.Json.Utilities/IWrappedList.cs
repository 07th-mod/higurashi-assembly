using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	internal interface IWrappedList : IEnumerable, IList, ICollection
	{
		object UnderlyingList
		{
			get;
		}
	}
}
