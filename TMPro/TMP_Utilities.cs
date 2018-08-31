using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	public static class TMP_Utilities
	{
		public static Dictionary<int, T> LoadDictionaryFromList<T>(List<T> list) where T : Object
		{
			Dictionary<int, T> dictionary = new Dictionary<int, T>();
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<int, T> dictionary2 = dictionary;
				T val = list[i];
				dictionary2.Add(val.GetInstanceID(), list[i]);
			}
			return dictionary;
		}

		public static List<T> SaveDictionaryToList<T>(Dictionary<int, T> dict) where T : Object
		{
			List<T> list = new List<T>();
			foreach (KeyValuePair<int, T> item in dict)
			{
				list.Add(item.Value);
			}
			return list;
		}
	}
}
