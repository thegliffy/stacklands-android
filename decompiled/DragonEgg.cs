using UnityEngine;

public class DragonEgg : CardData
{
	public int CrackedState;

	public Sprite NormalIcon;

	public Sprite CrackedIcon;

	public Sprite CrackedIcon_2;

	public AudioClip CrackedSound;

	public AudioClip CrackedSound2;

	public override void UpdateCard()
	{
		Icon = NormalIcon;
		if (CrackedState == 1)
		{
			Icon = CrackedIcon;
		}
		if (CrackedState == 2)
		{
			Icon = CrackedIcon_2;
		}
		NameTerm = ((CrackedState == 0) ? "card_dragon_egg_name" : "card_dragon_egg_name_cracked");
		MyGameCard.UpdateIcon();
		base.UpdateCard();
	}
}
