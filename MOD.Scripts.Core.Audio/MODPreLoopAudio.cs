using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MOD.Scripts.Core.Audio
{
	public class LoopPlaybackInformation
	{
		public float loopStartDelay;
		public float loopFirstStartTime;

		public LoopPlaybackInformation(float loopStartDelay, float loopInitialTime)
		{
			this.loopStartDelay = loopStartDelay;
			this.loopFirstStartTime = loopInitialTime;
		}
	}

	public class MODPreLoopAudio
	{
		List<AudioClip> AudioClips = new List<AudioClip>();
		List<AudioSource> AudioSources = new List<AudioSource>();
		bool LastTrackOverlaps = true;
		LoopPlaybackInformation playbackInformation = null;

		public IEnumerator LoadAudioClips(string filename, Assets.Scripts.Core.Audio.AudioType type)
		{
			for (int i = 0; i < 10; i++)
			{
				string preLoopFilename = Path.GetFileNameWithoutExtension(filename) + $"_{i}" + Path.GetExtension(filename);
				string preLoopPath = AssetManager.Instance.GetAudioFilePath(preLoopFilename, type);

				bool fileExists = false;

				if (File.Exists(preLoopPath))
				{
					fileExists = true;
				}
				else
				{
					// If file not found, try checking same path suffixed with 'f' - this means that the audio
					// file is a 'full audio' and should not overlap with the main audio loop
					preLoopFilename = Path.GetFileNameWithoutExtension(filename) + $"_{i}f" + Path.GetExtension(filename);
					preLoopPath = AssetManager.Instance.GetAudioFilePath(preLoopFilename, type);

					if (File.Exists(preLoopPath))
					{
						LastTrackOverlaps = false;
						fileExists = true;
					}
				}

				if (fileExists)
				{
					WWW audioLoaderIntro = new WWW("file://" + preLoopPath);
					yield return audioLoaderIntro;
					AudioClips.Add(audioLoaderIntro.audioClip);
				}
				else
				{
					break;
				}
			}
		}

		public LoopPlaybackInformation OnFinishLoad(GameObject gameObjectToAttach, int priority)
		{
			// Probably unnecessary, but there was a guard here in the original code to prevent creating extra AudioSource objects unecessarily
			if (playbackInformation != null)
			{
				return playbackInformation;
			}

			// First, generate an audio source for each audio clip
			foreach (AudioClip audioClip in AudioClips)
			{
				AudioSource audioSource = gameObjectToAttach.AddComponent<AudioSource>();

				audioSource.clip = audioClip;
				audioSource.loop = false;
				audioSource.priority = priority;

				AudioSources.Add(audioSource);
			}

			// Then, determine when each audio should start playing
			float overlapAmount = 0;
			float playHead = 0;
			foreach(AudioSource audioSource in AudioSources)
			{
				// Handle overlap by moving playhead back slightly (but never less than 0)
				playHead = Math.Max(0, playHead - overlapAmount);

				audioSource.PlayDelayed(playHead);

				// Increment playhead equal to length of played track
				playHead += audioSource.clip.length;
			}

			playbackInformation = new LoopPlaybackInformation(playHead, GetLoopInitialStartTime());
			return playbackInformation;
		}

		private float GetLoopInitialStartTime()
		{
			// Return the length of the last audio, if the last audio track overlaps the main loop
			if(LastTrackOverlaps && AudioClips.Count > 0)
			{
				return AudioClips[AudioClips.Count - 1].length;
			}

			// Otherwise, the main loop plays from the start
			return 0;
		}

		public void StopAudio()
		{
			foreach (AudioSource audioSource in AudioSources)
			{
				audioSource.Stop();
			}

			foreach(AudioClip audioClip in AudioClips)
			{
				UnityEngine.Object.Destroy(audioClip);
			}
		}

		public bool IsPlaying()
		{
			foreach(AudioSource audioSource in AudioSources)
			{
				if(audioSource.isPlaying)
				{
					return true;
				}
			}

			return false;
		}

		public float GetRemainingPlayTime()
		{
			float remainingPlayTime = 0;

			foreach (AudioSource audioSource in AudioSources)
			{
				if(audioSource.time != audioSource.clip.length)
				{
					remainingPlayTime = audioSource.clip.length - audioSource.time;
				}
			}

			return remainingPlayTime;
		}

		public int GetPlayTimeSamples()
		{
			int playTimeSamples = 0;

			foreach (AudioSource audioSource in AudioSources)
			{
				playTimeSamples += audioSource.timeSamples;
			}

			return playTimeSamples;
		}

		// To be called from AudioLayerUnity.cs's Update() function
		public void Update(float volume)
		{
			foreach (AudioSource audioSource in AudioSources)
			{
				audioSource.volume = volume;
			}
		}
	}
}
