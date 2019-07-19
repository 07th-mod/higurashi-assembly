using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_StyleSheet : ScriptableObject
	{
		public static TMP_StyleSheet Instance;

		private static bool m_isDictionaryLoaded;

		[SerializeField]
		private List<TMP_Style> m_StyleList = new List<TMP_Style>(1);

		private Dictionary<int, TMP_Style> m_StyleDictionary = new Dictionary<int, TMP_Style>();

		private void OnEnable()
		{
			if (!(Instance == null))
			{
				return;
			}
			TMP_Settings tMP_Settings = Resources.Load("TMP Settings") as TMP_Settings;
			if (!(tMP_Settings == null))
			{
				if (tMP_Settings.styleSheet != null)
				{
					Instance = tMP_Settings.styleSheet;
				}
				else
				{
					Instance = (Resources.Load("Style Sheets/TMP Default Style Sheet") as TMP_StyleSheet);
				}
				if (!m_isDictionaryLoaded)
				{
					Instance.LoadStyleDictionary();
				}
			}
		}

		public static TMP_StyleSheet LoadDefaultStyleSheet()
		{
			if (Instance == null)
			{
				TMP_Settings tMP_Settings = Resources.Load("TMP Settings") as TMP_Settings;
				if (tMP_Settings != null && tMP_Settings.styleSheet != null)
				{
					Instance = tMP_Settings.styleSheet;
				}
				else
				{
					Instance = (Resources.Load("Style Sheets/TMP Default Style Sheet") as TMP_StyleSheet);
				}
			}
			return Instance;
		}

		public TMP_Style GetStyle(int hashCode)
		{
			if (m_StyleDictionary.TryGetValue(hashCode, out TMP_Style value))
			{
				return value;
			}
			return null;
		}

		public void UpdateStyleDictionaryKey(int old_key, int new_key)
		{
			if (m_StyleDictionary.ContainsKey(old_key))
			{
				TMP_Style value = m_StyleDictionary[old_key];
				m_StyleDictionary.Add(new_key, value);
				m_StyleDictionary.Remove(old_key);
			}
		}

		public void LoadStyleDictionary()
		{
			m_StyleDictionary.Clear();
			for (int i = 0; i < m_StyleList.Count; i++)
			{
				m_StyleList[i].RefreshStyle();
				if (!m_StyleDictionary.ContainsKey(m_StyleList[i].hashCode))
				{
					m_StyleDictionary.Add(m_StyleList[i].hashCode, m_StyleList[i]);
				}
			}
			m_isDictionaryLoaded = true;
		}
	}
}
