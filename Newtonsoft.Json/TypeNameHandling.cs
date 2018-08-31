using System;

namespace Newtonsoft.Json
{
	[Flags]
	public enum TypeNameHandling
	{
		None = 0x0,
		Objects = 0x1,
		Arrays = 0x2,
		Auto = 0x4,
		All = 0x3
	}
}
