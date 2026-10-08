public class CardValue
{
	public int BaseValue;

	public int ExtraValue;

	public int TotalValue => BaseValue + ExtraValue;

	public CardValue(int baseValue)
	{
		BaseValue = baseValue;
	}

	public string ToValueString(GameBoard currentBoard)
	{
		string arg = Icons.Gold;
		if (currentBoard.Location == Location.Island)
		{
			arg = Icons.Shell;
		}
		else if (currentBoard.Location == Location.Cities)
		{
			arg = Icons.Dollar;
		}
		return "" + $"{BaseValue} {arg}";
	}
}
