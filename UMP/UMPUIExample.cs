using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UMP
{
	public class UMPUIExample : MonoBehaviour, IMediaListener, IPlayerOpeningListener, IPlayerBufferingListener, IPlayerImageReadyListener, IPlayerPreparedListener, IPlayerPlayingListener, IPlayerPausedListener, IPlayerStoppedListener, IPlayerEndReachedListener, IPlayerEncounteredErrorListener, IPlayerTimeChangedListener, IPlayerPositionChangedListener
	{
		private const string BUFFERING = "BUFFERING";

		[SerializeField]
		private RawImage _videoImage;

		[SerializeField]
		private InputField _videoPath;

		[SerializeField]
		private Text _playButtonText;

		[SerializeField]
		private Slider _volume;

		[SerializeField]
		private Slider _rate;

		[SerializeField]
		private Text _timeText;

		[SerializeField]
		private Slider _rewind;

		[SerializeField]
		private Text _debug;

		private MediaPlayer _mediaPlayer;

		private long _mediaLength = -1L;

		private IEnumerator _hideDebugEnumerator;

		private void Awake()
		{
			if (_videoImage != null)
			{
				_mediaPlayer = new MediaPlayer(this, new GameObject[1]
				{
					_videoImage.gameObject
				});
				_mediaPlayer.Mute = false;
				_mediaPlayer.Volume = (int)_volume.value;
				AddListeners();
			}
			SetDebugVisibility(visible: false);
		}

		private void SetDebugVisibility(bool visible)
		{
			_debug.transform.parent.gameObject.SetActive(visible);
			_debug.gameObject.SetActive(visible);
			if (_hideDebugEnumerator != null)
			{
				StopCoroutine(_hideDebugEnumerator);
			}
			_hideDebugEnumerator = HideDebugBehaviour();
			StartCoroutine(_hideDebugEnumerator);
		}

		private void SetDebugText(string text)
		{
			SetDebugVisibility(visible: true);
			_debug.text = text;
		}

		private IEnumerator HideDebugBehaviour()
		{
			yield return new WaitForSeconds(2f);
			if (_debug.gameObject.activeSelf)
			{
				SetDebugVisibility(visible: false);
			}
		}

		public void OnPlayClick()
		{
			if (_mediaPlayer == null)
			{
				return;
			}
			if (!_mediaPlayer.AbleToPlay)
			{
				if (!string.IsNullOrEmpty(_videoPath.text))
				{
					_mediaPlayer.DataSource = _videoPath.text;
				}
				if (_mediaPlayer.Play())
				{
					_playButtonText.text = "Pause";
				}
			}
			else if (_mediaPlayer.IsPlaying)
			{
				_mediaPlayer.Pause();
				_playButtonText.text = "Play";
			}
			else if (_mediaPlayer.Play())
			{
				_playButtonText.text = "Pause";
			}
		}

		public void OnVolumeChanged()
		{
			_mediaPlayer.Volume = (int)_volume.value;
			SetDebugText("Volume: " + _volume.value);
		}

		public void OnRateChanged()
		{
			_mediaPlayer.PlaybackRate = _rate.value;
			SetDebugText("Playback rate: " + _rate.value);
		}

		public void OnPositionChanged()
		{
			_mediaPlayer.Position = _rewind.value;
		}

		public void OnStopClick()
		{
			_mediaPlayer.Stop();
			_mediaLength = -1L;
			_videoPath.gameObject.SetActive(value: true);
			_playButtonText.text = "Play";
			SetPlayerTime(0L);
			SetMediaLength(0L);
			_rewind.value = 0f;
			_rewind.enabled = false;
		}

		public void OnSnapshotClick()
		{
			if (_mediaPlayer.AbleToPlay && _mediaPlayer.PlatformPlayer is MediaPlayerStandalone)
			{
				(_mediaPlayer.PlatformPlayer as MediaPlayerStandalone).TakeSnapShot(Application.persistentDataPath);
				SetDebugText("Snapshot path: " + Application.persistentDataPath);
			}
		}

		private void OnDestroy()
		{
			if (_mediaPlayer != null)
			{
				RemoveListeners();
				_mediaPlayer.Release();
			}
		}

		private void AddListeners()
		{
			_mediaPlayer.AddMediaListener(this);
			_mediaPlayer.EventManager.PlayerTimeChangedListener += OnPlayerTimeChanged;
			_mediaPlayer.EventManager.PlayerPositionChangedListener += OnPlayerPositionChanged;
		}

		private void RemoveListeners()
		{
			_mediaPlayer.RemoveMediaListener(this);
			_mediaPlayer.EventManager.PlayerTimeChangedListener -= OnPlayerTimeChanged;
			_mediaPlayer.EventManager.PlayerPositionChangedListener -= OnPlayerPositionChanged;
		}

		private void SetPlayerTime(long playedTime)
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(playedTime);
			string text = _timeText.text;
			int startIndex = text.IndexOf("\n", StringComparison.Ordinal);
			_timeText.text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}" + text.Substring(startIndex);
		}

		private void SetMediaLength(long mediaLength)
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(mediaLength);
			string text = _timeText.text;
			int num = text.IndexOf("\n", StringComparison.Ordinal);
			_timeText.text = text.Substring(0, num + 1) + $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		}

		public void OnPlayerOpening()
		{
			_videoPath.gameObject.SetActive(value: false);
			Debug.Log("Opening");
		}

		public void OnPlayerBuffering(float percentage)
		{
			if (!_debug.text.Contains("BUFFERING"))
			{
				_debug.text = "BUFFERING: " + percentage + "%";
			}
			SetDebugText("BUFFERING: " + percentage + "%");
			Debug.Log("Buffering: (" + percentage + "%)");
		}

		public void OnPlayerImageReady(Texture2D image)
		{
			Debug.Log("ImageReady:( " + image.width + ", " + image.height + ")");
		}

		public void OnPlayerPrepared(int videoWidth, int videoHeight)
		{
			Debug.Log("Prepared:( " + videoWidth + ", " + videoHeight + ")");
		}

		public void OnPlayerPlaying()
		{
			if (_mediaLength < 0)
			{
				_mediaLength = _mediaPlayer.Length;
				SetMediaLength(_mediaLength);
			}
			_rewind.enabled = true;
			Debug.Log("Playing");
		}

		public void OnPlayerPaused()
		{
			Debug.Log("Paused");
		}

		public void OnPlayerStopped()
		{
			Debug.Log("Stopped");
		}

		public void OnPlayerEndReached()
		{
			OnStopClick();
			Debug.Log("OnPlayerEndReached");
		}

		public void OnPlayerEncounteredError()
		{
			OnStopClick();
			if (_mediaPlayer.PlatformPlayer is MediaPlayerStandalone)
			{
				Debug.Log((_mediaPlayer.PlatformPlayer as MediaPlayerStandalone).GetLastError());
			}
		}

		public void OnPlayerTimeChanged(long time)
		{
			SetPlayerTime(time);
		}

		public void OnPlayerPositionChanged(float position)
		{
			_rewind.value = position;
		}
	}
}
