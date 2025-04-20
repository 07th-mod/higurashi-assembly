using MOD.Scripts.Core.Config;
using MOD.Scripts.Core.Scene;
using MOD.Scripts.Core.TextWindow;
using MOD.Scripts.UI;
using MOD.Scripts.UI.Tips;
using UnityEngine;
using System.IO;

using System.Runtime.InteropServices;
using System;

namespace MOD.Scripts.Core
{
	public class MODSystem
	{
		[DllImport("ntdll.dll")]
		private static extern string wine_get_version();

		public readonly MODMainUIController modMainUIController = new MODMainUIController();

		public readonly MODSceneController modSceneController = new MODSceneController();

		public readonly MODTextController modTextController = new MODTextController();

		public readonly MODTextureController modTextureController = fixedMODTextureControllerInstance;

		private static readonly MODTextureController fixedMODTextureControllerInstance = new MODTextureController();

		private static MODSystem _instance;
		public static MODSystem instance => _instance ?? (_instance = new MODSystem());

		public readonly MODConfig modConfig = fixedMODConfigInstance;

		private static readonly MODConfig fixedMODConfigInstance = MODConfigManager.Read();

		public readonly MODTipsController modTipsController = fixedMODTipsControllerInstance;

		private static readonly MODTipsController fixedMODTipsControllerInstance = new MODTipsController();

		// Wine detection occurs once when this class is first instantiated
		public readonly bool IsWine = false;

		private bool DetectWine(out string wineVersionString)
		{
			if (Application.platform != RuntimePlatform.WindowsPlayer)
			{
				// If Unity does not report OS as Windows, we can assume we are not running Wine/Proton,
				// as Wine should trick Unity that it is running under Windows
				wineVersionString = null;
				return false;
			}

			try
			{
				// If we can call the wine_get_version() function, then we are running under Wine/Proton
				wineVersionString = wine_get_version();
				return wineVersionString != null;
			}
			catch(EntryPointNotFoundException)
			{
				// If the function is not found, we are running under Windows
				wineVersionString = null;
				return false;
			}
			catch (Exception ex)
			{
				Debug.Log($"IsWine() Exception - {ex}");
				wineVersionString = null;
				return false;
			}
		}

		public MODSystem()
		{
			if (DetectWine(out string wineVersionString))
			{
				IsWine = true;
				Debug.Log($"WINE: Wine/Proton Detected, Version {wineVersionString} under {SystemInfo.operatingSystem}");
			}
			else
			{
				Debug.Log($"WINE: No Wine/Proton, running regular Windows version {SystemInfo.operatingSystem}");
			}
		}

		public static string BaseDirectory
		{
			get
			{
				if (Application.platform == RuntimePlatform.OSXPlayer)
				{
					return Path.Combine(Application.dataPath, "Resources/Data");
				}
				else
				{
					return Application.dataPath;
				}
			}
		}
	}
}
