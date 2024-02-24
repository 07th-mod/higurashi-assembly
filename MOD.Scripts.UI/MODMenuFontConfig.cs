﻿
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

		string TextArea_GeneratedScriptFragment = "";

		public void OnBeforeMenuVisible() {
			TextField_FontOutlineWidth = asPercent(GameSystem.Instance.OutlineWidth).ToString();
			TextField_NormalFontWeight = asPercent(GameSystem.Instance.MainUIController.GetNormalFontWeight()).ToString();
			TextField_BoldFontWeight = asPercent(GameSystem.Instance.MainUIController.GetBoldFontWeight()).ToString();
			TextField_FontSize = GameSystem.Instance.MainUIController.TextWindow.fontSize.ToString();
			TextField_ConfigMenuFontSize = GameSystem.Instance.ConfigMenuFontSize.ToString();

			UpdateGeneratedScriptFragment();
		}

		public void OnGUIFontDebug()
		{
			Label("Font Debugging");

			// --------- Font Weight Adjustment ---------
			GUILayout.BeginHorizontal();

			Label(new GUIContent("Normal Weight (%)", "Main Text Window Normal Font Weight"));
			TextField_NormalFontWeight = TextField(TextField_NormalFontWeight, true, out bool fontWeightHasChanged);

			Label(new GUIContent("Bold Weight (%)", "Main Text Window Bold Font Weight"));
			TextField_BoldFontWeight = TextField(TextField_BoldFontWeight, true, out bool fontBoldWeightHasChanged);

			GUILayout.EndHorizontal();

			// --------- Font Outline and Size Adjustment ---------
			GUILayout.BeginHorizontal();

			Label(new GUIContent("Outline (%)", "Main Text Window Font Outline Width"));
			TextField_FontOutlineWidth = TextField(TextField_FontOutlineWidth, true, out bool outlineHasChanged);

			Label(new GUIContent("Font Size", "Main Text Window Font Size"));
			TextField_FontSize = TextField(TextField_FontSize, true, out bool fontSizeHasChanged);

			GUILayout.EndHorizontal();

			// --------- Config Menu Font Size Adjustment ---------
			GUILayout.BeginHorizontal();

			Label(new GUIContent("Config Font Size", "Config Menu Font Size"));
			TextField_ConfigMenuFontSize = TextField(TextField_ConfigMenuFontSize, true, out bool configFontSizeHasChanged);

			GUILayout.EndHorizontal();

			// --------- Textbox showing example code ---------
			TextArea(TextArea_GeneratedScriptFragment, Screen.height / 16);

			if (outlineHasChanged ||
				fontWeightHasChanged ||
				fontBoldWeightHasChanged ||
				fontSizeHasChanged ||
				configFontSizeHasChanged)
			{
				try
				{
					// Update main text window font
					GameSystem.Instance.MainUIController.SetFontOutlineWidth(fromPercent(TextField_FontOutlineWidth));
					GameSystem.Instance.MainUIController.SetNormalFontWeight(fromPercent(TextField_NormalFontWeight));
					GameSystem.Instance.MainUIController.SetBoldFontWeight(fromPercent(TextField_BoldFontWeight));
					GameSystem.Instance.MainUIController.SetFontSize(int.Parse(TextField_FontSize)); // SetFontSize already takes an percentage as int

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
			// TODO: fix x100 scaling issue
			TextArea_GeneratedScriptFragment = $"//Set font size manually (need to set per each option)\n" +
				$"ModSetConfigFontSize({TextField_ConfigMenuFontSize})\n" +
				$"ModSetMainFontOutlineWidth({TextField_FontOutlineWidth})\n";
		}

		public int asPercent(float fontOption) => Mathf.RoundToInt(fontOption * 100);
		public float fromPercent(string fontOption) => (float)(float.Parse(fontOption) / 100.0f);
	}
}
