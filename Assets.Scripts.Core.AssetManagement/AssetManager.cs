using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using BGICompiler.Compiler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.AssetManagement
{

	/// <summary>
	/// Stores an ordered list of paths for the engine to check when trying to find an asset
	/// </summary>
	public class PathCascadeList {
		public readonly string nameEN;
		public readonly string nameJP;
		public readonly string[] paths;
		public PathCascadeList(string nameEN, string nameJP, string[] paths)
		{
			this.nameEN = nameEN;
			this.nameJP = nameJP;
			this.paths = paths;
		}
	}

	public class AssetManager {
		private static AssetManager _instance;

		public List<PathCascadeList> Artsets = new List<PathCascadeList>();
		public int CurrentArtsetIndex = 0;
		public int ArtsetCount => Artsets.Count == 0 ? 2 : Artsets.Count;
		public PathCascadeList CurrentArtset => GetArtset(CurrentArtsetIndex);
		public bool ShouldSerializeArtsets = false;

		private Texture2D windowTexture;
		private string windowTexturePath = string.Empty;
		private Texture2D dummyTexture;

		private string assetPath = Application.streamingAssetsPath;

		private List<string> scriptList = new List<string>();

		public static AssetManager Instance => _instance ?? (_instance = GameSystem.Instance.AssetManager);

		public string debugLastBGM { get; private set; } = "No BGM played yet";
		public string debugLastSE { get; private set; } = "No SE played yet";
		public string debugLastVoice { get; private set; } = "No voice played yet";
		public string debugLastOtherAudio { get; private set; } = "No other audio played yet";

		public List<PathCascadeList> BGMCascades = new List<PathCascadeList>()
		{
			new PathCascadeList("April Update", "April Update", new string[] { "BGM" }),
			new PathCascadeList("Original", "Original", new string[] { "OGBGM" , "BGM" }),
		};

		public List<PathCascadeList> SECascades = new List<PathCascadeList>()
		{
			new PathCascadeList("April Update", "April Update", new string[] { "SE" }),
			new PathCascadeList("Original", "Original", new string[] { "OGSE" , "SE" }),
		};

		public List<PathCascadeList> voiceCascades = new List<PathCascadeList>()
		{
			new PathCascadeList("PS3", "PS3", new string[] { "voice" }),
		};


		/// <summary>
		/// Get the artset at the given index
		/// </summary>
		/// <param name="index">The index of the artset to get</param>
		/// <returns></returns>
		public PathCascadeList GetArtset(int index)
		{
			// To maintain compatibility with scripts that don't specify artsets, if none have been added act like the base game
			if (Artsets.Count == 0)
			{
				if (index == 0)
				{
					return new PathCascadeList("Console", "ゲーム機", new string[] { "CG" });
				}
				if (index == 1)
				{
					return new PathCascadeList("Remake", "リメーク", new string[] { "CGAlt", "CG" });
				}
			}
			if (index >= 0 && index < Artsets.Count)
			{
				return Artsets[index];
			}
			return new PathCascadeList("Unknown (" + index + ")", "不明(" + index + ")", new string[] { "CG" });
		}

		public void AddArtset(PathCascadeList artset)
		{
			Artsets.Add(artset);
		}

		public void ClearArtsets()
		{
			Artsets.Clear();
		}

		/// <summary>
		/// Gets the path to an asset with the given name in the given artset, or null if none are found
		/// </summary>
		/// <returns>A path to an on-disk asset or null</returns>
		public string PathToAssetWithName(string name, PathCascadeList artset)
		{
			int backgroundSetIndex = BurikoMemory.Instance.GetGlobalFlag("GBackgroundSet").IntValue();

			// If force og backgrounds is enabled, always check OGBackgrounds first.
			if (backgroundSetIndex == 2)
			{
				string filePath = Path.Combine(Path.Combine(assetPath, "OGBackgrounds"), name);
				if (File.Exists(filePath))
				{
					return filePath;
				}
			}

			foreach (var artSetPath in artset.paths)
			{
				// If force console backgrounds is enabled, don't check OGBackgrounds
				if (backgroundSetIndex == 1 && artSetPath == "OGBackgrounds")
				{
					continue;
				}

				string filePath = Path.Combine(Path.Combine(assetPath, artSetPath), name);
				if (File.Exists(filePath))
				{
					return filePath;
				}
			}
			return null;
		}

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

			// If we want to use the game just to compile scripts in an automated manner, we need
			// some way to terminate the game after scripts are compiled.
			// The below code will terminate the game after scripts are compiled if "quitaftercompile"
			// is passed as a command-line argument to the game
			// The code will also try to write a higu_script_compile_status.txt as proof that
			// the scripts really did compile OK
			if (Environment.GetCommandLineArgs().Contains("quitaftercompile"))
			{
				GameSystem.Instance.CanExit = true;
				try
				{
					System.IO.File.WriteAllText("higu_script_compile_status.txt", "Compile OK");
				}
				catch
				{

				}
				Application.Quit();
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
					IL_008d:
					Texture2D result;
					return result;
				}
				catch (Exception)
				{
					return LoadTexture("no_data");
					IL_00a5:
					Texture2D result;
					return result;
				}
			}
			return LoadTexture("no_data");
		}

		public Texture2D LoadTexture(string textureName)
		{
			return LoadTexture(textureName, out _);
		}

		public Texture2D LoadTexture(string textureName, out string texturePath)
		{
			if (textureName == "windo_filter" && windowTexture != null)
			{
				texturePath = windowTexturePath;
				return windowTexture;
			}
			string path = null;

			// Load path from current artset
			if (path == null && !GameSystem.Instance.UseEnglishText)
			{
				path = PathToAssetWithName(textureName.ToLower() + "_j.png", CurrentArtset);
			}

			if (path == null)
			{
				path = PathToAssetWithName(textureName.ToLower() + ".png", CurrentArtset);
			}

			if (path == null)
			{
				Logger.LogWarning("Could not find texture asset " + textureName.ToLower() + " in " + CurrentArtset.nameEN);
				// When returning null here, most functions won't crash, but this call chain does crash:
				// OperationDrawSpriteWithFiltering() -> DrawSpriteWithFiltering() -> DrawLayerWithMask()
				// Returning a dummy texture instead of null prevents these crashes
				if (dummyTexture == null)
				{
					dummyTexture = new Texture2D(0, 0, TextureFormat.ARGB32, mipmap: true);
				}
				texturePath = "dummy_texture";
				return dummyTexture;
			}
			byte[] array = File.ReadAllBytes(path);
			if (array == null || array.Length == 0)
			{
				throw new Exception("Failed loading texture " + path);
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
				windowTexturePath = path;
			}
			texturePath = path;
			return texture2D;
		}

		public string getAssetUsingFlagAndCascade(string filename, int flag, List<PathCascadeList> cascades, out bool exists)
		{
			exists = false;

			PathCascadeList cascade = cascades[0];
			if(flag < cascades.Count)
			{
				cascade = cascades[flag];
			}

			// Use the first file that exists. If none exist, return the last one.
			string relativePath = "INVALID ASSET PATH";
			foreach (string assetSubFolder in cascade.paths)
			{
				relativePath = Path.Combine(assetSubFolder, filename);
				if (File.Exists(Path.Combine(assetPath, relativePath)))
				{
					exists = true;
					break;
				}
			}

			return relativePath;
		}

		public string _GetAudioFilePath(string filename, Audio.AudioType type, out bool ok)
		{
			switch (type)
			{
				case Audio.AudioType.BGM:
					return getAssetUsingFlagAndCascade(filename, BurikoMemory.Instance.GetGlobalFlag("GAltBGM").IntValue(), BGMCascades, out ok);

				case Audio.AudioType.SE:
				case Audio.AudioType.System:
					return getAssetUsingFlagAndCascade(filename, BurikoMemory.Instance.GetGlobalFlag("GAltSE").IntValue(), SECascades, out ok);

				case Audio.AudioType.Voice:
					int voiceFlag = BurikoMemory.Instance.GetGlobalFlag("GAltVoicePriority").IntValue();
					if (BurikoMemory.Instance.GetGlobalFlag("GAltVoice").IntValue() == 0)
					{
						voiceFlag = 0;
					}
					return getAssetUsingFlagAndCascade(filename, voiceFlag, voiceCascades, out ok);

				default:
					Debug.Log($"_GetAudioFilePath(): Cannot play '{filename}' due to unknown AudioType '{type}' - ignoring this file");
					ok = false;
					return "";
			}
		}

		public string GetAudioFilePath(string filename, Audio.AudioType type)
		{
			string relativePath = _GetAudioFilePath(filename, type, out bool ok);
			string debugRelativePath = $"{relativePath} ({(ok ? "OK" : "FAILED to load")})";
			// Record the last played BGM and SE only for debugging purposes
			switch (type)
			{
				case Audio.AudioType.BGM:
					debugLastBGM = debugRelativePath;
					break;
				case Audio.AudioType.SE:
					debugLastSE = debugRelativePath;
					break;
				case Audio.AudioType.Voice:
					debugLastVoice = debugRelativePath;
					break;
				default:
					debugLastOtherAudio = debugRelativePath;
					break;
			}
			return Path.Combine(assetPath, relativePath);
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
