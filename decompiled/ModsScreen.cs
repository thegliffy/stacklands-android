using System.Linq;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModsScreen : SokScreen
{
	public CustomButton BackButton;

	public CustomButton DisableModsButton;

	public CustomButton OpenWorkshopButton;

	public RectTransform ButtonsParent;

	private void Awake()
	{
		if (!PlatformHelper.HasModdingSupport)
		{
			return;
		}
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<MainMenu>();
		};
		DisableModsButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ModDisablingScreen>();
		};
		foreach (Mod mod in ModManager.LoadedMods.OrderBy((Mod m) => m.Manifest.Name))
		{
			CustomButton customButton = Object.Instantiate(PrefabManager.instance.ButtonPrefab, ButtonsParent);
			customButton.transform.localScale = Vector3.one;
			customButton.transform.localPosition = Vector3.zero;
			customButton.transform.localRotation = Quaternion.identity;
			customButton.TextMeshPro.text = mod.Manifest.Name;
			customButton.Clicked += delegate
			{
				ModOptionsScreen.SelectedMod = mod;
				GameCanvas.instance.SetScreen<ModOptionsScreen>();
			};
		}
		foreach (ModManifest item in ModManager.DisabledModManifests.OrderBy((ModManifest m) => m.Name))
		{
			CustomButton customButton2 = Object.Instantiate(PrefabManager.instance.ButtonPrefab, ButtonsParent);
			customButton2.transform.localScale = Vector3.one;
			customButton2.transform.localPosition = Vector3.zero;
			customButton2.transform.localRotation = Quaternion.identity;
			customButton2.TextMeshPro.text = "<color=#A1A1A1><s>" + item.Name + "</s>";
		}
		OpenWorkshopButton.Clicked += delegate
		{
			OpenWorkshop();
		};
		OpenWorkshopButton.gameObject.SetActive(PlatformHelper.UseSteam);
	}

	private void Update()
	{
		if (InputController.instance.CancelTriggered())
		{
			GameCanvas.instance.SetScreen<MainMenu>();
		}
	}

	private static void OpenWorkshop()
	{
		if (PlatformHelper.UseSteam && !InputController.instance.GetKey(Key.LeftAlt))
		{
			SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/app/1948280/workshop");
		}
	}
}
