using System;
using System.Collections.Generic;

namespace TMPro
{
	[Serializable]
	public class TMP_TextInfo
	{
		public int characterCount;

		public int spriteCount;

		public int spaceCount;

		public int wordCount;

		public int lineCount;

		public int pageCount;

		public TMP_CharacterInfo[] characterInfo;

		public List<TMP_WordInfo> wordInfo;

		public TMP_LineInfo[] lineInfo;

		public TMP_PageInfo[] pageInfo;

		public TMP_MeshInfo meshInfo;

		public TMP_TextInfo()
		{
			characterInfo = new TMP_CharacterInfo[0];
			wordInfo = new List<TMP_WordInfo>(32);
			lineInfo = new TMP_LineInfo[16];
			pageInfo = new TMP_PageInfo[8];
			meshInfo = default(TMP_MeshInfo);
		}

		public void Clear()
		{
			characterCount = 0;
			spaceCount = 0;
			wordCount = 0;
			lineCount = 0;
			pageCount = 0;
			spriteCount = 0;
			Array.Clear(characterInfo, 0, characterInfo.Length);
			wordInfo.Clear();
			Array.Clear(lineInfo, 0, lineInfo.Length);
			Array.Clear(pageInfo, 0, pageInfo.Length);
		}
	}
}
