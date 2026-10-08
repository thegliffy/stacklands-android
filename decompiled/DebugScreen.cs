using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class DebugScreen : MonoBehaviour
{
	public TMP_InputField SearchField;

	public RectTransform GeneralRect;

	public RectTransform CardRect;

	public RectTransform ShortcutRect;

	public RectTransform EffectRect;

	public RectTransform SavesRect;

	public RectTransform PresetRect;

	public RectTransform CutscenesRect;

	public RectTransform GeneralContent;

	public RectTransform CardContent;

	public RectTransform ShortcutContent;

	public RectTransform EffectContent;

	public CustomButton GeneralButton;

	public CustomButton CardsButton;

	public CustomButton ShortcutButton;

	public CustomButton EffectButton;

	public CustomButton SavesButton;

	public CustomButton PresetButton;

	public CustomButton CutscenesButton;

	public CustomButton EndlessMoonButton;

	public CustomButton NeedVillagersButton;

	public CustomButton EndCurrentMoonButton;

	public CustomButton UnlockBaseGameIdeasButton;

	public CustomButton UnlockIdeasButton;

	public CustomButton UnlockBoostersButton;

	public CustomButton UnlockQuestsButton;

	public CustomButton PeacefulModeButton;

	public CustomButton NoFoodButton;

	public CustomButton NoEnergyButton;

	public CustomButton CoinChestButton;

	public CustomButton ResetRunButton;

	public CustomButton SpawnEnemiesButton;

	public CustomButton StartCitiesRunButton;

	public CustomButton DemonScenarioButton;

	public CustomButton KrakenScenarioButton;

	public CustomButton IslandScenarioButton;

	public CustomButton DemonLordScenarioButton;

	public CustomButton WitchForestButton;

	public RectTransform StatusEffectButtonParent;

	public RectTransform CutsceneButtonParent;

	public RectTransform SavesParent;

	public CustomButton SaveButton;

	public CustomButton OpenSavesDirectoryButton;

	public RectTransform PresetsParent;

	public CustomButton PresetSaveButton;

	public CustomButton OpenPresetsDirectoryButton;

	private DebugTab OpenTab;

	private StatusEffect SelectedStatusEffect;

	private List<CardData> cardData = new List<CardData>();

	private List<CustomButton> cards = new List<CustomButton>();

	public static DebugScreen instance;

	private void InitializeDebugScreen()
	{
		cardData = WorldManager.instance.CardDataPrefabs.OrderBy((CardData x) => (x.MyCardType == CardType.Ideas) ? ("Idea: " + x.Name) : x.Name).ToList();
		cards.Clear();
		for (int i = 0; i < cardData.Count; i++)
		{
			CardData prefab = cardData[i];
			CustomButton customButton = UnityEngine.Object.Instantiate(PrefabManager.instance.DebugButtonPrefab);
			customButton.transform.SetParent(CardContent);
			customButton.transform.localPosition = Vector3.zero;
			customButton.transform.localScale = Vector3.one;
			customButton.transform.localRotation = Quaternion.identity;
			customButton.TextMeshPro.text = prefab.FullName;
			customButton.Clicked += delegate
			{
				CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), prefab, faceUp: true, checkAddToStack: false);
				WorldManager.instance.StackSend(cardData.MyGameCard, Vector3.zero);
			};
			cards.Add(customButton);
		}
		UpdateSaveElements();
	}

	private void Start()
	{
		SwitchTab(DebugTab.General);
		InitializeDebugScreen();
	}

	private void Update()
	{
		CheckDebugInput();
		GeneralButton.Image.color = (GeneralRect.gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		CardsButton.Image.color = (CardRect.gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		ShortcutButton.Image.color = (ShortcutRect.gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		EffectButton.Image.color = (EffectRect.gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		SavesButton.Image.color = (SavesRect.gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		CutscenesButton.Image.color = (CutscenesRect.gameObject.activeInHierarchy ? ColorManager.instance.BackgroundColor : ColorManager.instance.InactiveBackgroundColor);
		EndlessMoonButton.TextMeshPro.text = SokLoc.Translate("label_debug_endless_moon", LocParam.Create("on_off", YesNo(WorldManager.instance.DebugEndlessMoonEnabled)));
		PeacefulModeButton.TextMeshPro.text = SokLoc.Translate("label_debug_toggle_peaceful_mode", LocParam.Create("on_off", YesNo(WorldManager.instance.CurrentRunOptions.IsPeacefulMode)));
		NoFoodButton.TextMeshPro.text = SokLoc.Translate("label_debug_toggle_no_food", LocParam.Create("on_off", YesNo(WorldManager.instance.DebugNoFoodEnabled)));
		NeedVillagersButton.TextMeshPro.text = SokLoc.Translate("label_debug_need_villagers", LocParam.Create("on_off", YesNo(WorldManager.instance.DebugDontNeedVillagers)));
		NoEnergyButton.TextMeshPro.text = SokLoc.Translate("label_debug_no_energy", LocParam.Create("on_off", YesNo(WorldManager.instance.DebugNoEnergyEnabled)));
	}

	private void Awake()
	{
		instance = this;
		SetHandlers();
	}

	private void CheckDebugInput()
	{
		if (SelectedStatusEffect != null && InputController.instance.GetInputBegan(0))
		{
			WorldManager.instance.HoveredCard?.CardData?.AddStatusEffect(SelectedStatusEffect);
			SelectedStatusEffect = null;
		}
		if (!Application.isEditor)
		{
			WorldManager.instance.CheckDebugInput();
		}
	}

	private void SwitchTab(DebugTab tab)
	{
		GeneralRect.gameObject.SetActive(tab == DebugTab.General);
		CardRect.gameObject.SetActive(tab == DebugTab.Cards);
		ShortcutRect.gameObject.SetActive(tab == DebugTab.Shortcuts);
		EffectRect.gameObject.SetActive(tab == DebugTab.Effects);
		SavesRect.gameObject.SetActive(tab == DebugTab.Saves);
		PresetRect.gameObject.SetActive(tab == DebugTab.Presets);
		CutscenesRect.gameObject.SetActive(tab == DebugTab.Cutscenes);
	}

	private void SetHandlers()
	{
		GeneralButton.Clicked += delegate
		{
			OpenTab = DebugTab.General;
			SwitchTab(OpenTab);
		};
		CardsButton.Clicked += delegate
		{
			OpenTab = DebugTab.Cards;
			SwitchTab(OpenTab);
		};
		ShortcutButton.Clicked += delegate
		{
			OpenTab = DebugTab.Shortcuts;
			SwitchTab(OpenTab);
		};
		EffectButton.Clicked += delegate
		{
			OpenTab = DebugTab.Effects;
			SwitchTab(OpenTab);
		};
		SavesButton.Clicked += delegate
		{
			OpenTab = DebugTab.Saves;
			SwitchTab(OpenTab);
		};
		CutscenesButton.Clicked += delegate
		{
			OpenTab = DebugTab.Cutscenes;
			SwitchTab(OpenTab);
		};
		PresetButton.Clicked += delegate
		{
			if (Application.isEditor)
			{
				OpenTab = DebugTab.Presets;
				SwitchTab(OpenTab);
			}
			else
			{
				GameCanvas.instance.ShowSimpleModal("This option is only available in editor", "Not available!");
			}
		};
		SearchField.onValueChanged.AddListener(delegate(string value)
		{
			foreach (CustomButton card in cards)
			{
				card.gameObject.SetActive(value: false);
			}
			foreach (CustomButton item in cards.Where((CustomButton card) => card.TextMeshPro.text.ToLower().Replace(" ", "").Contains(value.ToLower().Replace(" ", ""))).ToList())
			{
				item.gameObject.SetActive(value: true);
			}
		});
		EndlessMoonButton.Clicked += delegate
		{
			ToggleEndlessMoon();
		};
		NeedVillagersButton.Clicked += delegate
		{
			ToggleNeedVillagers();
		};
		EndCurrentMoonButton.Clicked += delegate
		{
			EndCurrentMoon();
		};
		StartCitiesRunButton.Clicked += delegate
		{
			StartCitiesRun();
		};
		UnlockBoostersButton.Clicked += delegate
		{
			UnlockBoosters();
		};
		UnlockIdeasButton.Clicked += delegate
		{
			UnlockIdeas();
		};
		UnlockBaseGameIdeasButton.Clicked += delegate
		{
			UnlockBaseGameIdeas();
		};
		UnlockQuestsButton.Clicked += delegate
		{
			UnlockQuests();
		};
		PeacefulModeButton.Clicked += delegate
		{
			TogglePeacefulMode();
		};
		NoFoodButton.Clicked += delegate
		{
			ToggleNoFood();
		};
		NoEnergyButton.Clicked += delegate
		{
			ToggleNoEnergy();
		};
		CoinChestButton.Clicked += delegate
		{
			SpawnFullCoinChest();
		};
		ResetRunButton.Clicked += delegate
		{
			ResetRunVariables();
		};
		SpawnEnemiesButton.Clicked += delegate
		{
			SpawnEnemies();
		};
		DemonScenarioButton.Clicked += delegate
		{
			ScenarioDemon();
		};
		KrakenScenarioButton.Clicked += delegate
		{
			ScenarioKraken();
		};
		IslandScenarioButton.Clicked += delegate
		{
			ScenarioIsland();
		};
		DemonLordScenarioButton.Clicked += delegate
		{
			ScenarioDemonLord();
		};
		WitchForestButton.Clicked += delegate
		{
			ScenarioWitchForest();
		};
		foreach (Type statusEffect in (from type in typeof(StatusEffect).Assembly.GetTypes()
			where type.IsSubclassOf(typeof(StatusEffect))
			select type).ToList())
		{
			StatusEffect statusEffect2 = Activator.CreateInstance(statusEffect) as StatusEffect;
			CustomButton customButton = UnityEngine.Object.Instantiate(PrefabManager.instance.DebugButtonPrefab);
			customButton.TextMeshPro.text = statusEffect2.Name;
			customButton.transform.SetParentClean(StatusEffectButtonParent);
			customButton.Clicked += delegate
			{
				SelectedStatusEffect = Activator.CreateInstance(statusEffect) as StatusEffect;
			};
		}
		foreach (ScriptableCutscene cutscenes in WorldManager.instance.GameDataLoader.Cutscenes)
		{
			CustomButton customButton2 = UnityEngine.Object.Instantiate(PrefabManager.instance.DebugButtonPrefab);
			customButton2.TextMeshPro.text = cutscenes.CutsceneId;
			customButton2.transform.SetParentClean(CutsceneButtonParent);
			customButton2.Clicked += delegate
			{
				WorldManager.instance.QueueCutscene(cutscenes);
			};
		}
		SaveButton.Clicked += delegate
		{
			SaveManager.instance.CreateDebugSaveWithId(DateTime.Now.ToString("dd_MM_HHmm_ss"));
			UpdateSaveElements();
		};
		OpenSavesDirectoryButton.Clicked += delegate
		{
			SaveManager.OpenSavesDirectory();
		};
		PresetSaveButton.Clicked += delegate
		{
			SavePreset();
			UpdatePresetsElements();
		};
		OpenPresetsDirectoryButton.Clicked += delegate
		{
			openPresetsDirectory();
		};
	}

	public void SavePreset()
	{
		SavedPreset savedPreset = new SavedPreset();
		savedPreset.SaveId = WorldManager.instance.CurrentBoard.Id + "_" + DateTime.Now.ToString("dd_MM_HHmm_ss");
		savedPreset.SavedCards = new List<SavedCard>();
		foreach (GameCard item in WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id))
		{
			savedPreset.SavedCards.Add(item.ToSavedCard());
		}
		WorldManager.instance.SavePreset(savedPreset);
	}

	private void openPresetsDirectory()
	{
		string text = Application.dataPath + "/PresetSaves";
		text = text.Replace("/", "\\");
		Process.Start("explorer.exe", text);
	}

	public void SpawnEnemies()
	{
		foreach (CardIdWithEquipment item in SpawnHelper.GetEnemiesToSpawn(new List<SetCardBagType> { SetCardBagType.BasicEnemy }, 50f))
		{
			Combatable obj = WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), item) as Combatable;
			obj.HealthPoints = obj.ProcessedCombatStats.MaxHealth;
			obj.MyGameCard.SendIt();
		}
	}

	public void ResetRunVariables()
	{
		WorldManager.instance.CurrentRunVariables.VisitedForest = false;
		WorldManager.instance.CurrentRunVariables.VisitedIsland = false;
		WorldManager.instance.CurrentRunVariables.ForestWave = 1;
	}

	public void ToggleEndlessMoon()
	{
		WorldManager.instance.DebugEndlessMoonEnabled = !WorldManager.instance.DebugEndlessMoonEnabled;
	}

	public void ToggleNeedVillagers()
	{
		WorldManager.instance.DebugDontNeedVillagers = !WorldManager.instance.DebugDontNeedVillagers;
	}

	public void ToggleNoEnergy()
	{
		WorldManager.instance.DebugNoEnergyEnabled = !WorldManager.instance.DebugNoEnergyEnabled;
	}

	public void EndCurrentMoon()
	{
		WorldManager.instance.MonthTimer = WorldManager.instance.MonthTime - 1f;
	}

	public void StartCitiesRun()
	{
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "villager");
		WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "event_industrial_revolution").MyGameCard.SetChild(cardData.MyGameCard);
	}

	public void UnlockBoosters()
	{
	}

	public void UnlockIdeas()
	{
		WorldManager.instance.DebugUnlockIdeas(justBasegame: false);
		GameScreen.instance.UpdateIdeasLog();
	}

	public void UnlockBaseGameIdeas()
	{
		WorldManager.instance.DebugUnlockIdeas(justBasegame: true);
		GameScreen.instance.UpdateIdeasLog();
	}

	public void UnlockQuests()
	{
		QuestManager.instance.DebugUnlockAllQuests();
	}

	public void TogglePeacefulMode()
	{
		WorldManager.instance.CurrentRunOptions.IsPeacefulMode = !WorldManager.instance.CurrentRunOptions.IsPeacefulMode;
	}

	public void ToggleNoFood()
	{
		WorldManager.instance.DebugNoFoodEnabled = !WorldManager.instance.DebugNoFoodEnabled;
	}

	public void SpawnFullCoinChest()
	{
		if (WorldManager.instance.CurrentBoard.BoardOptions.UsesShells)
		{
			(WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "shell_chest", faceUp: true, checkAddToStack: false) as Chest).CoinCount = 100;
		}
		else
		{
			(WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "coin_chest", faceUp: true, checkAddToStack: false) as Chest).CoinCount = 100;
		}
	}

	public void ScenarioDemon()
	{
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "goblet", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "temple", faceUp: true, checkAddToStack: false);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public void ScenarioKraken()
	{
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "sacred_key", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "sacred_chest", faceUp: true, checkAddToStack: false);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public void ScenarioIsland()
	{
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "villager", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "rowboat", faceUp: true, checkAddToStack: false);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public void ScenarioDemonLord()
	{
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "island_relic", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "cathedral", faceUp: true, checkAddToStack: false);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public void ScenarioWitchForest()
	{
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "stable_portal", faceUp: true, checkAddToStack: false);
		CardData cardData2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "villager", faceUp: true, checkAddToStack: false);
		CardData cardData3 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "sword", faceUp: true, checkAddToStack: false);
		cardData2.MyGameCard.SendIt();
		cardData.MyGameCard.SendIt();
		WorldManager.instance.StackSendTo(cardData3.MyGameCard, cardData2.MyGameCard);
		WorldManager.instance.StackSendTo(cardData.MyGameCard, cardData2.MyGameCard);
	}

	public static string YesNo(bool a)
	{
		if (!a)
		{
			return SokLoc.Translate("label_off");
		}
		return SokLoc.Translate("label_on");
	}

	private void UpdateSaveElements()
	{
		foreach (Transform item in SavesParent)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (FileInfo file in SaveManager.GetDebugFiles())
		{
			CustomButton cb = UnityEngine.Object.Instantiate(PrefabManager.instance.ButtonPrefab);
			cb.HardSetText(file.Name.Replace("_", " ").Replace(".sav", ""));
			cb.TextMeshPro.fontSize = 20f;
			cb.Clicked += delegate
			{
				if (cb.WasRightClick)
				{
					FileHelper.ArchiveFile(file.FullName);
					UpdateSaveElements();
				}
				else
				{
					SaveManager.ForceReload(SaveManager.GetSaveFromFileInfo(file));
					WorldManager.RestartGame();
				}
			};
			cb.transform.SetParentClean(SavesParent);
		}
	}

	private void UpdatePresetsElements()
	{
		foreach (Transform item in PresetsParent)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (SavedPreset savedPreset in GetSavedPresets())
		{
			CustomButton customButton = UnityEngine.Object.Instantiate(PrefabManager.instance.ButtonPrefab);
			customButton.HardSetText(savedPreset.SaveId.Replace("_", " "));
			customButton.TextMeshPro.fontSize = 20f;
			customButton.Clicked += delegate
			{
			};
			customButton.transform.SetParentClean(PresetsParent);
		}
	}

	private List<SavedPreset> GetSavedPresets()
	{
		List<SavedPreset> list = new List<SavedPreset>();
		foreach (FileInfo presetFile in GetPresetFiles())
		{
			if (!(presetFile.Extension == ".meta"))
			{
				SavedPreset savedPreset = JsonUtility.FromJson<SavedPreset>(File.ReadAllText(presetFile.FullName));
				savedPreset.FullPath = presetFile.FullName;
				list.Add(savedPreset);
			}
		}
		return list;
	}

	public void AutoSave()
	{
		SaveGame currentSave = WorldManager.instance.CurrentSave;
		string saveId = currentSave.SaveId;
		string arg = DateTime.Now.ToString("dd_MM_HHmm");
		currentSave.SaveId = $"auto_{arg}_moon_{WorldManager.instance.CurrentMonth}";
		currentSave.LastPlayedRound = WorldManager.instance.GetSaveRound();
		string content = JsonUtility.ToJson(currentSave);
		FileHelper.SaveFile(currentSave.SaveId, content, "AutoSave");
		UnityEngine.Debug.Log("Auto saved! (" + currentSave.SaveId + ")");
		currentSave.SaveId = saveId;
		UpdateSaveElements();
	}

	private static List<FileInfo> GetPresetFiles()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/PresetSaves");
		List<FileInfo> list = new List<FileInfo>();
		list.AddRange(directoryInfo.GetFiles());
		List<FileInfo> list2 = list.OrderBy((FileInfo x) => x.Name).ToList();
		List<FileInfo> list3 = new List<FileInfo>();
		foreach (FileInfo item in list2)
		{
			if (item.Name.StartsWith("preset"))
			{
				list3.Add(item);
			}
		}
		return list3;
	}
}
