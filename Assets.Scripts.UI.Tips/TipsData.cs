using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.UI.Tips
{
	public static class TipsData
	{
		public static List<TipsDataEntry> Tips = new List<TipsDataEntry>();

		public static TipsDataGroup GetVisibleTips(bool onlyNew, bool global)
		{
			TipsDataGroup tipsDataGroup = new TipsDataGroup();
			BurikoMemory instance = BurikoMemory.Instance;
			if (Tips.Count == 0)
			{
				string value = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Data\\tips.txt"));
				Tips = JsonConvert.DeserializeObject<List<TipsDataEntry>>(value);
			}
			if (global)
			{
				int num = instance.GetGlobalFlag("GTotalTips").IntValue();
				if (instance.GetGlobalFlag("GFlag_GameClear").BoolValue())
				{
					num = 999;
				}
				Debug.Log("Displaying tips up to " + num);
				{
					foreach (TipsDataEntry tip in MODSystem.instance.modTipsController.Tips)
					{
						if (tip.Id < num)
						{
							tipsDataGroup.TipsAvailable++;
							tipsDataGroup.TipsUnlocked++;
							tipsDataGroup.Tips.Add(tip);
						}
					}
					return tipsDataGroup;
				}
			}
			int num2 = instance.GetFlag("NewTipsStart").IntValue();
			int num3 = num2 + instance.GetFlag("NewTipsCount").IntValue();
			Debug.Log("Displaying tips " + num2 + " to " + num3);
			var tips = MODSystem.instance.modTipsController.Tips;
			for (int i = 0; i < tips.Count; i++)
			{
				var tip = tips[i];
				int id = tip.Id;
				if (onlyNew)
				{
					if (id >= num2 && id < num3)
					{
						tipsDataGroup.TipsAvailable++;
						tipsDataGroup.TipsUnlocked++;
						tipsDataGroup.Tips.Add(tip);
					}
				}
				else if (id < num3)
				{
					tipsDataGroup.TipsAvailable++;
					tipsDataGroup.TipsUnlocked++;
					tipsDataGroup.Tips.Add(tip);
				}
			}
			return tipsDataGroup;
		}
	}
}
