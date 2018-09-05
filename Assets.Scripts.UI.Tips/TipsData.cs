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
				Id = 1,
				Script = "_meak_tips_01",
				Title = "The Notebook's Beginning",
				TitleJp = "考察メモ冒頭"
			},
			new TipsDataEntry
			{
				Id = 2,
				Script = "_meak_tips_02",
				Title = "A Smudged Diary",
				TitleJp = "にじんだ日記"
			},
			new TipsDataEntry
			{
				Id = 3,
				Script = "_meak_tips_03",
				Title = "A Crumpled Diary",
				TitleJp = "くしゃくしゃの日記"
			},
			new TipsDataEntry
			{
				Id = 4,
				Script = "_meak_tips_04",
				Title = "Notebook Page 21",
				TitleJp = "ノ\u30fcト２１ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 5,
				Script = "_meak_tips_05",
				Title = "Notebook Page 24",
				TitleJp = "ノ\u30fcト２４ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 6,
				Script = "_meak_tips_06",
				Title = "A Torn Diary",
				TitleJp = "粉\u3005の日記"
			},
			new TipsDataEntry
			{
				Id = 7,
				Script = "_meak_tips_07",
				Title = "Notebook Page 29",
				TitleJp = "ノ\u30fcト２９ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 8,
				Script = "_meak_tips_08",
				Title = "Notebook Page 34",
				TitleJp = "ノ\u30fcトの３４ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 9,
				Script = "_meak_tips_09",
				Title = "Notebook Page 42",
				TitleJp = "ノ\u30fcトの４２ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 10,
				Script = "_meak_tips_10",
				Title = "Notebook Page 50",
				TitleJp = "ノ\u30fcトの５０ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 11,
				Script = "_meak_tips_11",
				Title = "Notebook Page 64",
				TitleJp = "ノ\u30fcトの６４ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 12,
				Script = "_meak_tips_12",
				Title = "Notebook Page 85",
				TitleJp = "ノ\u30fcトの８５ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 13,
				Script = "_meak_tips_13",
				Title = "Notebook Page 172",
				TitleJp = "ノ\u30fcトの１７２ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 14,
				Script = "_meak_tips_14",
				Title = "Notebook Page 173",
				TitleJp = "ノ\u30fcトの１７３ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 15,
				Script = "_meak_tips_15",
				Title = "Notebook Page 179",
				TitleJp = "ノ\u30fcトの１７９ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 16,
				Script = "_meak_tips_16",
				Title = "Notebook Page 183",
				TitleJp = "ノ\u30fcトの１８３ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 17,
				Script = "_meak_tips_17",
				Title = "Notebook Page 185",
				TitleJp = "ノ\u30fcトの１８５ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 18,
				Script = "_meak_tips_18",
				Title = "Notebook Page 188",
				TitleJp = "ノ\u30fcトの１８８ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 19,
				Script = "_meak_tips_19",
				Title = "Notebook Page 195",
				TitleJp = "ノ\u30fcトの１９５ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 20,
				Script = "_meak_tips_20",
				Title = "Notebook Page 196",
				TitleJp = "ノ\u30fcトの１９６ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 21,
				Script = "_meak_tips_21",
				Title = "Notebook Page 199",
				TitleJp = "ノ\u30fcトの１９９ペ\u30fcジ"
			},
			new TipsDataEntry
			{
				Id = 22,
				Script = "_meak_tips_22",
				Title = "A Happy Diary",
				TitleJp = "幸せのノ\u30fcト"
			},
			new TipsDataEntry
			{
				Id = 23,
				Script = "_meak_tips_23",
				Title = "Disowned",
				TitleJp = "チャンバラで勘当"
			}
		};

		public static TipsDataGroup GetVisibleTips(bool onlyNew, bool global)
		{
			TipsDataGroup tipsDataGroup = new TipsDataGroup();
			BurikoMemory instance = BurikoMemory.Instance;
			if (global)
			{
				int num = instance.GetGlobalFlag("GTotalTips").IntValue();
				Debug.Log("Displaying tips up to " + num);
				{
					foreach (TipsDataEntry tip in MODSystem.instance.modTipsController.Tips)
					{
						if (tipsDataGroup.TipsAvailable < num)
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
			for (int i = 0; i < MODSystem.instance.modTipsController.Tips.Count; i++)
			{
				if (onlyNew)
				{
					if (i >= num2 && i < num3)
					{
						tipsDataGroup.TipsAvailable++;
						tipsDataGroup.TipsUnlocked++;
						tipsDataGroup.Tips.Add(Tips[i]);
					}
				}
				else if (i < num3)
				{
					tipsDataGroup.TipsAvailable++;
					tipsDataGroup.TipsUnlocked++;
					tipsDataGroup.Tips.Add(Tips[i]);
				}
			}
			return tipsDataGroup;
		}
	}
}
