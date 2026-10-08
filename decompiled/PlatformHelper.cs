using System;
using System.IO;
using System.Linq;
using Steamworks;
using UnityEngine;

public static class PlatformHelper
{
	public static bool UseSteam => SteamManager.Initialized;

	public static bool HasModdingSupport
	{
		get
		{
			if (Application.platform == RuntimePlatform.Switch)
			{
				return false;
			}
			if (Application.isEditor && !DebugOptions.Default.ModdingSupportEnabled)
			{
				return false;
			}
			if ((from s in Environment.GetCommandLineArgs()
				select s.ToLower()).Contains("--no-mods"))
			{
				return false;
			}
			return true;
		}
	}

	public static bool IsTestBuild
	{
		get
		{
			string pchName;
			if (SteamManager.Initialized)
			{
				return SteamApps.GetCurrentBetaName(out pchName, 100);
			}
			return false;
		}
	}

	public static string CurrentSavesDirectory
	{
		get
		{
			if (SteamManager.Initialized && SteamApps.GetCurrentBetaName(out var pchName, 100))
			{
				string text = Path.Combine(Application.persistentDataPath, pchName + "_Saves");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
			return Application.persistentDataPath;
		}
	}
}
