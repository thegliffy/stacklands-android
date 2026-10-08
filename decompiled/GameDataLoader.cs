using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GameDataLoader
{
	public static GameDataLoader instance;

	public List<BoosterpackData> BoosterpackDatas;

	public List<CardData> CardDataPrefabs;

	public List<Blueprint> BlueprintPrefabs;

	public List<Demand> Demands;

	public List<ScriptableCutscene> Cutscenes;

	private Dictionary<string, CardData> idToCard = new Dictionary<string, CardData>();

	private Dictionary<string, BoosterpackData> idToBooster = new Dictionary<string, BoosterpackData>();

	public List<SetCardBagData> SetCardBags = new List<SetCardBagData>();

	public string[] VillagerNames;

	private static string[] codeReferencedCards;

	private static EnemySetCardBag[] codeReferencedEnemySetCardBags = new EnemySetCardBag[3]
	{
		EnemySetCardBag.BasicEnemy,
		EnemySetCardBag.Forest_AdvancedEnemy,
		EnemySetCardBag.Forest_BasicEnemy
	};

	private List<ICardReference> cardReferences;

	private HashSet<string> cardReferenceKeys;

	public bool SpiritDlcLoaded;

	public bool CitiesDlcLoaded;

	public ProfanityChecker ProfanityChecker;

	internal Mod CurrentlyLoadingMod;

	public GameDataLoader(bool loadSpiritDlc = true, bool loadCitiesDlc = true)
	{
		instance = this;
		AssetBundle.UnloadAllAssetBundles(unloadAllObjects: true);
		List<CardData> list = Resources.LoadAll<CardData>("Cards/Main_Cards").ToList();
		List<CardData> list2 = Resources.LoadAll<CardData>("Cards/Island_Cards").ToList();
		List<CardData> list3 = Resources.LoadAll<CardData>("Cards/Forest_Cards").ToList();
		List<CardData> list4 = Resources.LoadAll<CardData>("Cards/Order_Cards").ToList();
		foreach (CardData item in list)
		{
			item.CardUpdateType = CardUpdateType.Main;
		}
		foreach (CardData item2 in list2)
		{
			item2.CardUpdateType = CardUpdateType.Island;
		}
		foreach (CardData item3 in list3)
		{
			item3.CardUpdateType = CardUpdateType.Forest;
		}
		foreach (CardData item4 in list4)
		{
			item4.CardUpdateType = CardUpdateType.Order;
		}
		CardDataPrefabs = list;
		CardDataPrefabs.AddRange(list2);
		CardDataPrefabs.AddRange(list3);
		CardDataPrefabs.AddRange(list4);
		if (Application.isPlaying && PlatformHelper.HasModdingSupport)
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			{
				Converters = new List<JsonConverter>
				{
					new StringColorConverter(),
					new StringEnumConverter(),
					new StringSpriteConverter()
				}
			};
			try
			{
				CardDataPrefabs.AddRange(LoadModCards());
				CardDataPrefabs.AddRange(LoadModBlueprints());
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		BoosterpackDatas = Resources.LoadAll<BoosterpackData>("Boosters").ToList();
		if (Application.isPlaying && PlatformHelper.HasModdingSupport)
		{
			try
			{
				BoosterpackDatas.AddRange(LoadModBoosters());
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
			}
		}
		Demands = Resources.LoadAll<Demand>("Greed_Demands").ToList();
		codeReferencedCards = Resources.Load<TextAsset>("Misc/CodeReferencedCards").text.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		TextAsset textAsset = Resources.Load<TextAsset>("Misc/VillagerNames");
		VillagerNames = textAsset.text.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		SetCardBags = new List<SetCardBagData>();
		SetCardBags.AddRange(Resources.LoadAll<SetCardBagData>("SetCardBags"));
		Cutscenes = Resources.LoadAll<ScriptableCutscene>("Cutscenes").ToList();
		if (loadSpiritDlc)
		{
			SpiritDlcLoaded = TryLoadDlc("Spirits_DLC", CardUpdateType.Spirit);
		}
		if (loadCitiesDlc)
		{
			CitiesDlcLoaded = TryLoadDlc("Cities_DLC", CardUpdateType.Cities);
		}
		InitializeLoadedData();
		ProfanityChecker = new ProfanityChecker();
		CurrentlyLoadingMod = null;
	}

	private bool TryLoadDlc(string folderPath, CardUpdateType updateType)
	{
		ISokBundle sokBundle2;
		if (!Application.isEditor)
		{
			ISokBundle sokBundle = new RuntimeSokBundle();
			sokBundle2 = sokBundle;
		}
		else
		{
			ISokBundle sokBundle = new EditorSokBundle();
			sokBundle2 = sokBundle;
		}
		ISokBundle sokBundle3 = sokBundle2;
		if (!sokBundle3.Load(folderPath))
		{
			Debug.LogError("Failed loading the " + folderPath + " bundle");
			return false;
		}
		foreach (GameObject item in sokBundle3.LoadAssets<GameObject>())
		{
			CardData component = item.GetComponent<CardData>();
			if (component != null)
			{
				component.CardUpdateType = updateType;
				CardDataPrefabs.Add(component);
			}
		}
		SetCardBags.AddRange(sokBundle3.LoadAssets<SetCardBagData>());
		BoosterpackDatas.AddRange(sokBundle3.LoadAssets<BoosterpackData>());
		return true;
	}

	private void InitializeLoadedData()
	{
		foreach (CardData cardDataPrefab in CardDataPrefabs)
		{
			if (idToCard.ContainsKey(cardDataPrefab.Id))
			{
				Debug.LogError("Duplicate card id! " + cardDataPrefab.Id + " - " + cardDataPrefab.gameObject.name + " & " + idToCard[cardDataPrefab.Id].gameObject.name);
			}
			else
			{
				idToCard[cardDataPrefab.Id] = cardDataPrefab;
			}
		}
		BoosterpackDatas = BoosterpackDatas.OrderBy((BoosterpackData x) => x.MinAchievementCount).ToList();
		foreach (BoosterpackData boosterpackData in BoosterpackDatas)
		{
			idToBooster[boosterpackData.BoosterId] = boosterpackData;
		}
		BlueprintPrefabs = CardDataPrefabs.OfType<Blueprint>().ToList();
		foreach (Blueprint blueprintPrefab in BlueprintPrefabs)
		{
			blueprintPrefab.Init(this);
		}
	}

	public List<CardData> LoadModCards()
	{
		List<CardData> list = new List<CardData>();
		foreach (Mod loadedMod in ModManager.LoadedMods)
		{
			Mod mod = (CurrentlyLoadingMod = loadedMod);
			string text = Path.Combine(mod.Path, "Cards");
			if (!Directory.Exists(text))
			{
				continue;
			}
			FileInfo[] files = new DirectoryInfo(text).GetFiles("*.json", SearchOption.AllDirectories);
			foreach (FileInfo fileInfo in files)
			{
				string value = File.ReadAllText(Path.Combine(text, fileInfo.Name));
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						ModCard modCard = JsonConvert.DeserializeObject<ModCard>(value);
						Debug.Log("loading modded card: " + modCard.Id);
						CardData cardData = LoadModCard(modCard, mod);
						cardData.gameObject.name = EnumHelper.GetName<CardType>((int)cardData.MyCardType) + "_" + cardData.Id;
						list.Add(cardData);
					}
					catch (Exception ex)
					{
						Debug.LogError("Failed to load card: " + ex.Message);
					}
				}
			}
		}
		foreach (CardData item in list)
		{
			item.CardUpdateType = CardUpdateType.Mod;
		}
		return list;
	}

	public CardData LoadModCard(ModCard mc, Mod mod)
	{
		GameObject gameObject = new GameObject("testcard");
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		gameObject.SetActive(value: false);
		if (!ModManager.CardClasses.ContainsKey(mc.Script))
		{
			throw new Exception("invalid script: " + mc.Script);
		}
		CardData cd = gameObject.AddComponent(ModManager.CardClasses[mc.Script]) as CardData;
		cd.Id = mc.Id;
		cd.NameTerm = mc.NameTerm ?? "";
		cd.DescriptionTerm = mc.DescriptionTerm ?? "";
		cd.nameOverride = mc.NameOverride;
		cd.descriptionOverride = mc.DescriptionOverride;
		cd.Value = mc.Value;
		cd.MyCardType = EnumHelper.ParseEnum<CardType>(mc.Type, 0);
		cd.HideFromCardopedia = mc.HideFromCardopedia;
		if (!string.IsNullOrEmpty(mc.Icon) && !mc.Icon.EndsWith(".png"))
		{
			cd.Icon = CardDataPrefabs.Find((CardData c) => c.Id == mc.Icon)?.Icon;
			if (cd.Icon == null)
			{
				Debug.LogWarning("Tried to find vanilla card " + mc.Icon + " for icon, but it does not exist! Did you mean " + mc.Icon + ".png?");
			}
		}
		else
		{
			string path = Path.Combine(mod.Path, "Icons", mc.Icon ?? (mc.Id + ".png"));
			if (File.Exists(path))
			{
				cd.Icon = ResourceHelper.LoadSpriteFromPath(path);
			}
			else
			{
				Debug.LogWarning("Missing or invalid Icon for " + cd.Id + "!");
			}
		}
		string path2 = Path.Combine(mod.Path, "Sounds", mc.PickupSound ?? (mc.Id + ".wav"));
		if (File.Exists(path2))
		{
			cd.PickupSoundGroup = PickupSoundGroup.Custom;
			ModManager.instance.StartCoroutine(ResourceHelper.LoadAudioClipFromPath(path2, delegate(AudioClip ac)
			{
				Debug.Log("Loaded audio for " + cd.Id);
				cd.PickupSound = ac;
			}));
		}
		else
		{
			Debug.LogWarning("Missing or invalid PickupSound for " + cd.Id + "!");
		}
		if (mc.ExtraProps != null)
		{
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(null);
			foreach (KeyValuePair<string, JToken> extraProp in mc.ExtraProps)
			{
				if (extraProp.Key.StartsWith("_"))
				{
					string text = extraProp.Key.TrimStart('_');
					FieldInfo field = cd.GetType().GetField(text);
					if (field != null)
					{
						field.SetValue(cd, jsonSerializer.Deserialize(extraProp.Value.CreateReader(), field.FieldType));
					}
					else
					{
						Debug.LogError($"Property {text} doesn't exist on {cd.GetType()}");
					}
				}
			}
		}
		return cd;
	}

	public List<CardData> LoadModBlueprints()
	{
		List<CardData> list = new List<CardData>();
		foreach (Mod loadedMod in ModManager.LoadedMods)
		{
			Mod mod = (CurrentlyLoadingMod = loadedMod);
			string text = Path.Combine(mod.Path, "Blueprints");
			if (!Directory.Exists(text))
			{
				continue;
			}
			FileInfo[] files = new DirectoryInfo(text).GetFiles("*.json", SearchOption.AllDirectories);
			foreach (FileInfo fileInfo in files)
			{
				string value = File.ReadAllText(Path.Combine(text, fileInfo.Name));
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						ModBlueprint modBlueprint = JsonConvert.DeserializeObject<ModBlueprint>(value);
						Debug.Log("loading modded blueprint: " + modBlueprint.Id);
						CardData cardData = LoadModBlueprint(modBlueprint, mod);
						cardData.gameObject.name = "Blueprint_" + cardData.Id;
						list.Add(cardData);
					}
					catch (Exception ex)
					{
						Debug.LogError("Failed to load blueprint: " + ex.Message + "\n" + ex.StackTrace);
					}
				}
			}
		}
		foreach (CardData item in list)
		{
			item.CardUpdateType = CardUpdateType.Mod;
		}
		return list;
	}

	public CardData LoadModBlueprint(ModBlueprint mp, Mod mod)
	{
		GameObject gameObject = new GameObject("testblueprint");
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		gameObject.SetActive(value: false);
		Blueprint blueprint = gameObject.AddComponent(ModManager.CardClasses[mp.Script]) as Blueprint;
		blueprint.Id = mp.Id;
		blueprint.NameTerm = mp.NameTerm ?? "";
		blueprint.nameOverride = mp.NameOverride;
		blueprint.BlueprintGroup = EnumHelper.ParseEnum<BlueprintGroup>(mp.Group, 0);
		blueprint.Value = mp.Value;
		blueprint.HideFromCardopedia = mp.HideFromCardopedia;
		blueprint.HideFromIdeasTab = mp.HideFromIdeasTab;
		blueprint.IsInvention = mp.IsInvention;
		blueprint.NeedsExactMatch = mp.NeedsExactMatch;
		blueprint.MyCardType = CardType.Ideas;
		if (!string.IsNullOrEmpty(mp.Icon) && !mp.Icon.EndsWith(".png"))
		{
			blueprint.Icon = CardDataPrefabs.Find((CardData c) => c.Id == mp.Icon)?.Icon;
			if (blueprint.Icon == null)
			{
				Debug.LogWarning("Tried to find vanilla card " + mp.Icon + " for icon, but it does not exist! Did you mean " + mp.Icon + ".png?");
			}
		}
		else
		{
			string path = Path.Combine(mod.Path, "Icons", mp.Icon ?? (mp.Id + ".png"));
			if (File.Exists(path))
			{
				blueprint.Icon = ResourceHelper.LoadSpriteFromPath(path);
			}
		}
		foreach (ModSubprint subprint2 in mp.Subprints)
		{
			Subprint subprint = new Subprint();
			subprint.RequiredCards = (from str in subprint2.RequiredCards.Split(',')
				select str.Trim()).ToArray();
			subprint.CardsToRemove = (from str in subprint2.CardsToRemove?.Split(',')
				select str.Trim()).ToArray();
			subprint.ResultCard = subprint2.ResultCard;
			subprint.ResultAction = subprint2.ResultAction;
			subprint.ExtraResultCards = (from str in subprint2.ExtraResultCards?.Split(',')
				select str.Trim()).ToArray() ?? new string[0];
			subprint.Time = subprint2.Time;
			subprint.StatusTerm = subprint2.StatusTerm ?? "";
			subprint.statusOverride = subprint2.StatusOverride;
			blueprint.Subprints.Add(subprint);
		}
		return blueprint;
	}

	public List<BoosterpackData> LoadModBoosters()
	{
		List<BoosterpackData> list = new List<BoosterpackData>();
		if (ModManager.LoadedMods == null)
		{
			return list;
		}
		foreach (Mod loadedMod in ModManager.LoadedMods)
		{
			Mod mod = (CurrentlyLoadingMod = loadedMod);
			string text = Path.Combine(mod.Path, "Boosterpacks");
			if (!Directory.Exists(text))
			{
				continue;
			}
			FileInfo[] files = new DirectoryInfo(text).GetFiles("*.json", SearchOption.AllDirectories);
			foreach (FileInfo fileInfo in files)
			{
				string value = File.ReadAllText(Path.Combine(text, fileInfo.Name));
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						ModBoosterpack modBoosterpack = JsonConvert.DeserializeObject<ModBoosterpack>(value);
						Debug.Log("loading modded boosterpack: " + modBoosterpack.Id);
						BoosterpackData item = LoadModBooster(modBoosterpack, mod);
						list.Add(item);
					}
					catch (Exception ex)
					{
						Debug.LogError("Failed to load pack: " + ex.Message);
					}
				}
			}
		}
		return list;
	}

	public BoosterpackData LoadModBooster(ModBoosterpack mp, Mod mod)
	{
		BoosterpackData boosterpackData = ScriptableObject.CreateInstance<BoosterpackData>();
		boosterpackData.hideFlags = HideFlags.HideAndDontSave;
		boosterpackData.name = "modbooster";
		boosterpackData.BoosterId = mp.Id;
		boosterpackData.NameTerm = mp.NameTerm ?? "";
		boosterpackData.nameOverride = mp.NameOverride;
		boosterpackData.MinAchievementCount = mp.MinQuestCount;
		boosterpackData.Cost = mp.Cost;
		boosterpackData.BoosterLocation = EnumHelper.ParseEnum<Location>(mp.Location, 0);
		if (!string.IsNullOrEmpty(mp.Icon) && !mp.Icon.EndsWith(".png"))
		{
			boosterpackData.Icon = BoosterpackDatas.Find((BoosterpackData c) => c.BoosterId == mp.Icon)?.Icon;
			if (boosterpackData.Icon == null)
			{
				Debug.LogWarning("Tried to find vanilla booster " + mp.Icon + " for icon, but it does not exist! Did you mean " + mp.Icon + ".png?");
			}
		}
		else
		{
			string path = Path.Combine(mod.Path, "Icons", mp.Icon ?? (mp.Id + ".png"));
			if (File.Exists(path))
			{
				boosterpackData.Icon = ResourceHelper.LoadSpriteFromPath(path);
			}
			else
			{
				Debug.LogWarning("Missing or invalid Icon for " + boosterpackData.BoosterId + "!");
			}
		}
		boosterpackData.CardBags = new List<CardBag>();
		foreach (ModCardBag cardBag2 in mp.CardBags)
		{
			CardBag cardBag = new CardBag();
			cardBag.CardBagType = EnumHelper.ParseEnum<CardBagType>(cardBag2.CardBagType, 0);
			cardBag.CardsInPack = cardBag2.CardsInPack;
			if (cardBag.CardBagType == CardBagType.Chances)
			{
				cardBag.Chances = cardBag2.Chances;
			}
			if (cardBag.CardBagType == CardBagType.SetPack)
			{
				cardBag.SetPackCards = cardBag2.SetPackCards;
			}
			if (cardBag.CardBagType == CardBagType.SetCardBag)
			{
				cardBag.SetCardBag = EnumHelper.ParseEnum<SetCardBagType>(cardBag2.SetCardBag, 0);
				cardBag.UseFallbackBag = cardBag2.UseFallbackBag;
				if (cardBag.UseFallbackBag)
				{
					cardBag.FallbackBag = EnumHelper.ParseEnum<SetCardBagType>(cardBag2.FallbackBag, 0);
				}
			}
			if (cardBag.CardBagType == CardBagType.Enemies)
			{
				cardBag.EnemyCardBag = EnumHelper.ParseEnum<EnemySetCardBag>(cardBag2.EnemyCardBag, 0);
				cardBag.StrengthLevel = cardBag2.StrengthLevel;
			}
			boosterpackData.CardBags.Add(cardBag);
		}
		return boosterpackData;
	}

	public CardData GetCardFromId(string cardId, bool throwError = true)
	{
		if (string.IsNullOrEmpty(cardId))
		{
			return null;
		}
		switch (cardId)
		{
		case "any_villager_old":
			return idToCard["old_villager"];
		case "any_villager_young":
			return idToCard["teenage_villager"];
		case "any_villager":
		case "breedable_villager":
			return idToCard["villager"];
		case "any_worker":
			return idToCard["worker"];
		case "any_educated_worker":
			return idToCard["educated_worker"];
		default:
		{
			if (idToCard.TryGetValue(cardId, out var value))
			{
				return value;
			}
			if (throwError)
			{
				Debug.LogError("Could not find card with id '" + cardId + "'");
			}
			return null;
		}
		}
	}

	public void AddCardToSetCardBag(SetCardBagType bagType, string cardId, int chance)
	{
		SetCardBagData setCardBagData = SetCardBags.Find((SetCardBagData s) => s.SetCardBagType == bagType);
		if (setCardBagData == null)
		{
			throw new Exception($"No matching card bag found for {bagType}");
		}
		setCardBagData.Chances.Add(new SimpleCardChance(cardId, chance));
	}

	public SetCardBagType GetSetCardBagForEnemyCardBag(EnemySetCardBag bag)
	{
		string value = bag.ToString();
		string[] names = Enum.GetNames(typeof(SetCardBagType));
		SetCardBagType[] array = (SetCardBagType[])Enum.GetValues(typeof(SetCardBagType));
		int num = Array.IndexOf(names, value);
		if (num == -1)
		{
			throw new Exception($"No matching card bag found for {bag}");
		}
		return array[num];
	}

	public List<SetCardBagType> GetSetCardBagForEnemyCardBagList(List<EnemySetCardBag> bags)
	{
		List<SetCardBagType> list = new List<SetCardBagType>();
		foreach (EnemySetCardBag bag in bags)
		{
			string value = bag.ToString();
			string[] names = Enum.GetNames(typeof(SetCardBagType));
			SetCardBagType[] array = (SetCardBagType[])Enum.GetValues(typeof(SetCardBagType));
			int num = Array.IndexOf(names, value);
			if (num == -1)
			{
				throw new Exception($"No matching card bag found for {bag}");
			}
			list.Add(array[num]);
		}
		return list;
	}

	public BoosterpackData GetBoosterData(string boosterId)
	{
		if (!idToBooster.TryGetValue(boosterId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddReference(ICardReference reference)
	{
		string key = reference.GetKey();
		if (!cardReferenceKeys.Contains(key))
		{
			cardReferences.Add(reference);
			cardReferenceKeys.Add(key);
		}
	}

	public List<ICardReference> DetermineCardReferences()
	{
		if (cardReferences != null)
		{
			return cardReferences;
		}
		cardReferences = new List<ICardReference>();
		cardReferenceKeys = new HashSet<string>();
		string[] array = codeReferencedCards;
		foreach (string cardId in array)
		{
			AddReference(new CardReferenceCode(cardId));
		}
		foreach (CardData cardDataPrefab in CardDataPrefabs)
		{
			if (cardDataPrefab is Equipable equipable)
			{
				AddReference(new CardReferenceCard(equipable.VillagerTypeOverride, cardDataPrefab.Id));
			}
			if (cardDataPrefab is Combatable combatable)
			{
				foreach (Equipable possibleEquipable in combatable.PossibleEquipables)
				{
					AddReference(new CardReferenceCard(possibleEquipable.Id, cardDataPrefab.Id));
					if (possibleEquipable.blueprint != null)
					{
						AddReference(new CardReferenceCard(possibleEquipable.blueprint.Id, cardDataPrefab.Id));
					}
				}
			}
			foreach (string cardsReferencedByCardBag in GetCardsReferencedByCardBags(cardDataPrefab))
			{
				AddReference(new CardReferenceCard(cardsReferencedByCardBag, cardDataPrefab.Id));
			}
		}
		foreach (BoosterpackData boosterpackData in BoosterpackDatas)
		{
			foreach (CardBag cardBag in boosterpackData.CardBags)
			{
				foreach (string item in cardBag.GetCardsInBag(this))
				{
					AddReference(new CardReferenceBooster(item, boosterpackData.BoosterId));
				}
			}
		}
		EnemySetCardBag[] array2 = codeReferencedEnemySetCardBags;
		foreach (EnemySetCardBag bag in array2)
		{
			foreach (CardChance item2 in CardBag.GetChancesForSetCardBag(this, GetSetCardBagForEnemyCardBag(bag)))
			{
				AddReference(new CardReferenceCode(item2.Id));
			}
		}
		foreach (Blueprint blueprintPrefab in BlueprintPrefabs)
		{
			if (blueprintPrefab is BlueprintGrowth)
			{
				continue;
			}
			foreach (Subprint subprint in blueprintPrefab.Subprints)
			{
				foreach (string item3 in subprint.RequiredCards.Distinct())
				{
					string text = item3;
					switch (text)
					{
					case "any_villager":
					case "breedable_villager":
					case "any_villager_old":
					case "any_villager_young":
						text = "villager";
						break;
					default:
						if (text.Split('|').Length != 0)
						{
							text = text.Split('|')[0];
						}
						break;
					}
					_ = text == "villager";
				}
				AddReference(new CardReferenceCard(subprint.ResultCard, blueprintPrefab.Id));
				array = subprint.ExtraResultCards;
				foreach (string referencedCardId in array)
				{
					AddReference(new CardReferenceCard(referencedCardId, blueprintPrefab.Id));
				}
			}
		}
		return cardReferences;
	}

	private List<string> GetCardsReferencedByCardBags(CardData card)
	{
		List<string> list = new List<string>();
		foreach (CardBag cardBag in card.GetCardBags())
		{
			list.AddRange(cardBag.GetCardsInBag(this));
		}
		return list;
	}

	public ScriptableCutscene GetCutsceneWithId(string id)
	{
		return Cutscenes.FirstOrDefault((ScriptableCutscene x) => x.CutsceneId == id);
	}
}
