using System;
using UnityEngine;

public class ModLogger
{
	public ModManifest Manifest;

	public ModLogger(ModManifest manifest)
	{
		Manifest = manifest;
	}

	public void Log(LogType logType, string message)
	{
		Debug.unityLogger.Log(logType, $"[{FormatTime(DateTime.Now)}] [{logType} : {Manifest.Id}] {message}");
	}

	public void Log(string message)
	{
		Log(LogType.Log, message);
	}

	public void LogWarning(string message)
	{
		Log(LogType.Warning, message);
	}

	public void LogError(string message)
	{
		Log(LogType.Error, message);
	}

	public void LogException(string message)
	{
		Log(LogType.Exception, message);
	}

	public static string FormatTime(DateTime dt)
	{
		return dt.ToString("HH:mm:ss");
	}
}
