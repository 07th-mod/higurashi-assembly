using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using System;
using System.Collections;
using UnityEngine;

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

		private long loopPoint;

		private byte[] bytes;

		private float[] chvolume = new float[2]
		{
		1f,
		1f
		};

		private bool isLoading;

		private bool isLoaded;

		private Coroutine loadCoroutine;

		public int pages;

		public void Prepare(int newid)
		{
			audioController = AudioController.Instance;
		}

		public void RegisterCallback(AudioFinishCallback callback)
		{
			finishCallback = (AudioFinishCallback)Delegate.Combine(finishCallback, callback);
		}

		public void SetBaseVolume(float val)
		{
			volume = val;
		}

		public void SetCurrentVolume(float val)
		{
			subVolume = val;
		}

		public void StartVolumeFade(float end, float time)
		{
			iTween.Stop(base.gameObject);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", subVolume, "to", end, "time", time, "onupdate", "SetCurrentVolume", "oncomplete", "SetCurrentVolume", "oncompleteparams", end));
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
				Destroy(audioClip);
			}
			loadedName = string.Empty;
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

		public float GetRemainingPlayTime()
		{
			if (audioSource.loop)
			{
				return -1f;
			}
			return audioSource.clip.length - audioSource.time;
		}

		private IEnumerator WaitForLoad(string filename, AudioType type)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();
			string path = AssetManager.Instance.GetAudioFilePath(filename, type);
			WWW audioLoader = new WWW("file://" + path);
			yield return audioLoader;
			loadedName = filename;
			audioClip = audioLoader.audioClip;
			isLoading = false;
			isLoaded = true;
			loadCoroutine = null;
			watch.Stop();
			MODUtility.FlagMonitorOnlyLog("Loaded audio " + filename + " in " + watch.ElapsedMilliseconds + " ms");
			OnFinishLoading();
		}

		public void PlayAudio(string filename, AudioType type, float startvolume = 1f, bool loop = false)
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
			loadCoroutine = StartCoroutine(WaitForLoad(filename, type));
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
			audioSource.clip = audioClip;
			audioSource.loop = isLoop;
			audioSource.priority = audioController.GetPriorityByType(audioType);
			volume = audioController.GetVolumeByType(audioType) * subVolume;
			audioSource.Play();
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
