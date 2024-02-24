
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
		string TextField_FontSize;
		string TextField_ConfigMenuFontSize;

		public void OnBeforeMenuVisible() {
			TextField_FontOutlineWidth = GameSystem.Instance.OutlineWidth.ToString();
			TextField_NormalFontWeight = GameSystem.Instance.MainUIController.GetNormalFontWeight().ToString();
			TextField_FontSize = GameSystem.Instance.MainUIController.TextWindow.fontSize.ToString();
			TextField_ConfigMenuFontSize = GameSystem.Instance.ConfigMenuFontSize.ToString();
		}

		public void OnGUIFontDebug()
		{
			Label("Font Debugging");

			// --------- Font Weight Adjustment ---------
			GUILayout.BeginHorizontal();

			Label(new GUIContent("Normal Weight", "Main Text Window Normal Font Weight"));
			TextField_NormalFontWeight = TextField(TextField_NormalFontWeight, true, out bool fontWeightHasChanged);

			// TODO: Bold Weight?

			GUILayout.EndHorizontal();

			// --------- Font Outline and Size Adjustment ---------
			GUILayout.BeginHorizontal();

			Label(new GUIContent("Outline", "Main Text Window Font Outline Width"));
			TextField_FontOutlineWidth = TextField(TextField_FontOutlineWidth, true, out bool outlineHasChanged);

			Label(new GUIContent("Size (int)", "Main Text Window Font Size"));
			TextField_FontSize = TextField(TextField_FontSize, true, out bool fontSizeHasChanged);

			GUILayout.EndHorizontal();

			// --------- Config Menu Font Size Adjustment ---------
			GUILayout.BeginHorizontal();

			Label(new GUIContent("Config Size", "Config Menu Font Size"));
			TextField_ConfigMenuFontSize = TextField(TextField_ConfigMenuFontSize, true, out bool configFontSizeHasChanged);

			GUILayout.EndHorizontal();

			if (outlineHasChanged ||
				fontWeightHasChanged ||
				fontSizeHasChanged ||
				configFontSizeHasChanged)
			{
				try
				{
					// Update main text window font
					GameSystem.Instance.MainUIController.SetFontOutlineWidth(float.Parse(TextField_FontOutlineWidth));
					GameSystem.Instance.MainUIController.SetNormalFontWeight(float.Parse(TextField_NormalFontWeight));
					GameSystem.Instance.MainUIController.SetFontSize(int.Parse(TextField_FontSize));

					// Update config menu font
					GameSystem.Instance.ConfigMenuFontSize = float.Parse(TextField_ConfigMenuFontSize);

					// Refresh the config menu only if it is open
					GameSystem.Instance.ConfigManager()?.RefreshFontSettings();
				}
				catch (Exception e)
				{
					MODToaster.Show("Failed to set font settings:" + e.Message);
				}
			}

		}
	}
}
