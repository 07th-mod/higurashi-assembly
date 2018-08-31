using System.Collections.Generic;

namespace Assets.Scripts.UI.Tips
{
	public class TipsDataGroup
	{
		public int TipsUnlocked;

		public int TipsAvailable;

		public List<TipsDataEntry> Tips = new List<TipsDataEntry>();
	}
}
