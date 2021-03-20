using MOD.Scripts.Core;
using MOD.Scripts.Core.Audio;
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
			AudioSettings.outputSampleRate = 44100;
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
			AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			return audioLayerUnity.IsPlaying();
		}

		public void AddVoiceFinishCallback(int channel, AudioFinishCallback callback)
		{
			AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			audioLayerUnity.RegisterCallback(callback);
		}

		public float GetRemainingSEPlayTime(int channel)
		{
			AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.SE, channel)];
			return audioLayerUnity.GetRemainingPlayTime();
		}

		public float GetRemainingVoicePlayTime(int channel)
		{
			AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			return audioLayerUnity.GetRemainingPlayTime();
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

		public void FadeOutBGM(int channel, int time, bool waitForFade, bool noBGMTracking = false)
		{
			if (!noBGMTracking)
			{
				MODAudioTracking.Instance.ForgetLastBGM(channel);
			}
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

		public void StopBGM(int channel, bool noBGMTracking = false)
		{
			if (!noBGMTracking)
			{
				MODAudioTracking.Instance.ForgetLastBGM(channel);
			}
			AudioLayerUnity audioLayerUnity = channelDictionary[channel];
			audioLayerUnity.StopAudio();
			if (currentAudio[AudioType.BGM].ContainsKey(channel))
			{
				currentAudio[AudioType.BGM].Remove(channel);
			}
		}

		public void FadeOutMultiBGM(int channelstart, int channelend, int time, bool waitForFade)
		{
			for (int i = channelstart; i <= channelend; i++)
			{
				MODAudioTracking.Instance.ForgetLastBGM(i);
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
			AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.SE, channel)];
			audioLayerUnity.StopAudio();
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
			AudioLayerUnity audio = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			MODTextController.MODCurrentVoiceLayerDetect = channel;
			if (currentAudio[AudioType.Voice].ContainsKey(channel))
			{
				currentAudio[AudioType.Voice].Remove(channel);
			}
			currentAudio[AudioType.Voice].Add(channel, new AudioInfo(volume, filename, channel));
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

		private void PlayVoices(List<List<AudioInfo>> voices, int index)
		{
			if (index >= voices.Count)
			{
				return;
			}
			List<AudioInfo> voiceSet = voices[index];
			var doneCount = 0;
			foreach (AudioInfo voice in voiceSet)
			{
				AudioLayerUnity audio = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, voice.Channel)];
				MODTextController.MODCurrentVoiceLayerDetect = voice.Channel;
				if (currentAudio[AudioType.Voice].ContainsKey(voice.Channel))
				{
					currentAudio[AudioType.Voice].Remove(voice.Channel);
				}
				currentAudio[AudioType.Voice].Add(voice.Channel, voice);
				if (audio.IsPlaying())
				{
					audio.StopAudio();
				}
				audio.PlayAudio(voice.Filename, AudioType.Voice, voice.Volume);
				audio.RegisterCallback(delegate
				{
					doneCount += 1;
					if (doneCount == voiceSet.Count)
					{
						PlayVoices(voices, index + 1);
					}
				});
			}
		}

		/// <summary>
		/// Plays multiple voices in parallel/series
		/// </summary>
		/// <param name="voices">The voices to play.  The outer array is played in series while inner arrays are played in parallel</param>
		public void PlayVoices(List<List<AudioInfo>> voices)
		{
			PlayVoices(voices, 0);
		}

		public void StopVoice(int channel)
		{
			AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			audioLayerUnity.StopAudio();
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
				MODAudioTracking.Instance.ForgetLastBGM(i);
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

		public void PlayAudio(string filename, AudioType type, int channel, float volume, float fadeintime = 0f, bool noBGMTracking = false)
		{
			float startvolume = volume;
			if (fadeintime > 0f)
			{
				fadeintime /= 1000f;
				startvolume = 0f;
			}
			AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(type, channel)];
			if (audioLayerUnity.IsPlaying())
			{
				audioLayerUnity.StopAudio();
			}
			bool loop = type == AudioType.BGM;
			if (type == AudioType.BGM)
			{
				if (!noBGMTracking)
				{
					MODAudioTracking.Instance.SaveLastBGM(new AudioInfo(volume, filename, channel));
				}

				if (currentAudio[AudioType.BGM].ContainsKey(channel))
				{
					currentAudio[AudioType.BGM].Remove(channel);
				}
				currentAudio[AudioType.BGM].Add(channel, new AudioInfo(volume, filename, channel));
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
				AudioLayerUnity audioLayerUnity = channelDictionary[GetChannelByTypeChannel(AudioType.BGM, i)];
				audioLayerUnity.SetBaseVolume(BGMVolume * GlobalVolume);
			}
			for (int j = 0; j < 8; j++)
			{
				AudioLayerUnity audioLayerUnity2 = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, j)];
				audioLayerUnity2.SetBaseVolume(VoiceVolume * GlobalVolume);
			}
			for (int k = 0; k < 8; k++)
			{
				AudioLayerUnity audioLayerUnity3 = channelDictionary[GetChannelByTypeChannel(AudioType.SE, k)];
				audioLayerUnity3.SetBaseVolume(SoundVolume * GlobalVolume);
			}
			for (int l = 0; l < 2; l++)
			{
				AudioLayerUnity audioLayerUnity4 = channelDictionary[GetChannelByTypeChannel(AudioType.System, l)];
				audioLayerUnity4.SetBaseVolume(SystemVolume * GlobalVolume);
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
			AudioLayerUnity audio = channelDictionary[GetChannelByTypeChannel(AudioType.Voice, channel)];
			if (currentAudio[AudioType.Voice].ContainsKey(channel))
			{
				currentAudio[AudioType.Voice].Remove(channel);
			}
			currentAudio[AudioType.Voice].Add(channel, new AudioInfo(volume, filename, channel));
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
