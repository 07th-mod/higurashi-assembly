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
				Id = 64,
				Script = "hima_tips_01",
				UnlockChapter = 1,
				Title = "Phone Call with Yukie",
				TitleJp = "雪絵との電話"
			},
			new TipsDataEntry
			{
				Id = 65,
				Script = "hima_tips_02",
				UnlockChapter = 1,
				Title = "A Record of Opening Remarks",
				TitleJp = "来賓挨拶用原稿"
			},
			new TipsDataEntry
			{
				Id = 66,
				Script = "hima_tips_03",
				UnlockChapter = 1,
				Title = "Gears and Fire and the Taste of Honey",
				TitleJp = "歯車と火事と蜜の味"
			},
			new TipsDataEntry
			{
				Id = 67,
				Script = "hima_tips_04",
				UnlockChapter = 1,
				Title = "The Chick in the Trunk",
				TitleJp = "トランクの雛"
			},
			new TipsDataEntry
			{
				Id = 68,
				Script = "hima_tips_05",
				UnlockChapter = 2,
				Title = "Going Smoothly",
				TitleJp = "順調"
			},
			new TipsDataEntry
			{
				Id = 69,
				Script = "hima_tips_06",
				UnlockChapter = 2,
				Title = "Why I Like Rainy Days",
				TitleJp = "雨雲に恋して"
			},
			new TipsDataEntry
			{
				Id = 70,
				Script = "hima_tips_07",
				UnlockChapter = 2,
				Title = "Barley Tea and Black Tea and a Millstone",
				TitleJp = "麦茶と紅茶と石臼と"
			},
			new TipsDataEntry
			{
				Id = 71,
				Script = "hima_tips_08",
				UnlockChapter = 3,
				Title = "The Investigation Runs Aground",
				TitleJp = "調査は暗礁"
			},
			new TipsDataEntry
			{
				Id = 72,
				Script = "hima_tips_09",
				UnlockChapter = 3,
				Title = "What's in the Box?",
				TitleJp = "箱選びゲ\u30fcム"
			},
			new TipsDataEntry
			{
				Id = 73,
				Script = "hima_tips_10",
				UnlockChapter = 3,
				Title = "The Glint in the Demon's Eye",
				TitleJp = "鬼の目にも何とか"
			},
			new TipsDataEntry
			{
				Id = 74,
				Script = "hima_tips_11",
				UnlockChapter = 4,
				Title = "A Very Kind Person",
				TitleJp = "とてもやさしい人なの"
			},
			new TipsDataEntry
			{
				Id = 75,
				Script = "hima_tips_12",
				UnlockChapter = 5,
				Title = "Mother's Diary",
				TitleJp = "母の日記"
			},
			new TipsDataEntry
			{
				Id = 76,
				Script = "hima_tips_13",
				UnlockChapter = 5,
				Title = "Mother's Diary",
				TitleJp = "母の日記"
			},
			new TipsDataEntry
			{
				Id = 77,
				Script = "hima_tips_14",
				UnlockChapter = 5,
				Title = "Mother's Diary",
				TitleJp = "母の日記"
			}
		};

		public static TipsDataGroup GetVisibleTips(bool onlyNew, bool global)
		{
			TipsDataGroup tipsDataGroup = new TipsDataGroup();
			BurikoMemory instance = BurikoMemory.Instance;
			if (global)
			{
				int num = instance.GetGlobalFlag("GHimatsubushiDay").IntValue();
				{
					foreach (TipsDataEntry tip in MODSystem.instance.modTipsController.Tips)
					{
						tipsDataGroup.TipsAvailable++;
						if (tip.UnlockChapter <= num)
						{
							tipsDataGroup.TipsUnlocked++;
							tipsDataGroup.Tips.Add(tip);
						}
					}
					return tipsDataGroup;
				}
			}
			int num2 = instance.GetFlag("LHimatsubushiDay").IntValue();
			Debug.Log("current chapter " + num2);
			foreach (TipsDataEntry tip2 in MODSystem.instance.modTipsController.Tips)
			{
				if (onlyNew)
				{
					if (tip2.UnlockChapter == num2)
					{
						tipsDataGroup.TipsAvailable++;
						tipsDataGroup.TipsUnlocked++;
						tipsDataGroup.Tips.Add(tip2);
					}
				}
				else if (tip2.UnlockChapter <= num2)
				{
					tipsDataGroup.TipsAvailable++;
					tipsDataGroup.TipsUnlocked++;
					tipsDataGroup.Tips.Add(tip2);
				}
			}
			return tipsDataGroup;
		}
	}
}
