using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using UnityEngine;

namespace MOD.Scripts.UI
{
	public class MODMainUIController : MonoBehaviour
	{
		private static string ADVModeNameFormat;

		private static int ADVModeWindowPosX;

		private static int ADVModeWindowPosY;

		private static int ADVModeWindowSizeX;

		private static int ADVModeWindowSizeY;

		private static int ADVModeLineSpacing;

		private static int ADVModeCharSpacing;

		private static int ADVModeFontSize;

		private static int ADVModeWindowMarginLeft;

		private static int ADVModeWindowMarginTop;

		private static int ADVModeWindowMarginRight;

		private static int ADVModeWindowMarginBottom;

		private static int ADVModeFontID;

		private static string NVLModeNameFormat;

		private static int NVLModeWindowPosX;

		private static int NVLModeWindowPosY;

		private static int NVLModeWindowSizeX;

		private static int NVLModeWindowSizeY;

		private static int NVLModeLineSpacing;

		private static int NVLModeCharSpacing;

		private static int NVLModeFontSize;

		private static int NVLModeWindowMarginLeft;

		private static int NVLModeWindowMarginTop;

		private static int NVLModeWindowMarginRight;

		private static int NVLModeWindowMarginBottom;

		private static int NVLModeFontID;

		private static string NVLADVModeNameFormat;

		private static int NVLADVModeWindowPosX;

		private static int NVLADVModeWindowPosY;

		private static int NVLADVModeWindowSizeX;

		private static int NVLADVModeWindowSizeY;

		private static int NVLADVModeLineSpacing;

		private static int NVLADVModeCharSpacing;

		private static int NVLADVModeFontSize;

		private static int NVLADVModeWindowMarginLeft;

		private static int NVLADVModeWindowMarginTop;

		private static int NVLADVModeWindowMarginRight;

		private static int NVLADVModeWindowMarginBottom;

		private static int NVLADVModeFontID;

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
			GameSystem.Instance.TextController.NameFormat = aDVModeNameFormat;
			GameSystem.Instance.MainUIController.SetWindowPos(aDVModeWindowPosX, aDVModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(aDVModeWindowSizeX, aDVModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(aDVModeWindowMarginLeft, aDVModeWindowMarginTop, aDVModeWindowMarginRight, aDVModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(aDVModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(aDVModeCharSpacing);
			GameSystem.Instance.MainUIController.SetLineSpacing(aDVModeLineSpacing);
			GameSystem.Instance.MainUIController.SetFontSize(aDVModeFontSize);
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
			GameSystem.Instance.TextController.NameFormat = nVLModeNameFormat;
			GameSystem.Instance.MainUIController.SetWindowPos(nVLModeWindowPosX, nVLModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(nVLModeWindowSizeX, nVLModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(nVLModeWindowMarginLeft, nVLModeWindowMarginTop, nVLModeWindowMarginRight, nVLModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(nVLModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(nVLModeCharSpacing);
			GameSystem.Instance.MainUIController.SetLineSpacing(nVLModeLineSpacing);
			GameSystem.Instance.MainUIController.SetFontSize(nVLModeFontSize);
		}

		public void NVLADVModeSettingStore()
		{
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
			GameSystem.Instance.TextController.NameFormat = nVLADVModeNameFormat;
			GameSystem.Instance.MainUIController.SetWindowPos(nVLADVModeWindowPosX, nVLADVModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(nVLADVModeWindowSizeX, nVLADVModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(nVLADVModeWindowMarginLeft, nVLADVModeWindowMarginTop, nVLADVModeWindowMarginRight, nVLADVModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(nVLADVModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(nVLADVModeCharSpacing);
			GameSystem.Instance.MainUIController.SetLineSpacing(nVLADVModeLineSpacing);
			GameSystem.Instance.MainUIController.SetFontSize(nVLADVModeFontSize);
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
			GameSystem.Instance.TextController.NameFormat = ryukishiModeNameFormat;
			GameSystem.Instance.MainUIController.SetWindowPos(ryukishiModeWindowPosX, ryukishiModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(ryukishiModeWindowSizeX, ryukishiModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(ryukishiModeWindowMarginLeft, ryukishiModeWindowMarginTop, ryukishiModeWindowMarginRight, ryukishiModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(ryukishiModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(ryukishiModeCharSpacing);
			GameSystem.Instance.MainUIController.SetLineSpacing(ryukishiModeLineSpacing);
			GameSystem.Instance.MainUIController.SetFontSize(ryukishiModeFontSize);
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
			GameSystem.Instance.TextController.NameFormat = aDVModeNameFormat;
			GameSystem.Instance.MainUIController.SetWindowPos(aDVModeWindowPosX, nVLModeWindowPosY);
			GameSystem.Instance.MainUIController.SetWindowSize(aDVModeWindowSizeX, nVLModeWindowSizeY);
			GameSystem.Instance.MainUIController.SetWindowMargins(aDVModeWindowMarginLeft, nVLModeWindowMarginTop, aDVModeWindowMarginRight, nVLModeWindowMarginBottom);
			GameSystem.Instance.MainUIController.ChangeFontId(aDVModeFontID);
			GameSystem.Instance.MainUIController.SetCharSpacing(aDVModeCharSpacing);
			GameSystem.Instance.MainUIController.SetLineSpacing(aDVModeLineSpacing);
			GameSystem.Instance.MainUIController.SetFontSize(aDVModeFontSize);
		}
	}
}
