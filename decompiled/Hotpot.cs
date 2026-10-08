using UnityEngine;

public class Hotpot : Food
{
	private int MaxFoodValue = 50;

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is Food { FoodValue: 0 })
		{
			return false;
		}
		if (otherCard.MyCardType == CardType.Food)
		{
			return true;
		}
		return false;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	public override void UpdateCard()
	{
		MyGameCard.SpecialValue = FoodValue;
		MyGameCard.SpecialIcon.sprite = SpriteManager.instance.FoodIcon;
		if (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation)
		{
			if (MyGameCard.HasChild && !MyGameCard.TimerRunning && !(MyGameCard.Child.CardData is Hotpot))
			{
				MyGameCard.StartTimer(10f, CookFood, SokLoc.Translate("card_hotpot_name"), GetActionId("CookFood"));
			}
			if (!MyGameCard.HasChild && MyGameCard.TimerRunning)
			{
				MyGameCard.CancelTimer(GetActionId("CookFood"));
			}
		}
		GameCard rootCard = MyGameCard.GetRootCard();
		if (rootCard != null && rootCard.CardData is MessHall)
		{
			MyGameCard.CancelTimer(GetActionId("CookFood"));
		}
		if (FoodValue > 0)
		{
			descriptionOverride = "";
		}
		base.UpdateCard();
	}

	[TimedAction("cook_food")]
	public void CookFood()
	{
		foreach (GameCard childCard in MyGameCard.GetChildCards())
		{
			if (childCard.CardData is Hotpot)
			{
				continue;
			}
			if (childCard.SpecialValue.HasValue && FoodValue + childCard.SpecialValue <= MaxFoodValue)
			{
				FoodValue += childCard.SpecialValue.Value;
				childCard.DestroyCard(spawnSmoke: true);
			}
			else if (childCard.CardData is Food food)
			{
				int num = Mathf.Min(MaxFoodValue - FoodValue, food.FoodValue);
				FoodValue += num;
				food.FoodValue -= num;
				if (food.FoodValue <= 0)
				{
					childCard.DestroyCard(spawnSmoke: true);
				}
			}
		}
	}
}
