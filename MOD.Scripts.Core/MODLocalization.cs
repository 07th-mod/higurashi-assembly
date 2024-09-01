using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.Core;
using MOD.Scripts.UI;
using System.Globalization;
using Assets.Scripts.UI.TitleScreen;

namespace MOD.Scripts.Core.Localization
{
	public class LocalizationEntry
	{
		public const string defaultValue = "#NOT SET#";

		public string comment = defaultValue;
		public string text = defaultValue;

		// Japanese Mode Note: For now, if no japanese text specified, just use the english text instead.
		private string _textJP = null;
		public string textJP
		{
			get
			{
				return _textJP ?? text;
			}
			set
			{
				_textJP = value;
			}
		}

		public bool TextHasValue() => text != null && text != defaultValue;
	}

	public class LocalizationInfo
	{
		public Dictionary<string, LocalizationEntry> allChapters;

		public LocalizationInfo()
		{
			allChapters = new Dictionary<string, LocalizationEntry>();
		}
	}

	public partial class Loc
	{
		private static bool initializeAttempted;
		private static readonly LocalizationEntry notInitializedEntry;
		private static readonly LocalizationEntry defaultEntry;
		private static readonly LocalizationInfo fallbackInfo;
		private static LocalizationInfo info;

		// These are used when displaying datetimes (eg d.Time.ToString(Loc.DateTimeFormat, Loc.cultureInfo))
		public static CultureInfo cultureInfo = CultureInfo.CurrentCulture;
		public static string dateTimeFormat = "ddd MMM dd, yyyy h:mm tt";

		static Loc()
		{
			// This entry is shown if you try to access this class too early (before GameSystem has inititalized).
			notInitializedEntry = new LocalizationEntry()
			{
				comment = "This shows if LoadFromJSON() has not been called yet",
				text = "#NOT INITIALIZED#",
			};

			defaultEntry = new LocalizationEntry()
			{
				comment = "This localization entry is missing!",
				text = "#TEXT MISSING#",
			};

			info = new LocalizationInfo();
			fallbackInfo = new LocalizationInfo();

			addFallbackEntry(
				name: "no-tips-available",
				comment: "This text appears at the top of the tips menu when there are no tips available",
				text: "No new tips available",
				textJP: "入手ＴＩＰＳはありません"
			);

			// Hou+ Music Box Text
			addFallbackEntry(
				name: "MusicBoxRepeat",
				comment: "Music box button (hou+)",
				text: "Repeat",
				textJP: "1曲リピート"
			);

			addFallbackEntry(
				name: "MusicBoxShuffle",
				comment: "Music box button (hou+)",
				text: "Shuffle",
				textJP: "気まぐれ演奏"
			);

			addFallbackEntry(
				name: "MusicBoxPlayAll",
				comment: "Music box button (hou+)",
				text: "Play All",
				textJP: "全曲演奏"
			);

			addFallbackEntry(
				name: "MusicBoxTitle",
				comment: "Music box button (hou+)",
				text: "[Title]",
				textJP: "【Title】"
			);
		}

		private static void addFallbackEntry(string name, string comment, string text, string textJP)
		{
			fallbackInfo.allChapters[name] = new LocalizationEntry()
			{
				comment = comment,
				text = text,
				textJP = textJP,
			};
		}

		private static void addFallbackEntry(string name, string text)
		{
			addFallbackEntry(name, "#TEXT MISSING#", text, null);
		}

		private static LocalizationEntry GetEntry(string name)
		{
			bool _ = GetEntry(name, out LocalizationEntry entry);
			return entry;
		}

		// Returns true if any entry was found at all (even if it was a fallback entry)
		// Returns false if no entry was found, or if not yet initialized (entry will
		// be populated with the 'default' entry showing #TEXT MISSING# or #NOT INITIALIZED# correspondingly)
		public static bool GetEntry(string name, out LocalizationEntry entry)
		{
			if (!initializeAttempted)
			{
				entry = notInitializedEntry;
				return false;
			}

			if (info.allChapters.TryGetValue(name, out entry))
			{
				return true;
			}

			if (fallbackInfo.allChapters.TryGetValue(name, out entry))
			{
				return true;
			}

			entry = defaultEntry;
			return false;
		}

		public static string Get(string name, string defaultValue)
		{
			if(!GetEntry(name, out LocalizationEntry entry) && !string.IsNullOrEmpty(defaultValue))
			{
				addFallbackEntry(name, defaultValue);
				entry = GetEntry(name);
			}

			return GameSystem.Instance.UseEnglishText ? entry.text : entry.textJP;
		}

		public static string Get(string name)
		{
			return Get(name, null);
		}

		public static void LoadFromJSON()
		{
			initializeAttempted = true;
			string localizationPath = Path.Combine(MODSystem.BaseDirectory, "localization.json");

			// For Hou+, also support loading from "HigurashiEp10_Data\StreamingAssets\Data" (most of the .json are in that folder)
			if (!File.Exists(localizationPath))
			{
				localizationPath = Path.Combine(Application.streamingAssetsPath, "Data", "localization.json");
			}

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

			try
			{
				if (Loc.GetEntry("CultureInfo", out LocalizationEntry localizationEntry))
				{
					// Make sure a value for 'text' is provided before using it
					if (localizationEntry.TextHasValue())
					{
						Loc.cultureInfo = new CultureInfo(localizationEntry.text);
					}
				}

				if (Loc.GetEntry("DateTimeFormat", out LocalizationEntry timeFormatEntry))
				{
					if (timeFormatEntry.TextHasValue())
					{
						Loc.dateTimeFormat = timeFormatEntry.text;
					}
				}

				if (Loc.GetEntry("HouPlusWideMenuButtons", out LocalizationEntry houWideMenuButtons))
				{
					if (houWideMenuButtons.TextHasValue())
					{
						if(MODUtility.TryParseInvariantCulture(houWideMenuButtons.text, out float opacity))
						{
							TitleScreenButton.UseHoverSpriteOnlyForTranslators(opacity > 0, opacity);
						}
					}
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError($"MODLocalizationController(): Failed to set CultureInfo or DateTimeFormat: {e.Message}");
				MODToaster.Show("localization.json cultureInfo fail - check logs");
			}
		}
	}
}
