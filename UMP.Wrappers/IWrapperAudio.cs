namespace UMP.Wrappers
{
	internal interface IWrapperAudio
	{
		MediaTrackInfo[] PlayerAudioGetTracks(object playerObject = null);

		int PlayerAudioGetTrack(object playerObject = null);

		int PlayerAudioSetTrack(int audioIndex, object playerObject = null);
	}
}
