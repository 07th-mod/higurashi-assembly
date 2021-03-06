using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	static class MODMenuCommon
	{
		public static void Label(string label)
		{
			GUILayout.Label(label, MODStyleManager.OnGUIInstance.Group.label);
		}

		public static void HeadingLabel(string label)
		{
			GUILayout.Label(label, MODStyleManager.OnGUIInstance.Group.headingLabel);
		}

		public static bool Button(GUIContent guiContent, bool selected = false)
		{
			return GUILayout.Button(guiContent, selected ? MODStyleManager.OnGUIInstance.Group.selectedButton : MODStyleManager.OnGUIInstance.Group.button);
		}

		public static int GetGlobal(string flagName) => BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue();
		public static void SetGlobal(string flagName, int flagValue) => BurikoMemory.Instance.SetGlobalFlag(flagName, flagValue);
	}
}
