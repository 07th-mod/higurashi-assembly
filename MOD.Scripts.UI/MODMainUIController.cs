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

		public void FixFullscreenUIScale()
		{
			float aspectRatio = GameSystem.Instance.AspectRatio;
			if (GameSystem.Instance.IsFullscreen)
			{
				Resolution resolution = GameSystem.Instance.GetFullscreenResolution();
				aspectRatio = resolution.width / (float)resolution.height;
			}
			float scale = aspectRatio / GameSystem.Instance.AspectRatio;
			GameSystem.Instance.MainUIController.UpdateGuiScale(scale, scale);
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
