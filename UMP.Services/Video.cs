namespace UMP.Services
{
	public abstract class Video
	{
		public abstract string Title
		{
			get;
		}

		public abstract string Url
		{
			get;
		}

		public virtual VideoFormat VideoFormat => VideoFormat.Unknown;

		public virtual AudioFormat AudioFormat => AudioFormat.Unknown;

		public string AudioExtension
		{
			get
			{
				switch (AudioFormat)
				{
				case AudioFormat.Mp3:
					return ".mp3";
				case AudioFormat.Aac:
					return ".aac";
				case AudioFormat.Vorbis:
					return ".ogg";
				case AudioFormat.Opus:
					return ".opus";
				default:
					return string.Empty;
				}
			}
		}

		public virtual string VideoExtension
		{
			get
			{
				switch (VideoFormat)
				{
				case VideoFormat.Mp4:
					return ".mp4";
				case VideoFormat.WebM:
					return ".webm";
				case VideoFormat.Mobile:
					return ".3gp";
				case VideoFormat.Flv:
					return ".flv";
				default:
					return string.Empty;
				}
			}
		}

		public override string ToString()
		{
			return $"Title: {Title}, Url: {Url}, VideoFormat: {VideoFormat}, AudioFormat: {AudioFormat}";
		}
	}
}
