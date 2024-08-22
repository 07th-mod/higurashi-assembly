#if STANDALONE_SCRIPT_COMPILER

using System;
using System.IO;
using BGICompiler.Compiler.Logger;

namespace BGICompiler.Compiler
{
	internal class NoUnityScriptCompiler
	{
		private static bool SaveCompileStatus(int numTotal, int numPass, int numFail)
		{
			bool allCompiledOK = numPass == numTotal;
			bool atLeastOneCompiled = numPass != 0;
			bool pass = allCompiledOK && atLeastOneCompiled;

			// Also consider compilation a failure if no scripts were compiled
			string statusString = pass ? "Compile OK" : "FAIL";
			statusString += $" | {numPass}/{numTotal} compiled and {numFail} failed";
			File.WriteAllText("higu_script_compile_status.txt", statusString);

			return pass;
		}

		private static bool CompileFolder(string srcDir, string destDir)
		{
			string[] txtPaths = Directory.GetFiles(srcDir, "*.txt");
			int numTotal = txtPaths.Length;

			int numPass = 0;
			int numFail = 0;
			int progress = 0;
			foreach (string txtPath in txtPaths)
			{
				progress++;

				try
				{
					string txtPathNoExt = Path.GetFileNameWithoutExtension(txtPath);
					string mgPath = Path.Combine(destDir, txtPathNoExt) + ".mg";
					Debug.Log($"Compiling [{progress}/{numTotal}] {txtPath} -> {mgPath}...");
					new BGItoMG(txtPath, mgPath);
					numPass++;
				}
				catch (Exception e)
				{
					Debug.LogWarning($"Failed to compile script {txtPath}!\r\n{e}");
					numFail++;
				}
			}

			return SaveCompileStatus(numTotal, numPass, numFail);
		}

		public static int Main(string[] args)
		{
			if(args.Length < 2)
			{
				Debug.LogError($"Got {args.Length} args but need at least two args:\n" +
					$" 1. path to the 'Update' (.txt) folder\n" +
					$" 2. path to the 'CompiledUpdateScripts' folder (.mg)\n" +
					$"Example: [HigurashiScriptCompiler Update CompiledUpdateScripts] when called from inside the StreamingAssets folder.");
				return -1;
			}

			Debug.Log($"Got {args.Length} args");

			string txtFolderPath = args[0];
			string mgFolderPath = args[1];

			Debug.Log($"Compiling Scripts In {txtFolderPath} -> {mgFolderPath}");

			if(!Directory.Exists(txtFolderPath))
			{
				Debug.LogError($"Source .txt script folder does not exist at [{txtFolderPath}]");
				return -1;
			}

			if (!Directory.Exists(mgFolderPath))
			{
				Debug.LogError($"Destination .mg script folder does not exist at [{mgFolderPath}]");
				return -1;
			}

			return CompileFolder(txtFolderPath, mgFolderPath) ? 0 : -1;
		}
	}
}

#endif
