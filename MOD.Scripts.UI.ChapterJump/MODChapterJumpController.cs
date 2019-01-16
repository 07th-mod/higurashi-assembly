using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace MOD.Scripts.UI.ChapterJump
{
	public class MODChapterJumpController
	{
		private static bool initialized = false;
		private static Dictionary<int, List<ChapterJumpEntry>> cachedChapterJumps;
		private static readonly string ChapterJumpFilePath = Path.Combine(MODSystem.BaseDirectory, "chapterjump.json");

		public class ChapterJumpEntry
		{
			public string English;
			public string Japanese;
			public int ChapterNumber;
			public int ArcNumber;
			public string BlockName;
			public string FileName;
		}

		public static List<ChapterJumpEntry> ChapterJumpsOrNull
		{
			get
			{
				if (!initialized)
				{
					initialized = true;
					if (File.Exists(ChapterJumpFilePath))
					{
						try
						{
							using (var reader = new JsonTextReader(new StreamReader(ChapterJumpFilePath)))
							{
								cachedChapterJumps = JsonSerializer.Create(new JsonSerializerSettings()).Deserialize<Dictionary<int, List<ChapterJumpEntry>>>(reader);
							}
						}
						catch (System.Exception e)
						{
							Debug.Log("Failed to parse chapter jump JSON, falling back to hardcoded chapter jumps: " + e.Message);
						}
					}
					else
					{
						Debug.Log("No chapter jump JSON available, falling back to hardcoded chapter jumps");
					}
				}
				if (cachedChapterJumps == null)
				{
					return null;
				}
				int arc;
				try
				{
					arc = BurikoMemory.Instance.GetFlag("LConsoleArc").IntValue();
				}
				catch
				{
					arc = 0;
				}
				cachedChapterJumps.TryGetValue(arc, out var entry);
				return entry;
			}
		}
	}
}
