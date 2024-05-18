namespace UMP
{
	public interface IMediaListener : IPlayerOpeningListener, IPlayerBufferingListener, IPlayerImageReadyListener, IPlayerPreparedListener, IPlayerPlayingListener, IPlayerPausedListener, IPlayerStoppedListener, IPlayerEndReachedListener, IPlayerEncounteredErrorListener
	{
	}
}
