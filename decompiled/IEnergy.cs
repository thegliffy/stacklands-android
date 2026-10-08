public interface IEnergy
{
	int EnergyAmount { get; }

	void UseEnergy(int energyAmount);

	CardData GetCardData();
}
