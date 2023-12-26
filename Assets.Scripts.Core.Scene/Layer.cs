using Assets.Scripts.Core.AssetManagement;
using MOD.Scripts.Core.Scene;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core.Scene
{
	public class Layer : MonoBehaviour
	{
		private Mesh mesh;

		private MeshFilter meshFilter;

		private MeshRenderer meshRenderer;

		private Material material;

		private Texture2D primary;

		private Texture2D secondary;

		private Texture2D mask;

		public string PrimaryName = string.Empty;

		public string SecondaryName = string.Empty;

		public string MaskName = string.Empty;

		private const string shaderDefaultName = "MGShader/LayerShader";

		private const string shaderAlphaBlendName = "MGShader/LayerShaderAlpha";

		private const string shaderCrossfadeName = "MGShader/LayerCrossfade4";

		private const string shaderMaskedName = "MGShader/LayerMasked";

		private const string shaderMultiplyName = "MGShader/LayerMultiply";

		private const string shaderReverseZName = "MGShader/LayerShaderReverseZ";

		private Shader shaderDefault;

		private Shader shaderAlphaBlend;

		private Shader shaderCrossfade;

		private Shader shaderMasked;

		private Shader shaderMultiply;

		private Shader shaderReverseZ;

		public int Priority;

		private int shaderType;

		public bool IsInitialized;

		public bool IsStatic;

		public bool FadingOut;

		private float startRange;

		private float targetRange;

		public Vector3 targetPosition = new Vector3(0f, 0f, 0f);

		public Vector3 targetScale = new Vector3(1f, 1f, 1f);

		public Vector2? Origin;

		public Vector2? ForceSize;

		public float targetAngle;

		public int activeScene;

		private bool isInMotion;

		private float targetAlpha;

		private LayerAlignment alignment;

		private float aspectRatio;

		private Vector2? origin;

		private MtnCtrlElement[] motion;

		private int? layerID; // The layer number in the scene controller, if it has one

		private bool cachedIsBustShot;
		private bool cachedStretchToFit;
		private bool cachedRyukishiClamp;
		private int cachedFinalXOffset;

		public int? LayerID
		{
			get => layerID;
			set => layerID = value;
		}

		public bool IsInUse => primary != null;

		public string PrimaryTextureName => (!(primary == null)) ? primary.name : null;

		public Material MODMaterial => material;

		public MeshRenderer MODMeshRenderer => meshRenderer;

		public void RestoreScaleAndPosition(Vector3 scale, Vector3 position)
		{
			targetPosition = position;
			targetScale = scale;
			base.transform.localPosition = position;
			base.transform.localScale = scale;
		}

		private IEnumerator ControlledMotion()
		{
			MtnCtrlElement[] array = motion;
			foreach (MtnCtrlElement mt in array)
			{
				float time = (float)mt.Time / 1000f;
				MoveLayerEx(mt.Route, mt.Points, 1f - (float)mt.Transparancy / 256f, time);
				yield return new WaitForSeconds(time);
				startRange = 1f - (float)mt.Transparancy / 256f;
			}
			FinishAll();
			if (motion[motion.Length - 1].Transparancy == 256)
			{
				HideLayer();
			}
			isInMotion = false;
		}

		public void ControlLayerMotion(MtnCtrlElement[] motions)
		{
			if (isInMotion)
			{
				base.transform.localPosition = targetPosition;
			}
			motion = motions;
			MtnCtrlElement mtnCtrlElement = motion[motion.Length - 1];
			Vector3 vector = mtnCtrlElement.Route[mtnCtrlElement.Points - 1];
			Vector3 localPosition = base.transform.localPosition;
			vector.z = localPosition.z;
			targetPosition = vector;
			targetRange = (float)mtnCtrlElement.Transparancy / 256f;
			GameSystem.Instance.RegisterAction(delegate
			{
				StartCoroutine("ControlledMotion");
			});
		}

		public void MoveLayerEx(Vector3[] path, int points, float alpha, float time)
		{
			iTween.Stop(base.gameObject);
			Vector3[] array = new Vector3[points + 1];
			array[0] = base.transform.localPosition;
			for (int i = 0; i < points; i++)
			{
				array[i + 1].x = path[i].x;
				array[i + 1].y = 0f - path[i].y;
				ref Vector3 reference = ref array[i + 1];
				Vector3 localPosition = base.transform.localPosition;
				reference.z = localPosition.z;
			}
			if (UsingCrossShader())
			{
				alpha = 1f - alpha;
			}
			startRange = targetAlpha;
			targetPosition = array[array.Length - 1];
			targetAlpha = alpha;
			FadeTo(alpha, time);
			isInMotion = true;
			if (path.Length > 1)
			{
				iTween.MoveTo(base.gameObject, iTween.Hash("path", array, "movetopath", false, "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
			}
			else
			{
				iTween.MoveTo(base.gameObject, iTween.Hash("position", array[1], "time", time, "islocal", true, "easetype", iTween.EaseType.linear));
			}
		}

		public void MoveLayer(int x, int y, int z, float alpha, int easetype, float wait, bool isBlocking, bool adjustAlpha)
		{
			float num = 1f;
			if (z > 0)
			{
				num = 1f - (float)z / 400f;
			}
			if (z < 0)
			{
				num = 1f + (float)z / -400f;
			}
			float x2 = x;
			float y2 = -y;
			Vector3 localPosition = base.transform.localPosition;
			targetPosition = new Vector3(x2, y2, localPosition.z);
			targetScale = new Vector3(num, num, 1f);
			if (adjustAlpha)
			{
				startRange = targetAlpha;
				targetRange = alpha;
				targetAlpha = alpha;
			}
			GameSystem.Instance.RegisterAction(delegate
			{
				if (Mathf.Approximately(wait, 0f))
				{
					FinishAll();
				}
				else
				{
					if (adjustAlpha)
					{
						if (Mathf.Approximately(alpha, 0f))
						{
							FadeOut(wait);
						}
						else
						{
							FadeTo(alpha, wait);
						}
					}
					iTween.EaseType easeType = iTween.EaseType.linear;
					switch (easetype)
					{
					case 0:
						easeType = iTween.EaseType.linear;
						break;
					case 1:
						easeType = iTween.EaseType.easeInOutSine;
						break;
					case 2:
						easeType = iTween.EaseType.easeInOutSine;
						break;
					case 3:
						easeType = iTween.EaseType.easeInOutQuad;
						break;
					case 4:
						easeType = iTween.EaseType.easeInSine;
						break;
					case 5:
						easeType = iTween.EaseType.easeOutSine;
						break;
					case 6:
						easeType = iTween.EaseType.easeInQuad;
						break;
					case 7:
						easeType = iTween.EaseType.easeOutQuad;
						break;
					case 8:
						easeType = iTween.EaseType.easeInCubic;
						break;
					case 9:
						easeType = iTween.EaseType.easeOutCubic;
						break;
					case 10:
						easeType = iTween.EaseType.easeInQuart;
						break;
					case 11:
						easeType = iTween.EaseType.easeOutQuart;
						break;
					case 12:
						easeType = iTween.EaseType.easeInExpo;
						break;
					case 13:
						easeType = iTween.EaseType.easeOutExpo;
						break;
					case 14:
						easeType = iTween.EaseType.easeInExpo;
						break;
					case 15:
						easeType = iTween.EaseType.easeOutExpo;
						break;
					}
					iTween.ScaleTo(base.gameObject, iTween.Hash("scale", targetScale, "time", wait, "islocal", true, "easetype", easeType, "oncomplete", "FinishAll", "oncompletetarget", base.gameObject));
					iTween.MoveTo(base.gameObject, iTween.Hash("position", targetPosition, "time", wait, "islocal", true, "easetype", easeType, "oncomplete", "FinishAll", "oncompletetarget", base.gameObject));
					if (isBlocking)
					{
						if (Mathf.Approximately(alpha, 0f) && adjustAlpha)
						{
							GameSystem.Instance.AddWait(new Wait(wait, WaitTypes.WaitForMove, HideLayer));
						}
						else
						{
							GameSystem.Instance.AddWait(new Wait(wait, WaitTypes.WaitForMove, FinishAll));
						}
					}
					else if (wait > 0f)
					{
						StartCoroutine(WaitThenFinish(wait));
					}
				}
			});
		}

		public IEnumerator WaitThenFinish(float time)
		{
			yield return new WaitForSeconds(time);
			FinishAll();
			yield break;
		}

		public void FadeOutLayer(float time, bool isBlocking)
		{
			if (!(primary == null))
			{
				float current = targetRange;
				targetRange = 0f;
				targetAlpha = 0f;
				GameSystem.Instance.RegisterAction(delegate
				{
					if (Mathf.Approximately(time, 0f))
					{
						HideLayer();
					}
					else
					{
						material.shader = shaderDefault;
						current = 1f;
						FadingOut = true;
						iTween.ValueTo(base.gameObject, iTween.Hash("from", current, "to", targetRange, "time", time, "onupdate", "SetRange", "oncomplete", "HideLayer"));
						if (isBlocking)
						{
							GameSystem.Instance.AddWait(new Wait(time, WaitTypes.WaitForMove, HideLayer));
						}
					}
				});
			}
		}

		private void EnsureCorrectlySizedMesh(int width, int height, LayerAlignment alignment, Vector2? origin, Vector2? forceSize, bool isBustShot, int finalXOffset, string texturePath)
		{
			if (forceSize is Vector2 nonnullForceSize)
			{
				width = Mathf.RoundToInt(nonnullForceSize.x);
				height = Mathf.RoundToInt(nonnullForceSize.y);
			}
			bool ryukishiClamp = false;
			bool stretchToFit = false;
			if (texturePath != null)
			{
				bool isSpriteOrPortrait = texturePath.Contains("sprite/") ||
						texturePath.Contains("sprite\\") ||
						texturePath.Contains("portrait/") ||
						texturePath.Contains("portrait\\");

				if (Buriko.BurikoMemory.Instance.GetGlobalFlag("GRyukishiMode43Aspect").IntValue() != 0)
				{
					// When using true 4:3 mode, we don't need to clamp the sprites, as they are automatically cut off by the viewport
					ryukishiClamp = false;

					// When using true 4:3 aspect mode, any 16:9 images (except for sprites) should be squished to 4:3.
					// This make sure any text or other images don't get cut off
					// We could letter-box the images, but in some cases whatever is behind the image may show up? Not sure.
					stretchToFit = !isSpriteOrPortrait;

					// Do not stretch if the image is more than 5% off a 16:9 aspect ratio.
					// Likely these are special images like credits images or effect images.
					float targetAspect = 16f / 9f;
					float imageAspect = (float)width / (float)height;
					Debug.Log($"{texturePath}: aspect: {imageAspect} ref: {targetAspect}");
					if(imageAspect > targetAspect * 1.05 || imageAspect < targetAspect * .95f)
					{
						stretchToFit = false;
					}
				}
				else
				{
					// We want to clamp sprites to 4:3 if you are using the OG backgrounds, and you are not stretching the background
					ryukishiClamp = isBustShot &&
						Buriko.BurikoMemory.Instance.GetGlobalFlag("GBackgroundSet").IntValue() == 1 &&      // Using OG Backgrounds AND
						Buriko.BurikoMemory.Instance.GetGlobalFlag("GStretchBackgrounds").IntValue() == 0 && // Not stretching backgrounds AND
						isSpriteOrPortrait; // Is a sprite or portrait image. I don't think we can rely only on isBustShot, as sometimes non-sprites are drawn with isBustShot

					// When using old backgrounds with stretch backgrounds enabled, stretch old 4:3 backgrounds to 16:9 to fill the screen
					stretchToFit = Buriko.BurikoMemory.Instance.GetGlobalFlag("GStretchBackgrounds").IntValue() == 1 && texturePath.Contains("OGBackgrounds");
				}
			}

			if (mesh == null ||
				!Mathf.Approximately((float)width / height, aspectRatio) ||
				this.alignment != alignment ||
				this.origin != origin ||
				cachedRyukishiClamp != ryukishiClamp ||
				cachedFinalXOffset != finalXOffset ||
				cachedStretchToFit != stretchToFit)
			{
				cachedFinalXOffset = finalXOffset;
				cachedRyukishiClamp = ryukishiClamp;

				if (origin is Vector2 nonnullOrigin)
				{
					CreateMesh(width, height, nonnullOrigin, ryukishiClamp, finalXOffset, stretchToFit);
				}
				else
				{
					CreateMesh(width, height, alignment, ryukishiClamp, finalXOffset, stretchToFit);
				}
			}
			this.origin = origin;
			this.alignment = alignment;
			this.ForceSize = forceSize;
			this.aspectRatio = (float)width / height;
			cachedStretchToFit = stretchToFit;

			// Do not rotate character sprites when using 4:3 letterboxing as current method does not handle it properly
			if (ryukishiClamp)
			{
				targetAngle = 0;
				transform.localRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
			}
		}

		public void DrawLayerWithMask(string textureName, string maskName, int x, int y, Vector2? origin, Vector2? forceSize, bool isBustshot, int style, float wait, bool isBlocking, System.Action<Texture2D> afterLayerUpdated)
		{
			cachedIsBustShot = isBustshot;
			Texture2D texture2D = MODSceneController.LoadTextureWithFilters(layerID, textureName, out string texturePath);
			Texture2D maskTexture = AssetManager.Instance.LoadTexture(maskName);
			material.shader = shaderMasked;
			SetPrimaryTexture(texture2D);
			SetMaskTexture(maskTexture);
			PrimaryName = textureName;
			MaskName = maskName;
			startRange = 0f;
			targetRange = 1f;
			targetAlpha = 1f;
			targetAngle = 0f;
			shaderType = 0;
			EnsureCorrectlySizedMesh(
				width: texture2D.width, height: texture2D.height,
				alignment: ((x != 0 || y != 0) && !isBustshot) ? LayerAlignment.AlignTopleft : LayerAlignment.AlignCenter,
				origin: origin,
				forceSize: forceSize,
				isBustShot: isBustshot,
				finalXOffset: x,
				texturePath: texturePath
			);
			SetRange(startRange);
			base.transform.localPosition = new Vector3(x, -y, (float)Priority * -0.1f);
			GameSystem.Instance.RegisterAction(delegate
			{
				meshRenderer.enabled = true;
				material.SetFloat("_Fuzziness", (style != 0) ? 0.15f : 0.45f);
				material.SetFloat("_Direction", 1f);
				FadeInLayer(wait);
				if (isBlocking)
				{
					GameSystem.Instance.AddWait(new Wait(wait, WaitTypes.WaitForMove, FinishAll));
				}
				afterLayerUpdated?.Invoke(texture2D);
			});
		}

		public void DrawLayerWithMask(string textureName, string maskName, int x, int y, Vector2? origin, Vector2? forceSize, bool isBustshot, int style, float wait, bool isBlocking)
		{
			DrawLayerWithMask(textureName, maskName, x, y, origin, forceSize, isBustshot, style, wait, isBlocking, afterLayerUpdated: null);
		}

		public void FadeLayerWithMask(string maskName, int style, float time, bool isBlocking)
		{
			Texture2D maskTexture = AssetManager.Instance.LoadTexture(maskName);
			material.shader = shaderMasked;
			SetMaskTexture(maskTexture);
			material.SetFloat("_Fuzziness", (style != 0) ? 0.15f : 0.45f);
			material.SetFloat("_Direction", 0f);
			startRange = 1f;
			targetRange = 0f;
			targetAlpha = 0f;
			SetRange(startRange);
			GameSystem.Instance.RegisterAction(delegate
			{
				iTween.ValueTo(base.gameObject, iTween.Hash("from", startRange, "to", targetRange, "time", time, "onupdate", "SetRange", "oncomplete", "HideLayer"));
				if (isBlocking)
				{
					GameSystem.Instance.AddWait(new Wait(time, WaitTypes.WaitForMove, HideLayer));
				}
			});
		}

		public void DrawLayer(string textureName, int x, int y, int z, Vector2? origin, Vector2? forceSize, float alpha, bool isBustshot, int type, float wait, bool isBlocking, System.Action<Texture2D> afterLayerUpdated)
		{
			cachedIsBustShot = isBustshot;
			FinishAll();
			if (textureName == string.Empty)
			{
				HideLayer();
				return;
			}
			Texture2D texture2D = MODSceneController.LoadTextureWithFilters(layerID, textureName, out string texturePath);
			if (texture2D == null)
			{
				Logger.LogError("Failed to load texture " + textureName);
				return;
			}
			startRange = 0f;
			targetRange = alpha;
			targetAlpha = alpha;
			meshRenderer.enabled = true;
			shaderType = type;
			PrimaryName = textureName;
			float num = 1f;
			if (z > 0)
			{
				num = 1f - (float)z / 400f;
			}
			if (z < 0)
			{
				num = 1f + (float)z / -400f;
			}
			if (origin.HasValue)
			{
				Origin = origin;
			}
			EnsureCorrectlySizedMesh(
				width: texture2D.width, height: texture2D.height,
				alignment: ((x != 0 || y != 0) && !isBustshot) ? LayerAlignment.AlignTopleft : LayerAlignment.AlignCenter,
				origin: origin, 
				forceSize: forceSize,
				isBustShot: isBustshot,
				finalXOffset: x,
				texturePath: texturePath
			);
			if (primary != null)
			{
				material.shader = shaderCrossfade;
				SetSecondaryTexture(primary);
				SetPrimaryTexture(texture2D);
				startRange = 1f;
				targetRange = 0f;
				targetAlpha = 1f;
			}
			else
			{
				material.shader = shaderDefault;
				if (type == 3)
				{
					material.shader = shaderMultiply;
				}
				SetPrimaryTexture(texture2D);
			}
			SetRange(startRange);
			base.transform.localPosition = new Vector3(x, -y, (float)Priority * -0.1f);
			base.transform.localScale = new Vector3(num, num, 1f);
			targetPosition = base.transform.localPosition;
			targetScale = base.transform.localScale;
			if (Mathf.Approximately(wait, 0f))
			{
				FinishFade();
			}
			else
			{
				GameSystem.Instance.RegisterAction(delegate
				{
					if (Mathf.Approximately(wait, 0f))
					{
						FinishFade();
					}
					else
					{
						FadeInLayer(wait);
						if (isBlocking)
						{
							GameSystem.Instance.AddWait(new Wait(wait, WaitTypes.WaitForMove, FinishFade));
						}
					}
				});
			}
			afterLayerUpdated?.Invoke(texture2D);
		}

		public void DrawLayer(string textureName, int x, int y, int z, Vector2? origin, Vector2? forceSize, float alpha, bool isBustshot, int type, float wait, bool isBlocking)
		{
			DrawLayer(textureName, x, y, z, origin, forceSize, alpha, isBustshot, type, wait, isBlocking, afterLayerUpdated: null);
		}

		public void SetAngle(float angle, float wait)
		{
			// Do not rotate character sprites when using 4:3 letterboxing as current method does not handle it properly
			if(cachedRyukishiClamp)
			{
				return;
			}

			base.transform.localRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
			targetAngle = angle;
			GameSystem.Instance.RegisterAction(delegate
			{
				if (Mathf.Approximately(wait, 0f))
				{
					base.transform.localRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
				}
				else
				{
					iTween.RotateTo(base.gameObject, iTween.Hash("z", targetAngle, "time", wait, "isLocal", true, "easetype", "linear", "oncomplete", "FinishAll"));
				}
			});
		}

		public void CrossfadeLayer(string targetImage, float wait, bool isBlocking)
		{
			Texture2D primaryTexture = MODSceneController.LoadTextureWithFilters(layerID, targetImage);
			material.shader = shaderCrossfade;
			SetSecondaryTexture(primary);
			SetPrimaryTexture(primaryTexture);
			PrimaryName = targetImage;
			startRange = 1f;
			targetRange = 0f;
			targetAlpha = 1f;
			SetRange(startRange);
			GameSystem.Instance.RegisterAction(delegate
			{
				if (Mathf.Approximately(wait, 0f))
				{
					FinishFade();
				}
				else
				{
					FadeInLayer(wait);
					if (isBlocking)
					{
						GameSystem.Instance.AddWait(new Wait(wait, WaitTypes.WaitForMove, FinishFade));
					}
				}
			});
		}

		public bool UsingCrossShader()
		{
			if (material.shader.name == shaderCrossfade.name)
			{
				return true;
			}
			return false;
		}

		public void SwitchToAlphaShader()
		{
			material.shader = shaderAlphaBlend;
		}

		public void SwitchToMaskedShader()
		{
			material.shader = shaderReverseZ;
		}

		public void SetPriority(int newpriority)
		{
			Priority = newpriority + 1;
			Vector3 localPosition = base.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = base.transform.localPosition;
			targetPosition = new Vector3(x, localPosition2.y, (float)Priority * -0.1f);
			base.transform.localPosition = targetPosition;
		}

		public void FadeInLayer(float time)
		{
			iTween.Stop(base.gameObject);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", startRange, "to", targetRange, "time", time, "onupdate", "SetRange", "oncomplete", "FinishFade"));
		}

		public void FadeTo(float alpha, float time)
		{
			iTween.Stop(base.gameObject);
			startRange = targetRange;
			targetRange = alpha;
			iTween.ValueTo(base.gameObject, iTween.Hash("from", startRange, "to", targetRange, "time", time, "onupdate", "SetRange", "oncomplete", "FinishFade"));
		}

		public void FadeOut(float time)
		{
			if (material.shader.name == shaderCrossfade.name)
			{
				material.shader = shaderDefault;
				startRange = 1f;
			}
			FadingOut = true;
			targetRange = 0f;
			iTween.ValueTo(base.gameObject, iTween.Hash("from", startRange, "to", targetRange, "time", time, "onupdate", "SetRange", "oncomplete", "HideLayer"));
		}

		public void FinishAll()
		{
			StopCoroutine("MoveLayerEx");
			iTween.Stop(base.gameObject);
			FinishFade();
			base.transform.localPosition = targetPosition;
			base.transform.localScale = targetScale;
			base.transform.localRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
		}

		public void FinishFade()
		{
			iTween.Stop(base.gameObject);
			SetRange(targetRange);
		}

		public void SetRange(float a)
		{
			if (material.shader.name != shaderCrossfade.name && material.shader.name != shaderMasked.name)
			{
				material.SetFloat("_Alpha", a);
			}
			else
			{
				material.SetFloat("_Range", a);
			}
		}

		public void SetPrimaryTexture(Texture2D tex)
		{
			primary = tex;
			material.SetTexture("_Primary", primary);
			meshRenderer.enabled = true;
		}

		private void SetSecondaryTexture(Texture2D tex)
		{
			secondary = tex;
			material.SetTexture("_Secondary", secondary);
		}

		private void SetMaskTexture(Texture2D tex)
		{
			mask = tex;
			material.SetTexture("_Mask", mask);
		}

		public void HideLayer()
		{
			iTween.Stop(base.gameObject);
			ReleaseTextures();
			if (!IsStatic)
			{
				GameSystem.Instance.SceneController.LayerPool.ReturnLayer(this);
				GameSystem.Instance.SceneController.RemoveLayerReference(this);
			}
			targetAngle = 0f;
		}

		public void ReloadTexture()
		{
			if (PrimaryName == string.Empty)
			{
				// PrimaryName is set to string.Empty when Layer's texture is released -
				// therefore there is no texture to reload, so do nothing here.
				//
				// Note: Previously there was a HideLayer() call here, which caused problems
				// with backgrounds if ReloadTexture() was called just after the game had started
				// running, and DrawSceneWithMask() had not yet been called. It would cause
				// backgrounds to display for one frame(?), then turn black.
				//
				// I think may be a special layer is activated on startup, but has no texture set,
				// triggering this if statement.
				//
				// See https://github.com/07th-mod/higurashi-assembly/issues/104 for details
			}
			else
			{
				Texture2D texture2D = MODSceneController.LoadTextureWithFilters(layerID, PrimaryName, out string texturePath);
				if (texture2D == null)
				{
					Logger.LogError("Failed to load texture " + PrimaryName);
				}
				else
				{
					SetPrimaryTexture(texture2D);
					EnsureCorrectlySizedMesh(
						texture2D.width,
						texture2D.height,
						alignment,
						origin,
						ForceSize,
						isBustShot: cachedIsBustShot,
						finalXOffset: (int) base.transform.localPosition.x,
						texturePath: texturePath
					);
				}
			}
		}

		public void ReleaseTextures()
		{
			if (!(primary == null))
			{
				ReleaseSecondaryTexture();
				ReleaseMaskTexture();
				Object.Destroy(primary);
				primary = null;
				material.shader = shaderDefault;
				material.SetTexture("_Primary", null);
				meshRenderer.enabled = false;
				PrimaryName = string.Empty;
				SecondaryName = string.Empty;
				MaskName = string.Empty;
				Object.Destroy(mesh);
				mesh = null;
				meshFilter.mesh = null;
				FadingOut = false;
				shaderType = 0;
				targetAngle = 0f;
			}
		}

		private void ReleaseSecondaryTexture()
		{
			if (!(secondary == null))
			{
				Object.Destroy(secondary);
				secondary = null;
				SecondaryName = string.Empty;
				material.SetTexture("_Secondary", null);
			}
		}

		private void ReleaseMaskTexture()
		{
			if (!(mask == null))
			{
				Object.Destroy(mask);
				mask = null;
				MaskName = string.Empty;
				material.SetTexture("_Mask", null);
			}
		}

		// The below two CreateMesh functions clamp the image height to 480 
		// (the height of the screen in vertex coords) while maintaining the aspect ratio. 
		private void CreateMesh(int width, int height, Vector2 origin, bool ryukishiClamp, int finalXOffset, bool stretchToFit)
		{
			int num = height;
			if (height == 960)
			{
				num = 480;
			}
			int num2 = num / height;
			int width2 = Mathf.RoundToInt((float)Mathf.Clamp(width, 1, num2 * width));
			if (num > width2)
			{
				width2 = width;
				if (width == 1280)
				{
					width = 640;
				}
				num2 = width2 / width;
				num = Mathf.RoundToInt((float)Mathf.Clamp(height, 1, num2 * height));
			}
			if(stretchToFit)
			{
				width2 = Mathf.RoundToInt(num * GameSystem.Instance.AspectRatio);
			}
			mesh = MGHelper.CreateMeshWithOrigin(width2, num, origin, ryukishiClamp, finalXOffset);
			meshFilter.mesh = mesh;
		}

		private void CreateMesh(int width, int height, LayerAlignment alignment, bool ryukishiClamp, int finalXOffset, bool stretchToFit)
		{
			int num = Mathf.Clamp(height, 1, 480);
			float num2 = (float)num / (float)height;
			int width2 = Mathf.RoundToInt(Mathf.Clamp((float)width, 1f, num2 * (float)width));
			if (num > width2)
			{
				width2 = Mathf.Clamp(width, 1, 640);
				num2 = (float)width2 / (float)width;
				num = Mathf.RoundToInt(Mathf.Clamp((float)height, 1f, num2 * (float)height));
			}
			if (stretchToFit)
			{
				width2 = Mathf.RoundToInt(num * GameSystem.Instance.AspectRatio);
			}
			mesh = MGHelper.CreateMesh(width2, num, alignment, ryukishiClamp, finalXOffset);
			meshFilter.mesh = mesh;
		}

		public void Initialize()
		{
			shaderDefault = Shader.Find("MGShader/LayerShader");
			shaderAlphaBlend = Shader.Find("MGShader/LayerShaderAlpha");
			shaderCrossfade = Shader.Find("MGShader/LayerCrossfade4");
			shaderMasked = Shader.Find("MGShader/LayerMasked");
			shaderMultiply = Shader.Find("MGShader/LayerMultiply");
			shaderReverseZ = Shader.Find("MGShader/LayerShaderReverseZ");
			shaderType = 0;
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			material = new Material(shaderDefault);
			meshRenderer.material = material;
			meshRenderer.enabled = false;
			targetAngle = 0f;
			Origin = null;
			ForceSize = null;
			IsInitialized = true;
		}

		public void Serialize(BinaryWriter br)
		{
			br.Write(targetPosition);
			br.Write(targetScale);
			br.Write(PrimaryName);
			br.Write(targetAlpha);
			br.Write((int)alignment);
			br.Write(Origin);
			br.Write(ForceSize);
			br.Write(shaderType);
		}

		private void Awake()
		{
			if (!IsInitialized)
			{
				Initialize();
			}
		}

		private void Update()
		{
		}

		public void MODOnlyRecompile()
		{
		}

		public Texture2D GetPrimary() => primary;
	}
}
