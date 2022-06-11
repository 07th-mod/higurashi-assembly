using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class SceneControllerOld : MonoBehaviour
	{
		private SceneOld[] scenes = new SceneOld[2];

		private GameSystem gameSystem;

		private GameObject effectCamera;

		private const int NumCharLayers = 32;

		private int foregroundScene;

		private int backgroundScene = 1;

		private void SwapScenes()
		{
			if (foregroundScene == 0)
			{
				foregroundScene = 1;
				backgroundScene = 0;
			}
			else
			{
				foregroundScene = 0;
				backgroundScene = 1;
			}
		}

		public void HideFilmEffector(float time, bool isBlocking)
		{
			effectCamera.GetComponent<FilmEffector>().FadeOut(time, isBlocking);
		}

		public void CreateFilmEffector(int effecttype, Color targetColor, int targetpower, int style, float length, bool isBlocking)
		{
			effectCamera.GetComponent<Camera>().enabled = true;
			effectCamera.AddComponent<FilmEffector>().Prepare(effecttype, targetColor, targetpower, style, length, isBlocking);
		}

		public SceneOld GetForegroundScene()
		{
			return scenes[foregroundScene];
		}

		public LayerOld GetLayer(int layNum)
		{
			return scenes[foregroundScene].GetLayer(layNum);
		}

		private void UpdateDepth()
		{
			scenes[foregroundScene].ChangeSceneDepth(1);
			scenes[backgroundScene].ChangeSceneDepth(0);
		}

		public void SetNewSceneFade(string textureName, int time)
		{
			scenes[foregroundScene].FadeScene(time);
			scenes[backgroundScene].SetLayerTextureImmediate(0, textureName);
			scenes[backgroundScene].ShowScene();
			UpdateDepth();
			SwapScenes();
		}

		public void SetNewSceneFadeWithMask(string textureName, string maskname, int time)
		{
			scenes[foregroundScene].FadeSceneWithMask(maskname, time);
			scenes[backgroundScene].SetLayerTextureImmediate(0, textureName);
			scenes[backgroundScene].ShowScene();
			UpdateDepth();
			SwapScenes();
		}

		public SceneControllerOld()
		{
			gameSystem = GameSystem.Instance;
			for (int i = 0; i < 2; i++)
			{
				GameObject gameObject = Object.Instantiate(gameSystem.ScenePrefab);
				scenes[i] = gameObject.GetComponent<SceneOld>();
				scenes[i].Initialize(i, 32);
			}
			scenes[0].ResetLayer();
			scenes[1].HideScene();
			effectCamera = GameObject.FindGameObjectWithTag("EffectCamera");
		}
	}
}
