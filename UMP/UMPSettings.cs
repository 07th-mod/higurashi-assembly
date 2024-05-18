using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UMP
{
	[Serializable]
	[CreateAssetMenu(fileName = "UMPSettings", menuName = "UMP/UMPSettings")]
	public class UMPSettings : ScriptableObject
	{
		public enum Platforms
		{
			None = 1,
			Win = 2,
			Mac = 4,
			Linux = 8,
			WebGL = 0x10,
			Android = 0x20,
			iOS = 0x40
		}

		public enum BitModes
		{
			x86,
			x86_64
		}

		private const string MAC_APPS_FOLDER_NAME = "/Applications";

		private const string MAC_VLC_PACKAGE_NAME = "vlc.app";

		private const string MAC_LIBVLC_PACKAGE_NAME = "libvlc.bundle";

		private const string MAC_PACKAGE_LIB_PATH = "Contents/MacOS/lib";

		private const string WIN_REG_KEY_X86 = "SOFTWARE\\WOW6432Node\\VideoLAN\\VLC";

		private const string WIN_REG_KEY_X86_64 = "SOFTWARE\\VideoLAN\\VLC";

		public const string SETTINGS_FILE_NAME = "UMPSettings";

		public const string ASSET_NAME = "UniversalMediaPlayer";

		public const string LIB_VLC_NAME = "libvlc";

		public const string LIB_VLC_CORE_NAME = "libvlccore";

		public const string DESKTOP_CATEGORY_NAME = "Desktop";

		public const string PLUGINS_FOLDER_NAME = "Plugins";

		private const string ASSETS_FOLDER_NAME = "Assets";

		private static string[] LIN_APPS_FOLDERS_PATHS = new string[3]
		{
			"/usr/lib",
			"/usr/lib64",
			"/usr/lib/x86_64-linux-gnu/"
		};

		private static UMPSettings _instance;

		[SerializeField]
		private string _assetPath = Path.Combine("Assets", "UniversalMediaPlayer").Replace("\\", "/");

		[SerializeField]
		private bool _useAudioSource;

		[SerializeField]
		private bool _useExternalLibraries;

		[SerializeField]
		private string _librariesPath = string.Empty;

		[SerializeField]
		private PlayerOptionsAndroid.PlayerTypes _playersAndroid = (PlayerOptionsAndroid.PlayerTypes)3;

		[SerializeField]
		private PlayerOptionsIPhone.PlayerTypes _playersIPhone = (PlayerOptionsIPhone.PlayerTypes)3;

		[SerializeField]
		private string[] _androidExportedPaths = new string[0];

		[SerializeField]
		private string _youtubeDecryptFunction = "\\bc\\s*&&\\s*d\\.set\\([^,]+\\s*,[^(]*\\(([a-zA-Z0-9$]+)\\(";

		public static UMPSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<UMPSettings>("UMPSettings");
					if (_instance == null)
					{
						Debug.LogError(string.Format("[UMPSetting] Could not find settings file '{0}' in UMP 'Resources' folder. Try to correctly import UMP asset to your project or create the new settings file by click with right mouse on UMP 'Resources' folder and choose: 'Create'->'UMP'->'UMPSettings'.", "UMPSettings"));
					}
					if (Application.isEditor && !_instance.IsValidAssetPath)
					{
						Debug.LogError("[UMPSetting] Asset path is not correct, please check the settings file in UMP 'Resources' folder.");
					}
					if ((RuntimePlatform & Desktop) == RuntimePlatform && !ContainsLibVLC(_instance.LibrariesPath))
					{
						Debug.LogError("[UMPSetting] Can't find LibVLC libraries, try to check the settings file in UMP 'Resources' folder.");
					}
				}
				return _instance;
			}
		}

		public string AssetPath
		{
			get
			{
				return _assetPath;
			}
			set
			{
				_assetPath = value;
			}
		}

		public bool IsValidAssetPath => Directory.Exists(_assetPath) && Directory.GetFiles(_assetPath).Length > 0;

		public bool UseAudioSource => _useAudioSource;

		public bool UseExternalLibraries => _useExternalLibraries;

		public string LibrariesPath
		{
			get
			{
				string librariesPath = GetLibrariesPath(RuntimePlatform, _useExternalLibraries);
				if (ContainsLibVLC(librariesPath))
				{
					_librariesPath = librariesPath;
				}
				else if (!ContainsLibVLC(_librariesPath))
				{
					_librariesPath = string.Empty;
				}
				return _librariesPath;
			}
		}

		public PlayerOptionsAndroid.PlayerTypes PlayersAndroid
		{
			get
			{
				return _playersAndroid;
			}
			set
			{
				_playersAndroid = value;
			}
		}

		public PlayerOptionsIPhone.PlayerTypes PlayersIPhone
		{
			get
			{
				return _playersIPhone;
			}
			set
			{
				_playersIPhone = value;
			}
		}

		public string[] AndroidExportedPaths
		{
			get
			{
				return _androidExportedPaths;
			}
			set
			{
				_androidExportedPaths = value;
			}
		}

		public string YoutubeDecryptFunction => _youtubeDecryptFunction;

		public static Platforms Desktop => (Platforms)14;

		public static Platforms Mobile => (Platforms)96;

		public static BitModes EditorBitMode => (IntPtr.Size != 4) ? BitModes.x86_64 : BitModes.x86;

		public static string EditorBitModeFolderName => Enum.GetName(typeof(BitModes), EditorBitMode);

		public static Platforms RuntimePlatform
		{
			get
			{
				Platforms result = Platforms.None;
				RuntimePlatform platform = Application.platform;
				if (platform == UnityEngine.RuntimePlatform.WindowsEditor || Application.platform == UnityEngine.RuntimePlatform.WindowsPlayer)
				{
					result = Platforms.Win;
				}
				if (platform == UnityEngine.RuntimePlatform.OSXEditor || Application.platform == UnityEngine.RuntimePlatform.OSXPlayer)
				{
					result = Platforms.Mac;
				}
				if (platform == UnityEngine.RuntimePlatform.LinuxPlayer || Application.platform == (RuntimePlatform)16)
				{
					result = Platforms.Linux;
				}
				if (platform == UnityEngine.RuntimePlatform.WebGLPlayer)
				{
					result = Platforms.WebGL;
				}
				if (platform == UnityEngine.RuntimePlatform.Android)
				{
					result = Platforms.Android;
				}
				if (platform == UnityEngine.RuntimePlatform.IPhonePlayer)
				{
					result = Platforms.iOS;
				}
				return result;
			}
		}

		public static string RuntimePlatformFolderName => PlatformFolderName(RuntimePlatform);

		public string GetLibrariesPath(Platforms platform, bool externalSpace)
		{
			string text = string.Empty;
			if (platform != Platforms.None)
			{
				if (!externalSpace)
				{
					if (Application.isEditor)
					{
						text = Path.Combine(_assetPath, "Plugins");
						text = Path.Combine(text, PlatformFolderName(platform));
						if (platform == Platforms.Win || platform == Platforms.Mac || platform == Platforms.Linux)
						{
							text = Path.Combine(text, EditorBitModeFolderName);
						}
					}
					else
					{
						text = Path.Combine(Application.dataPath, "Plugins");
						if (platform == Platforms.Linux)
						{
							text = Path.Combine(text, EditorBitModeFolderName);
						}
					}
					if (platform == Platforms.Mac)
					{
						text = Path.Combine(text, Path.Combine("libvlc.bundle", "Contents/MacOS/lib"));
					}
					if (!Directory.Exists(text))
					{
						text = string.Empty;
					}
				}
				else
				{
					if (platform == Platforms.Win)
					{
						text = NativeInterop.ReadLocalRegKey((EditorBitMode != 0) ? "SOFTWARE\\VideoLAN\\VLC" : "SOFTWARE\\WOW6432Node\\VideoLAN\\VLC", "InstallDir");
					}
					if (platform == Platforms.Mac)
					{
						DirectoryInfo directoryInfo = new DirectoryInfo("/Applications");
						DirectoryInfo[] directories = directoryInfo.GetDirectories();
						DirectoryInfo[] array = directories;
						foreach (DirectoryInfo directoryInfo2 in array)
						{
							if (directoryInfo2.FullName.ToLower().Contains("vlc.app"))
							{
								text = Path.Combine(directoryInfo2.FullName, "Contents/MacOS/lib");
							}
						}
					}
					if (platform == Platforms.Linux)
					{
						DirectoryInfo directoryInfo3 = null;
						string[] lIN_APPS_FOLDERS_PATHS = LIN_APPS_FOLDERS_PATHS;
						foreach (string text2 in lIN_APPS_FOLDERS_PATHS)
						{
							if (Directory.Exists(text2))
							{
								directoryInfo3 = new DirectoryInfo(text2);
							}
							if (directoryInfo3 == null)
							{
								continue;
							}
							FileInfo[] files = directoryInfo3.GetFiles();
							FileInfo[] array2 = files;
							foreach (FileInfo fileInfo in array2)
							{
								if (fileInfo.FullName.ToLower().Contains("libvlc"))
								{
									text = text2;
								}
							}
						}
					}
				}
				if (!text.Equals(string.Empty))
				{
					text = Path.GetFullPath(text + Path.AltDirectorySeparatorChar);
				}
			}
			return text;
		}

		public string[] GetInstalledPlatforms(Platforms category)
		{
			List<string> list = new List<string>();
			foreach (int value in Enum.GetValues(typeof(Platforms)))
			{
				string librariesPath = GetLibrariesPath((Platforms)value, externalSpace: false);
				if (!string.IsNullOrEmpty(librariesPath))
				{
					string[] files = Directory.GetFiles(librariesPath);
					foreach (string path in files)
					{
						if (Path.GetFileName(path).Contains("UniversalMediaPlayer"))
						{
							if ((category & Desktop) == Desktop && (value == 2 || value == 4 || value == 8) && !list.Contains("Desktop"))
							{
								list.Add("Desktop");
							}
							if ((category & Mobile) == Mobile && value == 32 && !list.Contains(Platforms.Android.ToString()))
							{
								list.Add(Platforms.Android.ToString());
							}
							if ((category & Mobile) == Mobile && value == 64 && !list.Contains(Platforms.iOS.ToString()))
							{
								list.Add(Platforms.iOS.ToString());
							}
							if ((category & Desktop) == Desktop && value == 16 && !list.Contains(Platforms.WebGL.ToString()))
							{
								list.Add(Platforms.WebGL.ToString());
							}
							break;
						}
					}
				}
			}
			return list.ToArray();
		}

		public static string PlatformFolderName(Platforms platform)
		{
			if (platform != Platforms.None)
			{
				return platform.ToString();
			}
			return string.Empty;
		}

		public static bool ContainsLibVLC(string path)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path);
				int num = 0;
				string arg = string.Empty;
				switch (RuntimePlatform)
				{
				case Platforms.Win:
					arg = "dll";
					break;
				case Platforms.Mac:
					arg = "dylib";
					break;
				case Platforms.Linux:
					arg = "so";
					break;
				}
				string[] array = files;
				foreach (string text in array)
				{
					if (text.EndsWith(string.Format("{0}.{1}", "libvlc", arg)) || text.EndsWith(string.Format("{0}.{1}", "libvlccore", arg)))
					{
						num++;
					}
				}
				if (num >= 2)
				{
					result = true;
				}
			}
			return result;
		}
	}
}
