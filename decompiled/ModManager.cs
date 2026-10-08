using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

public class ModManager : MonoBehaviour
{
	public static ModManager instance;

	public static List<Mod> LoadedMods;

	public static Dictionary<string, Type> CardClasses;

	public static List<ModManifest> DisabledModManifests;

	public static List<string> LocalModPaths = new List<string>();

	private void Awake()
	{
		if (!PlatformHelper.HasModdingSupport)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		LoadedMods = new List<Mod>();
		DisabledModManifests = new List<ModManifest>();
		List<string> modPaths = GetModPaths();
		Debug.Log($"found {modPaths.Count} mods");
		List<ModManifest> list = new List<ModManifest>();
		foreach (string item in modPaths)
		{
			string path = Path.Combine(item, "manifest.json");
			if (!File.Exists(path))
			{
				Debug.LogError("Could not find manifest.json in \"" + item + "\"; skipping!");
				continue;
			}
			ModManifest manifest = JsonConvert.DeserializeObject<ModManifest>(File.ReadAllText(path));
			if (list.Any((ModManifest m) => m.Id == manifest.Id) || DisabledModManifests.Any((ModManifest m) => m.Id == manifest.Id))
			{
				Debug.LogError("Already loaded a mod with id " + manifest.Id + "; skipping!");
			}
			else if (SaveManager.instance.CurrentSave.DisabledMods.Contains(manifest.Id))
			{
				DisabledModManifests.Add(manifest);
				Debug.LogWarning("Skipping " + manifest.Id + " because it is disabled!");
			}
			else
			{
				manifest.Folder = item;
				list.Add(manifest);
			}
		}
		try
		{
			list = DependencyHelper.GetValidModLoadOrder(list);
			Debug.Log("Found valid mod load order: " + string.Join(", ", list.Select((ModManifest m) => m.Id)));
		}
		catch (Exception ex)
		{
			Debug.LogError("Could not find valid mod load order!");
			Debug.LogError(ex.Message);
			return;
		}
		foreach (ModManifest item2 in list)
		{
			LoadModFromDir(new DirectoryInfo(item2.Folder));
		}
		FindClassesInAssemblies();
	}

	public static List<string> GetModPaths()
	{
		List<string> list = new List<string>();
		string path = Path.Combine(Application.persistentDataPath, "Mods");
		FileHelper.MakeOrCreatePath(path);
		DirectoryInfo[] directories = new DirectoryInfo(path).GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			LocalModPaths.Add(directoryInfo.FullName);
			list.Add(directoryInfo.FullName);
		}
		if (PlatformHelper.UseSteam)
		{
			list.AddRange(GetSteamWorkshopModPaths());
		}
		return list;
	}

	private static List<string> GetSteamWorkshopModPaths()
	{
		List<string> list = new List<string>();
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
		SteamUGC.GetSubscribedItems(array, numSubscribedItems);
		PublishedFileId_t[] array2 = array;
		foreach (PublishedFileId_t nPublishedFileID in array2)
		{
			uint cchFolderSize = 1024u;
			ulong punSizeOnDisk;
			string pchFolder;
			uint punTimeStamp;
			bool itemInstallInfo = SteamUGC.GetItemInstallInfo(nPublishedFileID, out punSizeOnDisk, out pchFolder, cchFolderSize, out punTimeStamp);
			if (!string.IsNullOrEmpty(pchFolder) && itemInstallInfo)
			{
				list.Add(pchFolder);
			}
		}
		return list;
	}

	private void LoadModFromDir(DirectoryInfo dir)
	{
		string path = Path.Combine(dir.FullName, "manifest.json");
		if (!File.Exists(path))
		{
			Debug.LogError("Could not find manifest.json in \"" + dir.FullName + "\"; skipping!");
			return;
		}
		ModManifest modManifest = JsonConvert.DeserializeObject<ModManifest>(File.ReadAllText(path));
		if (TryGetMod(modManifest.Id, out var _))
		{
			Debug.LogError("Already loaded a mod with id " + modManifest.Id + "; skipping!");
			return;
		}
		Debug.Log("loading mod :) " + modManifest.Name + " (" + modManifest.Id + ") v" + modManifest.Version);
		string text = "";
		Type type = null;
		string[] files = Directory.GetFiles(dir.FullName, "*.dll");
		if (files.Length > 1)
		{
			if (files.Contains(modManifest.Id + ".dll"))
			{
				text = Path.Combine(dir.FullName, modManifest.Id + ".dll");
			}
			else
			{
				if (string.IsNullOrEmpty(modManifest.Assembly))
				{
					Debug.LogError("Found more than 1 assemblies in the mod folder. Please specify the main one with the \"assembly\" property in your manifest.json");
					return;
				}
				text = Path.Combine(dir.FullName, modManifest.Assembly);
			}
		}
		if (files.Length == 1)
		{
			text = Path.Combine(dir.FullName, files[0]);
		}
		if (File.Exists(text))
		{
			Type[] types = Assembly.LoadFrom(text).GetTypes();
			foreach (Type type2 in types)
			{
				if (typeof(Mod).IsAssignableFrom(type2))
				{
					if (type != null)
					{
						Debug.LogWarning($"Found more than 1 Mod class! Keeping {type}, skipping {type2}!");
					}
					else
					{
						type = type2;
					}
				}
			}
		}
		else
		{
			Debug.LogWarning("Could not find Mods/" + dir.Name + "/" + modManifest.Id + ".dll; loading as codeless mod!");
		}
		if (type == null)
		{
			type = typeof(Mod);
		}
		base.gameObject.SetActive(value: false);
		Mod mod2 = base.gameObject.AddComponent(type) as Mod;
		mod2.Manifest = modManifest;
		mod2.Harmony = new Harmony(modManifest.Id);
		mod2.Path = dir.FullName;
		mod2.Logger = new ModLogger(modManifest);
		string text2 = Path.Combine(dir.FullName, "config.json");
		if (!File.Exists(text2))
		{
			File.WriteAllText(text2, "{}");
		}
		mod2.Config = new ConfigFile(mod2, text2);
		LoadedMods.Add(mod2);
		base.gameObject.SetActive(value: true);
	}

	internal void FindClassesInAssemblies()
	{
		CardClasses = new Dictionary<string, Type>();
		foreach (Type safeType in typeof(CardData).Assembly.GetSafeTypes())
		{
			if (typeof(CardData).IsAssignableFrom(safeType))
			{
				CardClasses[safeType.ToString()] = safeType;
			}
		}
		foreach (Mod loadedMod in LoadedMods)
		{
			if (loadedMod.GetType() == typeof(Mod))
			{
				continue;
			}
			foreach (Type safeType2 in loadedMod.GetType().Assembly.GetSafeTypes())
			{
				if (typeof(CardData).IsAssignableFrom(safeType2))
				{
					CardClasses[safeType2.ToString()] = safeType2;
				}
			}
		}
	}

	internal void ReadyUpMods()
	{
		Debug.Log("(ModManager) readying up mods");
		foreach (Mod mod in LoadedMods)
		{
			try
			{
				string locPath = Path.Combine(mod.Path, "localization.tsv");
				if (File.Exists(locPath))
				{
					SokLoc.instance.LoadTermsFromFile(locPath);
					SokLoc.instance.LanguageChanged += delegate
					{
						Debug.Log("Loading localization.tsv for " + mod.Manifest.Id);
						SokLoc.instance.LoadTermsFromFile(locPath, disableWarning: true);
					};
				}
			}
			catch (Exception exception)
			{
				Debug.LogError("Failed to load localization.tsv for " + mod.Manifest.Id);
				Debug.LogException(exception);
			}
			try
			{
				mod.Ready();
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
			}
		}
	}

	public static bool TryGetMod(string id, out Mod mod)
	{
		Mod mod2 = LoadedMods.FirstOrDefault((Mod m) => m.Manifest.Id == id);
		if (mod2 == null)
		{
			mod = null;
			return false;
		}
		mod = mod2;
		return true;
	}
}
