using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModOptionsScreen : SokScreen
{
	public static ModOptionsScreen instance;

	public CustomButton BackButton;

	public CustomButton OpenFolderButton;

	public CustomButton OpenWorkshopButton;

	public CustomButton UploadButton;

	public RectTransform ButtonsParent;

	public static Mod SelectedMod;

	public bool ShouldRestart;

	public RectTransform SpacerPrefab;

	public RectTransform InputPrefab;

	public RectTransform Title;

	public RectTransform Version;

	private CallResult<SteamUGCQueryCompleted_t> crQueryCompleted;

	private UGCQueryHandle_t queryHandle;

	private void Awake()
	{
		instance = this;
		BackButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ModsScreen>();
			SelectedMod?.Config.Save();
			if (ShouldRestart)
			{
				WorldManager.RebootGame();
			}
		};
		OpenFolderButton.Clicked += delegate
		{
			Application.OpenURL("file:///" + SelectedMod.Path);
		};
		UploadButton.Clicked += delegate
		{
			GameCanvas.instance.SetScreen<ModUploadScreen>();
		};
	}

	private void Update()
	{
		if (!InputController.instance.DisableAllInput && InputController.instance.GetKeyDown(Key.U) && PlatformHelper.UseSteam)
		{
			GameCanvas.instance.SetScreen<ModUploadScreen>();
		}
		if (InputController.instance.CancelTriggered())
		{
			GameCanvas.instance.SetScreen<ModsScreen>();
			SelectedMod?.Config.Save();
			if (ShouldRestart)
			{
				WorldManager.RebootGame();
			}
		}
	}

	private void OnEnable()
	{
		foreach (Transform item in ButtonsParent)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		if (SelectedMod == null)
		{
			return;
		}
		if (ModUploadScreen.GetWorkshopId(SelectedMod) != 0L)
		{
			OpenWorkshopButton.gameObject.SetActive(value: true);
			OpenWorkshopButton.Clicked += delegate
			{
				OpenWorkshop();
			};
		}
		else
		{
			OpenWorkshopButton.gameObject.SetActive(value: false);
		}
		Title.GetComponent<TextMeshProUGUI>().text = SelectedMod.Manifest.Name;
		Version.GetComponent<TextMeshProUGUI>().text = "v" + SelectedMod.Manifest.Version;
		foreach (ConfigEntryBase entry in SelectedMod.Config.Entries)
		{
			entry.UI.OnUI?.Invoke(entry);
			if (!entry.UI.Hidden)
			{
				Debug.Log($"creating ui for {entry.Name} ({entry.ValueType})");
				if (entry.ValueType == typeof(bool))
				{
					CreateBoolConfig(entry);
				}
				else if (entry.ValueType == typeof(string))
				{
					CreateTextConfig(entry);
				}
				else if (entry.ValueType == typeof(int) || entry.ValueType == typeof(float))
				{
					CreateNumberConfig(entry, entry.ValueType);
				}
			}
		}
		UploadButton.gameObject.SetActive(value: false);
		if (ModManager.LocalModPaths.Contains(SelectedMod.Path))
		{
			UploadButton.gameObject.SetActive(value: true);
			return;
		}
		crQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnQueryCompleted);
		ulong workshopId = ModUploadScreen.GetWorkshopId(SelectedMod);
		if (workshopId != 0L)
		{
			queryHandle = SteamUGC.CreateQueryUGCDetailsRequest(new PublishedFileId_t[1] { (PublishedFileId_t)workshopId }, 1u);
			SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(queryHandle);
			crQueryCompleted.Set(hAPICall);
		}
	}

	private void OnQueryCompleted(SteamUGCQueryCompleted_t result, bool failed)
	{
		if (result.m_eResult != EResult.k_EResultOK)
		{
			return;
		}
		if (SteamUGC.GetQueryUGCResult(queryHandle, 0u, out var pDetails))
		{
			Debug.Log($"owner of mod is {pDetails.m_ulSteamIDOwner}");
			if (pDetails.m_ulSteamIDOwner == (ulong)SteamUser.GetSteamID())
			{
				UploadButton.gameObject.SetActive(value: true);
			}
		}
		SteamUGC.ReleaseQueryUGCRequest(queryHandle);
	}

	private static void OpenWorkshop()
	{
		if (PlatformHelper.UseSteam && !InputController.instance.GetKey(Key.LeftAlt))
		{
			SteamFriends.ActivateGameOverlayToWebPage($"https://steamcommunity.com/sharedfiles/filedetails/?id={ModUploadScreen.GetWorkshopId(SelectedMod)}");
		}
		else
		{
			Application.OpenURL($"https://steamcommunity.com/sharedfiles/filedetails/?id={ModUploadScreen.GetWorkshopId(SelectedMod)}");
		}
	}

	private void CreateTextConfig(ConfigEntryBase entry)
	{
		CustomButton customButton = UnityEngine.Object.Instantiate(PrefabManager.instance.ButtonPrefab, ButtonsParent);
		customButton.transform.localScale = Vector3.one;
		customButton.transform.localPosition = Vector3.zero;
		customButton.transform.localRotation = Quaternion.identity;
		string text = entry.UI.GetName();
		string tooltip = entry.UI.GetTooltip();
		customButton.TextMeshPro.text = ((!string.IsNullOrEmpty(text)) ? text : entry.Name);
		if (!string.IsNullOrEmpty(tooltip))
		{
			customButton.TooltipText = tooltip;
		}
		TMP_InputField component = UnityEngine.Object.Instantiate(InputPrefab, ButtonsParent).GetComponent<TMP_InputField>();
		component.text = (string)entry.BoxedValue;
		component.characterLimit = 0;
		((TMP_Text)component.placeholder).text = entry.UI.PlaceholderText;
		component.onValueChanged.AddListener(delegate(string newValue)
		{
			entry.BoxedValue = newValue;
			if (entry.UI.RestartAfterChange)
			{
				ShouldRestart = true;
			}
		});
		UnityEngine.Object.Instantiate(SpacerPrefab, ButtonsParent);
	}

	private void CreateNumberConfig(ConfigEntryBase entry, Type inputType)
	{
		CustomButton customButton = UnityEngine.Object.Instantiate(PrefabManager.instance.ButtonPrefab, ButtonsParent);
		customButton.transform.localScale = Vector3.one;
		customButton.transform.localPosition = Vector3.zero;
		customButton.transform.localRotation = Quaternion.identity;
		string text = entry.UI.GetName();
		string tooltip = entry.UI.GetTooltip();
		customButton.TextMeshPro.text = ((!string.IsNullOrEmpty(text)) ? text : entry.Name);
		if (!string.IsNullOrEmpty(tooltip))
		{
			customButton.TooltipText = tooltip;
		}
		TMP_InputField component = UnityEngine.Object.Instantiate(InputPrefab, ButtonsParent).GetComponent<TMP_InputField>();
		component.characterValidation = ((inputType == typeof(int)) ? TMP_InputField.CharacterValidation.Integer : TMP_InputField.CharacterValidation.Decimal);
		component.text = ((inputType == typeof(int)) ? ((int)entry.BoxedValue).ToString() : ((float)entry.BoxedValue).ToString());
		component.onValueChanged.AddListener(delegate(string newValue)
		{
			entry.BoxedValue = ((inputType == typeof(int)) ? ((object)int.Parse(newValue)) : ((object)float.Parse(newValue)));
			if (entry.UI.RestartAfterChange)
			{
				ShouldRestart = true;
			}
		});
		UnityEngine.Object.Instantiate(SpacerPrefab, ButtonsParent);
	}

	private void CreateBoolConfig(ConfigEntryBase entry)
	{
		CustomButton btn = UnityEngine.Object.Instantiate(PrefabManager.instance.ButtonPrefab, ButtonsParent);
		btn.transform.localScale = Vector3.one;
		btn.transform.localPosition = Vector3.zero;
		btn.transform.localRotation = Quaternion.identity;
		string name = entry.UI.GetName();
		string tooltip = entry.UI.GetTooltip();
		btn.TextMeshPro.text = ((!string.IsNullOrEmpty(name)) ? name : entry.Name) + ": " + BoolToLabel((bool)entry.BoxedValue);
		if (!string.IsNullOrEmpty(tooltip))
		{
			btn.TooltipText = tooltip;
		}
		btn.Clicked += delegate
		{
			entry.BoxedValue = !(bool)entry.BoxedValue;
			btn.TextMeshPro.text = ((!string.IsNullOrEmpty(name)) ? name : entry.Name) + ": " + BoolToLabel((bool)entry.BoxedValue);
			if (entry.UI.RestartAfterChange)
			{
				ShouldRestart = true;
			}
		};
	}

	private string BoolToLabel(bool b)
	{
		if (!b)
		{
			return SokLoc.Translate("label_off");
		}
		return SokLoc.Translate("label_on");
	}
}
