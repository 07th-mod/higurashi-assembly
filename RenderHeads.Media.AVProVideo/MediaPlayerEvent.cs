using System;
using UnityEngine.Events;

namespace RenderHeads.Media.AVProVideo
{
	[Serializable]
	public class MediaPlayerEvent : UnityEvent<MediaPlayer, MediaPlayerEvent.EventType, ErrorCode>
	{
		public enum EventType
		{
			MetaDataReady,
			ReadyToPlay,
			Started,
			FirstFrameReady,
			FinishedPlaying,
			Closing,
			Error
		}
	}
}
