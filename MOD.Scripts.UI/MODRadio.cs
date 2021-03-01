using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	class MODRadio
	{
		public static bool anyRadioPressed;
		string label;
		private GUIContent[] radioContents;
		MODStyleManager styleManager;
		int itemsPerRow;

		public MODRadio(string label, GUIContent[] radioContents, MODStyleManager styleManager, int itemsPerRow = 0) //Action<int> onRadioChange, 
		{
			this.label = label;
			this.radioContents = radioContents;
			this.itemsPerRow = radioContents.Length == 0 ? 1 : radioContents.Length;
			if (itemsPerRow != 0)
			{
				this.itemsPerRow = itemsPerRow;
			}
			this.styleManager = styleManager;
		}

		/// <summary>
		/// NOTE: only call this function within OnGUI()
		/// Displays the radio, calling onRadioChange when the user clicks on a different radio value.
		/// </summary>
		/// <param name="displayedRadio">Sets the currently displayed radio. Use "-1" for "None selected"</param>
		/// <returns>If radio did not change value, null is returned, otherwise the new value is returned.</returns>
		public int? OnGUIFragment(int displayedRadio)
		{
			GUILayout.Label(this.label, styleManager.Group.label);

			if(radioContents.Length == 0)
			{
				GUILayout.Label("MODRadio Error: this radio has no options!", styleManager.Group.label);
			}

			int i = GUILayout.SelectionGrid(displayedRadio, radioContents, itemsPerRow, styleManager.Group.modMenuSelectionGrid);
			if (i != displayedRadio)
			{
				MODRadio.anyRadioPressed = true;
				return i;
			}

			return null;
		}

		public void SetContents(GUIContent[] content)
		{
			this.radioContents = content;
		}

		public GUIContent[] GetContents() => this.radioContents;
	}
}
