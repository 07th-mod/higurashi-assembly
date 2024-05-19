using System;

namespace UMP
{
	internal interface IPlayerSpu
	{
		MediaTrackInfo[] SpuTracks
		{
			get;
		}

		MediaTrackInfo SpuTrack
		{
			get;
			set;
		}

		bool SetSubtitleFile(Uri path);
	}
}
