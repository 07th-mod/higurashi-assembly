using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
	[AddComponentMenu("AVPro Video/Audio Output", 400)]
	[RequireComponent(typeof(AudioSource))]
	public class AudioOutput : MonoBehaviour
	{
		[SerializeField]
		private MediaPlayer _mediaPlayer;

		private AudioSource _audioSource;

		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		private void Start()
		{
			ChangeMediaPlayer(_mediaPlayer);
		}

		private void OnDestroy()
		{
			ChangeMediaPlayer(null);
		}

		private void Update()
		{
			if (_mediaPlayer != null && _mediaPlayer.Control != null && _mediaPlayer.Control.IsPlaying())
			{
				ApplyAudioSettings(_mediaPlayer, _audioSource);
			}
		}

		public void ChangeMediaPlayer(MediaPlayer newPlayer)
		{
			if (_mediaPlayer != null)
			{
				_mediaPlayer.Events.RemoveListener(OnMediaPlayerEvent);
				_mediaPlayer = null;
			}
			_mediaPlayer = newPlayer;
			if (_mediaPlayer != null)
			{
				_mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
			}
		}

		private void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
		{
			switch (et)
			{
			case MediaPlayerEvent.EventType.Closing:
				_audioSource.Stop();
				break;
			case MediaPlayerEvent.EventType.Started:
				ApplyAudioSettings(_mediaPlayer, _audioSource);
				_audioSource.Play();
				break;
			}
		}

		private static void ApplyAudioSettings(MediaPlayer player, AudioSource audioSource)
		{
			if (player != null && player.Control != null)
			{
				float volume = player.Control.GetVolume();
				bool mute = player.Control.IsMuted();
				float playbackRate = player.Control.GetPlaybackRate();
				audioSource.volume = volume;
				audioSource.mute = mute;
				audioSource.pitch = playbackRate;
			}
		}

		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (_mediaPlayer != null && _mediaPlayer.Control != null && _mediaPlayer.Control.IsPlaying())
			{
				_mediaPlayer.Control.GrabAudio(data, data.Length, channels);
			}
		}
	}
}
