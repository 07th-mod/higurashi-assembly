using MOD.Scripts.Core.Scene;
using MOD.Scripts.Core.TextWindow;
using MOD.Scripts.UI;

namespace MOD.Scripts.Core
{
	public class MODSystem
	{
		public readonly MODMainUIController modMainUIController = new MODMainUIController();

		public readonly MODSceneController modSceneController = new MODSceneController();

		public readonly MODTextController modTextController = new MODTextController();

		public readonly MODTextureController modTextureController = fixedMODTextureControllerInstance;

		private static MODTextureController fixedMODTextureControllerInstance = new MODTextureController();

		public static MODSystem instance => new MODSystem();
	}
}
