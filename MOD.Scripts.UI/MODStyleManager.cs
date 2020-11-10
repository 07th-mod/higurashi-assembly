using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	public class MODStyleManager
	{
		public GUIStyle labelStyle;
		public GUIStyle smallLabelStyle;
		public GUIStyle modGUIStyle;
		public GUIStyle modSelectorStyle;
		public GUIStyle errorLabelStyle;
		public Texture2D modGUIBackgroundTexture;

		/// <summary>
		/// Note that this static construct is automatically created
		/// </summary>
		public MODStyleManager()
		{
			int width = 1;
			int height = 1;
			Color[] pix = new Color[width * height];
			for (int i = 0; i < pix.Length; i++)
			{
				pix[i] = new Color(0.0f, 0.0f, 0.0f, 0.9f);
			}
			modGUIBackgroundTexture = new Texture2D(width, height);
			modGUIBackgroundTexture.SetPixels(pix);
			modGUIBackgroundTexture.Apply();

			OnGUIGetLabelStyle();
			OnGUIGetModStyle();
			OnGUIGetSelectorStyle();
		}

		// This sets up the style of the toast notification (mostly to make the font bigger and the text anchor location)
		// From what I've read, The GUIStyle must be initialized in OnGUI(), otherwise
		// GUI.skin.label will not be defined, so please do not move this part elsewhere without testing it.
		private void OnGUIGetLabelStyle()
		{
			if (labelStyle == null)
			{
				labelStyle = new GUIStyle(GUI.skin.box) //Copy the default style for 'box' as a base
				{
					alignment = TextAnchor.UpperCenter,
					fontSize = 40,
					fontStyle = FontStyle.Bold,
				};
				labelStyle.normal.background = modGUIBackgroundTexture;
				labelStyle.normal.textColor = Color.white;
			}

			if (smallLabelStyle == null)
			{
				smallLabelStyle = new GUIStyle(labelStyle)
				{
					fontSize = 20,
				};
			}

			if (errorLabelStyle == null)
			{
				errorLabelStyle = new GUIStyle(GUI.skin.label);
				errorLabelStyle.normal.textColor = Color.red;
			}
		}

		private void OnGUIGetModStyle()
		{
			if (modGUIStyle == null)
			{
				modGUIStyle = new GUIStyle(GUI.skin.box) //Copy the default style for 'box' as a base
				{
					//alignment = TextAnchor.UpperCenter,
					//fontSize = 40,
					//fontStyle = FontStyle.Bold,
				};
				modGUIStyle.normal.background = modGUIBackgroundTexture;
				modGUIStyle.normal.textColor = Color.white;
				modGUIStyle.wordWrap = true;
			}
		}

		private void OnGUIGetSelectorStyle()
		{
			if (modSelectorStyle == null)
			{
				modSelectorStyle = new GUIStyle(GUI.skin.button) //Copy the default style for 'box' as a base
				{
					//alignment = TextAnchor.UpperCenter,
					//fontSize = 40,
					//fontStyle = FontStyle.Bold,
				};
				modSelectorStyle.onHover.textColor = Color.green;
				modSelectorStyle.onNormal.textColor = Color.green; // Color of a selected option
			}
		}
	}
}
