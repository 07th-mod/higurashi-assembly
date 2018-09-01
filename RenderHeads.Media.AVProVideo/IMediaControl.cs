using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
	public interface IMediaControl
	{
		bool OpenVideoFromFile(string path, long offset);

		void CloseVideo();

		void SetLooping(bool bLooping);

		bool IsLooping();

		bool HasMetaData();

		bool CanPlay();

		bool IsPlaying();

		bool IsSeeking();

		bool IsPaused();

		bool IsFinished();

		bool IsBuffering();

		void Play();

		void Pause();

		void Stop();

		void Rewind();

		void Seek(float timeMs);

		void SeekFast(float timeMs);

		float GetCurrentTimeMs();

		float GetPlaybackRate();

		void SetPlaybackRate(float rate);

		void MuteAudio(bool bMute);

		bool IsMuted();

		void SetVolume(float volume);

		float GetVolume();

		int GetCurrentAudioTrack();

		void SetAudioTrack(int index);

		int GetCurrentVideoTrack();

		void SetVideoTrack(int index);

		float GetBufferingProgress();

		int GetBufferedTimeRangeCount();

		bool GetBufferedTimeRange(int index, ref float startTimeMs, ref float endTimeMs);

		ErrorCode GetLastError();

		void SetTextureProperties(FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, int anisoLevel = 1);

		void GrabAudio(float[] buffer, int floatCount, int channelCount);
	}
}
