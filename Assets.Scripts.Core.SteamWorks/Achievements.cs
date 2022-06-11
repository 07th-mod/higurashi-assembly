using Assets.Scripts.Core.AssetManagement;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.SteamWorks
{
	public static class Achievements
	{
		public static Achievement_t[] achievements;

		public static void Load()
		{
			List<string> list = AssetManager.Instance.LoadTextDataLines("achievements.txt");
			Debug.Log("Loading achievements.");
			achievements = new Achievement_t[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				achievements[i] = new Achievement_t(list[i], "", "");
			}
		}
	}
}
