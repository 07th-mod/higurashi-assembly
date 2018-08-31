namespace Assets.Scripts.Core.Buriko
{
	public class BurikoStackEntry
	{
		public BurikoScriptFile Script;

		public int Position;

		public int LineNum;

		public BurikoStackEntry(BurikoScriptFile script, int position, int linenum)
		{
			Script = script;
			Position = position;
			LineNum = linenum;
		}

		public void Restore()
		{
			Script.JumpToLineNum(LineNum);
		}
	}
}
