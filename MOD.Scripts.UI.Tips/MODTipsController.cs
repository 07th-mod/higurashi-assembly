using Assets.Scripts.Core.Buriko;
using Assets.Scripts.UI.Tips;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MOD.Scripts.Core;

namespace MOD.Scripts.UI.Tips
{
	public class MODTipsController
	{
		private bool initialized;
		private Dictionary<int, List<TipsDataEntry>> fixedTips;
		private static readonly string TipsFilePath = Path.Combine(MODSystem.BaseDirectory, "tips.json");

		/// <summary>
		/// This returns the current modded tips for the arc, if present.
		/// If no tips.json file is present or is malformed, gracefully falls back to the vanilla tips
		/// Otherwise, it returns the tips associated with the current value of the "GArc" flag, which is 0 by default.
		/// <seealso cref="TipsData.Tips"/>
		/// </summary>
		public List<TipsDataEntry> Tips
		{
			get
			{
				if (!initialized)
				{
					initialized = true;
					if (File.Exists(TipsFilePath))
					{
						try
						{
							using (var reader = new JsonTextReader(new StreamReader(TipsFilePath)))
							{
								fixedTips = JsonSerializer.Create(new JsonSerializerSettings()).Deserialize<Dictionary<int, List<TipsDataEntry>>>(reader);
							}
						}
						catch (System.Exception e)
						{
							Debug.LogError("Falling back to hard-coded tips; failed to read tips file: " + e.Message);
						}
					}
					else
					{
						Debug.Log("Tips file not present; falling back to hard-coded tips");
					}
				}
				if (fixedTips != null)
				{
					int arc;
					try
					{
						arc = BurikoMemory.Instance.GetFlag("LConsoleArc").IntValue();
					}
					catch
					{
						arc = 0;
					}
					fixedTips.TryGetValue(arc, out List<TipsDataEntry> value);
					return value ?? new List<TipsDataEntry>();
				}
				return TipsData.Tips;
			}
		}
	}
}
