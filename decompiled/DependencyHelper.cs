using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DependencyHelper
{
	public static List<ModManifest> GetValidModLoadOrder(List<ModManifest> mods)
	{
		List<ModManifest> list = new List<ModManifest>();
		ModManifest modManifest = new ModManifest
		{
			Id = "Stacklands"
		};
		foreach (ModManifest mod in mods)
		{
			modManifest.Dependencies.Add(mod.Id);
		}
		Resolve(mods, modManifest, list, new List<ModManifest>());
		return list.Take(list.Count - 1).ToList();
	}

	private static void Resolve(List<ModManifest> mods, ModManifest node, List<ModManifest> resolved, List<ModManifest> unresolved)
	{
		unresolved.Add(node);
		foreach (string edge in node.Dependencies)
		{
			if (resolved.Find((ModManifest x) => x.Id == edge) == null)
			{
				if (unresolved.Find((ModManifest x) => x.Id == edge) != null)
				{
					throw new Exception("CIRCULAR DEP " + node.Id + "<->" + edge);
				}
				ModManifest modManifest = mods.Find((ModManifest x) => x.Id == edge);
				if (modManifest == null)
				{
					throw new Exception("COULD NOT FIND " + edge);
				}
				Resolve(mods, modManifest, resolved, unresolved);
			}
		}
		foreach (string edge2 in node.OptionalDependencies)
		{
			if (resolved.Find((ModManifest x) => x.Id == edge2) == null)
			{
				if (unresolved.Find((ModManifest x) => x.Id == edge2) != null)
				{
					throw new Exception("CIRCULAR DEP " + node.Id + "<->" + edge2);
				}
				ModManifest modManifest2 = mods.Find((ModManifest x) => x.Id == edge2);
				if (modManifest2 != null)
				{
					Resolve(mods, modManifest2, resolved, unresolved);
				}
				else
				{
					Debug.LogWarning("Missing optional dependency for " + node.Id + ": " + edge2);
				}
			}
		}
		resolved.Add(node);
		unresolved.Remove(node);
	}
}
