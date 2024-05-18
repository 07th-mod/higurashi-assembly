using System;
using System.Runtime.InteropServices;

namespace UMP.Wrappers
{
	internal struct AudioOutputDevice
	{
		public IntPtr NextDevice;

		[MarshalAs(UnmanagedType.LPStr)]
		public string Device;

		[MarshalAs(UnmanagedType.LPStr)]
		public string Description;
	}
}
