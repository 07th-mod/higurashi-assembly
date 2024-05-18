using System;
using System.Collections.Generic;
using System.IO;

namespace UMP
{
	public class PlayerOptionsStandalone : PlayerOptions
	{
		public enum PlayerTypes
		{
			LibVLC = 1
		}

		private const string FILE_CACHING_KEY = ":file-caching";

		private const string LIVE_CACHING_KEY = ":live-caching";

		private const string DISC_CACHING_KEY = ":disc-caching";

		private const string NETWORK_CACHING_KEY = ":network-caching";

		private const string CR_AVERAGE_KEY = ":cr-average";

		private const string CLOCK_SYNCHRO_KEY = ":clock-synchro";

		private const string CLOCK_JITTER_KEY = ":clock-jitter";

		private const string FLIP_VERTICALLY = "flip-vertically";

		private const string VIDEO_BUFFER_SIZE = "video-buffer-size";

		private const string SOUT_KEY = "--sout";

		private const string HARDWARE_DECODING_STATE_KEY = "avcodec-hw-state";

		private const string HARDWARE_DECODING_KEY = ":avcodec-hw";

		private const string RTSP_OVER_TCP_KEY = "--rtsp-tcp";

		private const string DIRECTX_AUDIO_DEVICE = "--directx-audio-device";

		private const int DEFAULT_CR_AVERAGE_VALUE = 40;

		private const int DEFAULT_CLOCK_JITTER_VALUE = 5000;

		private static Dictionary<string, string> _platformsHWNames = new Dictionary<string, string>
		{
			{
				"Win",
				"dxva2"
			},
			{
				"Mac",
				"vda"
			},
			{
				"Linux",
				"vaapi"
			}
		};

		private LogLevels _logDetail;

		private Action<PlayerManagerLogs.PlayerLog> _logListener;

		internal LogLevels LogDetail
		{
			get
			{
				return _logDetail;
			}
			set
			{
				_logDetail = value;
			}
		}

		internal Action<PlayerManagerLogs.PlayerLog> LogListener
		{
			get
			{
				return _logListener;
			}
			set
			{
				_logListener = value;
			}
		}

		public AudioOutput[] AudioOutputs
		{
			get;
			set;
		}

		public States HardwareDecoding
		{
			get
			{
				return (States)GetValue<int>("avcodec-hw-state");
			}
			set
			{
				int num = (int)value;
				SetValue("avcodec-hw-state", num.ToString());
				switch (value)
				{
				case States.Default:
					SetValue(":avcodec-hw", _platformsHWNames[UMPSettings.RuntimePlatformFolderName]);
					break;
				case States.Disable:
					RemoveOption(":avcodec-hw");
					break;
				default:
					SetValue(":avcodec-hw", "any");
					break;
				}
			}
		}

		public bool FlipVertically
		{
			get
			{
				return GetValue<bool>("flip-vertically");
			}
			set
			{
				if (value)
				{
					SetValue("flip-vertically", string.Empty);
				}
				else
				{
					RemoveOption("flip-vertically");
				}
			}
		}

		public bool VideoBufferSize
		{
			get
			{
				return GetValue<bool>("video-buffer-size");
			}
			set
			{
				if (value)
				{
					SetValue("video-buffer-size", string.Empty);
				}
				else
				{
					RemoveOption("video-buffer-size");
				}
			}
		}

		public bool UseTCP
		{
			get
			{
				return GetValue<bool>("--rtsp-tcp");
			}
			set
			{
				if (value)
				{
					SetValue("--rtsp-tcp", string.Empty);
				}
				else
				{
					RemoveOption("--rtsp-tcp");
				}
			}
		}

		public string DirectAudioDevice
		{
			get
			{
				return GetValue<string>("--directx-audio-device");
			}
			set
			{
				SetValue("--directx-audio-device", value);
			}
		}

		public int FileCaching
		{
			get
			{
				return GetValue<int>(":file-caching");
			}
			set
			{
				SetValue(":file-caching", value.ToString());
			}
		}

		public int LiveCaching
		{
			get
			{
				return GetValue<int>(":live-caching");
			}
			set
			{
				SetValue(":live-caching", value.ToString());
			}
		}

		public int DiskCaching
		{
			get
			{
				return GetValue<int>(":disc-caching");
			}
			set
			{
				SetValue(":disc-caching", value.ToString());
			}
		}

		public int NetworkCaching
		{
			get
			{
				return GetValue<int>(":network-caching");
			}
			set
			{
				SetValue(":network-caching", value.ToString());
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

		public PlayerOptionsStandalone(string[] options)
			: base(options)
		{
			LogDetail = LogLevels.Disable;
			HardwareDecoding = States.Default;
			FlipVertically = true;
			FileCaching = 300;
			LiveCaching = 300;
			DiskCaching = 300;
			NetworkCaching = 300;
			CrAverage = 40;
			ClockSynchro = States.Default;
			ClockJitter = 5000;
		}

		public void RedirectToFile(bool display, string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				string str = "#duplicate{dst=file{dst=" + Path.GetFullPath(path) + "}";
				str += ((!display) ? string.Empty : ",dst=display");
				str += "}";
				SetValue("--sout", str);
			}
		}

		public void SetLogDetail(LogLevels level, Action<PlayerManagerLogs.PlayerLog> logListener)
		{
			LogDetail = level;
			LogListener = logListener;
		}
	}
}
