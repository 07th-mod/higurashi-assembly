using System;

namespace UMP
{
	public class PlayerOptionsAndroid : PlayerOptions
	{
		public enum PlayerTypes
		{
			Native = 1,
			LibVLC = 2,
			Exo = 4
		}

		public enum DecodingStates
		{
			Automatic = -1,
			Disabled,
			DecodingAcceleration,
			FullAcceleration
		}

		public enum ChromaTypes
		{
			RGB32Bit,
			RGB16Bit,
			YUV
		}

		private const string NETWORK_CACHING_KEY = "--network-caching";

		private const string CR_AVERAGE_KEY = ":cr-average";

		private const string CLOCK_SYNCHRO_KEY = ":clock-synchro";

		private const string CLOCK_JITTER_KEY = ":clock-jitter";

		private const string PLAYER_TYPE_KEY = "player-type";

		private const string HARDWARE_ACCELERATION_STATE_KEY = ":hw-state";

		private const string OPENGL_DECODING_STATE_KEY = "opengl-state";

		private const string OPENGL_DECODING_KEY = "--vout";

		private const string VIDEO_CHROMA_STATE_KEY = "chroma-state";

		private const string VIDEO_CHROMA_KEY = "--android-display-chroma";

		private const string PLAY_IN_BACKGROUND_KEY = ":play-in-background";

		private const string RTSP_OVER_TCP_KEY = ":rtsp-tcp";

		private const int DEFAULT_CR_AVERAGE_VALUE = 40;

		private const int DEFAULT_CLOCK_JITTER_VALUE = 5000;

		public PlayerTypes PlayerType
		{
			get
			{
				return (PlayerTypes)GetValue<int>("player-type");
			}
			set
			{
				UMPSettings instance = UMPSettings.Instance;
				Array values = Enum.GetValues(typeof(PlayerTypes));
				PlayerTypes playerTypes = PlayerTypes.Native;
				foreach (object item in values)
				{
					PlayerTypes playerTypes2 = (PlayerTypes)(int)item;
					if ((instance.PlayersAndroid & playerTypes2) == playerTypes2)
					{
						playerTypes = playerTypes2;
						if (playerTypes == value)
						{
							break;
						}
					}
				}
				int num = (int)playerTypes;
				SetValue("player-type", num.ToString());
			}
		}

		public DecodingStates HardwareAcceleration
		{
			get
			{
				return (DecodingStates)GetValue<int>(":hw-state");
			}
			set
			{
				int num = (int)value;
				SetValue(":hw-state", num.ToString());
			}
		}

		public States OpenGLDecoding
		{
			get
			{
				return (States)GetValue<int>("opengl-state");
			}
			set
			{
				int num = (int)value;
				SetValue("opengl-state", num.ToString());
				switch (value)
				{
				case States.Default:
					RemoveOption("--vout");
					break;
				case States.Enable:
					SetValue("--vout", "gles2,none");
					break;
				default:
					SetValue("--vout", "android_display,none");
					break;
				}
			}
		}

		public ChromaTypes VideoChroma
		{
			get
			{
				return (ChromaTypes)GetValue<int>("chroma-state");
			}
			set
			{
				int num = (int)value;
				SetValue("chroma-state", num.ToString());
				switch (value)
				{
				case ChromaTypes.RGB16Bit:
					SetValue("--android-display-chroma", "RV16");
					break;
				case ChromaTypes.YUV:
					SetValue("--android-display-chroma", "YV12");
					break;
				default:
					SetValue("--android-display-chroma", "RV32");
					break;
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

		public int NetworkCaching
		{
			get
			{
				return GetValue<int>("--network-caching");
			}
			set
			{
				if (value > 0)
				{
					SetValue("--network-caching", value.ToString());
				}
				else
				{
					RemoveOption("--network-caching");
				}
			}
		}

		public int CrAverage
		{
			get
			{
				return GetValue<int>(":cr-average");
			}
			set
			{
				SetValue(":cr-average", value.ToString());
			}
		}

		public States ClockSynchro
		{
			get
			{
				return (States)GetValue<int>(":clock-synchro");
			}
			set
			{
				int num = (int)value;
				SetValue(":cr-average", num.ToString());
			}
		}

		public int ClockJitter
		{
			get
			{
				return GetValue<int>(":clock-jitter");
			}
			set
			{
				SetValue(":clock-jitter", value.ToString());
			}
		}

		public PlayerOptionsAndroid(string[] options)
			: base(options)
		{
			PlayerType = PlayerTypes.Exo;
			NetworkCaching = 300;
			CrAverage = 40;
			ClockSynchro = States.Default;
			ClockJitter = 5000;
			HardwareAcceleration = DecodingStates.Automatic;
		}
	}
}
