using System.Globalization;
using UnityEngine;

namespace Assets.Scripts.Core.TextWindow
{
	public class TextCharacter
	{
		private readonly char ch;

		public float StartTime;

		public float FinishTime;

		private float time;

		public TextCharacter(char character, float start, float end)
		{
			ch = character;
			StartTime = start;
			FinishTime = end;
			time = 0f;
		}

		public char GetCharacter()
		{
			return ch;
		}

		public string GetCharacter(float delta)
		{
			time += delta;
			if (StartTime > time)
			{
				return "<alpha=#00>" + ch;
			}
			if (ch == ' ')
			{
				return ch.ToString(CultureInfo.InvariantCulture);
			}
			if (FinishTime < time)
			{
				return ch.ToString(CultureInfo.InvariantCulture);
			}
			float t = (time - StartTime) / (FinishTime - StartTime);
			int num = (int)Mathf.Lerp(0f, 255f, t);
			return "<alpha=#" + num.ToString("X2") + ">" + ch;
		}

		public void Finish()
		{
			StartTime = 0f;
			FinishTime = 0f;
		}
	}
}
