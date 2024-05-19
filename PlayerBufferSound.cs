using System;
using System.Runtime.InteropServices;

internal class PlayerBufferSound
{
	public const int DEFAULT_CLIP_SAMPLES_SIZE = 65536;

	public const int BITS_PER_SAMPLE = 16;

	private readonly float[] _samplesData;

	private GCHandle _gcHandle = default(GCHandle);

	public int Rate
	{
		get;
		private set;
	}

	public int Channels
	{
		get;
		private set;
	}

	public int BitsPerSample
	{
		get;
		private set;
	}

	public SoundType SoundType
	{
		get;
		private set;
	}

	public int BlockSize
	{
		get;
		private set;
	}

	public float[] SamplesData => _samplesData;

	internal IntPtr SamplesDataAddr
	{
		get
		{
			if (!_gcHandle.IsAllocated)
			{
				_gcHandle = GCHandle.Alloc(_samplesData, GCHandleType.Pinned);
			}
			return _gcHandle.AddrOfPinnedObject();
		}
	}

	public long Pts
	{
		get;
		set;
	}

	public PlayerBufferSound(int rate, int channels)
	{
		SoundType = SoundType.S16N;
		Rate = rate;
		Channels = channels;
		_samplesData = new float[2048];
	}

	public static SoundType GetSoundType(string format)
	{
		switch (format)
		{
		case "S16N":
			return SoundType.S16N;
		default:
			return SoundType.S16N;
		}
	}

	public void UpdateSamplesData(IntPtr samples, uint count)
	{
	}
}
