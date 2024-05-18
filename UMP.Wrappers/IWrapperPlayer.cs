namespace UMP.Wrappers
{
	internal interface IWrapperPlayer
	{
		void PlayerSetDataSource(string path, object playerObject = null);

		bool PlayerPlay(object playerObject = null);

		void PlayerPause(object playerObject = null);

		void PlayerStop(object playerObject = null);

		void PlayerRelease(object playerObject = null);

		bool PlayerIsPlaying(object playerObject = null);

		long PlayerGetLength(object playerObject = null);

		long PlayerGetTime(object playerObject = null);

		void PlayerSetTime(long time, object playerObject = null);

		float PlayerGetPosition(object playerObject = null);

		void PlayerSetPosition(float pos, object playerObject = null);

		float PlayerGetRate(object playerObject = null);

		bool PlayerSetRate(float rate, object playerObject = null);

		int PlayerGetVolume(object playerObject = null);

		int PlayerSetVolume(int volume, object playerObject = null);

		bool PlayerGetMute(object playerObject = null);

		void PlayerSetMute(bool mute, object playerObject = null);

		int PlayerVideoWidth(object playerObject = null);

		int PlayerVideoHeight(object playerObject = null);

		int PlayerVideoFramesCounter(object playerObject = null);

		PlayerState PlayerGetState();

		object PlayerGetStateValue();
	}
}
