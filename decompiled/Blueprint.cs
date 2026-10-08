using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blueprint : CardData, IKnowledge
{
	[Header("Prints")]
	public List<Subprint> Subprints = new List<Subprint>();

	public BlueprintGroup BlueprintGroup;

	public bool HideFromIdeasTab;

	public bool IsInvention;

	public bool IsLandmark;

	public bool NeedsExactMatch = true;

	public bool OverrideResultDescription;

	public bool HasMaxAmountOnBoard;

	public bool CombineResultCards;

	public int MaxAmountOnBoard = 1;

	public string ResultDescriptionTerm;

	public bool IgnoreEnergyWorkerDemand;

	protected List<CardData> allResultCards = new List<CardData>();

	public string KnowledgeName => SokLoc.Translate(NameTerm);

	public string KnowledgeText => GetText();

	public string CardId => Id;

	public virtual bool CanCurrentlyBeMade => true;

	public BlueprintGroup Group => BlueprintGroup;

	public bool IsIslandKnowledge
	{
		get
		{
			if (BlueprintGroup != BlueprintGroup.Island)
			{
				return BlueprintGroup == BlueprintGroup.Sailing;
			}
			return true;
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard is Blueprint))
		{
			return otherCard is Rumor;
		}
		return true;
	}

	public virtual void Init(GameDataLoader loader)
	{
		for (int i = 0; i < Subprints.Count; i++)
		{
			Subprint subprint = Subprints[i];
			subprint.ParentBlueprint = this;
			subprint.SubprintIndex = i;
		}
	}

	public override void UpdateCard()
	{
		descriptionOverride = GetTooltipText();
		base.UpdateCard();
	}

	public override void OnInitialCreate()
	{
		base.OnInitialCreate();
	}

	protected override string GetTooltipText()
	{
		return GetText();
	}

	public string GetText()
	{
		string text = Subprints[0].DefaultText();
		if (OverrideResultDescription)
		{
			string text2 = SokLoc.Translate(ResultDescriptionTerm);
			text = text + "\n\n\"" + text2 + "\"";
		}
		else
		{
			string text3 = Subprints[0].ResultCard;
			if (string.IsNullOrEmpty(text3) && Subprints[0].ExtraResultCards.Length != 0)
			{
				text3 = Subprints[0].ExtraResultCards[0];
			}
			CardData cardPrefab = WorldManager.instance.GetCardPrefab(text3);
			if (cardPrefab == null)
			{
				Debug.LogWarning("No result card set for " + Id);
				return text;
			}
			cardPrefab.UpdateCardText();
			if (string.IsNullOrEmpty(text3))
			{
				return null;
			}
			text = ((!(cardPrefab is Equipable equipable)) ? (text + "\n\n\"" + cardPrefab.Description + "\"") : (text + "\n\n\"" + cardPrefab.Description + "\"\n\n<i>" + equipable.GetEquipableCombatLevel() + "</i>"));
			if (Subprints[0].ResultWellbeing > 0)
			{
				text = text + "\n\n" + SokLoc.Translate("label_blueprint_wellbeing_generation", LocParam.Create("amount", Subprints[0].ResultWellbeing.ToString()), LocParam.Create("icon", Icons.Wellbeing));
			}
			if (Subprints[0].ResultPolution > 0)
			{
				text = text + "\n\n" + SokLoc.Translate("label_blueprint_pollution_generation", LocParam.Create("amount", Subprints[0].ResultPolution.ToString()), LocParam.Create("icon", Icons.Pollution));
			}
		}
		return text;
	}

	public virtual Subprint GetMatchingSubprint(GameCard card, out SubprintMatchInfo matchInfo)
	{
		matchInfo = default(SubprintMatchInfo);
		foreach (Subprint subprint in Subprints)
		{
			if (subprint.StackMatchesSubprint(card, out matchInfo))
			{
				return subprint;
			}
		}
		return null;
	}

	public virtual void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
	{
		List<GameCard> list = new List<GameCard>(involvedCards);
		List<string> allCardsToRemove = print.GetAllCardsToRemove();
		CardData cardData = null;
		List<CardData> list2 = new List<CardData>();
		for (int i = 0; i < allCardsToRemove.Count; i++)
		{
			string[] possibleRemovables = allCardsToRemove[i].Split('|');
			GameCard gameCard = list.FirstOrDefault((GameCard x) => possibleRemovables.Contains(x.CardData.Id));
			if (gameCard != null)
			{
				gameCard.DestroyCard(spawnSmoke: true);
				list.Remove(gameCard);
			}
		}
		allResultCards.Clear();
		Vector3 outputDirection = ((rootCard != null) ? rootCard.CardData.OutputDir : Vector3.zero);
		if (!string.IsNullOrEmpty(print.ResultCard))
		{
			cardData = WorldManager.instance.CreateCard(rootCard.transform.position, print.ResultCard, faceUp: false, checkAddToStack: false);
			allResultCards.Add(cardData);
		}
		if (!string.IsNullOrEmpty(print.ResultAction))
		{
			GameCard gameCard2 = involvedCards.FirstOrDefault((GameCard x) => x.CardData is Combatable);
			if (gameCard2 != null)
			{
				gameCard2.CardData.ParseAction(print.ResultAction);
			}
			else
			{
				rootCard.CardData.ParseAction(print.ResultAction);
			}
		}
		if (print.ExtraResultCards != null)
		{
			for (int j = 0; j < print.ExtraResultCards.Length; j++)
			{
				CardData item = WorldManager.instance.CreateCard(rootCard.transform.position, print.ExtraResultCards[j], faceUp: false, checkAddToStack: false);
				list2.Add(item);
				allResultCards.Add(item);
			}
		}
		GameCard gameCard3 = involvedCards.FirstOrDefault((GameCard x) => x.CardData.HasOutputConnector());
		if (CombineResultCards)
		{
			WorldManager.instance.Restack(allResultCards.Select((CardData x) => x.MyGameCard).ToList());
			if (gameCard3 != null)
			{
				WorldManager.instance.StackSendCheckTarget(gameCard3, allResultCards[0].MyGameCard, outputDirection, gameCard3);
			}
			else
			{
				WorldManager.instance.StackSend(allResultCards[0].MyGameCard, outputDirection);
			}
		}
		else
		{
			if (cardData != null)
			{
				if (gameCard3 != null)
				{
					WorldManager.instance.StackSendCheckTarget(gameCard3, cardData.MyGameCard, outputDirection, gameCard3);
				}
				else
				{
					WorldManager.instance.StackSend(cardData.MyGameCard, outputDirection);
				}
			}
			if (list2.Count > 0)
			{
				WorldManager.instance.Restack(list2.Select((CardData x) => x.MyGameCard).ToList());
				if (gameCard3 != null)
				{
					WorldManager.instance.StackSendCheckTarget(gameCard3, list2[0].MyGameCard, outputDirection, gameCard3);
				}
				else
				{
					WorldManager.instance.StackSend(list2[0].MyGameCard, outputDirection);
				}
			}
		}
		if (print.ResultPolution > 0)
		{
			(WorldManager.instance.CreateCard(rootCard.transform.position, "pollution", faceUp: true, checkAddToStack: false) as Pollution).PollutionAmount = print.ResultPolution;
		}
		if (print.ResultWellbeing != 0)
		{
			CitiesManager.instance.AddWellbeing(print.ResultWellbeing);
			WorldManager.instance.CreateFloatingText(allResultCards[0].MyGameCard, print.ResultWellbeing > 0, print.ResultWellbeing, SokLoc.Translate("label_blueprint_wellbeing"), Icons.Wellbeing, desiredBehaviour: true, 0, 0f, closeOnHover: true);
		}
		WorldManager.instance.Restack(list);
	}
}
