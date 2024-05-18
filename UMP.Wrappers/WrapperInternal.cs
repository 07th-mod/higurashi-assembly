using System;

namespace UMP.Wrappers
{
	internal class WrapperInternal : IWrapperNative, IWrapperPlayer
	{
		public int NativeIndex => 0;

		public WrapperInternal(PlayerOptionsIPhone options)
		{
		}

		public void NativeInitPlayer(string options)
		{
		}

		public IntPtr NativeGetTexture()
		{
			return IntPtr.Zero;
		}

		public void NativeUpdateTexture(IntPtr texture)
		{
		}

		public void NativeSetPixelsBuffer(IntPtr buffer, int width, int height)
		{
		}

		public void NativeUpdatePixelsBuffer()
		{
		}

		public void PlayerSetDataSource(string path, object playerObject = null)
		{
		}

		public bool PlayerPlay(object playerObject = null)
		{
			return false;
		}

		public void PlayerPause(object playerObject = null)
		{
		}

		public void PlayerStop(object playerObject = null)
		{
		}

		public void PlayerRelease(object playerObject = null)
		{
		}

		public bool PlayerIsPlaying(object playerObject = null)
		{
			return false;
		}

		public bool PlayerIsReady(object playerObject = null)
		{
			return false;
		}

		public long PlayerGetLength(object playerObject = null)
		{
			return 0L;
		}

		public long PlayerGetTime(object playerObject = null)
		{
			return 0L;
		}

		public void PlayerSetTime(long time, object playerObject = null)
		{
		}

		public float PlayerGetPosition(object playerObject = null)
		{
			return 0f;
		}

		public void PlayerSetPosition(float pos, object playerObject = null)
		{
		}

		public float PlayerGetRate(object playerObject = null)
		{
			return 1f;
		}

		public bool PlayerSetRate(float rate, object playerObject = null)
		{
			return true;
		}

		public int PlayerGetVolume(object playerObject = null)
		{
			return 0;
		}

		public int PlayerSetVolume(int volume, object playerObject = null)
		{
			return 1;
		}

		public bool PlayerGetMute(object playerObject = null)
		{
			return false;
		}

		public void PlayerSetMute(bool mute, object playerObject = null)
		{
		}

		public int PlayerVideoWidth(object playerObject = null)
		{
			return 0;
		}

		public int PlayerVideoHeight(object playerObject = null)
		{
			return 0;
		}

		public int PlayerVideoFramesCounter(object playerObject = null)
		{
			return 0;
		}

		public PlayerState PlayerGetState()
		{
			return PlayerState.Empty;
		}

		public object PlayerGetStateValue()
		{
			return null;
		}
	}
}
