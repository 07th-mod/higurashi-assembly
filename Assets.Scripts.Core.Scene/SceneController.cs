using Assets.Scripts.Core.AssetManagement;
using MOD.Scripts.Core;
using MOD.Scripts.Core.Scene;
using System;
using System.Collections;
using System.IO;
using System.Text;
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

		public GameObject Panel1;

		public GameObject Panel2;

		public GameObject PanelUpper;

		public GameObject FacePanel;

		public Camera ScreenshotCamera;

		public FragmentController FragmentController;

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

		private string faceName = "";

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
		private bool forceComputedLipsync;

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

		public void DebugLayerInfo()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("Layer info ");
			for (int i = 0; i < layers.Length; i++)
			{
				stringBuilder.Append(i);
				stringBuilder.Append(":");
				if (layers[i] == null)
				{
					stringBuilder.Append("_");
				}
				else
				{
					stringBuilder.Append(layers[i]?.IsInUse);
				}
				stringBuilder.Append(" ");
			}
			Debug.Log(stringBuilder.ToString());
		}

		public Layer GetLayer(int id)
		{
			if (layers[id] != null && !LayerPool.IsInPool(layers[id].gameObject) && layers[id].IsInUse && (layers[id].activeScene == activeScene || layers[id].activeScene > 1))
			{
				return layers[id];
			}
			if (layers[id] != null)
			{
				Debug.LogWarning($"Actively overriding existing layer {id}");
			}
			Layer layer = LayerPool.ActivateLayer();
			layer.name = "Layer " + id;
			layer.IsPersistent = false;
			layer.activeScene = activeScene;
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

		public int GetLayerPriority(int id)
		{
			if (layers[id] == null || (layers[id].IsInUse && !layers[id].IsPersistent))
			{
				return -1;
			}
			return layers[id].Priority;
		}

		private void UpdateLayerMask(Layer layer, int priority)
		{
			if (priority >= UpperLayerRange || layer.IsPersistent)
			{
				layer.gameObject.layer = LayerMask.NameToLayer("RenderBoth");
				layer.activeScene = 2;
				layer.IsPersistent = true;
				layer.gameObject.transform.SetParent(PanelUpper.transform, worldPositionStays: true);
			}
			else
			{
				layer.gameObject.layer = GetActiveLayerMask();
				layer.activeScene = activeScene;
				layer.IsPersistent = false;
			}
		}

		private void SetLayerActiveOnBothScenes(Layer layer)
		{
			layer.gameObject.layer = LayerMask.NameToLayer("RenderBoth");
			layer.activeScene = 2;
			layer.IsPersistent = true;
		}

		public void MakeLayerPersistent(int layerId)
		{
			if (layers[layerId] == null)
			{
				Debug.LogError($"Error in MakeLayerPersistent! Layer {layerId} does not exist!");
				return;
			}
			Layer layer = layers[layerId];
			if (!layer.IsPersistent)
			{
				layer.gameObject.layer = LayerMask.NameToLayer("RenderBoth");
				layer.activeScene = 2;
				layer.IsPersistent = true;
				layer.transform.SetParent(PanelUpper.transform, worldPositionStays: true);
			}
		}

		public void EndLayerPersistence(int layerId)
		{
			if (layers[layerId] == null)
			{
				Debug.LogWarning($"Error in EndLayerPersistence! Layer {layerId} does not exist!");
				Debug.Break();
				return;
			}
			Layer layer = layers[layerId];
			if (layers[layerId].IsPersistent)
			{
				layer.gameObject.layer = GetActiveLayerMask();
				layer.activeScene = activeScene;
				layer.IsPersistent = false;
				layer.transform.SetParent(GetActivePanel().transform, worldPositionStays: true);
			}
		}

		public void ControlMotionOfSprite(int layer, MtnCtrlElement[] motions, int style)
		{
			GetLayer(layer).ControlLayerMotion(motions);
		}

		public void MoveSprite(int layer, int x, int y, int z, int angle, int easetype, float alpha, float wait, bool isblocking)
		{
			Layer layer2 = GetLayer(layer);
			layer2.MoveLayer(x, y, z, alpha, easetype, wait, isblocking, adjustAlpha: true);
			layer2.SetAngle(angle, wait);
		}

		public void MoveSpriteEx(int layer, string filename, Vector3[] points, float alpha, float time, bool isblocking)
		{
			Layer i = GetLayer(layer);
			if (filename != "")
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

		public void DrawBustshot(int layer, string textureName, int x, int y, int z, int oldx, int oldy, int oldz, bool move, int priority, int type, float wait, bool isblocking, Action<Texture2D> afterLayerUpdated)
		{
			if (MODSkipImage(textureName))
			{
				return;
			}
			Layer layer2 = GetLayer(layer);
			while (layer2.FadingOut)
			{
				layer2.HideLayer();
				layer2 = GetLayer(layer);
			}
			if (!move)
			{
				oldx = x;
				oldy = y;
				oldz = z;
			}
			layer2.DrawLayer(textureName, oldx, oldy, oldz, null, null, 1f, isBustshot: true, type, wait, isblocking, afterLayerUpdated);
			layer2.SetPriority(priority);
			if (move)
			{
				layer2.MoveLayer(x, y, z, 1f, 0, wait, isblocking, adjustAlpha: false);
			}
			iTween.Stop(layer2.gameObject);
			if (layer2.UsingCrossShader() && layer2.gameObject.layer != GetActiveLayerMask())
			{
				SetLayerActiveOnBothScenes(layer2);
			}
			else
			{
				UpdateLayerMask(layer2, priority);
			}
			gameSystem.ExecuteActions();
		}

		public void DrawBustshot(int layer, string textureName, int x, int y, int z, int oldx, int oldy, int oldz, bool move, int priority, int type, float wait, bool isblocking)
		{
			DrawBustshot(layer, textureName, x, y, z, oldx, oldy, oldz, move, priority, type, wait, isblocking, null);
		}

		public void ChangeBustshot(int layer, string textureName, float wait, bool isblocking)
		{
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			Layer layer2 = GetLayer(layer);
			if (!layer2.IsInUse)
			{
				Debug.LogWarning("Attempting to call ChangeBustshot on layer " + layer + " which is not in use! Attempting to change bustshot to : " + textureName);
				// TODO: is this always called? Remove the below line if unnecessary (added by doddler)
				DebugLayerInfo();
			}
			layer2.CrossfadeLayer(textureName, wait, isblocking);
			gameSystem.ExecuteActions();
		}

		public void ChangeBustshotWithFiltering(int layer, string textureName, string maskName, int style, float wait, bool isblocking)
		{
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			Layer layer2 = GetLayer(layer);
			if (!layer2.IsInUse)
			{
				Debug.LogWarning("Attempting to call ChangeBustshotWithFiltering on layer " + layer + " which is not in use! Attempting to change bustshot to : " + textureName);
				DebugLayerInfo();
			}
			layer2.CrossfadeLayerWithMask(textureName, maskName, style, wait, isblocking);
			gameSystem.ExecuteActions();
		}

		public void FadeBustshotWithFiltering(int layer, string mask, int style, float wait, bool isblocking)
		{
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			Layer layer2 = GetLayer(layer);
			while (layer2.FadingOut)
			{
				layer2.HideLayer();
				layer2 = GetLayer(layer);
			}
			layer2.FadeLayerWithMask(mask, style, wait, isblocking);
			gameSystem.ExecuteActions();
		}

		public void DrawBustshotWithFiltering(int layer, string textureName, string mask, int x, int y, int z, int originx, int originy, int overridew, int overrideh, int oldx, int oldy, int oldz, bool move, int priority, int type, float wait, bool isblocking, Action<Texture2D> afterLayerUpdated)
		{
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			Layer layer2 = GetLayer(layer);
			while (layer2.FadingOut)
			{
				layer2.HideLayer();
				layer2 = GetLayer(layer);
			}
			if (!move)
			{
				oldx = x;
				oldy = y;
				oldz = z;
			}
			Vector2? origin = null;
			Vector2? forceSize = null;
			if (originx != 0 || originy != 0)
			{
				origin = new Vector2(originx, originy);
			}
			if (overridew != 0 && overrideh != 0)
			{
				forceSize = new Vector2(overridew, overrideh);
			}
			iTween.Stop(layer2.gameObject);
			layer2.DrawLayerWithMask(textureName, mask, oldx, oldy, origin, forceSize, isBustshot: true, type, wait, isblocking, afterLayerUpdated);
			layer2.SetPriority(priority);
			if (move)
			{
				layer2.MoveLayer(x, y, z, 1f, 0, wait, isblocking, adjustAlpha: false);
			}
			if (layer2.UsingCrossShader() && layer2.gameObject.layer != GetActiveLayerMask())
			{
				SetLayerActiveOnBothScenes(layer2);
			}
			else
			{
				UpdateLayerMask(layer2, priority);
			}
			gameSystem.ExecuteActions();
		}

		public void DrawBustshotWithFiltering(int layer, string textureName, string mask, int x, int y, int z, int originx, int originy, int overridew, int overrideh, int oldx, int oldy, int oldz, bool move, int priority, int type, float wait, bool isblocking)
		{
			DrawBustshotWithFiltering(layer, textureName, mask, x, y, z, originx, originy, overridew, overrideh, oldx, oldy, oldz, move, priority, type, wait, isblocking, afterLayerUpdated: null);
		}

		public void MoveBustshot(int layer, string textureName, int x, int y, int z, float wait, bool isblocking)
		{
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			Layer layer2 = GetLayer(layer);
			int x2 = (int)layer2.transform.localPosition.x;
			int y2 = (int)layer2.transform.localPosition.y;
			int z2 = (int)layer2.transform.localPosition.z;
			if (Mathf.Approximately(wait, 0f))
			{
				x2 = x;
				y2 = y;
				z2 = z;
			}
			if (textureName != "")
			{
				layer2.DrawLayer(textureName, x2, y2, z2, null, null, 1f, isBustshot: true, 0, wait, isBlocking: false);
			}
			layer2.MoveLayer(x, y, z, 1f, 0, wait, isblocking, adjustAlpha: true);
		}

		public void SetLayerToDepthMasked(int layer)
		{
			GetLayer(layer).SwitchToMaskedShader();
		}

		public void FadeSpriteWithFiltering(int layer, string mask, int style, float wait, bool isblocking)
		{
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			GetLayer(layer).FadeLayerWithMask(mask, style, wait, isblocking);
		}

		public void DrawSpriteWithFiltering(int layer, string texture, string mask, int x, int y, int overridew, int overrideh, int style, int priority, float wait, bool isBlocking)
		{
			if (MODSkipImage(texture))
			{
				return;
			}

			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			Layer layer2 = GetLayer(layer);
			UpdateLayerMask(layer2, priority);
			Vector2? origin = null;
			Vector2? forceSize = null;
			if (overridew != 0 && overrideh != 0)
			{
				forceSize = new Vector2(overridew, overrideh);
			}
			layer2.DrawLayerWithMask(texture, mask, x, y, origin, forceSize, isBustshot: false, style, wait, isBlocking);
			layer2.SetPriority(priority);
		}

		public void FadeSprite(int layer, float wait, bool isblocking)
		{
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
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

		public void DrawSprite(int layer, string texture, string mask, int x, int y, int z, int originx, int originy, int overridew, int overrideh, int angle, int style, float alpha, int priority, float wait, bool isblocking)
		{
			if (MODSkipImage(texture))
			{
				return;
			}

			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			Layer layer2 = GetLayer(layer);
			if (layer2.IsInUse)
			{
				layer2.ReleaseTextures();
			}
			UpdateLayerMask(layer2, priority);
			Vector2? origin = null;
			Vector2? forceSize = null;
			if (originx != 0 || originy != 0)
			{
				origin = new Vector2(originx, originy);
			}
			if (overridew != 0 && overrideh != 0)
			{
				forceSize = new Vector2(overridew, overrideh);
			}
			layer2.DrawLayer(texture, x, y, z, origin, forceSize, 1f - alpha, isBustshot: false, 0, wait, isblocking);
			layer2.SetAngle(angle, 0f);
			layer2.SetPriority(priority);
			if (style == 1)
			{
				layer2.SwitchToAlphaShader();
			}
			gameSystem.ExecuteActions();
		}

		public void DrawBG(string texture, float wait, bool isblocking)
		{
			if (MODSkipImage(texture))
			{
				return;
			}

			Scene scene = GetActiveScene();
			scene.BackgroundLayer.DrawLayer(texture, 0, 0, 0, null, null, 0f, /*isBustshot:*/ false, 0, wait, isblocking);
		}

		public void SetFaceToUpperLayer(bool isUpper)
		{
			faceToUpperLayer = isUpper;
			if (faceToUpperLayer)
			{
				faceLayer.gameObject.layer = LayerMask.NameToLayer("Scene3");
				faceLayer.activeScene = 2;
			}
			else
			{
				faceLayer.gameObject.layer = GetActiveLayerMask();
				faceLayer.activeScene = activeScene;
			}
		}

		public void FadeFace(float wait, bool isblocking)
		{
			if (faceLayer.IsInUse)
			{
				faceLayer.FadeOutLayer(wait, isblocking);
				faceName = "";
			}
		}

		public void DrawFace(string texture, float wait, bool isblocking)
		{
			faceName = texture;
			if (Mathf.Approximately(wait, 0f))
			{
				isblocking = false;
			}
			faceLayer.DrawLayer(texture, 0, 0, 0, null, null, 1f, isBustshot: false, 0, wait, isblocking);
			faceLayer.gameObject.layer = GetActiveLayerMask();
			if (faceToUpperLayer)
			{
				faceLayer.gameObject.layer = LayerMask.NameToLayer("Scene3");
			}
			faceLayer.SetPriority(63);
		}

		public void HideFace(float time)
		{
			if (!(faceName == ""))
			{
				faceLayer.FadeOutLayer(time, isBlocking: false);
			}
		}

		public void RevealFace(float time)
		{
			if (!(faceName == ""))
			{
				DrawFace(faceName, time, isblocking: false);
			}
		}

		public void DrawSceneWithMask(string backgroundfilename, string maskname, int style, float time)
		{
			if (MODSkipImage(backgroundfilename))
			{
				return;
			}

			SwapActiveScenes();
			Scene s = GetActiveScene();
			s.UpdateRange(0f);
			s.BackgroundLayer.ReleaseTextures();
			s.BackgroundLayer.DrawLayer(backgroundfilename, 0, 0, 0, null, null, 0f, isBustshot: false, 0, 0f, isBlocking: false);
			s.SetTransitionMask(maskname, style);
			faceLayer.HideLayer();
			gameSystem.RegisterAction(delegate
			{
				ResetViewportSize();
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
			s.BackgroundLayer.DrawLayer(backgroundfilename, 0, 0, 0, null, null, 0f, isBustshot: false, 0, 0f, isBlocking: false);
			faceLayer.HideLayer();
			gameSystem.RegisterAction(delegate
			{
				ResetViewportSize();
				s.GetComponent<Camera>().enabled = true;
				s.FadeSceneIn(time);
				s.BackgroundLayer.SetRange(1f);
				gameSystem.AddWait(new Wait(time, WaitTypes.WaitForScene, s.StopFadeIn));
			});
		}

		public void DrawSceneWithScroll(string backgroundfilename, int direction, float time)
		{
			Scene old = GetActiveScene();
			SwapActiveScenes();
			Scene s = GetActiveScene();
			s.GetComponent<Camera>().enabled = false;
			s.UpdateRange(0f);
			s.BackgroundLayer.ReleaseTextures();
			s.BackgroundLayer.DrawLayer(backgroundfilename, 0, 0, 0, null, null, 0f, isBustshot: false, 0, 0f, isBlocking: false);
			faceLayer.HideLayer();
			gameSystem.RegisterAction(delegate
			{
				ResetViewportSize();
				s.GetComponent<Camera>().enabled = true;
				s.ScrollSceneIn(time, direction);
				s.BackgroundLayer.SetRange(1f);
				gameSystem.AddWait(new Wait(time, WaitTypes.WaitForScene, s.StopFadeIn));
				old.ScrollSceneOut(time, direction);
			});
		}

		public GameObject GetActivePanel()
		{
			if (activeScene == 0)
			{
				return Panel1;
			}
			return Panel2;
		}

		public void FinalizeViewportChange()
		{
			GameObject activePanel = GetActivePanel();
			iTween.Stop(activePanel);
			activePanel.transform.localPosition = targetPosition;
			activePanel.transform.localScale = targetScale;
		}

		public void ResetViewportSize()
		{
			GameObject activePanel = GetActivePanel();
			iTween.Stop(activePanel);
			activePanel.transform.localPosition = (targetPosition = defaultOffset);
			activePanel.transform.localScale = (targetScale = new Vector3(1f, 1f, 1f));
		}

		public void EnlargeScene(float x, float y, float sx, float sy, float time, bool isblocking)
		{
			targetScale = new Vector3(640f / sx, 480f / sy, 1f);
			float num = 240f / targetScale.y;
			float num2 = 480f - num;
			float num3 = 320f / targetScale.x;
			float value = 640f - num3;
			float t = x / 640f;
			float t2 = y / 480f;
			float num4 = num.Remap(0f, 480f, -240f * targetScale.y, 240f * targetScale.y);
			float num5 = num2.Remap(0f, 480f, -240f * targetScale.y, 240f * targetScale.y);
			float a = num3.Remap(0f, 640f, 320f * targetScale.x, -320f * targetScale.x);
			float b = value.Remap(0f, 640f, 320f * targetScale.x, -320f * targetScale.x);
			Debug.LogFormat("Remap {0} to {1} <--into--> {2} to {3}", num, num2, num4, num5);
			float x2 = Mathf.Lerp(a, b, t);
			float y2 = Mathf.Lerp(num4, num5, t2);
			targetPosition = new Vector3(x2, y2, 0f) + defaultOffset;
			Debug.LogFormat("Perform Enlarge Scene -- Position: {0} Scale: {1}", targetPosition, targetScale);
			GameObject Panel = GetActivePanel();
			gameSystem.RegisterAction(delegate
			{
				iTween.ScaleTo(Panel, iTween.Hash("scale", targetScale, "time", time, "islocal", true, "easetype", iTween.EaseType.easeOutSine));
				iTween.MoveTo(Panel, iTween.Hash("position", targetPosition, "time", time, "islocal", true, "easetype", iTween.EaseType.easeOutSine));
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
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			Layer layer2 = GetLayer(layer);
			Shaker.ShakeObject(layer2.gameObject, speed, level, attenuation, vector, loopcount, isblocking);
		}

		public void StopBustshotShake(int layer)
		{
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateId(layer);
			Shaker component = GetLayer(layer).GetComponent<Shaker>();
			if (component != null)
			{
				component.StopShake();
			}
		}

		public void CreateVideoPlayer(string path, Vector2Int size)
		{
			GameVideoPlayer.CreateVideoPlayer(PanelUpper, path, size).gameObject.layer = LayerMask.NameToLayer("RenderBoth");
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
			GameObject gameObject = (activeScene != 0) ? scene1.gameObject : scene2.gameObject;
			iTween.Stop(gameObject);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.SetActive(value: false);
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
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateIdsForAll();
			for (int i = 0; i < layers.Length; i++)
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
			for (int i = 0; i < 64; i++)
			{
				if (!(layers[i] == null) && !layers[i].FadingOut && layers[i].IsInUse)
				{
					binaryWriter.Write(value: true);
					binaryWriter.Write(i);
					layers[i].Serialize(binaryWriter);
				}
			}
			binaryWriter.Write(value: false);
			FragmentController.Serialize(ms);
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
			for (int j = 0; j < layers.Length; j++)
			{
				if (layers[j] != null && layers[j].IsInUse)
				{
					layers[j].IsPersistent = false;
					layers[j].HideLayer();
				}
			}
			binaryReader.ReadVector3();
			binaryReader.ReadVector3();
			string text = binaryReader.ReadString();
			binaryReader.ReadSingle();
			binaryReader.ReadInt32();
			binaryReader.ReadOptionalVector2();
			binaryReader.ReadOptionalVector2();
			binaryReader.ReadInt32();
			binaryReader.ReadBoolean();
			AssetManager.Instance.PreloadTexture(text);
			DrawScene(text, 0.3f);
			while (binaryReader.ReadBoolean())
			{
				int num = binaryReader.ReadInt32();
				Vector3 position = binaryReader.ReadVector3();
				Vector3 scale = binaryReader.ReadVector3();
				string filename = binaryReader.ReadString();
				float range = binaryReader.ReadSingle();
				int num2 = binaryReader.ReadInt32();
				Vector2? origin = binaryReader.ReadOptionalVector2();
				Vector2? forcesize = binaryReader.ReadOptionalVector2();
				int type = binaryReader.ReadInt32();
				bool isPersistent = binaryReader.ReadBoolean();
				bool bustshot = num2 != 0;
				Layer i = GetLayer(num);
				i.IsPersistent = isPersistent;
				UpdateLayerMask(i, num);
				AssetManager.Instance.PreloadTexture(filename);
				GameSystem.Instance.RegisterAction(delegate
				{
					if (isPersistent)
					{
						i.DrawLayer(filename, (int)position.x, (int)position.y, 0, origin, forcesize, range, bustshot, type, 0.3f, isBlocking: true);
					}
					else
					{
						i.DrawLayer(filename, (int)position.x, (int)position.y, 0, origin, forcesize, range, bustshot, type, 0.3f, isBlocking: true);
					}
				});
				i.SetPriority(num);
				i.RestoreScaleAndPosition(scale, position);
			}
			FragmentController.Deserialize(ms);
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
			yield return new WaitForEndOfFrame();
			RenderTexture renderTexture = new RenderTexture(800, 600, 24);
			ScreenshotCamera.cullingMask = ((1 << GetActiveLayerMask()) | (1 << LayerMask.NameToLayer("Scene3")));
			ScreenshotCamera.targetTexture = renderTexture;
			ScreenshotCamera.Render();
			RenderTexture.active = renderTexture;
			Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
			texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0, recalculateMipMaps: true);
			texture2D.Apply();
			OnFinishAction(texture2D);
		}

		private IEnumerator WriteScreenshotToFile(string path)
		{
			yield return new WaitForEndOfFrame();
			RenderTexture renderTexture = new RenderTexture(800, 600, 24);
			ScreenshotCamera.cullingMask = ((1 << GetActiveLayerMask()) | (1 << LayerMask.NameToLayer("Scene3")));
			ScreenshotCamera.targetTexture = renderTexture;
			ScreenshotCamera.Render();
			RenderTexture.active = renderTexture;
			Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
			texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0, recalculateMipMaps: true);
			texture2D.Apply();
			byte[] bytes = texture2D.EncodeToPNG();
			ScreenshotCamera.targetTexture = null;
			UnityEngine.Object.Destroy(renderTexture);
			File.WriteAllBytes(path, bytes);
		}

		public void WriteScreenshot(string path)
		{
			StartCoroutine(WriteScreenshotToFile(path));
		}

		public void GetScreenshot(Action<Texture2D> onFinishAction)
		{
			StartCoroutine(GetScreenshotCoroutine(onFinishAction));
		}

		// This function is only used by the mod!
		public void ReloadAllImages()
		{
			AssetManager.Instance.InvalidateTextureCache();

			Layer[] componentsInChildren = Panel1.GetComponentsInChildren<Layer>();
			foreach (Layer layer in componentsInChildren)
			{
				if (Application.isEditor)
				{
					Debug.Log("Reloading layer " + layer.gameObject.name);
				}
				layer.ReloadTexture();
			}
			componentsInChildren = Panel2.GetComponentsInChildren<Layer>();
			foreach (Layer layer2 in componentsInChildren)
			{
				if (Application.isEditor)
				{
					Debug.Log("Reloading layer " + layer2.gameObject.name);
				}
				layer2.ReloadTexture();
			}
			componentsInChildren = PanelUpper.GetComponentsInChildren<Layer>();
			foreach (Layer layer3 in componentsInChildren)
			{
				if (Application.isEditor)
				{
					Debug.Log("Reloading layer " + layer3.gameObject.name);
				}
				layer3.ReloadTexture();
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
			MODSystem.instance.modSceneController.MODLipSyncInvalidateAndGenerateIdsForAll();
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
			Panel1.transform.localPosition = defaultOffset;
			Panel2.transform.localPosition = defaultOffset;
			FacePanel.transform.localPosition = defaultOffset;
			faceLayer = LayerPool.ActivateLayer();
			faceLayer.transform.parent = FacePanel.transform;
			faceLayer.gameObject.name = "Face Layer";
			faceLayer.gameObject.layer = LayerMask.NameToLayer("Face");
			faceLayer.IsStatic = true;
			faceLayer.gameObject.SetActive(value: false);
			gameSystem.CheckinSystem();
			FragmentController = new FragmentController();
			StartCoroutine(FixSize());
		}

		private IEnumerator FixSize()
		{
			yield return new WaitForSeconds(1f);
			lastWidth = 0;
		}

		// In Hou+, possibly this function was updated (or decompiler resulted in different output)
		// Just to be safe, I've used the Hou+ version of the function.
		public void UpdateScreenSize() {
			Vector2 screenSize = NGUITools.screenSize;
			float num = screenSize.x / screenSize.y;
			float num2 = GameSystem.Instance.AspectRatio * 480f;
			float num3 = 480f;
			float num4 = (num2 / num3 > num) ? ((float)Mathf.RoundToInt(num2 / num)) : num3;
			float num5 = 2f / num4;
			base.gameObject.transform.localScale = new Vector3(num5, num5, num5);
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
				if (!MODSystem.instance.modSceneController.MODLipSyncAnimationStillActive(character, coroutineId) || !MODSystem.instance.modSceneController.MODLipSyncIsEnabled())
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

			if(!MODLipsyncCache.LoadOrUseCache(character, out MODLipsyncCache.TextureGroup group))
			{
				yield break;
			}

			Texture2D exp4 = group.baseTexture_0;
			Texture2D exp3 = group.halfOpen_1;
			Texture2D exp2 = group.fullOpen_2;

			yield return MODComputedLipSync(character, audiolayer, audioClip, coroutineId, exp2, exp3, exp4);

			// Most of the time we want to reset the sprite back to the 'default' pose when lipsync finishes (even if the
			// lipsync option is already 'disabled') to prevent having a character with their mouth stuck open.
			//
			// However, if artset is changed while lipsync in progress (for example to OG sprites), then we don't want to
			// overwrite the OG sprite's texture with the Console texture.
			if(AssetManager.Instance.CurrentArtsetIndex == 0)
			{
				MODSystem.instance.modSceneController.MODLipSyncProcess(character, exp4, coroutineId);
			}
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
