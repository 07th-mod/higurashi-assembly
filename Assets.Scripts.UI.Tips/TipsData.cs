using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.Tips
{
	public static class TipsData
	{
		public static List<TipsDataEntry> Tips = new List<TipsDataEntry>
		{
			new TipsDataEntry
			{
				Id = 24,
				Script = "_tsum_tips_001",
				Title = "A Beautiful Day",
				TitleJp = "■いいお天気"
			},
			new TipsDataEntry
			{
				Id = 25,
				Script = "_tsum_tips_002",
				Title = "Business Card",
				TitleJp = "■名刺"
			},
			new TipsDataEntry
			{
				Id = 26,
				Script = "_tsum_tips_003",
				Title = "My Summer Picture Diary",
				TitleJp = "■夏休みの絵日記"
			},
			new TipsDataEntry
			{
				Id = 27,
				Script = "_tsum_tips_004",
				Title = "Rainy Days Are On Their Way",
				TitleJp = "■雨雲の予感"
			},
			new TipsDataEntry
			{
				Id = 28,
				Script = "_tsum_tips_005",
				Title = "Property Estimate",
				TitleJp = "■お見積書"
			},
			new TipsDataEntry
			{
				Id = 29,
				Script = "_tsum_tips_006",
				Title = "My Very Favorite Wine",
				TitleJp = "■お気に入りのワイン"
			},
			new TipsDataEntry
			{
				Id = 30,
				Script = "_tsum_tips_007",
				Title = "Notice from the Forestry Service",
				TitleJp = "■営林署便り"
			},
			new TipsDataEntry
			{
				Id = 31,
				Script = "_tsum_tips_008",
				Title = "Lunch Takeout List",
				TitleJp = "■昼の出前リスト"
			},
			new TipsDataEntry
			{
				Id = 32,
				Script = "_tsum_tips_009",
				Title = "Lunch Takeout List 2",
				TitleJp = "■昼の出前リスト２"
			},
			new TipsDataEntry
			{
				Id = 33,
				Script = "_tsum_tips_010",
				Title = "Lunch Takeout List 3",
				TitleJp = "■昼の出前リスト３"
			},
			new TipsDataEntry
			{
				Id = 34,
				Script = "_tsum_tips_011",
				Title = "The Others",
				TitleJp = "■やつら"
			},
			new TipsDataEntry
			{
				Id = 35,
				Script = "_tsum_tips_012",
				Title = "Our Reason to Move to Hinamizawa",
				TitleJp = "■雛見沢だった訳"
			},
			new TipsDataEntry
			{
				Id = 36,
				Script = "_tsum_tips_013",
				Title = "Last Night",
				TitleJp = "■前夜"
			},
			new TipsDataEntry
			{
				Id = 37,
				Script = "_tsum_tips_014",
				Title = "The Demon's Script",
				TitleJp = "■悪魔の脚本"
			}
		};

		public static TipsDataGroup GetVisibleTips(bool onlyNew, bool global)
		{
			TipsDataGroup tipsDataGroup = new TipsDataGroup();
			BurikoMemory instance = BurikoMemory.Instance;
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
