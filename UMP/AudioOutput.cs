using System;
using UnityEngine;

namespace UMP
{
	[RequireComponent(typeof(AudioSource))]
	public abstract class AudioOutput : MonoBehaviour
	{
		public enum AudioChannels
		{
			Both,
			Left,
			Right
		}

		[SerializeField]
		private AudioChannels _audioChannel;

		private int _id;

		private AudioSource _audioSource;

		private float[] _data;

		public AudioChannels AudioChannel
		{
			get
			{
				return _audioChannel;
			}
			set
			{
				_audioChannel = value;
			}
		}

		public int Id => _id;

		public AudioSource AudioSource
		{
			get
			{
				if (_audioSource == null)
				{
					_audioSource = GetComponent<AudioSource>();
				}
				return _audioSource;
			}
		}

		internal float[] Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
				if (this._outputDataListener != null && _data != null)
				{
					this._outputDataListener(_data, _audioChannel);
				}
			}
		}

		private event Action<float[], AudioChannels> _outputDataListener;

		public event Action<float[], AudioChannels> OutputDataListener
		{
			add
			{
				this._outputDataListener = (Action<float[], AudioChannels>)Delegate.Combine(this._outputDataListener, value);
			}
			remove
			{
				if (this._outputDataListener != null)
				{
					this._outputDataListener = (Action<float[], AudioChannels>)Delegate.Remove(this._outputDataListener, value);
				}
			}
		}

		private event Action<int, float[], AudioChannels> _audioFilterReadListener;

		internal event Action<int, float[], AudioChannels> AudioFilterReadListener
		{
			add
			{
				this._audioFilterReadListener = (Action<int, float[], AudioChannels>)Delegate.Combine(this._audioFilterReadListener, value);
			}
			remove
			{
				if (this._audioFilterReadListener != null)
				{
					this._audioFilterReadListener = (Action<int, float[], AudioChannels>)Delegate.Remove(this._audioFilterReadListener, value);
				}
			}
		}

		internal void Init()
		{
			_id = GetInstanceID();
		}

		public void Play()
		{
			if (_audioSource == null)
			{
				_audioSource = GetComponent<AudioSource>();
			}
			_audioSource.Play();
		}

		public void Pause()
		{
			if (_audioSource == null)
			{
				_audioSource = GetComponent<AudioSource>();
			}
			_audioSource.Pause();
		}

		public void Stop()
		{
			if (_audioSource == null)
			{
				_audioSource = GetComponent<AudioSource>();
			}
			_audioSource.Stop();
		}

		public void RemoveAllListeners()
		{
			if (this._audioFilterReadListener != null)
			{
				Delegate[] invocationList = this._audioFilterReadListener.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Action<int, float[], AudioChannels> value = (Action<int, float[], AudioChannels>)invocationList[i];
					this._audioFilterReadListener = (Action<int, float[], AudioChannels>)Delegate.Remove(this._audioFilterReadListener, value);
				}
			}
			if (this._outputDataListener != null)
			{
				Delegate[] invocationList2 = this._outputDataListener.GetInvocationList();
				for (int j = 0; j < invocationList2.Length; j++)
				{
					Action<float[], AudioChannels> value2 = (Action<float[], AudioChannels>)invocationList2[j];
					this._outputDataListener = (Action<float[], AudioChannels>)Delegate.Remove(this._outputDataListener, value2);
				}
			}
		}

		private void OnAudioFilterRead(float[] data, int nbChannels)
		{
			if (this._audioFilterReadListener != null)
			{
				this._audioFilterReadListener(Id, data, _audioChannel);
			}
		}
	}
}
