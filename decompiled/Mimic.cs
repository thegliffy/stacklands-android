using UnityEngine;

public class Mimic : Enemy
{
	public Sprite TreasureChestIcon;

	public Sprite RealIcon;

	[ExtraData("was_detected")]
	public bool WasDetected;

	public override bool CanBeDragged => !WasDetected;

	public override void Clicked()
	{
		Detected();
		base.Clicked();
	}

	public override void UpdateCard()
	{
		if (MyGameCard.IsDemoCard)
		{
			Detected();
		}
		if (!WasDetected && (base.InConflict || MyGameCard.BeingDragged))
		{
			Detected();
		}
		base.UpdateCard();
		Icon = (WasDetected ? RealIcon : TreasureChestIcon);
		MyGameCard.UpdateIcon();
		nameOverride = (WasDetected ? SokLoc.Translate("card_mimic_name") : SokLoc.Translate("card_treasure_chest_name"));
		if (!WasDetected)
		{
			descriptionOverride = SokLoc.Translate("card_treasure_chest_description");
		}
		if (!WasDetected)
		{
			MyGameCard.SpecialValue = null;
		}
	}

	private void Detected()
	{
		if (!WasDetected)
		{
			if (!MyGameCard.IsDemoCard)
			{
				WorldManager.instance.CreateSmoke(MyGameCard.transform.position);
			}
			MyGameCard.UpdateCardPalette();
			WasDetected = true;
		}
	}
}
