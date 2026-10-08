using UnityEngine;

public class ParticleCollider : Landmark
{
	[ExtraData("collider_running")]
	public bool ColliderRunning;

	[ExtraData("CutsceneQueued")]
	public bool CutsceneQueued;

	public AudioClip ColliderRunningSounds;

	public AudioClip ColliderDoneSounds;

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard.Id == "educated_worker") && !(otherCard.Id == "genius") && !(otherCard.Id == "robot_genius"))
		{
			return otherCard.Id == "uranium";
		}
		return true;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && GetChildCount() == 2 && ChildrenMatchingPredicate((CardData x) => x.Id == "uranium").Count == 2 && !CutsceneQueued && WorkerAmountMet() && HasEnergyInput())
		{
			CutsceneQueued = true;
			WorldManager.instance.QueueCutscene(CitiesCutscenes.CitiesParticleCollider(MyGameCard));
		}
		if (ColliderRunning)
		{
			MoveCard();
		}
		base.UpdateCard();
	}

	private void MoveCard()
	{
		MyGameCard.RotWobble(0.8f + Mathf.Cos(Time.time));
		GameCamera.instance.Screenshake = 0.1f;
	}
}
