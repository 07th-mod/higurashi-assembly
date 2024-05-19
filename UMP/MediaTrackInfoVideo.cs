using System.Text;

namespace UMP
{
	public class MediaTrackInfoVideo : MediaTrackInfoExpanded
	{
		private readonly int _trackWidth;

		private readonly int _trackHeight;

		public int Width => _trackWidth;

		public int Height => _trackHeight;

		internal MediaTrackInfoVideo(int trackId, int trackCodec, int trackProfile, int trackLevel, int trackWidth, int trackHeight)
			: base(trackId, trackCodec, trackProfile, trackLevel)
		{
			_trackWidth = trackWidth;
			_trackHeight = trackHeight;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append(base.ToString()).Append('[');
			stringBuilder.Append("WIDTH=").Append(_trackWidth).Append(", ");
			stringBuilder.Append("HEIGHT=").Append(_trackHeight).Append(']');
			return stringBuilder.ToString();
		}
	}
}
