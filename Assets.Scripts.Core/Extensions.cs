using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
	public static class Extensions
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			int num = list.Count;
			while (num > 1)
			{
				num--;
				int num2 = Random.Range(0, num + 1);
				int index = num2;
				int index2 = num;
				T val = list[num];
				T val2 = list[num2];
				T val4 = list[index] = val;
				val4 = (list[index2] = val2);
			}
		}
	}
}
