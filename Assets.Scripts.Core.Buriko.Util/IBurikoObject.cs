using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.Core.Buriko.Util
{
	public interface IBurikoObject
	{
		void Create(int members);

		int MemberCount();

		IEnumerator<BurikoVariable> GetObjects();

		BurikoVariable GetObject(BurikoReference reference);

		void SetValue(BurikoReference reference, BurikoVariable var);

		string GetObjectType();

		void Serialize(MemoryStream ms);

		void DeSerialize(MemoryStream ms);

		string StringValue(BurikoReference reference);

		int IntValue(BurikoReference reference);
	}
}
