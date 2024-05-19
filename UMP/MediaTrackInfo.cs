using System.Text;

namespace UMP
{
	public class MediaTrackInfo
	{
		private readonly int _trackId;

		private readonly string _trackName;

		public int Id => _trackId;

		public string Name => _trackName;

		internal MediaTrackInfo(int trackId, string trackName)
		{
			_trackId = trackId;
			_trackName = trackName;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("TrackInfo").Append('[');
			stringBuilder.Append("ID=").Append(_trackId).Append(',');
			stringBuilder.Append("NAME=").Append(_trackName).Append(']');
			return stringBuilder.ToString();
		}
	}
}
