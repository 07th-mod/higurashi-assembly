using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	internal interface IWrappedList : IList, ICollection, IEnumerable
	{
		object UnderlyingList
		{
			get;
		}
	}
}
