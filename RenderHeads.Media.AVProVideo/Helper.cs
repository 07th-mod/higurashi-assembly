using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
	public static class Helper
	{
		public const string ScriptVersion = "1.5.8";

		public static string GetName(Platform platform)
		{
			string text = "Unknown";
			return platform.ToString();
		}

		public static string[] GetPlatformNames()
		{
			return new string[8]
			{
				GetName(Platform.Windows),
				GetName(Platform.MacOSX),
				GetName(Platform.iOS),
				GetName(Platform.tvOS),
				GetName(Platform.Android),
				GetName(Platform.WindowsPhone),
				GetName(Platform.WindowsUWP),
				GetName(Platform.WebGL)
			};
		}

		public static void LogInfo(string message, Object context = null)
		{
			if (context == null)
			{
				Debug.Log("[AVProVideo] " + message);
			}
			else
			{
				Debug.Log("[AVProVideo] " + message, context);
			}
		}

		public static string GetTimeString(float totalSeconds)
		{
			int num = Mathf.FloorToInt(totalSeconds / 3600f);
			float num2 = (float)num * 60f * 60f;
			int num3 = Mathf.FloorToInt((totalSeconds - num2) / 60f);
			num2 += (float)num3 * 60f;
			int num4 = Mathf.RoundToInt(totalSeconds - num2);
			if (num <= 0)
			{
				return $"{num3:00}:{num4:00}";
			}
			return string.Format("{2}:{0:00}:{1:00}", num3, num4, num);
		}

		public static void SetupStereoMaterial(Material material, StereoPacking packing, bool displayDebugTinting)
		{
			material.DisableKeyword("STEREO_TOP_BOTTOM");
			material.DisableKeyword("STEREO_LEFT_RIGHT");
			material.DisableKeyword("MONOSCOPIC");
			switch (packing)
			{
			case StereoPacking.TopBottom:
				material.EnableKeyword("STEREO_TOP_BOTTOM");
				break;
			case StereoPacking.LeftRight:
				material.EnableKeyword("STEREO_LEFT_RIGHT");
				break;
			}
			if (displayDebugTinting)
			{
				material.EnableKeyword("STEREO_DEBUG");
			}
			else
			{
				material.DisableKeyword("STEREO_DEBUG");
			}
		}

		public static void SetupAlphaPackedMaterial(Material material, AlphaPacking packing)
		{
			material.DisableKeyword("ALPHAPACK_TOP_BOTTOM");
			material.DisableKeyword("ALPHAPACK_LEFT_RIGHT");
			material.DisableKeyword("ALPHAPACK_NONE");
			switch (packing)
			{
			case AlphaPacking.TopBottom:
				material.EnableKeyword("ALPHAPACK_TOP_BOTTOM");
				break;
			case AlphaPacking.LeftRight:
				material.EnableKeyword("ALPHAPACK_LEFT_RIGHT");
				break;
			}
		}

		public static void SetupGammaMaterial(Material material, bool playerSupportsLinear)
		{
			if (QualitySettings.activeColorSpace == ColorSpace.Linear && !playerSupportsLinear)
			{
				material.EnableKeyword("APPLY_GAMMA");
			}
			else
			{
				material.DisableKeyword("APPLY_GAMMA");
			}
		}

		public static float ConvertFrameToTimeSeconds(int frame, float frameRate)
		{
			float num = 1f / frameRate;
			return (float)frame * num + num * 0.5f;
		}

		public static void DrawTexture(Rect screenRect, Texture texture, ScaleMode scaleMode, AlphaPacking alphaPacking, Material material)
		{
			if (Event.current.type == EventType.Repaint)
			{
				float num = (float)texture.width;
				float num2 = (float)texture.height;
				switch (alphaPacking)
				{
				case AlphaPacking.LeftRight:
					num *= 0.5f;
					break;
				case AlphaPacking.TopBottom:
					num2 *= 0.5f;
					break;
				}
				float num3 = num / num2;
				Rect sourceRect = new Rect(0f, 0f, 1f, 1f);
				switch (scaleMode)
				{
				case ScaleMode.ScaleAndCrop:
				{
					float num7 = screenRect.width / screenRect.height;
					if (num7 > num3)
					{
						float num8 = num3 / num7;
						sourceRect = new Rect(0f, (1f - num8) * 0.5f, 1f, num8);
					}
					else
					{
						float num9 = num7 / num3;
						sourceRect = new Rect(0.5f - num9 * 0.5f, 0f, num9, 1f);
					}
					break;
				}
				case ScaleMode.ScaleToFit:
				{
					float num4 = screenRect.width / screenRect.height;
					if (num4 > num3)
					{
						float num5 = num3 / num4;
						screenRect = new Rect(screenRect.xMin + screenRect.width * (1f - num5) * 0.5f, screenRect.yMin, num5 * screenRect.width, screenRect.height);
					}
					else
					{
						float num6 = num4 / num3;
						screenRect = new Rect(screenRect.xMin, screenRect.yMin + screenRect.height * (1f - num6) * 0.5f, screenRect.width, num6 * screenRect.height);
					}
					break;
				}
				}
				Graphics.DrawTexture(screenRect, texture, sourceRect, 0, 0, 0, 0, GUI.color, material);
			}
		}

		public static Texture2D GetReadableTexture(Texture inputTexture, bool requiresVerticalFlip, Texture2D targetTexture)
		{
			Texture2D texture2D = targetTexture;
			RenderTexture active = RenderTexture.active;
			RenderTexture temporary = RenderTexture.GetTemporary(inputTexture.width, inputTexture.height, 0, RenderTextureFormat.ARGB32);
			if (!requiresVerticalFlip)
			{
				Graphics.Blit(inputTexture, temporary);
			}
			else
			{
				GL.PushMatrix();
				RenderTexture.active = temporary;
				GL.LoadPixelMatrix(0f, (float)temporary.width, 0f, (float)temporary.height);
				Rect sourceRect = new Rect(0f, 0f, 1f, 1f);
				Rect screenRect = new Rect(0f, -1f, (float)temporary.width, (float)temporary.height);
				Graphics.DrawTexture(screenRect, inputTexture, sourceRect, 0, 0, 0, 0);
				GL.PopMatrix();
				GL.InvalidateState();
			}
			if (texture2D == null)
			{
				texture2D = new Texture2D(inputTexture.width, inputTexture.height, TextureFormat.ARGB32, mipChain: false);
			}
			RenderTexture.active = temporary;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)inputTexture.width, (float)inputTexture.height), 0, 0, recalculateMipMaps: false);
			texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.active = active;
			return texture2D;
		}
	}
}
