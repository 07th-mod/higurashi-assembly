using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;
using Assets.Scripts.Core.AssetManagement;
using MOD.Scripts.UI;
using System.Linq;

namespace MOD.Debugging
{
	internal class MODDebugSpriteMapping
	{
		private static int id = 0;

		private static string LastVoiceLoadedFromSaveFile = "Load Not Performed Yet";

		private static List<string> JSONLoadStatuses = new List<string>();

		private static Queue<string> SpriteMappingLookupDebug = new Queue<string>();

		public static void OnGUISpriteMapping()
		{
			// TODO: add button to enable debugging?

			Label("---- Sprite Mapping ----");
			foreach(string s in JSONLoadStatuses)
			{
				Label(s);
			}

			Label("-- Last Voice --");
			Label($"Last Voice (Runtime): {AssetManager.Instance.lastVoiceFromMODPlayVoiceLSNoExt ?? "[null]"}");
			Label($"Last Voice Loaded From Save File: {LastVoiceLoadedFromSaveFile}");

			Label("-- Mapped Sprite Status --");
			foreach(string s in SpriteMappingLookupDebug.Reverse().ToList())
			{
				Label(s);
			}
		}

		public static void RecordJSONLoadStatus(string mappingPath, string mappingFile, string mappingStatus)
		{
			if(JSONLoadStatuses.Count < 10)
			{
				JSONLoadStatuses.Add($"StreamingAssets/{mappingPath}/{mappingFile}: {mappingStatus}");
			}
			else
			{
				MODToaster.Show("MODDebugSpriteMapping: Too many Sprite Mapping JSON Statuses - not showing any more");
			}
		}

		private static void QueueSpriteLookupDebug(string s)
		{
			if(SpriteMappingLookupDebug.Count > 10)
			{
				SpriteMappingLookupDebug.Dequeue();
			}

			SpriteMappingLookupDebug.Enqueue(s);
		}

		/// <summary>
		/// baseSpritePath is the path the sprite would be loaded from, if mapping was not performed
		/// </summary>
		public static void RecordLookup(string baseSpritePath, string scriptNameNoExt, string maybeLastPlayedVoice, string sourcePath, string maybeMappedPath, string resultDescription)
		{
			QueueSpriteLookupDebug($"{id++}: {baseSpritePath} - {scriptNameNoExt} - {maybeLastPlayedVoice ?? "[null]"} | [{sourcePath}] > [{maybeMappedPath ?? ("Failed")}]: {resultDescription}");
		}

		public static void RecordLastVoiceLoadedFromSaveFile(string lastVoiceFromSaveFile)
		{
			LastVoiceLoadedFromSaveFile = lastVoiceFromSaveFile;
		}
	}
}
