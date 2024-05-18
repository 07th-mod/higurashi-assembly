using System;
using UnityEngine;

namespace UMP
{
	public class MediaPlayer : IPlayer, IPlayerAudio, IPlayerSpu
	{
		private object _playerObject;

		private IPlayer _player;

		private IPlayerAudio _playerAudio;

		private IPlayerSpu _playerSpu;

		public object PlatformPlayer => _playerObject;

		public GameObject[] VideoOutputObjects
		{
			get
			{
				return _player.VideoOutputObjects;
			}
			set
			{
				_player.VideoOutputObjects = value;
			}
		}

		public PlayerManagerEvents EventManager => _player.EventManager;

		public PlayerOptions Options => _player.Options;

		public PlayerState State => _player.State;

		public object StateValue => _player.StateValue;

		public string DataSource
		{
			get
			{
				return _player.DataSource;
			}
			set
			{
				_player.DataSource = value;
			}
		}

		public bool IsPlaying => _player.IsPlaying;

		public bool IsReady => _player.IsReady;

		public bool AbleToPlay => _player.AbleToPlay;

		public long Length => _player.Length;

		public float FrameRate => _player.FrameRate;

		public int FramesCounter => _player.FramesCounter;

		public byte[] FramePixels => _player.FramePixels;

		public long Time
		{
			get
			{
				return _player.Time;
			}
			set
			{
				_player.Time = value;
			}
		}

		public float Position
		{
			get
			{
				return _player.Position;
			}
			set
			{
				_player.Position = value;
			}
		}

		public float PlaybackRate
		{
			get
			{
				return _player.PlaybackRate;
			}
			set
			{
				_player.PlaybackRate = value;
			}
		}

		public int Volume
		{
			get
			{
				return _player.Volume;
			}
			set
			{
				_player.Volume = value;
			}
		}

		public bool Mute
		{
			get
			{
				return _player.Mute;
			}
			set
			{
				_player.Mute = value;
			}
		}

		public int VideoWidth => _player.VideoWidth;

		public int VideoHeight => _player.VideoHeight;

		public Vector2 VideoSize => new Vector2(VideoWidth, VideoHeight);

		public MediaTrackInfo[] AudioTracks
		{
			get
			{
				if (_playerAudio != null)
				{
					return _playerAudio.AudioTracks;
				}
				return null;
			}
		}

		public MediaTrackInfo AudioTrack
		{
			get
			{
				if (_playerAudio != null)
				{
					return _playerAudio.AudioTrack;
				}
				return null;
			}
			set
			{
				if (_playerAudio != null)
				{
					_playerAudio.AudioTrack = value;
				}
			}
		}

		public MediaTrackInfo[] SpuTracks
		{
			get
			{
				if (_playerSpu != null)
				{
					return _playerSpu.SpuTracks;
				}
				return null;
			}
		}

		public MediaTrackInfo SpuTrack
		{
			get
			{
				if (_playerSpu != null)
				{
					return _playerSpu.SpuTrack;
				}
				return null;
			}
			set
			{
				if (_playerSpu != null)
				{
					_playerSpu.SpuTrack = value;
				}
			}
		}

		public MediaPlayer(MonoBehaviour monoObject, GameObject[] videoOutputObjects)
			: this(monoObject, videoOutputObjects, null)
		{
		}

		public MediaPlayer(MonoBehaviour monoObject, GameObject[] videoOutputObjects, PlayerOptions options)
		{
			switch (UMPSettings.RuntimePlatform)
			{
			case UMPSettings.Platforms.Win:
			case UMPSettings.Platforms.Mac:
			case UMPSettings.Platforms.Linux:
			{
				PlayerOptionsStandalone playerOptionsStandalone = null;
				playerOptionsStandalone = ((!(options is PlayerOptionsStandalone)) ? new PlayerOptionsStandalone(null) : (options as PlayerOptionsStandalone));
				_playerObject = new MediaPlayerStandalone(monoObject, videoOutputObjects, playerOptionsStandalone);
				break;
			}
			case UMPSettings.Platforms.WebGL:
				_playerObject = new MediaPlayerWebGL(monoObject, videoOutputObjects, options);
				break;
			}
			if (_playerObject is IPlayer)
			{
				_player = (_playerObject as IPlayer);
			}
			if (_playerObject is IPlayerAudio)
			{
				_playerAudio = (_playerObject as IPlayerAudio);
			}
			if (_playerObject is IPlayerSpu)
			{
				_playerSpu = (_playerObject as IPlayerSpu);
			}
		}

		public MediaPlayer(MonoBehaviour monoObject, MediaPlayer basedPlayer)
			: this(monoObject, basedPlayer.VideoOutputObjects, basedPlayer.Options)
		{
			if (basedPlayer.DataSource != null && string.IsNullOrEmpty(basedPlayer.DataSource.ToString()))
			{
				_player.DataSource = basedPlayer.DataSource;
			}
			_player.EventManager.CopyPlayerEvents(basedPlayer.EventManager);
			_player.Mute = basedPlayer.Mute;
			_player.Volume = basedPlayer.Volume;
			_player.PlaybackRate = basedPlayer.PlaybackRate;
		}

		public void AddMediaListener(IMediaListener listener)
		{
			_player.AddMediaListener(listener);
		}

		public void RemoveMediaListener(IMediaListener listener)
		{
			_player.RemoveMediaListener(listener);
		}

		public void Prepare()
		{
			_player.Prepare();
		}

		public bool Play()
		{
			return _player.Play();
		}

		public void Pause()
		{
			_player.Pause();
		}

		public void Stop(bool resetTexture)
		{
			_player.Stop(resetTexture);
		}

		public void Stop()
		{
			Stop(resetTexture: true);
		}

		public void Release()
		{
			_player.Release();
		}

		public string GetFormattedLength(bool detail)
		{
			return _player.GetFormattedLength(detail);
		}

		public bool SetSubtitleFile(Uri path)
		{
			if (_playerSpu != null)
			{
				return _playerSpu.SetSubtitleFile(path);
			}
			return false;
		}
	}
}
