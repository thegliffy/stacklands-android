using UnityEngine;

public class Euphoria : Resource
{
	public AudioClip CreateCardSound;

	public override void OnInitialCreate()
	{
		AudioManager.me.PlaySound2D(CreateCardSound, 1f, 0.5f);
		base.OnInitialCreate();
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is Curse { CurseType: CurseType.Happiness })
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
