using UnityEngine;

namespace UMP
{
	public class UMPAudioOutput : AudioOutput
	{
		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		private void OnOutputData(float[] data, AudioChannels channels)
		{
			Debug.Log("Handle audio output data");
		}
	}
}
