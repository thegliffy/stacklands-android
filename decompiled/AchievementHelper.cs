using Steamworks;
using UnityEngine;

public class AchievementHelper
{
	public static void UnlockAchievement(string achName)
	{
		if (!Application.isEditor && PlatformHelper.UseSteam)
		{
			SteamUserStats.SetAchievement(achName);
			SteamUserStats.StoreStats();
		}
	}
}
