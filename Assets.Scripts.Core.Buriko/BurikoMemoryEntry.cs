using Assets.Scripts.Core.Buriko.Util;

namespace Assets.Scripts.Core.Buriko
{
	internal class BurikoMemoryEntry
	{
		public int Scope;

		public IBurikoObject Obj;

		public BurikoMemoryEntry(int scope, IBurikoObject obj)
		{
			Scope = scope;
			Obj = obj;
		}
	}
}
