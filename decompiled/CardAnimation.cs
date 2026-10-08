using UnityEngine;

public class CardAnimation
{
	public Vector3 StartPosition;

	public Vector3 EndPosition;

	public Vector3 Position;

	public Vector3 TargetPosition;

	public bool IsDone;

	public bool HasStarted;

	protected float timer;

	public bool IsBlocking = true;

	public virtual void Start()
	{
		Position = (TargetPosition = StartPosition);
	}

	public virtual void Update()
	{
		timer += Time.deltaTime;
	}
}
