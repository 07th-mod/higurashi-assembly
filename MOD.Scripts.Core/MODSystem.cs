using MOD.Scripts.Core.Config;
using MOD.Scripts.Core.Scene;
using MOD.Scripts.Core.TextWindow;
using MOD.Scripts.UI;
using MOD.Scripts.UI.Tips;
using UnityEngine;
using System.IO;

namespace MOD.Scripts.Core
{
	public class MODSystem
	{
		public readonly MODMainUIController modMainUIController = new MODMainUIController();

		public readonly MODSceneController modSceneController = new MODSceneController();

		public readonly MODTextController modTextController = new MODTextController();

		public readonly MODTextureController modTextureController = fixedMODTextureControllerInstance;

		private static readonly MODTextureController fixedMODTextureControllerInstance = new MODTextureController();

		public static MODSystem instance => new MODSystem();

		public readonly MODConfig modConfig = fixedMODConfigInstance;

		private static readonly MODConfig fixedMODConfigInstance = MODConfigManager.Read();

		public readonly MODTipsController modTipsController = fixedMODTipsControllerInstance;

		private static readonly MODTipsController fixedMODTipsControllerInstance = new MODTipsController();

		public static string BaseDirectory
		{
			get
			{
				if (Application.platform == RuntimePlatform.OSXPlayer)
				{
					return Path.Combine(Application.dataPath, "Resources/Data");
				}
				else
				{
					return Application.dataPath;
				}
			}
		}
	}
}
