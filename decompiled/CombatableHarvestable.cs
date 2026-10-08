using UnityEngine;

public class CombatableHarvestable : CardData
{
	[Header("Harvestable")]
	public string StatusTerm;

	[ExtraData("amount")]
	public int Amount = 3;

	public bool IsUnlimited;

	public float HarvestTime = 10f;

	public CardBag MyCardBag;

	public string StatusText => SokLoc.Translate(StatusTerm);

	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard is BaseVillager;
	}

	public override void SetFoil()
	{
		base.SetFoil();
	}

	public override void UpdateCard()
	{
		if (HasCardOnTop(out BaseVillager card))
		{
			string actionId = GetActionId("CompleteHarvest");
			MyGameCard.StartTimer(card.GetActionTimeModifier(actionId, this) * HarvestTime, CompleteHarvest, StatusText, actionId);
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("CompleteHarvest"));
		}
		base.UpdateCard();
	}

	[TimedAction("complete_harvest")]
	public void CompleteHarvest()
	{
		if (!IsUnlimited)
		{
			Amount--;
		}
		CardData cardData = WorldManager.instance.CreateCard(MyGameCard.transform.position, MyCardBag.GetCard(), faceUp: false, checkAddToStack: false);
		WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
		if (HasCardOnTop(out BaseVillager card))
		{
			card.MyGameCard.RotWobble(0.5f);
		}
		if (!IsUnlimited && Amount <= 0)
		{
			MyGameCard.DestroyCard(spawnSmoke: true);
		}
	}
}
