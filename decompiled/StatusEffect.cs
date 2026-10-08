using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffect
{
	private CardData parentCard;

	public string ParentCardId;

	public float? FillAmount;

	public string NameTerm;

	public string DescriptionTerm;

	public string LoreTerm;

	public string ColorAName;

	public string ColorBName;

	private Color colorA;

	private Color colorB;

	[ExtraData("status_timer")]
	public float StatusTimer;

	private static List<Type> statusEffectTypes;

	public string Name => SokLoc.Translate(NameTerm);

	public virtual string Description => SokLoc.Translate(DescriptionTerm);

	public string Lore => SokLoc.Translate(LoreTerm);

	public virtual Color ColorA
	{
		get
		{
			if (WorldManager.instance.CurrentView != ViewType.Default && FadeInNonDefaultView)
			{
				return ColorManager.instance.GetColorWithName("statuseffect_energy_view_a");
			}
			return colorA;
		}
	}

	public virtual Color ColorB
	{
		get
		{
			if (WorldManager.instance.CurrentView != ViewType.Default && FadeInNonDefaultView)
			{
				return ColorManager.instance.GetColorWithName("statuseffect_energy_view_b");
			}
			return colorB;
		}
	}

	public virtual int? StatusNumber => null;

	public virtual Color? StatusNumberColor => null;

	protected virtual string TermId => "";

	public virtual bool FadeInNonDefaultView => true;

	public CardData ParentCard
	{
		get
		{
			if (parentCard == null && !string.IsNullOrEmpty(ParentCardId))
			{
				GameCard cardWithUniqueId = WorldManager.instance.GetCardWithUniqueId(ParentCardId);
				if (cardWithUniqueId != null)
				{
					parentCard = cardWithUniqueId.CardData;
				}
			}
			return parentCard;
		}
		set
		{
			parentCard = value;
			ParentCardId = value.Id;
		}
	}

	public virtual Sprite Sprite { get; }

	public StatusEffect()
	{
		NameTerm = "statuseffect_" + TermId + "_name";
		DescriptionTerm = "statuseffect_" + TermId + "_description";
		LoreTerm = "statuseffect_" + TermId + "_lore";
		ColorAName = "statuseffect_" + TermId + "_a";
		ColorBName = "statuseffect_" + TermId + "_b";
		colorA = ColorManager.instance.GetColorWithName(ColorAName);
		colorB = ColorManager.instance.GetColorWithName(ColorBName);
	}

	public virtual void Update()
	{
		StatusTimer += Time.deltaTime * WorldManager.instance.TimeScale;
	}

	public SavedStatusEffect ToSavedStatusEffect()
	{
		return new SavedStatusEffect
		{
			StatusEffectId = GetType().Name,
			ExtraDatas = CardData.GetExtraCardData(this)
		};
	}

	public static StatusEffect FromSavedStatusEffect(SavedStatusEffect savedEff)
	{
		InitStatusEffectTypes();
		StatusEffect statusEffect = CreateStatusEffectFromName(savedEff.StatusEffectId);
		if (statusEffect == null)
		{
			return null;
		}
		CardData.SetExtraCardData(statusEffect, savedEff.ExtraDatas);
		return statusEffect;
	}

	private static void InitStatusEffectTypes()
	{
		if (statusEffectTypes == null)
		{
			statusEffectTypes = (from type in typeof(StatusEffect).Assembly.GetTypes()
				where type.IsSubclassOf(typeof(StatusEffect))
				select type).ToList();
		}
	}

	public static bool StatusEffectExists(string name)
	{
		InitStatusEffectTypes();
		return statusEffectTypes.Any((Type x) => x.Name == name);
	}

	public static StatusEffect CreateStatusEffectFromName(string s)
	{
		InitStatusEffectTypes();
		Type type = statusEffectTypes.FirstOrDefault((Type x) => x.Name == s);
		if (type == null)
		{
			return null;
		}
		return Activator.CreateInstance(type) as StatusEffect;
	}
}
