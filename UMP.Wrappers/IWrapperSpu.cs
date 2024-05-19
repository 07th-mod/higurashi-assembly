namespace UMP.Wrappers
{
	internal interface IWrapperSpu
	{
		MediaTrackInfo[] PlayerSpuGetTracks(object playerObject = null);

		int PlayerSpuGetTrack(object playerObject = null);

		int PlayerSpuSetTrack(int spuIndex, object playerObject = null);
	}
}
