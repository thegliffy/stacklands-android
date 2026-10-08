using System;

[Serializable]
public class DemandEvent
{
	public string DemandId;

	public int MonthStarted;

	public int Duration;

	public string BoardId;

	public bool Completed;

	public bool Successful;

	public int AmountGiven;

	public Demand Demand => DemandManager.instance.GetDemandById(DemandId);

	public int MonthCompleted => Duration + MonthStarted;

	public DemandEvent()
	{
	}

	public DemandEvent(string demandId, int monthStarted, int duration, string boardId)
	{
		DemandId = demandId;
		MonthStarted = monthStarted;
		Duration = duration;
		BoardId = boardId;
	}
}
