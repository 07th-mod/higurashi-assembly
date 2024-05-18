using System.Runtime.InteropServices;

namespace UMP.Wrappers
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct TrackInfo
	{
		[FieldOffset(0)]
		public int Codec;

		[FieldOffset(4)]
		public int Id;

		[FieldOffset(8)]
		public TrackTypes Type;

		[FieldOffset(12)]
		public int Profile;

		[FieldOffset(16)]
		public int Level;

		[FieldOffset(20)]
		public AudioTrackInfo Audio;

		[FieldOffset(20)]
		public VideoTrackInfo Video;
	}
}
