using UnityEngine;

public class AttackAnimation
{
	public bool HasStarted;

	public bool IsDone;

	public Combatable Origin;

	public Combatable Target;

	public Vector3 AttackStartPosition;

	public Vector3 AttackTargetPosition;

	public Vector3 Position;

	public Vector3 TargetPosition;

	protected Vector3 knockback;

	public virtual bool IsBlocking { get; }

	public virtual void Start()
	{
		HasStarted = true;
		Position = (TargetPosition = AttackStartPosition);
	}

	public virtual void Update()
	{
		knockback = Vector3.Lerp(knockback, Vector3.zero, Time.deltaTime * 6f * WorldManager.instance.TimeScale);
	}

	public virtual void SetKnockback(Projectile p)
	{
		knockback = -(p.TargetPosition - p.StartPosition).normalized * p.KnockbackMultiplier;
		if (Origin != null)
		{
			Origin.MyGameCard.transform.localScale *= 0.9f;
		}
	}
}
