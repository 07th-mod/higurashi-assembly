using System;

namespace UMP
{
	public class PlayerOptionsIPhone : PlayerOptions
	{
		public enum PlayerTypes
		{
			Native = 1,
			FFmpeg
		}

		private const string PLAYER_TYPE_KEY = ":player-type";

		private const string FLIP_VERTICALLY = ":flip-vertically";

		private const string VIDEOTOOLBOX_KEY = ":videotoolbox";

		private const string VIDEOTOOLBOX_FRAME_WIDTH_KEY = ":videotoolbox-max-frame-width";

		private const string VIDEOTOOLBOX_ASYNC_KEY = ":videotoolbox-async";

		private const string VIDEOTOOLBOX_WAIT_ASYNC_KEY = ":videotoolbox-wait-async";

		private const string PACKET_BUFFERING_KEY = ":packet-buffering";

		private const string MAX_BUFFER_SIZE_KEY = ":max-buffer-size";

		private const string MIN_FRAMES_KEY = ":min-frames";

		private const string INFBUF_KEY = ":infbuf";

		private const string FRAMEDROP_KEY = ":framedrop";

		private const string MAX_FPS_KEY = ":max-fps";

		private const string PLAY_IN_BACKGROUND_KEY = ":play-in-background";

		private const string RTSP_OVER_TCP_KEY = ":rtsp-tcp";

		private const int DEFAULT_VIDEOTOOLBOX_FRAME_WIDTH_VALUE = 4096;

		private const int DEFAULT_MAX_BUFFER_SIZE_VALUE = 15728640;

		private const int DEFAULT_MIN_FRAMES_VALUE = 50000;

		private const int DEFAULT_FRAMEDROP_VALUE = 0;

		private const int DEFAULT_MAX_FPS_VALUE = 31;

		public PlayerTypes PlayerType
		{
			get
			{
				return (PlayerTypes)GetValue<int>(":player-type");
			}
			set
			{
				UMPSettings instance = UMPSettings.Instance;
				Array values = Enum.GetValues(typeof(PlayerTypes));
				PlayerTypes playerTypes = PlayerTypes.Native;
				foreach (object item in values)
				{
					PlayerTypes playerTypes2 = (PlayerTypes)(int)item;
					if ((instance.PlayersIPhone & playerTypes2) == playerTypes2)
					{
						playerTypes = playerTypes2;
						if (playerTypes == value)
						{
							break;
						}
					}
				}
				int num = (int)playerTypes;
				SetValue(":player-type", num.ToString());
			}
		}

		public bool FlipVertically
		{
			get
			{
				return GetValue<bool>(":flip-vertically");
			}
			set
			{
				if (value)
				{
					SetValue(":flip-vertically", string.Empty);
				}
				else
				{
					RemoveOption(":flip-vertically");
				}
			}
		}

		public bool VideoToolbox
		{
			get
			{
				return GetValue<bool>(":videotoolbox");
			}
			set
			{
				if (value)
				{
					SetValue(":videotoolbox", string.Empty);
				}
				else
				{
					RemoveOption(":videotoolbox");
				}
			}
		}

		public int VideoToolboxFrameWidth
		{
			get
			{
				return GetValue<int>(":videotoolbox-max-frame-width");
			}
			set
			{
				SetValue(":videotoolbox-max-frame-width", value.ToString());
			}
		}

		public bool VideoToolboxAsync
		{
			get
			{
				return GetValue<bool>(":videotoolbox-async");
			}
			set
			{
				if (VideoToolbox && value)
				{
					SetValue(":videotoolbox-async", string.Empty);
				}
				else
				{
					RemoveOption(":videotoolbox-async");
				}
			}
		}

		public bool VideoToolboxWaitAsync
		{
			get
			{
				return GetValue<bool>(":videotoolbox-wait-async");
			}
			set
			{
				if (VideoToolboxAsync && value)
				{
					SetValue(":videotoolbox-wait-async", string.Empty);
				}
				else
				{
					RemoveOption(":videotoolbox-wait-async");
				}
			}
		}

		public bool PlayInBackground
		{
			get
			{
				return GetValue<bool>(":play-in-background");
			}
			set
			{
				if (value)
				{
					SetValue(":play-in-background", string.Empty);
				}
				else
				{
					RemoveOption(":play-in-background");
				}
			}
		}

		public bool UseTCP
		{
			get
			{
				return GetValue<bool>(":rtsp-tcp");
			}
			set
			{
				if (value)
				{
					SetValue(":rtsp-tcp", string.Empty);
				}
				else
				{
					RemoveOption(":rtsp-tcp");
				}
			}
		}

		public bool PacketBuffering
		{
			get
			{
				return GetValue<bool>(":packet-buffering");
			}
			set
			{
				if (value)
				{
					SetValue(":packet-buffering", string.Empty);
				}
				else
				{
					RemoveOption(":packet-buffering");
				}
			}
		}

		public int MaxBufferSize
		{
			get
			{
				return GetValue<int>(":max-buffer-size");
			}
			set
			{
				SetValue(":max-buffer-size", value.ToString());
			}
		}

		public int MinFrames
		{
			get
			{
				return GetValue<int>(":min-frames");
			}
			set
			{
				SetValue(":min-frames", value.ToString());
			}
		}

		public bool Infbuf
		{
			get
			{
				return GetValue<bool>(":infbuf");
			}
			set
			{
				if (value)
				{
					SetValue(":infbuf", string.Empty);
				}
				else
				{
					RemoveOption(":infbuf");
				}
			}
		}

		public int Framedrop
		{
			get
			{
				return GetValue<int>(":framedrop");
			}
			set
			{
				SetValue(":framedrop", value.ToString());
			}
		}

		public int MaxFps
		{
			get
			{
				return GetValue<int>(":max-fps");
			}
			set
			{
				SetValue(":max-fps", value.ToString());
			}
		}

		public PlayerOptionsIPhone(string[] options)
			: base(options)
		{
			PlayerType = PlayerTypes.FFmpeg;
			FlipVertically = true;
			VideoToolbox = true;
			VideoToolboxFrameWidth = 4096;
			VideoToolboxAsync = false;
			VideoToolboxWaitAsync = true;
			PlayInBackground = false;
			UseTCP = false;
			PacketBuffering = true;
			MaxBufferSize = 15728640;
			MinFrames = 50000;
			Infbuf = false;
			Framedrop = 0;
			MaxFps = 31;
		}
	}
}
