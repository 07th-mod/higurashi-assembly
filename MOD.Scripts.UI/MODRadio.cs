using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MOD.Scripts.UI.MODMenuCommon;

namespace MOD.Scripts.UI
{
	class MODRadio
	{
		public static bool anyRadioPressed;
		string label;
		private GUIContent[] radioContents;
		int itemsPerRow;
		bool asButtons;

		/// <summary>
		/// Create a radio widget
		/// </summary>
		/// <param name="label">The heading label for this radio widget</param>
		/// <param name="radioContents">The button name and hover tooltip for each option</param>
		/// <param name="itemsPerRow">The number of buttons per row</param>
		/// <param name="asButtons">Implement this as Buttons rather than SelectionGrid, which allows clicking on an already selected item</param>
		public MODRadio(string label, GUIContent[] radioContents, int itemsPerRow = 0, bool asButtons = false)
		{
			this.label = label;
			this.radioContents = radioContents;
			this.itemsPerRow = radioContents.Length == 0 ? 1 : radioContents.Length;
			if (itemsPerRow != 0)
			{
				this.itemsPerRow = itemsPerRow;
			}
			this.asButtons = asButtons;
		}

		/// <summary>
		/// NOTE: only call this function within OnGUI()
		/// Displays the radio, calling onRadioChange when the user clicks on a different radio value.
		/// </summary>
		/// <param name="displayedRadio">Sets the currently displayed radio. Use "-1" for "None selected"</param>
		/// <returns>
		/// If asButtons is false, If radio did not change value, null is returned, otherwise the new value is returned.
		/// If asButtons is true, returns the clicked button's index, even if the radio did not change. If nothing clicked, null returned.
		/// </returns>
		public int? OnGUIFragment(int displayedRadio, bool hideLabel = false)
		{
			MODStyleManager styleManager = MODStyleManager.OnGUIInstance;
			if (!hideLabel)
			{
				GUILayout.Label(this.label, styleManager.Group.label);
			}

			if(radioContents.Length == 0)
			{
				GUILayout.Label("MODRadio Error: this radio has no options!", styleManager.Group.label);
			}

			if(asButtons)
			{
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				for(int i = 0; i < radioContents.Length; i++)
				{
					if ((i % itemsPerRow) == 0 && i != 0)
					{
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
					}

					if (Button(radioContents[i], selected: i == displayedRadio))
					{
						MODRadio.anyRadioPressed = true;
						return i;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			else
			{
				int i = GUILayout.SelectionGrid(displayedRadio, radioContents, itemsPerRow, styleManager.Group.modMenuSelectionGrid);
				if (i != displayedRadio)
				{
					MODRadio.anyRadioPressed = true;
					return i;
				}
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
