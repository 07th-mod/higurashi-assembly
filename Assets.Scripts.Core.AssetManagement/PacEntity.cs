namespace Assets.Scripts.Core.AssetManagement
{
	public class PacEntity
	{
		public string Name;

		public int Offset;

		public int Size;

		public PacEntity(string name, int offset, int size)
		{
			Name = name.Trim().ToLower();
			Offset = offset;
			Size = size;
		}

		public override string ToString()
		{
			return string.Format("(NameLength: {3}, Name: {0} Offset: {1} Size: {2})", Name, Offset, Size, Name.Length);
		}
	}
}
