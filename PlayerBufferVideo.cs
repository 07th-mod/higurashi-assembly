using System;
using System.Runtime.InteropServices;

internal class PlayerBufferVideo
{
	private const string CHROMA = "RV32";

	private const int PIXEL_SIZE_RGBA = 32;

	private readonly int _width;

	private readonly int _height;

	private readonly int _pitch;

	private readonly int _lines;

	private readonly byte[] _framePixels;

	private GCHandle _gcHandle = default(GCHandle);

	public int Width => _width;

	public int Height => _height;

	public int Pitch => _pitch;

	public int Lines => _lines;

	public byte[] FramePixels => _framePixels;

	public static string Chroma => "RV32";

	internal IntPtr FramePixelsAddr
	{
		get
		{
			if (!_gcHandle.IsAllocated)
			{
				_gcHandle = GCHandle.Alloc(_framePixels, GCHandleType.Pinned);
			}
			return _gcHandle.AddrOfPinnedObject();
		}
	}

	public PlayerBufferVideo(int width, int height)
	{
		_width = width;
		_height = height;
		_lines = _height;
		_pitch = CalculatePitch(_width);
		_framePixels = new byte[_pitch * _lines];
	}

	public static int CalculatePitch(int width)
	{
		return width * 32 / 8;
	}

	internal void ClearFramePixels()
	{
		if (_gcHandle.IsAllocated)
		{
			_gcHandle.Free();
		}
	}
}
