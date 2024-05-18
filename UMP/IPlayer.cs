using UnityEngine;

namespace UMP
{
	internal interface IPlayer
	{
		GameObject[] VideoOutputObjects
		{
			get;
			set;
		}

		PlayerManagerEvents EventManager
		{
			get;
		}

		PlayerOptions Options
		{
			get;
		}

		PlayerState State
		{
			get;
		}

		object StateValue
		{
			get;
		}

		string DataSource
		{
			get;
			set;
		}

		bool IsPlaying
		{
			get;
		}

		bool IsReady
		{
			get;
		}

		bool AbleToPlay
		{
			get;
		}

		long Length
		{
			get;
		}

		float FrameRate
		{
			get;
		}

		int FramesCounter
		{
			get;
		}

		byte[] FramePixels
		{
			get;
		}

		long Time
		{
			get;
			set;
		}

		float Position
		{
			get;
			set;
		}

		float PlaybackRate
		{
			get;
			set;
		}

		int Volume
		{
			get;
			set;
		}

		bool Mute
		{
			get;
			set;
		}

		int VideoWidth
		{
			get;
		}

		int VideoHeight
		{
			get;
		}

		Vector2 VideoSize
		{
			get;
		}

		void AddMediaListener(IMediaListener listener);

		void RemoveMediaListener(IMediaListener listener);

		void Prepare();

		bool Play();

		void Pause();

		void Stop(bool resetTexture);

		void Stop();

		void Release();

		string GetFormattedLength(bool detail);
	}
}
