
using Assets.Scripts.Core;
using System;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	class MODMenuFontConfig
	{
		string TextField_FontOutlineWidth;
		string TextField_NormalFontWeight;
		string TextField_BoldFontWeight;
		string TextField_FontSize;
		string TextField_ConfigMenuFontSize;
		string TextField_FaceDilation;

		string TextArea_GeneratedScriptFragment = "";

		public void OnBeforeMenuVisible() {
			TextField_FontOutlineWidth = asPercent(GameSystem.Instance.OutlineWidth).ToString();
			TextField_NormalFontWeight = asPercent(GameSystem.Instance.MainUIController.GetNormalFontWeight()).ToString();
			TextField_BoldFontWeight = asPercent(GameSystem.Instance.MainUIController.GetBoldFontWeight()).ToString();
			TextField_FontSize = GameSystem.Instance.MainUIController.TextWindow.fontSize.ToString();
			TextField_ConfigMenuFontSize = GameSystem.Instance.ConfigMenuFontSize.ToString();
			TextField_FaceDilation = asPercent(GameSystem.Instance.MainUIController.GetFaceDilation()).ToString();

			UpdateGeneratedScriptFragment();
		}

		public void OnGUIFontDebug()
		{
			Label("Font Debugging");

			// --------- Font Outline and Size Adjustment ---------
			GUILayout.BeginHorizontal();

			Label(new GUIContent("Face Dilate (%)", "Face Dilation\n\nExpands or shrinks the text thickness.\n\nTry adjusting this rather than font weight."));
			TextField_FaceDilation = TextField(TextField_FaceDilation, true, out bool faceDilationHasChanged);

			Label(new GUIContent("Outline (%)", "Main Text Window Font Outline Width"));
			TextField_FontOutlineWidth = TextField(TextField_FontOutlineWidth, true, out bool outlineHasChanged);

			GUILayout.EndHorizontal();

			// --------- Font Size Adjustment ---------
			GUILayout.BeginHorizontal();

			Label(new GUIContent("Font Size", "Main Text Window Font Size"));
			TextField_FontSize = TextField(TextField_FontSize, true, out bool fontSizeHasChanged);

			Label(new GUIContent("Config Font Size", "Config Menu Font Size"));
			TextField_ConfigMenuFontSize = TextField(TextField_ConfigMenuFontSize, true, out bool configFontSizeHasChanged);

			GUILayout.EndHorizontal();

			// --------- Font Weight Adjustment ---------
			Label("!Note: Adjust face dilation instead of these values!");

			GUILayout.BeginHorizontal();

			Label(new GUIContent("!Normal Weight (% change)!", "Main Text Window Normal Font Weight"));
			TextField_NormalFontWeight = TextField(TextField_NormalFontWeight, true, out bool fontWeightHasChanged);

			Label(new GUIContent("!Bold Weight (% change)!", "Main Text Window Bold Font Weight"));
			TextField_BoldFontWeight = TextField(TextField_BoldFontWeight, true, out bool fontBoldWeightHasChanged);

			GUILayout.EndHorizontal();

			// --------- Textbox showing example code ---------
			TextArea(TextArea_GeneratedScriptFragment, Screen.height / 8);

			if (outlineHasChanged ||
				fontWeightHasChanged ||
				fontBoldWeightHasChanged ||
				fontSizeHasChanged ||
				configFontSizeHasChanged ||
				faceDilationHasChanged)
			{
				try
				{
					// Update main text window font
					GameSystem.Instance.MainUIController.SetFontOutlineWidth(fromPercent(TextField_FontOutlineWidth));
					GameSystem.Instance.MainUIController.SetNormalFontWeight(fromPercent(TextField_NormalFontWeight));
					GameSystem.Instance.MainUIController.SetBoldFontWeight(fromPercent(TextField_BoldFontWeight));
					GameSystem.Instance.MainUIController.SetFontSize(int.Parse(TextField_FontSize)); // SetFontSize already takes an percentage as int
					GameSystem.Instance.MainUIController.SetFaceDilation(fromPercent(TextField_FaceDilation));

					// Update config menu font
					GameSystem.Instance.ConfigMenuFontSize = int.Parse(TextField_ConfigMenuFontSize);

					// Refresh the config menu only if it is open
					GameSystem.Instance.ConfigManager()?.RefreshFontSettings();

					UpdateGeneratedScriptFragment();
				}
				catch (Exception e)
				{
					MODToaster.Show("Failed to set font settings:" + e.Message);
				}
			}
		}

		private void UpdateGeneratedScriptFragment()
		{
			TextArea_GeneratedScriptFragment = $"//Font Size: [{TextField_FontSize}] - Please update init.txt accordingly\n" +
				$"ModSetConfigFontSize({TextField_ConfigMenuFontSize});\n" +
				$"ModSetMainFontOutlineWidth({TextField_FontOutlineWidth});\n" +
				$"ModGenericCall(\"NormalFontWeight\", \"{TextField_NormalFontWeight}\");\n" +
				$"ModGenericCall(\"BoldFontWeight\", \"{TextField_BoldFontWeight}\");\n" +
				$"ModGenericCall(\"FaceDilate\", \"{TextField_FaceDilation}\");\n";
		}

		public int asPercent(float fontOption) => Mathf.RoundToInt(fontOption * 100);
		public float fromPercent(string fontOption) => (float)(float.Parse(fontOption) / 100.0f);
	}
}
