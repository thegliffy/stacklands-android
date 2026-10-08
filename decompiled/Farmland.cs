using UnityEngine;

public class Farmland : CardData
{
	public bool CanDeplete;

	[Card]
	public string HarvestCardId;

	public int HarvestAmount = 3;

	[HideInInspector]
	[ExtraData("amount_harvested")]
	public int AmountHarvested;

	public bool IsDepleted;

	public float DepletedTime = 30f;

	public float HarvestTime = 10f;

	public AudioClip WateringSound;

	public Sprite DepletedIcon;

	public Sprite NormalIcon;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Worker))
		{
			return otherCard.Id == "water";
		}
		return true;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanToggleOnOff()
	{
		return true;
	}

	public override void UpdateCard()
	{
		if (!IsDepleted)
		{
			RemoveStatusEffect<StatusEffect_Depleted>();
			if (!WorkerAmountMet() && MyGameCard.TimerRunning)
			{
				MyGameCard.CancelTimer(GetActionId("Harvest"));
			}
			if (!CanDeplete || AmountHarvested < HarvestAmount)
			{
				if (WorkerAmountMet() && !MyGameCard.TimerRunning)
				{
					MyGameCard.StartTimer(HarvestTime, Harvest, SokLoc.Translate("card_farmland_status"), GetActionId("Harvest"));
				}
			}
			else
			{
				if (CanDeplete)
				{
					IsDepleted = true;
				}
				AmountHarvested = 0;
			}
			MyGameCard.IconRenderer.sprite = NormalIcon;
		}
		else
		{
			AddStatusEffect(new StatusEffect_Depleted());
			if (MyGameCard.HasChild && ChildrenMatchingPredicateCount((CardData x) => x.Id == "water") >= 1)
			{
				if (!MyGameCard.TimerRunning)
				{
					MyGameCard.StartTimer(DepletedTime, WaterFarmland, SokLoc.Translate("card_farmland_status_0"), GetActionId("WaterFarmland"));
				}
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("WaterFarmland"));
			}
			MyGameCard.IconRenderer.sprite = DepletedIcon;
		}
		MyGameCard.UpdateIcon();
		base.UpdateCard();
	}

	[TimedAction("harvest")]
	public void Harvest()
	{
		AmountHarvested++;
		CardData cardData = WorldManager.instance.CreateCard(base.Position, HarvestCardId, faceUp: true, checkAddToStack: false);
		WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
	}

	[TimedAction("water_farmland")]
	public void WaterFarmland()
	{
		DestroyChildrenMatchingPredicateAndRestack((CardData x) => x.Id == "water", 1);
		IsDepleted = false;
		AudioManager.me.PlaySound2D(WateringSound, 1f, 0.3f);
	}

	protected override bool CanSelectOutput()
	{
		return true;
	}
}
