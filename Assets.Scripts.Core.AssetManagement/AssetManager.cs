using Assets.Scripts.Core.Audio;
using BGICompiler.Compiler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core.AssetManagement
{
	public class AssetManager
	{
		private static AssetManager _instance;

		public bool UseNewArt = true;

		private Texture2D windowTexture;

		private string assetPath = Application.streamingAssetsPath;

		private List<string> scriptList = new List<string>();

		public static AssetManager Instance => _instance ?? (_instance = GameSystem.Instance.AssetManager);

		public void CompileFolder(string srcDir, string destDir)
		{
			string[] files = Directory.GetFiles(srcDir, "*.txt");
			string[] files2 = Directory.GetFiles(destDir, "*.mg");
			List<string> list = new List<string>();
			string[] array = files;
			foreach (string text in array)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				if (fileNameWithoutExtension != null)
				{
					list.Add(fileNameWithoutExtension);
					string text2 = text;
					string text3 = Path.Combine(destDir, fileNameWithoutExtension) + ".mg";
					if (File.Exists(text3))
					{
						if (File.GetLastWriteTime(text2) <= File.GetLastWriteTime(text3))
						{
							continue;
						}
						Debug.Log($"Script {text3} last compiled {File.GetLastWriteTime(text3)} (source {text2} updated on {File.GetLastWriteTime(text2)})");
					}
					Debug.Log("Compiling file " + text2);
					try
					{
						new BGItoMG(text2, text3);
					}
					catch (Exception arg)
					{
						Debug.LogError($"Failed to compile script {fileNameWithoutExtension}!\r\n{arg}");
					}
				}
			}
			string[] array2 = files2;
			foreach (string path in array2)
			{
				string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(path);
				if (!list.Contains(fileNameWithoutExtension2))
				{
					Debug.Log("Compiled script " + fileNameWithoutExtension2 + " has no matching script file. Removing...");
					File.Delete(path);
				}
			}
		}

		public void CompileIfNeeded()
		{
			string path = Path.Combine(assetPath, "Scripts");
			string text = Path.Combine(assetPath, "Update");
			string text2 = Path.Combine(assetPath, "CompiledScripts");
			string destDir = Path.Combine(assetPath, "CompiledUpdateScripts");
			string[] files = Directory.GetFiles(path, "*.txt");
			string[] files2 = Directory.GetFiles(text, "*.txt");
			Debug.Log("Checking update scripts for updates...");
			CompileFolder(text, destDir);
			string[] files3 = Directory.GetFiles(Path.Combine(assetPath, "CompiledScripts"));
			string[] files4 = Directory.GetFiles(Path.Combine(assetPath, "CompiledUpdateScripts"));
			string[] array = files3;
			foreach (string path2 in array)
			{
				if (!(Path.GetExtension(path2) != ".mg"))
				{
					string fileName = Path.GetFileName(path2);
					if (!scriptList.Contains(fileName))
					{
						scriptList.Add(fileName);
					}
				}
			}
			string[] array2 = files4;
			foreach (string path3 in array2)
			{
				if (!(Path.GetExtension(path3) != ".mg"))
				{
					string fileName2 = Path.GetFileName(path3);
					if (!scriptList.Contains(fileName2))
					{
						scriptList.Add(fileName2);
					}
				}
			}
		}

		private string GetArchiveNameByAudioType(Assets.Scripts.Core.Audio.AudioType audioType)
		{
			switch (audioType)
			{
			case Assets.Scripts.Core.Audio.AudioType.BGM:
				return "BGM";
			case Assets.Scripts.Core.Audio.AudioType.Voice:
				return "voice";
			case Assets.Scripts.Core.Audio.AudioType.SE:
				return "SE";
			case Assets.Scripts.Core.Audio.AudioType.System:
				return "SE";
			default:
				throw new InvalidEnumArgumentException("GetArchiveNameByAudioType: Invalid audiotype " + audioType);
			}
		}

		private static int ReadLittleEndianInt32(byte[] bytes)
		{
			byte[] array = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				array[3 - i] = bytes[i];
			}
			return BitConverter.ToInt32(array, 0);
		}

		public Texture2D LoadScreenshot(string filename)
		{
			string savePath = MGHelper.GetSavePath();
			string path = Path.Combine(savePath, filename.ToLower());
			if (File.Exists(path))
			{
				try
				{
					byte[] array = File.ReadAllBytes(path);
					byte[] array2 = new byte[4];
					Buffer.BlockCopy(array, 16, array2, 0, 4);
					int width = ReadLittleEndianInt32(array2);
					Buffer.BlockCopy(array, 20, array2, 0, 4);
					int height = ReadLittleEndianInt32(array2);
					Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, mipmap: false);
					texture2D.LoadImage(array);
					texture2D.filterMode = FilterMode.Bilinear;
					texture2D.wrapMode = TextureWrapMode.Clamp;
					return texture2D;
				}
				catch (Exception)
				{
					return LoadTexture("no_data");
				}
			}
			return LoadTexture("no_data");
		}

		public Texture2D LoadTexture(string textureName)
		{
			if (textureName == "windo_filter" && windowTexture != null)
			{
				return windowTexture;
			}
			string path = Path.Combine(assetPath, "CG/" + textureName.ToLower() + "_j.png");
			string path2 = Path.Combine(assetPath, "CGAlt/" + textureName.ToLower() + "_j.png");
			string text = Path.Combine(assetPath, "CG/" + textureName.ToLower() + ".png");
			string path3 = Path.Combine(assetPath, "CGAlt/" + textureName.ToLower() + ".png");
			byte[] array = new byte[0];
			bool flag = false;
			if (!GameSystem.Instance.UseEnglishText)
			{
				if (UseNewArt && File.Exists(path2))
				{
					array = File.ReadAllBytes(path2);
					flag = true;
				}
				else if (File.Exists(path))
				{
					array = File.ReadAllBytes(path);
					flag = true;
				}
			}
			if (!flag)
			{
				if (UseNewArt && File.Exists(path3))
				{
					array = File.ReadAllBytes(path3);
				}
				else
				{
					if (!File.Exists(text))
					{
						Logger.LogWarning("Could not find texture asset " + text);
						return null;
					}
					array = File.ReadAllBytes(text);
				}
			}
			if (array == null || array.Length == 0)
			{
				throw new Exception("Failed loading texture " + textureName.ToLower());
			}
			byte[] array2 = new byte[4];
			Buffer.BlockCopy(array, 16, array2, 0, 4);
			int width = ReadLittleEndianInt32(array2);
			Buffer.BlockCopy(array, 20, array2, 0, 4);
			int height = ReadLittleEndianInt32(array2);
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, mipmap: true);
			texture2D.mipMapBias = -0.5f;
			texture2D.LoadImage(array);
			texture2D.filterMode = FilterMode.Bilinear;
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.name = textureName;
			if (textureName == "windo_filter")
			{
				windowTexture = texture2D;
			}
			return texture2D;
		}

		public string GetAudioFilePath(string filename, Assets.Scripts.Core.Audio.AudioType type)
		{
			string archiveNameByAudioType = GetArchiveNameByAudioType(type);
			return Path.Combine(assetPath, archiveNameByAudioType + "/" + filename.ToLower()).Replace("\\", "/");
		}

		public byte[] GetAudioFile(string filename, Assets.Scripts.Core.Audio.AudioType type)
		{
			string archiveNameByAudioType = GetArchiveNameByAudioType(type);
			string path = Path.Combine(assetPath, archiveNameByAudioType + "/" + filename.ToLower());
			return File.ReadAllBytes(path);
		}

		public byte[] GetScriptData(string filename)
		{
			string path = Path.Combine(assetPath, "CompiledUpdateScripts/" + filename.ToLower());
			if (File.Exists(path))
			{
				Debug.Log("Loading script " + filename + " from update folder.");
				return File.ReadAllBytes(path);
			}
			path = Path.Combine(assetPath, "CompiledScripts/" + filename.ToLower());
			return File.ReadAllBytes(path);
		}

		public string[] GetAvailableScriptNames()
		{
			return scriptList.ToArray();
		}
	}
}
