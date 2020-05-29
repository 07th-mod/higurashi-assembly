using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	internal interface IWrappedList : IList, IEnumerable, ICollection
	{
		object UnderlyingList
		{
			get;
		}
	}
}
