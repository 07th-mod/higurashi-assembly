using System;
using System.Runtime.InteropServices;

namespace UMP.Wrappers
{
	internal struct TrackDescription
	{
		public int Id;

		[MarshalAs(UnmanagedType.LPStr)]
		public string Name;

		public IntPtr NextDescription;
	}
}
