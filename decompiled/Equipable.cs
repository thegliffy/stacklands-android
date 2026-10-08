using System.Collections.Generic;
using UnityEngine;

public class Equipable : CardData
{
	public string VillagerTypeOverride;

	[Header("Equipment")]
	public EquipableType EquipableType;

	public List<AudioClip> AttackSounds;

	public AttackType AttackType;

	public Blueprint blueprint;

	public CombatStats MyStats;

	[ExtraData("level")]
	public int Level;

	private string _equipableInfo;

	public override bool CanBeDragged
	{
		get
		{
			if (MyGameCard.EquipmentHolder != null && MyGameCard.EquipmentHolder.Combatable.Team != 0)
			{
				return false;
			}
			return true;
		}
	}

	public virtual void Process(CombatStats stats)
	{
	}

	public override void OnLanguageChange()
	{
		_equipableInfo = null;
		base.OnLanguageChange();
	}

	public override void StoppedDragging()
	{
		List<CardData> list = CardsInStackMatchingPredicate((CardData x) => x is Equipable);
		GameCard parent = MyGameCard.Parent;
		foreach (Equipable item in list)
		{
			item.TryEquipOnCard(parent);
		}
		base.StoppedDragging();
	}

	private void TryEquipOnCard(GameCard card)
	{
		if (MyGameCard.EquipmentHolder != null)
		{
			MyGameCard.EquipmentHolder.Unequip(this);
		}
		if (card != null && card.CardData.HasInventory)
		{
			card.OpenInventory(showInventory: true);
			card.CardData.EquipItem(this);
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyCardType != CardType.Equipable && otherCard.MyCardType != CardType.Humans)
		{
			return otherCard.MyCardType == CardType.Resources;
		}
		return true;
	}

	private string GetAdvancedEquipableInfo()
	{
		string text = SokLoc.Translate("label_combat_speed");
		string text2 = SokLoc.Translate("label_hit_chance");
		string text3 = SokLoc.Translate("label_damage");
		string text4 = SokLoc.Translate("label_defence");
		string text5 = SokLoc.Translate("label_health");
		string text6 = "";
		if (MyStats.MaxHealth != 0)
		{
			text6 = text6 + text5 + ": " + NumberToStringWithPlus(MyStats.MaxHealth) + "\n";
		}
		if (MyStats.AttackSpeedIncrement != 0)
		{
			text6 = text6 + text + " " + NumberToStringWithPlus(MyStats.AttackSpeedIncrement) + "\n";
		}
		if (MyStats.HitChanceIncrement != 0)
		{
			text6 = text6 + text2 + " " + NumberToStringWithPlus(MyStats.HitChanceIncrement) + "\n";
		}
		if (MyStats.AttackDamageIncrement != 0)
		{
			text6 = text6 + text3 + " " + NumberToStringWithPlus(MyStats.AttackDamageIncrement) + "\n";
		}
		if (MyStats.DefenceIncrement != 0)
		{
			text6 = text6 + text4 + " " + NumberToStringWithPlus(MyStats.DefenceIncrement) + "\n";
		}
		return text6;
	}

	private string NumberToStringWithPlus(int n)
	{
		if (n > 0)
		{
			return $"+{n}";
		}
		if (n < 0)
		{
			return $"{n}";
		}
		return "0";
	}

	public string GetEquipableInfo()
	{
		if (!string.IsNullOrEmpty(_equipableInfo))
		{
			return _equipableInfo;
		}
		string text = "";
		text += SokLoc.Translate("label_itemlevel", LocParam.Create("level", Mathf.RoundToInt(MyStats.ItemLevel).ToString()));
		text += "\\d<size=90%>";
		string text2 = MyStats.SummarizeSpecialHits();
		if (text2.Length > 0)
		{
			text = text + text2 + "\n\n";
		}
		text += GetEquipableInfoAdvanced();
		_equipableInfo = text;
		return _equipableInfo;
	}

	public string GetEquipableInfoAdvanced()
	{
		return GetAdvancedEquipableInfo() ?? "";
	}

	public string GetEquipableCombatLevel()
	{
		return "" + SokLoc.Translate("label_itemlevel", LocParam.Create("level", Mathf.RoundToInt(MyStats.ItemLevel).ToString()));
	}

	public override void UpdateCard()
	{
		if (Level > 0)
		{
			nameOverride = $"{SokLoc.Translate(NameTerm)}  (+{Level})";
		}
		descriptionOverride = SokLoc.Translate(DescriptionTerm) + "\n\n<i>" + GetEquipableInfo() + "</i>";
		MyGameCard.SpecialIcon.sprite = GetIconForEquipableType(EquipableType);
		MyGameCard.ShowSpecialIcon = true;
		base.UpdateCard();
	}

	private Sprite GetIconForEquipableType(EquipableType type)
	{
		return type switch
		{
			EquipableType.Head => SpriteManager.instance.HeadIconFilled, 
			EquipableType.Weapon => SpriteManager.instance.HandIconFilled, 
			EquipableType.Torso => SpriteManager.instance.TorsoIconFilled, 
			_ => null, 
		};
	}
}
