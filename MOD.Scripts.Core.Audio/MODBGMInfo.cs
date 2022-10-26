using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MOD.Scripts.UI;
using Assets.Scripts.Core;

namespace MOD.Scripts.Core
{
	public class BGMInfo
	{
		/// <summary>
		/// A comment to store misc info about the BGM (any errata, or changes made to the BGM)
		/// </summary>
		public string comment;
		/// <summary>
		/// The English/Japanese name of the song
		/// </summary>
		public string name;
		public string source;
		public string url;
	}

	public class BGMInfoDict
	{
		public Dictionary<string, BGMInfo> bgmList;

		public BGMInfoDict()
		{
			bgmList = new Dictionary<string, BGMInfo>();
		}
	}

	public class MODBGMInfo
	{
		/// <summary>
		/// Dictionary of (filepath without extension relative to streamingassets) -> BGMInfo
		/// </summary>
		private static Dictionary<string, BGMInfo> bgmDictionary;
		private static Dictionary<string, bool> loadedFolders;

		static MODBGMInfo()
		{
			bgmDictionary = new Dictionary<string, BGMInfo>();
			loadedFolders = new Dictionary<string, bool>();
		}

		public static string GetBGMName(string streamingAssetsRelativePath)
		{
			string pathWithoutExtension = Path.Combine(Path.GetDirectoryName(streamingAssetsRelativePath), Path.GetFileNameWithoutExtension(streamingAssetsRelativePath));
			if(bgmDictionary.TryGetValue(pathWithoutExtension, out BGMInfo info))
			{
				return info.name;
			}
			else
			{
				return GameSystem.Instance.ChooseJapaneseEnglish("不明 BGM", "Unknown BGM");
			}
		}

		// Load the bgm information from the bgmInfo.json in a given BGM Folder (located in the StreamingAssets folder)
		public static void LoadFromJSON(string BGMFolder)
		{
			// Skip already loaded folders
			if(loadedFolders.ContainsKey(BGMFolder))
			{
				return;
			}
			loadedFolders[BGMFolder] = true;

			string path = Path.Combine(Application.streamingAssetsPath, Path.Combine(BGMFolder, "bgmInfo.json"));

			if (!File.Exists(path))
			{
				Debug.Log($"MODBGMInfo(): No bgmInfo.json at [{path}] - BGM will just show as filenames");
				return;
			}

			try
			{
				using (var reader = new JsonTextReader(new StreamReader(path)))
				{
					BGMInfoDict bgmInfoDict = JsonSerializer.Create(new JsonSerializerSettings()).Deserialize<BGMInfoDict>(reader);
					foreach(KeyValuePair<string, BGMInfo> kvp in bgmInfoDict.bgmList)
					{
						// Allow missing comment in JSON
						if(kvp.Value.comment == null)
						{
							kvp.Value.comment = "";
						}

						string relativePathWithoutExtension = Path.Combine(BGMFolder, kvp.Key);
						bgmDictionary[relativePathWithoutExtension] = kvp.Value;
					}
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError($"MODBGMInfo(): Failed to read bgm info file at [{path}]: {e.Message}");
				MODToaster.Show("bgmInfo.json fail - check logs");
			}
		}
	}
}
