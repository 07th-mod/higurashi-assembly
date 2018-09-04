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
				Id = 21,
				Script = "wata_tips_01",
				UnlockChapter = 1,
				Title = "Who's Mion's Uncle?",
				TitleJp = "魅音の叔父さんって？"
			},
			new TipsDataEntry
			{
				Id = 22,
				Script = "wata_tips_02",
				UnlockChapter = 2,
				Title = "Is There Really a Shion?",
				TitleJp = "詩音って本当にいるの？"
			},
			new TipsDataEntry
			{
				Id = 23,
				Script = "wata_tips_03",
				UnlockChapter = 2,
				Title = "An Introduction to Angel Mort",
				TitleJp = "エンジェルモ\u30fcト紹介記事"
			},
			new TipsDataEntry
			{
				Id = 24,
				Script = "wata_tips_04",
				UnlockChapter = 3,
				Title = "It Was Delicious",
				TitleJp = "ごちそうさま"
			},
			new TipsDataEntry
			{
				Id = 25,
				Script = "wata_tips_05",
				UnlockChapter = 4,
				Title = "Details of the Delinquent Three",
				TitleJp = "三人組の顛末"
			},
			new TipsDataEntry
			{
				Id = 26,
				Script = "wata_tips_06",
				UnlockChapter = 5,
				Title = "It Hasn't Been a While",
				TitleJp = "初めましてじゃないです"
			},
			new TipsDataEntry
			{
				Id = 27,
				Script = "wata_tips_07",
				UnlockChapter = 7,
				Title = "From the Scrapbook I",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 28,
				Script = "wata_tips_08",
				UnlockChapter = 7,
				Title = "From the Scrapbook II",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 29,
				Script = "wata_tips_09",
				UnlockChapter = 7,
				Title = "From the Scrapbook III",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 30,
				Script = "wata_tips_10",
				UnlockChapter = 7,
				Title = "Festival's Around the Corner",
				TitleJp = "いよいよお祭り"
			},
			new TipsDataEntry
			{
				Id = 31,
				Script = "wata_tips_11",
				UnlockChapter = 8,
				Title = "From the Scrapbook IV",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 32,
				Script = "wata_tips_12",
				UnlockChapter = 8,
				Title = "From the Scrapbook V",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 33,
				Script = "wata_tips_13",
				UnlockChapter = 8,
				Title = "After the Festival",
				TitleJp = "後夜祭"
			},
			new TipsDataEntry
			{
				Id = 34,
				Script = "wata_tips_14",
				UnlockChapter = 10,
				Title = "From the Scrapbook VI",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 35,
				Script = "wata_tips_15",
				UnlockChapter = 10,
				Title = "From the Scrapbook VII",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 36,
				Script = "wata_tips_16",
				UnlockChapter = 10,
				Title = "Late-Night Phone Call",
				TitleJp = "深夜の電話"
			},
			new TipsDataEntry
			{
				Id = 37,
				Script = "wata_tips_17",
				UnlockChapter = 12,
				Title = "From the Scrapbook VIII",
				TitleJp = "脅迫"
			},
			new TipsDataEntry
			{
				Id = 38,
				Script = "wata_tips_18",
				UnlockChapter = 12,
				Title = "Their Punishment Isn't Over?",
				TitleJp = "４人だけの罪に終わらない？"
			},
			new TipsDataEntry
			{
				Id = 39,
				Script = "wata_tips_19",
				UnlockChapter = 14,
				Title = "From the Scrapbook IX",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 40,
				Script = "wata_tips_20",
				UnlockChapter = 14,
				Title = "The Elderly Leader of the Sonozaki Family?",
				TitleJp = "園崎家の老当主は？"
			},
			new TipsDataEntry
			{
				Id = 41,
				Script = "wata_tips_21",
				UnlockChapter = 14,
				Title = "From the Scrapbook X",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 42,
				Script = "wata_tips_22",
				UnlockChapter = 14,
				Title = "Request Denied",
				TitleJp = "請求却下"
			},
			new TipsDataEntry
			{
				Id = 43,
				Script = "wata_tips_23",
				UnlockChapter = 16,
				Title = "From the Scrapbook XI",
				TitleJp = "スクラップ帳より"
			},
			new TipsDataEntry
			{
				Id = 44,
				Script = "wata_tips_24",
				UnlockChapter = 16,
				Title = "At the Suzu Mahjong Parlor",
				TitleJp = "雀荘「鈴」"
			}
		};

		public static TipsDataGroup GetVisibleTips(bool onlyNew, bool global)
		{
			TipsDataGroup tipsDataGroup = new TipsDataGroup();
			BurikoMemory instance = BurikoMemory.Instance;
			if (global)
			{
				int num = instance.GetGlobalFlag("GWatanagashiDay").IntValue();
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
			int num2 = instance.GetFlag("LWatanagashiDay").IntValue();
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
