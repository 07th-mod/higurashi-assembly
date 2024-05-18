using System.Runtime.InteropServices;

namespace UMP.Wrappers
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct MediaDescriptorUnion
	{
		[FieldOffset(0)]
		public MetaChanged MetaChanged;

		[FieldOffset(0)]
		public SubitemAdded SubitemAdded;

		[FieldOffset(0)]
		public PlayerBuffering PlayerBuffering;

		[FieldOffset(0)]
		public DurationChanged DurationChanged;

		[FieldOffset(0)]
		public ParsedChanged ParsedChanged;

		[FieldOffset(0)]
		public Freed Freed;

		[FieldOffset(0)]
		public StateChanged StateChanged;

		[FieldOffset(0)]
		public PlayerPositionChanged PlayerPositionChanged;

		[FieldOffset(0)]
		public PlayerTimeChanged PlayerTimeChanged;

		[FieldOffset(0)]
		public PlayerTitleChanged PlayerTitleChanged;

		[FieldOffset(0)]
		public PlayerSeekableChanged PlayerSeekableChanged;

		[FieldOffset(0)]
		public PlayerPausableChanged PlayerPausableChanged;

		[FieldOffset(0)]
		public ListItemAdded ListItemAdded;

		[FieldOffset(0)]
		public ListWillAddItem ListWillAddItem;

		[FieldOffset(0)]
		public ListItemDeleted ListItemDeleted;

		[FieldOffset(0)]
		public ListWillDeleteItem ListWillDeleteItem;

		[FieldOffset(0)]
		public ListPlayerNextItemSet ListPlayerNextItemSet;

		[FieldOffset(0)]
		public PlayerSnapshotTaken PlayerSnapshotTaken;

		[FieldOffset(0)]
		public PlayerLengthChanged PlayerLengthChanged;

		[FieldOffset(0)]
		public VlmMediaEvent VlmMediaEvent;

		[FieldOffset(0)]
		public PlayerMediaChanged PlayerMediaChanged;
	}
}
