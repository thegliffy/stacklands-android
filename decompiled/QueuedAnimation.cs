using System;

public class QueuedAnimation
{
	public Action OnActivate;

	public string Id;

	public QueuedAnimation(Action act, string id = null)
	{
		OnActivate = act;
		Id = id;
	}
}
