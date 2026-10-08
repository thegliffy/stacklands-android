using UnityEngine;

public class MagicProjectile : Projectile
{
	public float WaitTime = 0.5f;

	private float waitTimer;

	private Vector3 startScale;

	private bool knockedBack;

	protected override void Start()
	{
		base.Start();
		startScale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
	}

	protected override void Update()
	{
		Vector3 vector = TargetPosition - StartPosition;
		waitTimer += Time.deltaTime * WorldManager.instance.TimeScale;
		if (waitTimer >= WaitTime)
		{
			position += vector.normalized * Speed * Time.deltaTime * WorldManager.instance.TimeScale;
			if (!knockedBack)
			{
				knockedBack = true;
				AudioManager.me.PlaySound2D(AudioManager.me.MagicRelease, Random.Range(0.8f, 1.2f), 0.5f);
				OriginAnimation.SetKnockback(this);
			}
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, startScale, Time.deltaTime * 6f);
		base.Update();
	}
}
