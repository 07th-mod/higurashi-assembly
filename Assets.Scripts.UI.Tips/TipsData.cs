using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Buriko;
using MOD.Scripts.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI.Tips
{
	public static class TipsData
	{
		/// <summary>
		/// This returns the current tips for the arc.
		/// Prioritizes mod system's tips, falling back to built-in tips.
		/// <seealso cref="MODTipsController.Tips"/>
		/// </summary>
		public static List<TipsDataEntry> Tips
		{
			get
			{
				var modTips = MODSystem.instance.modTipsController.Tips;
				if (modTips != null && modTips.Any())
				{
					return modTips;
				}
				return LoadBuiltInTips();
			}
		}

		/// <summary>
		/// Tips that came with the game
		/// </summary>
		private static List<TipsDataEntry> BuiltInTips = new List<TipsDataEntry>();

		/// <summary>
		/// Initialize and load tips
		/// </summary>
		/// <returns></returns>
		private static List<TipsDataEntry> LoadBuiltInTips()
		{
			if (BuiltInTips.Count == 0)
			{
				string value = AssetManager.Instance.LoadTextDataString("tips.txt");
				BuiltInTips = JsonConvert.DeserializeObject<List<TipsDataEntry>>(value);
			}
			return BuiltInTips;
		}

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
					foreach (TipsDataEntry tip in Tips)
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
			for (int i = 0; i < Tips.Count; i++)
			{
				var tip = Tips[i];
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
