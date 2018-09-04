namespace RenderHeads.Media.AVProVideo
{
	public interface IMediaInfo
	{
		float GetDurationMs();

		int GetVideoWidth();

		int GetVideoHeight();

		float GetVideoFrameRate();

		float GetVideoDisplayRate();

		bool HasVideo();

		bool HasAudio();

		int GetAudioTrackCount();

		string GetCurrentAudioTrackId();

		int GetCurrentAudioTrackBitrate();

		int GetVideoTrackCount();

		string GetCurrentVideoTrackId();

		int GetCurrentVideoTrackBitrate();

		string GetPlayerDescription();

		bool PlayerSupportsLinearColorSpace();
	}
}
