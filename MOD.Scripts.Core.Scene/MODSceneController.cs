using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.Scene;
using UnityEngine;

namespace MOD.Scripts.Core.Scene
{
	public class MODSceneController
	{
		public static int MODLipSync_Character_Audio;

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

		public void MODLipSyncPrepare(int charnum, string expressionnum)
		{
			int layer = MODLipSync_Layer[charnum];
			string textureName = MODLipSync_Texture[charnum] + expressionnum;
			int x = MODLipSync_X[charnum];
			int y = MODLipSync_Y[charnum];
			int z = MODLipSync_Z[charnum];
			int priority = MODLipSync_Priority[charnum];
			int type = MODLipSync_Type[charnum];
			Texture2D tex2d = GameSystem.Instance.AssetManager.LoadTexture(textureName);
			GameSystem.Instance.SceneController.MODDrawBustshot(layer, textureName, tex2d, x, y, z, 0, 0, 0, /*move:*/ false, priority, type, 0f, /*isblocking:*/ false);
		}

		public Texture2D MODLipSyncPrepare_fix(int charnum, string expressionnum)
		{
			int num = MODLipSync_Layer[charnum];
			string textureName = MODLipSync_Texture[charnum] + expressionnum;
			return GameSystem.Instance.AssetManager.LoadTexture(textureName);
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
