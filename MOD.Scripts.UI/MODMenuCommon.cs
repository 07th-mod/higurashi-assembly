using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	static class MODMenuCommon
	{
		public static bool anyButtonPressed;
		public static void Label(string label, params GUILayoutOption[] options)
		{
			GUILayout.Label(label, MODStyleManager.OnGUIInstance.Group.label, options);
		}

		public static void LabelRightAlign(string label, params GUILayoutOption[] options)
		{
			GUILayout.Label(label, MODStyleManager.OnGUIInstance.Group.labelRightAlign, options);
		}

		public static void Label(GUIContent content, params GUILayoutOption[] options)
		{
			GUILayout.Label(content, MODStyleManager.OnGUIInstance.Group.label, options);
		}

		public static void SelectableLabel(string label, params GUILayoutOption[] options)
		{
			GUILayout.TextArea(label, MODStyleManager.OnGUIInstance.Group.label, options);
		}

		public static void HeadingLabel(string label, bool alignLeft = false, params GUILayoutOption[] options)
		{
			GUILayout.Label(label, alignLeft ? MODStyleManager.OnGUIInstance.Group.upperLeftHeadingLabel : MODStyleManager.OnGUIInstance.Group.headingLabel, options);
		}

		public static bool ButtonNoExpandWithPadding(string label, bool selected = false)
		{
			return Button(new GUIContent($"  {label}  "), selected, false, GUILayout.ExpandWidth(false));
		}

		public static bool Button(string label, bool selected = false, bool stlyeAsLabel = false, params GUILayoutOption[] options)
		{
			return Button(new GUIContent(label), selected, stlyeAsLabel, options);
		}

		public static bool Button(GUIContent guiContent, bool selected = false, bool stlyeAsLabel = false, params GUILayoutOption[] options)
		{
			GUIStyle style = selected ? MODStyleManager.OnGUIInstance.Group.selectedButton : MODStyleManager.OnGUIInstance.Group.button;
			if(stlyeAsLabel)
			{
				style = MODStyleManager.OnGUIInstance.Group.label;
			}

			if (GUILayout.Button(guiContent, style, options))
			{
				anyButtonPressed = true;
				return true;
			}
			else
			{
				return false;
			}
		}
		public static string TextField(string text, params GUILayoutOption[] options)
		{
			return GUILayout.TextField(text, MODStyleManager.OnGUIInstance.Group.textField, options);
		}

		public static string TextField(string text, int maxLength, params GUILayoutOption[] options)
		{
			return GUILayout.TextField(text, maxLength, MODStyleManager.OnGUIInstance.Group.textField, options);
		}

		public static string TextField(string text, int maxLength, out bool hasChanged, params GUILayoutOption[] options)
		{
			string newValue = GUILayout.TextField(text, maxLength, MODStyleManager.OnGUIInstance.Group.textField, options);

			hasChanged = text != newValue;
			return newValue;
		}

		public static int GetGlobal(string flagName) => BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue();
		public static void SetGlobal(string flagName, int flagValue) => BurikoMemory.Instance.SetGlobalFlag(flagName, flagValue);
	}
}
