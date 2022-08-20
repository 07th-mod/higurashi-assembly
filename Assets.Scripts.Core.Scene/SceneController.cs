using MOD.Scripts.Core;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class SceneController : MonoBehaviour
	{
		public GameObject Scene1;

		public GameObject Scene2;

		public GameObject Scene3;

		public GameObject SceneCameras;

		public GameObject EffectCamera;

		public LayerPool LayerPool;

		public GameObject Panel;

		public GameObject FacePanel;

		public Camera ScreenshotCamera;

		private Scene scene1;

		private Scene scene2;

		private GameSystem gameSystem;

		private Layer[] layers = new Layer[64];

		private Layer faceLayer;

		private int activeScene;

		public static int UpperLayerRange = 32;

		private Vector3 defaultOffset = new Vector3(0f, 0f, 0f);

		private Vector3 targetPosition;

		private Vector3 targetScale;

		private bool faceToUpperLayer = true;

		private string faceName = string.Empty;

		private bool useFilm;

		private bool useHorizontalBlur;

		private bool useBlur;

		private int filmType;

		private int filmPower;

		private int filmStyle;

		private Color filmColor = Color.white;

		private FilmEffector effector;

		private int lastWidth;

		private int lastHeight;

		private IEnumerator MODLipSyncCoroutine;

		public Scene MODActiveScene => GetActiveScene();

		private float expression2Threshold = .7f;
		private float expression1Threshold = .3f;
		private bool forceComputedLipsync = true;

		static SceneController()
		{
			UpperLayerRange = 32;
		}

		public Layer GetIfInUse(int id)
		{
			if (layers[id] != null && !LayerPool.IsInPool(layers[id].gameObject) && layers[id].IsInUse)
			{
				return layers[id];
			}
			return null;
		}

		public Layer GetLayer(int id)
		{
			if (layers[id] != null && !LayerPool.IsInPool(layers[id].gameObject) && layers[id].IsInUse)
			{
				return layers[id];
			}
			Layer layer = LayerPool.ActivateLayer();
			layer.name = "Layer " + id;
			layer.LayerID = id;
			layers[id] = layer;
			return layer;
		}

		public void RemoveLayerReference(Layer l)
		{
			for (int i = 0; i < layers.Length; i++)
			{
				if (l == layers[i])
				{
					layers[i] = null;
				}
			}
		}

		public int GetActiveLayerMask()
		{
			if (activeScene == 0)
			{
				return LayerMask.NameToLayer("Scene1");
			}
			return LayerMask.NameToLayer("Scene2");
		}

		private void UpdateLayerMask(Layer layer, int priority)
		{
			if (priority >= UpperLayerRange)
			{
				layer.gameObject.layer = LayerMask.NameToLayer("Scene3");
			}
			else
			{
				layer.gameObject.layer = GetActiveLayerMask();
			}
		}

		private void SetLayerActiveOnBothScenes(Layer layer)
		{
			layer.gameObject.layer = LayerMask.NameToLayer("RenderBoth");
		}

		public void ControlMotionOfSprite(int layer, MtnCtrlElement[] motions, int style)
		{
			Layer layer2 = GetLayer(layer);
			layer2.ControlLayerMotion(motions);
		}

		public void MoveSprite(int layer, int x, int y, int z, int angle, int easetype, float alpha, float wait, bool isblocking)
		{
			Layer layer2 = GetLayer(layer);
			layer2.MoveLayer(x, y, z, alpha, easetype, wait, isblocking, adjustAlpha: true);
			layer2.SetAngle((float)angle, wait);
		}

		public void MoveSpriteEx(int layer, string filename, Vector3[] points, float alpha, float time, bool isblocking)
		{
			Layer i = GetLayer(layer);
			if (filename != string.Empty)
			{
				i.CrossfadeLayer(filename, time, isblocking);
			}
			gameSystem.RegisterAction(delegate
			{
				i.MoveLayerEx(points, points.Length, 1f - alpha, time);
				if (isblocking)
				{
					gameSystem.AddWait(new Wait(time, WaitTypes.WaitForMove, i.FinishAll));
				}
			});
		}

		public void DrawBustshot(int layer, string textureName, int x, int y, int z, int oldx, int oldy, int oldz, bool move, int priority, int type, float wait, bool isblocking)
		{
			if (MODSkipImage(textureName))
			{
				return;
			}

			Layer i = GetLayer(layer);
			while (i.FadingOut)
			{
				i.HideLayer();
				i = GetLayer(layer);
			}
			if (!move)
			{
				oldx = x;
				oldy = y;
				oldz = z;
			}
			i.DrawLayer(textureName, oldx, oldy, oldz, null, 1f, /*isBustshot:*/ true, type, wait, isblocking);
			i.SetPriority(priority);
			if (move)
			{
				i.MoveLayer(x, y, z, 1f, 0, wait, isblocking, adjustAlpha: false);
			}
			iTween.Stop(i.gameObject);
			gameSystem.RegisterAction(delegate
			{
				if (i.UsingCrossShader() && i.gameObject.layer != GetActiveLayerMask())
				{
					SetLayerActiveOnBothScenes(i);
				}
				else
				{
					UpdateLayerMask(i, priority);
				}
			});
		}

		public void FadeBustshotWithFiltering(int layer, string mask, int style, float wait, bool isblocking)
		{
			Layer layer2 = GetLayer(layer);
			while (layer2.FadingOut)
			{
				layer2.HideLayer();
				layer2 = GetLayer(layer);
			}
			layer2.FadeLayerWithMask(mask, style, wait, isblocking);
			gameSystem.RegisterAction(delegate
			{
			});
		}

		public void DrawBustshotWithFiltering(int layer, string textureName, string mask, int x, int y, int z, int originx, int originy, int oldx, int oldy, int oldz, bool move, int priority, int type, float wait, bool isblocking)
		{
			Layer i = GetLayer(layer);
			while (i.FadingOut)
			{
				i.HideLayer();
				i = GetLayer(layer);
			}
			if (!move)
			{
				oldx = x;
				oldy = y;
				oldz = z;
			}
			Vector2? origin = null;
			if (originx != 0 || originy != 0)
			{
				origin = new Vector2((float)originx, (float)originy);
			}
			i.DrawLayerWithMask(textureName, mask, oldx, oldy, origin, /*isBustshot:*/ true, type, wait, isblocking);
			i.SetPriority(priority);
			if (move)
			{
				i.MoveLayer(x, y, z, 1f, 0, wait, isblocking, adjustAlpha: false);
			}
			iTween.Stop(i.gameObject);
			gameSystem.RegisterAction(delegate
			{
				if (i.UsingCrossShader() && i.gameObject.layer != GetActiveLayerMask())
				{
					SetLayerActiveOnBothScenes(i);
				}
				else
				{
					UpdateLayerMask(i, priority);
				}
			});
		}

		public void MoveBustshot(int layer, string textureName, int x, int y, int z, float wait, bool isblocking)
		{
			Layer layer2 = GetLayer(layer);
			Vector3 localPosition = layer2.transform.localPosition;
			int x2 = (int)localPosition.x;
			Vector3 localPosition2 = layer2.transform.localPosition;
			int y2 = (int)localPosition2.y;
			Vector3 localPosition3 = layer2.transform.localPosition;
			int z2 = (int)localPosition3.z;
			if (Mathf.Approximately(wait, 0f))
			{
				x2 = x;
				y2 = y;
				z2 = z;
			}
			if (textureName != string.Empty)
			{
				layer2.DrawLayer(textureName, x2, y2, z2, null, 1f, /*isBustshot:*/ true, 0, wait, isBlocking: false);
			}
			layer2.MoveLayer(x, y, z, 1f, 0, wait, isblocking, adjustAlpha: true);
		}

		public void SetLayerToDepthMasked(int layer)
		{
			Layer layer2 = GetLayer(layer);
			layer2.SwitchToMaskedShader();
		}

		public void FadeSpriteWithFiltering(int layer, string mask, int style, float wait, bool isblocking)
		{
			Layer layer2 = GetLayer(layer);
			layer2.FadeLayerWithMask(mask, style, wait, isblocking);
		}

		public void DrawSpriteWithFiltering(int layer, string texture, string mask, int x, int y, int style, int priority, float wait, bool isBlocking)
		{
			if (MODSkipImage(texture))
			{
				return;
			}

			Layer layer2 = GetLayer(layer);
			UpdateLayerMask(layer2, priority);
			layer2.DrawLayerWithMask(texture, mask, x, y, null, /*isBustshot:*/ false, style, wait, isBlocking);
			layer2.SetPriority(priority);
		}

		public void FadeSprite(int layer, float wait, bool isblocking)
		{
			Layer ifInUse = GetIfInUse(layer);
			if (ifInUse != null)
			{
				ifInUse.FadeOutLayer(wait, isblocking);
			}
			else
			{
				MOD.Scripts.Core.MODLogger.Log($"WARNING: FadeSprite Failed as layer not in use", true);
			}
		}

		public void DrawSprite(int layer, string texture, string mask, int x, int y, int z, int originx, int originy, int angle, int style, float alpha, int priority, float wait, bool isblocking)
		{
			if (MODSkipImage(texture))
			{
				return;
			}

			Layer layer2 = GetLayer(layer);
			UpdateLayerMask(layer2, priority);
			Vector2? origin = null;
			if (originx != 0 || originy != 0)
			{
				origin = new Vector2((float)originx, (float)originy);
			}
			layer2.DrawLayer(texture, x, y, z, origin, 1f - alpha, /*isBustshot:*/ false, 0, wait, isblocking);
			layer2.SetAngle((float)angle, 0f);
			layer2.SetPriority(priority);
			if (style == 1)
			{
				layer2.SwitchToAlphaShader();
			}
		}

		public void DrawBG(string texture, float wait, bool isblocking)
		{
			if (MODSkipImage(texture))
			{
				return;
			}

			Scene scene = GetActiveScene();
			scene.BackgroundLayer.DrawLayer(texture, 0, 0, 0, null, 0f, /*isBustshot:*/ false, 0, wait, isblocking);
		}

		public void SetFaceToUpperLayer(bool isUpper)
		{
			faceToUpperLayer = isUpper;
			faceLayer.gameObject.layer = ((!faceToUpperLayer) ? GetActiveLayerMask() : LayerMask.NameToLayer("Scene3"));
		}

		public void FadeFace(float wait, bool isblocking)
		{
			faceLayer.FadeOutLayer(wait, isblocking);
			faceName = string.Empty;
		}

		public void DrawFace(string texture, float wait, bool isblocking)
		{
			faceName = texture;
			if (Mathf.Approximately(wait, 0f))
			{
				isblocking = false;
			}
			faceLayer.DrawLayer(texture, 0, 0, 0, null, 1f, /*isBustshot:*/ false, 0, wait, isblocking);
			faceLayer.gameObject.layer = GetActiveLayerMask();
			if (faceToUpperLayer)
			{
				faceLayer.gameObject.layer = LayerMask.NameToLayer("Scene3");
			}
			faceLayer.SetPriority(63);
		}

		public void HideFace(float time)
		{
			if (!(faceName == string.Empty))
			{
				faceLayer.FadeOutLayer(time, isBlocking: false);
			}
		}

		public void RevealFace(float time)
		{
			if (!(faceName == string.Empty))
			{
				DrawFace(faceName, time, isblocking: false);
			}
		}

		public void DrawSceneWithMask(string backgroundfilename, string maskname, float time)
		{
			if (MODSkipImage(backgroundfilename))
			{
				return;
			}

			SwapActiveScenes();
			Scene s = GetActiveScene();
			s.UpdateRange(0f);
			s.BackgroundLayer.ReleaseTextures();
			s.BackgroundLayer.DrawLayer(backgroundfilename, 0, 0, 0, null, 0f, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
			s.SetTransitionMask(maskname);
			faceLayer.HideLayer();
			gameSystem.RegisterAction(delegate
			{
				s.StartTransition(time);
				s.BackgroundLayer.SetRange(1f);
				gameSystem.AddWait(new Wait(time, WaitTypes.WaitForScene, s.StopFadeIn));
			});
		}

		public void DrawScene(string backgroundfilename, float time)
		{
			if (MODSkipImage(backgroundfilename))
			{
				return;
			}

			SwapActiveScenes();
			Scene s = GetActiveScene();
			s.GetComponent<Camera>().enabled = false;
			s.UpdateRange(0f);
			s.BackgroundLayer.ReleaseTextures();
			s.BackgroundLayer.DrawLayer(backgroundfilename, 0, 0, 0, null, 0f, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
			faceLayer.HideLayer();
			gameSystem.RegisterAction(delegate
			{
				s.GetComponent<Camera>().enabled = true;
				s.FadeSceneIn(time);
				s.BackgroundLayer.SetRange(1f);
				gameSystem.AddWait(new Wait(time, WaitTypes.WaitForScene, s.StopFadeIn));
			});
		}

		public void FinalizeViewportChange()
		{
			iTween.Stop(Panel);
			Panel.transform.localPosition = targetPosition;
			Panel.transform.localScale = targetScale;
		}

		public void ResetViewportSize()
		{
			iTween.Stop(Panel);
			Panel.transform.localPosition = (targetPosition = defaultOffset);
			Panel.transform.localScale = (targetScale = new Vector3(1f, 1f, 1f));
		}

		public void EnlargeScene(float x, float y, float sx, float sy, float time, bool isblocking)
		{
			targetPosition = new Vector3(x, 0f - y, 0f) + defaultOffset;
			targetScale = new Vector3(800f / sx, 600f / sy, 1f);
			gameSystem.RegisterAction(delegate
			{
				iTween.ScaleTo(Panel, iTween.Hash("scale", targetScale, "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
				iTween.MoveTo(Panel, iTween.Hash("position", targetPosition, "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
				if (isblocking)
				{
					GameSystem.Instance.AddWait(new Wait(time, WaitTypes.WaitForMove, FinalizeViewportChange));
				}
			});
			if (isblocking)
			{
				gameSystem.ExecuteActions();
			}
		}

		public void ShakeBustshot(int layer, float speed, int level, int attenuation, int vector, int loopcount, bool isblocking)
		{
			Layer layer2 = GetLayer(layer);
			Shaker.ShakeObject(layer2.gameObject, speed, level, attenuation, vector, loopcount, isblocking);
		}

		public void StopBustshotShake(int layer)
		{
			Shaker component = GetLayer(layer).GetComponent<Shaker>();
			if (component != null)
			{
				component.StopShake();
			}
		}

		public void ShakeScene(float speed, int level, int attenuation, int vector, int loopcount, bool isblocking)
		{
			Shaker.ShakeObject(SceneCameras, speed, level, attenuation, vector, loopcount, isblocking);
		}

		public void StopSceneShake()
		{
			Shaker component = SceneCameras.GetComponent<Shaker>();
			if (component != null)
			{
				component.StopShake();
			}
			SceneCameras.transform.localPosition = Vector3.zero;
		}

		public void HideBackgroundSceneObject()
		{
			if (activeScene == 0)
			{
				scene2.gameObject.SetActive(value: false);
			}
			else
			{
				scene1.gameObject.SetActive(value: true);
			}
		}

		public Vector3 GetPositionOfLayer(int layer)
		{
			return layers[layer].targetPosition;
		}

		public void HideFilmEffector(float time, bool isBlocking)
		{
			useFilm = false;
			useBlur = false;
			useHorizontalBlur = false;
			if (!(effector == null))
			{
				FilmEffector targeteffector = effector;
				if (Mathf.Approximately(time, 0f))
				{
					targeteffector.RemoveEffector();
				}
				else
				{
					gameSystem.RegisterAction(delegate
					{
						targeteffector.FadeOut(time, isBlocking);
					});
				}
				effector = null;
			}
		}

		public void CreateFilmEffector(int effecttype, Color targetColor, int targetpower, int style, float length, bool isBlocking)
		{
			useFilm = true;
			filmPower = targetpower;
			filmColor = targetColor;
			filmType = effecttype;
			filmStyle = style;
			gameSystem.RegisterAction(delegate
			{
				EffectCamera.GetComponent<Camera>().enabled = true;
				if (effector == null)
				{
					effector = EffectCamera.AddComponent<FilmEffector>();
				}
				effector.Prepare(effecttype, targetColor, targetpower, style, length, isBlocking);
			});
		}

		public void CreateHorizontalGradation(int targetpower, float length, bool isBlocking)
		{
			useHorizontalBlur = true;
			filmPower = targetpower;
			if (effector != null)
			{
				HideFilmEffector(length, isBlocking: false);
				effector = null;
			}
			gameSystem.RegisterAction(delegate
			{
				EffectCamera.GetComponent<Camera>().enabled = true;
				effector = EffectCamera.AddComponent<FilmEffector>();
				effector.Prepare(10, Color.white, targetpower, 0, length, isBlocking);
			});
		}

		public void CreateBlur(int targetpower, float length, bool isBlocking)
		{
			useBlur = true;
			filmPower = targetpower;
			if (effector != null)
			{
				HideFilmEffector(length, isBlocking: false);
				effector = null;
			}
			gameSystem.RegisterAction(delegate
			{
				EffectCamera.GetComponent<Camera>().enabled = true;
				effector = EffectCamera.AddComponent<FilmEffector>();
				effector.Prepare(12, Color.white, targetpower, 0, length, isBlocking);
			});
		}

		public void HideAllLayers(float time)
		{
			for (int i = 0; i < 32; i++)
			{
				if (layers[i] != null && layers[i].IsInUse)
				{
					layers[i].FadeOutLayer(time, isBlocking: false);
				}
			}
		}

		public void SerializeScene(MemoryStream ms)
		{
			BinaryWriter binaryWriter = new BinaryWriter(ms);
			binaryWriter.Write(faceToUpperLayer);
			binaryWriter.Write(useFilm);
			binaryWriter.Write(useBlur);
			binaryWriter.Write(useHorizontalBlur);
			binaryWriter.Write(filmPower);
			binaryWriter.Write(filmType);
			binaryWriter.Write(filmStyle);
			MGHelper.WriteColor(binaryWriter, filmColor);
			if (activeScene == 0)
			{
				scene1.BackgroundLayer.Serialize(binaryWriter);
			}
			else
			{
				scene2.BackgroundLayer.Serialize(binaryWriter);
			}
			if (faceLayer.IsInUse)
			{
				binaryWriter.Write(value: true);
				faceLayer.Serialize(binaryWriter);
			}
			else
			{
				binaryWriter.Write(value: false);
			}
			for (int i = 0; i < 64; i++)
			{
				if (layers[i] == null)
				{
					binaryWriter.Write(value: false);
				}
				else if (layers[i].FadingOut || !layers[i].IsInUse)
				{
					binaryWriter.Write(value: false);
				}
				else
				{
					binaryWriter.Write(value: true);
					layers[i].Serialize(binaryWriter);
				}
			}
		}

		public void DeSerializeScene(MemoryStream ms)
		{
			BinaryReader binaryReader = new BinaryReader(ms);
			HideFilmEffector(0f, isBlocking: false);
			DrawScene("black", 0f);
			gameSystem.ExecuteActions();
			faceToUpperLayer = binaryReader.ReadBoolean();
			useFilm = binaryReader.ReadBoolean();
			useBlur = binaryReader.ReadBoolean();
			useHorizontalBlur = binaryReader.ReadBoolean();
			filmPower = binaryReader.ReadInt32();
			filmType = binaryReader.ReadInt32();
			filmStyle = binaryReader.ReadInt32();
			filmColor = MGHelper.ReadColor(binaryReader);
			MGHelper.ReadVector3(binaryReader);
			MGHelper.ReadVector3(binaryReader);
			string backgroundfilename = binaryReader.ReadString();
			binaryReader.ReadSingle();
			binaryReader.ReadInt32();
			binaryReader.ReadInt32();
			DrawScene(backgroundfilename, 0.3f);
			if (binaryReader.ReadBoolean())
			{
				MGHelper.ReadVector3(binaryReader);
				MGHelper.ReadVector3(binaryReader);
				string texture = binaryReader.ReadString();
				binaryReader.ReadSingle();
				binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				DrawFace(texture, 0f, isblocking: false);
			}
			for (int i = 0; i < 64; i++)
			{
				if (layers[i] != null && layers[i].IsInUse)
				{
					layers[i].HideLayer();
				}
				if (binaryReader.ReadBoolean())
				{
					Vector3 position = MGHelper.ReadVector3(binaryReader);
					Vector3 scale = MGHelper.ReadVector3(binaryReader);
					string textureName = binaryReader.ReadString();
					float alpha = binaryReader.ReadSingle();
					int num = binaryReader.ReadInt32();
					int type = binaryReader.ReadInt32();
					if (i != 50)
					{
						bool isBustshot = num != 0;
						Layer layer = GetLayer(i);
						UpdateLayerMask(layer, i);
						layer.DrawLayer(textureName, (int)position.x, (int)position.y, 0, null, alpha, isBustshot, type, 0f, isBlocking: false);
						layer.SetPriority(i);
						layer.RestoreScaleAndPosition(scale, position);
					}
				}
			}
			if (useFilm)
			{
				CreateFilmEffector(filmType, filmColor, filmPower, filmStyle, 0f, isBlocking: false);
			}
			if (useHorizontalBlur)
			{
				CreateHorizontalGradation(filmPower, 0f, isBlocking: false);
			}
			SetFaceToUpperLayer(faceToUpperLayer);
			gameSystem.ExecuteActions();
		}

		private IEnumerator GetScreenshotCoroutine(Action<Texture2D> OnFinishAction)
		{
			yield return (object)new WaitForEndOfFrame();
			RenderTexture rt = new RenderTexture(800, 600, 24);
			ScreenshotCamera.cullingMask = ((1 << GetActiveLayerMask()) | (1 << LayerMask.NameToLayer("Scene3")));
			ScreenshotCamera.targetTexture = rt;
			ScreenshotCamera.Render();
			RenderTexture.active = rt;
			Texture2D tex = new Texture2D(rt.width, rt.height);
			tex.ReadPixels(new Rect(0f, 0f, (float)rt.width, (float)rt.height), 0, 0, recalculateMipMaps: true);
			tex.Apply();
			OnFinishAction(tex);
		}

		private IEnumerator WriteScreenshotToFile(string path)
		{
			yield return (object)new WaitForEndOfFrame();
			RenderTexture rt = new RenderTexture(800, 600, 24);
			ScreenshotCamera.cullingMask = ((1 << GetActiveLayerMask()) | (1 << LayerMask.NameToLayer("Scene3")));
			ScreenshotCamera.targetTexture = rt;
			ScreenshotCamera.Render();
			RenderTexture.active = rt;
			Texture2D tex = new Texture2D(rt.width, rt.height);
			tex.ReadPixels(new Rect(0f, 0f, (float)rt.width, (float)rt.height), 0, 0, recalculateMipMaps: true);
			tex.Apply();
			byte[] texout = tex.EncodeToPNG();
			ScreenshotCamera.targetTexture = null;
			UnityEngine.Object.Destroy(rt);
			File.WriteAllBytes(path, texout);
		}

		public void WriteScreenshot(string path)
		{
			StartCoroutine(WriteScreenshotToFile(path));
		}

		public void GetScreenshot(Action<Texture2D> onFinishAction)
		{
			StartCoroutine(GetScreenshotCoroutine(onFinishAction));
		}

		public void ReloadAllImages()
		{
			Layer[] componentsInChildren = Panel.GetComponentsInChildren<Layer>();
			foreach (Layer layer in componentsInChildren)
			{
				layer.ReloadTexture();
			}

			// Force Unity to unload unused assets.
			// Higuarshi will do this if you play the game normally each time ExecuteActions()
			// is called (it appears as "Unloading 7 unused Assets to reduce memory usage." in the log).
			// However, if you don't advance the text, it won't ever clean up.
			// Eventually, you can run out of memory if this function is repeatedly
			// called (for example, constantly toggling art styles)
			Resources.UnloadUnusedAssets();
		}

		private Scene GetActiveScene()
		{
			if (activeScene == 0)
			{
				return scene1;
			}
			return scene2;
		}

		private void SwapActiveScenes()
		{
			if (activeScene == 0)
			{
				activeScene = 1;
				scene1.Depth = 0f;
				scene2.Depth = 1f;
			}
			else
			{
				activeScene = 0;
				scene1.Depth = 1f;
				scene2.Depth = 0f;
			}
		}

		private void Start()
		{
			scene1 = Scene1.GetComponent<Scene>();
			scene2 = Scene2.GetComponent<Scene>();
			scene1.Depth = 1f;
			scene2.Depth = 0f;
			scene2.gameObject.SetActive(value: false);
			gameSystem = GameSystem.Instance;
			activeScene = 0;
			scene1.BackgroundLayer.transform.localPosition = defaultOffset;
			scene2.BackgroundLayer.transform.localPosition = defaultOffset;
			Panel.transform.localPosition = defaultOffset;
			FacePanel.transform.localPosition = defaultOffset;
			faceLayer = LayerPool.ActivateLayer();
			faceLayer.transform.parent = FacePanel.transform;
			faceLayer.gameObject.name = "Face Layer";
			faceLayer.gameObject.layer = LayerMask.NameToLayer("Face");
			faceLayer.IsStatic = true;
			faceLayer.gameObject.SetActive(value: false);
			gameSystem.CheckinSystem();
		}

		public void UpdateScreenSize() {
			float screenAspectRatio = Screen.width / (float)Screen.height;
			float gameAspectRatio = GameSystem.Instance.AspectRatio;
			float scale = (!(gameAspectRatio > screenAspectRatio)) ? 1f : (gameAspectRatio / screenAspectRatio);
			float num6 = 2f / (scale * 480f);
			base.gameObject.transform.localScale = new Vector3(num6, num6, num6);
			GameSystem.Instance.MainUIController.UpdateGuiScale(1 / scale, 1 / scale);
			lastWidth = Screen.width;
			lastHeight = Screen.height;
		}

		private void Update()
		{
			if (Screen.width != lastWidth || Screen.height != lastHeight)
			{
				UpdateScreenSize();
			}
		}

		public void MODOnlyRecompile()
		{
		}

		private IEnumerator MODBakedLipSync(string path, int character, ulong coroutineId, Texture2D exp2, Texture2D exp3, Texture2D exp4)
		{
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadLine();
			string[] exparray = text.Split(',');
			streamReader.Close();
			for (int k = 0; k < exparray.Length; k++)
			{
				if (!MODSystem.instance.modSceneController.MODLipSyncAnimationStillActive(character, coroutineId))
				{
					break;
				}
				if (exparray[k] == string.Empty)
				{
					exparray[k] = "0";
				}
				if (k > 1 && !exparray[k].Equals(exparray[k - 1]))
				{
					switch (exparray[k])
					{
						case "2":
							MODSystem.instance.modSceneController.MODLipSyncProcess(character, exp2, coroutineId);
							break;
						case "1":
							MODSystem.instance.modSceneController.MODLipSyncProcess(character, exp3, coroutineId);
							break;
						case "0":
							MODSystem.instance.modSceneController.MODLipSyncProcess(character, exp4, coroutineId);
							break;
					}
				}
				yield return (object)new WaitForSeconds(0.0666f);
			}
		}

		private IEnumerator MODComputedLipSync(int character, int audiolayer, AudioClip audioClip, ulong coroutineId, Texture2D exp2, Texture2D exp3, Texture2D exp4)
		{
			const float CHUNK_LENGTH_SECONDS = .05f;

			int frameRate = audioClip.frequency;
			//NOTE: Unity calls a "sample" what is usually called a "frame". Frames contain multiple channel's worth of information for a given point in time.
			int numFrames = audioClip.samples;
			int samplesPerFrame = audioClip.channels;

			int framesPerChunk = (int)(CHUNK_LENGTH_SECONDS * frameRate);
			int samplesPerChunk = framesPerChunk * samplesPerFrame;

			// If there is a leftover chunk (this happens if there is a remainder during the below division), it will be excluded/skipped.
			int numChunks = numFrames / framesPerChunk;

			bool voiceStartedPlaying = false;

			float[] rawAudioData = new float[samplesPerChunk];

			WaitForSeconds waitTime = new WaitForSeconds(CHUNK_LENGTH_SECONDS);

			// This for loop condition shouldn't ever be met, it's just here
			// to set some upper limit (~10x the expected number of loops) on the number of iterations
			for (int chunk = 0; chunk < 10 * numChunks; chunk++)
			{
				// Forcibly stop the lipsync animation if it's not meant to be playing at this time?
				if (!MODSystem.instance.modSceneController.MODLipSyncAnimationStillActive(character, coroutineId))
				{
					break;
				}

				if (GameSystem.Instance.AudioController.IsVoicePlaying(audiolayer))
				{
					voiceStartedPlaying = true;

					// If there is less than one chunk of audio left, we are finished.
					int frameOffset = GameSystem.Instance.AudioController.GetPlayTimeSamples(audiolayer);
					int framesLeft = numFrames - frameOffset;
					if (framesLeft < framesPerChunk)
					{
						break;
					}

					// Get the chunk we are interested in
					// Note: If the audio finishes playing before the coroutine finishes, then
					// audioClip somehow becomes null. I've added handling here in case this happens,
					// although the above "is audio playing" checks should above should stop this from happening.
					try
					{
						if (audioClip == null)
						{
							break;
						}
						audioClip.GetData(rawAudioData, frameOffset);
					}
					catch (Exception)
					{
						break;
					}

					// Find the max value in the chunk, with some shortcuts
					//  - We don't care about which channel the audio comes from, so just process every sample the same, even if it's a different channel
					//  - We only check the positive maximum of the waveform (we don't need to check the negative maximum of the waveform for our application)
					float max = 0;
					foreach (float data in rawAudioData)
					{
						if (data > max)
						{
							max = data;
						}
					}

					//Use the max to determine what mouth sprite should be displayed
					if (max > expression2Threshold)
					{
						MODSystem.instance.modSceneController.MODLipSyncProcess(character, exp2, coroutineId);
					}
					else if (max > expression1Threshold)
					{
						MODSystem.instance.modSceneController.MODLipSyncProcess(character, exp3, coroutineId);
					}
					else
					{
						MODSystem.instance.modSceneController.MODLipSyncProcess(character, exp4, coroutineId);
					}
				}
				else if (voiceStartedPlaying)
				{
					// If the voice previously was playing, but now has stopped playing, lipsync is finished.
					// This happens if this coroutine resumes just after the audio finishes playing.
					break;
				}

				// This delay sets the approximate rate at which the lipsync updates
				// the exact delay time is not critical
				yield return waitTime;
			}
		}

		public IEnumerator MODDrawLipSync(int character, int audiolayer, string audiofile, AudioClip audioClip)
		{
			ulong coroutineId = MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(character);
			Texture2D exp4 = MODSystem.instance.modSceneController.MODLipSyncPrepare(character, "0");
			Texture2D exp3 = MODSystem.instance.modSceneController.MODLipSyncPrepare(character, "1");
			Texture2D exp2 = MODSystem.instance.modSceneController.MODLipSyncPrepare(character, "2");

			if (forceComputedLipsync)
			{
				yield return MODComputedLipSync(character, audiolayer, audioClip, coroutineId, exp2, exp3, exp4);
			}
			else // If the spectrum file doesn't exist, use the "computed lipsync" method
			{
				string str = audiofile.Replace(".ogg", ".txt");
				string path = Path.Combine(Application.streamingAssetsPath, "spectrum/" + str);
				if (File.Exists(path))
				{
					yield return MODBakedLipSync(path, character, coroutineId, exp2, exp3, exp4);
				}
				else
				{
					yield return MODComputedLipSync(character, audiolayer, audioClip, coroutineId, exp2, exp3, exp4);
				}
			}

			MODSystem.instance.modSceneController.MODLipSyncProcess(character, exp4, coroutineId);
		}

		public void MODLipSyncStart(int character, int audiolayer, string audiofile, AudioClip audioClip)
		{
			MODLipSyncCoroutine = MODDrawLipSync(character, audiolayer, audiofile, audioClip);
			StartCoroutine(MODLipSyncCoroutine);
		}

		private bool MODSkipImage(string backgroundfilename)
		{
			if(Buriko.BurikoMemory.Instance.GetGlobalFlag("GHideCG").IntValue() == 1 && backgroundfilename.Contains("scene/"))
			{
				return true;
			}

			return false;
		}

		public void MODSetForceComputedLipsync(bool forceComputedLipsync) => this.forceComputedLipsync = forceComputedLipsync;
		public bool MODGetForceComputedLipsync() => this.forceComputedLipsync;

		public void MODSetExpressionThresholds(float threshold1, float threshold2)
		{
			this.expression1Threshold = threshold1;
			this.expression2Threshold = threshold2;
		}

		public void MODGetExpressionThresholds(out float threshold1, out float threshold2)
		{
			threshold1 = this.expression1Threshold;
			threshold2 = this.expression2Threshold;
		}
	}
}
