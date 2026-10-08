using UnityEngine;

public class CardAnimation_FakeMeleeAttack : CardAnimation
{
	private GameCard startCard;

	private GameCard endCard;

	private bool attacked;

	public CardAnimation_FakeMeleeAttack(GameCard start, GameCard end)
	{
		startCard = start;
		endCard = end;
		StartPosition = startCard.Position;
		EndPosition = endCard.Position;
		Position = (TargetPosition = StartPosition);
	}

	public override void Update()
	{
		timer += Time.deltaTime * WorldManager.instance.CombatSpeed;
		float t = WorldManager.instance.CombatFlatPositionCurve.Evaluate(timer);
		float num = WorldManager.instance.CombatYPosition.Evaluate(timer);
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Lerp(StartPosition.x, EndPosition.x, t);
		zero.y = EndPosition.y + num;
		zero.z = Mathf.Lerp(StartPosition.z, EndPosition.z, t);
		Position = (TargetPosition = zero);
		if (timer >= 0.5f && !attacked)
		{
			attacked = true;
			endCard.SetHitEffect();
			AudioManager.me.PlaySound2D(AudioManager.me.HitMelee, Random.Range(0.8f, 1.2f), 0.2f);
		}
		if (timer >= 1f)
		{
			IsDone = true;
		}
	}
}
