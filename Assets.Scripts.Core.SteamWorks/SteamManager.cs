using Steamworks;
using System;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Core.SteamWorks
{
	internal class SteamManager : MonoBehaviour
	{
		private static SteamManager s_instance;

		private bool m_bInitialized;

		private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

		private static SteamManager Instance => s_instance ?? new GameObject("SteamManager").AddComponent<SteamManager>();

		public static bool Initialized => Instance.m_bInitialized;

		private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
		{
			Debug.LogWarning(pchDebugText);
		}

		private void Awake()
		{
			if (s_instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				s_instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				if (!Packsize.Test())
				{
					Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
				}
				if (!DllCheck.Test())
				{
					Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
				}
				m_bInitialized = SteamAPI.Init();
				if (!m_bInitialized)
				{
					Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
				}
			}
		}

		private void OnEnable()
		{
			if (s_instance == null)
			{
				s_instance = this;
			}
			if (m_bInitialized && m_SteamAPIWarningMessageHook == null)
			{
				m_SteamAPIWarningMessageHook = SteamAPIDebugTextHook;
				SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
			}
		}

		private void OnDestroy()
		{
			if (!(s_instance != this))
			{
				s_instance = null;
				if (m_bInitialized)
				{
					SteamAPI.Shutdown();
				}
			}
		}

		private void Update()
		{
			if (m_bInitialized)
			{
				SteamAPI.RunCallbacks();
			}
		}
	}
}
