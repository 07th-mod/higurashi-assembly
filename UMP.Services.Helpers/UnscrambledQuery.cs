namespace UMP.Services.Helpers
{
	internal struct UnscrambledQuery
	{
		private string _uri;

		private bool _encrypted;

		public string Uri => _uri;

		public bool IsEncrypted => _encrypted;

		public UnscrambledQuery(string uri, bool encrypted)
		{
			_uri = uri;
			_encrypted = encrypted;
		}
	}
}
