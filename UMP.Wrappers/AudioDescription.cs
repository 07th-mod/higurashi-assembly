using System;
using System.Runtime.InteropServices;

namespace UMP.Wrappers
{
	internal struct AudioDescription
	{
		[MarshalAs(UnmanagedType.LPStr)]
		public string Name;

		[MarshalAs(UnmanagedType.LPStr)]
		public string Description;

		public IntPtr NextDescription;
	}
}
