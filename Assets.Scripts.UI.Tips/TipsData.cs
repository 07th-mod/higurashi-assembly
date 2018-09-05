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
				Id = 45,
				Script = "tata_tips_01",
				UnlockChapter = 1,
				Title = "Satoko's Trap Course (Elementary)",
				TitleJp = "沙都子のトラップ講座（初級）"
			},
			new TipsDataEntry
			{
				Id = 46,
				Script = "tata_tips_02",
				UnlockChapter = 2,
				Title = "Satoko's Trap Course (Intermediate)",
				TitleJp = "沙都子のトラップ講座（中級）"
			},
			new TipsDataEntry
			{
				Id = 47,
				Script = "tata_tips_03",
				UnlockChapter = 3,
				Title = "Satoko's Trap Course (Advanced)",
				TitleJp = "沙都子のトラップ講座（上級）"
			},
			new TipsDataEntry
			{
				Id = 48,
				Script = "tata_tips_04",
				UnlockChapter = 4,
				Title = "The Slacking Manager",
				TitleJp = "サボりマネ\u30fcジャ\u30fc"
			},
			new TipsDataEntry
			{
				Id = 49,
				Script = "tata_tips_05",
				UnlockChapter = 4,
				Title = "Copy of the Preliminary Autopsy",
				TitleJp = "検死初見コピ\u30fc"
			},
			new TipsDataEntry
			{
				Id = 50,
				Script = "tata_tips_06",
				UnlockChapter = 4,
				Title = "East Round Three",
				TitleJp = "東三局"
			},
			new TipsDataEntry
			{
				Id = 51,
				Script = "tata_tips_07",
				UnlockChapter = 5,
				Title = "Research Notes",
				TitleJp = "研究ノ\u30fcト"
			},
			new TipsDataEntry
			{
				Id = 52,
				Script = "tata_tips_08",
				UnlockChapter = 5,
				Title = "Ooishi's Post Memo",
				TitleJp = "大石席のメモ"
			},
			new TipsDataEntry
			{
				Id = 53,
				Script = "tata_tips_09",
				UnlockChapter = 6,
				Title = "Case 31",
				TitleJp = "事例３１"
			},
			new TipsDataEntry
			{
				Id = 54,
				Script = "tata_tips_10",
				UnlockChapter = 8,
				Title = "Article",
				TitleJp = "条文"
			},
			new TipsDataEntry
			{
				Id = 55,
				Script = "tata_tips_11",
				UnlockChapter = 8,
				Title = "Statistics from the Ministry of Health and Welfare",
				TitleJp = "厚生省統計"
			},
			new TipsDataEntry
			{
				Id = 56,
				Script = "tata_tips_12",
				UnlockChapter = 11,
				Title = "Emergency",
				TitleJp = "緊急"
			},
			new TipsDataEntry
			{
				Id = 57,
				Script = "tata_tips_13",
				UnlockChapter = 11,
				Title = "D2-3 Number 44",
				TitleJp = "エ２−３第４４号"
			},
			new TipsDataEntry
			{
				Id = 58,
				Script = "tata_tips_14",
				UnlockChapter = 13,
				Title = "For the attention of those on the housewife slaughter incident case",
				TitleJp = "主婦撲殺事件担当課御中"
			},
			new TipsDataEntry
			{
				Id = 59,
				Script = "tata_tips_15",
				UnlockChapter = 16,
				Title = "Fire from Hell",
				TitleJp = "地獄の業火"
			},
			new TipsDataEntry
			{
				Id = 60,
				Script = "tata_tips_16",
				UnlockChapter = 16,
				Title = "Victim of the Fifth Year",
				TitleJp = "５年目の犠牲者"
			},
			new TipsDataEntry
			{
				Id = 61,
				Script = "tata_tips_17",
				UnlockChapter = 17,
				Title = "Inquiry Request",
				TitleJp = "照会要請"
			},
			new TipsDataEntry
			{
				Id = 62,
				Script = "tata_tips_18",
				UnlockChapter = 17,
				Title = "Record of Malice",
				TitleJp = "恨み帳？"
			},
			new TipsDataEntry
			{
				Id = 63,
				Script = "tata_tips_19",
				UnlockChapter = 19,
				Title = "Research Notes",
				TitleJp = "研究ノ\u30fcト"
			}
		};

		public static TipsDataGroup GetVisibleTips(bool onlyNew, bool global)
		{
			TipsDataGroup tipsDataGroup = new TipsDataGroup();
			BurikoMemory instance = BurikoMemory.Instance;
			if (global)
			{
				int num = instance.GetGlobalFlag("GTatarigoroshiDay").IntValue();
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
			int num2 = instance.GetFlag("LTatarigoroshiDay").IntValue();
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
