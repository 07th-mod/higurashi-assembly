using Assets.Scripts.Core.Buriko;
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
				Id = 0,
				Script = "onik_tips_01",
				UnlockChapter = 1,
				Title = "We're a mixed grade?",
				TitleJp = "うちって学年混在？"
			},
			new TipsDataEntry
			{
				Id = 1,
				Script = "onik_tips_02",
				UnlockChapter = 1,
				Title = "We don't have a uniform?",
				TitleJp = "うちって制服自由？"
			},
			new TipsDataEntry
			{
				Id = 2,
				Script = "onik_tips_03",
				UnlockChapter = 2,
				Title = "The Maebara Manor",
				TitleJp = "前原屋敷"
			},
			new TipsDataEntry
			{
				Id = 3,
				Script = "onik_tips_04",
				UnlockChapter = 2,
				Title = "The dam site murder/dismemberment (Newspaper edition)",
				TitleJp = "ダム現場のバラバラ殺人（新聞版）"
			},
			new TipsDataEntry
			{
				Id = 4,
				Script = "onik_tips_05",
				UnlockChapter = 3,
				Title = "The Hinamizawa Dam Project",
				TitleJp = "雛見沢ダム計画"
			},
			new TipsDataEntry
			{
				Id = 5,
				Script = "onik_tips_06",
				UnlockChapter = 3,
				Title = "Special tabloid report",
				TitleJp = "週刊誌の特集記事"
			},
			new TipsDataEntry
			{
				Id = 6,
				Script = "onik_tips_07",
				UnlockChapter = 4,
				Title = "What kind of name is Rena?",
				TitleJp = "レナってどういう名前だよ？"
			},
			new TipsDataEntry
			{
				Id = 7,
				Script = "onik_tips_08",
				UnlockChapter = 5,
				Title = "Community Notice",
				TitleJp = "回覧板"
			},
			new TipsDataEntry
			{
				Id = 8,
				Script = "onik_tips_09",
				UnlockChapter = 7,
				Title = "Houjou couple's falling incident",
				TitleJp = "北条両親の転落事故"
			},
			new TipsDataEntry
			{
				Id = 9,
				Script = "onik_tips_10",
				UnlockChapter = 7,
				Title = "The terminal illness death of Shinto Priest Furude",
				TitleJp = "古手神社の神主の病死"
			},
			new TipsDataEntry
			{
				Id = 10,
				Script = "onik_tips_11",
				UnlockChapter = 7,
				Title = "Housewife murder",
				TitleJp = "主婦殺人事件"
			},
			new TipsDataEntry
			{
				Id = 11,
				Script = "onik_tips_12",
				UnlockChapter = 7,
				Title = "Radio Log",
				TitleJp = "無線記録"
			},
			new TipsDataEntry
			{
				Id = 12,
				Script = "onik_tips_13",
				UnlockChapter = 8,
				Title = "There are more than four perpetrators?",
				TitleJp = "犯人は４人以上？"
			},
			new TipsDataEntry
			{
				Id = 13,
				Script = "onik_tips_14",
				UnlockChapter = 8,
				Title = "Search notice",
				TitleJp = "捜査メモ"
			},
			new TipsDataEntry
			{
				Id = 14,
				Script = "onik_tips_15",
				UnlockChapter = 9,
				Title = "Notice from the police chief",
				TitleJp = "本部長通達"
			},
			new TipsDataEntry
			{
				Id = 15,
				Script = "onik_tips_16",
				UnlockChapter = 10,
				Title = "What's a drug that makes you commit suicide?",
				TitleJp = "自殺を誘発するクスリは？"
			},
			new TipsDataEntry
			{
				Id = 16,
				Script = "onik_tips_17",
				UnlockChapter = 10,
				Title = "Threat",
				TitleJp = "脅迫"
			},
			new TipsDataEntry
			{
				Id = 17,
				Script = "onik_tips_18",
				UnlockChapter = 11,
				Title = "Not feeling so hot",
				TitleJp = "元気ないね。"
			},
			new TipsDataEntry
			{
				Id = 18,
				Script = "onik_tips_19",
				UnlockChapter = 13,
				Title = "Split personality???",
				TitleJp = "二重人格？？？"
			},
			new TipsDataEntry
			{
				Id = 19,
				Script = "onik_tips_20",
				UnlockChapter = 13,
				Title = "At the Seventh Mart",
				TitleJp = "セブンスマ\u30fcトにて"
			}
		};

		public static TipsDataGroup GetVisibleTips(bool onlyNew, bool global)
		{
			TipsDataGroup tipsDataGroup = new TipsDataGroup();
			BurikoMemory instance = BurikoMemory.Instance;
			if (global)
			{
				int num = instance.GetGlobalFlag("GOnikakushiDay").IntValue();
				{
					foreach (TipsDataEntry tip in Tips)
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
			int num2 = instance.GetFlag("LOnikakushiDay").IntValue();
			Debug.Log("current chapter " + num2);
			foreach (TipsDataEntry tip2 in Tips)
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
