using UnityEngine;

public class Battery : CardData, IEnergy
{
	public int EnergyCapacity = 50;

	[ExtraData("stored_energy")]
	public int StoredEnergy;

	public Sprite SpecialIcon;

	public int EnergyAmount => StoredEnergy;

	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard is Energy;
	}

	public override void UpdateCard()
	{
		MyGameCard.SpecialIcon.sprite = SpecialIcon;
		MyGameCard.SpecialValue = StoredEnergy;
		if (MyGameCard.HasChild)
		{
			foreach (GameCard childCard in MyGameCard.GetChildCards())
			{
				if (StoredEnergy < EnergyCapacity)
				{
					StoredEnergy++;
					childCard.DestroyCard(spawnSmoke: true);
					continue;
				}
				childCard.RemoveFromParent();
				break;
			}
		}
		base.UpdateCard();
	}

	public void UseEnergy(int energyAmount)
	{
		StoredEnergy -= energyAmount;
		WorldManager.instance.CreateMinusElectricity(base.Position);
	}

	public CardData GetCardData()
	{
		return this;
	}
}
