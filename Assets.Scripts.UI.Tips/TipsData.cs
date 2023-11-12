using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.Tips
{
	public static class TipsData
	{
		/// <summary>
		/// This returns the BUILT-IN tips for the arc.
		/// In previous arcs this was a hard-coded list. It's now lazily loaded from StreamingAssets/Data/tips.txt
		/// <seealso cref="MODTipsController.Tips"/>
		/// </summary>
		public static List<TipsDataEntry> Tips
		{
			get
			{
				if (BuiltInTips.Count == 0)
				{
					string value = AssetManager.Instance.LoadTextDataString("tips.txt");
					BuiltInTips = JsonConvert.DeserializeObject<List<TipsDataEntry>>(value);
				}
				return BuiltInTips;
			}
		}

		/// <summary>
		/// Tips that came with the game
		/// </summary>
		private static List<TipsDataEntry> BuiltInTips = new List<TipsDataEntry>();

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
