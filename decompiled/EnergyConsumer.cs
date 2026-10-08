using System.Linq;

public class EnergyConsumer : SewerCard, IEnergyConsumer
{
	public override void OnInitialCreate()
	{
		if (MyGameCard.CardConnectorChildren.Count((CardConnector x) => x.CardDirection == CardDirection.input && x.ConnectionType == ConnectionType.LV) > 0)
		{
			WorldManager.instance.QueueCutsceneIfNotPlayed("cities_first_energy");
		}
		base.OnInitialCreate();
	}

	protected override bool CanToggleOnOff()
	{
		return true;
	}

	protected override bool CanSelectOutput()
	{
		return true;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	public override void UpdateCard()
	{
		if (!WorkerAmountMet() && MyGameCard.TimerRunning && !MyGameCard.SkipCitiesChecks)
		{
			MyGameCard.CancelAnyTimer();
		}
		if (!HasEnergyInput())
		{
			if (!HasStatusEffectOfType<StatusEffect_NoEnergy>())
			{
				AddStatusEffect(new StatusEffect_NoEnergy());
			}
			if (MyGameCard.TimerRunning && !MyGameCard.SkipCitiesChecks)
			{
				MyGameCard.CancelAnyTimer();
			}
		}
		else
		{
			RemoveStatusEffect<StatusEffect_NoEnergy>();
		}
		base.UpdateCard();
	}

	string IEnergyConsumer.GetEnergyConsumptionString()
	{
		return GetEnergyInputString();
	}
}
