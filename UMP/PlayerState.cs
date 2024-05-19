namespace UMP
{
	public enum PlayerState
	{
		Empty,
		Opening,
		Buffering,
		ImageReady,
		Prepared,
		Playing,
		Paused,
		Stopped,
		EndReached,
		EncounteredError,
		TimeChanged,
		PositionChanged,
		SnapshotTaken
	}
}
