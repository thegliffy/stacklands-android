using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[ExecuteAlways]
public class ColorManager : MonoBehaviour
{
	public static ColorManager instance;

	[Header("UI")]
	public Color ButtonColor;

	public Color BackgroundColor;

	public Color BackgroundColor2;

	public Color TextColor;

	public Color RedTextColor;

	public Color YellowTextColor;

	public Color DisabledTextColor;

	public Color DisabledButtonTextColor;

	public Color ButtonTextColor;

	public Color DisabledColor;

	public Color HoverButtonColor;

	public Color InactiveBackgroundColor;

	public Color CombatLineColor;

	public Color EffectiveCombatLineColor;

	public Color FloatingTextColorSuccess;

	public Color FloatingTextColorFailed;

	public Color HighVoltageConnector;

	public Color HighVoltageConnectorActive;

	public Color LowVoltageConnector;

	public Color LowVoltageConnectorActive;

	public Color SewerConnector;

	public Color SewerConnectorActive;

	public Color TransportConnector;

	public Color TransportConnectorActive;

	[Header("Cards")]
	public CardPalette Default;

	public CardPalette Structure;

	public CardPalette Blueprint;

	public CardPalette Location;

	public CardPalette Resource;

	public CardPalette Food;

	public CardPalette Gold;

	public CardPalette Mob;

	public CardPalette Fish;

	public CardPalette Rumor;

	public CardPalette Corpse;

	public CardPalette AggresiveMob;

	public CardPalette Building;

	public CardPalette Equipable;

	public CardPalette Happiness;

	public CardPalette Unhappiness;

	public CardPalette Curse;

	public CardPalette Spirit;

	public CardPalette Poop;

	public CardPalette Ingredient;

	public CardPalette Energy;

	public CardPalette Disaster;

	public CardPalette Event;

	public CardPalette Landmark;

	public CardPalette EnergyProducer;

	public CardPalette Weather;

	[Header("Cards 2")]
	public CardPalette EnergyCards;

	public CardPalette SewerCards;

	public CardPalette TransportCards;

	public CardPalette TransportMainCards;

	public CardPalette CalamityCards;

	[Header("Status Effect")]
	public Color StatusEffect_Poison_A;

	public Color StatusEffect_Poison_B;

	public Color StatusEffect_Drunk_A;

	public Color StatusEffect_Drunk_B;

	public Color StatusEffect_Spoiling_A;

	public Color StatusEffect_Spoiling_B;

	public Color StatusEffect_Stunned_A;

	public Color StatusEffect_Stunned_B;

	public Color StatusEffect_WellFed_A;

	public Color StatusEffect_WellFed_B;

	public Color StatusEffect_Frenzy_A;

	public Color StatusEffect_Frenzy_B;

	public Color StatusEffect_Invulnerable_A;

	public Color StatusEffect_Invulnerable_B;

	public Color StatusEffect_Bleeding_A;

	public Color StatusEffect_Bleeding_B;

	public Color StatusEffect_Demand_A;

	public Color StatusEffect_Demand_B;

	public Color StatusEffect_Sick_A;

	public Color StatusEffect_Sick_B;

	public Color StatusEffect_Anxious_A;

	public Color StatusEffect_Anxious_B;

	public Color StatusEffect_No_Energy_A;

	public Color StatusEffect_No_Energy_B;

	public Color StatusEffect_No_Workers_A;

	public Color StatusEffect_No_Workers_B;

	public Color StatusEffect_Homeless_A;

	public Color StatusEffect_Homeless_B;

	public Color StatusEffect_Max_On_Board_A;

	public Color StatusEffect_Max_On_Board_B;

	public Color StatusEffect_No_Educated_Workers_A;

	public Color StatusEffect_No_Educated_Workers_B;

	public Color StatusEffect_Dissolving_A;

	public Color StatusEffect_Dissolving_B;

	public Color StatusEffect_Radar_A;

	public Color StatusEffect_Radar_B;

	public Color StatusEffect_Damaged_A;

	public Color StatusEffect_Damaged_B;

	public Color StatusEffect_On_Fire_A;

	public Color StatusEffect_On_Fire_B;

	public Color StatusEffect_Space_A;

	public Color StatusEffect_Space_B;

	public Color StatusEffect_Card_Off_A;

	public Color StatusEffect_Card_Off_B;

	public Color StatusEffect_Depleted_A;

	public Color StatusEffect_Depleted_B;

	public Color StatusEffect_Energy_View_A;

	public Color StatusEffect_Energy_View_B;

	public Color StatusEffect_Drought_A;

	public Color StatusEffect_Drought_B;

	public Color StatusEffect_No_Sewer_A;

	public Color StatusEffect_No_Sewer_B;

	private Dictionary<string, FieldInfo> nameToField = new Dictionary<string, FieldInfo>();

	private List<CardColorRule> cardColorRules = new List<CardColorRule>();

	private void Awake()
	{
		instance = this;
		InitNameToField();
		InitializeCardColorRules();
	}

	private void InitializeCardColorRules()
	{
		AddCardColorRule(CalamityCards, (CardData c) => WorldManager.instance.CurrentView == ViewType.Calamity && (c.IsDamaged || (c is EventCard eventCard && !eventCard.IsPositiveEvent)));
		AddCardColorRule(EnergyCards, (CardData c) => (c.MyGameCard.HasConnectorOfType(ConnectionType.HV) || c.MyGameCard.HasConnectorOfType(ConnectionType.LV)) && WorldManager.instance.CurrentView == ViewType.Energy);
		AddCardColorRule(SewerCards, (CardData c) => c.MyGameCard.HasConnectorOfType(ConnectionType.Sewer) && WorldManager.instance.CurrentView == ViewType.Sewer);
		AddCardColorRule(TransportCards, (CardData c) => c.MyGameCard.HasConnectorOfType(ConnectionType.Transport) && WorldManager.instance.CurrentView == ViewType.Transport && WorldManager.instance.CurrentBoard.Id == "cities");
		AddCardColorRule(TransportMainCards, (CardData c) => c.MyGameCard.HasConnectorOfType(ConnectionType.Transport) && WorldManager.instance.CurrentView == ViewType.Transport && WorldManager.instance.CurrentBoard.Id != "cities");
		foreach (GameBoard board in UnityEngine.Object.FindObjectsOfType<GameBoard>().ToList())
		{
			AddCardColorRule(board.BoardOptions.CardBackgroundPallete, (CardData c) => WorldManager.instance.CurrentView != ViewType.Default && WorldManager.instance.GetCurrentBoardSafe().Id == board.Id);
		}
		AddCardColorRule(EnergyProducer, (CardData c) => c.MyGameCard.CardData.EnergyConnectors.Any((CardConnectorData x) => x.EnergyConnectionType == CardDirection.output && (x.EnergyConnectionStrength == ConnectionType.LV || x.EnergyConnectionStrength == ConnectionType.HV)));
		AddCardColorRule(Landmark, (CardData c) => c is Landmark);
		AddCardColorRule(Gold, (CardData c) => c is Dollar);
		AddCardColorRule(Energy, (CardData c) => c is Energy);
		AddCardColorRule(Gold, (CardData c) => c is DragonEgg);
		AddCardColorRule(Gold, (CardData c) => c.Id == "merch");
		AddCardColorRule(Ingredient, (CardData c) => c.MyCardType == CardType.Food && c is Food food && food.FoodValue < 1);
		AddCardColorRule(Poop, (CardData c) => c is Poop);
		AddCardColorRule(Spirit, (CardData c) => c is Spirit);
		AddCardColorRule(Curse, (CardData c) => c is Curse);
		AddCardColorRule(Happiness, (CardData c) => c.Id == "happiness" || c.Id == "euphoria");
		AddCardColorRule(Unhappiness, (CardData c) => c.Id == "unhappiness");
		AddCardColorRule(Rumor, (CardData c) => c.MyCardType == CardType.Rumors);
		AddCardColorRule(Location, (CardData c) => c.MyCardType == CardType.Locations);
		AddCardColorRule(Gold, (CardData c) => c is Mimic mimic && !mimic.WasDetected);
		AddCardColorRule(AggresiveMob, (CardData c) => c is Mob { IsAggressive: not false } || c is StrangePortal || c is PirateBoat || c is GoblinAttack);
		AddCardColorRule(Fish, (CardData c) => c.MyCardType == CardType.Fish);
		AddCardColorRule(Gold, (CardData c) => c.Id == "gold" || c.Id == "goblet" || c.Id == "key" || c.Id == "treasure_chest" || c.Id == "temple" || c.Id == "shell" || c.Id == "sacred_key" || c.Id == "sacred_chest" || c.Id == "island_relic" || c.Id == "compass" || c.Id == "city_hall");
		AddCardColorRule(Corpse, (CardData c) => c.Id == "corpse");
		AddCardColorRule(Building, (CardData c) => c.MyCardType == CardType.Structures && c.IsBuilding);
		AddCardColorRule(Structure, (CardData c) => c.MyCardType == CardType.Structures && !c.IsBuilding);
		AddCardColorRule(Blueprint, (CardData c) => c.MyCardType == CardType.Ideas);
		AddCardColorRule(Resource, (CardData c) => c.MyCardType == CardType.Resources);
		AddCardColorRule(Food, (CardData c) => c.MyCardType == CardType.Food);
		AddCardColorRule(Mob, (CardData c) => c.MyCardType == CardType.Mobs);
		AddCardColorRule(Weather, (CardData c) => c.MyCardType == CardType.Weather);
		AddCardColorRule(Equipable, (CardData c) => c.MyCardType == CardType.Equipable);
		AddCardColorRule(Disaster, (CardData c) => c.MyCardType == CardType.Disaster);
		AddCardColorRule(Event, (CardData c) => c.MyCardType == CardType.Event);
		AddCardColorRule(Energy, (CardData c) => c is Battery);
	}

	private void InitNameToField()
	{
		FieldInfo[] fields = GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.FieldType == typeof(Color))
			{
				nameToField[fieldInfo.Name.ToLower()] = fieldInfo;
			}
		}
	}

	private void OnValidate()
	{
		instance = this;
	}

	public Color GetColorWithName(string colorName)
	{
		if (!nameToField.ContainsKey(colorName))
		{
			throw new ArgumentException("No color with name '" + colorName + "'");
		}
		return (Color)nameToField[colorName].GetValue(this);
	}

	public Color GetColor(UIColor col)
	{
		return col switch
		{
			UIColor.Background => BackgroundColor, 
			UIColor.Button => ButtonColor, 
			UIColor.Text => TextColor, 
			UIColor.DisabledText => DisabledTextColor, 
			UIColor.Background2 => BackgroundColor2, 
			UIColor.InactiveBackground => InactiveBackgroundColor, 
			_ => Color.cyan, 
		};
	}

	public void AddCardColorRule(CardPalette palette, Predicate<CardData> pred)
	{
		cardColorRules.Add(new CardColorRule(palette, pred));
	}

	public void InsertCardColorRule(int index, CardPalette palette, Predicate<CardData> pred)
	{
		cardColorRules.Insert(index, new CardColorRule(palette, pred));
	}

	public CardPalette GetPaletteForCard(CardData cd)
	{
		if (cd.HasUniquePalette)
		{
			return cd.MyPalette;
		}
		foreach (CardColorRule cardColorRule in cardColorRules)
		{
			if (cardColorRule.Predicate(cd))
			{
				return cardColorRule.Palette;
			}
		}
		return Default;
	}
}
