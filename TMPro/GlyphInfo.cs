using System;

namespace TMPro
{
	[Serializable]
	public class GlyphInfo
	{
		public int id;

		public float x;

		public float y;

		public float width;

		public float height;

		public float xOffset;

		public float yOffset;

		public float xAdvance;

		public static GlyphInfo Clone(GlyphInfo source)
		{
			GlyphInfo glyphInfo = new GlyphInfo();
			glyphInfo.id = source.id;
			glyphInfo.x = source.x;
			glyphInfo.y = source.y;
			glyphInfo.width = source.width;
			glyphInfo.height = source.height;
			glyphInfo.xOffset = source.xOffset;
			glyphInfo.yOffset = source.yOffset;
			glyphInfo.xAdvance = source.xAdvance;
			return glyphInfo;
		}
	}
}
