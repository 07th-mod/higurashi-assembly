using Assets.Scripts.Core.AssetManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.SteamWorks
{
	public static class Achievements
	{
		public static Achievement_t[] achievements;

		public static void Load()
		{
			List<string> list = File.ReadAllLines(Path.Combine(Application.streamingAssetsPath, Path.Combine("Data", "achievements.txt"))).ToList();
			Debug.Log("Loading achievements.");
			achievements = new Achievement_t[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				achievements[i] = new Achievement_t(list[i], "", "");
			}
		}
	}
}
