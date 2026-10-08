public interface ICurrency
{
	int CurrencyValue { get; set; }

	CardData Card { get; }

	void UseCurrency(int currencyAmount, bool spawnSmoke);
}
