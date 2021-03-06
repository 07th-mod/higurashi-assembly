using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODMenuCommon
	{
		MODStyleManager styleManager;
		public MODMenuCommon(MODStyleManager styleManager)
		{
			this.styleManager = styleManager;
		}

		public void Label(string label)
		{
			GUILayout.Label(label, styleManager.Group.label);
		}

		public void HeadingLabel(string label)
		{
			GUILayout.Label(label, styleManager.Group.headingLabel);
		}

		public static bool Button(GUIContent guiContent, MODStyleManager styleManager, bool selected = false)
		{
			return GUILayout.Button(guiContent, selected ? styleManager.Group.selectedButton : styleManager.Group.button);
		}
		public bool Button(GUIContent guiContent, bool selected = false) => Button(guiContent, styleManager, selected);

		public int GetGlobal(string flagName) => BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue();
		public void SetGlobal(string flagName, int flagValue) => BurikoMemory.Instance.SetGlobalFlag(flagName, flagValue);
	}
}
