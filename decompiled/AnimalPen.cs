using System.Collections.Generic;
using UnityEngine;

public class AnimalPen : CardData
{
	[Header("Animals")]
	public int MaxAnimalCount = 5;

	public bool IsForFish;

	private List<CardData> animals = new List<CardData>();

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (Id == "animal_cage" && otherCard.Id == "animal_cage")
		{
			return true;
		}
		if (otherCard.Id == "wheat")
		{
			return true;
		}
		if (otherCard.Id == "egg")
		{
			return true;
		}
		if (otherCard.Id == "magic_dust" || otherCard.Id == "soil")
		{
			return true;
		}
		int num = GetChildCount() + (1 + otherCard.GetChildCount());
		if (!IsForFish)
		{
			if (otherCard is Animal && otherCard.MyCardType != CardType.Fish)
			{
				return num <= MaxAnimalCount;
			}
			return false;
		}
		if (otherCard is Animal && otherCard.MyCardType == CardType.Fish)
		{
			return num <= MaxAnimalCount;
		}
		return false;
	}

	private Animal GetAnimalInStack()
	{
		GetChildrenMatchingPredicate((CardData x) => x is Animal, animals);
		if (animals.Count == 0)
		{
			return null;
		}
		return animals.Choose() as Animal;
	}

	public override void UpdateCard()
	{
		if (AnyChildMatchesPredicate((CardData x) => x.Id == "wheat", out var _))
		{
			if (GetAnimalInStack() != null)
			{
				MyGameCard.StartTimer(5f, EatWheat, SokLoc.Translate("card_animal_eating_status"), "eat_wheat");
			}
		}
		else
		{
			MyGameCard.CancelTimer("eat_wheat");
			ShowFakeAnimalProgressBar();
		}
		base.UpdateCard();
	}

	private void ShowFakeAnimalProgressBar()
	{
		Animal firstAnimalToProduce = GetFirstAnimalToProduce();
		if (firstAnimalToProduce != null)
		{
			CardData cardPrefab = WorldManager.instance.GetCardPrefab(firstAnimalToProduce.CreateCard);
			string status = SokLoc.Translate("card_animal_pen_status", LocParam.Create("card", cardPrefab.Name), LocParam.Create("name", firstAnimalToProduce.Name));
			MyGameCard.StartTimer(firstAnimalToProduce.CreateTime, AnimalCreate, status, "animal_create");
			MyGameCard.CurrentTimerTime = firstAnimalToProduce.CreateTimer;
		}
		else
		{
			MyGameCard.CancelTimer("animal_create");
		}
	}

	[TimedAction("animal_create")]
	public void AnimalCreate()
	{
	}

	private Animal GetFirstAnimalToProduce()
	{
		GetChildrenMatchingPredicate((CardData x) => x is Animal, animals);
		Animal result = null;
		float num = float.MaxValue;
		foreach (Animal animal in animals)
		{
			if (animal.CanCreate && animal.TimeUntilCreate < num)
			{
				num = animal.TimeUntilCreate;
				result = animal;
			}
		}
		return result;
	}

	[TimedAction("eat_wheat")]
	public void EatWheat()
	{
		Animal animalInStack = GetAnimalInStack();
		if (AnyChildMatchesPredicate((CardData x) => x.Id == "wheat", out var match) && animalInStack != null)
		{
			animalInStack.ConsumeWheat(match);
		}
	}
}
