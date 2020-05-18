using Assets.Scripts.Core.Buriko;
using Assets.Scripts.UI.Tips;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.SteamWorks
{
	public class SteamController : MonoBehaviour
	{
		private SteamManager steamManager;

		private Callback<UserStatsReceived_t> userStatsReceived;

		private CGameID gameID;

		private List<string> achievementList;

		private bool requestedStats;

		private bool hasStats;

		private bool needPushStats;

		private void Awake()
		{
			if (Environment.GetCommandLineArgs().Contains("nosteam"))
			{
				Debug.Log("-nosteam switch set, skipping steamworks initialization.");
				return;
			}
			string path = Path.Combine(Application.streamingAssetsPath, "Data/steamId.txt");
			string path2 = Path.Combine(Application.streamingAssetsPath, "Data/achievements.txt");
			if (!File.Exists(path) || !File.Exists(path2))
			{
				Debug.Log("Steam initialization skipped as one of the requisite files was not present.");
				return;
			}
			GameObject gameObject = new GameObject("SteamManager");
			steamManager = gameObject.AddComponent<SteamManager>();
			userStatsReceived = new Callback<UserStatsReceived_t>(OnUserStatsReceived);
			gameID = new CGameID(SteamUtils.GetAppID());
			Debug.Log("Steamworks initialized. AppId: " + gameID);
		}

		private void Update()
		{
			if (!(steamManager == null) && SteamManager.Initialized)
			{
				if (!requestedStats)
				{
					bool flag = requestedStats = SteamUserStats.RequestCurrentStats();
				}
				if (Input.GetKeyDown(KeyCode.R) && Application.platform == RuntimePlatform.WindowsEditor)
				{
					Debug.Log("Resetting achievements");
					SteamUserStats.ResetAllStats(bAchievementsToo: true);
					requestedStats = false;
				}
				if (needPushStats)
				{
					Debug.Log("Storing steam stats.");
					SteamUserStats.StoreStats();
					needPushStats = false;
				}
			}
		}

		public void Close()
		{
			if (!(steamManager == null))
			{
				SteamAPI.Shutdown();
			}
		}

		public void CheckTipsAchievements()
		{
			if (!(steamManager == null))
			{
				foreach (TipsDataEntry tip in TipsData.Tips)
				{
					if (!BurikoMemory.Instance.HasReadScript(tip.Script + ".mg"))
					{
						return;
					}
				}
				Achievement_t achievement_t = Achievements.achievements.FirstOrDefault((Achievement_t a) => a.m_eAchievementID.Contains("_TIPS"));
				if (achievement_t != null)
				{
					AddAchievement(achievement_t.m_eAchievementID);
				}
			}
		}

		public void AddAchievement(string id)
		{
			if (!(steamManager == null) && SteamManager.Initialized && hasStats)
			{
				if (Achievements.achievements == null)
				{
					Achievements.Load();
				}
				Achievement_t achievement_t = Achievements.achievements.SingleOrDefault((Achievement_t a) => a.m_eAchievementID.ToString() == id);
				if (achievement_t == null)
				{
					Debug.LogWarning("Achievement id " + id + " does not exist!");
				}
				else if (!achievement_t.m_bAchieved)
				{
					achievement_t.m_bAchieved = true;
					SteamUserStats.SetAchievement(id);
					needPushStats = true;
				}
			}
		}

		private void OnUserStatsReceived(UserStatsReceived_t pCallback)
		{
			if (steamManager == null || !SteamManager.Initialized)
			{
				return;
			}
			Debug.Log("Steamworks UserStats Received");
			if ((ulong)gameID == pCallback.m_nGameID)
			{
				if (pCallback.m_eResult == EResult.k_EResultOK)
				{
					hasStats = true;
					if (Achievements.achievements == null)
					{
						Achievements.Load();
					}
					Achievement_t[] achievements = Achievements.achievements;
					foreach (Achievement_t achievement_t in achievements)
					{
						if (SteamUserStats.GetAchievement(achievement_t.m_eAchievementID.ToString(), out achievement_t.m_bAchieved))
						{
							achievement_t.m_strName = SteamUserStats.GetAchievementDisplayAttribute(achievement_t.m_eAchievementID.ToString(), "name").Trim();
							achievement_t.m_strDescription = SteamUserStats.GetAchievementDisplayAttribute(achievement_t.m_eAchievementID.ToString(), "desc").Trim();
							Debug.Log(achievement_t.m_strName + " : " + achievement_t.m_bAchieved);
						}
						else
						{
							Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + achievement_t.m_eAchievementID + "\nIs it registered in the Steam Partner site?");
						}
					}
				}
				else
				{
					Debug.LogWarning("Steamworks UserStats failed to retrieve: " + pCallback.m_eResult);
				}
			}
			else
			{
				Debug.Log("Received stats for incorrect gameID: " + pCallback.m_nGameID);
			}
		}
	}
}
