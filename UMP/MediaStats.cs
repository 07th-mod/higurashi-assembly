namespace UMP
{
	public struct MediaStats
	{
		public int InputReadBytes;

		public float InputBitrate;

		public int DemuxReadBytes;

		public float DemuxBitrate;

		public int DemuxCorrupted;

		public int DemuxDiscontinuity;

		public int DecodedVideo;

		public int DecodedAudio;

		public int VideoDisplayedPictures;

		public int VideoLostPictures;

		public int AudioPlayedAbuffers;

		public int AudioLostAbuffers;

		public int StreamSentPackets;

		public int StreamSentBytes;

		public float StreamSendBitrate;
	}
}
