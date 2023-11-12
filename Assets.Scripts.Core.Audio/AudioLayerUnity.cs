using Assets.Scripts.Core.AssetManagement;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Core.Audio
{
	internal class AudioLayerUnity : MonoBehaviour
	{
		private AudioController audioController;

		private AudioSource audioSource;

		private AudioClip audioClip;

		private AudioFinishCallback finishCallback;

		private OnFinishLoad onFinishLoad;

		private bool isReady;

		private bool isLoop;

		private AudioType audioType;

		private float volume = 0.5f;

		private float subVolume = 1f;

		private string loadedName;

		private byte[] bytes;

		private float[] chvolume = new float[2]
		{
			1f,
			1f
		};

		private bool isLoading;

		private bool isLoaded;

		private Coroutine loadCoroutine;

		private string trackName;

		public int pages;

		public bool hasCallback => finishCallback != null;

		public void Prepare(int newid)
		{
			audioController = AudioController.Instance;
		}

		public void RegisterCallback(AudioFinishCallback callback)
		{
			Debug.Log("RegisterAudioCallback");
			finishCallback = callback;
		}

		public void SetBaseVolume(float val)
		{
			volume = val;
		}

		public void SetCurrentVolume(float val)
		{
			subVolume = val;
		}

		public void FinishChangingVolume(float val)
		{
			iTween.Stop(base.gameObject);
			subVolume = val;
		}

		public void ChangeLoopingStatus(bool loop)
		{
			isLoop = loop;
			audioSource.loop = loop;
		}

		public void StartVolumeFade(float end, float time)
		{
			iTween.Stop(base.gameObject);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", subVolume, "to", end, "time", time, "onupdate", "SetCurrentVolume", "oncomplete", "FinishChangingVolume", "oncompleteparams", end));
		}

		public void FadeOut(float time)
		{
			iTween.Stop(base.gameObject);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", subVolume, "to", 0, "time", time, "onupdate", "SetCurrentVolume", "oncomplete", "StopAudio"));
		}

		public void UpdatePan(float pan)
		{
		}

		public void StopAudio()
		{
			if (loadCoroutine != null)
			{
				StopCoroutine(loadCoroutine);
			}
			loadCoroutine = null;
			isReady = false;
			isLoading = false;
			isLoaded = false;
			if (audioSource != null)
			{
				audioSource.Stop();
			}
			if (audioClip != null)
			{
				AudioClip ac = audioClip;
				GameSystem.Instance.RegisterAction(delegate
				{
					UnityEngine.Object.Destroy(ac);
				});
			}
			loadedName = "";
			audioClip = null;
			iTween.Stop(base.gameObject);
			finishCallback = null;
		}

		public bool IsPlaying()
		{
			if (audioSource == null)
			{
				return false;
			}
			if (audioClip == null)
			{
				return false;
			}
			return audioSource.isPlaying;
		}

		public string GetTrackTime()
		{
			if (!audioSource.isPlaying)
			{
				return "";
			}
			string str = TimeSpan.FromSeconds(audioSource.time).ToString("m\\:ss");
			string str2 = TimeSpan.FromSeconds(audioClip.length).ToString("m\\:ss");
			return str + " / " + str2;
		}

		public float GetRemainingPlayTime()
		{
			if (audioSource.loop)
			{
				return -1f;
			}
			return audioSource.clip.length - audioSource.time;
		}

		public int GetPlayTimeSamples() => audioSource.timeSamples;

		private IEnumerator WaitForLoad(string filename, AudioType type, Action<string, AudioType, AudioClip> onAudioDataLoaded = null)
		{
			string text = AssetManager.Instance.GetAudioFilePath(filename, type);
			bool flag = File.Exists(text);
			if (!flag && type == AudioType.BGM)
			{
				string audioFilePath = AssetManager.Instance.GetAudioFilePath(filename, AudioType.SE);
				if (File.Exists(audioFilePath))
				{
					text = audioFilePath;
					flag = true;
				}
				if (!flag && filename.StartsWith("se/"))
				{
					audioFilePath = AssetManager.Instance.GetAudioFilePath(filename.Substring(3), AudioType.SE);
					if (File.Exists(audioFilePath))
					{
						text = audioFilePath;
						flag = true;
					}
				}
			}
			if (!flag)
			{
				Debug.Log("Audio file does not exist: " + text);
				yield break;
			}
			string s = text.Replace("\\", "/");
			trackName = filename;
			string uri2 = UnityWebRequest.EscapeURL(s);
			uri2 = ((Application.platform != RuntimePlatform.OSXPlayer && Application.platform != RuntimePlatform.LinuxPlayer) ? ("file:///" + uri2) : ("file://" + uri2));
			UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(uri2, UnityEngine.AudioType.OGGVORBIS);
			yield return req.SendWebRequest();
			loadedName = filename;
			if (req.isNetworkError || req.isHttpError)
			{
				Debug.LogError("Could not load audio clip \"" + uri2 + "\", error: " + req.error);
			}
			audioClip = DownloadHandlerAudioClip.GetContent(req);
			while (audioClip.loadState != AudioDataLoadState.Loaded)
			{
				yield return null;
			}

			if(audioClip != null && onAudioDataLoaded != null)
			{
				onAudioDataLoaded(filename, type, audioClip);
			}

			isLoading = false;
			isLoaded = true;
			loadCoroutine = null;
			OnFinishLoading();
		}

		public void PlayAudio(string filename, AudioType type, float startvolume = 1f, bool loop = false, Action<string, AudioType, AudioClip> onAudioLoaded = null)
		{
			if (IsPlaying())
			{
				StopAudio();
			}
			isReady = false;
			isLoading = true;
			isLoaded = false;
			if (loadCoroutine != null)
			{
				StopCoroutine(loadCoroutine);
			}
			audioType = type;
			subVolume = startvolume;
			isLoop = loop;
			finishCallback = null;
			loadCoroutine = StartCoroutine(WaitForLoad(filename, type, onAudioLoaded));
		}

		public void OnLoadCallback(OnFinishLoad callback)
		{
			if (isReady)
			{
				callback();
			}
			else
			{
				onFinishLoad = (OnFinishLoad)Delegate.Combine(onFinishLoad, callback);
			}
		}

		private void OnFinishLoading()
		{
			if (audioSource == null)
			{
				audioSource = base.gameObject.AddComponent<AudioSource>();
			}
			if (audioController == null)
			{
				audioController = AudioController.Instance;
			}
			audioClip.name = trackName;
			audioSource.clip = audioClip;
			audioSource.loop = isLoop;
			audioSource.bypassEffects = true;
			audioSource.bypassListenerEffects = true;
			audioSource.bypassReverbZones = true;
			audioSource.spatialBlend = 0f;
			audioSource.priority = audioController.GetPriorityByType(audioType);
			volume = audioController.GetVolumeByType(audioType) * subVolume;
			audioSource.PlayDelayed(0.0001f);
			isReady = true;
			if (onFinishLoad != null)
			{
				onFinishLoad();
			}
			onFinishLoad = null;
		}

		private void OnAudioEnd()
		{
			AudioFinishCallback callback = finishCallback;
			StopAudio();
			if (callback != null)
			{
				callback();
			}
		}

		private void Start()
		{
			audioController = AudioController.Instance;
		}

		private void Update()
		{
			if (isReady && !(audioSource == null) && !(audioClip == null))
			{
				if (audioController == null)
				{
					audioController = AudioController.Instance;
				}
				volume = audioController.GetVolumeByType(audioType);
				audioSource.volume = volume * subVolume;
				if (!audioSource.isPlaying && !isLoop && !isLoading && isReady && isLoaded)
				{
					OnAudioEnd();
				}
			}
		}
	}
}
