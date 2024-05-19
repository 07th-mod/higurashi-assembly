using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UMP.Services;
using UMP.Services.Youtube;
using UnityEngine;
using UnityEngine.Events;

namespace UMP
{
	public class UniversalMediaPlayer : MonoBehaviour, IMediaListener, IPathPreparedListener, IPlayerOpeningListener, IPlayerBufferingListener, IPlayerImageReadyListener, IPlayerPreparedListener, IPlayerPlayingListener, IPlayerPausedListener, IPlayerStoppedListener, IPlayerEndReachedListener, IPlayerEncounteredErrorListener, IPlayerTimeChangedListener, IPlayerPositionChangedListener, IPlayerSnapshotTakenListener
	{
		[Serializable]
		private class EventTextType : UnityEvent<string>
		{
		}

		[Serializable]
		private class EventFloatType : UnityEvent<float>
		{
		}

		[Serializable]
		private class EventLongType : UnityEvent<long>
		{
		}

		[Serializable]
		private class EventSizeType : UnityEvent<int, int>
		{
		}

		[Serializable]
		private class EventTextureType : UnityEvent<Texture2D>
		{
		}

		private const float DEFAULT_POSITION_CHANGED_OFFSET = 0.05f;

		[SerializeField]
		private GameObject[] _renderingObjects;

		[SerializeField]
		private string _path = string.Empty;

		[SerializeField]
		private bool _autoPlay;

		[SerializeField]
		private bool _loop;

		[SerializeField]
		private bool _loopSmooth;

		[SerializeField]
		private bool _mute;

		[SerializeField]
		private bool _useAdvanced;

		[SerializeField]
		private bool _useFixedSize;

		[SerializeField]
		private int _fixedVideoWidth = -1;

		[SerializeField]
		private int _fixedVideoHeight = -1;

		[SerializeField]
		private int _chosenPlatform;

		[SerializeField]
		private int _volume = 50;

		[SerializeField]
		private float _playRate = 1f;

		[SerializeField]
		private float _position;

		[SerializeField]
		private LogLevels _logDetail;

		[SerializeField]
		private string _lastEventMsg = string.Empty;

		[SerializeField]
		private AudioOutput[] _desktopAudioOutputs;

		[SerializeField]
		private PlayerOptions.States _desktopHardwareDecoding = PlayerOptions.States.Default;

		[SerializeField]
		private bool _desktopFlipVertically = true;

		[SerializeField]
		private bool _desktopVideoBufferSize;

		[SerializeField]
		private bool _desktopOutputToFile;

		[SerializeField]
		private bool _desktopDisplayOutput;

		[SerializeField]
		private string _desktopOutputFilePath = string.Empty;

		[SerializeField]
		private bool _desktopRtspOverTcp;

		[SerializeField]
		private int _desktopFileCaching = 300;

		[SerializeField]
		private int _desktopLiveCaching = 300;

		[SerializeField]
		private int _desktopDiskCaching = 300;

		[SerializeField]
		private int _desktopNetworkCaching = 300;

		[SerializeField]
		private PlayerOptionsAndroid.PlayerTypes _androidPlayerType = PlayerOptionsAndroid.PlayerTypes.LibVLC;

		[SerializeField]
		private PlayerOptionsAndroid.DecodingStates _androidHardwareAcceleration = PlayerOptionsAndroid.DecodingStates.Automatic;

		[SerializeField]
		private PlayerOptions.States _androidOpenGLDecoding;

		[SerializeField]
		private PlayerOptionsAndroid.ChromaTypes _androidVideoChroma = PlayerOptionsAndroid.ChromaTypes.RGB16Bit;

		[SerializeField]
		private bool _androidPlayInBackground;

		[SerializeField]
		private bool _androidRtspOverTcp;

		[SerializeField]
		private int _androidNetworkCaching = 300;

		[SerializeField]
		private PlayerOptionsIPhone.PlayerTypes _iphonePlayerType = PlayerOptionsIPhone.PlayerTypes.FFmpeg;

		[SerializeField]
		private bool _iphoneFlipVertically = true;

		[SerializeField]
		private bool _iphoneVideoToolbox = true;

		[SerializeField]
		private int _iphoneVideoToolboxMaxFrameWidth = 4096;

		[SerializeField]
		private bool _iphoneVideoToolboxAsync;

		[SerializeField]
		private bool _iphoneVideoToolboxWaitAsync = true;

		[SerializeField]
		private bool _iphonePlayInBackground;

		[SerializeField]
		private bool _iphoneRtspOverTcp;

		[SerializeField]
		private bool _iphonePacketBuffering = true;

		[SerializeField]
		private int _iphoneMaxBufferSize = 15728640;

		[SerializeField]
		private int _iphoneMinFrames = 50000;

		[SerializeField]
		private bool _iphoneInfbuf;

		[SerializeField]
		private int _iphoneFramedrop;

		[SerializeField]
		private int _iphoneMaxFps = 31;

		[SerializeField]
		private EventTextType _pathPreparedEvent = new EventTextType();

		[SerializeField]
		private UnityEvent _openingEvent = new UnityEvent();

		[SerializeField]
		private EventFloatType _bufferingEvent = new EventFloatType();

		[SerializeField]
		private EventTextureType _imageReadyEvent = new EventTextureType();

		[SerializeField]
		private EventSizeType _preparedEvent = new EventSizeType();

		[SerializeField]
		private UnityEvent _playingEvent = new UnityEvent();

		[SerializeField]
		private UnityEvent _pausedEvent = new UnityEvent();

		[SerializeField]
		private UnityEvent _stoppedEvent = new UnityEvent();

		[SerializeField]
		private UnityEvent _endReachedEvent = new UnityEvent();

		[SerializeField]
		private UnityEvent _encounteredErrorEvent = new UnityEvent();

		[SerializeField]
		private EventLongType _timeChangedEvent = new EventLongType();

		[SerializeField]
		private EventFloatType _positionChangedEvent = new EventFloatType();

		[SerializeField]
		private EventTextType _snapshotTakenEvent = new EventTextType();

		private MediaPlayer _mediaPlayer;

		private MediaPlayer _mediaPlayerLoop;

		private VideoServices _videoServices;

		private string _tmpPath = string.Empty;

		private bool _isParsing;

		private static bool _isExportCompleted;

		private static Dictionary<string, string> _cachedVideoPaths = new Dictionary<string, string>();

		private static IEnumerator _exportedHandlerEnum;

		private IEnumerator _videoPathPreparingEnum;

		private bool _isFirstEditorStateChange = true;

		public GameObject[] RenderingObjects
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.VideoOutputObjects;
				}
				return null;
			}
			set
			{
				if (_mediaPlayer != null)
				{
					_mediaPlayer.VideoOutputObjects = value;
				}
				_renderingObjects = value;
			}
		}

		public AudioOutput[] AudioOutputs => _desktopAudioOutputs;

		public object PlatformPlayer => (_mediaPlayer == null) ? null : _mediaPlayer.PlatformPlayer;

		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}

		public bool AutoPlay
		{
			get
			{
				return _autoPlay;
			}
			set
			{
				_autoPlay = value;
			}
		}

		public bool Loop
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;
			}
		}

		public bool Mute
		{
			get
			{
				return _mediaPlayer.Mute;
			}
			set
			{
				_mediaPlayer.Mute = value;
				_mute = value;
			}
		}

		public float Volume
		{
			get
			{
				return _mediaPlayer.Volume;
			}
			set
			{
				_mediaPlayer.Volume = (int)value;
				_volume = (int)value;
			}
		}

		public float Position
		{
			get
			{
				return _mediaPlayer.Position;
			}
			set
			{
				_mediaPlayer.Position = value;
			}
		}

		public long Time
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.Time;
				}
				return -1L;
			}
			set
			{
				if (_mediaPlayer != null)
				{
					_mediaPlayer.Time = value;
				}
			}
		}

		public float PlayRate
		{
			get
			{
				return _mediaPlayer.PlaybackRate;
			}
			set
			{
				_mediaPlayer.PlaybackRate = value;
				_playRate = value;
			}
		}

		public bool AbleToPlay
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.AbleToPlay;
				}
				return false;
			}
		}

		public bool IsPlaying
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.IsPlaying;
				}
				return false;
			}
		}

		public bool IsReady
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.IsReady;
				}
				return false;
			}
		}

		public bool IsParsing => _isParsing;

		public float FrameRate
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.FrameRate;
				}
				return 0f;
			}
		}

		public long FramesCounter
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.FramesCounter;
				}
				return 0L;
			}
		}

		public byte[] FramePixels
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.FramePixels;
				}
				return null;
			}
		}

		public long Length
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.Length;
				}
				return 0L;
			}
		}

		public int VideoWidth
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.VideoWidth;
				}
				return 0;
			}
		}

		public int VideoHeight
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.VideoHeight;
				}
				return 0;
			}
		}

		public Vector2 VideoSize
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.VideoSize;
				}
				return new Vector2(0f, 0f);
			}
		}

		public MediaTrackInfo AudioTrack
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.AudioTrack;
				}
				return null;
			}
			set
			{
				if (_mediaPlayer != null)
				{
					_mediaPlayer.AudioTrack = value;
				}
			}
		}

		public MediaTrackInfo[] AudioTracks
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.AudioTracks;
				}
				return null;
			}
		}

		public MediaTrackInfo SpuTrack
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.SpuTrack;
				}
				return null;
			}
			set
			{
				if (_mediaPlayer != null)
				{
					_mediaPlayer.SpuTrack = value;
				}
			}
		}

		public MediaTrackInfo[] SpuTracks
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.SpuTracks;
				}
				return null;
			}
		}

		public PlayerManagerEvents EventManager
		{
			get
			{
				if (_mediaPlayer != null)
				{
					return _mediaPlayer.EventManager;
				}
				return null;
			}
		}

		private void Awake()
		{
			if (UMPSettings.Instance.UseAudioSource && (_desktopAudioOutputs == null || _desktopAudioOutputs.Length <= 0))
			{
				UMPAudioOutput uMPAudioOutput = base.gameObject.AddComponent<UMPAudioOutput>();
				_desktopAudioOutputs = new UMPAudioOutput[1]
				{
					uMPAudioOutput
				};
			}
			PlayerOptions options = new PlayerOptions(null);
			string str = Application.dataPath + "/StreamingAssets/mv01.ass";
			string text = Application.dataPath + "/StreamingAssets/mv01.ttf";
			Debug.Log(text);
			switch (UMPSettings.RuntimePlatform)
			{
			case UMPSettings.Platforms.Win:
			{
				PlayerOptionsStandalone playerOptionsStandalone = new PlayerOptionsStandalone(new string[2]
				{
					"--sub-file=" + str,
					"--freetype-font=" + text
				});
				playerOptionsStandalone.FixedVideoSize = ((!_useFixedSize) ? Vector2.zero : new Vector2(_fixedVideoWidth, _fixedVideoHeight));
				playerOptionsStandalone.AudioOutputs = _desktopAudioOutputs;
				playerOptionsStandalone.HardwareDecoding = _desktopHardwareDecoding;
				playerOptionsStandalone.FlipVertically = _desktopFlipVertically;
				playerOptionsStandalone.VideoBufferSize = _desktopVideoBufferSize;
				playerOptionsStandalone.UseTCP = _desktopRtspOverTcp;
				playerOptionsStandalone.FileCaching = _desktopFileCaching;
				playerOptionsStandalone.LiveCaching = _desktopLiveCaching;
				playerOptionsStandalone.DiskCaching = _desktopDiskCaching;
				playerOptionsStandalone.NetworkCaching = _desktopNetworkCaching;
				PlayerOptionsStandalone playerOptionsStandalone2 = playerOptionsStandalone;
				if (_desktopOutputToFile)
				{
					playerOptionsStandalone2.RedirectToFile(_desktopDisplayOutput, _desktopOutputFilePath);
				}
				playerOptionsStandalone2.SetLogDetail(_logDetail, UnityConsoleLogging);
				options = playerOptionsStandalone2;
				break;
			}
			case UMPSettings.Platforms.Android:
			{
				PlayerOptionsAndroid playerOptionsAndroid = new PlayerOptionsAndroid(null);
				playerOptionsAndroid.FixedVideoSize = ((!_useFixedSize) ? Vector2.zero : new Vector2(_fixedVideoWidth, _fixedVideoHeight));
				playerOptionsAndroid.PlayerType = _androidPlayerType;
				playerOptionsAndroid.HardwareAcceleration = _androidHardwareAcceleration;
				playerOptionsAndroid.OpenGLDecoding = _androidOpenGLDecoding;
				playerOptionsAndroid.VideoChroma = _androidVideoChroma;
				playerOptionsAndroid.PlayInBackground = _androidPlayInBackground;
				playerOptionsAndroid.UseTCP = _androidRtspOverTcp;
				playerOptionsAndroid.NetworkCaching = _androidNetworkCaching;
				PlayerOptionsAndroid playerOptionsAndroid2 = playerOptionsAndroid;
				options = playerOptionsAndroid2;
				if (_exportedHandlerEnum == null)
				{
					_exportedHandlerEnum = AndroidExpoterdHandler();
					StartCoroutine(_exportedHandlerEnum);
				}
				break;
			}
			case UMPSettings.Platforms.iOS:
			{
				PlayerOptionsIPhone playerOptionsIPhone = new PlayerOptionsIPhone(null);
				playerOptionsIPhone.FixedVideoSize = ((!_useFixedSize) ? Vector2.zero : new Vector2(_fixedVideoWidth, _fixedVideoHeight));
				playerOptionsIPhone.PlayerType = _iphonePlayerType;
				playerOptionsIPhone.FlipVertically = _iphoneFlipVertically;
				playerOptionsIPhone.VideoToolbox = _iphoneVideoToolbox;
				playerOptionsIPhone.VideoToolboxFrameWidth = _iphoneVideoToolboxMaxFrameWidth;
				playerOptionsIPhone.VideoToolboxAsync = _iphoneVideoToolboxAsync;
				playerOptionsIPhone.VideoToolboxWaitAsync = _iphoneVideoToolboxWaitAsync;
				playerOptionsIPhone.PlayInBackground = _iphonePlayInBackground;
				playerOptionsIPhone.UseTCP = _iphoneRtspOverTcp;
				playerOptionsIPhone.PacketBuffering = _iphonePacketBuffering;
				playerOptionsIPhone.MaxBufferSize = _iphoneMaxBufferSize;
				playerOptionsIPhone.MinFrames = _iphoneMinFrames;
				playerOptionsIPhone.Infbuf = _iphoneInfbuf;
				playerOptionsIPhone.Framedrop = _iphoneFramedrop;
				playerOptionsIPhone.MaxFps = _iphoneMaxFps;
				PlayerOptionsIPhone playerOptionsIPhone2 = playerOptionsIPhone;
				options = playerOptionsIPhone2;
				break;
			}
			}
			_mediaPlayer = new MediaPlayer(this, _renderingObjects, options);
			_videoServices = new VideoServices(this);
			AddListeners();
			if (_loopSmooth)
			{
				_mediaPlayerLoop = new MediaPlayer(this, _mediaPlayer);
				_mediaPlayerLoop.VideoOutputObjects = null;
				_mediaPlayerLoop.EventManager.RemoveAllEvents();
			}
		}

		private IEnumerator AndroidExpoterdHandler()
		{
			UMPSettings settings = UMPSettings.Instance;
			string[] androidExportedPaths = settings.AndroidExportedPaths;
			foreach (string exportedPath in androidExportedPaths)
			{
				string tempFilePath = System.IO.Path.Combine(Application.temporaryCachePath, exportedPath);
				string saPath = "Assets" + System.IO.Path.AltDirectorySeparatorChar + "StreamingAssets" + System.IO.Path.AltDirectorySeparatorChar;
				string localFilePath = exportedPath.Replace(saPath, string.Empty);
				if (File.Exists(tempFilePath))
				{
					_cachedVideoPaths.Add(localFilePath, tempFilePath);
					continue;
				}
				byte[] data2 = new byte[0];
				WWW www = new WWW(System.IO.Path.Combine(Application.streamingAssetsPath, localFilePath));
				yield return www;
				data2 = www.bytes;
				if (string.IsNullOrEmpty(www.error))
				{
					FileInfo tempFile = new FileInfo(tempFilePath);
					tempFile.Directory.Create();
					File.WriteAllBytes(tempFile.FullName, data2);
					_cachedVideoPaths.Add(localFilePath, tempFilePath);
				}
				else
				{
					Debug.LogError("Can't create temp file from asset folder: " + www.error);
				}
				www.Dispose();
			}
			_isExportCompleted = true;
		}

		private void Start()
		{
			if (_autoPlay)
			{
				Play();
			}
		}

		private void OnDisable()
		{
			if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
			{
				Stop();
			}
		}

		private void OnDestroy()
		{
			if (_mediaPlayer != null)
			{
				Release();
			}
		}

		private void AddListeners()
		{
			if (_mediaPlayer != null && _mediaPlayer.EventManager != null)
			{
				_mediaPlayer.AddMediaListener(this);
				_mediaPlayer.EventManager.PlayerTimeChangedListener += OnPlayerTimeChanged;
				_mediaPlayer.EventManager.PlayerPositionChangedListener += OnPlayerPositionChanged;
				_mediaPlayer.EventManager.PlayerSnapshotTakenListener += OnPlayerSnapshotTaken;
			}
		}

		private void RemoveListeners()
		{
			if (_mediaPlayer != null)
			{
				_mediaPlayer.RemoveMediaListener(this);
				_mediaPlayer.EventManager.PlayerTimeChangedListener -= OnPlayerTimeChanged;
				_mediaPlayer.EventManager.PlayerPositionChangedListener -= OnPlayerPositionChanged;
				_mediaPlayer.EventManager.PlayerSnapshotTakenListener -= OnPlayerSnapshotTaken;
			}
		}

		private IEnumerator VideoPathPreparing(string path, bool playImmediately, IPathPreparedListener listener)
		{
			if (_cachedVideoPaths.ContainsKey(path))
			{
				listener.OnPathPrepared(_cachedVideoPaths[path], playImmediately);
				yield break;
			}
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Android)
			{
				string[] exptPaths = UMPSettings.Instance.AndroidExportedPaths;
				string filePath = path.Replace("file:///", string.Empty);
				string[] array = exptPaths;
				foreach (string exptPath in array)
				{
					if (exptPath.Contains(filePath))
					{
						while (!_isExportCompleted)
						{
							yield return null;
						}
						if (_cachedVideoPaths.ContainsKey(filePath))
						{
							listener.OnPathPrepared(_cachedVideoPaths[filePath], playImmediately);
							yield break;
						}
						break;
					}
				}
				if ((_mediaPlayer.Options as PlayerOptionsAndroid).PlayerType == PlayerOptionsAndroid.PlayerTypes.LibVLC && MediaPlayerHelper.IsAssetsFile(path))
				{
					string tempFilePath = System.IO.Path.Combine(Application.temporaryCachePath, filePath);
					if (File.Exists(tempFilePath))
					{
						_cachedVideoPaths.Add(path, tempFilePath);
						listener.OnPathPrepared(tempFilePath, playImmediately);
						yield break;
					}
					byte[] data2 = new byte[0];
					WWW www = new WWW(System.IO.Path.Combine(Application.streamingAssetsPath, filePath));
					yield return www;
					data2 = www.bytes;
					if (string.IsNullOrEmpty(www.error))
					{
						FileInfo tempFile = new FileInfo(tempFilePath);
						tempFile.Directory.Create();
						File.WriteAllBytes(tempFile.FullName, data2);
						_cachedVideoPaths.Add(path, tempFilePath);
						path = tempFilePath;
					}
					else
					{
						Debug.LogError("Can't create temp file from asset folder: " + www.error);
					}
					www.Dispose();
				}
			}
			if (_videoServices.ValidUrl(path))
			{
				Video serviceVideo = null;
				_isParsing = true;
				yield return _videoServices.GetVideos(path, delegate(List<Video> videos)
				{
					_isParsing = false;
					serviceVideo = VideoServices.FindVideo(videos, int.MaxValue, int.MaxValue);
				}, delegate(string error)
				{
					_isParsing = false;
					Debug.LogError($"[UniversalMediaPlayer.GetVideos] {error}");
					OnPlayerEncounteredError();
				});
				if (serviceVideo == null)
				{
					Debug.LogError("[UniversalMediaPlayer.VideoPathPreparing] Can't get service video information");
					OnPlayerEncounteredError();
				}
				else
				{
					if (serviceVideo is YoutubeVideo)
					{
						yield return (serviceVideo as YoutubeVideo).Decrypt(UMPSettings.Instance.YoutubeDecryptFunction, delegate(string error)
						{
							Debug.LogError($"[UniversalMediaPlayer.Decrypt] {error}");
							OnPlayerEncounteredError();
						});
					}
					path = serviceVideo.Url;
				}
			}
			listener.OnPathPrepared(path, playImmediately);
			yield return null;
		}

		public void Prepare()
		{
			if (_videoPathPreparingEnum != null)
			{
				StopCoroutine(_videoPathPreparingEnum);
			}
			_videoPathPreparingEnum = VideoPathPreparing(_path, playImmediately: false, listener: this);
			StartCoroutine(_videoPathPreparingEnum);
		}

		public void Play()
		{
			if (_videoPathPreparingEnum != null)
			{
				StopCoroutine(_videoPathPreparingEnum);
			}
			_videoPathPreparingEnum = VideoPathPreparing(_path, playImmediately: true, listener: this);
			StartCoroutine(_videoPathPreparingEnum);
		}

		public void Pause()
		{
			if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
			{
				_mediaPlayer.Pause();
			}
		}

		public void Stop()
		{
			Stop(clearVideoTexture: true);
		}

		public void Stop(bool clearVideoTexture)
		{
			if (_videoPathPreparingEnum != null)
			{
				StopCoroutine(_videoPathPreparingEnum);
			}
			_position = 0f;
			if (_mediaPlayer != null)
			{
				_mediaPlayer.Stop(clearVideoTexture);
				if (_mediaPlayerLoop != null)
				{
					_mediaPlayerLoop.Stop(clearVideoTexture);
				}
			}
		}

		public void Release()
		{
			Stop();
			if (_mediaPlayer != null)
			{
				_mediaPlayer.Release();
				_mediaPlayer = null;
				if (_mediaPlayerLoop != null)
				{
					_mediaPlayerLoop.Release();
				}
				RemoveListeners();
				_openingEvent.RemoveAllListeners();
				_bufferingEvent.RemoveAllListeners();
				_imageReadyEvent.RemoveAllListeners();
				_preparedEvent.RemoveAllListeners();
				_playingEvent.RemoveAllListeners();
				_pausedEvent.RemoveAllListeners();
				_stoppedEvent.RemoveAllListeners();
				_endReachedEvent.RemoveAllListeners();
				_encounteredErrorEvent.RemoveAllListeners();
				_timeChangedEvent.RemoveAllListeners();
				_positionChangedEvent.RemoveAllListeners();
				_snapshotTakenEvent.RemoveAllListeners();
			}
		}

		public string GetFormattedLength(bool detail)
		{
			if (_mediaPlayer != null)
			{
				return _mediaPlayer.GetFormattedLength(detail);
			}
			return string.Empty;
		}

		public void Snapshot(string path)
		{
			if (_mediaPlayer != null && _mediaPlayer.AbleToPlay && _mediaPlayer.PlatformPlayer is MediaPlayerStandalone)
			{
				(_mediaPlayer.PlatformPlayer as MediaPlayerStandalone).TakeSnapShot(path);
			}
		}

		private void UnityConsoleLogging(PlayerManagerLogs.PlayerLog args)
		{
			if (args.Level == _logDetail)
			{
				Debug.Log(args.Level.ToString() + ": " + args.Message);
			}
		}

		public void OnPathPrepared(string path, bool playImmediately)
		{
			_mediaPlayer.Mute = _mute;
			_mediaPlayer.Volume = _volume;
			_mediaPlayer.PlaybackRate = _playRate;
			if (!_path.Equals(_tmpPath))
			{
				if (IsPlaying)
				{
					Stop();
				}
				_tmpPath = _path;
				_mediaPlayer.DataSource = path;
			}
			if (!playImmediately)
			{
				_mediaPlayer.Prepare();
			}
			else
			{
				_mediaPlayer.Play();
			}
			if (_mediaPlayerLoop != null && !_mediaPlayerLoop.IsReady)
			{
				_mediaPlayerLoop.DataSource = _mediaPlayer.DataSource;
				_mediaPlayerLoop.Prepare();
			}
			if (_pathPreparedEvent != null)
			{
				_pathPreparedEvent.Invoke(path);
			}
		}

		public void AddPathPreparedEvent(UnityAction<string> action)
		{
			_pathPreparedEvent.AddListener(action);
		}

		public void RemovePathPreparedEvent(UnityAction<string> action)
		{
			_pathPreparedEvent.RemoveListener(action);
		}

		public void OnPlayerOpening()
		{
			if (_openingEvent != null)
			{
				_openingEvent.Invoke();
			}
		}

		public void AddOpeningEvent(UnityAction action)
		{
			_openingEvent.AddListener(action);
		}

		public void RemoveOpeningEvent(UnityAction action)
		{
			_openingEvent.RemoveListener(action);
		}

		public void OnPlayerBuffering(float percentage)
		{
			if (_bufferingEvent != null)
			{
				_bufferingEvent.Invoke(percentage);
			}
		}

		public void AddBufferingEvent(UnityAction<float> action)
		{
			_bufferingEvent.AddListener(action);
		}

		public void RemoveBufferingEvent(UnityAction<float> action)
		{
			_bufferingEvent.RemoveListener(action);
		}

		public void OnPlayerImageReady(Texture2D image)
		{
			if (_imageReadyEvent != null)
			{
				_imageReadyEvent.Invoke(image);
			}
		}

		public void AddImageReadyEvent(UnityAction<Texture2D> action)
		{
			_imageReadyEvent.AddListener(action);
		}

		public void RemoveImageReadyEvent(UnityAction<Texture2D> action)
		{
			_imageReadyEvent.RemoveListener(action);
		}

		public void OnPlayerPrepared(int videoWidth, int videoHeight)
		{
			_mediaPlayer.Mute = _mute;
			_mediaPlayer.Volume = _volume;
			_mediaPlayer.PlaybackRate = _playRate;
			if (_preparedEvent != null)
			{
				_preparedEvent.Invoke(videoWidth, videoHeight);
			}
		}

		public void AddPreparedEvent(UnityAction<int, int> action)
		{
			_preparedEvent.AddListener(action);
		}

		public void RemovePreparedEvent(UnityAction<int, int> action)
		{
			_preparedEvent.RemoveListener(action);
		}

		public void OnPlayerPlaying()
		{
			if (_playingEvent != null)
			{
				_playingEvent.Invoke();
			}
		}

		public void AddPlayingEvent(UnityAction action)
		{
			_playingEvent.AddListener(action);
		}

		public void RemovePlayingEvent(UnityAction action)
		{
			_playingEvent.RemoveListener(action);
		}

		public void OnPlayerPaused()
		{
			if (_pausedEvent != null)
			{
				_pausedEvent.Invoke();
			}
		}

		public void AddPausedEvent(UnityAction action)
		{
			_pausedEvent.AddListener(action);
		}

		public void RemovePausedEvent(UnityAction action)
		{
			_pausedEvent.RemoveListener(action);
		}

		public void OnPlayerStopped()
		{
			if (_stoppedEvent != null)
			{
				_stoppedEvent.Invoke();
			}
		}

		public void AddStoppedEvent(UnityAction action)
		{
			_stoppedEvent.AddListener(action);
		}

		public void RemoveStoppedEvent(UnityAction action)
		{
			_stoppedEvent.RemoveListener(action);
		}

		public void OnPlayerEndReached()
		{
			_position = 0f;
			_mediaPlayer.Stop(!_loop);
			if (_loop)
			{
				if (_mediaPlayerLoop != null)
				{
					_mediaPlayerLoop.EventManager.CopyPlayerEvents(_mediaPlayer.EventManager);
					_mediaPlayerLoop.VideoOutputObjects = _mediaPlayer.VideoOutputObjects;
					_mediaPlayer.VideoOutputObjects = null;
					_mediaPlayer.EventManager.RemoveAllEvents();
					MediaPlayer mediaPlayer = _mediaPlayer;
					_mediaPlayer = _mediaPlayerLoop;
					_mediaPlayerLoop = mediaPlayer;
				}
				if (!string.IsNullOrEmpty(_path))
				{
					Play();
				}
			}
			if (_endReachedEvent != null)
			{
				_endReachedEvent.Invoke();
			}
		}

		public void AddEndReachedEvent(UnityAction action)
		{
			_endReachedEvent.AddListener(action);
		}

		public void RemoveEndReachedEvent(UnityAction action)
		{
			_endReachedEvent.RemoveListener(action);
		}

		public void OnPlayerEncounteredError()
		{
			Stop();
			if (_encounteredErrorEvent != null)
			{
				_encounteredErrorEvent.Invoke();
			}
		}

		public void AddEncounteredErrorEvent(UnityAction action)
		{
			_encounteredErrorEvent.AddListener(action);
		}

		public void RemoveEncounteredErrorEvent(UnityAction action)
		{
			_encounteredErrorEvent.RemoveListener(action);
		}

		public void OnPlayerTimeChanged(long time)
		{
			if (_timeChangedEvent != null)
			{
				_timeChangedEvent.Invoke(time);
			}
		}

		public void AddTimeChangedEvent(UnityAction<long> action)
		{
			_timeChangedEvent.AddListener(action);
		}

		public void RemoveTimeChangedEvent(UnityAction<long> action)
		{
			_timeChangedEvent.RemoveListener(action);
		}

		public void OnPlayerPositionChanged(float position)
		{
			_position = _mediaPlayer.Position;
			if (_positionChangedEvent != null)
			{
				_positionChangedEvent.Invoke(position);
			}
		}

		public void AddPositionChangedEvent(UnityAction<float> action)
		{
			_positionChangedEvent.AddListener(action);
		}

		public void RemovePositionChangedEvent(UnityAction<float> action)
		{
			_positionChangedEvent.RemoveListener(action);
		}

		public void OnPlayerSnapshotTaken(string path)
		{
			if (_snapshotTakenEvent != null)
			{
				_snapshotTakenEvent.Invoke(path);
			}
		}

		public void AddSnapshotTakenEvent(UnityAction<string> action)
		{
			_snapshotTakenEvent.AddListener(action);
		}

		public void RemoveSnapshotTakenEvent(UnityAction<string> action)
		{
			_snapshotTakenEvent.RemoveListener(action);
		}
	}
}
