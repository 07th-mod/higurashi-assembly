using System.Text;

namespace UMP
{
	public class MediaTrackInfoAudio : MediaTrackInfoExpanded
	{
		private readonly int _trackChannels;

		private readonly int _trackRate;

		public int Channels => _trackChannels;

		public int Rate => _trackRate;

		internal MediaTrackInfoAudio(int trackId, int trackCodec, int trackProfile, int trackLevel, int trackChannels, int trackRate)
			: base(trackId, trackCodec, trackProfile, trackLevel)
		{
			_trackChannels = trackChannels;
			_trackRate = trackRate;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append(base.ToString()).Append('[');
			stringBuilder.Append("CHANNELS=").Append(_trackChannels).Append(", ");
			stringBuilder.Append("RATE=").Append(_trackRate).Append(']');
			return stringBuilder.ToString();
		}
	}
}
