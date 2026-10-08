using UnityEngine;

public class Projectile : MonoBehaviour
{
	[HideInInspector]
	public Vector3 StartPosition;

	[HideInInspector]
	public Vector3 TargetPosition;

	[HideInInspector]
	public Combatable ShotBy;

	[HideInInspector]
	public Combatable Target;

	public AttackAnimation OriginAnimation;

	public float KnockbackMultiplier = 0.3f;

	public float Speed = 3f;

	public float WobbleSpeed = 1f;

	public float WobbleAmplitude = 0.1f;

	private float timer2;

	protected Vector3 position;

	private float distanceToTravel;

	protected virtual void Start()
	{
		position = base.transform.position;
		distanceToTravel = (TargetPosition - StartPosition).magnitude;
	}

	protected virtual void Update()
	{
		Vector3 forward = TargetPosition - StartPosition;
		timer2 += Time.deltaTime;
		base.transform.position = position + Extensions.Perlin(timer2 * WobbleSpeed) * WobbleAmplitude;
		base.transform.rotation = Quaternion.LookRotation(forward);
		if ((position - StartPosition).magnitude >= distanceToTravel)
		{
			if (ShotBy != null)
			{
				ShotBy.PerformAttack(Target, OriginAnimation.AttackTargetPosition);
				OriginAnimation.IsDone = true;
			}
			Object.Destroy(base.gameObject);
		}
	}
}
