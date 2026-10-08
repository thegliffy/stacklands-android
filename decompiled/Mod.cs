using HarmonyLib;
using UnityEngine;

public class Mod : MonoBehaviour
{
	public ModManifest Manifest;

	protected internal ModLogger Logger;

	protected internal Harmony Harmony;

	public ConfigFile Config;

	public string Path;

	public virtual void Ready()
	{
	}

	public virtual object Call(params object[] args)
	{
		return null;
	}
}
