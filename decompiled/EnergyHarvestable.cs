using System.Linq;
using UnityEngine;

public class EnergyHarvestable : Harvestable, IEnergyConsumer
{
	[Header("Cities options")]
	public int PollutionPerHarvest;

	public override void OnInitialCreate()
	{
		if (MyGameCard.CardConnectorChildren.Count((CardConnector x) => x.CardDirection == CardDirection.input && x.ConnectionType == ConnectionType.LV) > 0)
		{
			WorldManager.instance.QueueCutsceneIfNotPlayed("cities_first_energy");
		}
		base.OnInitialCreate();
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		return base.CanHaveCard(otherCard);
	}

	public override void OnHarvestComplete()
	{
		if (PollutionPerHarvest > 0)
		{
			(WorldManager.instance.CreateCard(base.Position, "pollution", faceUp: true, checkAddToStack: false) as Pollution).PollutionAmount = PollutionPerHarvest;
		}
		if (Id == "uranium_mine")
		{
			CardData cardData = WorldManager.instance.CreateCard(base.Position, "gravel", faceUp: true, checkAddToStack: false);
			SendCard(cardData.MyGameCard);
		}
	}

	public override void SendCard(GameCard card)
	{
		WorldManager.instance.StackSendCheckTarget(MyGameCard, card, OutputDir);
	}

	protected override bool CanSelectOutput()
	{
		return true;
	}

	protected override bool CanToggleOnOff()
	{
		return true;
	}

	protected virtual bool CanStartHarvesting()
	{
		return true;
	}

	public override void UpdateCard()
	{
		if (!WorkerAmountMet())
		{
			MyGameCard.CancelTimer(GetActionId("CompleteHarvest"));
		}
		else if (WorkerAmountMet() && RequiredVillagerCount <= 0 && !MyGameCard.TimerRunning && CanStartHarvesting())
		{
			MyGameCard.StartTimer(HarvestTime, base.CompleteHarvest, base.StatusText, GetActionId("CompleteHarvest"));
		}
		if (!HasEnergyInput())
		{
			if (!HasStatusEffectOfType<StatusEffect_NoEnergy>())
			{
				AddStatusEffect(new StatusEffect_NoEnergy());
			}
			MyGameCard.CancelTimer(GetActionId("CompleteHarvest"));
		}
		else if (HasStatusEffectOfType<StatusEffect_NoEnergy>())
		{
			RemoveStatusEffect<StatusEffect_NoEnergy>();
		}
		if (!HasSewerConnected())
		{
			if (!HasStatusEffectOfType<StatusEffect_NoSewer>())
			{
				AddStatusEffect(new StatusEffect_NoSewer());
			}
			MyGameCard.CancelTimer(GetActionId("CompleteHarvest"));
		}
		else if (HasStatusEffectOfType<StatusEffect_NoSewer>())
		{
			RemoveStatusEffect<StatusEffect_NoSewer>();
		}
		base.UpdateCard();
	}

	string IEnergyConsumer.GetEnergyConsumptionString()
	{
		return GetEnergyInputString();
	}
}
