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

		private readonly Dictionary<int, AudioLayerUnity> channelDictionary = new Dictionary<int, AudioLayerUnity>();

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
				AudioLayerUnity audioLayerUnity = gameObject.AddComponent<AudioLayerUnity>();
				gameObject.transform.parent = audioParent.transform;
				audioLayerUnity.Prepare(num);
				channelDictionary.Add(num, audioLayerUnity);
				num++;
			}
			for (int j = 0; j < 8; j++)
			{
				GameObject gameObject2 = new GameObject("Voice Channel " + j.ToString("D2"));
				AudioLayerUnity audioLayerUnity2 = gameObject2.AddComponent<AudioLayerUnity>();
				gameObject2.transform.parent = audioParent.transform;
				audioLayerUnity2.Prepare(num);
				channelDictionary.Add(num, audioLayerUnity2);
				num++;
			}
			for (int k = 0; k < 8; k++)
			{
				GameObject gameObject3 = new GameObject("SE Channel " + k.ToString("D2"));
				AudioLayerUnity audioLayerUnity3 = gameObject3.AddComponent<AudioLayerUnity>();
				gameObject3.transform.parent = audioParent.transform;
				audioLayerUnity3.Prepare(num);
				channelDictionary.Add(num, audioLayerUnity3);
				num++;
			}
			for (int l = 0; l < 2; l++)
			{
				GameObject gameObject4 = new GameObject("System Channel " + l.ToString("D2"));
				AudioLayerUnity audioLayerUnity4 = gameObject4.AddComponent<AudioLayerUnity>();
				gameObject4.transform.parent = audioParent.transform;
				audioLayerUnity4.Prepare(num);
				channelDictionary.Add(num, audioLayerUnity4);
				num++;
			}
			AudioConfiguration configuration = AudioSettings.GetConfiguration();
			configuration.sampleRate = 44100;
			AudioSettings.Reset(configuration);
		}

		public void SerializeCurrentAudio(MemoryStream ms)
		{
			using (BsonWriter jsonWriter = new BsonWriter(ms)
			{
				CloseOutput = false
			})
			{
				new JsonSerializer().Serialize(jsonWriter, currentAudio);
			}
		}

		public void DeSerializeCurrentAudio(MemoryStream ms)
		{
			StopAllAudio();
			using (BsonReader reader = new BsonReader(ms)
			{
				CloseInput = false
			})
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
					PlayAudio(audioInfo.Filename, AudioType.BGM, i, audioInfo.Volume);
				}
			}
			Dictionary<int, AudioInfo> dictionary2 = currentAudio[AudioType.Voice];
			for (int j = 0; j < 8; j++)
			{
				if (dictionary2.ContainsKey(j))
				{
					AudioInfo audioInfo2 = dictionary2[j];
					PlayAudio(audioInfo2.Filename, AudioType.Voice, j, audioInfo2.Volume);
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
				return;
			}
			StopAllAudio();
			RestoreTemp();
			SerializeTemp();
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
			return channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)].IsPlaying();
		}

		public void AddVoiceFinishCallback(int channel, AudioFinishCallback callback)
		{
			channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)].RegisterCallback(callback);
		}

		public float GetRemainingSEPlayTime(int channel)
		{
			return channelDictionary[GetChannelByTypeChannel(AudioType.SE, channel)].GetRemainingPlayTime();
		}

		public float GetRemainingVoicePlayTime(int channel)
		{
			return channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)].GetRemainingPlayTime();
		}

		public void ChangeVolumeOfBGM(int channel, float volume, float time)
		{
			int channelByTypeChannel = GetChannelByTypeChannel(AudioType.BGM, channel);
			float time2 = time / 1000f;
			AudioLayerUnity audioLayerUnity = channelDictionary[channelByTypeChannel];
			if (currentAudio[AudioType.BGM].ContainsKey(channel))
			{
				currentAudio[AudioType.BGM][channel].Volume = volume;
			}
			else
			{
				Debug.LogWarning("ChangeVolumeOfBGM could not find existing currentAudio for channel!");
			}
			audioLayerUnity.StartVolumeFade(volume, time2);
		}

		public void FadeOutBGM(int channel, int time, bool waitForFade)
		{
			float num = (float)time / 1000f;
			int channelByTypeChannel = GetChannelByTypeChannel(AudioType.BGM, channel);
			AudioLayerUnity audioLayerUnity = channelDictionary[channelByTypeChannel];
			audioLayerUnity.FadeOut(num);
			if (waitForFade)
			{
				GameSystem.Instance.AddWait(new Wait(num, WaitTypes.WaitForAudio, audioLayerUnity.StopAudio));
			}
			if (currentAudio[AudioType.BGM].ContainsKey(channel))
			{
				currentAudio[AudioType.BGM].Remove(channel);
			}
		}

		public void StopBGM(int channel)
		{
			channelDictionary[channel].StopAudio();
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
			AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.SE, channel)];
			if (audioLayerUnity.IsPlaying())
			{
				audioLayerUnity.StopAudio();
			}
			audioLayerUnity.PlayAudio(filename, AudioType.SE, volume);
		}

		public void StopSE(int channel)
		{
			channelDictionary[GetChannelByTypeChannel(AudioType.SE, channel)].StopAudio();
		}

		public void FadeOutSE(int channel, float time, bool waitForFade)
		{
			float num = time / 1000f;
			int channelByTypeChannel = GetChannelByTypeChannel(AudioType.SE, channel);
			AudioLayerUnity audioLayerUnity = channelDictionary[channelByTypeChannel];
			audioLayerUnity.FadeOut(num);
			if (waitForFade)
			{
				GameSystem.Instance.AddWait(new Wait(num, WaitTypes.WaitForAudio, audioLayerUnity.StopAudio));
			}
		}

		public void PlayVoice(string filename, int channel, float volume)
		{
			string text = filename.Substring(0, 4);
			if (0 == 0)
			{
				AudioLayerUnity audio = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
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
		}

		public void StopVoice(int channel)
		{
			channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)].StopAudio();
			if (currentAudio[AudioType.Voice].ContainsKey(channel))
			{
				currentAudio[AudioType.Voice].Remove(channel);
			}
		}

		public void PlaySystemSound(string filename, int channel = 0)
		{
			if (GameSystem.Instance.UseSystemSounds)
			{
				PlayAudio(filename, AudioType.System, channel, 0.7f);
			}
		}

		public void StopAllVoice()
		{
			for (int i = 0; i < 8; i++)
			{
				AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, i)];
				if (audioLayerUnity.IsPlaying())
				{
					audioLayerUnity.StopAudio();
				}
			}
		}

		public void StopAllAudio()
		{
			for (int i = 0; i < 6; i++)
			{
				AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.BGM, i)];
				if (audioLayerUnity.IsPlaying())
				{
					audioLayerUnity.StopAudio();
				}
			}
			for (int j = 0; j < 8; j++)
			{
				AudioLayerUnity audioLayerUnity2 = channelDictionary[GetChannelByTypeChannel(AudioType.SE, j)];
				if (audioLayerUnity2.IsPlaying())
				{
					audioLayerUnity2.StopAudio();
				}
			}
			for (int k = 0; k < 8; k++)
			{
				AudioLayerUnity audioLayerUnity3 = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, k)];
				if (audioLayerUnity3.IsPlaying())
				{
					audioLayerUnity3.StopAudio();
				}
			}
		}

		public void PlayAudio(string filename, AudioType type, int channel, float volume, float fadeintime = 0f)
		{
			float startvolume = volume;
			if (fadeintime > 0f)
			{
				fadeintime /= 1000f;
				startvolume = 0f;
			}
			AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(type, channel)];
			if (audioLayerUnity.IsPlaying() && currentAudio[AudioType.BGM].TryGetValue(channel, out AudioInfo value) && value.Filename == filename)
			{
				audioLayerUnity.SetCurrentVolume(volume);
				return;
			}
			if (audioLayerUnity.IsPlaying())
			{
				audioLayerUnity.StopAudio();
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
			audioLayerUnity.PlayAudio(filename, type, startvolume, loop);
			if (fadeintime > 0.05f)
			{
				audioLayerUnity.StartVolumeFade(volume, fadeintime);
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
				channelDictionary[GetChannelByTypeChannel(AudioType.BGM, i)].SetBaseVolume(BGMVolume * GlobalVolume);
			}
			for (int j = 0; j < 8; j++)
			{
				channelDictionary[GetChannelByTypeChannel(AudioType.Voice, j)].SetBaseVolume(VoiceVolume * GlobalVolume);
			}
			for (int k = 0; k < 8; k++)
			{
				channelDictionary[GetChannelByTypeChannel(AudioType.SE, k)].SetBaseVolume(SoundVolume * GlobalVolume);
			}
			for (int l = 0; l < 2; l++)
			{
				channelDictionary[GetChannelByTypeChannel(AudioType.System, l)].SetBaseVolume(SystemVolume * GlobalVolume);
			}
		}
	}
}
