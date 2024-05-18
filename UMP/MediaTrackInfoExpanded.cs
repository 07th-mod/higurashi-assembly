using System.Text;

namespace UMP
{
	public abstract class MediaTrackInfoExpanded : MediaTrackInfo
	{
		private readonly int _trackCodec;

		private readonly int _trackProfile;

		private readonly int _trackLevel;

		public int Codec => _trackCodec;

		public int Profile => _trackProfile;

		public int Level => _trackLevel;

		internal MediaTrackInfoExpanded(int trackId, int trackCodec, int trackProfile, int trackLevel)
			: base(trackId, (trackCodec == 0) ? null : Encoding.ASCII.GetString(new byte[4]
			{
				(byte)trackCodec,
				(byte)(trackCodec >> 8),
				(byte)(trackCodec >> 16),
				(byte)(trackCodec >> 24)
			}).Trim())
		{
			_trackCodec = trackCodec;
			_trackProfile = trackProfile;
			_trackLevel = trackLevel;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("TrackInfoExpanded").Append('[');
			stringBuilder.Append("ID=").Append(base.Id).Append(", ");
			stringBuilder.Append("NAME=").Append(base.Name).Append(", ");
			stringBuilder.Append("CODEC=").Append(Codec).Append(", ");
			stringBuilder.Append("PROFILE=").Append(Profile).Append(", ");
			stringBuilder.Append("LEVEL=").Append(Level).Append(']');
			return stringBuilder.ToString();
		}
	}
}
