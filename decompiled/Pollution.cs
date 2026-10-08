using System.Linq;

public class Pollution : CardData
{
	public int PollutionEventAmount = 50;

	[ExtraData("pollution_amount")]
	public int PollutionAmount = 1;

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild)
		{
			foreach (GameCard childCard in MyGameCard.GetChildCards())
			{
				if (childCard.CardData is Pollution pollution)
				{
					PollutionAmount += pollution.PollutionAmount;
					childCard.RemoveFromStack();
					childCard.DestroyCard(spawnSmoke: true);
				}
			}
		}
		MyGameCard.SpecialIcon.sprite = SpriteManager.instance.PollutionIcon;
		MyGameCard.SpecialValue = PollutionAmount;
		base.UpdateCard();
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == Id)
		{
			return true;
		}
		return false;
	}

	public override void OnInitialCreate()
	{
		AudioManager.me.PlaySound(AudioManager.me.SpawnPollution, base.transform, 1f, 0.25f);
		RecyclingCenter recyclingCenter = (from x in WorldManager.instance.GetCards<RecyclingCenter>()
			where !x.IsOverflowing && x.HasEnergyInput() && x.HasSewerConnected()
			orderby x.StoredPollution
			select x).FirstOrDefault();
		Landfill landfill = (from x in WorldManager.instance.GetCards<Landfill>()
			where !x.IsOverflowing && x.HasSewerConnected()
			orderby x.StoredPollution
			select x).FirstOrDefault();
		if (recyclingCenter != null && landfill != null)
		{
			if (recyclingCenter.StoredPollution <= landfill.StoredPollution)
			{
				WorldManager.instance.StackSendTo(MyGameCard, recyclingCenter.MyGameCard);
			}
			else
			{
				WorldManager.instance.StackSendTo(MyGameCard, landfill.MyGameCard);
			}
			return;
		}
		if (recyclingCenter != null)
		{
			WorldManager.instance.StackSendTo(MyGameCard, recyclingCenter.MyGameCard);
			return;
		}
		if (landfill != null)
		{
			WorldManager.instance.StackSendTo(MyGameCard, landfill.MyGameCard);
			return;
		}
		Pollution pollution = (from x in WorldManager.instance.GetCards<Pollution>()
			where x != this && x.MyGameCard.BounceTarget == null
			select x).FirstOrDefault();
		if (pollution != null)
		{
			WorldManager.instance.StackSendTo(MyGameCard, pollution.MyGameCard);
		}
	}
}
