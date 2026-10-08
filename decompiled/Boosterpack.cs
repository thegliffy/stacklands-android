using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Boosterpack : Draggable
{
	public BoosterpackData PackData;

	public MeshRenderer PackRenderer;

	private MaterialPropertyBlock propBlock;

	public TextMeshPro BoosterText;

	public TextMeshPro CardCountText;

	protected List<MaterialChanger> materialChangers = new List<MaterialChanger>();

	private Vector3 startScale;

	public int TotalCardsInPack;

	private CardId spawnedEvent;

	[HideInInspector]
	public int TimesOpened;

	[HideInInspector]
	public bool WasClicked;

	public string Name
	{
		get
		{
			if (!string.IsNullOrEmpty(PackData.nameOverride))
			{
				return PackData.nameOverride;
			}
			return SokLoc.Translate(PackData.NameTerm);
		}
	}

	protected override bool HasPhysics => true;

	public string BoosterId => PackData.BoosterId;

	public bool IsIntroPack => PackData.IsIntroPack;

	public int Cost => PackData.Cost;

	public Location BoosterLocation => PackData.BoosterLocation;

	public List<CardBag> CardBags => PackData.CardBags;

	protected override float Mass => 10f;

	public override bool CanBeDragged()
	{
		if (IsIntroPack)
		{
			return false;
		}
		return base.CanBeDragged();
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		GetComponentsInChildren(includeInactive: true, materialChangers);
		MaterialChanger component = GetComponent<MaterialChanger>();
		if (component != null)
		{
			materialChangers.Add(component);
		}
		foreach (MaterialChanger materialChanger in materialChangers)
		{
			materialChanger.Init();
		}
		startScale = base.transform.localScale;
		propBlock = new MaterialPropertyBlock();
		if (!WorldManager.instance.AllBoosters.Contains(this))
		{
			WorldManager.instance.AllBoosters.Add(this);
		}
		base.Start();
	}

	protected override void OnDestroy()
	{
		WorldManager.instance.AllBoosters.Remove(this);
		base.OnDestroy();
	}

	public override bool CanBePushedBy(Draggable draggable)
	{
		if (draggable is GameCard)
		{
			return false;
		}
		return base.CanBePushedBy(draggable);
	}

	protected override void Update()
	{
		PackRenderer.GetPropertyBlock(propBlock, 1);
		if (PackData.Icon != null)
		{
			propBlock.SetTexture("_IconTex", PackData.Icon.texture);
		}
		else
		{
			propBlock.SetTexture("_IconTex", SpriteManager.instance.EmptyTexture.texture);
		}
		PackRenderer.SetPropertyBlock(propBlock, 1);
		BoosterText.text = Name;
		int num = PackData.CardBags.Sum((CardBag x) => x.CardsInPack);
		CardCountText.text = num.ToString();
		if (num == 0)
		{
			CardCountText.transform.parent.gameObject.SetActive(value: false);
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, startScale, Time.deltaTime * 16f);
		base.Update();
	}

	private Vector3 GetCardVelocity()
	{
		if (TotalCardsInPack == 1)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle.normalized * 4f;
			Velocity = new Vector3(vector.x, 6f, vector.y);
		}
		int num = TotalCardsInPack - PackData.CardBags.Sum((CardBag x) => x.CardsInPack);
		int totalCardsInPack = TotalCardsInPack;
		float f = (float)num / (float)totalCardsInPack * 360f * (MathF.PI / 180f);
		return new Vector3(Mathf.Cos(f) * 4.5f, 6f, Mathf.Sin(f) * 4.5f);
	}

	public override void Clicked()
	{
		CardBag currentCardBag = PackData.CardBags.FirstOrDefault((CardBag x) => x.CardsInPack > 0);
		if (currentCardBag == null)
		{
			return;
		}
		WasClicked = true;
		ICardId cardId = currentCardBag.GetCard();
		int timesBoosterWasBoughtOnLocation = WorldManager.instance.GetTimesBoosterWasBoughtOnLocation(BoosterLocation);
		GameCamera.instance.Screenshake = 0.3f;
		if (BoosterLocation == Location.Mainland && timesBoosterWasBoughtOnLocation == 1)
		{
			if (currentCardBag.CardsInPack == 1)
			{
				cardId = (CardId)"berrybush";
			}
			else if (currentCardBag.CardsInPack == 0)
			{
				cardId = (CardId)"tree";
			}
		}
		if (BoosterId == "cities_weather" && TimesOpened == TotalCardsInPack - 1)
		{
			if (CitiesManager.instance.ShouldTriggerEvent())
			{
				cardId = (spawnedEvent = CitiesManager.instance.GetEvent());
			}
		}
		else
		{
			int cardCount = WorldManager.instance.GetCardCount<Worker>();
			if (BoosterLocation == Location.Cities && cardCount <= 1 && ((2 - cardCount >= 0) ? (2 - cardCount) : 0) - TimesOpened >= 0)
			{
				cardId = (CardId)"worker";
			}
		}
		if (WorldManager.instance.GetBoardWithLocation(BoosterLocation).BoardOptions.NewVillagerSpawnsFromPack)
		{
			int cardCount2 = WorldManager.instance.GetCardCount((BaseVillager x) => x.CanBreed);
			cardCount2 += WorldManager.instance.GetCardCount<TeenageVillager>();
			if (!IsIntroPack && (timesBoosterWasBoughtOnLocation == 7 || (timesBoosterWasBoughtOnLocation > 7 && timesBoosterWasBoughtOnLocation % 5 == 0)) && currentCardBag.CardsInPack == 0 && cardCount2 <= 1)
			{
				cardId = (CardId)"villager";
			}
			if (MyBoard.BoardOptions.CanSpawnCombatIntro && timesBoosterWasBoughtOnLocation >= 10 && !WorldManager.instance.CurrentSave.FoundBoosterIds.Contains("combat_intro") && currentCardBag.CardsInPack == 0)
			{
				WorldManager.instance.CreateBoosterpack(base.transform.position, "combat_intro").SendIt();
			}
		}
		CardData cardData = WorldManager.instance.CreateCard(base.transform.position, cardId, faceUp: false, checkAddToStack: false);
		if (cardData == null)
		{
			Debug.LogError($"CardData is null after creating card with id '{cardId}'");
		}
		cardData.MyGameCard.RotWobble(1f);
		cardData.MyGameCard.Velocity = GetCardVelocity();
		AudioManager.me.PlaySound2D(AudioManager.me.OpenBooster, UnityEngine.Random.Range(0.9f, 1.1f), 0.3f);
		WorldManager.instance.GivenCards.Add(cardData.Id);
		if (currentCardBag.CardBagType != CardBagType.SetPack && UnityEngine.Random.value <= 0.01f)
		{
			cardData.SetFoil();
		}
		TimesOpened++;
		SetHitEffect(delegate
		{
			if (currentCardBag.CardsInPack <= 0 && currentCardBag == PackData.CardBags[PackData.CardBags.Count - 1])
			{
				WorldManager.instance.CreateSmoke(base.transform.position);
				UnityEngine.Object.Destroy(base.gameObject);
				QuestManager.instance.SpecialActionComplete(PackData.BoosterId + "_opened");
			}
		});
		if (TimesOpened == TotalCardsInPack)
		{
			WorldManager.instance.OnBoosterOpened(PackData.BoosterId);
			if (spawnedEvent != null)
			{
				CardData cardFromId = WorldManager.instance.GameDataLoader.GetCardFromId(spawnedEvent.Id);
				if (cardFromId.MyCardType == CardType.Disaster)
				{
					WorldManager.instance.QueueCutsceneIfNotPlayed("cities_disaster");
				}
				else if (cardFromId.Id == "ufo_event")
				{
					WorldManager.instance.QueueCutscene("cities_event_ufo");
				}
				spawnedEvent = null;
			}
		}
		base.transform.localScale *= 1.2f;
		base.Clicked();
	}

	protected void SetHitEffect(Action after)
	{
		foreach (MaterialChanger mc in materialChangers)
		{
			if (mc != null)
			{
				mc.SetMaterial(WorldManager.instance.HitMaterial);
				StartCoroutine(WaitFor(0.1f, delegate
				{
					mc.ResetMaterials();
				}));
			}
		}
		StartCoroutine(WaitFor(0.1f, delegate
		{
			after?.Invoke();
		}));
	}

	private IEnumerator WaitFor(float time, Action a)
	{
		yield return new WaitForSeconds(time);
		a?.Invoke();
	}
}
