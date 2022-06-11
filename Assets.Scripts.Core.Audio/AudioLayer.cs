using Assets.Scripts.Core.AssetManagement;
using NVorbis;
using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Core.Audio
{
	internal class AudioLayer : MonoBehaviour
	{
		private AudioController audioController;

		private AudioSource audioSource;

		private AudioClip audioClip;

		private VorbisReader vorbisReader;

		private AudioFinishCallback finishCallback;

		private OnFinishLoad onFinishLoad;

		private bool isReady;

		private bool isLoop;

		private AudioType audioType;

		private float volume = 0.5f;

		private float subVolume = 1f;

		private long loopPoint;

		private byte[] bytes;

		private float[] chvolume = new float[2]
		{
			1f,
			1f
		};

		private Thread loadThread;

		private bool isLoading;

		private bool isLoaded;

		public int pages;

		public void OnAudioFilterRead(float[] data, int ch)
		{
			if (isReady)
			{
				int num = vorbisReader.ReadSamples(data, 0, data.Length);
				if (num != data.Length && vorbisReader.DecodedPosition >= vorbisReader.TotalSamples && isLoop)
				{
					vorbisReader.DecodedPosition = loopPoint;
					num += vorbisReader.ReadSamples(data, num, data.Length - num);
				}
				for (int i = 0; i < num; i++)
				{
					data[i] = data[i] * volume * subVolume * chvolume[i % 2];
				}
			}
		}

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
			if (loadThread != null && loadThread.IsAlive)
			{
				loadThread.Abort();
			}
			isReady = false;
			isLoading = false;
			isLoaded = false;
			if (audioSource != null)
			{
				audioSource.Stop();
			}
			if (vorbisReader != null)
			{
				vorbisReader.Dispose();
			}
			vorbisReader = null;
			bytes = null;
			iTween.Stop(base.gameObject);
		}

		public bool IsPlaying()
		{
			return vorbisReader != null;
		}

		public float GetRemainingPlayTime()
		{
			if (audioSource.loop)
			{
				return -1f;
			}
			long decodedPosition = vorbisReader.DecodedPosition;
			return (float)(vorbisReader.TotalSamples - decodedPosition) / (float)vorbisReader.SampleRate;
		}

		private IEnumerator WaitForLoad()
		{
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			IVorbisStreamStatus[] stats = vorbisReader.Stats;
			for (int i = 0; i < stats.Length; i++)
			{
				Debug.Log(stats[i].TotalPages);
			}
			isReady = true;
		}

		public void PlayAudio(string filename, AudioType type, float startvolume = 1f, bool loop = false)
		{
			if (IsPlaying())
			{
				StopAudio();
			}
			isLoading = true;
			isLoaded = false;
			loopPoint = 0L;
			loadThread = new Thread((ThreadStart)delegate
			{
				audioType = type;
				subVolume = startvolume;
				isLoop = loop;
				bytes = AssetManager.Instance.GetAudioFile(filename, type);
				isLoading = false;
				isLoaded = true;
			});
			loadThread.Start();
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
			if (bytes != null)
			{
				if (audioSource == null)
				{
					audioSource = base.gameObject.AddComponent<AudioSource>();
				}
				if (audioController == null)
				{
					audioController = AudioController.Instance;
				}
				vorbisReader = new VorbisReader(new MemoryStream(bytes), closeStreamOnDispose: true);
				if (audioClip == null)
				{
					audioClip = AudioClip.Create("Audio Clip", 1000, vorbisReader.Channels, vorbisReader.SampleRate, _3D: false, stream: true);
				}
				audioSource.clip = audioClip;
				audioSource.loop = isLoop;
				audioSource.priority = audioController.GetPriorityByType(audioType);
				volume = audioController.GetVolumeByType(audioType) * subVolume;
				audioSource.Play();
				isLoop = audioSource.loop;
				pages = vorbisReader.Stats[0].TotalPages;
				isReady = true;
				if (onFinishLoad != null)
				{
					onFinishLoad();
				}
				onFinishLoad = null;
			}
		}

		private void OnAudioEnd()
		{
			if (finishCallback != null)
			{
				finishCallback();
			}
			finishCallback = null;
			StopAudio();
		}

		private void Start()
		{
			audioController = AudioController.Instance;
		}

		private void Update()
		{
			if (!isLoading && isLoaded && !isReady)
			{
				OnFinishLoading();
			}
			if (!isReady)
			{
				return;
			}
			volume = audioController.GetVolumeByType(audioType);
			audioSource.volume = volume * subVolume;
			if (vorbisReader.DecodedPosition >= vorbisReader.TotalSamples)
			{
				if (audioSource.loop)
				{
					vorbisReader.DecodedPosition = loopPoint;
				}
				else
				{
					OnAudioEnd();
				}
			}
		}
	}
}
