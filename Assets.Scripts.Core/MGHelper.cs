using MOD.Scripts.Core;
using MOD.Scripts.UI;
using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core
{
	public static class MGHelper
	{
		public delegate bool InputHandler();

		private static readonly byte[] Key = new byte[9]
		{
			229,
			99,
			174,
			4,
			45,
			166,
			127,
			158,
			69
		};

		private static string _savepath = string.Empty;

		private static bool isD3d9;

		private static bool typeCheck;

		// All chapters have prefix 'mod-' in front of save files
		private const string MOD_SAVE_PREFIX = "mod-";

		public static void ClampValuesIfNecessary(Vector3[] vertices, Vector2[] uv, bool ryukishiClamp, int finalXOffset)
		{
			float scaleFromOrigin(float value, float scale)
			{
				return ((value - .5f) * scale) + .5f;
			}

			Vector2 scaleVector2FromOrigin(Vector2 vec, float xScale, float yScale)
			{
				return new Vector2(scaleFromOrigin(vec.x, xScale), scaleFromOrigin(vec.y, yScale));
			}


			if (ryukishiClamp)
			{
				// Ryukishi is nominally has x limited to [-320, 320]
				// but since the x transform happens afterwards, we need to adjust for that
				float minX = -320 - finalXOffset;
				float maxX = 320 - finalXOffset;

				for (int i = 0; i < 4; i++)
				{
					// Clamp the x of each vertex
					float newX = Mathf.Clamp(vertices[i].x, minX, maxX);
					float ratio = newX / vertices[i].x;
					vertices[i].x = newX;

					// Adjust UV X coordinates proportionally by how much we adjusted each vector
					uv[i].x = scaleFromOrigin(uv[i].x, ratio);
				}
			}
		}

		public static Mesh CreateMeshWithOrigin(int width, int height, Vector2 origin, bool ryukishiClamp, int finalXOffset)
		{
			Mesh mesh = new Mesh();
			float num = Mathf.Round((float)width / 2f);
			float num2 = Mathf.Round((float)height / 2f);
			origin.x -= num;
			origin.y -= num2;
			Vector3 vector = new Vector3(0f - num - origin.x, num2 + origin.y, 0f);
			Vector3 vector2 = new Vector3(num - origin.x, num2 + origin.y, 0f);
			Vector3 vector3 = new Vector3(num - origin.x, 0f - num2 + origin.y, 0f);
			Vector3 vector4 = new Vector3(0f - num - origin.x, 0f - num2 + origin.y, 0f);
			Vector3[] vertices = new Vector3[4]
			{
				// I'm not sure which one is actually top/bottom left/right, 
				// as it depends on where the camera is, and unity's coordinate system.
				vector2, // 426,  240
				vector3, // 426, -240
				vector,  //-426,  240
				vector4  //-426, -240
			};

			Vector2[] uv = new Vector2[4]
			{
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, 0f)
			};

			ClampValuesIfNecessary(vertices, uv, ryukishiClamp, finalXOffset);

			int[] triangles = new int[6]
			{
				0,
				1,
				2,
				2,
				1,
				3
			};
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			return mesh;
		}

		public static Mesh CreateMesh(int width, int height, LayerAlignment alignment, bool ryukishiClamp, int finalXOffset)
		{
			Mesh mesh = new Mesh();
			float num = Mathf.Round((float)width / 2f);
			float num2 = Mathf.Round((float)height / 2f);
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			Vector3 vector4;
			switch (alignment)
			{
			case LayerAlignment.AlignTopleft:
				vector = new Vector3(0f, 0f, 0f);
				vector2 = new Vector3((float)width, 0f, 0f);
				vector3 = new Vector3((float)width, (float)(-height), 0f);
				vector4 = new Vector3(0f, (float)(-height), 0f);
				break;
			case LayerAlignment.AlignBottomCenter:
				vector = new Vector3(0f - num, (float)height, 0f);
				vector2 = new Vector3(num, (float)height, 0f);
				vector3 = new Vector3(num, 0f, 0f);
				vector4 = new Vector3(0f - num, 0f, 0f);
				break;
			case LayerAlignment.AlignCenter:
				vector = new Vector3(0f - num, num2, 0f);
				vector2 = new Vector3(num, num2, 0f);
				vector3 = new Vector3(num, 0f - num2, 0f);
				vector4 = new Vector3(0f - num, 0f - num2, 0f);
				break;
			default:
				Logger.LogError("Could not CreateMesh, unexpected alignment " + alignment);
				return null;
			}
			Vector3[] vertices = new Vector3[4]
			{
				vector2,
				vector3,
				vector,
				vector4
			};
			Vector2[] uv = new Vector2[4]
			{
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, 0f)
			};

			ClampValuesIfNecessary(vertices, uv, ryukishiClamp, finalXOffset);

			int[] triangles = new int[6]
			{
				0,
				1,
				2,
				2,
				1,
				3
			};
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			return mesh;
		}

		public static void KeyEncode(byte[] b)
		{
			byte[] array = (byte[])Key.Clone();
			for (int i = 0; i < b.Length; i++)
			{
				b[i] = (byte)(b[i] ^ array[i % Key.Length]);
				array[i % array.Length] += 27;
			}
		}

		public static void WriteVector3(BinaryWriter br, Vector3 v)
		{
			br.Write(v.x);
			br.Write(v.y);
			br.Write(v.z);
		}

		public static Vector3 ReadVector3(BinaryReader br)
		{
			Vector3 result = default(Vector3);
			result.x = br.ReadSingle();
			result.y = br.ReadSingle();
			result.z = br.ReadSingle();
			return result;
		}

		public static void WriteColor(BinaryWriter br, Color c)
		{
			br.Write(c.r);
			br.Write(c.g);
			br.Write(c.b);
			br.Write(c.a);
		}

		public static Color ReadColor(BinaryReader br)
		{
			Color result = default(Color);
			result.r = br.ReadSingle();
			result.g = br.ReadSingle();
			result.b = br.ReadSingle();
			result.a = br.ReadSingle();
			return result;
		}

		// Upgrade old console arc saves to new format compatible with cloud saves
		//
		// For console arcs, we used to save to a subdirectory called "console", located inside
		// the normal Ch.4 save directory, to prevent interfering with regular Ch.4 saves or accidentally
		// reading Ch.4 normal or global save data into the Console arcs.
		//
		// However, Steam is not setup to recognize subdirectories, so Steam Cloud won't
		// synchronize anything in the subfolder
		//
		// The new save format saves everything into the normal Ch.4 save folder,
		// but with a prefix 'console-', to prevent interference with normal Ch.4 saves
		//
		// To upgrade users still on the old save format, This function copies out the files
		// in the 'console-' folder to the upper directory, prefixing them with 'console-'
		//
		// This function should be called exactly once every time the game runs, before
		// the global.dat or any saves are loaded.
		public static void UpgradeSavesIfNecessary()
		{
			string subdir = MODSystem.instance.modConfig.SaveSubdirectory;

			// exit if the "already upgraded" marker exists
			string markerPrefix = string.IsNullOrEmpty(subdir) ? "mod" : subdir;
			string upgradeMarkerPath = Path.Combine(Application.streamingAssetsPath, $"subdir-{markerPrefix}-upgrade-marker.txt");
			if (File.Exists(upgradeMarkerPath))
			{
				return;
			}

			string defaultSaveFolder = GetSaveFolder();

			// If a subdir was previously used, check that folder,
			// otherwise check the normal save folder for saves to migrate.
			string folderToProcess;
			if (string.IsNullOrEmpty(subdir))
			{
				/////////// For normal chapters, save files will be in the root of the save folder ///////////
				folderToProcess = defaultSaveFolder;

				string legacyGlobalDatPath = Path.Combine(GetSaveFolder(), "global.dat");

				// Exit if global.dat missing - no legacy modded saves to migrate
				if (!File.Exists(legacyGlobalDatPath))
				{
					return;
				}

				// Exit if global.dat is vanilla, as we don't want to migrate vanilla saves
				// If can't determine the type of global.dat, assume it is modded.
				GlobalSaveType saveType = MODSaves.GetGlobalSaveType(legacyGlobalDatPath);
				if (saveType == GlobalSaveType.Vanilla)
				{
					return;
				}
				else if(saveType == GlobalSaveType.Unknown)
				{
					Logger.LogWarning("WARNING: Global Save might be corrupted! Couldn't determine if it was modded or unmodded.");
				}
			}
			else
			{
				/////////// For console arcs, save files will be in the 'console' subfolder (subdir = 'console') ///////////
				// These files cannot be vanilla, since the vanilla game does not save in a subfolder.

				// exit if subdir folder doesn't exist
				string legacySaveFolder = Path.Combine(defaultSaveFolder, subdir);
				if (!Directory.Exists(legacySaveFolder))
				{
					return;
				}

				folderToProcess = legacySaveFolder;
			}

			// copy every file from subdir folder to save folder directory, prefixed with the name of the subdir
			// note that the below will never overwrite any existing files as we NEVER want to overwrite existing saves
			Debug.Log($"UpgradeSavesIfNecessary(): Beginning '{subdir}' upgrade");

			string failingFilename = "";
			int passCount = 0;
			int failCount = 0;
			foreach (string sourcePath in Directory.GetFiles(folderToProcess))
			{
				string filename = Path.GetFileName(sourcePath);

				// Don't process any saves which already have the prefix - they are not legacy saves.
				if(filename.StartsWith(MOD_SAVE_PREFIX))
				{
					continue;
				}

				string destPath = GetSavePath(filename);
				if(File.Exists(destPath))
				{
					Logger.Log($"UpgradeSavesIfNecessary(): Skipping {destPath} as it already exists...");
					continue;
				}

				Logger.Log($"UpgradeSavesIfNecessary(): Copying {sourcePath} to {destPath}...");
				try
				{
					// Copy the save to a temp file, then move the temp file into place
					// This should help make this process more atomic/prevent
					// errors with half-written save files
					string tempPath = Path.GetTempFileName();
					File.Copy(sourcePath, tempPath, true);
					File.Move(tempPath, destPath);
					File.Delete(tempPath);
					passCount++;
				}
				catch (Exception e)
				{
					failCount++;
					failingFilename = filename;
					Logger.Log($"UpgradeSavesIfNecessary(): Failed to copy from {sourcePath} to {destPath}: {e}");
				}
			}

			// ONLY if upgrade was successful, write the "already upgraded" marker.
			// If upgrade failed, will try again next time game launches
			// NOTE: cannot play sound with MODToaster because BurikoMemory isn't loaded at this point.
			if(failCount == 0)
			{
				File.WriteAllText(upgradeMarkerPath, $"{subdir} Subdirectory Upgrade Successful");
				MODToaster.Show($"Cloud Saves Fix: Successfully copied {passCount} files!", maybeSound: null, toastDuration: 10);
			}
			else
			{
				MODToaster.Show($"Cloud Saves Fix: {failCount} files failed like {failingFilename}!", maybeSound: null, toastDuration: 10);
			}
		}

		public static string GetSaveFolder()
		{
			string savePath;
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				savePath = Application.persistentDataPath;
			}
			else if (Application.platform == RuntimePlatform.LinuxPlayer)
			{
				savePath = Application.persistentDataPath;
			}
			else
			{
				if (_savepath == string.Empty)
				{
					string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					if (!Directory.Exists(text) || text == string.Empty)
					{
						text = Environment.ExpandEnvironmentVariables("%appdata%");
					}
					_savepath = Path.Combine(text, "Mangagamer\\higurashi04");
					Directory.CreateDirectory(_savepath);
				}
				savePath = _savepath;
			}
			Directory.CreateDirectory(savePath);
			return savePath;
		}

		private static string GetSavePrefix(string filename)
		{
			string prefix = MOD_SAVE_PREFIX;

			// Previously saves went to a subdirectory for Console chapters. Now
			// we amend the subdirectory name to the prefix, so that Steam
			// Cloud can synchronize the saves (Steam cloud won't look inside subdirectories)
			string subdir = MODSystem.instance.modConfig.SaveSubdirectory;
			if (!string.IsNullOrEmpty(subdir))
			{
				prefix += $"{subdir}-";
			}

			// If the filename already has the prefix, don't add it again
			// This is necessary because some of the code isn't aware of the prefix, and can call
			// GetSavePath on a filename which already has the prefix, which would add it twice without the below check
			if (filename.StartsWith(MOD_SAVE_PREFIX))
			{
				return "";
			}

			return MOD_SAVE_PREFIX;
		}

		public static string GetSavePath(string filename)
		{
			return Path.Combine(GetSaveFolder(), GetSavePrefix(filename) + filename);
		}

		public static string GetDataPath()
		{
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				return "Resources/GameData";
			}
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return "ExternalAssets/Archives";
			}
			return "GameData";
		}

		public static Vector3 GetReverseOffsetPosition(Vector3 pos)
		{
			return pos;
		}

		public static Vector3 GetOffsetPosition(Vector3 pos)
		{
			return pos;
		}

		public static int GetLoopPoint(string bgm)
		{
			string text = bgm;
			if (Path.HasExtension(bgm))
			{
				text = Path.GetFileNameWithoutExtension(bgm);
			}
			switch (text)
			{
			case "bgm_dd_011":
				return 1152298;
			case "bgm_dd_020":
				return 194763;
			case "bgm_dd_030":
				return 1637632;
			case "bgm_dd_040":
				return 782606;
			case "bgm_dd_050":
				return 287424;
			case "bgm_dd_060":
				return 1360950;
			case "bgm_dd_061":
				return 1988958;
			case "bgm_dd_070":
				return 673727;
			case "bgm_dd_080":
				return 314858;
			case "bgm_dd_081":
				return 126326;
			case "bgm_dd_090":
				return 1849827;
			case "bgm_dd_100":
				return 267264;
			case "bgm_dd_130":
				return 243138;
			case "bgm_dd_160":
				return 0;
			case "bgm_dd_170":
				return 0;
			case "bgm_dd_190":
				return 724;
			case "bgm_dd_200":
				return 1210327;
			case "bgm_kk_010":
				return 1615391;
			case "bgm_kk_020":
				return 2118652;
			case "bgm_kk_021":
				return 188346;
			case "bgm_kk_030":
				return 1442255;
			case "bgm_kk_040":
				return 644216;
			case "bgm_kk_050":
				return 4498436;
			case "bgm_kk_070":
				return 269311;
			case "bgm_kk_080":
				return 0;
			case "bgm_kk_090":
				return 175389;
			case "bgm_kk_091":
				return 104493;
			case "bgm_kk_110":
				return 151411;
			case "bgm_kk_120":
				return 555457;
			case "bgm_kk_130":
				return 251693;
			case "bgm_kk_140":
				return 826890;
			case "bgm_kk_210":
				return 3611530;
			case "bgm_kk_220":
				return 2618304;
			case "bgm_kk_240":
				return 4814598;
			case "bgm_kk_250":
				return 376703;
			case "bgm_kk_260":
				return 366437;
			case "bgm_kk_280":
				return 1144466;
			case "bgm_sys_dd_01":
				return 2429215;
			case "bgm_sys_dd_02":
				return 2508024;
			case "bgm_sys_dd_03":
				return 665967;
			case "bgm_sys_dd_03_2":
				return 1163094;
			case "bgm_sys_dd_04":
				return 3040630;
			case "bgm_sys_dd_05":
				return 787193;
			case "bgm_sys_dded_01":
				return 1177981;
			case "bgm_sys_dded_02":
				return 1225806;
			case "bgm_sys_dded_03":
				return 4014307;
			case "bgm_sys_dded_04":
				return 709553;
			case "bgm_sys_kk_01":
				return 149919;
			case "bgm_sys_kk_02":
				return 418378;
			case "bgm_sys_kk_03":
				return 4174863;
			case "bgm_sys_kk_04":
				return 655403;
			case "bgm_sys_kk_05":
				return 1819877;
			case "bgm_sys_kked_01":
				return 364179;
			case "bgm_sys_kked_02":
				return 2429792;
			case "bgm_sys_kked_03":
				return 1893150;
			case "bgm_sys_kked_04":
				return 508168;
			case "bgm_title":
				return 4905424;
			case "bsel0051":
				return 3053760;
			case "happycrossing":
				return 5844753;
			case "kankyo_25_d":
				return 0;
			case "se1011":
				return 0;
			case "se1015_d":
				return 0;
			case "se2020_d":
				return 0;
			case "se2110_d":
				return 0;
			case "se2111_d":
				return 0;
			case "sel0120_k":
				return 0;
			case "sel2040_d":
				return 0;
			case "sel2041":
				return 0;
			case "sel3010_d":
				return 0;
			case "sel_0120_d":
				return 0;
			case "sel_0220_d":
				return 0;
			case "sel_4050_d":
				return 0;
			case "sel_ashi_11_s":
				return 0;
			case "sel_dd_0010":
				return 0;
			case "sel_dd_0021":
				return 0;
			case "sel_dd_0030":
				return 0;
			case "sel_dd_0033":
				return 0;
			case "sel_dd_0035":
				return 0;
			case "sel_dd_0240":
				return 0;
			case "sel_dd_0245":
				return 0;
			case "sel_dd_1010":
				return 0;
			case "sel_dd_1021":
				return 0;
			case "sel_dd_2030":
				return 0;
			case "sel_dd_2700":
				return 0;
			case "sel_dvd_0080":
				return 0;
			case "sel_dvd_0100_02":
				return 0;
			case "sel_dvd_0110":
				return 0;
			case "sel_dvd_0120":
				return 0;
			case "sel_dvd_0160":
				return 0;
			case "sel_dvd_0170":
				return 0;
			case "sel_dvd_0180":
				return 0;
			case "sel_dvd_0190":
				return 0;
			case "sel_dvd_0250":
				return 0;
			case "sel_kk_0090":
				return 0;
			case "sel_ot_21_0010":
				return 0;
			case "sel_ot_21_0030":
				return 0;
			case "sel_ot_26_0010":
				return 0;
			case "selp_dd_0050":
				return 88692;
			case "selp_dd_0060":
				return 417357;
			case "selp_dd_0070":
				return 71684;
			default:
				return 0;
			}
		}
	}
}
