public class Slime : Enemy
{
	public override void Die()
	{
		for (int i = 0; i < 3; i++)
		{
			WorldManager.instance.CreateCard(base.transform.position, "small_slime", faceUp: true, checkAddToStack: false).MyGameCard.SendIt();
		}
		base.Die();
	}
}
