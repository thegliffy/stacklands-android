using UnityEngine;

public class Food : CardData
{
	[Header("Food")]
	[ExtraData("food_value")]
	public int FoodValue = 1;

	public bool CanBePlacedOnVillager;

	[ExtraData("spoil_time")]
	[HideInInspector]
	public float SpoilTime;

	public bool CanSpoil = true;

	[Header("Special Actions")]
	public string ResultAction;

	public string FullyConsumeResultAction;

	[HideInInspector]
	public bool IsReserved;

	[HideInInspector]
	public bool IsConsumed;

	public bool IsSpoiling => HasStatusEffectOfType<StatusEffect_Spoiling>();

	public override void UpdateCard()
	{
		if (FoodValue > 0)
		{
			MyGameCard.SpecialValue = FoodValue;
			MyGameCard.SpecialIcon.sprite = SpriteManager.instance.FoodIcon;
		}
		if (FoodValue <= 0)
		{
			FoodValue = 0;
		}
		if (!MyGameCard.IsDemoCard && MyGameCard.MyBoard.BoardOptions.FoodSpoils && !IsSpoiling && CanSpoil)
		{
			SpoilTime += Time.deltaTime * WorldManager.instance.TimeScale;
			float num = WorldManager.instance.MonthTime;
			if (IsCookedFood)
			{
				num = WorldManager.instance.MonthTime * 2f;
			}
			if (SpoilTime >= num)
			{
				AddStatusEffect(new StatusEffect_Spoiling());
			}
		}
		base.UpdateCard();
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Food) && otherCard.MyCardType != CardType.Resources)
		{
			if (otherCard is BaseVillager || otherCard is Worker)
			{
				return CanBePlacedOnVillager;
			}
			return false;
		}
		return true;
	}

	public virtual void ConsumedBy(Combatable vill)
	{
		vill.ParseAction(ResultAction);
	}

	private bool CanGiveFoodPoisoning()
	{
		return WorldManager.instance.CurseIsActive(CurseType.Death);
	}

	public virtual void FullyConsumed(CardData c)
	{
		c.ParseAction(FullyConsumeResultAction);
	}

	public override void OnSellCard()
	{
		if (WorldManager.instance.CurseIsActive(CurseType.Happiness) && FoodValue > 0)
		{
			WorldManager.instance.TryCreateUnhappiness(base.transform.position, 1);
			WorldManager.instance.QueueCutsceneIfNotPlayed("happiness_food");
		}
		base.OnSellCard();
	}
}
