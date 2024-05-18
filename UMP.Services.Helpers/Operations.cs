namespace UMP.Services.Helpers
{
	internal struct Operations
	{
		private string _reverse;

		private string _swap;

		private string _splice;

		public string Reverse => _reverse;

		public string Swap => _swap;

		public string Splice => _splice;

		public Operations(string reverse, string swap, string splice)
		{
			_reverse = reverse;
			_swap = swap;
			_splice = splice;
		}
	}
}
