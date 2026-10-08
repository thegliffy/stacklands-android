using UnityEngine;

public class Enemy : Mob
{
	protected override void Move()
	{
		Vector3 vector;
		if (CurrentTarget != null)
		{
			vector = base.transform.position - CurrentTarget.transform.position;
			vector.y = 0f;
			vector.Normalize();
			Debug.DrawLine(base.transform.position, base.transform.position - vector, Color.red, 1f);
			vector = Wiggle(vector, 45f);
			Debug.DrawLine(base.transform.position, base.transform.position - vector, Color.green, 1f);
			vector = -vector * 4f;
		}
		else if (WorldManager.instance.CurrentBoard.Id == "cities")
		{
			vector = Vector3.zero;
		}
		else
		{
			Vector2 vector2 = Random.insideUnitCircle.normalized * 4f;
			vector = new Vector3(vector2.x, 0f, vector2.y);
		}
		MyGameCard.Velocity = new Vector3(vector.x, 0f, vector.z);
	}

	public override void UpdateCard()
	{
		CardData cardData2;
		if (Id == "wolf")
		{
			if (HasCardOnTop("bone", out var cardData))
			{
				cardData.MyGameCard.DestroyCard();
				MyGameCard.DestroyCard();
				WorldManager.instance.CreateCard(base.transform.position, "dog", faceUp: true, checkAddToStack: false);
			}
		}
		else if (Id == "feral_cat" && HasCardOnTop("milk", out cardData2) && WorldManager.instance.IsSpiritDlcActive())
		{
			cardData2.MyGameCard.DestroyCard();
			MyGameCard.DestroyCard();
			WorldManager.instance.CreateCard(base.transform.position, "cat", faceUp: true, checkAddToStack: false);
		}
		base.UpdateCard();
	}

	private Vector3 Wiggle(Vector3 vec, float angle)
	{
		return Quaternion.AngleAxis(Random.Range(0f - angle, angle), Vector3.up) * vec;
	}
}
