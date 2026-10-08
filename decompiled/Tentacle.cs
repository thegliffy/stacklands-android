using UnityEngine;

public class Tentacle : Enemy
{
	protected override void Move()
	{
		Vector2 vector = Random.insideUnitCircle.normalized * 1f;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.y);
		MyGameCard.Velocity = new Vector3(vector2.x, 0f, vector2.z);
	}

	public override void Die()
	{
		bool flag = false;
		if (WorldManager.instance.GetCardCount<Tentacle>() == 1)
		{
			flag = true;
		}
		if (flag)
		{
			WorldManager.instance.QueueCutscene(Cutscenes.SpawnKraken());
		}
		base.Die();
	}
}
