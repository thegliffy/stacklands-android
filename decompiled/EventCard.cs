using UnityEngine;

public class EventCard : CardData
{
	public bool IsPositiveEvent;

	public float PreEventTime;

	[Term]
	public string PreEventText;

	[Term]
	public string EventText;

	[HideInInspector]
	public bool ShouldStartEvent;

	public CardEventType EventType;

	[ExtraData("event_is_active")]
	public bool EventIsActive;

	public AudioClip EventStartOverride;

	public override void OnInitialCreate()
	{
		if (!MyGameCard.TimerRunning)
		{
			MyGameCard.StartTimer(PreEventTime, StartEvent, SokLoc.Translate(PreEventText), GetActionId("StartEvent"));
		}
		if (IsPositiveEvent)
		{
			AudioManager.me.PlaySound((EventStartOverride != null) ? EventStartOverride : AudioManager.me.PositiveEventSpawn, base.transform, Random.Range(0.9f, 1.1f), 0.5f);
		}
		else
		{
			AudioManager.me.PlaySound((EventStartOverride != null) ? EventStartOverride : AudioManager.me.NegativeEventSpawn, base.transform, Random.Range(0.9f, 1.1f), 0.5f);
		}
		base.OnInitialCreate();
	}

	public override void UpdateCard()
	{
		if (ShouldStartEvent && !MyGameCard.TimerRunning)
		{
			ExecuteEvent();
		}
		ShouldStartEvent = false;
		base.UpdateCard();
	}

	[TimedAction("start_disaster")]
	public void StartEvent()
	{
		ShouldStartEvent = true;
		QuestManager.instance.SpecialActionComplete("event_disaster", this);
	}

	protected virtual void ExecuteEvent()
	{
	}

	protected virtual void EndEvent()
	{
		MyGameCard.DestroyCard(spawnSmoke: true);
	}
}
