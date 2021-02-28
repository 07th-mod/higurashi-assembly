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
		bool anyButtonPressed;
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

		public bool Button(GUIContent guiContent, bool selected = false)
		{
			if (GUILayout.Button(guiContent, selected ? styleManager.Group.selectedButton : styleManager.Group.button))
			{
				anyButtonPressed = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		public int GetGlobal(string flagName) => BurikoMemory.Instance.GetGlobalFlag(flagName).IntValue();
		public void SetGlobal(string flagName, int flagValue) => BurikoMemory.Instance.SetGlobalFlag(flagName, flagValue);
	}
}
