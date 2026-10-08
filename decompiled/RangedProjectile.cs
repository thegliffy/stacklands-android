using UnityEngine;

public class RangedProjectile : Projectile
{
	protected override void Update()
	{
		position += (TargetPosition - StartPosition).normalized * Speed * Time.deltaTime * WorldManager.instance.TimeScale;
		base.Update();
	}
}
