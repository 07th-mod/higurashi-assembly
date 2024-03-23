using Assets.Scripts.Core;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	internal class MODFontAdjuster
	{
		private static void SetFontMaterialSetting(int settingID, float value)
		{
			GameSystem.Instance.MainUIController.TextWindow.fontSharedMaterial.SetFloat(settingID, value);

			// Note: sometimes only updating the material value is not enough, and causes letters to be cut-off
			// UpdateMeshPadding(); seems to fix this
			// I also call UpdateVertexData() and ForceMeshUpdate() but I'm not sure if it is necessary
			GameSystem.Instance.MainUIController.TextWindow.ForceMeshUpdate();
			GameSystem.Instance.MainUIController.TextWindow.UpdateMeshPadding();

			// The following can only be called on newer versions of TextMeshPro
			GameSystem.Instance.MainUIController.TextWindow.UpdateVertexData();
		}

		private static float GetFontMaterialSetting(int settingID)
		{
			return GameSystem.Instance.MainUIController.TextWindow.fontSharedMaterial.GetFloat(settingID);
		}

		/// <summary>
		/// Sets the font outline width, and updates the current textwindow outline width
		/// Hopefully if you toggle language (which toggles the font on chapters 1-9), the outline width is retained.
		/// You will likely need to increase the font weight if you increase the outline width, as the outline 'eats'
		/// into the characters, rather than expanding outwards.
		/// </summary>
		/// <param name="outlineWidth">The outline width as a float typically between (0, 1)</param>
		public static void SetFontOutlineWidth(float outlineWidth)
		{
			GameSystem.Instance.OutlineWidth = outlineWidth;
			GameSystem.Instance.MainUIController.TextWindow.outlineWidth = GameSystem.Instance.OutlineWidth;
			SetFontMaterialSetting(TMPro.ShaderUtilities.ID_OutlineWidth, outlineWidth);
		}

		public static float GetNormalFontWeight() => GetFontMaterialSetting(TMPro.ShaderUtilities.ID_WeightNormal);
		public static float GetBoldFontWeight() => GetFontMaterialSetting(TMPro.ShaderUtilities.ID_WeightBold);

		/// <summary>
		/// Some chapters TextWindow don't let you set the font weight. For those chapters, the "_WeightNormal"
		/// parameter on the material is set directly.
		/// </summary>
		/// <param name="weight">The font weight, between -1 and 1 (more or less).
		/// Negative numbers reduce the weight, positive numbers increase it. </param>
		public static void SetNormalFontWeight(float weight)
		{
			// Modifying fontSharedMaterial works, I guess doing so modifies all instances,
			// while fontMaterial only modifies a single instance?
			// See the docs:
			// - http://digitalnativestudios.com/textmeshpro/docs/ScriptReference/TextMeshPro.html
			// - http://digitalnativestudios.com/textmeshpro/docs/ScriptReference/TextMeshPro-fontSharedMaterial.html
			SetFontMaterialSetting(TMPro.ShaderUtilities.ID_WeightNormal, weight);
		}

		public static void SetBoldFontWeight(float weight) => SetFontMaterialSetting(TMPro.ShaderUtilities.ID_WeightBold, weight);

		public static void SetFaceDilation(float faceDilate) => SetFontMaterialSetting(TMPro.ShaderUtilities.ID_FaceDilate, faceDilate);

		public static float GetFaceDilation() => GetFontMaterialSetting(TMPro.ShaderUtilities.ID_FaceDilate);

		//private void SetFontMaterialSetting(int settingID, float value)
		//{
		//	FontMaterialSettingCache[settingID] = value;
		//	TextWindow.fontSharedMaterial.SetFloat(settingID, value);
		//}

		//private float GetFontMaterialSetting(int settingID)
		//{
		//	return TextWindow.fontSharedMaterial.GetFloat(settingID);
		//}

		//public static int ID_UnderlayOffsetX;

		//public static int ID_UnderlayOffsetY;

		//public static int ID_UnderlayDilate;

		//public static int ID_UnderlaySoftness;


		// TODO: Also allow setting these values controlling drop shadow?
		// For rei, the following values are used:
		// _UnderlayDilate   = 0.0900000036
		// _UnderlayOffsetX  = 0.5
		// _UnderlayOffsetY  = -0.5
		// _UnderlaySoftness = 0.312000006

	}
}
