using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using UnityEngine;

namespace MOD.Scripts.UI
{
	public class MODMainUIController : MonoBehaviour
	{
		private static string ADVModeNameFormat = "<size=+4>{0}\n</size>";

		private static int ADVModeWindowPosX = -170;

		private static int ADVModeWindowPosY = -300;

		private static int ADVModeWindowSizeX = 1150;

		private static int ADVModeWindowSizeY = 250;

		private static int ADVModeLineSpacing = 8;

		private static int ADVModeCharSpacing = 0;

		private static int ADVModeFontSize = 30;

		private static int ADVModeWindowMarginLeft = 60;

		private static int ADVModeWindowMarginTop = 30;

		private static int ADVModeWindowMarginRight = 50;

		private static int ADVModeWindowMarginBottom = 30;

		private static int ADVModeFontID = 1;

		private static string NVLModeNameFormat = "";

		private static int NVLModeWindowPosX = -170;

		private static int NVLModeWindowPosY = -10;

		private static int NVLModeWindowSizeX = 1240;

		private static int NVLModeWindowSizeY = 720;

		private static int NVLModeLineSpacing = 8;

		private static int NVLModeCharSpacing = 0;

		private static int NVLModeFontSize = 34;

		private static int NVLModeWindowMarginLeft = 60;

		private static int NVLModeWindowMarginTop = 30;

		private static int NVLModeWindowMarginRight = 50;

		private static int NVLModeWindowMarginBottom = 30;

		private static int NVLModeFontID = 1;

		private static string NVLADVModeNameFormat = "";

		private static int NVLADVModeWindowPosX = -170;

		private static int NVLADVModeWindowPosY = -10;

		private static int NVLADVModeWindowSizeX = 1240;

		private static int NVLADVModeWindowSizeY = 720;

		private static int NVLADVModeLineSpacing = 8;

		private static int NVLADVModeCharSpacing = 0;

		private static int NVLADVModeFontSize = 34;

		private static int NVLADVModeWindowMarginLeft = 60;

		private static int NVLADVModeWindowMarginTop = 30;

		private static int NVLADVModeWindowMarginRight = 50;

		private static int NVLADVModeWindowMarginBottom = 30;

		private static int NVLADVModeFontID = 1;

		// Ryukishi Mode Settings, with default values
		private static string RyukishiModeNameFormat = "";
		private static int RyukishiModeWindowPosX = 0;
		private static int RyukishiModeWindowPosY = 10;
		private static int RyukishiModeWindowSizeX = 1024;
		private static int RyukishiModeWindowSizeY = 768;
		private static int RyukishiModeWindowMarginLeft = 60;
		private static int RyukishiModeWindowMarginTop = 30;
		private static int RyukishiModeWindowMarginRight = 50;
		private static int RyukishiModeWindowMarginBottom = 30;
		private static int RyukishiModeFontID = 1;
		private static int RyukishiModeCharSpacing = 0;
		private static int RyukishiModeLineSpacing = 8;
		private static int RyukishiModeFontSize = 34;

		// Wide Mode/Ryukishi Mode Window Settings, with default values
		private static int WideModeGuiPosX = 0;
		private static int WideModeGuiPosY = 0;
		private static int RyukishiModeGuiPosX = 0;
		private static int RyukishiModeGuiPosY = 0;

		private void StoreRyukishiModeIfFlagSet()
		{
			if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() == 1)
			{
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 2); // Set NVL Line Mode
				RyukishiModeSettingStore();
			}
		}

		private void StoreRyukishiGuiPositionIfFlagSet()
		{
			if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() == 1)
			{
				RyukishiModeSettingStore();
			}
		}

		public void WideGuiPositionLoad(int posx, int posy)
		{
			WideModeGuiPosX = posx;
			WideModeGuiPosY = posy;
			if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode").IntValue() != 1)
			{
				WideGuiPositionStore();
			}

			// This ensures scripts without a "ModRyukishiSetGuiPosition" will still initialize using the default values. Can be moved elsewhere (must be before reaching normal gameplay)
			StoreRyukishiGuiPositionIfFlagSet();
		}

		public void RyukishiGuiPositionLoad(int posx, int posy)
		{
			RyukishiModeGuiPosX = posx;
			RyukishiModeGuiPosY = posy;
			StoreRyukishiGuiPositionIfFlagSet();
		}

		public void WideGuiPositionStore()
		{
			if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode43Aspect").IntValue() != 0)
			{
				RyukishiGuiPositionStore();
				return;
			}

			GameSystem.Instance.MainUIController.UpdateGuiPosition(WideModeGuiPosX, WideModeGuiPosY);
		}

		public void RyukishiGuiPositionStore()
		{
			GameSystem.Instance.MainUIController.UpdateGuiPosition(RyukishiModeGuiPosX, RyukishiModeGuiPosY);
		}

		public void ADVModeSettingLoad(string name, int posx, int posy, int sizex, int sizey, int mleft, int mtop, int mright, int mbottom, int font, int cspace, int lspace, int fsize)
		{
			ADVModeNameFormat = name;
			ADVModeWindowPosX = posx;
			ADVModeWindowPosY = posy;
			ADVModeWindowSizeX = sizex;
			ADVModeWindowSizeY = sizey;
			ADVModeWindowMarginLeft = mleft;
			ADVModeWindowMarginTop = mtop;
			ADVModeWindowMarginRight = mright;
			ADVModeWindowMarginBottom = mbottom;
			ADVModeFontID = font;
			ADVModeCharSpacing = cspace;
			ADVModeLineSpacing = lspace;
			ADVModeFontSize = fsize;
			if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() == 1)
			{
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 0);
				ADVModeSettingStore();
			}
		}

		public void NVLModeSettingLoad(string name, int posx, int posy, int sizex, int sizey, int mleft, int mtop, int mright, int mbottom, int font, int cspace, int lspace, int fsize)
		{
			NVLModeNameFormat = name;
			NVLModeWindowPosX = posx;
			NVLModeWindowPosY = posy;
			NVLModeWindowSizeX = sizex;
			NVLModeWindowSizeY = sizey;
			NVLModeWindowMarginLeft = mleft;
			NVLModeWindowMarginTop = mtop;
			NVLModeWindowMarginRight = mright;
			NVLModeWindowMarginBottom = mbottom;
			NVLModeFontID = font;
			NVLModeCharSpacing = cspace;
			NVLModeLineSpacing = lspace;
			NVLModeFontSize = fsize;
			if (BurikoMemory.Instance.GetGlobalFlag("GADVMode").IntValue() != 1)
			{
				BurikoMemory.Instance.SetGlobalFlag("GLinemodeSp", 2);
				NVLModeSettingStore();
			}

			// This ensures scripts without a "ModRyukishiModeSettingLoad" will still initialize using the default values. Can be moved elsewhere (must be before reaching normal gameplay)
			StoreRyukishiModeIfFlagSet();
		}

		public void NVLADVModeSettingLoad(string name, int posx, int posy, int sizex, int sizey, int mleft, int mtop, int mright, int mbottom, int font, int cspace, int lspace, int fsize)
		{
			NVLADVModeNameFormat = name;
			NVLADVModeWindowPosX = posx;
			NVLADVModeWindowPosY = posy;
			NVLADVModeWindowSizeX = sizex;
			NVLADVModeWindowSizeY = sizey;
			NVLADVModeWindowMarginLeft = mleft;
			NVLADVModeWindowMarginTop = mtop;
			NVLADVModeWindowMarginRight = mright;
			NVLADVModeWindowMarginBottom = mbottom;
			NVLADVModeFontID = font;
			NVLADVModeCharSpacing = cspace;
			NVLADVModeLineSpacing = lspace;
			NVLADVModeFontSize = fsize;
		}

		public void RyukishiModeSettingLoad(string name, int posx, int posy, int sizex, int sizey, int mleft, int mtop, int mright, int mbottom, int font, int cspace, int lspace, int fsize)
		{
			RyukishiModeNameFormat = name;
			RyukishiModeWindowPosX = posx;
			RyukishiModeWindowPosY = posy;
			RyukishiModeWindowSizeX = sizex;
			RyukishiModeWindowSizeY = sizey;
			RyukishiModeWindowMarginLeft = mleft;
			RyukishiModeWindowMarginTop = mtop;
			RyukishiModeWindowMarginRight = mright;
			RyukishiModeWindowMarginBottom = mbottom;
			RyukishiModeFontID = font;
			RyukishiModeCharSpacing = cspace;
			RyukishiModeLineSpacing = lspace;
			RyukishiModeFontSize = fsize;

			StoreRyukishiModeIfFlagSet();
		}

		public void ADVModeSettingStore()
		{
			string aDVModeNameFormat = ADVModeNameFormat;
			int aDVModeWindowPosX = ADVModeWindowPosX;
			int aDVModeWindowPosY = ADVModeWindowPosY;
			int aDVModeWindowSizeX = ADVModeWindowSizeX;
			int aDVModeWindowSizeY = ADVModeWindowSizeY;
			int aDVModeWindowMarginLeft = ADVModeWindowMarginLeft;
			int aDVModeWindowMarginTop = ADVModeWindowMarginTop;
			int aDVModeWindowMarginRight = ADVModeWindowMarginRight;
			int aDVModeWindowMarginBottom = ADVModeWindowMarginBottom;
			int aDVModeFontID = ADVModeFontID;
			int aDVModeCharSpacing = ADVModeCharSpacing;
			int aDVModeLineSpacing = ADVModeLineSpacing;
			int aDVModeFontSize = ADVModeFontSize;

			if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode43Aspect").IntValue() != 0)
			{
				aDVModeWindowPosX = RyukishiModeWindowPosX - 20;
				aDVModeWindowSizeX = RyukishiModeWindowSizeX - 60;
				aDVModeFontSize = ADVModeFontSize * 85 / 100;
			}

			GameSystem.Instance.TextController.SetNameFormat(aDVModeNameFormat);
			GameSystem.Instance.MainUIController.SetWindowPos(aDVModeWindowPosX, aDVModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(aDVModeWindowSizeX, aDVModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(aDVModeWindowMarginLeft, aDVModeWindowMarginTop, aDVModeWindowMarginRight, aDVModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(aDVModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(aDVModeCharSpacing);
			GameSystem.Instance.TextController.SetLineSpacing(aDVModeLineSpacing);
			GameSystem.Instance.TextController.SetFontSize(aDVModeFontSize);

			// TODO: Properly implement different settings for Japanese mode (but perhaps this only applies when using unmodded ADV window?)
			GameSystem.Instance.TextController.SetNameFormatJp(aDVModeNameFormat);
			GameSystem.Instance.TextController.SetLineSpacingJp(aDVModeLineSpacing);
			GameSystem.Instance.TextController.SetJpFontSize(aDVModeFontSize);
		}

		public void NVLModeSettingStore()
		{
			string nVLModeNameFormat = NVLModeNameFormat;
			int nVLModeWindowPosX = NVLModeWindowPosX;
			int nVLModeWindowPosY = NVLModeWindowPosY;
			int nVLModeWindowSizeX = NVLModeWindowSizeX;
			int nVLModeWindowSizeY = NVLModeWindowSizeY;
			int nVLModeWindowMarginLeft = NVLModeWindowMarginLeft;
			int nVLModeWindowMarginTop = NVLModeWindowMarginTop;
			int nVLModeWindowMarginRight = NVLModeWindowMarginRight;
			int nVLModeWindowMarginBottom = NVLModeWindowMarginBottom;
			int nVLModeFontID = NVLModeFontID;
			int nVLModeCharSpacing = NVLModeCharSpacing;
			int nVLModeLineSpacing = NVLModeLineSpacing;
			int nVLModeFontSize = NVLModeFontSize;

			if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode43Aspect").IntValue() != 0)
			{
				nVLModeWindowPosX = RyukishiModeWindowPosX;
				nVLModeWindowSizeX = RyukishiModeWindowSizeX - 100;
			}

			GameSystem.Instance.TextController.SetNameFormat(nVLModeNameFormat);
			GameSystem.Instance.MainUIController.SetWindowPos(nVLModeWindowPosX, nVLModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(nVLModeWindowSizeX, nVLModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(nVLModeWindowMarginLeft, nVLModeWindowMarginTop, nVLModeWindowMarginRight, nVLModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(nVLModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(nVLModeCharSpacing);
			GameSystem.Instance.TextController.SetLineSpacing(nVLModeLineSpacing);
			GameSystem.Instance.TextController.SetFontSize(nVLModeFontSize);

			// TODO: Properly implement different settings for Japanese mode (but perhaps this only applies when using unmodded ADV window?)
			GameSystem.Instance.TextController.SetNameFormatJp(nVLModeNameFormat);
			GameSystem.Instance.TextController.SetLineSpacingJp(nVLModeLineSpacing);
			GameSystem.Instance.TextController.SetJpFontSize(nVLModeFontSize);
		}

		public void NVLADVModeSettingStore()
		{
			if (BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode43Aspect").IntValue() != 0)
			{
				RyukishiModeSettingStore();
				return;
			}

			string nVLADVModeNameFormat = NVLADVModeNameFormat;
			int nVLADVModeWindowPosX = NVLADVModeWindowPosX;
			int nVLADVModeWindowPosY = NVLADVModeWindowPosY;
			int nVLADVModeWindowSizeX = NVLADVModeWindowSizeX;
			int nVLADVModeWindowSizeY = NVLADVModeWindowSizeY;
			int nVLADVModeWindowMarginLeft = NVLADVModeWindowMarginLeft;
			int nVLADVModeWindowMarginTop = NVLADVModeWindowMarginTop;
			int nVLADVModeWindowMarginRight = NVLADVModeWindowMarginRight;
			int nVLADVModeWindowMarginBottom = NVLADVModeWindowMarginBottom;
			int nVLADVModeFontID = NVLADVModeFontID;
			int nVLADVModeCharSpacing = NVLADVModeCharSpacing;
			int nVLADVModeLineSpacing = NVLADVModeLineSpacing;
			int nVLADVModeFontSize = NVLADVModeFontSize;
			GameSystem.Instance.TextController.SetNameFormat(nVLADVModeNameFormat);
			GameSystem.Instance.MainUIController.SetWindowPos(nVLADVModeWindowPosX, nVLADVModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(nVLADVModeWindowSizeX, nVLADVModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(nVLADVModeWindowMarginLeft, nVLADVModeWindowMarginTop, nVLADVModeWindowMarginRight, nVLADVModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(nVLADVModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(nVLADVModeCharSpacing);
			GameSystem.Instance.TextController.SetLineSpacing(nVLADVModeLineSpacing);
			GameSystem.Instance.TextController.SetFontSize(nVLADVModeFontSize);

			// TODO: Properly implement different settings for Japanese mode (but perhaps this only applies when using unmodded ADV window?)
			GameSystem.Instance.TextController.SetNameFormatJp(nVLADVModeNameFormat);
			GameSystem.Instance.TextController.SetLineSpacingJp(nVLADVModeLineSpacing);
			GameSystem.Instance.TextController.SetJpFontSize(nVLADVModeFontSize);
		}

		public void RyukishiModeSettingStore()
		{
			string ryukishiModeNameFormat = RyukishiModeNameFormat;
			int ryukishiModeWindowPosX = RyukishiModeWindowPosX;
			int ryukishiModeWindowPosY = RyukishiModeWindowPosY;
			int ryukishiModeWindowSizeX = RyukishiModeWindowSizeX;
			int ryukishiModeWindowSizeY = RyukishiModeWindowSizeY;
			int ryukishiModeWindowMarginLeft = RyukishiModeWindowMarginLeft;
			int ryukishiModeWindowMarginTop = RyukishiModeWindowMarginTop;
			int ryukishiModeWindowMarginRight = RyukishiModeWindowMarginRight;
			int ryukishiModeWindowMarginBottom = RyukishiModeWindowMarginBottom;
			int ryukishiModeFontID = RyukishiModeFontID;
			int ryukishiModeCharSpacing = RyukishiModeCharSpacing;
			int ryukishiModeLineSpacing = RyukishiModeLineSpacing;
			int ryukishiModeFontSize = RyukishiModeFontSize;
			GameSystem.Instance.TextController.SetNameFormat(ryukishiModeNameFormat);
			GameSystem.Instance.MainUIController.SetWindowPos(ryukishiModeWindowPosX, ryukishiModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(ryukishiModeWindowSizeX, ryukishiModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(ryukishiModeWindowMarginLeft, ryukishiModeWindowMarginTop, ryukishiModeWindowMarginRight, ryukishiModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(ryukishiModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(ryukishiModeCharSpacing);
			GameSystem.Instance.TextController.SetLineSpacing(ryukishiModeLineSpacing);
			GameSystem.Instance.TextController.SetFontSize(ryukishiModeFontSize);

			// TODO: Properly implement different settings for Japanese mode (but perhaps this only applies when using unmodded ADV window?)
			GameSystem.Instance.TextController.SetNameFormatJp(ryukishiModeNameFormat);
			GameSystem.Instance.TextController.SetLineSpacingJp(ryukishiModeLineSpacing);
			GameSystem.Instance.TextController.SetJpFontSize(ryukishiModeFontSize);
		}

		public void DebugFontChangerSettingStore()
		{
			string aDVModeNameFormat = ADVModeNameFormat;
			int aDVModeWindowPosX = ADVModeWindowPosX;
			int nVLModeWindowPosY = NVLModeWindowPosY;
			int aDVModeWindowSizeX = ADVModeWindowSizeX;
			int nVLModeWindowSizeY = NVLModeWindowSizeY;
			int aDVModeWindowMarginLeft = ADVModeWindowMarginLeft;
			int nVLModeWindowMarginTop = NVLModeWindowMarginTop;
			int aDVModeWindowMarginRight = ADVModeWindowMarginRight;
			int nVLModeWindowMarginBottom = NVLModeWindowMarginBottom;
			int aDVModeFontID = ADVModeFontID;
			int aDVModeCharSpacing = ADVModeCharSpacing;
			int aDVModeLineSpacing = ADVModeLineSpacing;
			int aDVModeFontSize = ADVModeFontSize;
			GameSystem.Instance.TextController.SetNameFormat(aDVModeNameFormat);
			GameSystem.Instance.MainUIController.SetWindowPos(aDVModeWindowPosX, nVLModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(aDVModeWindowSizeX, nVLModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(aDVModeWindowMarginLeft, nVLModeWindowMarginTop, aDVModeWindowMarginRight, nVLModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(aDVModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(aDVModeCharSpacing);
			GameSystem.Instance.TextController.SetLineSpacing(aDVModeLineSpacing);
			GameSystem.Instance.TextController.SetFontSize(aDVModeFontSize);

			// TODO: Properly implement different settings for Japanese mode (but perhaps this only applies when using unmodded ADV window?)
			GameSystem.Instance.TextController.SetNameFormatJp(aDVModeNameFormat);
			GameSystem.Instance.TextController.SetLineSpacingJp(aDVModeLineSpacing);
			GameSystem.Instance.TextController.SetJpFontSize(aDVModeFontSize);
		}
	}
}
