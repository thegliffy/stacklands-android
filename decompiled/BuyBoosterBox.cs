using System.Collections.Generic;
using System.Linq;
using Shapes;
using TMPro;
using UnityEngine;

public class BuyBoosterBox : CardTarget
{
	public int Cost;

	public int StoredCostAmount;

	public string BoosterId;

	public Transform SpawnTarget;

	public TextMeshPro BuyText;

	public TextMeshPro NameText;

	public GameObject NewLabel;

	public Rectangle HighlightRectangle;

	public GameObject IdeaIcon;

	public BoardCurrency BoardCurrency;

	private int[] takeOrder = new int[4] { 10, 20, 50, 100 };

	public BoosterpackData Booster => WorldManager.instance.GetBoosterData(BoosterId);

	public int GetCost()
	{
		if (Booster.BoosterLocation == Location.Cities)
		{
			if (CitiesManager.instance.ActiveEvent == CardEventType.FinancialCrisis)
			{
				return Mathf.RoundToInt((float)Cost * 1.5f / 10f) * 10;
			}
			if (CitiesManager.instance.ActiveEvent == CardEventType.PackSale)
			{
				return Mathf.CeilToInt((float)Cost * 0.75f / 10f) * 10;
			}
		}
		return Cost;
	}

	public override bool CanHaveCard(GameCard card)
	{
		if (!MyBoard.IsCurrent)
		{
			return false;
		}
		if (!Booster.IsUnlocked)
		{
			return false;
		}
		if (WorldManager.instance.RemovingCards)
		{
			return false;
		}
		if (BoardCurrency == BoardCurrency.Gold)
		{
			if (WorldManager.instance.GetCardCountInStack(card, (CardData x) => x.Id == "gold") > 0 || WorldManager.instance.GetAmountInChest(card, "gold") > 0)
			{
				return true;
			}
		}
		else if (BoardCurrency == BoardCurrency.Shell)
		{
			if (WorldManager.instance.GetCardCountInStack(card, (CardData x) => x.Id == "shell") > 0 || WorldManager.instance.GetAmountInChest(card, "shell") > 0)
			{
				return true;
			}
		}
		else if (BoardCurrency == BoardCurrency.Dollar && (WorldManager.instance.GetCardCountInStack(card, (CardData x) => x is Dollar) > 0 || WorldManager.instance.GetDollarsInCreditcard(card) > 0))
		{
			return true;
		}
		return false;
	}

	public int GetCurrentCost()
	{
		return GetCost() - StoredCostAmount;
	}

	public override void CardDropped(GameCard card)
	{
		int currentCost = GetCurrentCost();
		if (BoardCurrency == BoardCurrency.Gold)
		{
			int cardCountInStack = WorldManager.instance.GetCardCountInStack(card, (CardData x) => x.Id == "gold");
			int amountInChest = WorldManager.instance.GetAmountInChest(card, "gold");
			int num = 0;
			if (cardCountInStack > 0)
			{
				num = ((cardCountInStack > currentCost) ? currentCost : cardCountInStack);
				WorldManager.instance.RemoveCardsFromStackPred(card, num, (GameCard x) => x.CardData.Id == "gold");
				StoredCostAmount += num;
			}
			else if (amountInChest > 0)
			{
				num = ((amountInChest > currentCost) ? currentCost : amountInChest);
				WorldManager.instance.BuyWithChest(card, num);
				StoredCostAmount += num;
			}
		}
		else if (BoardCurrency == BoardCurrency.Shell)
		{
			int cardCountInStack2 = WorldManager.instance.GetCardCountInStack(card, (CardData x) => x.Id == "shell");
			int amountInChest2 = WorldManager.instance.GetAmountInChest(card, "shell");
			int num2 = 0;
			if (cardCountInStack2 > 0)
			{
				num2 = ((cardCountInStack2 > currentCost) ? currentCost : cardCountInStack2);
				WorldManager.instance.RemoveCardsFromStackPred(card, num2, (GameCard x) => x.CardData.Id == "shell");
				StoredCostAmount += num2;
			}
			else if (amountInChest2 > 0)
			{
				num2 = ((amountInChest2 > currentCost) ? currentCost : amountInChest2);
				WorldManager.instance.BuyWithChest(card, num2);
				StoredCostAmount += num2;
			}
		}
		else
		{
			List<Dollar> list = (from x in card.GetAllCardsInStack()
				where x.CardData is Dollar
				select x.CardData as Dollar).ToList();
			int num3 = list.Sum((Dollar x) => x.DollarValue);
			int dollarsInCreditcard = WorldManager.instance.GetDollarsInCreditcard(card);
			int num4 = 0;
			if (num3 > 0)
			{
				num4 = Mathf.Min(currentCost, num3);
				int num5 = num4;
				for (int i = 0; i < takeOrder.Length; i++)
				{
					int curBillAmount = takeOrder[i];
					int b = num5 / curBillAmount;
					b = Mathf.Min(list.Count((Dollar x) => (object)x != null && x.DollarValue == curBillAmount), b);
					num5 -= b * curBillAmount;
					for (int j = 0; j < b; j++)
					{
						Dollar dollar = list.Where((Dollar x) => (object)x != null && x.DollarValue == curBillAmount).FirstOrDefault();
						list.Remove(dollar);
						dollar.MyGameCard.DestroyCard();
					}
					if (num5 <= 0)
					{
						break;
					}
				}
				if (num5 > 0 && list.Count > 0)
				{
					Dollar dollar2 = list.OrderBy((Dollar x) => x.DollarValue).FirstOrDefault();
					int value = dollar2.DollarValue - num5;
					list.Remove(dollar2);
					dollar2.MyGameCard.DestroyCard();
					num4 = currentCost;
					WorldManager.instance.CreateDollarsFromValue(value, base.transform.position);
				}
				WorldManager.instance.Restack(list.Select((Dollar x) => x.MyGameCard).ToList());
				StoredCostAmount += num4;
			}
			if (dollarsInCreditcard > 0)
			{
				num4 = ((dollarsInCreditcard > currentCost) ? currentCost : dollarsInCreditcard);
				WorldManager.instance.BuyWithCreditcard(card, num4);
				StoredCostAmount += num4;
			}
		}
		WorldManager.instance.CreateSmoke(base.transform.position);
		if (StoredCostAmount == GetCost())
		{
			CreateBoosterPack();
			StoredCostAmount = 0;
		}
		base.CardDropped(card);
	}

	private void CreateBoosterPack(GameCard card = null)
	{
		QuestManager.instance.SpecialActionComplete("buy_" + BoosterId + "_pack");
		WorldManager.instance.BoughtBoosterIds.Add(BoosterId);
		Boosterpack boosterpack = WorldManager.instance.CreateBoosterpack(base.transform.position, BoosterId);
		boosterpack.transform.position = (boosterpack.TargetPosition = SpawnTarget.position);
		if (card != null)
		{
			Vector3 vector = new Vector3(0.4f, 0f, 0f);
			boosterpack.transform.position = (boosterpack.TargetPosition = SpawnTarget.position + vector);
			card.transform.position = (card.TargetPosition = SpawnTarget.position - vector);
		}
		UpdateUndiscoveredCards();
	}

	protected override void Update()
	{
		if (!MyBoard.IsCurrent)
		{
			return;
		}
		if (Booster.IsUnlocked)
		{
			base.gameObject.name = Booster.Name;
			if (BoardCurrency == BoardCurrency.Gold)
			{
				BuyText.text = $"{GetCost() - StoredCostAmount}{Icons.Gold}";
			}
			else if (BoardCurrency == BoardCurrency.Shell)
			{
				BuyText.text = $"{GetCost() - StoredCostAmount}{Icons.Shell}";
			}
			else if (BoardCurrency == BoardCurrency.Dollar)
			{
				BuyText.text = $"{GetCost() - StoredCostAmount}{Icons.Dollar}";
			}
			NameText.text = Booster.Name;
			NewLabel.gameObject.SetActive(!WorldManager.instance.CurrentSave.FoundBoosterIds.Contains(BoosterId));
		}
		else
		{
			base.gameObject.name = "???";
			NameText.text = "???";
			BuyText.text = "";
			NewLabel.gameObject.SetActive(value: false);
		}
		if (WorldManager.instance.CurrentBoard != null)
		{
			HighlightRectangle.Color = WorldManager.instance.CurrentBoard.CardHighlightColor;
		}
		HighlightRectangle.enabled = WorldManager.instance.DraggingCard != null && CanHaveCard(WorldManager.instance.DraggingCard);
		HighlightRectangle.DashOffset += Time.deltaTime;
		if (HighlightRectangle.DashOffset >= 1f)
		{
			HighlightRectangle.DashOffset -= 1f;
		}
		base.Update();
	}

	public void UpdateUndiscoveredCards()
	{
		if (Booster.IsUnlocked && Booster.UndiscoveredCardCount >= 1 && WorldManager.instance.CurrentSave.FoundBoosterIds.Contains(BoosterId))
		{
			IdeaIcon.SetActive(value: true);
		}
		else
		{
			IdeaIcon.SetActive(value: false);
		}
	}

	public override string GetTooltipText()
	{
		if (Booster.IsUnlocked)
		{
			string value = Icons.Gold;
			if (BoardCurrency == BoardCurrency.Shell)
			{
				value = Icons.Shell;
			}
			else if (BoardCurrency == BoardCurrency.Dollar)
			{
				value = Icons.Dollar;
			}
			return SokLoc.Translate("label_drag_coins_to_buy_pack", LocParam.Create("goldicon", value), LocParam.Create("cost", GetCost().ToString())) + "\n\n" + Booster.GetSummary();
		}
		string termId = "label_complete_more_quests_for_pack";
		if (Booster.BoosterLocation == Location.Island)
		{
			termId = "label_complete_more_island_quests_for_pack";
		}
		return SokLoc.Translate(termId, LocParam.Plural("remaining", Booster.RemainingAchievementCountToUnlock));
	}
}
