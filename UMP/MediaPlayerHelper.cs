using Assets.Scripts.Core.Scene;
using MOD.Scripts.Core.Movie;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace UMP
{
	public class MediaPlayerHelper
	{
		private enum ManageLogLevel
		{
			DEBUG,
			WARNING,
			ERROR
		}

		private delegate void ManageLogCallback(string msg, ManageLogLevel level);

		private const string UMP_FOLDER_NAME = "/UniversalMediaPlayer";

		private const string LOCAL_FILE_ROOT = "file:///";

		private ManageLogCallback _manageLogCallback;

		private static Regex _androidStorageRoot = new Regex("(^\\/.*)Android");

		private void DebugLogHandler(string msg, ManageLogLevel level)
		{
			Debug.LogError(msg);
		}

		public static void ApplyTextureToRenderingObjects(Texture2D texture, GameObject[] renderingObjects)
		{
			if (renderingObjects == null)
			{
				return;
			}
			foreach (GameObject gameObject in renderingObjects)
			{
				if (gameObject == null)
				{
					continue;
				}
				// Hacky workaround

				Layer layer = gameObject.GetComponent<Layer>();
				layer.MODMaterial.SetTexture("_Primary", texture);
				Debug.Log($"Set layer {layer.name} to {texture.name}");

				//RawImage component = gameObject.GetComponent<RawImage>();
				//if (component != null)
				//{
				//	component.texture = texture;
				//	continue;
				//}
				//MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
				//if (component2 != null && component2.material != null)
				//{
				//	component2.material.mainTexture = texture;
				//}
				//else
				//{
				//	Debug.LogError(gameObject.name + ": don't have 'RawImage' or 'MeshRenderer' component - ignored");
				//}
			}
		}

		public static Texture2D GenVideoTexture(int width, int height)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Win || UMPSettings.RuntimePlatform == UMPSettings.Platforms.Mac || UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return new Texture2D(width, height, TextureFormat.BGRA32, mipmap: false);
			}
			return new Texture2D(width, height, TextureFormat.RGBA32, mipmap: false);
		}

		internal static Texture2D GenPluginTexture(int width, int height)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Win || UMPSettings.RuntimePlatform == UMPSettings.Platforms.Mac || UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return new Texture2D(width, height, TextureFormat.BGRA32, mipmap: false);
			}
			return new Texture2D(width, height, TextureFormat.RGBA32, mipmap: false);
		}

		public static Color GetAverageColor(byte[] frameBuffer)
		{
			if (frameBuffer == null)
			{
				return Color.black;
			}
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			int num5 = frameBuffer.Length / 4;
			if (num5 <= 0 || num5 % 4 != 0)
			{
				return Color.black;
			}
			for (int i = 0; i < frameBuffer.Length; i += 4)
			{
				num += (int)frameBuffer[i];
				num2 += (int)frameBuffer[i + 1];
				num3 += (int)frameBuffer[i + 2];
				num4 += (int)frameBuffer[i + 3];
			}
			return new Color(num / num5, num2 / num5, num3 / num5, num4 / num5);
		}

		public static Color32[] GetFrameColors(byte[] frameBuffer)
		{
			Color32[] array = new Color32[frameBuffer.Length / 4];
			for (int i = 0; i < frameBuffer.Length; i += 4)
			{
				Color32 color = new Color32(frameBuffer[i + 2], frameBuffer[i + 1], frameBuffer[i], frameBuffer[i + 3]);
				array[i / 4] = color;
			}
			return array;
		}

		public static string GetDeviceRootPath()
		{
			Match match = _androidStorageRoot.Match(Application.persistentDataPath);
			if (match.Length > 1)
			{
				return match.Groups[1].Value;
			}
			return Application.persistentDataPath;
		}

		public static bool IsAssetsFile(string filePath)
		{
			if (filePath.StartsWith("file:///"))
			{
				filePath = filePath.Substring("file:///".Length);
			}
			if (!filePath.Contains(Application.streamingAssetsPath))
			{
				filePath = Path.Combine(Application.streamingAssetsPath, filePath);
			}
			if (UMPSettings.RuntimePlatform != UMPSettings.Platforms.Android)
			{
				return File.Exists(filePath);
			}
			WWW wWW = new WWW(filePath);
			while (!wWW.isDone && wWW.progress <= 0f)
			{
			}
			bool result = string.IsNullOrEmpty(wWW.error);
			wWW.Dispose();
			return result;
		}

		public static string GetDataSourcePath(string relativePath)
		{
			if (relativePath.StartsWith("file:///"))
			{
				relativePath = relativePath.Substring("file:///".Length);
				if (IsAssetsFile(relativePath))
				{
					relativePath = Path.Combine(Application.streamingAssetsPath, relativePath);
				}
			}
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Android)
			{
				string text = GetDeviceRootPath() + relativePath;
				if (File.Exists(text))
				{
					relativePath = text;
				}
			}
			if (File.Exists(relativePath))
			{
				relativePath = "file:///" + relativePath;
			}
			return relativePath;
		}
	}
}
