using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.UI
{
	public class MODStyleManager
	{
		private static MODStyleManager _instance;
		/// <summary>
		/// Only call this function from within an OnGUI() context, as Unity gives an error if you try to create styles elsewhere.
		/// </summary>
		public static MODStyleManager OnGUIInstance => _instance ?? (_instance = new MODStyleManager());

		public class StyleGroup
		{
			public int menuHeight;
			public int menuWidth;
			public float toolTipShrinkage;
			public float toastWidth;
			public GUIStyle modMenuSelectionGrid;   // Used for SelectionGrid widgets
			public GUIStyle errorLabel;
			public GUIStyle button;
			public GUIStyle selectedButton;
			public GUIStyle label;             //Used for normal Label widgets
			public GUIStyle labelRightAlign;
			public GUIStyle headingLabel;
			public GUIStyle upperLeftHeadingLabel;
			public GUIStyle bigToastLabelStyle;     // Styles used for Toasts (text popups)
			public GUIStyle smallToastLabelStyle;
		}

		// Styles used for the Mod menu
		public GUIStyle modMenuAreaStyle;       //Used for Area widgets
		public GUIStyle modMenuAreaStyleLight;
		public GUIStyle modMenuAreaStyleTransparent;

		public Texture2D modGUIBackgroundTexture;
		public Texture2D modGUIBackgroundTextureTransparent;
		public Texture2D modGUIBackgroundTextureLight;

		// These style groups use the naming convention:
		// style[VERTICAL_SCREEN_HEIGHT]_[MENU_WIDTH]x[MENU_HEIGHT]
		StyleGroup style1080_1200x950;
		StyleGroup style720_1000x660;
		StyleGroup style720_960x660;
		StyleGroup style480_850x480;
		StyleGroup style480_640x480;
		StyleGroup style2160_2400x1900;

		public StyleGroup Group
		{
			get
			{
				if (Screen.height >= 2160 && Screen.width >= 2400)
				{
					return style2160_2400x1900;
				}
				else if (Screen.height >= 1080 && Screen.width >= 1200)
				{
					return style1080_1200x950;
				}
				else if (Screen.height >= 720 && Screen.width >= 1000)
				{
					return style720_1000x660;
				}
				else if(Screen.height >= 720 && Screen.width >= 960)
				{
					return style720_960x660;
				}
				else if(Screen.height >= 480 && Screen.width >= 850)
				{
					return style480_850x480;
				}
				else
				{
					return style480_640x480;
				}
			}
		}

		public float baseFontSize = 14;

		private MODStyleManager()
		{
			int width = 10;
			int height = 10;
			Color[] pix = new Color[width * height];

			for (int i = 0; i < pix.Length; i++)
			{
				pix[i] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			}

			modGUIBackgroundTextureTransparent = new Texture2D(width, height);
			modGUIBackgroundTextureTransparent.SetPixels(pix);
			modGUIBackgroundTextureTransparent.Apply();

			for (int i = 0; i < pix.Length; i++)
			{
				pix[i] = new Color(0.7f, 0.7f, 0.7f);
			}

			for (int y = 2; y < height-2; y++)
			{
				for (int x = 2; x < width-2; x++)
				{
					pix[x + y * width] = new Color(0.0f, 0.0f, 0.0f, 0.9f);
				}
			}

			modGUIBackgroundTexture = new Texture2D(width, height);
			modGUIBackgroundTexture.SetPixels(pix);
			modGUIBackgroundTexture.Apply();

			for (int i = 0; i < pix.Length; i++)
			{
				pix[i] = new Color(1.0f, 1.0f, 1.0f, 0.7f);
			}

			for (int y = 2; y < height-2; y++)
			{
				for (int x = 2; x < width-2; x++)
				{
					pix[x + y * width] = new Color(0.0f, 0.0f, 0.0f, 0.7f);
				}
			}

			modGUIBackgroundTextureLight = new Texture2D(width, height);
			modGUIBackgroundTextureLight.SetPixels(pix);
			modGUIBackgroundTextureLight.Apply();


			style480_640x480 = GenerateWidgetStyles(
				menuWidth: 640,
				menuHeight: 480,
				guiScale: .8f,
				margin: new RectOffset(0, 0, 0, 0),
				padding: new RectOffset(0, 0, 0, 0),
				toolTipShrinkage: .15f
			);

			style480_850x480 = GenerateWidgetStyles(
				menuWidth: 850,
				menuHeight: 480,
				guiScale: .9f,
				margin: new RectOffset(0, 0, 0, 0),
				padding: new RectOffset(0, 0, 0, 0)
			);

			style720_960x660 = GenerateWidgetStyles(
				menuWidth: 960,
				menuHeight: 660,
				guiScale: 1.1f,
				margin: new RectOffset(1, 1, 1, 1),
				padding: new RectOffset(1, 1, 1, 1)
			);

			style720_1000x660 = GenerateWidgetStyles(
				menuWidth: 1000,
				menuHeight: 660,
				guiScale: 1.1f,
				margin: new RectOffset(1, 1, 1, 1),
				padding: new RectOffset(1, 1, 1, 1)
			);

			style1080_1200x950 = GenerateWidgetStyles(
				menuWidth: 1200,
				menuHeight: 950,
				guiScale: 1.25f,
				margin: new RectOffset(2, 2, 2, 2),
				padding: new RectOffset(2, 2, 2, 2)
			);

			style2160_2400x1900 = GenerateWidgetStyles(
				menuWidth: 2400,
				menuHeight: 1900,
				guiScale: 2.5f,
				margin: new RectOffset(4, 4, 4, 4),
				padding: new RectOffset(4, 4, 4, 4)
			);

			//OnGUIGetLabelStyle();
			OnGUIGetModStyle();
		}

		private void OnGUIGetModStyle()
		{
			modMenuAreaStyle = new GUIStyle(GUI.skin.box) //Copy the default style for 'box' as a base
			{
				//alignment = TextAnchor.UpperCenter,
				//fontSize = 40,
				//fontStyle = FontStyle.Bold,
			};
			modMenuAreaStyle.normal.background = modGUIBackgroundTexture;
			modMenuAreaStyle.normal.textColor = Color.white;
			modMenuAreaStyle.wordWrap = true;

			modMenuAreaStyleLight = new GUIStyle(modMenuAreaStyle);
			modMenuAreaStyleLight.normal.background = modGUIBackgroundTextureLight;

			modMenuAreaStyleTransparent = new GUIStyle(modMenuAreaStyle);
			modMenuAreaStyleTransparent.normal.background = modGUIBackgroundTextureTransparent;
		}

		private StyleGroup GenerateWidgetStyles(int menuWidth, int menuHeight, float guiScale, RectOffset margin, RectOffset padding, float toolTipShrinkage = 0)
		{
			// Button style
			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fontSize = Mathf.RoundToInt(guiScale * baseFontSize);

			// Selected button style (to show if a the option a button represents is activated)
			GUIStyle selectedButtonStyle = new GUIStyle(buttonStyle);
			selectedButtonStyle.normal.textColor = Color.green;
			selectedButtonStyle.hover.textColor = Color.green;

			// Label style
			GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = Mathf.RoundToInt(guiScale * baseFontSize),
				margin = margin,
				padding = padding,
			};

			GUIStyle labelStyleRightAlign = new GUIStyle(labelStyle)
			{
				alignment = TextAnchor.MiddleRight,
			};

			// Heading text style
			GUIStyle headingLabelStyle = new GUIStyle(labelStyle)
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.LowerCenter,
			};
			headingLabelStyle.padding.top *= 5;

			GUIStyle upperLeftHeadingLabelStyle = new GUIStyle(headingLabelStyle);
			upperLeftHeadingLabelStyle.alignment = TextAnchor.UpperLeft;

			// Menu selection grid/radio
			GUIStyle modMenuSelectionGrid = new GUIStyle(GUI.skin.button) //Copy the default style for 'box' as a base
			{
				//alignment = TextAnchor.UpperCenter,
				fontSize = Mathf.RoundToInt(guiScale * baseFontSize),
			};
			modMenuSelectionGrid.onHover.textColor = Color.green;
			modMenuSelectionGrid.onNormal.textColor = Color.green; // Color of a selected option

			// Error label (this is used for the description on the right hand side if the game encounters an error
			GUIStyle errorLabelStyle = new GUIStyle(GUI.skin.label);
			errorLabelStyle.normal.textColor = Color.red;
			errorLabelStyle.fontSize = Mathf.RoundToInt(guiScale * baseFontSize);

			// Toast popup styles
			GUIStyle bigToastLabelStyle = new GUIStyle(GUI.skin.box) //Copy the default style for 'box' as a base
			{
				alignment = TextAnchor.UpperCenter,
				fontSize = Mathf.RoundToInt(guiScale * 3 * baseFontSize),
				fontStyle = FontStyle.Bold,
				wordWrap = true,
			};
			bigToastLabelStyle.normal.background = modGUIBackgroundTexture;
			bigToastLabelStyle.normal.textColor = Color.white;

			GUIStyle smallToastLabelStyle = new GUIStyle(bigToastLabelStyle)
			{
				fontSize = bigToastLabelStyle.fontSize / 2,
			};

			return new StyleGroup()
			{
				menuWidth = menuWidth,
				menuHeight = menuHeight,
				toastWidth = menuWidth,
				modMenuSelectionGrid = modMenuSelectionGrid,
				errorLabel = errorLabelStyle,
				button = buttonStyle,
				selectedButton = selectedButtonStyle,
				label = labelStyle,
				labelRightAlign = labelStyleRightAlign,
				headingLabel = headingLabelStyle,
				upperLeftHeadingLabel = upperLeftHeadingLabelStyle,
				toolTipShrinkage = toolTipShrinkage,
				bigToastLabelStyle = bigToastLabelStyle,
				smallToastLabelStyle = smallToastLabelStyle,
			};
		}
	}
}
