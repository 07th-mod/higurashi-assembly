using System;

namespace UMP.Wrappers
{
	internal struct EventStruct
	{
		public EventTypes Type;

		public IntPtr PObj;

		public MediaDescriptorUnion MediaDescriptor;
	}
}
