﻿using Assets.Scripts.Core;
using MOD.Scripts.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MOD.Scripts.Core
{
	// Refer to https://partner.steamgames.com/doc/features/cloud#steam_auto-cloud
	public class SteamCloudSettings
	{
		public bool isSteamInstall { get; set; } = false;
		public int quotaBytes { get; set; } = 50000000;
		public int maxNumFiles { get; set; } = 200;
		public bool windows { get; set; } = true;
		public bool macOS { get; set; } = true;
		public bool linux { get; set; } = true;
	}


	public static class MODSteamCloudManager
	{
		class SteamCloudStatus
		{
			public bool steamCloudSupported;
			public bool steamCloudIsFull;
			public string humanReadableStatus;

			public SteamCloudStatus(bool steamCloudSupported, bool steamCloudIsFull, string humanReadableStatus)
			{
				this.steamCloudSupported = steamCloudSupported;
				this.steamCloudIsFull = steamCloudIsFull;
				this.humanReadableStatus = humanReadableStatus;
			}
		}

		// This file is to be generated by the installer?
		private static readonly string SteamCloudSettingsJSONPath = Path.Combine(MODSystem.BaseDirectory, "steam.json");

		static SteamCloudSettings SteamCloudSettings = new SteamCloudSettings();

		public static void LoadSteamSettings()
		{
			Logger.Log($"Loading steam settings from {SteamCloudSettingsJSONPath}");
			if (!File.Exists(SteamCloudSettingsJSONPath))
			{
				Logger.Log($"Can't find {SteamCloudSettingsJSONPath}");
				return;
			}

			try
			{
				using (var reader = new JsonTextReader(new StreamReader(SteamCloudSettingsJSONPath)))
				{
					SteamCloudSettings = JsonSerializer.Create(new JsonSerializerSettings()).Deserialize<SteamCloudSettings>(reader);
				}

				using (var writer = new JsonTextWriter(new StreamWriter(SteamCloudSettingsJSONPath + ".out")))
				{
					JsonSerializer.Create(new JsonSerializerSettings()).Serialize(writer, SteamCloudSettings);
				}
			}
			catch (Exception e)
			{
				// Steam Cloud Warning is not critical for the mod, so just ignore any errors here.
				Logger.Log(e.ToString());
			}
		}
		public static void ShowSteamCloudUsage()
		{
			try
			{
				// Only show steam cloud status if we think steam cloud is in use on this system
				// Note that it's not always so clear cut - for example, if steam is not running, and you
				// launch the modded game, Steam Cloud won't sync. But we still show the message for this
				// case, as the user may open Steam later on, causing a sync to occur.
				SteamCloudStatus status = MODSteamCloudManager.GetSteamCloudStatus();
				if (status.steamCloudSupported)
				{
					MODToaster.Show(status.humanReadableStatus);
				}
			}
			catch(Exception)
			{
				string msg = "ShowSteamCloudUsage() threw an exception!";
				MODToaster.Show(msg);
				MODLogger.Log(msg, true);
			}
		}

		private static bool PlatformSupported()
		{
			switch (MODUtility.GetPlatform())
			{
				case MODUtility.Platform.Windows:
					if (SteamCloudSettings.windows)
					{
						return true;
					}
					break;
				case MODUtility.Platform.MacOS:
					if (SteamCloudSettings.macOS)
					{
						return true;
					}
					break;
				case MODUtility.Platform.Linux:
					if (SteamCloudSettings.linux)
					{
						return true;
					}
					break;
			}
			return false;
		}

		private static SteamCloudStatus GetSteamCloudStatus()
		{
			if (!SteamCloudSettings.isSteamInstall)
			{
				return new SteamCloudStatus(false, false, "Game is not Steam, so Steam Cloud not supported");
			}

			if (!PlatformSupported())
			{
				return new SteamCloudStatus(false, false, "Steam Cloud only supported on" + $"Win: {SteamCloudSettings.windows} Linux: {SteamCloudSettings.linux} Mac: {SteamCloudSettings.macOS}");
			}

			// Count number of files and total space used
			// Subdirectories are not synchronized so just ignore them
			string[] files = Directory.GetFiles(MGHelper.GetSaveFolder());
			int fileCount = files.Length;
			long totalSizeBytes = 0;
			foreach (string path in files)
			{
				try
				{
					FileInfo fileInfo = new FileInfo(path);
					totalSizeBytes += fileInfo.Length;
				}
				catch (Exception e)
				{

				}
			}

			bool steamCloudIsFull = fileCount > SteamCloudSettings.maxNumFiles || totalSizeBytes > SteamCloudSettings.quotaBytes;

			float fileCountUsed = (float)fileCount / (float)SteamCloudSettings.maxNumFiles;
			float spaceUsed = (float)totalSizeBytes / (float)SteamCloudSettings.quotaBytes;
			float maxUsed = Math.Max(fileCountUsed, spaceUsed);

			string humanReadableStatus = "Steam Cloud Usage:" + $" {Math.Round(maxUsed * 100):0.#}%";

			if (steamCloudIsFull)
			{
				humanReadableStatus += "\nPlease delete some saves!";
			}

			return new SteamCloudStatus(true, steamCloudIsFull, humanReadableStatus);
		}
	}
}