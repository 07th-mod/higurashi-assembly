using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;
using Assets.Scripts.Core.AssetManagement;

namespace MOD.Debugging
{
	internal class MODDebugSpriteMapping
	{
		private static string LastVoiceLoadedFromSaveFile = "Load Not Performed Yet";

		private static string JSONLoadStatus = "";

		private static string SpriteMappingLookupArguments = "No sprite looked up yet";
		private static string SpriteMappingLookupStatus = "No sprite looked up yet";

		public static void OnGUISpriteMapping()
		{
			// TODO: add button to enable debugging?

			Label("---- Sprite Mapping ----");
			Label($"JSON: {JSONLoadStatus}");

			Label("-- Last Voice --");
			Label($"Last Voice (Runtime): {AssetManager.Instance.lastVoiceFromMODPlayVoiceLSNoExt}");
			Label($"Last Voice Loaded From Save File: {LastVoiceLoadedFromSaveFile}");

			Label("-- Mapped Sprite Status --");
			Label($"Args: {SpriteMappingLookupArguments}");
			Label($"Status: {SpriteMappingLookupStatus}");
		}

		public static void RecordJSONLoadStatus(string mappingPath, string mappingStatus)
		{
			if(JSONLoadStatus.Length < 100)
			{
				JSONLoadStatus += $"{mappingPath}:{mappingStatus}";
			}
		}

		/// <summary>
		/// baseSpritePath is the path the sprite would be loaded from, if mapping was not performed
		/// </summary>
		public static void RecordSpriteMappingLookupArguments(string baseSpritePath, string scriptNameNoExt, string maybeLastPlayedVoice, string spritePathNoExt)
		{
			SpriteMappingLookupArguments = $"{baseSpritePath} - {scriptNameNoExt} - {maybeLastPlayedVoice ?? "[null]"} - {spritePathNoExt}";
		}

		public static void RecordSuccessfulLookupResult(string sourcePath, string mappedPath, string resultDescription)
		{
			SpriteMappingLookupStatus = $"{sourcePath}->{mappedPath}: {resultDescription}";
		}

		public static void RecordFailedLookupResult(string sourcePath, string resultDescription)
		{
			SpriteMappingLookupStatus = $"{sourcePath}-><Failed>: {resultDescription}";
		}

		public static void RecordLastVoiceLoadedFromSaveFile(string lastVoiceFromSaveFile)
		{
			LastVoiceLoadedFromSaveFile = lastVoiceFromSaveFile;
		}
	}
}
