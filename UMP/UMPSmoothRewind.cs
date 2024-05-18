using UnityEngine;
using UnityEngine.UI;

namespace UMP
{
	public class UMPSmoothRewind : MonoBehaviour
	{
		[SerializeField]
		private UniversalMediaPlayer _mediaPlayer;

		[SerializeField]
		private Slider _rewindSlider;

		private long _framesCounterCahce;

		private void Update()
		{
			if (_mediaPlayer.PlatformPlayer is MediaPlayerStandalone && _mediaPlayer.IsPlaying && _framesCounterCahce != _mediaPlayer.FramesCounter)
			{
				_framesCounterCahce = _mediaPlayer.FramesCounter;
				int framesAmount = (_mediaPlayer.PlatformPlayer as MediaPlayerStandalone).FramesAmount;
				if (framesAmount > 0)
				{
					_rewindSlider.value = (float)_framesCounterCahce / (float)framesAmount;
				}
			}
		}

		public void OnPositionChanged()
		{
			_mediaPlayer.Position = _rewindSlider.value;
		}
	}
}
