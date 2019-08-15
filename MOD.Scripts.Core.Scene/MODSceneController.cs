using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.Scene;
using UnityEngine;

namespace MOD.Scripts.Core.Scene
{
	public class MODSceneController
	{
		public static int MODLipSync_Character_Audio;

		public struct Filter
		{
			public static readonly Filter Identity = new Filter(256, 0, 0, 0, 256, 0, 0, 0, 256, 256);
			public static readonly Filter Flashback = new Filter(136, 60, 60, 69, 115, 69, 84, 84, 88, 256); // 77 47 4 preserving luminosity

			public int rr;
			public int rg;
			public int rb;
			public int gr;
			public int gg;
			public int gb;
			public int br;
			public int bg;
			public int bb;
			public int a;

			/// <summary>All colors should be in the range 0...256</summary>
			public Filter(int rr, int rg, int rb, int gr, int gg, int gb, int br, int bg, int bb, int a)
			{
				this.rr = rr;
				this.rg = rg;
				this.rb = rb;
				this.gr = gr;
				this.gg = gg;
				this.gb = gb;
				this.br = br;
				this.bg = bg;
				this.bb = bb;
				this.a = a;
			}

			/// <summary>
			/// Gets values from a short array
			/// </summary>
			public Filter(short[] arr)
			{
				rr = arr[0];
				rg = arr[1];
				rb = arr[2];
				gr = arr[3];
				gg = arr[4];
				gb = arr[5];
				br = arr[6];
				bg = arr[7];
				bb = arr[8];
				a = arr[9];
			}

			public bool MixesColors => rg != 0 || rb != 0 || gr != 0 || gb != 0 || br != 0 || bg != 0;
			public bool ChangesColors => MixesColors || rr != 256 || gg != 256 || bb != 256;
			public bool ChangesAlpha => a != 256;
			public bool IsIdentity => !ChangesAlpha && !ChangesColors;

			private static float F(int color)
			{
				return color / 256f;
			}

			public short[] AsShortArray => new short[]
			{
				(short)rr,
				(short)rg,
				(short)rb,
				(short)gr,
				(short)gg,
				(short)gb,
				(short)br,
				(short)bg,
				(short)bb,
				(short)a
			};

			public Color AsColorMultiplier => new Color(F(rr), F(gg), F(bb), F(a));
			public Matrix4x4 AsMatrix4x4 => new Matrix4x4
			{
				m00 = F(rr),
				m01 = F(rg),
				m02 = F(rb),
				m10 = F(gr),
				m11 = F(gg),
				m12 = F(gb),
				m20 = F(br),
				m21 = F(bg),
				m22 = F(bb),
				m33 = F(a)
			};
		}

		private static Dictionary<int, Filter> layerFilters = new Dictionary<int, Filter>();

		public static void ClearLayerFilters() { layerFilters.Clear(); }

		public static Dictionary<int, short[]> serializableLayerFilters
		{
			get => layerFilters.ToDictionary(x => x.Key, x => x.Value.AsShortArray);
			set => layerFilters = value.ToDictionary(x => x.Key, x => new Filter(x.Value));
		}

		private static bool[] MODLipSync_Bool;

		private static int[] MODLipSync_Layer;

		private static string[] MODLipSync_Texture;

		private static int[] MODLipSync_X;

		private static int[] MODLipSync_Y;

		private static int[] MODLipSync_Z;

		private static int[] MODLipSync_Priority;

		private static int[] MODLipSync_Type;

		public static int[] MODLipSync_Channel;

		public static ulong[] MODLipSync_CoroutineId;

		public static void SetLayerFilter(int layer, Filter filter)
		{
			if (filter.IsIdentity)
			{
				layerFilters.Remove(layer);
			}
			else
			{
				layerFilters[layer] = filter;
			}
		}

		public static bool TryGetLayerFilter(int layer, out Filter value)
		{
			return layerFilters.TryGetValue(layer, out value);
		}

		public static void ApplyFilters(int layer, Texture2D texture)
		{
			if (!TryGetLayerFilter(layer, out Filter value)) { return; }
			var watch = System.Diagnostics.Stopwatch.StartNew();
			System.Diagnostics.Stopwatch innerWatch;

			string filterString;
			if (value.MixesColors)
			{
				var pixels = texture.GetPixels();
				Matrix4x4 filter = value.AsMatrix4x4;
				filterString = "<Matrix4x4 Filter>";
				innerWatch = System.Diagnostics.Stopwatch.StartNew();
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = filter * pixels[i];
				}
				innerWatch.Stop();
				texture.SetPixels(pixels);
			}
			else if (value.ChangesColors)
			{
				var pixels = texture.GetPixels();
				Color filter = value.AsColorMultiplier;
				filterString = filter.ToString();
				innerWatch = System.Diagnostics.Stopwatch.StartNew();
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] *= filter;
				}
				innerWatch.Stop();
				texture.SetPixels(pixels);
			}
			else
			{
				var pixels = texture.GetPixels32();
				filterString = "Alpha " + value.a;
				innerWatch = System.Diagnostics.Stopwatch.StartNew();
				for (int i = 0; i < pixels.Length; i++)
				{
					Color32 pixel = pixels[i];
					pixel.a = (byte)((pixel.a * value.a) >> 8);
					pixels[i] = pixel;
				}
				innerWatch.Stop();
				texture.SetPixels32(pixels);
			}
			watch.Stop();
			texture.Apply();
			MODUtility.FlagMonitorOnlyLog("Applied filter " + filterString + " to " + texture.name + " in " + watch.ElapsedMilliseconds + "ms, filter took " + innerWatch.ElapsedMilliseconds + "ms");
		}

		public static Texture2D LoadTextureWithFilters(int? layer, string textureName)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();
			Texture2D texture = GameSystem.Instance.AssetManager.LoadTexture(textureName);
			watch.Stop();
			MODUtility.FlagMonitorOnlyLog("Loaded " + textureName + " in " + watch.ElapsedMilliseconds + "ms");
			if (layer is int actualLayer) { ApplyFilters(actualLayer, texture); }
			return texture;
		}

		public void MODLipSyncDisableAll()
		{
			for (int i = 0; i < MODLipSync_Bool.Length; i++)
			{
				MODLipSync_Bool[i] = false;
			}
		}

		public void MODLipSyncDisableCurrentLayer(int layer)
		{
			for (int i = 0; i < MODLipSync_Layer.Length; i++)
			{
				if (MODLipSync_Layer[i] == layer)
				{
					MODLipSync_Bool[i] = false;
				}
			}
		}

		public void MODLipSyncStoreValue(int layer, int character, string texture, int x, int y, int z, int type, int priority)
		{
			MODLipSync_Bool[character] = true;
			MODLipSync_Layer[character] = layer;
			MODLipSync_Texture[character] = texture;
			MODLipSync_X[character] = x;
			MODLipSync_Y[character] = y;
			MODLipSync_Z[character] = z;
			MODLipSync_Type[character] = type;
			MODLipSync_Priority[character] = priority;
		}

		public bool MODLipSyncBoolCheck(int character)
		{
			string value = MODLipSync_Texture[character];
			Layer ifInUse = GameSystem.Instance.SceneController.GetIfInUse(MODLipSync_Layer[character]);
			string text = (ifInUse == null) ? null : ifInUse.PrimaryName;
			if (MODLipSync_Bool[character] && !string.IsNullOrEmpty(value))
			{
				return (text ?? "").Contains(value);
			}
			return false;
		}

		public void MODLipSyncInitializeAll()
		{
			MODLipSync_Character_Audio = 0;
			MODLipSync_Bool = new bool[50];
			MODLipSync_Layer = new int[50];
			MODLipSync_Texture = new string[50];
			MODLipSync_X = new int[50];
			MODLipSync_Y = new int[50];
			MODLipSync_Z = new int[50];
			MODLipSync_Priority = new int[50];
			MODLipSync_Channel = new int[50];
			MODLipSync_CoroutineId = new ulong[50];
		}

		public Texture2D MODLipSyncPrepare(int charnum, string expressionnum)
		{
			int num = MODLipSync_Layer[charnum];
			string textureName = MODLipSync_Texture[charnum] + expressionnum;
			return LoadTextureWithFilters(num, textureName);
		}

		static MODSceneController()
		{
			MODLipSync_Bool = new bool[50];
			MODLipSync_Layer = new int[50];
			MODLipSync_Texture = new string[50];
			MODLipSync_X = new int[50];
			MODLipSync_Y = new int[50];
			MODLipSync_Z = new int[50];
			MODLipSync_Priority = new int[50];
			MODLipSync_Type = new int[50];
			MODLipSync_Channel = new int[50];
			MODLipSync_CoroutineId = new ulong[50];
		}

		private void MODLipSyncStoreAudioChannel(int character, int channel)
		{
			MODLipSync_Channel[character] = channel;
		}

		private void MODLipSyncDisableOthersOnChannel(int character, int channel)
		{
			for (int i = 0; i < MODLipSync_Bool.Length; i++)
			{
				if (i != character && MODLipSync_Channel[i] == channel)
				{
					MODLipSync_Bool[i] = false;
				}
			}
		}

		public void MODLipSyncPrepareVoice(int character, int channel)
		{
			MODLipSyncStoreAudioChannel(character, channel);
			MODLipSyncDisableOthersOnChannel(character, channel);
			MODLipSync_Bool[character] = true;
		}

		public bool MODLipSyncIsEnabled()
		{
			if (BurikoMemory.Instance.GetGlobalFlag("GArtStyle").IntValue() == 0)
			{
				return BurikoMemory.Instance.GetGlobalFlag("GLipSync").IntValue() == 1;
			}
			return false;
		}

		public ulong MODLipSyncInvalidateAndGenerateId(int character)
		{
			return ++MODLipSync_CoroutineId[character];
		}

		public bool MODLipSyncAnimationStillActive(int character, ulong coroutineId)
		{
			if (MODLipSyncBoolCheck(character))
			{
				return MODLipSyncIsAnimationCurrent(character, coroutineId);
			}
			return false;
		}

		public bool MODLipSyncIsAnimationCurrent(int character, ulong coroutineId)
		{
			return coroutineId == MODLipSync_CoroutineId[character];
		}

		public void MODLipSyncProcess(int charnum, string expressionnum, Texture2D tex2d, ulong coroutineId)
		{
			if (MODLipSyncIsAnimationCurrent(charnum, coroutineId))
			{
				int layer = MODLipSync_Layer[charnum];
				string textureName = MODLipSync_Texture[charnum] + expressionnum;
				int x = MODLipSync_X[charnum];
				int y = MODLipSync_Y[charnum];
				int z = MODLipSync_Z[charnum];
				int priority = MODLipSync_Priority[charnum];
				int type = MODLipSync_Type[charnum];
				GameSystem.Instance.SceneController.MODDrawBustshot(layer, textureName, tex2d, x, y, z, 0, 0, 0, /*move:*/ false, priority, type, 0f, /*isblocking:*/ false);
			}
		}
	}
}
