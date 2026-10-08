using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModDisablingScreen : SokScreen
{
	public CustomButton BackButton;

	public RectTransform ButtonsParent;

	public bool ShouldRestart;

	private void Awake()
	{
		BackButton.Clicked += delegate
		{
			SaveManager.instance.Save(saveRound: false);
			if (ShouldRestart)
			{
				WorldManager.RebootGame();
			}
			GameCanvas.instance.SetScreen<ModsScreen>();
		};
		List<ModManifest> list = new List<ModManifest>();
		if (ModManager.LoadedMods != null)
		{
			list.AddRange(ModManager.LoadedMods.Select((Mod m) => m.Manifest));
		}
		if (ModManager.DisabledModManifests != null)
		{
			list.AddRange(ModManager.DisabledModManifests);
		}
		list.Sort((ModManifest x, ModManifest y) => string.Compare(x.Name, y.Name));
		foreach (ModManifest manifest in list)
		{
			CustomButton btn = Object.Instantiate(PrefabManager.instance.ButtonPrefab, ButtonsParent);
			btn.transform.localScale = Vector3.one;
			btn.transform.localPosition = Vector3.zero;
			btn.transform.localRotation = Quaternion.identity;
			if (ModManager.DisabledModManifests.Contains(manifest))
			{
				btn.TextMeshPro.text = "<color=#A1A1A1><s>" + manifest.Name + "</s>";
			}
			else
			{
				btn.TextMeshPro.text = manifest.Name;
			}
			btn.Clicked += delegate
			{
				if (ModManager.DisabledModManifests.Contains(manifest))
				{
					ModManager.DisabledModManifests.Remove(manifest);
					SaveManager.instance.CurrentSave.DisabledMods.Remove(manifest.Id);
					btn.TextMeshPro.text = manifest.Name;
				}
				else
				{
					ModManager.DisabledModManifests.Add(manifest);
					SaveManager.instance.CurrentSave.DisabledMods.Add(manifest.Id);
					btn.TextMeshPro.text = "<color=#A1A1A1><s>" + manifest.Name + "</s>";
				}
				ShouldRestart = true;
			};
		}
	}
}
