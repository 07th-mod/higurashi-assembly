using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[ExecuteInEditMode]
	public class TMPro_UpdateManager : MonoBehaviour
	{
		private static List<TextMeshPro> m_objectList;

		private void Awake()
		{
		}

		public void ScheduleObjectForUpdate(TextMeshPro obj)
		{
			if (m_objectList == null)
			{
				m_objectList = new List<TextMeshPro>();
			}
			m_objectList.Add(obj);
		}

		private void OnPreRender()
		{
			TMPro_EventManager.ON_PRE_RENDER_OBJECT_CHANGED();
		}
	}
}
