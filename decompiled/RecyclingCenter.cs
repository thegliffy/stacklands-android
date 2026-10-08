using UnityEngine;

public class RecyclingCenter : Factory
{
	public int PollutionOverflowMin;

	public int PollutionOverflowMax;

	[ExtraData("is_overflowing")]
	public bool IsOverflowing;

	public Sprite EmptyIcon;

	public Sprite HalfFullIcon;

	public Sprite FullIcon;

	[HideInInspector]
	[ExtraData("stored_pollution")]
	public int StoredPollution;

	[HideInInspector]
	[ExtraData("pollution_overflow")]
	public int PollutionOverflow;

	public int PollutionRemovalRate = 5;

	public override void OnInitialCreate()
	{
		PollutionOverflow = Random.Range(PollutionOverflowMin, PollutionOverflowMax);
		base.OnInitialCreate();
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!IsOverflowing)
		{
			if (!(otherCard.Id == "pollution"))
			{
				return otherCard is Worker;
			}
			return true;
		}
		return false;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	public override void UpdateCardText()
	{
		if (!IsOverflowing)
		{
			if (StoredPollution <= 0)
			{
				descriptionOverride = SokLoc.Translate("card_recycling_center_description", LocParam.Create("amount", PollutionOverflowMin.ToString()));
			}
			else
			{
				descriptionOverride = SokLoc.Translate("card_recycling_center_description_long", LocParam.Create("amount", PollutionOverflowMin.ToString()), LocParam.Create("current", StoredPollution.ToString()));
			}
		}
		else
		{
			nameOverride = SokLoc.Translate("card_overflowing_recycling_center_name");
			descriptionOverride = SokLoc.Translate("card_overflowing_recycling_center_description");
		}
	}

	public override void UpdateCard()
	{
		if (!IsOverflowing)
		{
			if (MyGameCard.HasChild && AllChildrenMatchPredicate((CardData x) => x is Pollution))
			{
				foreach (Pollution item in ChildrenMatchingPredicate((CardData x) => x is Pollution))
				{
					StoredPollution += item.PollutionAmount;
					item.PollutionAmount -= item.PollutionAmount;
					if (item.PollutionAmount == 0)
					{
						item.MyGameCard.DestroyCard(spawnSmoke: true);
					}
					if (StoredPollution >= PollutionOverflow)
					{
						IsOverflowing = true;
						GameCamera.instance.Screenshake = 1f;
						if (MyGameCard.HasChild)
						{
							item.MyGameCard.RemoveFromParent();
						}
						AudioManager.me.PlaySound(AudioManager.me.LandfillOverflow, base.transform, 1f, 0.3f);
						WorldManager.instance.QueueCutscene("cities_landfill_overflow");
						break;
					}
				}
			}
			if (!MyGameCard.TimerRunning && StoredPollution > 0)
			{
				MyGameCard.StartTimer(30f, DumpPollution, SokLoc.Translate("card_landfill_status_1", LocParam.Create("amount", PollutionRemovalRate.ToString())), GetActionId("DumpPollution"));
			}
			if (StoredPollution >= PollutionOverflowMin / 2)
			{
				Icon = HalfFullIcon;
				MyGameCard.UpdateIcon();
			}
			else
			{
				Icon = EmptyIcon;
				MyGameCard.UpdateIcon();
			}
			MyGameCard.SpecialValue = StoredPollution;
		}
		else
		{
			Icon = FullIcon;
			MyGameCard.UpdateIcon();
			MyGameCard.CancelTimer(GetActionId("DumpPollution"));
			if (!MyGameCard.TimerRunning)
			{
				MyGameCard.StartTimer(120f, ResolveOverflow, SokLoc.Translate("card_landfill_status_2"), GetActionId("ResolveOverflow"));
			}
		}
		MyGameCard.SpecialIcon.sprite = SpriteManager.instance.PollutionIcon;
		base.UpdateCard();
	}

	[TimedAction("resolve_overflow")]
	public void ResolveOverflow()
	{
		WorldManager.instance.CreateSmoke(base.Position);
		StoredPollution = PollutionOverflow - 10;
		IsOverflowing = false;
	}

	[TimedAction("dump_pollution")]
	public void DumpPollution()
	{
		if (StoredPollution > 0)
		{
			int num = Mathf.Min(StoredPollution, PollutionRemovalRate);
			StoredPollution -= num;
			CardData cardData = WorldManager.instance.CreateCard(base.Position, "metal_scraps");
			WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
			AudioManager.me.PlaySound(AudioManager.me.ClearPollution, base.transform, 1f, 0.3f);
			WorldManager.instance.CreateSmoke(base.Position);
		}
	}
}
