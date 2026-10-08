using UnityEngine;

public class AttackAnimationMelee : AttackAnimation
{
	private bool attacked;

	private float timer;

	public override bool IsBlocking => true;

	public override void Update()
	{
		timer += Time.deltaTime * WorldManager.instance.TimeScale * WorldManager.instance.CombatSpeed;
		float t = WorldManager.instance.CombatFlatPositionCurve.Evaluate(timer);
		float num = WorldManager.instance.CombatYPosition.Evaluate(timer);
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Lerp(AttackStartPosition.x, AttackTargetPosition.x, t);
		zero.y = AttackTargetPosition.y + num;
		zero.z = Mathf.Lerp(AttackStartPosition.z, AttackTargetPosition.z, t);
		Position = (TargetPosition = zero);
		if (timer >= 0.5f && !attacked)
		{
			attacked = true;
			Origin.PerformAttack(Target, AttackTargetPosition);
		}
		if (timer >= 1f)
		{
			IsDone = true;
		}
		base.Update();
	}
}
