namespace UMP
{
	internal interface IPlayerAudio
	{
		MediaTrackInfo[] AudioTracks
		{
			get;
		}

		MediaTrackInfo AudioTrack
		{
			get;
			set;
		}
	}
}
