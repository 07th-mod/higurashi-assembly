using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.SteamWorks
{
	public static class Achievements
	{
		private static Achievement_t[] _achievements;

		public static Achievement_t[] achievements
		{
			get
			{
				if(_achievements == null)
				{
					Load();
				}

				return _achievements;
			}
			set
			{
				_achievements = value;
			}
		}

		public static void Load()
		{
			List<string> list = File.ReadAllLines(Path.Combine(Application.streamingAssetsPath, Path.Combine("Data", "achievements.txt"))).ToList();
			Debug.Log("Loading achievements.");
			achievements = new Achievement_t[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				achievements[i] = new Achievement_t(list[i], string.Empty, string.Empty);
			}
		}
	}
}
