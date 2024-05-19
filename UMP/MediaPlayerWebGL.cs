using System;
using System.Collections;
using System.IO;
using UMP.Wrappers;
using UnityEngine;

namespace UMP
{
	public class MediaPlayerWebGL : IPlayer
	{
		private delegate void UpdateTexture(int index);

		private MonoBehaviour _monoObject;

		private WrapperInternal _wrapper;

		private float _frameRate;

		private float _tmpTime;

		private int _tmpFramesCounter;

		private bool _isStarted;

		private bool _isPlaying;

		private bool _isLoad;

		private bool _isReady;

		private bool _isImageReady;

		private bool _isTextureExist;

		private string _dataSource;

		private PlayerBufferVideo _videoBuffer;

		private PlayerManagerEvents _eventManager;

		private PlayerOptions _options;

		private Texture2D _videoTexture;

		private GameObject[] _videoOutputObjects;

		private IEnumerator _updateVideoTextureEnum;

		private UpdateTexture _updateTexture;

		private IntPtr _updatePointer;

		public GameObject[] VideoOutputObjects
		{
			get
			{
				return _videoOutputObjects;
			}
			set
			{
				_videoOutputObjects = value;
				MediaPlayerHelper.ApplyTextureToRenderingObjects(_videoTexture, _videoOutputObjects);
			}
		}

		public PlayerManagerEvents EventManager => _eventManager;

		public PlayerOptions Options => _options;

		public PlayerState State => _wrapper.PlayerGetState();

		public object StateValue => _wrapper.PlayerGetStateValue();

		public string DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
				string text = _dataSource;
				if (text.StartsWith("file:///"))
				{
					text = text.Substring("file:///".Length);
					if (!text.Contains(":/"))
					{
						text = Path.Combine(Application.streamingAssetsPath, text);
					}
				}
				_wrapper.PlayerSetDataSource(text);
			}
		}

		public bool IsPlaying => _wrapper.PlayerIsPlaying();

		public bool IsReady => _isReady;

		public bool AbleToPlay => _dataSource != null && !string.IsNullOrEmpty(_dataSource.ToString());

		public long Length => _wrapper.PlayerGetLength();

		public float FrameRate => _frameRate;

		public int FramesCounter => _wrapper.PlayerVideoFramesCounter();

		public byte[] FramePixels => null;

		public long Time
		{
			get
			{
				return _wrapper.PlayerGetTime();
			}
			set
			{
				_wrapper.PlayerSetTime(value);
			}
		}

		public float Position
		{
			get
			{
				return _wrapper.PlayerGetPosition();
			}
			set
			{
				_wrapper.PlayerSetPosition(value);
			}
		}

		public float PlaybackRate
		{
			get
			{
				return _wrapper.PlayerGetRate();
			}
			set
			{
				_wrapper.PlayerSetRate(value);
			}
		}

		public int Volume
		{
			get
			{
				return _wrapper.PlayerGetVolume();
			}
			set
			{
				_wrapper.PlayerSetVolume(value);
			}
		}

		public bool Mute
		{
			get
			{
				return _wrapper.PlayerGetMute();
			}
			set
			{
				_wrapper.PlayerSetMute(value);
			}
		}

		public int VideoWidth
		{
			get
			{
				int num = _wrapper.PlayerVideoWidth();
				if (_videoBuffer != null && (num <= 0 || _options.FixedVideoSize != Vector2.zero))
				{
					num = _videoBuffer.Width;
				}
				return num;
			}
		}

		public int VideoHeight
		{
			get
			{
				int num = _wrapper.PlayerVideoHeight();
				if (_videoBuffer != null && (num <= 0 || _options.FixedVideoSize != Vector2.zero))
				{
					num = _videoBuffer.Height;
				}
				return num;
			}
		}

		public Vector2 VideoSize => new Vector2(VideoWidth, VideoHeight);

		public MediaPlayerWebGL(MonoBehaviour monoObject, GameObject[] videoOutputObjects, PlayerOptions options)
		{
			_monoObject = monoObject;
			_videoOutputObjects = videoOutputObjects;
			_options = options;
			_wrapper = new WrapperInternal(null);
			if (_options != null && _options.FixedVideoSize != Vector2.zero)
			{
				Vector2 fixedVideoSize = _options.FixedVideoSize;
				int width = (int)fixedVideoSize.x;
				Vector2 fixedVideoSize2 = _options.FixedVideoSize;
				_videoBuffer = new PlayerBufferVideo(width, (int)fixedVideoSize2.y);
			}
			_eventManager = new PlayerManagerEvents(_monoObject, this);
			_eventManager.PlayerPlayingListener += OnPlayerPlaying;
			_eventManager.PlayerPausedListener += OnPlayerPaused;
		}

		private void UpdateFpsCounter()
		{
			float time = UnityEngine.Time.time;
			time = ((!(time > _tmpTime)) ? 0f : (time - _tmpTime));
			if (time >= 1f)
			{
				_frameRate = FramesCounter - _tmpFramesCounter;
				_tmpFramesCounter = FramesCounter;
				_tmpTime = UnityEngine.Time.time;
			}
		}

		private IEnumerator UpdateVideoTexture()
		{
			while (true)
			{
				if (FramesCounter > 0)
				{
					UpdateFpsCounter();
					if (!_isTextureExist)
					{
						if (_videoTexture != null)
						{
							UnityEngine.Object.Destroy(_videoTexture);
							_videoTexture = null;
						}
						if (_options.FixedVideoSize == Vector2.zero)
						{
							int width = VideoWidth;
							int height = VideoHeight;
							if (_videoBuffer == null || (_videoBuffer != null && _videoBuffer.Width != width) || _videoBuffer.Height != height)
							{
								if (_videoBuffer != null)
								{
									_videoBuffer.ClearFramePixels();
								}
								_videoBuffer = new PlayerBufferVideo(width, height);
							}
						}
						_videoTexture = MediaPlayerHelper.GenPluginTexture(_videoBuffer.Width, _videoBuffer.Height);
						MediaPlayerHelper.ApplyTextureToRenderingObjects(_videoTexture, _videoOutputObjects);
						_isTextureExist = true;
						_isImageReady = false;
					}
					_wrapper.NativeUpdateTexture(_videoTexture.GetNativeTexturePtr());
					if (!_isImageReady)
					{
						_eventManager.SetEvent(PlayerState.ImageReady, _videoTexture);
						_isImageReady = true;
					}
				}
				if (_wrapper.PlayerIsReady() && !_isReady)
				{
					_isReady = true;
					if (_isLoad)
					{
						_eventManager.ReplaceEvent(PlayerState.Paused, PlayerState.Prepared, new Vector2(VideoWidth, VideoHeight));
						Pause();
					}
					else
					{
						_eventManager.SetEvent(PlayerState.Prepared, new Vector2(VideoWidth, VideoHeight));
						_eventManager.SetEvent(PlayerState.Playing);
					}
				}
				yield return null;
			}
		}

		private void OnPlayerPlaying()
		{
			_isPlaying = true;
		}

		private void OnPlayerPaused()
		{
			_isPlaying = false;
		}

		public void AddMediaListener(IMediaListener listener)
		{
			if (_eventManager != null)
			{
				_eventManager.PlayerOpeningListener += listener.OnPlayerOpening;
				_eventManager.PlayerBufferingListener += listener.OnPlayerBuffering;
				_eventManager.PlayerImageReadyListener += listener.OnPlayerImageReady;
				_eventManager.PlayerPreparedListener += listener.OnPlayerPrepared;
				_eventManager.PlayerPlayingListener += listener.OnPlayerPlaying;
				_eventManager.PlayerPausedListener += listener.OnPlayerPaused;
				_eventManager.PlayerStoppedListener += listener.OnPlayerStopped;
				_eventManager.PlayerEndReachedListener += listener.OnPlayerEndReached;
				_eventManager.PlayerEncounteredErrorListener += listener.OnPlayerEncounteredError;
			}
		}

		public void RemoveMediaListener(IMediaListener listener)
		{
			if (_eventManager != null)
			{
				_eventManager.PlayerOpeningListener -= listener.OnPlayerOpening;
				_eventManager.PlayerBufferingListener -= listener.OnPlayerBuffering;
				_eventManager.PlayerImageReadyListener -= listener.OnPlayerImageReady;
				_eventManager.PlayerPreparedListener -= listener.OnPlayerPrepared;
				_eventManager.PlayerPlayingListener -= listener.OnPlayerPlaying;
				_eventManager.PlayerPausedListener -= listener.OnPlayerPaused;
				_eventManager.PlayerStoppedListener -= listener.OnPlayerStopped;
				_eventManager.PlayerEndReachedListener -= listener.OnPlayerEndReached;
				_eventManager.PlayerEncounteredErrorListener -= listener.OnPlayerEncounteredError;
			}
		}

		public void Prepare()
		{
			_isLoad = true;
			Play();
		}

		public bool Play()
		{
			if (!_isStarted)
			{
				if (_eventManager != null)
				{
					_eventManager.StartListener();
				}
				_eventManager.SetEvent(PlayerState.Opening);
			}
			if (_updateVideoTextureEnum == null)
			{
				_updateVideoTextureEnum = UpdateVideoTexture();
				_monoObject.StartCoroutine(_updateVideoTextureEnum);
			}
			_isStarted = _wrapper.PlayerPlay();
			if (_isStarted)
			{
				if (_isReady && !_isPlaying)
				{
					_eventManager.SetEvent(PlayerState.Playing);
				}
			}
			else
			{
				Stop();
			}
			return _isStarted;
		}

		public void Pause()
		{
			if (_videoOutputObjects == null && _updateVideoTextureEnum != null)
			{
				_monoObject.StopCoroutine(_updateVideoTextureEnum);
				_updateVideoTextureEnum = null;
			}
			_wrapper.PlayerPause();
		}

		public void Stop(bool resetTexture)
		{
			if (_isStarted)
			{
				_wrapper.PlayerStop();
				if (_updateVideoTextureEnum != null)
				{
					_monoObject.StopCoroutine(_updateVideoTextureEnum);
					_updateVideoTextureEnum = null;
				}
				_frameRate = 0f;
				_tmpFramesCounter = 0;
				_tmpTime = 0f;
				_isStarted = false;
				_isPlaying = false;
				_isLoad = false;
				_isReady = false;
				_isImageReady = false;
				_isTextureExist = !resetTexture;
				if (resetTexture && _videoTexture != null)
				{
					UnityEngine.Object.Destroy(_videoTexture);
					_videoTexture = null;
				}
				if (_eventManager != null)
				{
					_eventManager.StopListener();
				}
			}
		}

		public void Stop()
		{
			Stop(resetTexture: true);
		}

		public void Release()
		{
			Stop();
			if (_eventManager != null)
			{
				_eventManager.RemoveAllEvents();
				_eventManager = null;
			}
			_wrapper.PlayerRelease();
		}

		public string GetFormattedLength(bool detail)
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(Length);
			string format = (!detail) ? "{0:D2}:{1:D2}:{2:D2}" : "{0:D2}:{1:D2}:{2:D2}:{3:D3}";
			return string.Format(format, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		}
	}
}
