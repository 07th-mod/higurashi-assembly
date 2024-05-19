using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UMP.Wrappers;
using UnityEngine;

namespace UMP
{
	public class MediaPlayerStandalone : IPlayer, IPlayerAudio, IPlayerSpu
	{
		private delegate void ManageBufferSizeCallback(int width, int height);

		private MonoBehaviour _monoObject;

		private WrapperStandalone _wrapper;

		private IntPtr _lockPtr;

		private IntPtr _displayPtr;

		private IntPtr _formatSetupPtr;

		private IntPtr _audioPlayPtr;

		private IntPtr _audioFormatPtr;

		private IntPtr _eventHandlerPtr;

		private IntPtr _eventManagerPtr;

		private IntPtr _vlcObj;

		private IntPtr _mediaObj;

		private IntPtr _playerObj;

		private int _framesCounter;

		private float _frameRate;

		private float _tmpTime;

		private int _tmpFramesCounter;

		private bool _isStarted;

		private bool _isPlaying;

		private bool _isLoad;

		private bool _isReady;

		private bool _isImageReady;

		private bool _isTextureExist;

		private LogLevels _logDetail;

		private Action<PlayerManagerLogs.PlayerLog> _logListener;

		private string _dataSource;

		private PlayerBufferVideo _videoBuffer;

		private PlayerManagerLogs _logManager;

		private PlayerManagerEvents _eventManager;

		private PlayerOptionsStandalone _options;

		private MediaStats _mediaStats;

		private string[] _arguments;

		private Texture2D _videoTexture;

		private GameObject[] _videoOutputObjects;

		private PlayerManagerAudios _audioManager;

		private GCHandle _audioDataHandle = default(GCHandle);

		private ManageBufferSizeCallback _manageBufferSizeCallback;

		private IEnumerator _updateVideoTextureEnum;

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

		public PlayerState State
		{
			get
			{
				if (_eventManagerPtr != IntPtr.Zero)
				{
					return _wrapper.PlayerGetState();
				}
				return PlayerState.Empty;
			}
		}

		public object StateValue
		{
			get
			{
				if (_eventManagerPtr != IntPtr.Zero)
				{
					return _wrapper.PlayerGetStateValue();
				}
				return null;
			}
		}

		public string DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				if (!(_playerObj != IntPtr.Zero))
				{
					return;
				}
				_dataSource = value;
				if (_mediaObj != IntPtr.Zero)
				{
					_wrapper.ExpandedMediaRelease(_mediaObj);
				}
				_mediaObj = _wrapper.ExpandedMediaNewLocation(_vlcObj, MediaPlayerHelper.GetDataSourcePath(_dataSource));
				if (_arguments != null)
				{
					string[] arguments = _arguments;
					foreach (string text in arguments)
					{
						if (text.Contains(":"))
						{
							_wrapper.ExpandedAddOption(_mediaObj, text);
						}
					}
				}
				_wrapper.ExpandedSetMedia(_playerObj, _mediaObj);
			}
		}

		public bool IsPlaying => _isPlaying;

		public bool IsReady => _isReady;

		public bool AbleToPlay
		{
			get
			{
				if (_playerObj != IntPtr.Zero)
				{
					return _wrapper.PlayerWillPlay(_playerObj);
				}
				return false;
			}
		}

		public long Length
		{
			get
			{
				if (_playerObj != IntPtr.Zero)
				{
					return _wrapper.PlayerGetLength(_playerObj);
				}
				return 0L;
			}
		}

		public float FrameRate => _frameRate;

		public byte[] FramePixels
		{
			get
			{
				if (_videoBuffer != null)
				{
					return _videoBuffer.FramePixels;
				}
				return new byte[0];
			}
		}

		public int FramesCounter => _wrapper.NativeGetFramesCounter();

		public long Time
		{
			get
			{
				if (_playerObj != IntPtr.Zero)
				{
					return _wrapper.PlayerGetTime(_playerObj);
				}
				return 0L;
			}
			set
			{
				_wrapper.NativeClearAudioSamples(0);
				if (_playerObj != IntPtr.Zero)
				{
					_wrapper.PlayerSetTime(value, _playerObj);
				}
			}
		}

		public float Position
		{
			get
			{
				if (_playerObj != IntPtr.Zero)
				{
					return _wrapper.PlayerGetPosition(_playerObj);
				}
				return 0f;
			}
			set
			{
				_wrapper.NativeClearAudioSamples(0);
				if (_playerObj != IntPtr.Zero)
				{
					_wrapper.PlayerSetPosition(value, _playerObj);
				}
				_wrapper.NativeUpdateFramesCounter((FramesAmount > 0) ? ((int)(value * (float)FramesAmount)) : 0);
			}
		}

		public float PlaybackRate
		{
			get
			{
				if (_playerObj != IntPtr.Zero)
				{
					return _wrapper.PlayerGetRate(_playerObj);
				}
				return 0f;
			}
			set
			{
				if (_playerObj != IntPtr.Zero && !_wrapper.PlayerSetRate(value, _playerObj))
				{
					throw new Exception("Native library problem: can't change playback rate");
				}
			}
		}

		public int Volume
		{
			get
			{
				if (_playerObj != IntPtr.Zero)
				{
					return _wrapper.PlayerGetVolume(_playerObj);
				}
				return 0;
			}
			set
			{
				if (_playerObj != IntPtr.Zero)
				{
					_wrapper.PlayerSetVolume(value, _playerObj);
				}
			}
		}

		public bool Mute
		{
			get
			{
				if (_playerObj != IntPtr.Zero)
				{
					return _wrapper.PlayerGetMute(_playerObj);
				}
				return false;
			}
			set
			{
				if (_playerObj != IntPtr.Zero)
				{
					_wrapper.PlayerSetMute(value, _playerObj);
				}
			}
		}

		public int VideoWidth
		{
			get
			{
				int num = 0;
				if (_playerObj != IntPtr.Zero)
				{
					num = _wrapper.PlayerVideoWidth(_playerObj);
					if (_videoBuffer != null && (num <= 0 || _options.FixedVideoSize != Vector2.zero || _options.VideoBufferSize))
					{
						num = _videoBuffer.Width;
					}
				}
				return num;
			}
		}

		public int VideoHeight
		{
			get
			{
				int num = 0;
				if (_playerObj != IntPtr.Zero)
				{
					num = _wrapper.PlayerVideoHeight(_playerObj);
					if (_videoBuffer != null && (num <= 0 || _options.FixedVideoSize != Vector2.zero || _options.VideoBufferSize))
					{
						num = _videoBuffer.Height;
					}
				}
				return num;
			}
		}

		public Vector2 VideoSize => new Vector2(VideoWidth, VideoHeight);

		public MediaTrackInfo[] AudioTracks => (!(_playerObj != IntPtr.Zero)) ? null : _wrapper.PlayerAudioGetTracks(_playerObj);

		public MediaTrackInfo AudioTrack
		{
			get
			{
				if (_playerObj == IntPtr.Zero)
				{
					return null;
				}
				int id = _wrapper.PlayerAudioGetTrack(_playerObj);
				return AudioTracks.SingleOrDefault((MediaTrackInfo t) => t.Id == id);
			}
			set
			{
				int num = _wrapper.PlayerAudioSetTrack(value.Id, _playerObj);
				if (num == -1)
				{
					throw new Exception("Native library problem: can't set new audio track");
				}
			}
		}

		public MediaTrackInfo[] SpuTracks => (!(_playerObj != IntPtr.Zero)) ? null : _wrapper.PlayerSpuGetTracks(_playerObj);

		public MediaTrackInfo SpuTrack
		{
			get
			{
				if (_playerObj == IntPtr.Zero)
				{
					return null;
				}
				int id = _wrapper.PlayerSpuGetTrack(_playerObj);
				return SpuTracks.SingleOrDefault((MediaTrackInfo t) => t.Id == id);
			}
			set
			{
				int num = _wrapper.PlayerSpuSetTrack(value.Id, _playerObj);
				if (num == -1)
				{
					throw new Exception("Native library problem: can't set new spu track");
				}
			}
		}

		public MediaStats MediaStats
		{
			get
			{
				if (_mediaObj != IntPtr.Zero)
				{
					_wrapper.ExpandedMediaGetStats(_mediaObj, out _mediaStats);
				}
				return _mediaStats;
			}
		}

		public MediaTrackInfoExpanded[] TracksInfo
		{
			get
			{
				if (_mediaObj != IntPtr.Zero)
				{
					TrackInfo[] array = _wrapper.ExpandedMediaGetTracksInfo(_mediaObj);
					if (array != null && array.Length > 0)
					{
						List<MediaTrackInfoExpanded> list = new List<MediaTrackInfoExpanded>(array.Length);
						TrackInfo[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							TrackInfo trackInfo = array2[i];
							switch (trackInfo.Type)
							{
							case TrackTypes.Unknown:
								list.Add(new MediaTrackInfoUnknown(trackInfo.Codec, trackInfo.Id, trackInfo.Profile, trackInfo.Level));
								break;
							case TrackTypes.Video:
								list.Add(new MediaTrackInfoVideo(trackInfo.Codec, trackInfo.Id, trackInfo.Profile, trackInfo.Level, trackInfo.Video.Width, trackInfo.Video.Height));
								break;
							case TrackTypes.Audio:
								list.Add(new MediaTrackInfoAudio(trackInfo.Codec, trackInfo.Id, trackInfo.Profile, trackInfo.Level, trackInfo.Audio.Channels, trackInfo.Audio.Rate));
								break;
							case TrackTypes.Text:
								list.Add(new MediaTrackInfoSpu(trackInfo.Codec, trackInfo.Id, trackInfo.Profile, trackInfo.Level));
								break;
							}
						}
						return list.ToArray();
					}
				}
				return null;
			}
		}

		public long AudioDelay
		{
			get
			{
				if (_playerObj != IntPtr.Zero)
				{
					return _wrapper.ExpandedGetAudioDelay(_playerObj);
				}
				return 0L;
			}
			set
			{
				if (_playerObj != IntPtr.Zero)
				{
					_wrapper.ExpandedSetAudioDelay(_playerObj, value);
				}
			}
		}

		public float VideoScale
		{
			get
			{
				if (_playerObj != IntPtr.Zero)
				{
					return _wrapper.ExpandedVideoGetScale(_playerObj);
				}
				return 0f;
			}
			set
			{
				if (_playerObj != IntPtr.Zero)
				{
					_wrapper.ExpandedVideoSetScale(_playerObj, value);
				}
			}
		}

		public float FrameRateStable
		{
			get
			{
				if (_playerObj != IntPtr.Zero && IsReady)
				{
					return _wrapper.ExpandedVideoFrameRate(_playerObj);
				}
				return 0f;
			}
		}

		public int FramesAmount
		{
			get
			{
				if (_playerObj != IntPtr.Zero && IsReady)
				{
					return (int)((float)Length * FrameRateStable * 0.001f);
				}
				return 0;
			}
		}

		public string LogMessage
		{
			get
			{
				if (_vlcObj != IntPtr.Zero)
				{
					return _wrapper.NativeGetLogMessage();
				}
				return null;
			}
		}

		public int LogLevel
		{
			get
			{
				if (_vlcObj != IntPtr.Zero)
				{
					return _wrapper.NativeGetLogLevel();
				}
				return -1;
			}
		}

		public MediaPlayerStandalone(MonoBehaviour monoObject, GameObject[] videoOutputObjects, PlayerOptionsStandalone options)
		{
			_monoObject = monoObject;
			_videoOutputObjects = videoOutputObjects;
			_options = options;
			_wrapper = new WrapperStandalone(_options);
			if (_wrapper.NativeIndex < 0)
			{
				Debug.LogError("Don't support video playback on current platform or you use incorrect UMP libraries!");
				throw new Exception();
			}
			if (_options != null)
			{
				if (!string.IsNullOrEmpty(_options.DirectAudioDevice))
				{
					_options.DirectAudioDevice = GetAudioDevice(_options.DirectAudioDevice);
				}
				_wrapper.NativeSetPixelsVerticalFlip(_options.FlipVertically);
				if (_options.AudioOutputs != null && _options.AudioOutputs.Length > 0)
				{
					_audioManager = new PlayerManagerAudios(_options.AudioOutputs);
					_audioManager.AddListener(OnAudioFilterRead);
				}
				_arguments = _options.GetOptions('\n').Split('\n');
				_logDetail = _options.LogDetail;
				_logListener = _options.LogListener;
			}
			MediaPlayerInit();
		}

		private void MediaPlayerInit()
		{
			_vlcObj = _wrapper.ExpandedLibVLCNew(_arguments);
			if (_vlcObj == IntPtr.Zero)
			{
				throw new Exception("Can't create new libVLC object instance");
			}
			_playerObj = _wrapper.ExpandedMediaPlayerNew(_vlcObj);
			if (_playerObj == IntPtr.Zero)
			{
				throw new Exception("Can't create new media player object instance");
			}
			_eventManagerPtr = _wrapper.ExpandedEventManager(_playerObj);
			_eventHandlerPtr = _wrapper.NativeMediaPlayerEventCallback();
			EventsAttach(_eventManagerPtr, _eventHandlerPtr);
			_eventManager = new PlayerManagerEvents(_monoObject, this);
			_eventManager.PlayerPlayingListener += OnPlayerPlaying;
			_eventManager.PlayerPausedListener += OnPlayerPaused;
			if (_logDetail != 0)
			{
				_wrapper.ExpandedLogSet(_vlcObj, _wrapper.NativeGetLogMessageCallback(), new IntPtr(_wrapper.NativeIndex));
			}
			_logManager = new PlayerManagerLogs(_monoObject, this);
			_logManager.LogDetail = _logDetail;
			_logManager.LogMessageListener += _logListener;
			_lockPtr = _wrapper.NativeGetVideoLockCallback();
			_displayPtr = _wrapper.NativeGetVideoDisplayCallback();
			_formatSetupPtr = _wrapper.NativeGetVideoFormatCallback();
			_audioFormatPtr = _wrapper.NativeGetAudioSetupCallback();
			_audioPlayPtr = _wrapper.NativeGetAudioPlayCallback();
			_wrapper.ExpandedVideoSetCallbacks(_playerObj, _lockPtr, IntPtr.Zero, _displayPtr, new IntPtr(_wrapper.NativeIndex));
			if (_options.FixedVideoSize == Vector2.zero)
			{
				_wrapper.ExpandedVideoSetFormatCallbacks(_playerObj, _formatSetupPtr, IntPtr.Zero);
			}
			else
			{
				WrapperStandalone wrapper = _wrapper;
				IntPtr playerObj = _playerObj;
				string chroma = PlayerBufferVideo.Chroma;
				Vector2 fixedVideoSize = _options.FixedVideoSize;
				int width = (int)fixedVideoSize.x;
				Vector2 fixedVideoSize2 = _options.FixedVideoSize;
				int height = (int)fixedVideoSize2.y;
				Vector2 fixedVideoSize3 = _options.FixedVideoSize;
				wrapper.ExpandedVideoSetFormat(playerObj, chroma, width, height, PlayerBufferVideo.CalculatePitch((int)fixedVideoSize3.x));
			}
			_manageBufferSizeCallback = InitBufferSize;
			_wrapper.NativeSetBufferSizeCallback(Marshal.GetFunctionPointerForDelegate(_manageBufferSizeCallback));
			if (_audioManager != null && _audioManager.IsValid)
			{
				_wrapper.ExpandedAudioSetFormatCallbacks(_playerObj, _audioFormatPtr, IntPtr.Zero);
				_wrapper.ExpandedAudioSetCallbacks(_playerObj, _audioPlayPtr, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, new IntPtr(_wrapper.NativeIndex));
				_wrapper.NativeSetAudioParams(2, AudioSettings.outputSampleRate);
			}
			_mediaStats = default(MediaStats);
		}

		private void OnRelease()
		{
			bool flag = false;
			while (!flag)
			{
				flag = (_monoObject == null);
				Thread.Sleep(1000);
			}
			Release();
		}

		private void InitBufferSize(int width, int height)
		{
			_wrapper.NativePixelsBufferRelease();
			if (_videoBuffer != null && (_videoBuffer.Width != width || _videoBuffer.Height != height))
			{
				_videoBuffer.ClearFramePixels();
				_videoBuffer = null;
			}
			if (_videoBuffer == null)
			{
				_videoBuffer = new PlayerBufferVideo(width, height);
				_wrapper.NativeSetPixelsBuffer(_videoBuffer.FramePixelsAddr, _videoBuffer.Width, _videoBuffer.Height);
				_isTextureExist = false;
			}
		}

		private string GetAudioDevice(string description)
		{
			string[] array = new string[2]
			{
				"directsound",
				"directx"
			};
			IntPtr intPtr = _wrapper.ExpandedLibVLCNew(null);
			IntPtr intPtr2 = _wrapper.ExpandedAudioOutputListGet(intPtr);
			IntPtr intPtr3 = IntPtr.Zero;
			string text = string.Empty;
			AudioDescription audioDescription = default(AudioDescription);
			audioDescription.NextDescription = intPtr2;
			audioDescription.Description = null;
			audioDescription.Name = null;
			AudioDescription audioDescription2 = audioDescription;
			while (audioDescription2.NextDescription != IntPtr.Zero)
			{
				audioDescription2 = (AudioDescription)Marshal.PtrToStructure(audioDescription2.NextDescription, typeof(AudioDescription));
				for (int i = 0; i < array.Length; i += 2)
				{
					if (audioDescription2.Name.Contains(array[i]))
					{
						intPtr3 = _wrapper.ExpandedAudioOutputDeviceListGet(intPtr, array[i + 1]);
						break;
					}
				}
			}
			if (intPtr3 == IntPtr.Zero)
			{
				Debug.Log("GetAudioDevice: Can't get audio output device list for " + audioDescription2.Name);
				return text;
			}
			AudioOutputDevice audioOutputDevice = default(AudioOutputDevice);
			audioOutputDevice.NextDevice = intPtr3;
			audioOutputDevice.Description = null;
			audioOutputDevice.Device = null;
			AudioOutputDevice audioOutputDevice2 = audioOutputDevice;
			try
			{
				while (audioOutputDevice2.NextDevice != IntPtr.Zero)
				{
					audioOutputDevice2 = (AudioOutputDevice)Marshal.PtrToStructure(audioOutputDevice2.NextDevice, typeof(AudioOutputDevice));
					if (audioOutputDevice2.Description.Contains(description))
					{
						Debug.Log("GetAudioDevice: New audio output device \nDevice: " + audioOutputDevice2.Device + "\nDescription: " + audioOutputDevice2.Description);
						text = audioOutputDevice2.Device;
					}
				}
			}
			finally
			{
				if (intPtr2 != IntPtr.Zero)
				{
					_wrapper.ExpandedAudioOutputListRelease(intPtr2);
				}
				if (intPtr3 != IntPtr.Zero)
				{
					_wrapper.ExpandedAudioOutputDeviceListRelease(intPtr3);
				}
				_wrapper.ExpandedLibVLCRelease(intPtr);
			}
			if (string.IsNullOrEmpty(text))
			{
				Debug.Log("GetAudioDevice: Can't find audio output device - switched to Default");
			}
			return text;
		}

		private void UpdateFpsCounter(int frameCounter)
		{
			float time = UnityEngine.Time.time;
			time = ((!(time > _tmpTime)) ? 0f : (time - _tmpTime));
			if (time >= 1f)
			{
				_frameRate = frameCounter - _tmpFramesCounter;
				_tmpFramesCounter = frameCounter;
				_tmpTime = UnityEngine.Time.time;
			}
		}

		private void EventsAttach(IntPtr eventManager, IntPtr enentHandlerPtr)
		{
			string str = "Failed to subscribe to event notification";
			if (_wrapper.ExpandedEventAttach(eventManager, EventTypes.MediaPlayerOpening, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex) != 0)
			{
				throw new OutOfMemoryException(str + " (Opening)");
			}
			if (_wrapper.ExpandedEventAttach(eventManager, EventTypes.MediaPlayerBuffering, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex) != 0)
			{
				throw new OutOfMemoryException(str + " (Buffering)");
			}
			if (_wrapper.ExpandedEventAttach(eventManager, EventTypes.MediaPlayerPaused, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex) != 0)
			{
				throw new OutOfMemoryException(str + " (Paused)");
			}
			if (_wrapper.ExpandedEventAttach(eventManager, EventTypes.MediaPlayerStopped, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex) != 0)
			{
				throw new OutOfMemoryException(str + " (Stopped)");
			}
			if (_wrapper.ExpandedEventAttach(eventManager, EventTypes.MediaPlayerEndReached, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex) != 0)
			{
				throw new OutOfMemoryException(str + " (EndReached)");
			}
			if (_wrapper.ExpandedEventAttach(eventManager, EventTypes.MediaPlayerEncounteredError, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex) != 0)
			{
				throw new OutOfMemoryException(str + " (EncounteredError)");
			}
			if (_wrapper.ExpandedEventAttach(eventManager, EventTypes.MediaPlayerTimeChanged, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex) != 0)
			{
				throw new OutOfMemoryException(str + " (TimeChanged)");
			}
			if (_wrapper.ExpandedEventAttach(eventManager, EventTypes.MediaPlayerPositionChanged, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex) != 0)
			{
				throw new OutOfMemoryException(str + " (PositionChanged)");
			}
			if (_wrapper.ExpandedEventAttach(eventManager, EventTypes.MediaPlayerSnapshotTaken, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex) != 0)
			{
				throw new OutOfMemoryException(str + " (SnapshotTaken)");
			}
		}

		private void EventsDettach(IntPtr eventManager, IntPtr enentHandlerPtr)
		{
			_wrapper.ExpandedEventDetach(eventManager, EventTypes.MediaPlayerOpening, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex);
			_wrapper.ExpandedEventDetach(eventManager, EventTypes.MediaPlayerBuffering, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex);
			_wrapper.ExpandedEventDetach(eventManager, EventTypes.MediaPlayerPaused, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex);
			_wrapper.ExpandedEventDetach(eventManager, EventTypes.MediaPlayerStopped, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex);
			_wrapper.ExpandedEventDetach(eventManager, EventTypes.MediaPlayerEndReached, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex);
			_wrapper.ExpandedEventDetach(eventManager, EventTypes.MediaPlayerEncounteredError, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex);
			_wrapper.ExpandedEventDetach(eventManager, EventTypes.MediaPlayerTimeChanged, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex);
			_wrapper.ExpandedEventDetach(eventManager, EventTypes.MediaPlayerPositionChanged, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex);
			_wrapper.ExpandedEventDetach(eventManager, EventTypes.MediaPlayerSnapshotTaken, enentHandlerPtr, (IntPtr)_wrapper.NativeIndex);
		}

		private IEnumerator UpdateVideoTexture()
		{
			MediaTrackInfoExpanded[] tracks = null;
			bool hasVideo = false;
			while (true)
			{
				if (_playerObj != IntPtr.Zero && _wrapper.PlayerIsPlaying(_playerObj))
				{
					if (tracks == null)
					{
						tracks = TracksInfo;
						if (tracks == null)
						{
							yield return null;
							continue;
						}
						MediaTrackInfoExpanded[] array = tracks;
						foreach (MediaTrackInfoExpanded track in array)
						{
							if (track is MediaTrackInfoVideo)
							{
								hasVideo = true;
							}
						}
					}
					if (FramesCounter != _framesCounter)
					{
						_framesCounter = FramesCounter;
						UpdateFpsCounter(_framesCounter);
						if (!_isTextureExist)
						{
							if (_videoTexture != null)
							{
								UnityEngine.Object.Destroy(_videoTexture);
								_videoTexture = null;
							}
							_videoTexture = MediaPlayerHelper.GenPluginTexture(_videoBuffer.Width, _videoBuffer.Height);
							MediaPlayerHelper.ApplyTextureToRenderingObjects(_videoTexture, _videoOutputObjects);
							_wrapper.NativeSetTexture(_videoTexture.GetNativeTexturePtr());
							_isTextureExist = true;
							_isImageReady = false;
						}
						GL.IssuePluginEvent(_wrapper.NativeGetUnityRenderCallback(), _wrapper.NativeIndex);
						if (!_isImageReady)
						{
							_eventManager.SetEvent(PlayerState.ImageReady, _videoTexture);
							_isImageReady = true;
						}
					}
					if (!_isReady && ((!hasVideo) ? (tracks != null) : (_videoTexture != null && _videoBuffer != null)))
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
				}
				yield return null;
			}
		}

		private IEnumerator InitAudioOutput()
		{
			while (true)
			{
				string paramsLine = _wrapper.NativeGetAudioParams('@');
				int rate = 0;
				if (!string.IsNullOrEmpty(paramsLine))
				{
					string[] options = paramsLine.Split('@');
					if (options.Length >= 1)
					{
						int.TryParse(options[0], out int channels);
						int.TryParse(options[1], out rate);
						if (channels > 0 || rate > 0)
						{
							break;
						}
					}
				}
				yield return null;
			}
		}

		private void OnAudioFilterRead(int id, float[] data, AudioOutput.AudioChannels audioChannel)
		{
			int num = _wrapper.NativeGetAudioSamples(IntPtr.Zero, 0, audioChannel);
			if (num >= data.Length)
			{
				if (!_audioDataHandle.IsAllocated)
				{
					_audioDataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
				}
				_wrapper.NativeGetAudioSamples(_audioDataHandle.AddrOfPinnedObject(), data.Length, audioChannel);
				_audioManager.SetOutputData(id, data);
			}
			if (_audioManager.OutputsDataUpdated)
			{
				_wrapper.NativeClearAudioSamples(data.Length);
				_audioManager.ResetOutputsData();
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
			if (_playerObj != IntPtr.Zero)
			{
				if (!_isStarted)
				{
					if (_eventManager != null)
					{
						_eventManager.StartListener();
					}
					if (_logManager != null)
					{
						_logManager.StartListener();
					}
					if (_options.FixedVideoSize != Vector2.zero)
					{
						Vector2 fixedVideoSize = _options.FixedVideoSize;
						int width = (int)fixedVideoSize.x;
						Vector2 fixedVideoSize2 = _options.FixedVideoSize;
						InitBufferSize(width, (int)fixedVideoSize2.y);
					}
					_wrapper.NativeUpdateFramesCounter(0);
				}
				if (_updateVideoTextureEnum == null)
				{
					_updateVideoTextureEnum = UpdateVideoTexture();
					_monoObject.StartCoroutine(_updateVideoTextureEnum);
				}
				_isStarted = _wrapper.PlayerPlay(_playerObj);
				if (_isStarted)
				{
					if (_audioManager != null)
					{
						_audioManager.Play();
					}
					if (_isReady && !_isPlaying)
					{
						_eventManager.SetEvent(PlayerState.Playing);
					}
				}
				else
				{
					Stop();
				}
			}
			return _isStarted;
		}

		public void Pause()
		{
			if (_playerObj != IntPtr.Zero)
			{
				if (_videoOutputObjects == null && _updateVideoTextureEnum != null)
				{
					_monoObject.StopCoroutine(_updateVideoTextureEnum);
					_updateVideoTextureEnum = null;
				}
				_wrapper.PlayerPause(_playerObj);
				if (_audioManager != null && _videoOutputObjects != null)
				{
					_audioManager.Pause();
				}
			}
		}

		public void Stop(bool resetTexture)
		{
			if (!(_playerObj != IntPtr.Zero) || !_isStarted)
			{
				return;
			}
			_wrapper.PlayerStop(_playerObj);
			if (_updateVideoTextureEnum != null)
			{
				_monoObject.StopCoroutine(_updateVideoTextureEnum);
				_updateVideoTextureEnum = null;
			}
			_framesCounter = 0;
			_frameRate = 0f;
			_tmpFramesCounter = 0;
			_tmpTime = 0f;
			_isStarted = false;
			_isPlaying = false;
			_isLoad = false;
			_isReady = false;
			_isImageReady = false;
			_wrapper.NativeUpdateFramesCounter(0);
			_wrapper.NativeClearAudioSamples(0);
			_wrapper.NativePixelsBufferRelease();
			_isTextureExist = !resetTexture;
			if (resetTexture)
			{
				if (_videoTexture != null)
				{
					UnityEngine.Object.Destroy(_videoTexture);
					_videoTexture = null;
				}
				if (_videoBuffer != null)
				{
					_videoBuffer.ClearFramePixels();
					_videoBuffer = null;
				}
			}
			if (_audioDataHandle.IsAllocated)
			{
				_audioDataHandle.Free();
			}
			if (_audioManager != null)
			{
				_audioManager.Stop();
			}
			if (_eventManager != null)
			{
				_eventManager.StopListener();
			}
			if (_logManager != null)
			{
				_logManager.StopListener();
			}
		}

		public void Stop()
		{
			Stop(resetTexture: true);
		}

		public void Release()
		{
			if (_playerObj != IntPtr.Zero)
			{
				Stop();
			}
			if (_eventManager != null)
			{
				_eventManager.RemoveAllEvents();
				_eventManager = null;
				if (_eventHandlerPtr != IntPtr.Zero)
				{
					EventsDettach(_eventManagerPtr, _eventHandlerPtr);
				}
			}
			if (_logManager != null)
			{
				_logManager.RemoveAllEvents();
				if (_logDetail != 0 && _vlcObj != IntPtr.Zero)
				{
					_wrapper.ExpandedLogUnset(_vlcObj);
				}
			}
			if (_audioManager != null)
			{
				_audioManager.RemoveAllListeners();
			}
			if (_playerObj != IntPtr.Zero)
			{
				_wrapper.PlayerRelease(_playerObj);
			}
			_playerObj = IntPtr.Zero;
			if (_vlcObj != IntPtr.Zero)
			{
				_wrapper.ExpandedLibVLCRelease(_vlcObj);
			}
			_vlcObj = IntPtr.Zero;
		}

		public string GetFormattedLength(bool detail)
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(Length);
			string format = (!detail) ? "{0:D2}:{1:D2}:{2:D2}" : "{0:D2}:{1:D2}:{2:D2}:{3:D3}";
			return string.Format(format, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		}

		public bool SetSubtitleFile(Uri path)
		{
			if (_playerObj != IntPtr.Zero)
			{
				return _wrapper.ExpandedSpuSetFile(_playerObj, path.AbsolutePath) == 1;
			}
			return false;
		}

		public void TakeSnapShot(string path)
		{
			if (_playerObj != IntPtr.Zero)
			{
				_wrapper.ExpandedVideoTakeSnapshot(_playerObj, 0u, path, 0u, 0u);
			}
		}

		public string GetLastError()
		{
			if (_logManager != null)
			{
				return _logManager.LastError;
			}
			return string.Empty;
		}
	}
}
