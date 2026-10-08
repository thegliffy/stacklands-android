using UnityEngine;

public class Animal : Mob
{
	[Header("Animal")]
	public float CreateTime = 10f;

	[Card]
	public string CreateCard;

	public bool IsBreedable;

	[ExtraData("age")]
	public int Age;

	public bool IsOld;

	[ExtraData("createtimer")]
	[HideInInspector]
	public float CreateTimer;

	[ExtraData("itemscreated")]
	[HideInInspector]
	public int ItemsCreated;

	public override bool CanBeDragged => true;

	public override bool CanMove
	{
		get
		{
			if (!InAnimalPen && !MyGameCard.HasParent && !MyGameCard.HasChild)
			{
				return MyGameCard.GetCardWithStatusInStack() == null;
			}
			return false;
		}
	}

	public AnimalPen RootPen
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as AnimalPen;
		}
	}

	public BreedingPen RootBreedingPen
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as BreedingPen;
		}
	}

	public ResourceMagnet RootMagnet
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as ResourceMagnet;
		}
	}

	public SlaughterHouse RootSlaughterHouse
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as SlaughterHouse;
		}
	}

	public PettingZoo RootPettingZoo
	{
		get
		{
			if (!MyGameCard.HasParent)
			{
				return null;
			}
			return rootStructure as PettingZoo;
		}
	}

	private CardData rootStructure
	{
		get
		{
			GameCard gameCard = MyGameCard.GetRootCard();
			if (gameCard.CardData is HeavyFoundation && gameCard.HasChild)
			{
				gameCard = gameCard.Child;
			}
			return gameCard.CardData;
		}
	}

	public bool InAnimalPen
	{
		get
		{
			if (!(RootPen != null) && !(RootBreedingPen != null) && !(RootMagnet != null) && !(RootSlaughterHouse != null))
			{
				return RootPettingZoo != null;
			}
			return true;
		}
	}

	public virtual bool CanCreate
	{
		get
		{
			if (string.IsNullOrEmpty(CreateCard))
			{
				return false;
			}
			if (base.InConflict)
			{
				return false;
			}
			return true;
		}
	}

	public float TimeUntilCreate
	{
		get
		{
			if (!CanCreate)
			{
				return -1f;
			}
			return CreateTime - CreateTimer;
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is NamingStone)
		{
			return true;
		}
		if (otherCard is Animal)
		{
			return false;
		}
		if (otherCard.Id == "wheat")
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}

	public override void UpdateCardText()
	{
		if (!string.IsNullOrEmpty(CustomName))
		{
			if (IsOld)
			{
				nameOverride = SokLoc.Translate("card_animal_old_name", LocParam.Create("name", CustomName));
			}
			else
			{
				nameOverride = CustomName;
			}
		}
		else if (IsOld)
		{
			nameOverride = SokLoc.Translate(NameTerm + "_old");
		}
		else
		{
			nameOverride = SokLoc.Translate(NameTerm);
		}
		base.UpdateCardText();
	}

	[TimedAction("eat_wheat")]
	private void EatWheat()
	{
		if (HasCardOnTop("wheat", out var cardData))
		{
			ConsumeWheat(cardData);
		}
	}

	public void ConsumeWheat(CardData wheat)
	{
		MyGameCard.GetRootCard().CardData.RestackChildrenMatchingPredicate((CardData x) => x == wheat);
		wheat.MyGameCard.DestroyCard();
		CreateTimer = 0f;
		TryCreateItem();
		MyGameCard.RotWobble(1f);
	}

	public override void UpdateCard()
	{
		base.UpdateCard();
		if (CanCreate)
		{
			CreateTimer += Time.deltaTime * WorldManager.instance.TimeScale;
		}
		if (CreateTimer >= CreateTime && (moveFlag || InAnimalPen || MyGameCard.GetRootCard().CardData is HeavyFoundation))
		{
			CreateTimer -= CreateTime;
			TryCreateItem();
		}
		if (HasCardOnTop("wheat", out var _) && !InAnimalPen)
		{
			MyGameCard.StartTimer(5f, EatWheat, SokLoc.Translate("card_animal_eating_status"), "eat_wheat");
		}
		else
		{
			MyGameCard.CancelTimer("eat_wheat");
		}
		if (!MyGameCard.BeingDragged && !InAnimalPen && !base.InConflict && HasCardOnTop(out Animal card))
		{
			card.MyGameCard.RemoveFromStack();
		}
	}

	private void TryCreateItem()
	{
		if (!CanCreate)
		{
			return;
		}
		CardData cardData = WorldManager.instance.CreateCard(MyGameCard.transform.position, CreateCard, faceUp: true, checkAddToStack: false);
		if (RootBreedingPen != null || RootSlaughterHouse != null || RootPen != null)
		{
			WorldManager.instance.StackSendCheckTarget(rootStructure.MyGameCard, cardData.MyGameCard, OutputDir);
		}
		else
		{
			WorldManager.instance.StackSend(cardData.MyGameCard, OutputDir);
		}
		ItemsCreated++;
		if (WorldManager.instance.CurseIsActive(CurseType.Death) && ItemsCreated % 4 == 0 && CreateCard != "poop")
		{
			CardData cardData2 = WorldManager.instance.CreateCard(MyGameCard.transform.position, "poop", faceUp: true, checkAddToStack: false);
			if (RootBreedingPen != null || RootSlaughterHouse != null || RootPen != null)
			{
				WorldManager.instance.StackSendCheckTarget(rootStructure.MyGameCard, cardData2.MyGameCard, OutputDir);
			}
			else
			{
				WorldManager.instance.StackSend(cardData2.MyGameCard, OutputDir);
			}
		}
	}

	public override void Clicked()
	{
		if (!InAnimalPen)
		{
			if (!MyGameCard.Velocity.HasValue)
			{
				MoveTimer = MoveTime;
			}
			base.Clicked();
		}
	}

	protected override void Move()
	{
		AudioManager.me.PlaySound2D(AudioManager.me.AnimalMove, Random.Range(0.8f, 1.2f), 0.2f);
		base.Move();
	}

	public override void Die()
	{
		if (!IsAggressive)
		{
			WorldManager.instance.TryCreateUnhappiness(base.transform.position, 2);
		}
		base.Die();
	}

	private bool CanGrowOld()
	{
		return WorldManager.instance.CurseIsActive(CurseType.Death);
	}
}
