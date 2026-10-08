using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UImGui;
using UnityEngine;
using UnityEngine.InputSystem;

public class PackUI : MonoBehaviour
{
	private bool Show;

	private void Start()
	{
		UImGuiUtility.Layout += DrawPackUI;
	}

	private void Update()
	{
		if (Keyboard.current[Key.F5].wasPressedThisFrame)
		{
			Show = !Show;
		}
	}

	private void SetPackContextMenu(BoosterpackData bpd, int i)
	{
		if (ImGui.Selectable("Add card to pack"))
		{
			GUIUtility.systemCopyBuffer = $"WorldManager.instance.GetBoosterData(\"{bpd.BoosterId}\").CardBags[{i}].SetPackCards.Add(\"card_id\");";
		}
		if (ImGui.Selectable("Remove card from pack"))
		{
			GUIUtility.systemCopyBuffer = $"WorldManager.instance.GetBoosterData(\"{bpd.BoosterId}\").CardBags[{i}].SetPackCards.Remove(\"card_id\");";
		}
	}

	private void ChancesContextMenu(BoosterpackData bpd, int i)
	{
		if (ImGui.Selectable("Add chance"))
		{
			GUIUtility.systemCopyBuffer = $"WorldManager.instance.GetBoosterData(\"{bpd.BoosterId}\").CardBags[{i}].Chances.Add(new CardChance(\"card_id\", 1))";
		}
	}

	private void SetCardBagContextMenu(BoosterpackData bpd, int i)
	{
		CardBag cardBag = bpd.CardBags[i];
		if (ImGui.Selectable("Add card to SetCardBag"))
		{
			string text = ((!((int[])Enum.GetValues(typeof(SetCardBagType))).Contains((int)cardBag.SetCardBag)) ? $"(SetCardBagType){(int)cardBag.SetCardBag}" : ("SetCardBagType." + EnumHelper.GetName<SetCardBagType>((int)cardBag.SetCardBag)));
			GUIUtility.systemCopyBuffer = "WorldManager.instance.GameDataLoader.AddCardToSetCardBag(" + text + ", \"card_id\", 1)";
		}
	}

	private void EnemiesContextMenu(BoosterpackData bpd, int i)
	{
	}

	private void DrawPackUI(global::UImGui.UImGui _)
	{
		if (!Show)
		{
			return;
		}
		ImGui.Begin("Pack Data Inspector");
		foreach (BoosterpackData boosterPackData in WorldManager.instance.BoosterPackDatas)
		{
			if (!ImGui.CollapsingHeader(boosterPackData.Name + " (" + boosterPackData.BoosterId + ")"))
			{
				continue;
			}
			ImGui.PushID(boosterPackData.BoosterId);
			for (int i = 0; i < boosterPackData.CardBags.Count; i++)
			{
				CardBag cardBag = boosterPackData.CardBags[i];
				string text = $"{i} - {EnumHelper.GetName<CardBagType>((int)cardBag.CardBagType)}";
				if (cardBag.CardBagType == CardBagType.SetCardBag)
				{
					text = text + ": " + EnumHelper.GetName<SetCardBagType>((int)cardBag.SetCardBag);
				}
				if (cardBag.CardBagType == CardBagType.Enemies)
				{
					text += $": {EnumHelper.GetName<EnemySetCardBag>((int)cardBag.EnemyCardBag)} ({cardBag.StrengthLevel} strength)";
				}
				text += string.Format(" | {0} card{1}", cardBag.CardsInPack, (cardBag.CardsInPack != 1) ? "s" : "");
				if (!ImGui.TreeNode(text))
				{
					continue;
				}
				if (cardBag.CardBagType == CardBagType.SetPack)
				{
					if (ImGui.BeginPopupContextItem())
					{
						ImGui.Text("Copy code..");
						SetPackContextMenu(boosterPackData, i);
						ImGui.EndPopup();
					}
					foreach (string setPackCard in cardBag.SetPackCards)
					{
						ImGui.Text("  " + setPackCard);
						IdTooltip(setPackCard);
					}
				}
				if (cardBag.CardBagType == CardBagType.Chances)
				{
					if (ImGui.BeginPopupContextItem())
					{
						ImGui.Text("Copy code..");
						ChancesContextMenu(boosterPackData, i);
						ImGui.EndPopup();
					}
					float num = 0f;
					foreach (CardChance chance in cardBag.Chances)
					{
						num += (float)chance.Chance;
					}
					foreach (CardChance chance2 in cardBag.Chances)
					{
						chance2.PercentageChance = (chance2.PercentageChance = (float)chance2.Chance / num);
					}
					foreach (CardChance chance3 in cardBag.Chances)
					{
						string text2 = $"   {chance3.Chance} {chance3.Id} ({chance3.PercentageChance * 100f:F2}%%)";
						if (chance3.HasMaxCount)
						{
							text2 += $" | max {chance3.MaxCountToGive}";
						}
						if (chance3.HasPrerequisiteCard)
						{
							text2 = text2 + " | prereq. " + chance3.PrerequisiteCardId;
						}
						ImGui.Text(text2);
						IdTooltip(chance3.Id);
					}
				}
				if (cardBag.CardBagType == CardBagType.SetCardBag)
				{
					if (ImGui.BeginPopupContextItem())
					{
						ImGui.Text("Copy code..");
						SetCardBagContextMenu(boosterPackData, i);
						ImGui.EndPopup();
					}
					List<CardChance> chancesForSetCardBag = CardBag.GetChancesForSetCardBag(WorldManager.instance.GameDataLoader, cardBag.SetCardBag);
					float num2 = 0f;
					foreach (CardChance item in chancesForSetCardBag)
					{
						num2 += (float)item.Chance;
					}
					foreach (CardChance item2 in chancesForSetCardBag)
					{
						item2.PercentageChance = (item2.PercentageChance = (float)item2.Chance / num2);
					}
					foreach (CardChance item3 in chancesForSetCardBag)
					{
						string text3 = $"   {item3.Chance} {item3.Id} ({item3.PercentageChance * 100f:F2}%%)";
						if (item3.HasMaxCount)
						{
							text3 += $" | max {item3.MaxCountToGive}";
						}
						if (item3.HasPrerequisiteCard)
						{
							text3 = text3 + " | prereq. " + item3.PrerequisiteCardId;
						}
						ImGui.Text(text3);
						IdTooltip(item3.Id);
					}
				}
				if (cardBag.CardBagType == CardBagType.Enemies)
				{
					if (ImGui.BeginPopupContextItem())
					{
						ImGui.Text("Copy code..");
						EnemiesContextMenu(boosterPackData, i);
						ImGui.EndPopup();
					}
					List<CardChance> chancesForSetCardBag2 = CardBag.GetChancesForSetCardBag(WorldManager.instance.GameDataLoader, WorldManager.instance.GameDataLoader.GetSetCardBagForEnemyCardBag(cardBag.EnemyCardBag));
					float num3 = 0f;
					foreach (CardChance item4 in chancesForSetCardBag2)
					{
						num3 += (float)item4.Chance;
					}
					foreach (CardChance item5 in chancesForSetCardBag2)
					{
						item5.PercentageChance = (item5.PercentageChance = (float)item5.Chance / num3);
					}
					foreach (CardChance item6 in chancesForSetCardBag2)
					{
						string text4 = $"{item6.Chance} {item6.Id} ({item6.PercentageChance * 100f:F2}%)";
						if (item6.HasMaxCount)
						{
							text4 += $" | max {item6.MaxCountToGive}";
						}
						if (item6.HasPrerequisiteCard)
						{
							text4 = text4 + " | prereq. " + item6.PrerequisiteCardId;
						}
						if (!ImGui.TreeNode(text4))
						{
							continue;
						}
						IdTooltip(item6.Id);
						Combatable combatable = (Combatable)WorldManager.instance.GetCardPrefab(item6.Id);
						ImGui.Text("Head:");
						foreach (Equipable item7 in combatable.PossibleEquipables.FindAll((Equipable e) => e.EquipableType == EquipableType.Head))
						{
							ImGui.Text("  " + item7.Id);
							IdTooltip(item7.Id);
						}
						ImGui.Text("Torso:");
						foreach (Equipable item8 in combatable.PossibleEquipables.FindAll((Equipable e) => e.EquipableType == EquipableType.Torso))
						{
							ImGui.Text("  " + item8.Id);
							IdTooltip(item8.Id);
						}
						ImGui.Text("Weapon:");
						foreach (Equipable item9 in combatable.PossibleEquipables.FindAll((Equipable e) => e.EquipableType == EquipableType.Weapon))
						{
							ImGui.Text("  " + item9.Id);
							IdTooltip(item9.Id);
						}
						ImGui.TreePop();
					}
				}
				ImGui.TreePop();
			}
			ImGui.PopID();
		}
		ImGui.End();
	}

	public static void IdTooltip(string id)
	{
		if (ImGui.IsItemHovered())
		{
			ImGui.BeginTooltip();
			ImGui.Text(WorldManager.instance.GetCardPrefab(id).Name);
			ImGui.EndTooltip();
		}
	}
}
