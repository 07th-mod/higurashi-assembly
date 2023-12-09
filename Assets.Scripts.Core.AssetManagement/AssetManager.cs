using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using BGICompiler.Compiler;
using MOD.Scripts.Core.Audio;
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

		public bool PrimaryFolder(out string primaryFolder)
		{
			if(paths.Length == 0)
			{
				primaryFolder = "";
				return false;
			}

			primaryFolder = paths[0];
			return true;
		}

		public bool IsInstalled(string rootPath)
		{
			if (!PrimaryFolder(out string primaryFolder))
			{
				return false;
			}

			return Directory.Exists(Path.Combine(rootPath, primaryFolder));
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

		public int CurrentLoading;

		public int MaxLoading;

		public bool AbortLoading;

		private List<string> scriptList = new List<string>();

		public static AssetManager Instance => _instance ?? (_instance = GameSystem.Instance.AssetManager);

		public string debugLastBGM { get; private set; } = "No BGM played yet";
		public string debugLastSE { get; private set; } = "No SE played yet";
		public string debugLastVoice { get; private set; } = "No voice played yet";
		public string debugLastOtherAudio { get; private set; } = "No other audio played yet";

		public int numCompileOK { get; private set; }
		public int numCompileFail { get; private set; }

		public static readonly int ScreenshotWidth = 382;
		public static readonly int ScreenshotHeight = 286;

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

		/// <param name="pathNoExt">The part of the path before the first dot (.)</param>
		/// <param name="ext">The part of the path after the first dot (.). Does not include the dot.</param>
		private void SplitPathOnFileExtension(string inputPath, out string pathNoExt, out string ext)
		{
			string[] splitPath = inputPath.Split(new char[] { '.' }, 2);

			if (splitPath.Length < 2)
			{
				pathNoExt = inputPath;
				ext = "";
				return;
			}
			else
			{
				pathNoExt = splitPath[0];
				ext = splitPath[1];
				return;
			}
		}

		/// <param name="subFolder">Subfolder in the StreamingAssets folder</param>
		/// <param name="relativePath">File path relative to subFolder</param>
		/// <param name="filePath">Output filepath - only valid if function returns true</param>
		/// <returns></returns>
		private bool CheckStreamingAssetsPathExistsInner(string subFolder, string relativePath, out string filePath)
		{
			filePath = Path.Combine(Path.Combine(assetPath, subFolder), relativePath);
			if (File.Exists(filePath))
			{
				return true;
			}

			return false;
		}

		private bool CheckStreamingAssetsPathExists(string subFolder, string relativePath, out string filePath)
		{
			if(CheckStreamingAssetsPathExistsInner(subFolder, relativePath, out filePath))
			{
				return true;
			}

			// Don't allow substituting sprite lipsync variants if using the Console sprites,
			// to make missing Console sprites more obvious during development.
			if (CurrentArtsetIndex == 0)
			{
				return false;
			}

			// Only allow lipsync variants for files in the 'sprite' or 'portrait' folder.
			bool isSprite = relativePath.StartsWith("sprite/") || relativePath.StartsWith("portrait/");
			if (!isSprite)
			{
				return false;
			}

			SplitPathOnFileExtension(relativePath, out string pathNoExt, out string ext);

			string pathWithoutVariantNumber = pathNoExt.Substring(0, Math.Max(pathNoExt.Length - 1, 0));

			for (int i = 0; i < 3; i++)
			{
				string lipsyncVariantFileName = $"{pathWithoutVariantNumber}{i}.{ext}";

				// If the name to test is the same as the initial name, don't test it again
				if (lipsyncVariantFileName == relativePath)
				{
					continue;
				}

				if (CheckStreamingAssetsPathExistsInner(subFolder, lipsyncVariantFileName, out filePath))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the path to an asset with the given name in the given artset, or null if none are found
		/// </summary>
		/// <returns>A path to an on-disk asset or null</returns>
		public string PathToAssetWithName(string name, PathCascadeList artset)
		{
			int backgroundSetIndex = BurikoMemory.Instance.GetGlobalFlag("GBackgroundSet").IntValue();

			// If OG backgrounds are enabled, always check OGBackgrounds first.
			if (backgroundSetIndex == 1)
			{
				if(CheckStreamingAssetsPathExists("OGBackgrounds", name, out string filePath))
				{
					return filePath;
				}
			}

			foreach (var artSetPath in artset.paths)
			{
				// If console backgrounds are enabled, don't check OGBackgrounds
				if (backgroundSetIndex == 0 && artSetPath == "OGBackgrounds")
				{
					continue;
				}

				if (CheckStreamingAssetsPathExists(artSetPath, name, out string filePath))
				{
					return filePath;
				}
			}

			return null;
		}

		// The arguments AbortLoading, MaxLoading, and CurrentLoading are only used for meakashi onwards
		// MaxLoading, and CurrentLoading are are used to display the Script Compilation Progress Text
		// I'm not sure if AbortLoading is ever used
		private void CompileFolder(string srcDir, string destDir)
		{
			MODCompileRequiredDetector detector = new MODCompileRequiredDetector(destDir);
			detector.Load();

			MaxLoading = 0;
			CurrentLoading = 0;

			string[] txtList1 = Directory.GetFiles(srcDir, "*.txt");
			string[] mgList1 = Directory.GetFiles(destDir, "*.mg");
			List<string> scriptNames = new List<string>();

			List<string> txtToCompileList = new List<string>();
			List<string> mgToCompileList = new List<string>();

			string[] txtList = txtList1;
			foreach (string txtPath1 in txtList)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(txtPath1);
				if (fileNameWithoutExtension == null)
				{
					continue;
				}
				scriptNames.Add(fileNameWithoutExtension);
				string txtPath = txtPath1;
				string mgPath = Path.Combine(destDir, fileNameWithoutExtension) + ".mg";
				if(!detector.SaveScriptInfoAndCheckScriptNeedsCompile(txtPath, mgPath))
				{
					detector.MarkScriptCompiled(fileNameWithoutExtension);
					continue;
				}
				txtToCompileList.Add(txtPath);
				mgToCompileList.Add(mgPath);
			}

			MaxLoading = txtToCompileList.Count;
			for (int j = 0; j < txtToCompileList.Count; j++)
			{
				CurrentLoading = j + 1;
				string text4 = txtToCompileList[j];
				string outname = mgToCompileList[j];
				string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(text4);
				Debug.Log("Compiling file " + text4);
				try
				{
					new BGItoMG(text4, outname);
					numCompileOK++;
					detector.MarkScriptCompiled(fileNameWithoutExtension2);
				}
				catch (Exception arg)
				{
					Debug.LogError($"Failed to compile script {fileNameWithoutExtension2}!\r\n{arg}");
					numCompileFail++;
				}
				if (AbortLoading)
				{
					return;
				}
			}

			// Only update .txt compile status if at least one file compiled
			if (numCompileOK > 0)
			{
				detector.Save();
			}

			string[] mgList = mgList1;
			foreach (string path in mgList)
			{
				string fileNameWithoutExtension3 = Path.GetFileNameWithoutExtension(path);
				if (!scriptNames.Contains(fileNameWithoutExtension3))
				{
					Debug.Log("Compiled script " + fileNameWithoutExtension3 + " has no matching script file. Removing...");
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

		public Texture2D LoadScreenshotByNameWithoutExtension(string filenameNoExt)
		{
			string path = MGHelper.GetSavePath((filenameNoExt + ".jpg").ToLower(), allowLegacyFallback: true);
			if(!File.Exists(path))
			{
				path = MGHelper.GetSavePath((filenameNoExt + ".png").ToLower(), allowLegacyFallback: true);
			}

			if (File.Exists(path))
			{
				try
				{
					byte[] array = File.ReadAllBytes(path);
					Texture2D texture2D = new Texture2D(ScreenshotWidth, ScreenshotHeight, TextureFormat.ARGB32, mipmap: false);
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

		public string getAssetFromCascade(string filenameAnyCase, PathCascadeList cascade, out bool exists)
		{
			// Assume that all files are lowercase on disk, but are sometimes not fully lowercase in the game script or as args to this function
			string filename = filenameAnyCase.ToLower();

			exists = false;

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

		public string _GetAudioFilePath(string filename, Audio.AudioType type, out bool exists, out bool flagValid)
		{
			switch (type)
			{
				case Audio.AudioType.BGM:
					{
						flagValid = MODAudioSet.Instance.GetBGMCascade(BurikoMemory.Instance.GetGlobalFlag("GAltBGM").IntValue(), out PathCascadeList cascade);
						return getAssetFromCascade(filename, cascade, out exists);
					}

				case Audio.AudioType.SE:
				case Audio.AudioType.System:
					{
						flagValid = MODAudioSet.Instance.GetSECascade(BurikoMemory.Instance.GetGlobalFlag("GAltSE").IntValue(), out PathCascadeList cascade);
						return getAssetFromCascade(filename, cascade, out exists);
					}

				case Audio.AudioType.Voice:
					{
						int voiceFlag = BurikoMemory.Instance.GetGlobalFlag("GAltVoicePriority").IntValue();
						if (BurikoMemory.Instance.GetGlobalFlag("GAltVoice").IntValue() == 0)
						{
							voiceFlag = 0;
						}
						flagValid = MODAudioSet.Instance.GetVoiceCascade(voiceFlag, out PathCascadeList cascade);
						return getAssetFromCascade(filename, cascade, out exists);
					}

				default:
					Debug.Log($"_GetAudioFilePath(): Cannot play '{filename}' due to unknown AudioType '{type}' - ignoring this file");
					exists = false;
					flagValid = true;
					return "";
			}
		}

		public string GetAudioFilePath(string filename, Audio.AudioType type)
		{
			string relativePath = _GetAudioFilePath(filename, type, out bool exists, out bool flagValid);
			string debugRelativePath = $"{relativePath} ({(exists ? "File exists" : "File does not exist!")}, {(flagValid ? "Flag Valid" : "Unknown Flag!")})";
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
