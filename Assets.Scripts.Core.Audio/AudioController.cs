using MOD.Scripts.Core;
using MOD.Scripts.Core.TextWindow;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core.Audio
{
	public class AudioController
	{
		private const int BGMLayers = 6;

		private const int VoiceLayers = 8;

		private const int SoundLayers = 8;

		private const int SystemLayers = 2;

		private static AudioController _instance;

		public float GlobalVolume = 0.5f;

		public float BGMVolume = 1f;

		public float VoiceVolume = 1f;

		public float SoundVolume = 1f;

		public float SystemVolume = 1f;

		private readonly Dictionary<int, AudioLayer> channelDictionary = new Dictionary<int, AudioLayer>();

		private Dictionary<AudioType, Dictionary<int, AudioInfo>> currentAudio = new Dictionary<AudioType, Dictionary<int, AudioInfo>>();

		private GameObject audioParent;

		private byte[] tempSavedAudio;

		public static AudioController Instance => _instance ?? (_instance = GameSystem.Instance.AudioController);

		public AudioController()
		{
			GameSystem.Instance.AudioController = this;
			currentAudio.Add(AudioType.BGM, new Dictionary<int, AudioInfo>());
			currentAudio.Add(AudioType.Voice, new Dictionary<int, AudioInfo>());
			audioParent = new GameObject("AudioLayers");
			int num = 0;
			for (int i = 0; i < 6; i++)
			{
				GameObject gameObject = new GameObject("BGM Channel " + i.ToString("D2"));
				AudioLayer audioLayer = gameObject.AddComponent<AudioLayer>();
				gameObject.transform.parent = audioParent.transform;
				audioLayer.Prepare(num);
				channelDictionary.Add(num, audioLayer);
				num++;
			}
			for (int j = 0; j < 8; j++)
			{
				GameObject gameObject2 = new GameObject("Voice Channel " + j.ToString("D2"));
				AudioLayer audioLayer2 = gameObject2.AddComponent<AudioLayer>();
				gameObject2.transform.parent = audioParent.transform;
				audioLayer2.Prepare(num);
				channelDictionary.Add(num, audioLayer2);
				num++;
			}
			for (int k = 0; k < 8; k++)
			{
				GameObject gameObject3 = new GameObject("SE Channel " + k.ToString("D2"));
				AudioLayer audioLayer3 = gameObject3.AddComponent<AudioLayer>();
				gameObject3.transform.parent = audioParent.transform;
				audioLayer3.Prepare(num);
				channelDictionary.Add(num, audioLayer3);
				num++;
			}
			for (int l = 0; l < 2; l++)
			{
				GameObject gameObject4 = new GameObject("System Channel " + l.ToString("D2"));
				AudioLayer audioLayer4 = gameObject4.AddComponent<AudioLayer>();
				gameObject4.transform.parent = audioParent.transform;
				audioLayer4.Prepare(num);
				channelDictionary.Add(num, audioLayer4);
				num++;
			}
			AudioConfiguration configuration = AudioSettings.GetConfiguration();
			configuration.sampleRate = 44100;
			AudioSettings.Reset(configuration);
		}

		public void SerializeCurrentAudio(MemoryStream ms)
		{
			BsonWriter bsonWriter = new BsonWriter(ms);
			bsonWriter.CloseOutput = false;
			using (BsonWriter jsonWriter = bsonWriter)
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				jsonSerializer.Serialize(jsonWriter, currentAudio);
			}
		}

		public void DeSerializeCurrentAudio(MemoryStream ms)
		{
			StopAllAudio();
			BsonReader bsonReader = new BsonReader(ms);
			bsonReader.CloseInput = false;
			using (BsonReader reader = bsonReader)
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				currentAudio = jsonSerializer.Deserialize<Dictionary<AudioType, Dictionary<int, AudioInfo>>>(reader);
			}
			Dictionary<int, AudioInfo> dictionary = currentAudio[AudioType.BGM];
			for (int i = 0; i < 6; i++)
			{
				if (dictionary.ContainsKey(i))
				{
					AudioInfo audioInfo = dictionary[i];
					PlayAudio(audioInfo.Filename, AudioType.BGM, i, audioInfo.Volume, 0f);
				}
			}
			Dictionary<int, AudioInfo> dictionary2 = currentAudio[AudioType.Voice];
			for (int j = 0; j < 8; j++)
			{
				if (dictionary2.ContainsKey(j))
				{
					AudioInfo audioInfo2 = dictionary2[j];
					PlayAudio(audioInfo2.Filename, AudioType.Voice, j, audioInfo2.Volume, 0f);
				}
			}
			tempSavedAudio = null;
		}

		private void SerializeTemp()
		{
			MemoryStream memoryStream = new MemoryStream();
			SerializeCurrentAudio(memoryStream);
			tempSavedAudio = memoryStream.ToArray();
		}

		private void RestoreTemp()
		{
			if (tempSavedAudio != null)
			{
				MemoryStream ms = new MemoryStream(tempSavedAudio);
				DeSerializeCurrentAudio(ms);
			}
		}

		public void SaveRestoreTempAudio()
		{
			if (tempSavedAudio == null)
			{
				SerializeTemp();
			}
			else
			{
				StopAllAudio();
				RestoreTemp();
				SerializeTemp();
			}
		}

		public void ClearTempAudio()
		{
			tempSavedAudio = null;
		}

		public AudioInfo GetVoiceQueue(int channel)
		{
			if (currentAudio[AudioType.Voice].ContainsKey(channel))
			{
				return currentAudio[AudioType.Voice][channel];
			}
			return null;
		}

		public void ClearVoiceQueue()
		{
			currentAudio[AudioType.Voice].Clear();
		}

		public bool IsVoicePlaying(int channel)
		{
			AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			return audioLayer.IsPlaying();
		}

		public void AddVoiceFinishCallback(int channel, AudioFinishCallback callback)
		{
			AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			audioLayer.RegisterCallback(callback);
		}

		public float GetRemainingSEPlayTime(int channel)
		{
			AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.SE, channel)];
			return audioLayer.GetRemainingPlayTime();
		}

		public float GetRemainingVoicePlayTime(int channel)
		{
			AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			return audioLayer.GetRemainingPlayTime();
		}

		public void ChangeVolumeOfBGM(int channel, float volume, float time)
		{
			int channelByTypeChannel = GetChannelByTypeChannel(AudioType.BGM, channel);
			float time2 = time / 1000f;
			AudioLayer audioLayer = channelDictionary[channelByTypeChannel];
			if (currentAudio[AudioType.BGM].ContainsKey(channel))
			{
				currentAudio[AudioType.BGM][channel].Volume = volume;
			}
			else
			{
				Debug.LogWarning("ChangeVolumeOfBGM could not find existing currentAudio for channel!");
			}
			audioLayer.StartVolumeFade(volume, time2);
		}

		public void FadeOutBGM(int channel, int time, bool waitForFade)
		{
			float num = (float)time / 1000f;
			int channelByTypeChannel = GetChannelByTypeChannel(AudioType.BGM, channel);
			AudioLayer audioLayer = channelDictionary[channelByTypeChannel];
			audioLayer.FadeOut(num);
			if (waitForFade)
			{
				GameSystem.Instance.AddWait(new Wait(num, WaitTypes.WaitForAudio, audioLayer.StopAudio));
			}
			if (currentAudio[AudioType.BGM].ContainsKey(channel))
			{
				currentAudio[AudioType.BGM].Remove(channel);
			}
		}

		public void StopBGM(int channel)
		{
			AudioLayer audioLayer = channelDictionary[channel];
			audioLayer.StopAudio();
			if (currentAudio[AudioType.BGM].ContainsKey(channel))
			{
				currentAudio[AudioType.BGM].Remove(channel);
			}
		}

		public void FadeOutMultiBGM(int channelstart, int channelend, int time, bool waitForFade)
		{
			for (int i = channelstart; i <= channelend; i++)
			{
				FadeOutBGM(i, time, waitForFade);
			}
		}

		public void PlaySE(string filename, int channel, float volume, float pan)
		{
			AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.SE, channel)];
			if (audioLayer.IsPlaying())
			{
				audioLayer.StopAudio();
			}
			audioLayer.PlayAudio(filename, AudioType.SE, volume);
		}

		public void StopSE(int channel)
		{
			AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.SE, channel)];
			audioLayer.StopAudio();
		}

		public void FadeOutSE(int channel, float time, bool waitForFade)
		{
			float num = time / 1000f;
			int channelByTypeChannel = GetChannelByTypeChannel(AudioType.SE, channel);
			AudioLayer audioLayer = channelDictionary[channelByTypeChannel];
			audioLayer.FadeOut(num);
			if (waitForFade)
			{
				GameSystem.Instance.AddWait(new Wait(num, WaitTypes.WaitForAudio, audioLayer.StopAudio));
			}
		}

		public void PlayVoice(string filename, int channel, float volume)
		{
			MODTextController.MODCurrentVoiceLayerDetect = channel;
			AudioLayer audio = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			if (currentAudio[AudioType.Voice].ContainsKey(channel))
			{
				currentAudio[AudioType.Voice].Remove(channel);
			}
			currentAudio[AudioType.Voice].Add(channel, new AudioInfo(volume, filename));
			if (audio.IsPlaying())
			{
				audio.StopAudio();
			}
			audio.PlayAudio(filename, AudioType.Voice, volume);
			if (GameSystem.Instance.IsAuto)
			{
				audio.OnLoadCallback(delegate
				{
					GameSystem.Instance.AddWait(new Wait(audio.GetRemainingPlayTime(), WaitTypes.WaitForVoice, null));
				});
			}
		}

		public void StopVoice(int channel)
		{
			AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			audioLayer.StopAudio();
			if (currentAudio[AudioType.Voice].ContainsKey(channel))
			{
				currentAudio[AudioType.Voice].Remove(channel);
			}
		}

		public void PlaySystemSound(string filename, int channel = 0)
		{
			if (GameSystem.Instance.UseSystemSounds)
			{
				PlayAudio(filename, AudioType.System, channel, 0.7f, 0f);
			}
		}

		public void StopAllVoice()
		{
			for (int i = 0; i < 8; i++)
			{
				AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, i)];
				if (audioLayer.IsPlaying())
				{
					audioLayer.StopAudio();
				}
			}
		}

		public void StopAllAudio()
		{
			for (int i = 0; i < 6; i++)
			{
				AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.BGM, i)];
				if (audioLayer.IsPlaying())
				{
					audioLayer.StopAudio();
				}
			}
			for (int j = 0; j < 8; j++)
			{
				AudioLayer audioLayer2 = channelDictionary[GetChannelByTypeChannel(AudioType.SE, j)];
				if (audioLayer2.IsPlaying())
				{
					audioLayer2.StopAudio();
				}
			}
			for (int k = 0; k < 8; k++)
			{
				AudioLayer audioLayer3 = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, k)];
				if (audioLayer3.IsPlaying())
				{
					audioLayer3.StopAudio();
				}
			}
		}

		public void PlayAudio(string filename, AudioType type, int channel, float volume, float fadeintime = 0)
		{
			float startvolume = volume;
			if (fadeintime > 0f)
			{
				fadeintime /= 1000f;
				startvolume = 0f;
			}
			AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(type, channel)];
			if (audioLayer.IsPlaying())
			{
				audioLayer.StopAudio();
			}
			bool loop = type == AudioType.BGM;
			if (type == AudioType.BGM)
			{
				if (currentAudio[AudioType.BGM].ContainsKey(channel))
				{
					currentAudio[AudioType.BGM].Remove(channel);
				}
				currentAudio[AudioType.BGM].Add(channel, new AudioInfo(volume, filename));
			}
			audioLayer.PlayAudio(filename, type, startvolume, loop);
			if (fadeintime > 0.05f)
			{
				audioLayer.StartVolumeFade(volume, fadeintime);
			}
		}

		private int GetChannelByTypeChannel(AudioType type, int ch)
		{
			switch (type)
			{
			case AudioType.BGM:
				return ch;
			case AudioType.Voice:
				return ch + 6;
			case AudioType.SE:
				return ch + 6 + 8;
			case AudioType.System:
				return ch + 6 + 8 + 8;
			default:
				throw new InvalidEnumArgumentException("GetChannelByTypeChannel: Invalid audiotype " + type);
			}
		}

		public float GetVolumeByType(AudioType type)
		{
			switch (type)
			{
			case AudioType.BGM:
				return GlobalVolume * BGMVolume;
			case AudioType.Voice:
				return GlobalVolume * VoiceVolume;
			case AudioType.SE:
				return GlobalVolume * SoundVolume;
			case AudioType.System:
				return GlobalVolume * SystemVolume;
			default:
				Logger.LogWarning("GetVolumeByType called with unidentified audio type: " + type);
				return 0f;
			}
		}

		public int GetPriorityByType(AudioType audioType)
		{
			switch (audioType)
			{
			case AudioType.BGM:
				return 128;
			case AudioType.Voice:
				return 64;
			case AudioType.SE:
				return 32;
			case AudioType.System:
				return 16;
			default:
				Logger.LogWarning("GetPriorityByType called with unidentified audio type: " + audioType);
				return 1;
			}
		}

		public void RefreshLayerVolumes()
		{
			for (int i = 0; i < 6; i++)
			{
				AudioLayer audioLayer = channelDictionary[GetChannelByTypeChannel(AudioType.BGM, i)];
				audioLayer.SetBaseVolume(BGMVolume * GlobalVolume);
			}
			for (int j = 0; j < 8; j++)
			{
				AudioLayer audioLayer2 = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, j)];
				audioLayer2.SetBaseVolume(VoiceVolume * GlobalVolume);
			}
			for (int k = 0; k < 8; k++)
			{
				AudioLayer audioLayer3 = channelDictionary[GetChannelByTypeChannel(AudioType.SE, k)];
				audioLayer3.SetBaseVolume(SoundVolume * GlobalVolume);
			}
			for (int l = 0; l < 2; l++)
			{
				AudioLayer audioLayer4 = channelDictionary[GetChannelByTypeChannel(AudioType.System, l)];
				audioLayer4.SetBaseVolume(SystemVolume * GlobalVolume);
			}
		}

		public bool IsSEPlaying(int channel)
		{
			return channelDictionary[GetChannelByTypeChannel(AudioType.SE, channel)].IsPlaying();
		}

		public void MODOnlyRecompile()
		{
		}

		public void MODPlayVoiceLS(string filename, int channel, float volume, int character)
		{
			MODTextController.MODCurrentVoiceLayerDetect = channel;
			AudioLayer audio = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			if (currentAudio[AudioType.Voice].ContainsKey(channel))
			{
				currentAudio[AudioType.Voice].Remove(channel);
			}
			currentAudio[AudioType.Voice].Add(channel, new AudioInfo(volume, filename));
			if (audio.IsPlaying())
			{
				audio.StopAudio();
			}
			audio.PlayAudio(filename, AudioType.Voice, volume);
			if (MODSystem.instance.modSceneController.MODLipSyncBoolCheck(character))
			{
				GameSystem.Instance.SceneController.MODLipSyncStart(character, channel, filename);
			}
			if (GameSystem.Instance.IsAuto)
			{
				audio.OnLoadCallback(delegate
				{
					GameSystem.Instance.AddWait(new Wait(audio.GetRemainingPlayTime(), WaitTypes.WaitForVoice, null));
				});
			}
		}
	}
}
