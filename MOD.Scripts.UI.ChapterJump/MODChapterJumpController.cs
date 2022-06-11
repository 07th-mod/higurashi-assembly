using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

namespace MOD.Scripts.UI.ChapterJump
{
	public class MODChapterJumpController
	{
		private static bool initialized = false;
		private static List<ChapterJumpEntry> cachedChapterJumps;
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
								var loadedJumps = JsonSerializer.Create(new JsonSerializerSettings()).Deserialize<Dictionary<int, List<ChapterJumpEntry>>>(reader).ToList();
								loadedJumps.Sort( (a, b) => a.Key.CompareTo(b.Key) );
								cachedChapterJumps = new List<ChapterJumpEntry>();
								foreach (var arc in loadedJumps)
								{
									foreach (var chapter in arc.Value)
									{
										chapter.ArcNumber = arc.Key;
										cachedChapterJumps.Add(chapter);
									}
								}
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
				return cachedChapterJumps;
			}
		}
	}
}
