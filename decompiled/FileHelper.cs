using System;
using System.IO;
using UnityEngine;

public static class FileHelper
{
	public static string LoadFile(string id)
	{
		string path = Path.Combine(PlatformHelper.CurrentSavesDirectory, "save_" + id + ".sav");
		if (!File.Exists(path))
		{
			return "";
		}
		return File.ReadAllText(path);
	}

	public static bool SaveFile(string id, string content)
	{
		string path = Path.Combine(PlatformHelper.CurrentSavesDirectory, "save_" + id + ".sav");
		try
		{
			File.WriteAllText(path, content);
		}
		catch (Exception arg)
		{
			Debug.Log($"Exception while writing save '{id}'. {arg}");
			return false;
		}
		return true;
	}

	public static bool SaveFile(string id, string content, string subDir)
	{
		string text = Path.Combine(PlatformHelper.CurrentSavesDirectory, subDir);
		MakeOrCreatePath(text);
		string path = Path.Combine(text, "save_" + id + ".sav");
		try
		{
			File.WriteAllText(path, content);
		}
		catch (Exception arg)
		{
			Debug.Log($"Exception while writing save '{id}'. {arg}");
			return false;
		}
		return true;
	}

	public static void DeleteFile(string id)
	{
		string path = Path.Combine(PlatformHelper.CurrentSavesDirectory, "save_" + id + ".sav");
		try
		{
			File.Delete(path);
		}
		catch (Exception arg)
		{
			Debug.Log($"Exception while deleting save '{id}'. {arg}");
		}
	}

	public static void MakeOrCreatePath(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	public static void ArchiveFile(string fullFileName)
	{
		string text = Path.Combine(PlatformHelper.CurrentSavesDirectory, "SaveArchive");
		FileInfo fileInfo = new FileInfo(fullFileName);
		string destFileName = Path.Combine(text, fileInfo.Name + ".sav");
		MakeOrCreatePath(text);
		try
		{
			File.Move(fullFileName, destFileName);
		}
		catch (Exception arg)
		{
			Debug.Log($"Exception while deleting save '{fullFileName}'. {arg}");
		}
	}

	public static string LoadPresetFile(string id)
	{
		string path = Path.Combine(Application.dataPath + "/PresetSaves", "preset_" + id + ".json");
		if (!File.Exists(path))
		{
			return "";
		}
		return File.ReadAllText(path);
	}

	public static bool SavePresetFile(string id, string content)
	{
		string path = Path.Combine(Application.dataPath + "/PresetSaves", "preset_" + id + ".json");
		try
		{
			File.WriteAllText(path, content);
		}
		catch (Exception arg)
		{
			Debug.Log($"Exception while writing save '{id}'. {arg}");
			return false;
		}
		return true;
	}
}
