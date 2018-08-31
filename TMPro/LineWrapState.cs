using UnityEngine;

namespace TMPro
{
	public struct LineWrapState
	{
		public int previous_LineBreak;

		public int total_CharacterCount;

		public int visible_CharacterCount;

		public int visible_SpriteCount;

		public float maxAscender;

		public float maxDescender;

		public float maxFontScale;

		public int wordCount;

		public FontStyles fontStyle;

		public float fontScale;

		public float xAdvance;

		public float currentFontSize;

		public float baselineOffset;

		public float lineOffset;

		public TMP_TextInfo textInfo;

		public TMP_LineInfo lineInfo;

		public Color32 vertexColor;

		public Extents meshExtents;
	}
}
