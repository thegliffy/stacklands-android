using UnityEngine;

public class SewerCard : CardData
{
	[ExtraData("poop_timer")]
	[HideInInspector]
	public float PoopTimer;

	private bool shouldRunTimer;

	private bool HasSewerConnector()
	{
		foreach (CardConnectorData energyConnector in EnergyConnectors)
		{
			if (energyConnector.EnergyConnectionStrength == ConnectionType.Sewer)
			{
				return true;
			}
		}
		return false;
	}

	public override void UpdateCard()
	{
		if (HasSewerConnector())
		{
			if (!HasSewerConnected())
			{
				if (!HasStatusEffectOfType<StatusEffect_NoSewer>())
				{
					AddStatusEffect(new StatusEffect_NoSewer());
				}
				shouldRunTimer = true;
			}
			else
			{
				RemoveStatusEffect<StatusEffect_NoSewer>();
				shouldRunTimer = false;
			}
		}
		CheckSpawnPoop();
		base.UpdateCard();
	}

	[TimedAction("check_spawn_poop")]
	public void CheckSpawnPoop()
	{
		if (shouldRunTimer)
		{
			PoopTimer += Time.deltaTime * WorldManager.instance.TimeScale;
		}
		if (PoopTimer >= 30f && (double)Random.value > 0.5)
		{
			CardData cardData = WorldManager.instance.CreateCard(base.Position, "poop");
			WorldManager.instance.StackSend(cardData.MyGameCard, OutputDir);
			PoopTimer = 0f;
		}
	}
}
