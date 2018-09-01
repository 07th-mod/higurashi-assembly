using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_Settings : ScriptableObject
	{
		public static TMP_Settings Instance;

		public bool enableWordWrapping;

		public bool enableKerning;

		public bool enableExtraPadding;

		public TextMeshProFont fontAsset;

		public SpriteAsset spriteAsset;

		public TMP_StyleSheet styleSheet;

		public static TMP_Settings LoadDefaultSettings()
		{
			if (Instance == null)
			{
				TMP_Settings tMP_Settings = Resources.Load("TMP Settings") as TMP_Settings;
				if (tMP_Settings != null)
				{
					Instance = tMP_Settings;
				}
			}
			return Instance;
		}
	}
}
