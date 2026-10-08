using System;

public class BoardMonths
{
	public int MainMonth;

	public int IslandMonth;

	public int ForestMonth;

	public int GreedMonth;

	public int HappinessMonth;

	public int DeathMonth;

	public int CitiesMonth;

	public bool IsEmpty
	{
		get
		{
			if (MainMonth <= 1 && IslandMonth <= 0 && ForestMonth <= 1 && GreedMonth <= 1 && HappinessMonth <= 1)
			{
				return DeathMonth <= 1;
			}
			return false;
		}
	}

	public BoardMonths()
	{
		MainMonth = 1;
		IslandMonth = 0;
		ForestMonth = 1;
		GreedMonth = 1;
		HappinessMonth = 1;
		DeathMonth = 1;
		CitiesMonth = 1;
	}

	public BoardMonths(SavedMonth saved)
	{
		MainMonth = saved.MainMonth;
		IslandMonth = saved.IslandMonth;
		ForestMonth = saved.ForestMonth;
		GreedMonth = saved.GreedMonth;
		HappinessMonth = saved.HappinessMonth;
		DeathMonth = saved.DeathMonth;
		CitiesMonth = saved.CitiesMonth;
	}

	public int GetCurrentMonth()
	{
		GameBoard currentBoard = WorldManager.instance.CurrentBoard;
		if (currentBoard != null)
		{
			if (currentBoard.Id == "main")
			{
				return MainMonth + IslandMonth;
			}
			if (currentBoard.Id == "island")
			{
				return MainMonth + IslandMonth;
			}
			if (currentBoard.Id == "forest")
			{
				return ForestMonth;
			}
			if (currentBoard.Id == "greed")
			{
				return GreedMonth;
			}
			if (currentBoard.Id == "happiness")
			{
				return HappinessMonth;
			}
			if (currentBoard.Id == "death")
			{
				return DeathMonth;
			}
			if (currentBoard.Id == "cities")
			{
				return CitiesMonth;
			}
			throw new Exception("Board is not implemented in the BoardMonths");
		}
		return 0;
	}

	public void IncrementMonth()
	{
		GameBoard currentBoard = WorldManager.instance.CurrentBoard;
		if (!(currentBoard != null))
		{
			return;
		}
		if (currentBoard.Id == "main")
		{
			MainMonth++;
			return;
		}
		if (currentBoard.Id == "island")
		{
			IslandMonth++;
			return;
		}
		if (currentBoard.Id == "forest")
		{
			ForestMonth++;
			return;
		}
		if (currentBoard.Id == "greed")
		{
			GreedMonth++;
			return;
		}
		if (currentBoard.Id == "happiness")
		{
			HappinessMonth++;
			return;
		}
		if (currentBoard.Id == "death")
		{
			DeathMonth++;
			return;
		}
		if (currentBoard.Id == "cities")
		{
			CitiesMonth++;
			return;
		}
		throw new Exception("Board is not implemented in the BoardMonths");
	}

	public SavedMonth ToSavedMonth()
	{
		return new SavedMonth
		{
			MainMonth = MainMonth,
			IslandMonth = IslandMonth,
			ForestMonth = ForestMonth,
			GreedMonth = GreedMonth,
			HappinessMonth = HappinessMonth,
			DeathMonth = DeathMonth,
			CitiesMonth = CitiesMonth
		};
	}

	public void ResetMonths()
	{
		MainMonth = 1;
		IslandMonth = 0;
		ForestMonth = 1;
		GreedMonth = 1;
		HappinessMonth = 1;
		DeathMonth = 1;
		CitiesMonth = 1;
	}
}
