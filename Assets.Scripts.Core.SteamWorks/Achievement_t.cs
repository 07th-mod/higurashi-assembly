namespace Assets.Scripts.Core.SteamWorks
{
	public class Achievement_t
	{
		public string m_eAchievementID;

		public string m_strName;

		public string m_strDescription;

		public bool m_bAchieved;

		public Achievement_t(string achievementID, string name, string desc)
		{
			m_eAchievementID = achievementID;
			m_strName = name;
			m_strDescription = desc;
			m_bAchieved = false;
		}
	}
}
