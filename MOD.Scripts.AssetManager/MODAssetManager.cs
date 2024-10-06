#define USE_DATE

using MOD.Scripts.Core.MODXMLWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using MOD.Scripts.Core.UnityLoggerShim;

public class MODCompileRequiredDetector
{
	public class ScriptInfo
	{
		public string name { get; set; }
		#if USE_DATE
		public DateTime lastWriteTime { get; set; }
		#endif
		public long length { get; set; }
		public string md5String { get; set; }

		public static ScriptInfo TryGetOrNull(string path)
		{
			try
			{
				string md5String;
				using (var md5 = MD5.Create())
				{
					using (var stream = File.OpenRead(path))
					{
						md5String = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
					}
				}

				FileInfo fileInfo = new FileInfo(path);

				return new ScriptInfo
				{
					name = Path.GetFileNameWithoutExtension(fileInfo.Name),
					#if USE_DATE
					lastWriteTime = fileInfo.LastWriteTime,
					#endif
					length = fileInfo.Length,
					md5String = md5String,
				};
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to get script info for {path}: {e}");
			}

			return null;
		}
	}

	readonly Dictionary<string, ScriptInfo> oldTxtInfoDictionary;
	readonly Dictionary<string, ScriptInfo> newTxtInfoDictionary;
	readonly List<string> successfullyCompiledScriptNameList;
	readonly string txtInfoDictionaryPath;

	public MODCompileRequiredDetector(string destDir)
	{
		oldTxtInfoDictionary = new Dictionary<string, ScriptInfo>();
		newTxtInfoDictionary = new Dictionary<string, ScriptInfo>();
		successfullyCompiledScriptNameList = new List<string>();
		txtInfoDictionaryPath = Path.Combine(destDir, "txtInfoDictionary.xml");
	}

	public void Load()
	{
		try
		{
			if (File.Exists(txtInfoDictionaryPath))
			{
				List<ScriptInfo> scriptInfoList = MODXMLWrapper.Deserialize<List<ScriptInfo>>(txtInfoDictionaryPath);
				foreach (ScriptInfo info in scriptInfoList)
				{
					oldTxtInfoDictionary[info.name] = info;
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError($"CompileFolder(): Failed to deserialize {txtInfoDictionaryPath}: {e}");
		}
	}

	public void Save()
	{
		List<ScriptInfo> txtInfoToSave = new List<ScriptInfo>();

		foreach(string scriptName in successfullyCompiledScriptNameList)
		{
			if(newTxtInfoDictionary.TryGetValue(scriptName, out ScriptInfo value))
			{
				txtInfoToSave.Add(value);
			}
			else
			{
				Debug.LogError($"MODCompileRequiredDetector:{scriptName} was marked as compiled but no ScriptInfo was recorded for it!");
			}
		}

		// save scriptInfoDictionary to file as at least one .txt file has changed and succesfully been compiled
		MODXMLWrapper.Serialize(txtInfoDictionaryPath, txtInfoToSave);
	}

	// Save to memory that a .txt file has been compiled or has already been compiled in the past
	// NOTE: this information is only saved to file when Save() is called!
	public void MarkScriptCompiled(string scriptNameNoExtension)
	{
		successfullyCompiledScriptNameList.Add(scriptNameNoExtension);
	}

	public bool SaveScriptInfoAndCheckScriptNeedsCompile(string txtPath, string mgPath, bool saveOnly=false)
	{
		// These variables are only used for printing to logs
		string textDescription = Path.GetFileName(txtPath);
		string mgDescription = Path.GetFileName(mgPath);

		ScriptInfo txt = ScriptInfo.TryGetOrNull(txtPath);
		ScriptInfo mg = File.Exists(mgPath) ? ScriptInfo.TryGetOrNull(mgPath) : null;
		newTxtInfoDictionary[txt.name] = txt;

		if(saveOnly)
		{
			return true;
		}

		// If the mg file doensn't exist or can't be accessed, do re-compile
		if (mg == null)
		{
			Debug.Log($"ScriptNeedsCompile(): Compiling {textDescription} as mg file {mgDescription} does not exist or can't be accessed");
			return true;
		}

		// This implies the txt file doesn't exist, which should never happen but just try to recompile anyway in this case
		if (txt == null)
		{
			Debug.Log($"ScriptNeedsCompile(): WARNING: {textDescription} can't be accessed or does not exist - trying to compile anyway");
			return true;
		}

		#if USE_DATE
		// Compile if the .txt is newer than the .mg file
		if (txt.lastWriteTime > mg.lastWriteTime)
		{
			Debug.Log($"ScriptNeedsCompile(): Compiling {textDescription} as it is newer than mg file {mgDescription} (txt: {txt.lastWriteTime}), mg: {mg.lastWriteTime}");
			return true;
		}
		#endif

		// Compare the current .txt file against the last recorded information about the .txt file
		if (oldTxtInfoDictionary.TryGetValue(txt.name, out ScriptInfo oldInfo))
		{
			// Compile if the file size has changed since last time
			if (oldInfo.length != txt.length)
			{
				Debug.Log($"ScriptNeedsCompile(): Compiling {textDescription} as size differs from previous (old: {oldInfo.length} bytes, new: {txt.length} bytes)");
				return true;
			}

			// Compile if the file's MD5 has changed
			if (oldInfo.md5String != txt.md5String)
			{
				Debug.Log($"ScriptNeedsCompile(): Compiling {textDescription} as MD5 differs from previous (old: {oldInfo.md5String}, new: {txt.md5String})");
				return true;
			}
		}
		else
		{
			// If the file hasn't been recorded in the dictionary, re-compile it
			Debug.Log($"ScriptNeedsCompile(): Compiling {textDescription} as it doesn't exist in the .txt info dictionary.");
			return true;
		}

		return false;
	}
}