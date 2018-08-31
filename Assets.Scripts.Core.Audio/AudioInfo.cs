using System;

namespace Assets.Scripts.Core.Audio
{
	[Serializable]
	public class AudioInfo
	{
		public float Volume;

		public string Filename;

		public AudioInfo(float volume, string filename)
		{
			Volume = volume;
			Filename = filename;
		}
	}
}
