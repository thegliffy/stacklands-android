using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameCard : Draggable, IGameCardOrCardData
{
	private enum PositionType
	{
		InConflict,
		InAttack,
		InStack,
		IsRoot,
		IsEquipped,
		InAnimation,
		None,
		IsWorking
	}

	public TextMeshPro CardNameText;

	public SpriteRenderer IconRenderer;

	public CardData CardData;

	public GameCard Parent;

	public GameCard Child;

	public GameCard LastParent;

	public Transform HitTextPosition;

	public Transform Visuals;

	public int ConnectorOutputIndex;

	public bool IsEquipped;

	public bool IsWorking;

	public bool ShowInventory;

	public GameCard EquipmentHolder;

	public List<GameCard> EquipmentChildren;

	public GameCard WorkerHolder;

	public List<GameCard> WorkerChildren = new List<GameCard>();

	public GameObject EnergyConnectorPrefab;

	public Transform EnergyConnectorTransform;

	private Vector3 startScale;

	public Renderer CardRenderer;

	public Rectangle HighlightRectangle;

	public SpriteRenderer CoinIcon;

	public TextMeshPro CoinText;

	public TextMeshPro SpecialText;

	public SpriteRenderer SpecialIcon;

	public SpriteRenderer CombatStatusCircle;

	public SpriteRenderer DropShadowRenderer;

	public Transform EquipmentRectangle;

	public Transform WorkerRectangle;

	public WorkerTransformHolder WorkerTransformHolder;

	public InventoryInteractable InventoryInteractable;

	public InventoryInteractable WorkerInventoryInteractable;

	public OnOffInteractable OnOffInteractable;

	private Vector3 onOffBasePosition;

	private Vector3 onOffTargetPosition;

	public SpriteRenderer HeadInventoryIcon;

	public SpriteRenderer TorsoInventoryIcon;

	public SpriteRenderer HandInventoryIcon;

	public SpriteRenderer WorkerInventoryIcon;

	public GameObject HeadEquipmentPosition;

	public GameObject TorsoEquipmentPosition;

	public GameObject HandEquipmentPosition;

	public Rectangle EquipmentButton;

	public Rectangle WorkerButton;

	public int? SpecialValue;

	public bool HighlightActive;

	private Vector3 lastPosition;

	public Vector3 SpawnRotation;

	private bool snappedToParent;

	private MaterialPropertyBlock propBlock;

	private MaterialPropertyBlock combatCirclePropBlock;

	public bool FaceUp;

	public SpriteRenderer NewCircle;

	private Vector3 newCircleStartSize;

	public ParticleSystem FoilParticles;

	protected List<MaterialChanger> materialChangers = new List<MaterialChanger>();

	[HideInInspector]
	public bool IsDemoCard;

	public GameCard BounceTarget;

	[HideInInspector]
	public bool PushEnabled = true;

	[HideInInspector]
	public bool SetY = true;

	[Header("Status")]
	public float DistanceBetweenStatusses = 0.01f;

	[HideInInspector]
	public List<StatusEffectElement> StatusEffectElements = new List<StatusEffectElement>();

	public Vector3 equipmentRectangleStartOffset;

	[HideInInspector]
	public bool ShowSpecialIcon;

	[HideInInspector]
	public bool StackUpdate;

	private CardPalette myCardPalette;

	public List<CardAnimation> CardAnimations = new List<CardAnimation>();

	[HideInInspector]
	private Action closeToTargetPositionCallback;

	[HideInInspector]
	public List<CardConnector> CardConnectorChildren = new List<CardConnector>();

	public Color CombatCircleColor;

	private int propColor = Shader.PropertyToID("_Color");

	private int propColor2 = Shader.PropertyToID("_Color2");

	private int propIconColor = Shader.PropertyToID("_IconColor");

	private int propHasSecondaryIcon = Shader.PropertyToID("_HasSecondaryIcon");

	private int propHasOutputDir = Shader.PropertyToID("_HasOutputDir");

	private int propSecondaryTex = Shader.PropertyToID("_SecondaryTex");

	private int propBigShineStrength = Shader.PropertyToID("_BigShineStrength");

	private int propShineStrength = Shader.PropertyToID("_ShineStrength");

	private int propFoil = Shader.PropertyToID("_Foil");

	private int propDamaged = Shader.PropertyToID("_Damaged");

	private int propIconTex = Shader.PropertyToID("_IconTex");

	[HideInInspector]
	public bool Destroyed;

	private List<GameCard> cardsInvolved = new List<GameCard>();

	public bool WasClicked;

	public bool IsNew;

	public float ZRotOffset;

	private Vector3 onOffVelocity;

	private Vector3 onOffTargetPos;

	private Color colOff = new Color(0f, 0f, 0f, 0.5f);

	private Color colOn = new Color(0f, 0f, 0f, 1f);

	private float ConnectorAmountOffset = 0.077f;

	private float CardTextOffset = 0.01f;

	public Rectangle StatusEffectBackground;

	private Transform statusEffectBackgroundTransform;

	private float statusEffectBackgroundWidth;

	private float flipTimer;

	public float RotWobbleAmp = 1f;

	public float RotWobbleSpeed = 1f;

	public float RotWobbleSpringiness = 1f;

	private float wobbleRotVelo;

	public bool AutoRotWobble;

	public float AutoRotWobbleTimer;

	public float AutoRotWobbleAmount = 0.1f;

	private float timer;

	private float rotWobbleTimer;

	private float curZ = 270f;

	public bool TimerRunning;

	public string Status;

	public float CurrentTimerTime;

	public float TargetTimerTime;

	public TimerAction TimerAction;

	public string TimerBlueprintId;

	public int TimerSubprintIndex;

	public bool SkipCitiesChecks;

	public string TimerActionId;

	public Statusbar CurrentStatusbar;

	[HideInInspector]
	public GameCard removedChild;

	public Transform StatusEffectElementParent;

	private float curHeight;

	[HideInInspector]
	public bool IsHit;

	protected override bool HasPhysics => true;

	public Vector3 Position => base.transform.position;

	public override bool IsHovered => base.IsHovered;

	public static float CardHeight => PrefabManager.instance.GameCardPrefab.GetHeight();

	public bool BeingHovered
	{
		get
		{
			if (WorldManager.instance.HoveredCard == this)
			{
				return true;
			}
			if (IsParentOf(WorldManager.instance.HoveredCard) || IsChildOf(WorldManager.instance.HoveredCard))
			{
				return true;
			}
			return false;
		}
	}

	public override Vector3 AutoMoveSnapPosition
	{
		get
		{
			if (WorldManager.instance != null && WorldManager.instance.DraggingCard != null)
			{
				return CardNameText.transform.position + new Vector3(0f, WorldManager.instance.CardOverlayHeightOffset, 0f - WorldManager.instance.CardOverlayOffset);
			}
			if (Child == null && Parent == null)
			{
				return base.transform.position;
			}
			return CardNameText.transform.position;
		}
	}

	public override bool CanBeAutoMovedTo
	{
		get
		{
			if (WorldManager.instance.DraggingCard != null && Child != null)
			{
				return false;
			}
			if (IsEquipped && (WorldManager.instance.DraggingCard == EquipmentHolder || !EquipmentHolder.ShowInventory))
			{
				return false;
			}
			if (IsWorking && (WorldManager.instance.DraggingCard == WorkerHolder || !WorkerHolder.ShowInventory))
			{
				return false;
			}
			return !BeingDragged;
		}
	}

	public bool InventoryVisible => ShowInventory;

	public bool IsWorkerInventory
	{
		get
		{
			CardData cardData = CardData;
			if ((object)cardData == null)
			{
				return false;
			}
			return cardData.WorkerAmount > 0;
		}
	}

	public bool TimerRunningInStack => GetAllCardsInStack().Any((GameCard x) => x.TimerRunning);

	public bool HasParent => Parent != null;

	public bool HasChild => Child != null;

	protected override float Mass
	{
		get
		{
			float num = 1f;
			if (CardData is Mob)
			{
				num += 50f;
			}
			if (CardData.MyCardType == CardType.Structures && CardData.IsBuilding)
			{
				num += 8f;
			}
			if (CardData is HeavyFoundation)
			{
				num += 1000f;
			}
			if (Child != null)
			{
				num += Child.Mass;
			}
			return num;
		}
	}

	public bool IsCollapsed
	{
		get
		{
			if (!BeingDragged)
			{
				return false;
			}
			if (WorldManager.instance.NearbyCardTarget != null)
			{
				return true;
			}
			if (GetRootCard().GetChildCount() >= 10 && !WorldManager.instance.IsShiftDragging)
			{
				return true;
			}
			return false;
		}
	}

	public Combatable Combatable => CardData as Combatable;

	public bool InConflict
	{
		get
		{
			if (Combatable != null)
			{
				return Combatable.InConflict;
			}
			return false;
		}
	}

	public bool InAttack
	{
		get
		{
			if (Combatable != null)
			{
				return Combatable.InAttack;
			}
			return false;
		}
	}

	public GameCard TryGetNthChild(int n)
	{
		GameCard gameCard = this;
		for (int i = 0; i < n; i++)
		{
			if (gameCard.Child != null)
			{
				gameCard = gameCard.Child;
				continue;
			}
			return null;
		}
		return gameCard;
	}

	protected override void Awake()
	{
		base.Awake();
		base.transform.rotation = Quaternion.Euler(270f, 90f, 90f);
		propBlock = new MaterialPropertyBlock();
		combatCirclePropBlock = new MaterialPropertyBlock();
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
		CombatStatusCircle.gameObject.SetActiveFast(active: false);
		DropShadowRenderer.enabled = false;
		newCircleStartSize = NewCircle.transform.localScale;
		NewCircle.gameObject.SetActiveFast(active: true);
		NewCircle.transform.localScale = Vector3.zero;
		CombatCircleColor = CombatStatusCircle.color;
		StatusEffectBackground.transform.localScale = Vector3.zero;
		ParticleSystem.EmissionModule emission = FoilParticles.emission;
		emission.enabled = false;
		EquipmentRectangle.gameObject.SetActiveFast(active: true);
		WorkerRectangle.gameObject.SetActiveFast(active: true);
		SpecialText.gameObject.SetActiveFast(active: false);
		CoinText.gameObject.SetActiveFast(active: false);
		CoinIcon.gameObject.SetActiveFast(active: false);
		statusEffectBackgroundTransform = StatusEffectBackground.transform;
	}

	protected override void Start()
	{
		startScale = base.transform.localScale;
		if (IsDemoCard)
		{
			startScale *= 0.2f;
			base.transform.localScale = startScale;
		}
		UpdateIcon();
		lastPosition = (TargetPosition = base.transform.position);
		UpdateCardPalette();
		SetColors();
		HighlightRectangle.enabled = false;
		if (!WorldManager.instance.AllCards.Contains(this) && !IsDemoCard)
		{
			WorldManager.instance.AllCards.Add(this);
		}
		if (!WorldManager.instance.UniqueIdToCard.ContainsKey(CardData.UniqueId))
		{
			WorldManager.instance.UniqueIdToCard[CardData.UniqueId] = this;
		}
		onOffBasePosition = OnOffInteractable.transform.localPosition;
		onOffTargetPosition = onOffBasePosition + new Vector3(0.09f, 0f, 0f);
		onOffTargetPos = onOffBasePosition;
		OnOffInteractable.gameObject.SetActive(value: false);
		if (!CardData.HasInventory)
		{
			UnityEngine.Object.Destroy(HeadEquipmentPosition);
			UnityEngine.Object.Destroy(HandEquipmentPosition);
			UnityEngine.Object.Destroy(TorsoEquipmentPosition);
		}
	}

	public void UpdateIcon()
	{
		if (CardData.MyCardType == CardType.Ideas)
		{
			if (CardData.CardUpdateType == CardUpdateType.Main)
			{
				IconRenderer.sprite = SpriteManager.instance.IdeaIcon;
			}
			else if (CardData.CardUpdateType == CardUpdateType.Island)
			{
				IconRenderer.sprite = SpriteManager.instance.IslandIdeaIcon;
			}
			else if (CardData.CardUpdateType == CardUpdateType.Spirit)
			{
				IconRenderer.sprite = SpriteManager.instance.SpiritIdeaIcon;
			}
			else if (CardData.CardUpdateType == CardUpdateType.Cities)
			{
				IconRenderer.sprite = SpriteManager.instance.CitiesIdeaIcon;
			}
			else
			{
				IconRenderer.sprite = SpriteManager.instance.IdeaIcon;
			}
		}
		if (CardData.Icon != null)
		{
			IconRenderer.sprite = CardData.Icon;
		}
	}

	public void UpdateCardPalette()
	{
		myCardPalette = ColorManager.instance.GetPaletteForCard(CardData);
	}

	public void ToggleDirection()
	{
		if (CardData.OutputDir == Vector3.zero)
		{
			CardData.OutputDir = Vector3.right;
		}
		else if (CardData.OutputDir == Vector3.right)
		{
			CardData.OutputDir = Vector3.back;
		}
		else if (CardData.OutputDir == Vector3.back)
		{
			CardData.OutputDir = Vector3.left;
		}
		else if (CardData.OutputDir == Vector3.left)
		{
			CardData.OutputDir = Vector3.forward;
		}
		else if (CardData.OutputDir == Vector3.forward)
		{
			CardData.OutputDir = Vector3.zero;
		}
		QuestManager.instance.SpecialActionComplete("output_direction_changed", CardData);
	}

	private void SetColors()
	{
		CombatStatusCircle.color = CombatCircleColor;
		CombatStatusCircle.color = Color.red;
		if (myCardPalette == null)
		{
			Debug.LogError("Could not find card color pallet");
			return;
		}
		Color color = myCardPalette.Color;
		Color value = myCardPalette.Color2;
		Color color2 = myCardPalette.Icon;
		if (IsHit)
		{
			CombatStatusCircle.color = Color.white;
			color = (value = (color2 = Color.white));
		}
		CardRenderer.shadowCastingMode = ((!IsEquipped && !IsWorking) ? ShadowCastingMode.On : ShadowCastingMode.Off);
		CardRenderer.GetPropertyBlock(propBlock, 2);
		propBlock.SetColor(propColor, color);
		propBlock.SetColor(propColor2, value);
		propBlock.SetColor(propIconColor, color2);
		Texture2D texture2D = null;
		bool flag = false;
		if (CardData is ResourceChest || CardData is FoodWarehouse)
		{
			texture2D = SpriteManager.instance.ChestIconSecondary.texture;
		}
		else if (CardData is ResourceMagnet)
		{
			texture2D = SpriteManager.instance.MagnetIconSecondary.texture;
		}
		bool flag2 = texture2D != null;
		propBlock.SetFloat(propHasSecondaryIcon, flag2 ? 1f : 0f);
		propBlock.SetFloat(propHasOutputDir, flag ? 1f : 0f);
		if (texture2D != null)
		{
			propBlock.SetTexture(propSecondaryTex, texture2D);
		}
		float value2 = ((CardData is Equipable) ? 0.3f : 1f);
		propBlock.SetFloat(propBigShineStrength, (CardData is Equipable) ? 0f : 1f);
		propBlock.SetFloat(propShineStrength, value2);
		propBlock.SetFloat(propFoil, (CardData.IsFoil || CardData.IsShiny || CardData is Equipable) ? 1f : 0f);
		propBlock.SetFloat(propDamaged, CardData.IsDamaged ? 1f : 0f);
		if (IconRenderer.sprite != null)
		{
			propBlock.SetTexture(propIconTex, IconRenderer.sprite.texture);
		}
		else
		{
			propBlock.SetTexture(propIconTex, SpriteManager.instance.EmptyTexture.texture);
		}
		CardRenderer.SetPropertyBlock(propBlock, 2);
		if (SpecialText.color != color)
		{
			SpecialText.color = color;
		}
		SpecialIcon.color = color2;
		IconRenderer.color = color2;
		if (CoinText.color != color)
		{
			CoinText.color = color;
		}
		CoinIcon.color = color2;
		if (EquipmentButton.Color != color)
		{
			EquipmentButton.Color = color;
		}
		if (WorkerButton.Color != color)
		{
			WorkerButton.Color = color;
		}
		Color color3 = color2;
		color3.a = 0.5f;
		WorkerInventoryIcon.color = (HasAnyWorkers() ? color2 : color3);
		if (CardNameText.color != color2)
		{
			CardNameText.color = color2;
		}
	}

	private static Sprite GetSpriteForAttackType(AttackType type)
	{
		return type switch
		{
			AttackType.Magic => SpriteManager.instance.MagicFightIcon, 
			AttackType.Melee => SpriteManager.instance.MeleeFightIcon, 
			AttackType.Ranged => SpriteManager.instance.RangedFightIcon, 
			AttackType.Foot => SpriteManager.instance.FootFightIcon, 
			AttackType.Armour => SpriteManager.instance.ArmourFightIcon, 
			AttackType.Air => SpriteManager.instance.AirFightIcon, 
			_ => null, 
		};
	}

	protected override void OnDestroy()
	{
		if (WorldManager.instance != null)
		{
			WorldManager.instance.AllCards.Remove(this);
			if (WorldManager.instance.UniqueIdToCard.ContainsKey(CardData.UniqueId) && WorldManager.instance.UniqueIdToCard[CardData.UniqueId] == this)
			{
				WorldManager.instance.UniqueIdToCard.Remove(CardData.UniqueId);
			}
		}
		base.OnDestroy();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(debugBounds.center, debugBounds.size);
	}

	public virtual void DestroyCard(bool spawnSmoke = false, bool playSound = true)
	{
		RemoveFromStack();
		WorldManager.instance.AllCards.Remove(this);
		WorldManager.instance.UniqueIdToCard.Remove(CardData.UniqueId);
		Destroyed = true;
		CardData.OnDestroyCard();
		if (playSound)
		{
			AudioManager.me.PlaySound2D(AudioManager.me.CardDestroy, UnityEngine.Random.Range(0.8f, 1.2f), 0.3f);
		}
		if (spawnSmoke)
		{
			WorldManager.instance.CreateSmoke(base.transform.position);
		}
		if (CardData is Curse item)
		{
			WorldManager.instance.ActiveCurses.Remove(item);
		}
		if (CardData.HasInventory)
		{
			foreach (GameCard equipmentChild in EquipmentChildren)
			{
				equipmentChild.EquipmentHolder = null;
				equipmentChild.IsEquipped = false;
				equipmentChild.DestroyCard(spawnSmoke: false, playSound: false);
			}
		}
		if (CardData.WorkerAmount > 0)
		{
			foreach (GameCard workerChild in WorkerChildren)
			{
				workerChild.WorkerHolder = null;
				workerChild.IsWorking = false;
				workerChild.DestroyCard(spawnSmoke: false, playSound: false);
			}
		}
		if (Combatable != null && Combatable.InConflict)
		{
			Combatable.MyConflict.LeaveConflict(Combatable);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SetChild(GameCard card)
	{
		cardsInvolved.Clear();
		cardsInvolved.Add(this);
		if (card == this)
		{
			Debug.LogError("Child is same as Parent");
		}
		else if (card == null)
		{
			if (Child != null)
			{
				cardsInvolved.Add(Child);
				Child.Parent = null;
			}
			Child = null;
			NotifyStackUpdate(cardsInvolved);
		}
		else
		{
			Child = card;
			card.Parent = this;
			cardsInvolved.Add(card);
			NotifyStackUpdate(cardsInvolved);
		}
	}

	public void SetParent(GameCard card)
	{
		cardsInvolved.Clear();
		cardsInvolved.Add(this);
		if (card == this)
		{
			Debug.LogError("Child is same as Parent");
		}
		else if (card == null)
		{
			if (Parent != null)
			{
				cardsInvolved.Add(Parent);
				Parent.Child = null;
			}
			Parent = null;
			NotifyStackUpdate(cardsInvolved);
		}
		else
		{
			Parent = card;
			card.Child = this;
			cardsInvolved.Add(card);
			NotifyStackUpdate(cardsInvolved);
		}
	}

	public void RemoveFromStack()
	{
		SetParent(null);
		SetChild(null);
	}

	private void NotifyStackUpdate(List<GameCard> cardsInvolved)
	{
		foreach (GameCard item in cardsInvolved)
		{
			item.GetRootCard().StackUpdate = true;
			item.StackUpdate = true;
		}
	}

	public void RemoveFromParent()
	{
		if (Parent != null)
		{
			Parent.SetChild(null);
		}
		SetParent(null);
	}

	public override bool CanBePushed()
	{
		if (CardData is Food && WorldManager.instance.InEatingAnimation)
		{
			return false;
		}
		if (CardData is Spirit || CardData is CityAdvisor)
		{
			return false;
		}
		if (IsWorking || IsEquipped)
		{
			return false;
		}
		if (!BeingDragged)
		{
			return PushEnabled;
		}
		return false;
	}

	public override bool CanBePushedBy(Draggable draggable)
	{
		if (IsEquipped || IsWorking)
		{
			return false;
		}
		if (draggable is Boosterpack && WorldManager.instance.CurrentBoard.Id == "cities" && GetRootCard().CardData.MyCardType == CardType.Structures)
		{
			return false;
		}
		if (draggable is GameCard gameCard)
		{
			if (gameCard.IsChildOf(this) || gameCard.IsParentOf(this))
			{
				return false;
			}
			if (gameCard.BounceTarget != null)
			{
				return false;
			}
			if (gameCard.Destroyed)
			{
				return false;
			}
			if (!gameCard.PushEnabled)
			{
				return false;
			}
			if (gameCard.CardData is Food && WorldManager.instance.InEatingAnimation)
			{
				return false;
			}
			if (WorldManager.instance.CurrentBoard.Id == "cities" && GetRootCard().CardData.MyCardType == CardType.Structures && (gameCard.CardData is Resource || gameCard.CardData is Food))
			{
				return false;
			}
			if (gameCard.CardData is Spirit || gameCard.CardData is CityAdvisor)
			{
				return false;
			}
			if (gameCard.CardData is Energy)
			{
				return false;
			}
			if (gameCard.IsEquipped || gameCard.IsWorking)
			{
				return false;
			}
			if (!CardData.CanBePushedBy(gameCard.CardData))
			{
				return false;
			}
		}
		return base.CanBePushedBy(draggable);
	}

	public override bool CanBeDragged()
	{
		if (CardData is Combatable { BeingAttacked: not false })
		{
			return false;
		}
		if (WorldManager.instance.RemovingCards && GetRootCard().CardData is Boat { InSailOff: not false })
		{
			return false;
		}
		if (!BeingDragged && CardData.CanBeDragged)
		{
			return FaceUp;
		}
		return false;
	}

	public override void Clicked()
	{
		if (!FaceUp)
		{
			FaceUp = true;
		}
		if (DragTag == "inventory")
		{
			InventoryInteractable.Clicked();
			WorkerInventoryInteractable.Clicked();
		}
		else
		{
			CardData.Clicked();
		}
		WasClicked = true;
		base.Clicked();
	}

	public void ForceUpdate()
	{
		Update();
	}

	public void Equip(Equipable equipable)
	{
		GameCard myGameCard = equipable.MyGameCard;
		EquipmentChildren.Add(myGameCard);
		myGameCard.EquipmentHolder = this;
		myGameCard.IsEquipped = true;
		myGameCard.RemoveFromStack();
		CardData.OnEquipItem(equipable);
	}

	public void Unequip(Equipable equipable)
	{
		GameCard myGameCard = equipable.MyGameCard;
		EquipmentChildren.Remove(myGameCard);
		myGameCard.EquipmentHolder = null;
		myGameCard.IsEquipped = false;
		CardData.OnUnequipItem(equipable);
		if (Combatable != null && Combatable.HealthPoints > Combatable.ProcessedCombatStats.MaxHealth)
		{
			Combatable.HealthPoints = Combatable.ProcessedCombatStats.MaxHealth;
		}
	}

	public void EquipWorker(Worker worker, int index)
	{
		GameCard myGameCard = worker.MyGameCard;
		worker.WorkerIndex = index;
		WorkerChildren.Add(myGameCard);
		myGameCard.WorkerHolder = this;
		myGameCard.IsWorking = true;
		myGameCard.RemoveFromStack();
		CardData.OnEquipItem(null);
	}

	public void UnequipWorker(GameCard worker)
	{
		WorkerChildren.Remove(worker);
		worker.CardData.WorkerIndex = -1;
		worker.WorkerHolder = null;
		worker.IsWorking = false;
		GetRootCard().StackUpdate = true;
		CardData?.OnUnequipItem(null);
	}

	protected override void Bounce()
	{
		if (HasParent)
		{
			BounceTarget = null;
		}
		if (BounceTarget != null)
		{
			GameCard gameCard = BounceTarget;
			if (gameCard.Child != null)
			{
				gameCard = gameCard.GetLeafCard();
			}
			BounceTarget = null;
			if (gameCard == this || gameCard.BounceTarget != null || gameCard.GetCardInCombatInStack() != null || gameCard.BeingDragged)
			{
				return;
			}
			GameCard cardWithStatusInStack = gameCard.GetCardWithStatusInStack();
			if (cardWithStatusInStack != null && !cardWithStatusInStack.CardData.CanHaveCardsWhileHasStatus())
			{
				return;
			}
			if (gameCard.CardData.CanHaveCardOnTop(CardData))
			{
				SetParent(gameCard);
				Velocity = null;
				AudioManager.me.PlaySound2D(AudioManager.me.DropOnStack, UnityEngine.Random.Range(0.8f, 1.2f), 0.3f);
			}
		}
		base.Bounce();
	}

	protected override void Update()
	{
		if (!IsDemoCard && !MyBoard.IsCurrent)
		{
			return;
		}
		if (HasChild && CardData.IsDamaged)
		{
			if (CardData.DamageType == CardDamageType.Fire && Child.CardData.Id == "water")
			{
				Child.DestroyCard();
				CardData.SetCardUndamaged();
				WorldManager.instance.CreateSmoke(Position);
				AudioManager.me.PlaySound2D(AudioManager.me.ExtinguishCardSound, UnityEngine.Random.Range(0.9f, 1.1f), 0.3f);
			}
			else if (CardData.DamageType == CardDamageType.Drought && CardData.ChildrenMatchingPredicate((CardData x) => x.Id == "water").Count >= 3)
			{
				CardData.DestroyChildrenMatchingPredicateAndRestack((CardData x) => x.Id == "water", 3);
				CardData.SetCardUndamaged();
				WorldManager.instance.CreateSmoke(Position);
				AudioManager.me.PlaySound2D(AudioManager.me.DroughtSolved, UnityEngine.Random.Range(0.9f, 1.1f), 0.3f);
			}
			else if (CardData.DamageType == CardDamageType.Damaged && Child.CardData is ICurrency && CardData.GetDollarCountInStack(includeInChest: true) >= CardData.GetRepairCost())
			{
				List<ICurrency> currencyList = CardData.ChildrenMatchingPredicate((CardData x) => x is ICurrency).Cast<ICurrency>().ToList();
				CitiesManager.instance.TryUseDollars(currencyList, CardData.GetRepairCost(), onlyTakeIfAmountMet: true);
				CardData.SetCardUndamaged();
				AudioManager.me.PlaySound2D(AudioManager.me.RepairCardSound, UnityEngine.Random.Range(0.9f, 1.1f), 0.3f);
			}
		}
		CardData.UpdateCard();
		SetColors();
		string text = CardData.Name;
		if (CardNameText.text != text)
		{
			CardNameText.text = CardData.Name;
		}
		Vector3 b = (IsNew ? newCircleStartSize : Vector3.zero);
		NewCircle.transform.localScale = Vector3.Lerp(NewCircle.transform.localScale, b, Time.deltaTime * 20f);
		bool flag = WorldManager.instance.DraggingCard != null && WorldManager.instance.DraggingCard.CardData.Id == CardData.Id;
		if (BeingDragged || WasClicked || Child != null || Parent != null || InConflict || GetCardWithStatusInStack() != null || flag || CardData is Spirit || CardData is CityAdvisor)
		{
			IsNew = false;
		}
		if (Child != null && !(Child.CardData is Equipable))
		{
			ShowInventory = false;
		}
		if (BeingDragged)
		{
			ShowInventory = false;
		}
		if (Combatable != null && Combatable.InAttack)
		{
			ShowInventory = false;
		}
		ParticleSystem.EmissionModule emission = FoilParticles.emission;
		emission.enabled = !IsDemoCard && (CardData.IsFoil || CardData.Id == "goblet");
		PerformanceHelper.SetActive(CombatStatusCircle.gameObject, InConflict || (Combatable != null && Combatable is Enemy && !IsDemoCard));
		if (Combatable != null)
		{
			CombatStatusCircle.sprite = GetSpriteForAttackType(Combatable.ProcessedAttackType);
			CombatStatusCircle.GetPropertyBlock(combatCirclePropBlock);
			float value = (Combatable.InConflict ? Combatable.TimeToAttackNormalized : 1f);
			combatCirclePropBlock.SetFloat("_FillAmount", value);
			CombatStatusCircle.SetPropertyBlock(combatCirclePropBlock);
		}
		PerformanceHelper.SetActive(SpecialText.gameObject, SpecialValue.HasValue);
		if (SpecialValue.HasValue)
		{
			SpecialText.text = SpecialValue.Value.ToStringCached();
		}
		PerformanceHelper.SetActive(SpecialIcon.gameObject, SpecialValue.HasValue || ShowSpecialIcon);
		int value2 = CardData.GetValue();
		if (value2 != -1)
		{
			CoinText.text = value2.ToStringCached();
			PerformanceHelper.SetActive(CoinIcon.gameObject, active: true);
			PerformanceHelper.SetActive(CoinText.gameObject, active: true);
		}
		else
		{
			PerformanceHelper.SetActive(CoinIcon.gameObject, active: false);
			PerformanceHelper.SetActive(CoinText.gameObject, active: false);
		}
		UpdateShowInventory();
		UpdateShowWorkerInventory();
		if (CardData.HasInventory)
		{
			HandInventoryIcon.color = (CardData.HasEquipableOfEquipableType(EquipableType.Weapon) ? colOn : colOff);
			TorsoInventoryIcon.color = (CardData.HasEquipableOfEquipableType(EquipableType.Torso) ? colOn : colOff);
			HeadInventoryIcon.color = (CardData.HasEquipableOfEquipableType(EquipableType.Head) ? colOn : colOff);
		}
		DropShadowRenderer.enabled = IsEquipped && EquipmentHolder.ShowInventory;
		DropShadowRenderer.enabled = IsWorking && WorkerHolder.ShowInventory;
		OnOffInteractable.gameObject.SetActiveFast(active: false);
		Vector3 b2 = startScale;
		if ((IsEquipped || IsWorking) && !BeingDragged)
		{
			b2 = startScale * 0.8f;
		}
		if (!IsDemoCard)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, b2, Time.deltaTime * 12f);
		}
		if (!IsDemoCard)
		{
			UpdatePosition();
		}
		Vector3 position = base.transform.position;
		position.y = (0f - position.z) * 0.001f;
		EquipmentRectangle.position = position + equipmentRectangleStartOffset;
		WorkerRectangle.position = position + equipmentRectangleStartOffset;
		if (!IsDemoCard && !FaceUp)
		{
			flipTimer += Time.deltaTime * WorldManager.instance.PhysicsTimeScale;
			if (flipTimer >= 0.1f)
			{
				FaceUp = true;
			}
		}
		wobbleRotVelo -= Time.deltaTime * RotWobbleSpringiness;
		if (wobbleRotVelo <= 0f)
		{
			wobbleRotVelo = 0f;
		}
		float num = RotWobbleAmp * Mathf.Sin(wobbleRotVelo * RotWobbleSpeed) * wobbleRotVelo;
		if (AutoRotWobble)
		{
			rotWobbleTimer += Time.deltaTime;
			if (rotWobbleTimer > AutoRotWobbleTimer)
			{
				rotWobbleTimer -= AutoRotWobbleTimer;
				RotWobble(AutoRotWobbleAmount);
			}
		}
		bool active = true;
		if (!IsDemoCard)
		{
			if (IsEquipped)
			{
				if (EquipmentHolder.ShowInventory && !BeingDragged)
				{
					Transform myEquipmentStackPosition = GetMyEquipmentStackPosition();
					base.transform.localRotation = Camera.main.transform.localRotation;
					base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, myEquipmentStackPosition.localEulerAngles.z);
				}
				else if (!BeingDragged)
				{
					active = false;
				}
			}
			else if (IsWorking)
			{
				if (WorkerHolder.ShowInventory && !BeingDragged)
				{
					Transform transformAtIndex = WorkerHolder.WorkerTransformHolder.GetTransformAtIndex(CardData.WorkerIndex);
					base.transform.localRotation = Camera.main.transform.localRotation;
					base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, transformAtIndex.localEulerAngles.z);
				}
				else if (!BeingDragged)
				{
					active = false;
				}
			}
			else
			{
				float b3 = (FaceUp ? 90f : 270f);
				curZ = Mathf.Lerp(curZ, b3, Time.deltaTime * 14f * WorldManager.instance.PhysicsTimeScale);
				if (Parent != null)
				{
					curZ = b3;
				}
				base.transform.localRotation = Quaternion.Euler(curZ, 0f + num + ZRotOffset, 0f);
			}
		}
		else
		{
			SetDemoCardRotation();
		}
		PerformanceHelper.SetActive(Visuals.gameObject, active);
		if (Parent == null)
		{
			snappedToParent = false;
		}
		if (WorldManager.instance.CurrentBoard != null && HighlightActive)
		{
			HighlightRectangle.Color = WorldManager.instance.CurrentBoard.CardHighlightColor;
		}
		HighlightRectangle.enabled = HighlightActive;
		if (HighlightActive)
		{
			HighlightRectangle.DashOffset += Time.deltaTime;
			if (HighlightRectangle.DashOffset >= 1f)
			{
				HighlightRectangle.DashOffset -= 1f;
			}
		}
		lastPosition = base.transform.position;
		UpdateTimer();
		if (removedChild != null && !removedChild.BeingDragged)
		{
			removedChild = null;
			StackUpdate = true;
		}
		UpdateStatusEffectElements();
		UpdateCardAnimations();
		if (CardData.IsDamaged)
		{
			if (CardData.DamageType == CardDamageType.Damaged)
			{
				CardData.AddStatusEffect(new StatusEffect_Damaged());
			}
			if (CardData.DamageType == CardDamageType.Fire)
			{
				CardData.AddStatusEffect(new StatusEffect_OnFire());
			}
			if (CardData.DamageType == CardDamageType.Drought)
			{
				CardData.AddStatusEffect(new StatusEffect_Drought());
			}
		}
		else
		{
			CardData.RemoveStatusEffect<StatusEffect_Damaged>();
			CardData.RemoveStatusEffect<StatusEffect_OnFire>();
			CardData.RemoveStatusEffect<StatusEffect_Drought>();
		}
		if (IsHovered && CardData.IsDamaged)
		{
			if (CardData.DamageType == CardDamageType.Damaged)
			{
				Tooltip.Text = "<b>" + SokLoc.Translate("label_damaged") + "</b>\n" + SokLoc.Translate("label_damaged_card_cost", LocParam.Create("amount", CardData.GetRepairCost().ToStringCached()), LocParam.Create("icon", Icons.Dollar));
			}
			if (CardData.DamageType == CardDamageType.Fire)
			{
				Tooltip.Text = "<b>" + SokLoc.Translate("label_on_fire") + "</b>\n" + SokLoc.Translate("label_fire_card_cost");
			}
		}
	}

	private bool HasAnyWorkers()
	{
		List<GameCard> workerChildren = CardData.MyGameCard.WorkerChildren;
		for (int i = 0; i < workerChildren.Count; i++)
		{
			if (workerChildren[i] != null)
			{
				return true;
			}
		}
		return false;
	}

	private void animateOnOffInteractable()
	{
		bool flag = false;
		if (!CardData.WorkerAmountMet())
		{
			flag = false;
		}
		if (WorldManager.instance.CurrentView != ViewType.Default)
		{
			flag = true;
		}
		if (CardData.CanToggleCardOnOff())
		{
			if (!OnOffInteractable.Velocity.HasValue && onOffBasePosition.magnitude - OnOffInteractable.transform.localPosition.magnitude < 0.001f && onOffBasePosition.magnitude - OnOffInteractable.transform.localPosition.magnitude > -0.001f)
			{
				if (flag)
				{
					OnOffInteractable.gameObject.SetActive(value: true);
					onOffTargetPos = onOffTargetPosition;
				}
				else
				{
					OnOffInteractable.gameObject.SetActive(value: false);
				}
			}
			else if (!flag && !OnOffInteractable.Velocity.HasValue && onOffTargetPosition.magnitude - OnOffInteractable.transform.localPosition.magnitude < 0.001f && onOffTargetPosition.magnitude - OnOffInteractable.transform.localPosition.magnitude > -0.001f)
			{
				onOffTargetPos = onOffBasePosition;
			}
		}
		else
		{
			OnOffInteractable.gameObject.SetActive(value: false);
		}
		OnOffInteractable.transform.localPosition = FRILerp.Spring(OnOffInteractable.transform.localPosition, onOffTargetPos, 20f, 30f, ref onOffVelocity);
	}

	public void UpdateCardAnimations()
	{
		for (int i = 0; i < CardAnimations.Count; i++)
		{
			CardAnimation cardAnimation = CardAnimations[i];
			if (!cardAnimation.HasStarted)
			{
				cardAnimation.Start();
			}
			cardAnimation.Update();
			if (cardAnimation.IsDone)
			{
				CardAnimations.RemoveAt(i);
				i--;
			}
			else if (cardAnimation.IsBlocking)
			{
				break;
			}
		}
	}

	public void CreateCardConnectors()
	{
		CardData.EnergyConnectors.OrderBy((CardConnectorData x) => x.EnergyConnectionStrength);
		foreach (CardConnectorData energyConnector in CardData.EnergyConnectors)
		{
			int energyConnectionAmount = energyConnector.EnergyConnectionAmount;
			float x2 = ((energyConnector.EnergyConnectionType == CardDirection.input) ? (-0.19f) : 0.19f);
			for (int i = 0; i < energyConnectionAmount; i++)
			{
				Vector3 localPosition = new Vector3(x2, (float)i * ConnectorAmountOffset - (float)(energyConnectionAmount / 2) * ConnectorAmountOffset + ConnectorAmountOffset / 2f * ((energyConnectionAmount % 2 == 0) ? 1f : 0f) - CardTextOffset, -0.03f);
				GameObject obj = UnityEngine.Object.Instantiate(EnergyConnectorPrefab, Vector3.zero, base.transform.rotation, EnergyConnectorTransform);
				obj.transform.localPosition = localPosition;
				CardConnector component = obj.GetComponent<CardConnector>();
				component.InitializeEnergyNode(energyConnector, this);
				CardConnectorChildren.Add(component);
			}
		}
	}

	private void UpdateConnectors()
	{
		foreach (CardConnector cardConnectorChild in CardConnectorChildren)
		{
			if (WorldManager.instance.CurrentBoard.Id != "cities")
			{
				cardConnectorChild.gameObject.SetActive(value: false);
				break;
			}
			if (WorldManager.instance.CurrentView == ViewType.Default)
			{
				cardConnectorChild.gameObject.SetActive(value: true);
			}
			else if (WorldManager.instance.CurrentView == ViewType.Energy)
			{
				cardConnectorChild.gameObject.SetActive(cardConnectorChild.ConnectionType == ConnectionType.LV || cardConnectorChild.ConnectionType == ConnectionType.HV);
			}
			else if (WorldManager.instance.CurrentView == ViewType.Sewer)
			{
				cardConnectorChild.gameObject.SetActive(cardConnectorChild.ConnectionType == ConnectionType.Sewer);
			}
			else if (WorldManager.instance.CurrentView == ViewType.Transport)
			{
				cardConnectorChild.gameObject.SetActive(cardConnectorChild.ConnectionType == ConnectionType.Transport);
			}
		}
	}

	private void UpdateShowInventory()
	{
		bool flag = CardData.WorkerAmount > 0 && Child == null && !CardData.HasInventory;
		bool flag2 = CardData.HasInventory && Child == null && EquipmentChildren.Count > 0;
		PerformanceHelper.SetActive(EquipmentButton.gameObject, flag2);
		PerformanceHelper.SetActive(InventoryInteractable.gameObject, flag2);
		if (ShowInventory && !flag2 && !flag)
		{
			ShowInventory = false;
		}
	}

	private void UpdateShowWorkerInventory()
	{
		bool active = CardData.WorkerAmount > 0 && Child == null && !CardData.HasInventory && !IsDemoCard;
		PerformanceHelper.SetActive(WorkerButton.gameObject, active);
		PerformanceHelper.SetActive(WorkerInventoryInteractable.gameObject, active);
	}

	private PositionType DeterminePositionType()
	{
		if (CardAnimations.Count > 0)
		{
			return PositionType.InAnimation;
		}
		if (IsEquipped)
		{
			if (BeingDragged)
			{
				return PositionType.None;
			}
			return PositionType.IsEquipped;
		}
		if (IsWorking)
		{
			if (BeingDragged)
			{
				return PositionType.None;
			}
			return PositionType.IsWorking;
		}
		if (InConflict)
		{
			if (BeingDragged)
			{
				return PositionType.None;
			}
			if (InAttack)
			{
				return PositionType.InAttack;
			}
			return PositionType.InConflict;
		}
		if (Parent != null)
		{
			return PositionType.InStack;
		}
		if (Parent == null)
		{
			return PositionType.IsRoot;
		}
		return PositionType.None;
	}

	private void UpdatePosition()
	{
		switch (DeterminePositionType())
		{
		case PositionType.InConflict:
			TargetPosition = Combatable.MyConflict.GetPositionInConflict(Combatable);
			base.transform.position = Vector3.Lerp(base.transform.position, TargetPosition, Time.deltaTime * 20f);
			break;
		case PositionType.InAttack:
		{
			AttackAnimation currentAttackAnimation = Combatable.CurrentAttackAnimation;
			base.transform.position = currentAttackAnimation.Position;
			TargetPosition = currentAttackAnimation.TargetPosition;
			break;
		}
		case PositionType.InAnimation:
		{
			CardAnimation cardAnimation = CardAnimations[0];
			base.transform.position = cardAnimation.Position;
			TargetPosition = cardAnimation.TargetPosition;
			break;
		}
		case PositionType.IsEquipped:
			if (EquipmentHolder.InventoryVisible)
			{
				TargetPosition = GetMyEquipmentStackPosition().position;
				if (IsHovered)
				{
					TargetPosition -= base.transform.forward * 0.1f;
				}
				base.transform.position = TargetPosition;
			}
			else
			{
				TargetPosition = EquipmentHolder.transform.position + new Vector3(0f, -0.1f, 0f);
				base.transform.position = TargetPosition;
			}
			break;
		case PositionType.IsWorking:
			if (WorkerHolder.InventoryVisible)
			{
				TargetPosition = WorkerHolder.WorkerTransformHolder.GetTransformAtIndex(CardData.WorkerIndex).position;
				if (IsHovered)
				{
					TargetPosition -= base.transform.forward * 0.1f;
				}
				base.transform.position = TargetPosition;
			}
			else
			{
				TargetPosition = WorkerHolder.transform.position + new Vector3(0f, -0.1f, 0f);
				base.transform.position = TargetPosition;
			}
			break;
		case PositionType.InStack:
			SetToParentPosition();
			TargetPosition = base.transform.position;
			break;
		case PositionType.IsRoot:
		case PositionType.None:
			if (!Velocity.HasValue)
			{
				Vector3 targetPosition = TargetPosition;
				float num = 20f;
				if (SetY)
				{
					targetPosition.y = (0f - targetPosition.z) * 0.001f;
					targetPosition.y += (BeingDragged ? 0.1f : 0f);
					if (IsHovered && CanBeDragged() && WorldManager.instance.CanInteract)
					{
						targetPosition.y += 0.06f;
					}
					if (CardData is Spirit || CardData is CityAdvisor)
					{
						targetPosition.y += 0.25f;
					}
				}
				else
				{
					num = 10f + WorldManager.instance.EndOfMonthSpeedup * 3f;
				}
				base.transform.position = Vector3.Lerp(base.transform.position, targetPosition, Time.deltaTime * num);
			}
			UpdateChildPositions();
			break;
		}
		if (closeToTargetPositionCallback != null && Vector3.Distance(base.transform.position, TargetPosition) < 0.1f)
		{
			closeToTargetPositionCallback();
		}
	}

	public Transform GetEquipmentStackPosition(EquipableType equipableType)
	{
		return equipableType switch
		{
			EquipableType.Head => HeadEquipmentPosition.transform, 
			EquipableType.Torso => TorsoEquipmentPosition.transform, 
			EquipableType.Weapon => HandEquipmentPosition.transform, 
			_ => throw new ArgumentException($"EquipableType does not have a stack position set for {equipableType}"), 
		};
	}

	public void ToggleInventory()
	{
		OpenInventory(!ShowInventory);
	}

	public void ToggleCardOnOff()
	{
		CardData.ToggleCardOnOff();
	}

	public void OpenInventory(bool showInventory)
	{
		if (showInventory == ShowInventory)
		{
			return;
		}
		ShowInventory = showInventory;
		if (!ShowInventory)
		{
			return;
		}
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (allCard != this && allCard.ShowInventory)
			{
				allCard.ShowInventory = false;
			}
		}
	}

	public void StatusEffectsChanged()
	{
		foreach (StatusEffect statusEffect in CardData.StatusEffects)
		{
			if (!ElementExistsForStatusEffect(statusEffect))
			{
				StatusEffectElement item = CreateElementForStatusEffect(statusEffect);
				StatusEffectElements.Add(item);
			}
		}
		for (int i = 0; i < StatusEffectElements.Count; i++)
		{
			if (!CardData.StatusEffects.Contains(StatusEffectElements[i].MyStatusEffect))
			{
				StatusEffectElements[i].DestroyMe = true;
			}
		}
		List<StatusEffectElement> list = StatusEffectElements.Where((StatusEffectElement x) => !x.DestroyMe).ToList();
		for (int j = 0; j < list.Count; j++)
		{
			float x2 = (float)j * DistanceBetweenStatusses - (float)(CardData.StatusEffects.Count - 1) * DistanceBetweenStatusses * 0.5f;
			list[j].TargetLocalPosition = new Vector3(x2, 0f, -0.001f);
		}
	}

	private void UpdateStatusEffectElements()
	{
		Vector3 b = ((StatusEffectElements.Count == 0) ? Vector3.zero : Vector3.one);
		statusEffectBackgroundTransform.localScale = Vector3.Lerp(statusEffectBackgroundTransform.localScale, b, Time.deltaTime * 12f);
		PerformanceHelper.SetActive(StatusEffectBackground.gameObject, statusEffectBackgroundTransform.localScale.sqrMagnitude > 0.001f);
		float b2 = 0.1125f + (float)(StatusEffectElements.Count - 1) * DistanceBetweenStatusses;
		statusEffectBackgroundWidth = Mathf.Lerp(statusEffectBackgroundWidth, b2, Time.deltaTime * 12f);
		if (Mathf.Abs(statusEffectBackgroundWidth - StatusEffectBackground.Width) > 0.01f)
		{
			StatusEffectBackground.Width = statusEffectBackgroundWidth;
		}
	}

	public void SetDemoCardRotation()
	{
		if (FaceUp)
		{
			base.transform.rotation = Camera.main.transform.rotation;
		}
		else
		{
			base.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
	}

	private Transform GetMyEquipmentStackPosition()
	{
		if (!IsEquipped)
		{
			throw new Exception("Not equipped!");
		}
		return EquipmentHolder.GetEquipmentStackPosition(((Equipable)CardData).EquipableType);
	}

	protected override void LateUpdate()
	{
		if (!(MyBoard != null) || MyBoard.IsCurrent)
		{
			PushAwayFromOthers();
			if (Parent == null && !IsEquipped && !IsWorking)
			{
				ClampPos();
			}
			if (Parent != null)
			{
				LastParent = Parent;
			}
		}
	}

	public void SetFaceUp(bool faceUp)
	{
		FaceUp = faceUp;
		curZ = (FaceUp ? 90f : 270f);
		base.transform.localRotation = Quaternion.Euler(curZ, 0f, 0f);
	}

	public override void SendIt()
	{
		if (MyBoard.Id == "cities" && HasParent)
		{
			Velocity = GetRootCard().CardData.OutputDir * 7f;
		}
		else
		{
			base.SendIt();
		}
		RotWobble(1f);
	}

	public GameCard FindNextGameCardInDirection(Vector3 direction, CardType? type = null)
	{
		float num = float.MinValue;
		GameCard result = null;
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (!allCard.gameObject.activeInHierarchy || allCard == WorldManager.instance.DraggingDraggable)
			{
				continue;
			}
			if (allCard.MyBoard == null)
			{
				Debug.Log(allCard?.ToString() + " does not have a board");
			}
			else
			{
				if (!allCard.MyBoard.IsCurrent || !allCard.CanBeAutoMovedTo || (type.HasValue && allCard.CardData.MyCardType != type))
				{
					continue;
				}
				Vector3 rhs = allCard.AutoMoveSnapPosition - base.transform.position;
				float num2 = Vector3.Dot(direction, rhs);
				if (!((double)num2 <= 0.3))
				{
					float num3 = num2 / rhs.sqrMagnitude;
					if (num3 > num && rhs.sqrMagnitude < 1f)
					{
						num = num3;
						result = allCard;
					}
				}
			}
		}
		return result;
	}

	public override void SendDirection(Vector3 direction)
	{
		RotWobble(1f);
		base.SendDirection(direction);
	}

	public override void SendToPosition(Vector3 position)
	{
		RotWobble(1f);
		base.SendToPosition(position);
	}

	public void SendToPositionCallback(Vector3 position, Action callback)
	{
		RotWobble(1f);
		TargetPosition = position;
		closeToTargetPositionCallback = callback;
	}

	public void RotWobble(float amount)
	{
		wobbleRotVelo = amount;
	}

	private void SetToParentPosition(bool hardSetPos = false)
	{
		Vector3 vector = ((!IsCollapsed) ? (Parent.transform.position + new Vector3(0f, WorldManager.instance.CardOverlayHeightOffset, 0f - WorldManager.instance.CardOverlayOffset)) : (Parent.transform.position + new Vector3(0f, WorldManager.instance.CardOverlayHeightOffset, 0f - WorldManager.instance.CollapsedCardOverlayOffset)));
		if (!snappedToParent)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, vector, Time.deltaTime * 20f);
			if (Vector3.Distance(base.transform.position, vector) < 0.001f)
			{
				snappedToParent = true;
			}
		}
		else
		{
			base.transform.position = Vector3.Lerp(base.transform.position, vector, Time.deltaTime * 20f);
			Vector3 position = base.transform.position;
			position.y = vector.y;
			base.transform.position = position;
		}
		if (hardSetPos)
		{
			base.transform.position = (TargetPosition = vector);
		}
	}

	public void UpdateChildPositions(bool hardSetPos = false)
	{
		if (!(Child == null))
		{
			Child.SetToParentPosition(hardSetPos);
			Child.UpdateChildPositions(hardSetPos);
		}
	}

	public Conflict GetOverlappingConflict()
	{
		foreach (Conflict allConflict in WorldManager.instance.GetAllConflicts())
		{
			if (allConflict.GetBounds().Intersects(base.DraggableBounds))
			{
				return allConflict;
			}
		}
		return null;
	}

	public List<GameCard> GetOverlappingCardsInBox(Vector3 center, Vector3 size)
	{
		List<GameCard> list = new List<GameCard>();
		int num = Physics.OverlapBoxNonAlloc(center, size * 0.5f, hits, Quaternion.identity, -5, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < num; i++)
		{
			GameCard component = hits[i].gameObject.GetComponent<GameCard>();
			if (component != null && component != this)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public List<GameCard> GetOverlappingCards()
	{
		List<GameCard> list = new List<GameCard>();
		int num = PhysicsExtensions.OverlapBoxNonAlloc(boxCollider, hits, -5, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < num; i++)
		{
			GameCard component = hits[i].gameObject.GetComponent<GameCard>();
			if (component != null && component != this)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public void StartBlueprintTimer(float time, TimerAction a, string status, string actionId, string blueprintId, int subprintIndex, CardData consumer, bool skipWorkerEnergyCheck = false)
	{
		if (IsDemoCard || BeingDragged)
		{
			return;
		}
		GameCard gameCard = GetRootCard();
		if (gameCard.CardData is HeavyFoundation && gameCard.HasChild)
		{
			gameCard = gameCard.Child;
		}
		if ((!HasTransportCard() || !(actionId != "sail_off") || !(actionId != "leave_spirit") || !(actionId != "take_portal")) && (!(removedChild != null) || !removedChild.BeingDragged))
		{
			if (TimerActionId == actionId && TimerBlueprintId == blueprintId && TimerSubprintIndex == subprintIndex)
			{
				TargetTimerTime = time;
			}
			else if (CardData.IsOn && (skipWorkerEnergyCheck || gameCard.CardData.ShouldStartTimerWorkers(actionId)) && (skipWorkerEnergyCheck || gameCard.CardData.ShouldStartTimerEnergy(consumer, actionId)) && !gameCard.CardData.IsDamaged)
			{
				TimerBlueprintId = blueprintId;
				TimerSubprintIndex = subprintIndex;
				SkipCitiesChecks = skipWorkerEnergyCheck;
				InitTimer(time, a, status, actionId);
			}
		}
	}

	public void StartTimer(float time, TimerAction a, string status, string actionId, bool withStatusBar = true, bool skipWorkerEnergyCheck = false, bool skipDamageOnOffCheck = false)
	{
		if (!IsDemoCard && !BeingDragged)
		{
			if (TimerActionId == actionId)
			{
				TargetTimerTime = time;
			}
			else if ((CardData.IsOn || skipDamageOnOffCheck) && (skipWorkerEnergyCheck || CardData.ShouldStartTimerWorkers(actionId)) && (skipWorkerEnergyCheck || CardData.HasEnergyInput()) && (skipWorkerEnergyCheck || CardData.HasSewerConnected()) && (!CardData.IsDamaged || skipDamageOnOffCheck))
			{
				InitTimer(time, a, status, actionId, withStatusBar);
			}
		}
	}

	private void InitTimer(float time, TimerAction a, string status, string actionId, bool withStatusBar = true)
	{
		if (withStatusBar)
		{
			Statusbar statusbar = UnityEngine.Object.Instantiate(PrefabManager.instance.StatusBarPrefab);
			statusbar.StatusTime = time;
			statusbar.ParentCard = this;
			CurrentStatusbar = statusbar;
		}
		Status = status;
		TimerRunning = true;
		TimerAction = a;
		TimerActionId = actionId;
		CurrentTimerTime = 0f;
		TargetTimerTime = time;
	}

	public void CancelTimer(string actionId)
	{
		if ((!(removedChild != null) || !removedChild.BeingDragged) && TimerRunning && !(TimerActionId != actionId))
		{
			StopTimer();
		}
	}

	private void StopTimer()
	{
		TimerRunning = false;
		TimerActionId = "";
		Status = "";
		TimerBlueprintId = "";
		TimerSubprintIndex = 0;
		CurrentTimerTime = 0f;
		SkipCitiesChecks = false;
		if (CurrentStatusbar != null)
		{
			CurrentStatusbar.DestroyMe = true;
			CurrentStatusbar = null;
		}
	}

	public void CancelAnyTimer()
	{
		if (TimerRunning)
		{
			StopTimer();
		}
	}

	public void UpdateTimer()
	{
		if (!TimerRunning)
		{
			return;
		}
		if (removedChild == null || !removedChild.BeingDragged)
		{
			CurrentTimerTime += Time.deltaTime * WorldManager.instance.TimeScale;
		}
		if (CurrentStatusbar != null)
		{
			CurrentStatusbar.Paused = removedChild != null && removedChild.BeingDragged;
		}
		if (!(CurrentTimerTime >= TargetTimerTime))
		{
			return;
		}
		TimerRunning = false;
		if (!ShouldCompleteTimer(TimerActionId))
		{
			TimerActionId = "";
			Status = "";
			TimerBlueprintId = "";
			TimerSubprintIndex = 0;
			CurrentTimerTime = 0f;
			CurrentStatusbar.DestroyMe = true;
			CurrentStatusbar = null;
			return;
		}
		try
		{
			TimerAction();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		if (TimerActionId == "finish_blueprint")
		{
			QuestManager.instance.ActionComplete(WorldManager.instance.GetBlueprintWithId(TimerBlueprintId), TimerActionId, CardData);
		}
		else
		{
			QuestManager.instance.ActionComplete(CardData, TimerActionId);
		}
		TimerActionId = "";
		Status = "";
		TimerBlueprintId = "";
		TimerSubprintIndex = 0;
		CurrentTimerTime = 0f;
		if (CurrentStatusbar != null)
		{
			CurrentStatusbar.DestroyMe = true;
		}
		CurrentStatusbar = null;
	}

	public virtual bool ShouldCompleteTimer(string timerActionId)
	{
		return CardData.ShouldCompleteTimer(timerActionId);
	}

	public bool HasTransportCard()
	{
		GameCard gameCard = GetRootCard();
		if (gameCard.CardData is HeavyFoundation && gameCard.HasChild)
		{
			gameCard = gameCard.Child;
		}
		if (gameCard.CardData is Boat || gameCard.CardData is Spirit || gameCard.CardData is Portal)
		{
			return true;
		}
		return false;
	}

	public bool ElementExistsForStatusEffect(StatusEffect effect)
	{
		foreach (StatusEffectElement statusEffectElement in StatusEffectElements)
		{
			if (statusEffectElement.MyStatusEffect == effect)
			{
				return true;
			}
		}
		return false;
	}

	public StatusEffectElement CreateElementForStatusEffect(StatusEffect effect)
	{
		StatusEffectElement statusEffectElement = UnityEngine.Object.Instantiate(PrefabManager.instance.StatusEffectElementPrefab);
		statusEffectElement.SetStatusEffect(this, effect);
		statusEffectElement.transform.SetParent(StatusEffectElementParent);
		statusEffectElement.transform.localRotation = Quaternion.identity;
		statusEffectElement.transform.localScale = Vector3.zero;
		float x = (float)StatusEffectElements.Count * DistanceBetweenStatusses - (float)(StatusEffectElements.Count - 1) * DistanceBetweenStatusses * 0.5f;
		statusEffectElement.transform.localPosition = new Vector3(x, 0f, -0.001f);
		return statusEffectElement;
	}

	public bool IsPartOfStack()
	{
		if (!(Parent != null))
		{
			return Child != null;
		}
		return true;
	}

	public GameCard GetCardWithStatusInStack()
	{
		GameCard gameCard = GetRootCard();
		while (gameCard != null)
		{
			if (gameCard.TimerRunning)
			{
				return gameCard;
			}
			gameCard = gameCard.Child;
		}
		return null;
	}

	public int GetCardIndex()
	{
		GameCard gameCard = GetRootCard();
		int num = 0;
		while (gameCard != null)
		{
			if (gameCard == this)
			{
				return num;
			}
			gameCard = gameCard.Child;
			num++;
		}
		return -1;
	}

	public GameCard GetCardInCombatInStack()
	{
		GameCard gameCard = GetRootCard();
		while (gameCard != null)
		{
			if (gameCard.Combatable != null && gameCard.Combatable.InConflict)
			{
				return gameCard;
			}
			gameCard = gameCard.Child;
		}
		return null;
	}

	public List<GameCard> GetAllCardsInStack()
	{
		GameCard rootCard = GetRootCard();
		List<GameCard> childCards = rootCard.GetChildCards();
		childCards.Insert(0, rootCard);
		return childCards;
	}

	public CardData HasCardInStack(Predicate<CardData> pred)
	{
		GameCard gameCard = GetRootCard();
		while (gameCard != null)
		{
			if (pred(gameCard.CardData))
			{
				return gameCard.CardData;
			}
			gameCard = gameCard.Child;
		}
		return null;
	}

	public bool IsPartOfSameStack(GameCard otherCard)
	{
		GameCard gameCard = GetRootCard();
		while (gameCard != null)
		{
			if (gameCard == otherCard)
			{
				return true;
			}
			gameCard = gameCard.Child;
		}
		return false;
	}

	public string GetStackSummary()
	{
		return WorldManager.instance.GetStackSummary(GetAllCardsInStack());
	}

	public bool IsChildOf(GameCard card)
	{
		if (card == null)
		{
			return false;
		}
		GameCard parent = Parent;
		while ((object)parent != null)
		{
			if (parent == card)
			{
				return true;
			}
			parent = parent.Parent;
		}
		return false;
	}

	public bool IsParentOf(GameCard card)
	{
		if (card == null)
		{
			return false;
		}
		GameCard child = Child;
		while ((object)child != null)
		{
			if (child == card)
			{
				return true;
			}
			child = child.Child;
		}
		return false;
	}

	public void SetCollidersInStack(bool enabled)
	{
		GameCard gameCard = this;
		while ((object)gameCard != null)
		{
			gameCard.boxCollider.enabled = enabled;
			gameCard = gameCard.Child;
		}
	}

	public List<GameCard> GetChildCards()
	{
		List<GameCard> list = new List<GameCard>();
		GameCard child = Child;
		while (child != null)
		{
			list.Add(child);
			child = child.Child;
		}
		return list;
	}

	public GameCard GetRootCard()
	{
		GameCard gameCard = this;
		while (gameCard.Parent != null)
		{
			gameCard = gameCard.Parent;
		}
		return gameCard;
	}

	public GameCard GetLeafCard()
	{
		GameCard gameCard = this;
		while (gameCard.Child != null)
		{
			gameCard = gameCard.Child;
		}
		return gameCard;
	}

	public int GetChildCount()
	{
		GameCard gameCard = this;
		int num = 0;
		while (gameCard.Child != null)
		{
			num++;
			gameCard = gameCard.Child;
		}
		return num;
	}

	public int GetStackCount()
	{
		GameCard gameCard = GetRootCard();
		int num = 1;
		while (gameCard.Child != null)
		{
			num++;
			gameCard = gameCard.Child;
		}
		return num;
	}

	private void NotifyChildDrag(GameCard card)
	{
		removedChild = card;
	}

	public override void StopDragging()
	{
		if (Parent != null)
		{
			AudioManager.me.PlaySound2D(AudioManager.me.DropOnStack, UnityEngine.Random.Range(0.8f, 1.2f), 0.3f);
		}
		else if (CardData.PickupSound != null && CardData.PickupSoundGroup == PickupSoundGroup.Custom)
		{
			AudioManager.me.PlaySound2D(CardData.PickupSound, UnityEngine.Random.Range(0.8f, 1f), 0.5f);
		}
		else
		{
			List<AudioClip> soundForPickupSoundGroup = AudioManager.me.GetSoundForPickupSoundGroup(CardData.PickupSoundGroup);
			AudioManager.me.PlaySound2D(soundForPickupSoundGroup, UnityEngine.Random.Range(0.8f, 1f), 0.5f);
		}
		GameCard child = Child;
		while (child != null)
		{
			child.BeingDragged = false;
			child = child.Child;
		}
		CardData.StoppedDragging();
		StackUpdate = true;
		base.StopDragging();
	}

	public override void StartDragging()
	{
		if (CardData.PickupSound != null && CardData.PickupSoundGroup == PickupSoundGroup.Custom)
		{
			AudioManager.me.PlaySound2D(CardData.PickupSound, UnityEngine.Random.Range(1f, 1.2f), 0.5f);
		}
		else
		{
			List<AudioClip> soundForPickupSoundGroup = AudioManager.me.GetSoundForPickupSoundGroup(CardData.PickupSoundGroup);
			AudioManager.me.PlaySound2D(soundForPickupSoundGroup, UnityEngine.Random.Range(1f, 1.2f), 0.5f);
		}
		GameCard parent = Parent;
		while (parent != null)
		{
			parent.NotifyChildDrag(this);
			parent = parent.Parent;
		}
		if (Parent != null)
		{
			SetParent(null);
		}
		parent = Child;
		while (parent != null)
		{
			parent.BeingDragged = true;
			parent = parent.Child;
		}
		BounceTarget = null;
		base.StartDragging();
	}

	public void Clampieee()
	{
		ClampPos();
	}

	protected override void ClampPos()
	{
		if (!IsDemoCard && SetY)
		{
			int childCount = GetChildCount();
			float b = (float)childCount * WorldManager.instance.CardOverlayOffset;
			if (IsCollapsed)
			{
				b = (float)childCount * WorldManager.instance.CollapsedCardOverlayOffset;
			}
			curHeight = Mathf.Lerp(curHeight, b, Time.deltaTime * 12f);
			base.transform.position = ClampPos2(base.transform.position);
			TargetPosition = ClampPos2(TargetPosition);
		}
	}

	public float GetHeight()
	{
		PrefabManager.instance.GameCardPrefab.boxCollider.ToWorldSpaceBox(out var _, out var halfExtents, out var _);
		return halfExtents.y * 2f;
	}

	public float GetWidth()
	{
		PrefabManager.instance.GameCardPrefab.boxCollider.ToWorldSpaceBox(out var _, out var halfExtents, out var _);
		return halfExtents.x * 2f;
	}

	public Bounds GetBounds()
	{
		return new Bounds(base.transform.position, new Vector3(GetWidth(), 0.01f, GetHeight()));
	}

	private Vector3 ClampPos2(Vector3 p)
	{
		Bounds bounds = (BeingDragged ? MyBoard.WorldBounds : MyBoard.TightWorldBounds);
		boxCollider.ToWorldSpaceBox2(out var halfExtents);
		float num = 0.1f;
		p.x = Mathf.Clamp(p.x, bounds.min.x + halfExtents.x + num, bounds.max.x - halfExtents.x - num);
		p.z = Mathf.Clamp(p.z, bounds.min.z + halfExtents.y + num + curHeight, bounds.max.z - halfExtents.y - num);
		return p;
	}

	public SavedCard ToSavedCard()
	{
		SavedCard savedCard = new SavedCard();
		savedCard.CardPosition = base.transform.position;
		savedCard.CardPrefabId = CardData.Id;
		savedCard.UniqueId = CardData.UniqueId;
		savedCard.IsFoil = CardData.IsFoil;
		savedCard.FaceUp = FaceUp;
		savedCard.IsDamaged = CardData.IsDamaged;
		savedCard.DamageType = CardData.DamageType;
		if (Parent != null)
		{
			savedCard.ParentUniqueId = Parent.CardData.UniqueId;
		}
		if (EquipmentHolder != null)
		{
			savedCard.EquipmentHolderUniqueId = EquipmentHolder.CardData.UniqueId;
		}
		if (WorkerHolder != null)
		{
			savedCard.WorkerHolderUniqueId = WorkerHolder.CardData.UniqueId;
			savedCard.WorkerIndex = CardData.WorkerIndex;
		}
		savedCard.ExtraCardData = CardData.GetExtraCardData();
		savedCard.TimerRunning = TimerRunning;
		savedCard.WithStatusBar = CurrentStatusbar != null;
		savedCard.TimerActionId = TimerActionId;
		savedCard.Status = Status;
		savedCard.CurrentTimerTime = CurrentTimerTime;
		savedCard.TargetTimerTime = TargetTimerTime;
		savedCard.TimerBlueprintId = TimerBlueprintId;
		savedCard.SkipCitiesChecks = SkipCitiesChecks;
		savedCard.SubprintIndex = TimerSubprintIndex;
		savedCard.BoardId = MyBoard.Id;
		savedCard.StatusEffects = CardData.StatusEffects.Select((StatusEffect x) => x.ToSavedStatusEffect()).ToList();
		savedCard.CardConnectors = (from x in CardConnectorChildren
			select x.ToSavedEnergyConnector() into x
			where x != null
			select x).ToList();
		return savedCard;
	}

	public void SetHitEffect(Action after = null)
	{
		IsHit = true;
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
		StartCoroutine(WaitFor(0.11f, delegate
		{
			IsHit = false;
			after?.Invoke();
		}));
	}

	public bool HasConnectorOfType(ConnectionType connectionType)
	{
		for (int i = 0; i < CardData.EnergyConnectors.Count; i++)
		{
			if (CardData.EnergyConnectors[i].EnergyConnectionStrength == connectionType)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator WaitFor(float time, Action a)
	{
		yield return new WaitForSeconds(time);
		a?.Invoke();
	}
}
