using UnityEngine;

public class AttackAnimationMagic : AttackAnimation
{
	public override void Start()
	{
		Origin.CreateProjectile(PrefabManager.instance.MagicProjectilePrefab, Target, this);
		AudioManager.me.PlaySound2D(AudioManager.me.MagicCharge, Random.Range(0.8f, 1.2f), 0.5f);
		base.Start();
	}

	public override void Update()
	{
		Position = (TargetPosition = AttackStartPosition + knockback);
		base.Update();
	}
}
