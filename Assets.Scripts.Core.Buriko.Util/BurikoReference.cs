namespace Assets.Scripts.Core.Buriko.Util
{
	public class BurikoReference
	{
		public string Property;

		public int Member;

		public BurikoReference Reference;

		public BurikoReference(string property, int member)
		{
			Property = property;
			Member = member;
		}
	}
}
