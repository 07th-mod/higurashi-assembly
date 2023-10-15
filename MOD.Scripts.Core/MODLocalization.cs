using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.Core;
using MOD.Scripts.UI;

namespace MOD.Scripts.Core
{
	public class LocalizationEntry
	{
		public string comment;
		public string text;
		public string textJP;
	}

	public class LocalizationInfo
	{
		public Dictionary<string, LocalizationEntry> allChapters;

		public LocalizationInfo()
		{
			allChapters = new Dictionary<string, LocalizationEntry>();
		}
	}

	public class Loc
	{
		private static bool initializeAttempted;
		private static readonly LocalizationEntry notInitializedEntry;
		private static readonly LocalizationEntry defaultEntry;
		private static readonly LocalizationInfo fallbackInfo;
		private static LocalizationInfo info;

		static Loc()
		{
			// This entry is shown if you try to access this class too early (before GameSystem has inititalized).
			notInitializedEntry = new LocalizationEntry()
			{
				comment = "This shows if LoadFromJSON() has not been called yet",
				text = "#NOT INITIALIZED#",
				textJP = "#NOT INITIALIZED#",
			};

			defaultEntry = new LocalizationEntry()
			{
				comment = "This localization entry is missing!",
				text = "#TEXT MISSING#",
				textJP = "#TEXT MISSING#",
			};

			info = new LocalizationInfo();
			fallbackInfo = new LocalizationInfo();

			void addFallbackEntry(string name, string comment, string text, string textJP)
			{
				fallbackInfo.allChapters[name] = new LocalizationEntry()
				{
					comment = comment,
					text = text,
					textJP = textJP,
				};
			}

			addFallbackEntry(
				name: "no-tips-available",
				comment: "This text appears at the top of the tips menu when there are no tips available",
				text: "No new tips available",
				textJP: "入手ＴＩＰＳはありません"
			);
		}

		private static LocalizationEntry GetEntry(string name)
		{
			if(!initializeAttempted)
			{
				return notInitializedEntry;
			}

			if (info.allChapters.TryGetValue(name, out LocalizationEntry entry))
			{
				return entry;
			}

			if (fallbackInfo.allChapters.TryGetValue(name, out LocalizationEntry fallbackEntry))
			{
				return fallbackEntry;
			}

			return defaultEntry;
		}

		public static string Get(string name)
		{
			LocalizationEntry entry = GetEntry(name);
			return GameSystem.Instance.UseEnglishText ? entry.text : entry.textJP;
		}

		public static void LoadFromJSON()
		{
			initializeAttempted = true;
			string localizationPath = Path.Combine(MODSystem.BaseDirectory, "localization.json");

			if (!File.Exists(localizationPath))
			{
				Debug.Log($"MODLocalizationController(): No localization file at [{localizationPath}] - will use hardcoded localization");
				return;
			}

			try
			{
				using (var reader = new JsonTextReader(new StreamReader(localizationPath)))
				{
					info = JsonSerializer.Create(new JsonSerializerSettings()).Deserialize<LocalizationInfo>(reader);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError($"MODLocalizationController(): Failed to read localization file at [{localizationPath}]: {e.Message}");
				MODToaster.Show("localization.json fail - check logs");
			}
		}
	}
}
