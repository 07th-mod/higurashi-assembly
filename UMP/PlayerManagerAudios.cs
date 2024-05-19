using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMP
{
	internal class PlayerManagerAudios
	{
		private AudioOutput[] _audioOutputs;

		public AudioOutput[] AudioOutputs => _audioOutputs;

		public AudioSource[] AudioSources
		{
			get
			{
				List<AudioSource> list = new List<AudioSource>();
				if (_audioOutputs != null && IsValid)
				{
					AudioOutput[] audioOutputs = _audioOutputs;
					foreach (AudioOutput audioOutput in audioOutputs)
					{
						list.Add(audioOutput.AudioSource);
					}
				}
				return list.ToArray();
			}
		}

		public bool IsValid
		{
			get
			{
				if (_audioOutputs != null)
				{
					AudioOutput[] audioOutputs = _audioOutputs;
					foreach (AudioOutput audioOutput in audioOutputs)
					{
						if (audioOutput == null || audioOutput.AudioSource == null)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public bool OutputsDataUpdated
		{
			get
			{
				if (_audioOutputs != null && IsValid)
				{
					AudioOutput[] audioOutputs = _audioOutputs;
					foreach (AudioOutput audioOutput in audioOutputs)
					{
						if (audioOutput.Data == null)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public PlayerManagerAudios(AudioOutput[] audioOutputs)
		{
			_audioOutputs = audioOutputs;
			if (_audioOutputs != null && IsValid)
			{
				AudioOutput[] audioOutputs2 = _audioOutputs;
				foreach (AudioOutput audioOutput in audioOutputs2)
				{
					audioOutput.Init();
				}
			}
		}

		public void AddListener(Action<int, float[], AudioOutput.AudioChannels> listener)
		{
			if (_audioOutputs != null && IsValid)
			{
				AudioOutput[] audioOutputs = _audioOutputs;
				foreach (AudioOutput audioOutput in audioOutputs)
				{
					audioOutput.AudioFilterReadListener += listener;
				}
			}
		}

		public void RemoveAllListeners()
		{
			if (_audioOutputs != null && IsValid)
			{
				AudioOutput[] audioOutputs = _audioOutputs;
				foreach (AudioOutput audioOutput in audioOutputs)
				{
					audioOutput.RemoveAllListeners();
				}
			}
		}

		public bool SetOutputData(int id, float[] data)
		{
			if (_audioOutputs != null && IsValid)
			{
				AudioOutput[] audioOutputs = _audioOutputs;
				foreach (AudioOutput audioOutput in audioOutputs)
				{
					if (audioOutput.Id == id)
					{
						audioOutput.Data = data;
						return true;
					}
				}
			}
			return false;
		}

		public void ResetOutputsData()
		{
			if (_audioOutputs != null && IsValid)
			{
				AudioOutput[] audioOutputs = _audioOutputs;
				foreach (AudioOutput audioOutput in audioOutputs)
				{
					audioOutput.Data = null;
				}
			}
		}

		public void Play()
		{
			if (_audioOutputs != null && IsValid)
			{
				AudioOutput[] audioOutputs = _audioOutputs;
				foreach (AudioOutput audioOutput in audioOutputs)
				{
					audioOutput.Play();
				}
			}
		}

		public void Pause()
		{
			if (_audioOutputs != null && IsValid)
			{
				AudioOutput[] audioOutputs = _audioOutputs;
				foreach (AudioOutput audioOutput in audioOutputs)
				{
					audioOutput.Pause();
				}
			}
		}

		public void Stop()
		{
			if (_audioOutputs != null && IsValid)
			{
				AudioOutput[] audioOutputs = _audioOutputs;
				foreach (AudioOutput audioOutput in audioOutputs)
				{
					audioOutput.Stop();
				}
			}
		}
	}
}
