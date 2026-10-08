public class WellbeingGenerator : Landmark, IEnergyConsumer
{
	public int WellbeingAmountPerCycle;

	public int HarvestTime = 10;

	[Term]
	public string StatusResultTerm;

	[Term]
	public string StatusTerm;

	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard is Worker;
	}

	protected override bool CanToggleOnOff()
	{
		return true;
	}

	public override void UpdateCard()
	{
		if (WorkerAmountMet() && !MyGameCard.TimerRunning)
		{
			MyGameCard.StartTimer(HarvestTime, Complete, SokLoc.Translate(StatusTerm), GetActionId("Complete"));
		}
		else if (!WorkerAmountMet())
		{
			MyGameCard.CancelAnyTimer();
		}
		if (!MyGameCard.TimerRunning && !HasEnergyInput())
		{
			if (!HasStatusEffectOfType<StatusEffect_NoEnergy>())
			{
				AddStatusEffect(new StatusEffect_NoEnergy());
			}
		}
		else
		{
			RemoveStatusEffect<StatusEffect_NoEnergy>();
		}
		base.UpdateCard();
	}

	[TimedAction("complete")]
	public void Complete()
	{
		CitiesManager.instance.AddWellbeing(WellbeingAmountPerCycle);
		WorldManager.instance.CreateFloatingText(MyGameCard, WellbeingAmountPerCycle > 0, WellbeingAmountPerCycle, SokLoc.Translate(StatusResultTerm), Icons.Wellbeing, desiredBehaviour: true, 0, 0f, closeOnHover: true);
	}

	string IEnergyConsumer.GetEnergyConsumptionString()
	{
		return GetEnergyInputString();
	}
}
