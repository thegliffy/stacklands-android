using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AllQuests
{
	public static Quest OpenTheStarterPack = new Quest("open_starter_pack")
	{
		OnSpecialAction = (string action) => action == "starter_opened",
		QuestGroup = QuestGroup.Starter
	};

	public static Quest PickBerryBush = new Quest("pick_berrybush")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "berrybush" && action == "complete_harvest") ? true : false,
		QuestGroup = QuestGroup.Starter,
		IsSteamAchievement = true
	};

	public static Quest PunchARock = new Quest("punch_rock")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "rock" && action == "complete_harvest") ? true : false,
		QuestGroup = QuestGroup.Starter
	};

	public static Quest SellACard = new Quest("sell_card")
	{
		OnSpecialAction = (string action) => action == "sell_card",
		QuestGroup = QuestGroup.Starter,
		IsSteamAchievement = true
	};

	public static Quest BuyBooster = new Quest("buy_booster")
	{
		OnSpecialAction = (string action) => action == "buy_basic_pack",
		QuestGroup = QuestGroup.Starter
	};

	public static Quest PunchATree = new Quest("punch_tree")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "tree" && action == "complete_harvest") ? true : false,
		QuestGroup = QuestGroup.Starter
	};

	public static Quest CarveAStick = new Quest("carve_stick")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_carving" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Starter
	};

	public static Quest PauseGame = new Quest("pause_game")
	{
		OnSpecialAction = (string action) => (action == "pause_game" && QuestManager.instance.QuestIsComplete("carve_stick")) ? true : false,
		QuestGroup = QuestGroup.Starter
	};

	public static Quest PlantBerryBush = new Quest("plant_berry_bush")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_growth" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Starter
	};

	public static Quest BuildAHouse = new Quest("build_house")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_house" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Starter,
		IsSteamAchievement = true
	};

	public static Quest SecondVillager = new Quest("second_villager")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount<BaseVillager>() == 2,
		QuestGroup = QuestGroup.Starter,
		IsSteamAchievement = true
	};

	public static Quest CreateOffspring = new Quest("create_offspring")
	{
		OnCardCreate = (CardData card) => card.Id == "kid",
		QuestGroup = QuestGroup.Starter,
		IsSteamAchievement = true
	};

	public static Quest TrainMilitia = new Quest("train_militia")
	{
		OnCardCreate = (CardData card) => card.Id == "militia",
		QuestGroup = QuestGroup.Fighting
	};

	public static Quest Build3Houses = new Quest("build_3_houses")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount<House>() >= 3,
		QuestGroup = QuestGroup.Building
	};

	public static Quest KillARat = new Quest("kill_a_rat")
	{
		OnSpecialAction = (string action) => action == "rat_killed",
		QuestGroup = QuestGroup.Fighting,
		IsSteamAchievement = true,
		PossibleInPeacefulMode = false
	};

	public static Quest ExploreAForest = new Quest("explore_forest")
	{
		OnActionComplete = (CardData card, string action) => (action == "complete_harvest" && card.Id == "forest") ? true : false,
		QuestGroup = QuestGroup.Exploration
	};

	public static Quest ExploreAMountain = new Quest("explore_mountain")
	{
		OnActionComplete = (CardData card, string action) => (action == "complete_harvest" && card.Id == "mountain") ? true : false,
		QuestGroup = QuestGroup.Exploration
	};

	public static Quest OpenATreasureChest = new Quest("open_treasure_chest")
	{
		OnSpecialAction = (string action) => action == "treasure_chest_opened",
		QuestGroup = QuestGroup.Exploration
	};

	public static Quest KillASkeleton = new Quest("kill_a_skeleton")
	{
		OnSpecialAction = (string action) => action == "skeleton_killed",
		QuestGroup = QuestGroup.Fighting,
		IsSteamAchievement = true,
		PossibleInPeacefulMode = false
	};

	public static Quest FindAGraveyard = new Quest("find_graveyard")
	{
		OnCardCreate = (CardData card) => card.Id == "graveyard",
		QuestGroup = QuestGroup.Exploration
	};

	public static Quest StartCampfire = new Quest("start_campfire")
	{
		OnCardCreate = (CardData card) => card.Id == "campfire",
		QuestGroup = QuestGroup.Cooking
	};

	public static Quest CookMeat = new Quest("cook_meat")
	{
		OnCardCreate = (CardData card) => card.Id == "cooked_meat",
		QuestGroup = QuestGroup.Cooking
	};

	public static Quest CookAnOmelette = new Quest("cook_omelette")
	{
		OnCardCreate = (CardData card) => card.Id == "omelette",
		QuestGroup = QuestGroup.Cooking
	};

	public static Quest CookAFrittata = new Quest("cook_frittata")
	{
		OnCardCreate = (CardData card) => card.Id == "frittata",
		QuestGroup = QuestGroup.Cooking,
		IsSteamAchievement = true
	};

	public static Quest Have5Ideas = new Quest("have_5_ideas")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetIdeaCount() >= 5,
		QuestGroup = QuestGroup.Resources
	};

	public static Quest Have10Ideas = new Quest("have_10_ideas")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetIdeaCount() >= 10,
		QuestGroup = QuestGroup.Resources
	};

	public static Quest Have10Wood = new Quest("have_10_wood")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCountWithChest("wood") >= 10,
		QuestGroup = QuestGroup.Resources,
		DefaultVisible = true
	};

	public static Quest Have10Stone = new Quest("have_10_stone")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCountWithChest("stone") >= 10,
		QuestGroup = QuestGroup.Resources,
		DefaultVisible = true
	};

	public static Quest GetADog = new Quest("get_a_dog")
	{
		OnCardCreate = (CardData card) => card.Id == "dog",
		QuestGroup = QuestGroup.Exploration,
		IsSteamAchievement = true
	};

	public static Quest GetIronBar = new Quest("get_iron_bar")
	{
		OnCardCreate = (CardData card) => card.Id == "iron_bar",
		QuestGroup = QuestGroup.Resources
	};

	public static Quest TrainExplorer = new Quest("train_explorer")
	{
		OnCardCreate = (CardData card) => card.Id == "explorer",
		QuestGroup = QuestGroup.Exploration
	};

	public static Quest CreateAShed = new Quest("create_shed")
	{
		OnCardCreate = (CardData card) => card.Id == "shed",
		QuestGroup = QuestGroup.Building
	};

	public static Quest CreateAQuarry = new Quest("create_quarry")
	{
		OnCardCreate = (CardData card) => card.Id == "quarry",
		QuestGroup = QuestGroup.Building
	};

	public static Quest CreateALumberCamp = new Quest("create_lumbercamp")
	{
		OnCardCreate = (CardData card) => card.Id == "lumbercamp",
		QuestGroup = QuestGroup.Building
	};

	public static Quest CreateAFarm = new Quest("build_farm")
	{
		OnCardCreate = (CardData card) => card.Id == "farm",
		QuestGroup = QuestGroup.Building
	};

	public static Quest BuildABrickyard = new Quest("create_brickyard")
	{
		OnCardCreate = (CardData card) => card.Id == "brickyard",
		QuestGroup = QuestGroup.Building
	};

	public static Quest SellACardAtTheMarket = new Quest("sell_at_market")
	{
		OnSpecialAction = (string action) => action == "sell_at_market",
		IsSteamAchievement = true,
		QuestGroup = QuestGroup.Building
	};

	public static Quest BuyFromTravellingCart = new Quest("buy_from_travelling_cart")
	{
		OnSpecialAction = (string action) => action == "travelling_cart_buy",
		QuestGroup = QuestGroup.Exploration,
		IsSteamAchievement = true
	};

	public static Quest Have5Food = new Quest("have_5_food")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetFoodCount(allowDebug: false) >= 5,
		DefaultVisible = true,
		QuestGroup = QuestGroup.Resources,
		DescriptionTermOverride = "quest_have_food_text",
		RequiredCount = 5
	};

	public static Quest Have10Food = new Quest("have_10_food")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetFoodCount(allowDebug: false) >= 10,
		QuestGroup = QuestGroup.Resources,
		DescriptionTermOverride = "quest_have_food_text",
		RequiredCount = 10
	};

	public static Quest Have20Food = new Quest("have_20_food")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetFoodCount(allowDebug: false) >= 20,
		QuestGroup = QuestGroup.Resources,
		DescriptionTermOverride = "quest_have_food_text",
		RequiredCount = 20
	};

	public static Quest Have50Food = new Quest("have_50_food")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetFoodCount(allowDebug: false) >= 50,
		QuestGroup = QuestGroup.Resources,
		DescriptionTermOverride = "quest_have_food_text",
		RequiredCount = 50
	};

	public static Quest ReachMonth6 = new Quest("reach_month_6")
	{
		OnSpecialAction = (string action) => (action == "month_end" && WorldManager.instance.CurrentMonth >= 6) ? true : false,
		QuestGroup = QuestGroup.Survival,
		DescriptionTermOverride = "quest_reach_month_text",
		RequiredCount = 6
	};

	public static Quest ReachMonth12 = new Quest("reach_month_12")
	{
		OnSpecialAction = (string action) => (action == "month_end" && WorldManager.instance.CurrentMonth >= 12) ? true : false,
		QuestGroup = QuestGroup.Survival,
		DescriptionTermOverride = "quest_reach_month_text",
		RequiredCount = 12
	};

	public static Quest ReachMonth24 = new Quest("reach_month_24")
	{
		OnSpecialAction = (string action) => (action == "month_end" && WorldManager.instance.CurrentMonth >= 24) ? true : false,
		QuestGroup = QuestGroup.Survival,
		IsSteamAchievement = true,
		DescriptionTermOverride = "quest_reach_month_text",
		RequiredCount = 24
	};

	public static Quest ReachMonth36 = new Quest("reach_month_36")
	{
		OnSpecialAction = (string action) => (action == "month_end" && WorldManager.instance.CurrentMonth >= 36) ? true : false,
		QuestGroup = QuestGroup.Survival,
		DescriptionTermOverride = "quest_reach_month_text",
		RequiredCount = 36
	};

	public static Quest UnlockAllPacks = new Quest("unlock_all_packs")
	{
		OnSpecialAction = (string action) => action == "unlocked_all_packs",
		QuestGroup = QuestGroup.MainQuest,
		IsSteamAchievement = true
	};

	public static Quest ThreeVillagers = new Quest("3_villagers")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount<BaseVillager>() == 3,
		QuestGroup = QuestGroup.MainQuest
	};

	public static Quest FindCatacombs = new Quest("find_catacombs")
	{
		OnCardCreate = (CardData card) => card.Id == "catacombs",
		QuestGroup = QuestGroup.MainQuest,
		PossibleInPeacefulMode = false
	};

	public static Quest FindGoblet = new Quest("find_goblet")
	{
		OnCardCreate = (CardData card) => card.Id == "goblet",
		QuestGroup = QuestGroup.MainQuest,
		IsSteamAchievement = true
	};

	public static Quest BuildTemple = new Quest("build_temple")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_temple" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.MainQuest
	};

	public static Quest BringGobletToTemple = new Quest("bring_goblet")
	{
		OnSpecialAction = (string action) => action == "goblet_to_temple",
		QuestGroup = QuestGroup.MainQuest,
		ShowCompleteAnimation = false
	};

	public static Quest KillDemon = new Quest("kill_demon")
	{
		OnSpecialAction = (string action) => action == "demon_killed",
		QuestGroup = QuestGroup.MainQuest,
		ShowCompleteAnimation = false,
		IsSteamAchievement = true,
		PossibleInPeacefulMode = false
	};

	public static Quest BuildARowboat = new Quest("build_rowboat")
	{
		OnCardCreate = (CardData card) => card.Id == "rowboat",
		QuestGroup = QuestGroup.MainQuest,
		IsSteamAchievement = true
	};

	public static Quest BuildACathedral = new Quest("build_cathedral")
	{
		OnCardCreate = (CardData card) => card.Id == "cathedral" && card.MyGameCard.MyBoard.Id == "main",
		QuestGroup = QuestGroup.MainQuest,
		ShowCompleteAnimation = false
	};

	public static Quest BringIslandRelicToCathedral = new Quest("island_relic_to_cathedral")
	{
		OnSpecialAction = (string action) => action == "island_relic_to_cathedral",
		QuestGroup = QuestGroup.MainQuest,
		ShowCompleteAnimation = false
	};

	public static Quest KillDemonLord = new Quest("kill_demon_lord")
	{
		OnSpecialAction = (string action) => action == "demon_lord_killed",
		QuestGroup = QuestGroup.MainQuest,
		ShowCompleteAnimation = false,
		PossibleInPeacefulMode = false,
		IsSteamAchievement = true
	};

	public static Quest Have10Gold = new Quest("have_10_gold")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetGoldCount(includeInChest: true) >= 10,
		QuestGroup = QuestGroup.Resources,
		DefaultVisible = true,
		DescriptionTermOverride = "quest_have_gold_text",
		RequiredCount = 10
	};

	public static Quest Have30Gold = new Quest("have_30_gold")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetGoldCount(includeInChest: true) >= 30,
		QuestGroup = QuestGroup.Resources,
		DescriptionTermOverride = "quest_have_gold_text",
		RequiredCount = 30
	};

	public static Quest Have50Gold = new Quest("have_50_gold")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetGoldCount(includeInChest: true) >= 50,
		QuestGroup = QuestGroup.Resources,
		IsSteamAchievement = true,
		DescriptionTermOverride = "quest_have_gold_text",
		RequiredCount = 50
	};

	public static Quest Get2Bananas = new Quest("get_2_bananas")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount("banana") >= 2,
		QuestGroup = QuestGroup.Island_Beginnings
	};

	public static Quest PunchDriftwood = new Quest("punch_driftwood")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "driftwood" && action == "complete_harvest") ? true : false,
		QuestGroup = QuestGroup.Island_Beginnings
	};

	public static Quest Have3Shells = new Quest("have_3_shells")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetShellCount(includeInChest: true) >= 3,
		QuestGroup = QuestGroup.Island_Beginnings,
		DescriptionTermOverride = "quest_have_shells_text",
		RequiredCount = 3
	};

	public static Quest CatchAFish = new Quest("catch_a_fish")
	{
		OnCardCreate = (CardData card) => card.Id == "cod" || card.Id == "mackerel" || card.Id == "tuna",
		QuestGroup = QuestGroup.Island_Beginnings,
		IsSteamAchievement = true
	};

	public static Quest MakeRope = new Quest("make_rope")
	{
		OnCardCreate = (CardData card) => card.Id == "rope",
		QuestGroup = QuestGroup.Island_Beginnings
	};

	public static Quest MakeFishTrap = new Quest("make_fish_trap")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_fish_trap" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Beginnings
	};

	public static Quest MakeSail = new Quest("make_sail")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_sail" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Beginnings
	};

	public static Quest BuildSloop = new Quest("build_sloop")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_sloop" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Beginnings,
		IsSteamAchievement = true
	};

	public static Quest UnlockAllIslandPacks = new Quest("unlock_all_island_packs")
	{
		OnSpecialAction = (string action) => action == "unlocked_all_island_packs",
		QuestGroup = QuestGroup.Island_MainQuest,
		IsSteamAchievement = true
	};

	public static Quest FindATreasureMap = new Quest("find_treasure_map")
	{
		OnCardCreate = (CardData card) => card.Id == "treasure_map",
		QuestGroup = QuestGroup.Island_MainQuest
	};

	public static Quest FindTreasure = new Quest("find_treasure")
	{
		OnSpecialAction = (string action) => action == "find_treasure",
		QuestGroup = QuestGroup.Island_MainQuest,
		IsSteamAchievement = true
	};

	public static Quest ForgeSacredKey = new Quest("forge_sacred_key")
	{
		OnCardCreate = (CardData card) => card.Id == "sacred_key",
		QuestGroup = QuestGroup.Island_MainQuest
	};

	public static Quest OpenSacredChest = new Quest("open_sacred_chest")
	{
		OnSpecialAction = (string action) => action == "sacred_chest_opened",
		QuestGroup = QuestGroup.Island_MainQuest
	};

	public static Quest KillTheKraken = new Quest("kill_kraken")
	{
		OnSpecialAction = (string action) => action == "kraken_killed",
		QuestGroup = QuestGroup.Island_MainQuest,
		PossibleInPeacefulMode = false,
		IsSteamAchievement = true
	};

	public static Quest Have10Shells = new Quest("have_10_shells")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetShellCount(includeInChest: true) >= 10,
		QuestGroup = QuestGroup.Island_Misc,
		DescriptionTermOverride = "quest_have_shells_text",
		RequiredCount = 10
	};

	public static Quest GetAVillagerDrunk = new Quest("drunk_villager")
	{
		OnSpecialAction = (string action) => action == "add_status_StatusEffect_Drunk",
		QuestGroup = QuestGroup.Island_Misc,
		IsSteamAchievement = true
	};

	public static Quest MakeSandstone = new Quest("make_sandstone")
	{
		OnSpecialAction = (string action) => action == "make_sandstone",
		QuestGroup = QuestGroup.Island_Misc
	};

	public static Quest BuildMessHall = new Quest("build_mess_hall")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_mess_hall" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Misc
	};

	public static Quest BuildGreenhouse = new Quest("build_greenhouse")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_greenhouse" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Misc,
		IsSteamAchievement = true
	};

	public static Quest HaveAPoisonedVillager = new Quest("poisoned_villager")
	{
		OnSpecialAction = (string action) => action == "add_status_StatusEffect_Poison",
		QuestGroup = QuestGroup.Island_Misc,
		PossibleInPeacefulMode = false
	};

	public static Quest CureAPoisonedVillager = new Quest("cure_poison")
	{
		OnSpecialAction = (string action) => action == "remove_status_StatusEffect_Poison",
		QuestGroup = QuestGroup.Island_Misc,
		PossibleInPeacefulMode = false
	};

	public static Quest MakeSushi = new Quest("make_sushi")
	{
		OnActionComplete = (CardData card, string action) => ((card.Id == "blueprint_sushi" || card.Id == "blueprint_tamago_sushi") && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Cooking
	};

	public static Quest CookCrabMeat = new Quest("cook_crab_meat")
	{
		OnCardCreate = (CardData card) => card.Id == "cooked_crab_meat",
		QuestGroup = QuestGroup.Island_Cooking
	};

	public static Quest MakeCeviche = new Quest("make_ceviche")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_ceviche" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Cooking,
		IsSteamAchievement = true
	};

	public static Quest BuildComposter = new Quest("build_composter")
	{
		OnCardCreate = (CardData card) => card.Id == "composter",
		QuestGroup = QuestGroup.Island_Misc
	};

	public static Quest MakeSeafoodStew = new Quest("make_seafood_stew")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_seafood_stew" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Cooking
	};

	public static Quest MakeRum = new Quest("make_rum")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_rum" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Cooking,
		IsSteamAchievement = true
	};

	public static Quest BribePirateBoat = new Quest("bribe_pirate_boat")
	{
		OnSpecialAction = (string action) => action == "bribe_pirate_boat",
		QuestGroup = QuestGroup.Island_Misc,
		PossibleInPeacefulMode = false
	};

	public static Quest BefriendAPirate = new Quest("befriend_a_pirate")
	{
		OnSpecialAction = (string action) => action == "befriend_pirate",
		QuestGroup = QuestGroup.Island_Misc,
		PossibleInPeacefulMode = false,
		IsSteamAchievement = true
	};

	public static Quest MakeGoldBar = new Quest("make_gold_bar")
	{
		OnSpecialAction = (string action) => action == "make_gold_bar",
		QuestGroup = QuestGroup.Island_Misc
	};

	public static Quest MakeGlass = new Quest("make_glass")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_glass" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Island_Misc
	};

	public static Quest FindDarkForest = new Quest("find_dark_forest")
	{
		OnActionComplete = (CardData card, string action) => action == "take_portal",
		QuestGroup = QuestGroup.Forest_MainQuest,
		IsSteamAchievement = true
	};

	public static Quest FinishFirstWave = new Quest("finish_first_wave")
	{
		OnSpecialAction = (string action) => (action == "completed_forest_wave" && WorldManager.instance.CurrentRunVariables.ForestWave == 1) ? true : false,
		QuestGroup = QuestGroup.Forest_MainQuest
	};

	public static Quest BuildStablePortal = new Quest("build_stable_portal")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_stable_portal" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Forest_MainQuest,
		IsSteamAchievement = true
	};

	public static Quest FightWaveSix = new Quest("fight_wave_six")
	{
		OnSpecialAction = (string action) => (action == "completed_forest_wave" && WorldManager.instance.CurrentRunVariables.ForestWave == 6) ? true : false,
		QuestGroup = QuestGroup.Forest_MainQuest,
		IsSteamAchievement = true
	};

	public static Quest FightWickedWitch = new Quest("fight_wicked_witch")
	{
		OnSpecialAction = (string action) => action == "fight_wicked_witch",
		QuestGroup = QuestGroup.Forest_MainQuest,
		IsSteamAchievement = true
	};

	public static Quest TrainArcher = new Quest("train_archer")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "archer" && action == "equip_item") ? true : false,
		QuestGroup = QuestGroup.Equipment,
		IsSteamAchievement = true
	};

	public static Quest WearBunnyHat = new Quest("wear_bunny_hat")
	{
		OnActionComplete = (CardData card, string action) => (card.GetAllEquipables().Any((Equipable card) => card.Id == "bunny_hat") && action == "equip_item") ? true : false,
		QuestGroup = QuestGroup.Equipment,
		IsSteamAchievement = true
	};

	public static Quest BuildSmithy = new Quest("build_smithy")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_smithy" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Equipment,
		IsSteamAchievement = true
	};

	public static Quest BreakBottle = new Quest("break_bottle")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_broken_bottle" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Equipment
	};

	public static Quest WearCrabScaleArmor = new Quest("wear_crab_scale_armor")
	{
		OnActionComplete = (CardData card, string action) => (card.GetAllEquipables().Any((Equipable card) => card.Id == "crab_scale_armor") && action == "equip_item") ? true : false,
		QuestGroup = QuestGroup.Equipment
	};

	public static Quest TrainWizard = new Quest("train_wizard")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "wizard" && action == "equip_item") ? true : false,
		QuestGroup = QuestGroup.Equipment
	};

	public static Quest CraftAmuletForest = new Quest("craft_amulet_forest")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_amulet_of_forest" && action == "finish_blueprint") ? true : false,
		QuestGroup = QuestGroup.Equipment
	};

	public static Quest EquipArcherQuiver = new Quest("equip_archer_quiver")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "archer" && card.GetAllEquipables().Any((Equipable card) => card.Id == "quiver") && action == "equip_item") ? true : false,
		QuestGroup = QuestGroup.Equipment
	};

	public static Quest VillagerCombat20 = new Quest("villager_combat_20")
	{
		OnActionComplete = (CardData card, string action) => (card is Combatable combatable && Mathf.RoundToInt(combatable.ProcessedCombatStats.CombatLevel) >= 20 && action == "equip_item") ? true : false,
		QuestGroup = QuestGroup.Equipment,
		IsSteamAchievement = true
	};

	public static Quest TrainNinja = new Quest("train_ninja")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "ninja" && action == "equip_item") ? true : false,
		QuestGroup = QuestGroup.Equipment
	};

	public static Quest EnterDeathWorld = new Quest("enter_death_world")
	{
		OnSpecialAction = (string action) => action == "board_death",
		QuestGroup = QuestGroup.Death_Starter
	};

	public static Quest OpenDeathIntroPack = new Quest("open_death_intro_pack")
	{
		OnSpecialAction = (string action) => action == "death_intro_opened",
		QuestGroup = QuestGroup.Death_Starter
	};

	public static Quest Have3Poop = new Quest("death_have_3_poop")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount<Poop>() >= 3,
		QuestGroup = QuestGroup.Death_Starter
	};

	public static Quest BuildALumbercamp = new Quest("death_build_lumbercamp")
	{
		OnCardCreate = (CardData card) => card.Id == "lumbercamp",
		QuestGroup = QuestGroup.Death_Starter
	};

	public static Quest BuildAnOuthouse = new Quest("death_build_outhouse")
	{
		OnCardCreate = (CardData card) => card.Id == "outhouse",
		QuestGroup = QuestGroup.Death_Starter
	};

	public static Quest GetASecondVillager = new Quest("death_second_villager")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount<BaseVillager>() >= 2,
		QuestGroup = QuestGroup.Death_Starter
	};

	public static Quest GetAThirdVillager = new Quest("death_third_villager")
	{
		OnCardCreate = (CardData card) => card.Id == "kid",
		QuestGroup = QuestGroup.Death_MainQuest
	};

	public static Quest MakeCheese = new Quest("death_make_cheese")
	{
		OnCardCreate = (CardData card) => card.Id == "cheese",
		QuestGroup = QuestGroup.Death_MainQuest
	};

	public static Quest NameVillager = new Quest("death_name_villager")
	{
		OnSpecialAction = (string action) => action == "name_villager",
		QuestGroup = QuestGroup.Death_MainQuest
	};

	public static Quest DeathUnlockAllPacks = new Quest("death_unlock_packs")
	{
		OnSpecialAction = (string action) => action == "unlocked_death_locations_pack",
		QuestGroup = QuestGroup.Death_MainQuest
	};

	public static Quest ExploreTheRuins = new Quest("death_explore_ruins")
	{
		OnActionComplete = (CardData card, string action) => action == "complete_harvest" && card.Id == "ruins",
		QuestGroup = QuestGroup.Death_MainQuest
	};

	public static Quest DeathFindCure = new Quest("death_find_cure")
	{
		OnCardCreate = (CardData card) => card.Id == "blueprint_fountain_of_youth",
		QuestGroup = QuestGroup.Death_MainQuest
	};

	public static Quest ConstructTheFountainOfYouth = new Quest("death_create_fountain_of_youth")
	{
		OnActionComplete = (CardData card, string action) => action == "finish_blueprint" && card.Id == "blueprint_fountain_of_youth",
		QuestGroup = QuestGroup.Death_MainQuest
	};

	public static Quest LiftCurseOfDeath = new Quest("death_find_fix")
	{
		OnActionComplete = (CardData card, string action) => action == "finish_blueprint" && card.Id == "blueprint_death_curse_fix",
		QuestGroup = QuestGroup.Death_MainQuest
	};

	public static Quest HaveAnOldVillager = new Quest("death_villager_old")
	{
		OnSpecialAction = (string action) => action == "villager_old",
		QuestGroup = QuestGroup.Death_Misc
	};

	public static Quest CureSickVillager = new Quest("death_cure_sick_villager")
	{
		OnSpecialAction = (string action) => action == "remove_status_StatusEffect_Sick",
		QuestGroup = QuestGroup.Death_Misc
	};

	public static Quest HaveVillagerDieOfOldAge = new Quest("death_villager_old_age_dead")
	{
		OnSpecialAction = (string action) => action == "villager_old_age_dead",
		QuestGroup = QuestGroup.Death_Misc
	};

	public static Quest BreedACow = new Quest("death_breed_cow")
	{
		OnSpecialAction = (string action) => action == "breed_cow",
		QuestGroup = QuestGroup.Death_Misc
	};

	public static Quest BefriendACat = new Quest("death_befriend_cat")
	{
		OnCardCreate = (CardData card) => card.Id == "cat",
		QuestGroup = QuestGroup.Death_Misc
	};

	public static Quest CreateAltar = new Quest("create_altar")
	{
		OnCardCreate = (CardData card) => card.Id == "blueprint_altar" || card.Id == "altar",
		QuestGroup = QuestGroup.Discover_Spirits
	};

	public static Quest FindDeathWorld = new Quest("find_death_world")
	{
		OnCardCreate = (CardData card) => card.Id == "death_recipe",
		QuestGroup = QuestGroup.Discover_Spirits
	};

	public static Quest FindGreedWorld = new Quest("find_greed_world")
	{
		OnCardCreate = (CardData card) => card.Id == "greed_recipe",
		QuestGroup = QuestGroup.Discover_Spirits
	};

	public static Quest FindHappinessWorld = new Quest("find_happiness_world")
	{
		OnCardCreate = (CardData card) => card.Id == "happiness_recipe",
		QuestGroup = QuestGroup.Discover_Spirits
	};

	public static Quest EnterGreedWorld = new Quest("enter_greed_world")
	{
		OnSpecialAction = (string action) => action == "board_greed",
		QuestGroup = QuestGroup.Greed_Starter
	};

	public static Quest OpenPack = new Quest("open_pack")
	{
		OnSpecialAction = (string action) => action == "greed_intro_opened",
		QuestGroup = QuestGroup.Greed_Starter
	};

	public static Quest GetGrape = new Quest("get_grape")
	{
		OnCardCreate = (CardData card) => card.Id == "grape",
		QuestGroup = QuestGroup.Greed_Starter
	};

	public static Quest FirstDemand = new Quest("first_demand")
	{
		OnSpecialAction = (string action) => action == "demand_start" && WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count == 0,
		QuestGroup = QuestGroup.Greed_Starter
	};

	public static Quest ThreeSuccessfullDemands = new Quest("3_succesfull_demands")
	{
		OnSpecialAction = (string action) => action == "demand_success" && WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count((DemandEvent x) => x.Successful) >= 3,
		QuestGroup = QuestGroup.Greed_MainQuest
	};

	public static Quest FiveSuccessfullDemands = new Quest("5_succesfull_demands")
	{
		OnSpecialAction = (string action) => action == "demand_success" && WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count((DemandEvent x) => x.Successful) >= 5,
		QuestGroup = QuestGroup.Greed_MainQuest
	};

	public static Quest TenSuccessfullDemands = new Quest("8_succesfull_demands")
	{
		OnSpecialAction = (string action) => action == "demand_success" && WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count((DemandEvent x) => x.Successful) >= 8,
		QuestGroup = QuestGroup.Greed_MainQuest
	};

	public static Quest FindCurseOfGreedFix = new Quest("greed_find_fix")
	{
		OnActionComplete = (CardData card, string action) => action == "finish_blueprint" && card.Id == "blueprint_greed_curse_fix",
		QuestGroup = QuestGroup.Greed_MainQuest
	};

	public static Quest GetWool = new Quest("greed_wool")
	{
		OnCardCreate = (CardData card) => card.Id == "wool",
		QuestGroup = QuestGroup.Greed_Misc
	};

	public static Quest GetOakBarrel = new Quest("greed_oak_barrel")
	{
		OnCardCreate = (CardData card) => card.Id == "barrel",
		QuestGroup = QuestGroup.Greed_Misc
	};

	public static Quest GetOliveOil = new Quest("greed_olive_oil")
	{
		OnCardCreate = (CardData card) => card.Id == "olive_oil",
		QuestGroup = QuestGroup.Greed_Misc
	};

	public static Quest GetGarden = new Quest("greed_garden")
	{
		OnCardCreate = (CardData card) => card.Id == "garden",
		QuestGroup = QuestGroup.Greed_Thriving
	};

	public static Quest GetLumbercamp = new Quest("greed_lumbercamp")
	{
		OnCardCreate = (CardData card) => card.Id == "lumbercamp",
		QuestGroup = QuestGroup.Greed_Thriving
	};

	public static Quest GetQuarry = new Quest("greed_quarry")
	{
		OnCardCreate = (CardData card) => card.Id == "quarry",
		QuestGroup = QuestGroup.Greed_Thriving
	};

	public static Quest GetSawmill = new Quest("greed_sawmill")
	{
		OnCardCreate = (CardData card) => card.Id == "sawmill",
		QuestGroup = QuestGroup.Greed_Thriving
	};

	public static Quest GetBrickyard = new Quest("greed_brickyard")
	{
		OnCardCreate = (CardData card) => card.Id == "brickyard",
		QuestGroup = QuestGroup.Greed_Thriving
	};

	public static Quest GetMine = new Quest("greed_mine")
	{
		OnCardCreate = (CardData card) => card.Id == "mine",
		QuestGroup = QuestGroup.Greed_Thriving
	};

	public static Quest GetSmelter = new Quest("greed_smelter")
	{
		OnCardCreate = (CardData card) => card.Id == "smelter",
		QuestGroup = QuestGroup.Greed_Thriving
	};

	public static Quest EnterHappinessWorld = new Quest("enter_happiness_world")
	{
		OnSpecialAction = (string action) => action == "board_happiness",
		QuestGroup = QuestGroup.Happiness_Starter
	};

	public static Quest OpenHappinessIntroPack = new Quest("open_happiness_intro_pack")
	{
		OnSpecialAction = (string action) => action == "happiness_intro_opened",
		QuestGroup = QuestGroup.Happiness_Starter
	};

	public static Quest AdmireACoin = new Quest("happiness_admire_a_coin")
	{
		OnSpecialAction = (string action) => action == "admire_coin",
		QuestGroup = QuestGroup.Happiness_Starter
	};

	public static Quest ReadABook = new Quest("happiness_read_book")
	{
		OnSpecialAction = (string action) => action == "read_book",
		QuestGroup = QuestGroup.Happiness_Starter
	};

	public static Quest Get2Happiness = new Quest("happiness_get_2_happiness")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount<Happiness>() >= 2,
		QuestGroup = QuestGroup.Happiness_Starter
	};

	public static Quest BuildCharity = new Quest("happiness_build_charity")
	{
		OnCardCreate = (CardData card) => card.Id == "charity",
		QuestGroup = QuestGroup.Happiness_MainQuest
	};

	public static Quest UnlockAllHappinessPacks = new Quest("happiness_all_packs")
	{
		OnSpecialAction = (string action) => action == "unlocked_happiness_ideas_2_pack",
		QuestGroup = QuestGroup.Happiness_MainQuest
	};

	public static Quest FindCurseOfHappinessFix = new Quest("happiness_find_fix")
	{
		OnActionComplete = (CardData card, string action) => action == "finish_blueprint" && card.Id == "blueprint_happiness_curse_fix",
		QuestGroup = QuestGroup.Happiness_MainQuest
	};

	public static Quest CreateFlour = new Quest("happiness_create_flour")
	{
		OnCardCreate = (CardData card) => card.Id == "flour",
		QuestGroup = QuestGroup.Happiness_Cooking
	};

	public static Quest MakeDough = new Quest("happiness_make_dough")
	{
		OnCardCreate = (CardData card) => card.Id == "dough",
		QuestGroup = QuestGroup.Happiness_Cooking
	};

	public static Quest BakeBread = new Quest("happiness_bake_bread")
	{
		OnCardCreate = (CardData card) => card.Id == "bread",
		QuestGroup = QuestGroup.Happiness_Cooking
	};

	public static Quest BuildTavern = new Quest("happiness_build_tavern")
	{
		OnCardCreate = (CardData card) => card.Id == "tavern",
		QuestGroup = QuestGroup.Happiness_Cooking
	};

	public static Quest Have1Unhappiness = new Quest("happiness_have_1_unhappiness")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount<Unhappiness>() >= 1 && card.MyGameCard.MyBoard.Id == "happiness",
		QuestGroup = QuestGroup.Happiness_Unhappy
	};

	public static Quest Have5Unhappiness = new Quest("happiness_have_5_unhappiness")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount<Unhappiness>() >= 5 && card.MyGameCard.MyBoard.Id == "happiness",
		QuestGroup = QuestGroup.Happiness_Unhappy
	};

	public static Quest Have10Unhappiness = new Quest("happiness_have_10_unhappiness")
	{
		OnCardCreate = (CardData card) => WorldManager.instance.GetCardCount<Unhappiness>() >= 10 && card.MyGameCard.MyBoard.Id == "happiness",
		QuestGroup = QuestGroup.Happiness_Unhappy
	};

	public static Quest CitiesWelcome = new Quest("cities_welcome")
	{
		OnSpecialAction = (string action) => action == "board_cities",
		QuestGroup = QuestGroup.Cities_Starter
	};

	public static Quest CitiesOpenPack = new Quest("cities_open_pack")
	{
		OnSpecialAction = (string action) => action == "cities_intro_opened",
		QuestGroup = QuestGroup.Cities_Starter
	};

	public static Quest CitiesRemoveTrash = new Quest("cities_remove_trash")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "trash" && action == "complete_harvest") ? true : false,
		QuestGroup = QuestGroup.Cities_Starter
	};

	public static Quest CitiesShack = new Quest("cities_make_shack")
	{
		OnCardCreate = (CardData card) => card.Id == "shack",
		QuestGroup = QuestGroup.Cities_Starter
	};

	public static Quest CitiesGetFarmland = new Quest("cities_get_farmland")
	{
		OnCardCreate = (CardData card) => card.Id == "farmland",
		QuestGroup = QuestGroup.Cities_Starter
	};

	public static Quest CitiesHarvestGrain = new Quest("cities_harvest_grain")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "farmland" && action == "harvest") ? true : false,
		QuestGroup = QuestGroup.Cities_Starter
	};

	public static Quest CitiesBasicFactory = new Quest("cities_build_basic_factory")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_factory_parts" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_Starter
	};

	public static Quest CitiesAutomaticResources = new Quest("cities_automatic_resources")
	{
		OnCardCreate = (CardData card) => card.Id == "lumber_mill",
		QuestGroup = QuestGroup.Cities_Starter
	};

	public static Quest CitiesCityPark = new Quest("cities_city_park")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_park" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_Starter
	};

	public static Quest CitiesBuildLandmark = new Quest("cities_build_second_landmark")
	{
		OnActionComplete = delegate(CardData card, string action)
		{
			List<string> list3 = (from x in WorldManager.instance.GameDataLoader.CardDataPrefabs
				where x is Landmark
				select x.Id).ToList();
			return card is Blueprint blueprint && list3.Contains(blueprint.Subprints.FirstOrDefault()?.ResultCard) && card.Id != "blueprint_park" && action == "finish_blueprint";
		},
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest CitiesDisaster = new Quest("cities_disaster")
	{
		OnSpecialAction = (string action) => action == "event_disaster",
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest Cities20Wellbeing = new Quest("cities_20_wellbeing")
	{
		OnSpecialAction = (string action) => action == "cities_wellbeing_changed" && CitiesManager.instance.Wellbeing >= 20,
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest CitiesGetBar = new Quest("cities_get_bar")
	{
		OnActionComplete = (CardData card, string action) => (card.Id == "blueprint_copperbar" || card.Id == "blueprint_iron_bar") && action == "finish_blueprint",
		QuestGroup = QuestGroup.MainQuest
	};

	public static Quest CitiesLandfill = new Quest("cities_make_landfill")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_landfill" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest CitiesVillagerStatue = new Quest("cities_build_villager_statue")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_villager_statue" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest Cities30Wellbeing = new Quest("cities_30_wellbeing")
	{
		OnSpecialAction = (string action) => action == "cities_wellbeing_changed" && CitiesManager.instance.Wellbeing >= 30,
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest CitiesBuildThemepark = new Quest("cities_build_themepark")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_themepark" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest UnlockAllCitiesPacks = new Quest("unlocked_all_cities_packs")
	{
		OnSpecialAction = (string action) => action == "unlocked_all_cities_packs",
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest Cities40Wellbeing = new Quest("cities_40_wellbeing")
	{
		OnSpecialAction = (string action) => action == "cities_wellbeing_changed" && CitiesManager.instance.Wellbeing >= 40,
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest CitiesGenius = new Quest("cities_genius")
	{
		OnCardCreate = (CardData card) => card.Id == "genius" || card.Id == "robot_genius",
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest CitiesParticleCollider = new Quest("cities_particle_collider")
	{
		OnCardCreate = (CardData card) => card.Id == "particle_collider",
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest CitiesTimeMachine = new Quest("cities_time_machine")
	{
		OnCardCreate = (CardData card) => card.Id == "time_machine",
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest CitiesAllLandmarks = new Quest("cities_all_landmarks")
	{
		OnActionComplete = delegate
		{
			List<string> list = (from x in WorldManager.instance.GameDataLoader.CardDataPrefabs
				where x is Landmark
				select x.Id).ToList();
			list.Remove("particle_collider");
			list.Remove("time_machine");
			List<string> list2 = (from x in WorldManager.instance.GetAllCardsOnBoard("cities")
				where x.CardData is Landmark
				select x.CardData.Id).ToList();
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list2.Contains(list[num]))
				{
					list.RemoveAt(num);
				}
			}
			return list.Count == 0;
		},
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest Cities50Wellbeing = new Quest("cities_50_wellbeing")
	{
		OnSpecialAction = (string action) => action == "cities_wellbeing_changed" && CitiesManager.instance.Wellbeing >= 50,
		QuestGroup = QuestGroup.Cities_MainQuest
	};

	public static Quest Cities6Workers = new Quest("cities_6_workers")
	{
		OnCardCreate = (CardData card) => card is Worker && WorldManager.instance.GetCardCount<Worker>() >= 6,
		QuestGroup = QuestGroup.Cities_Worker
	};

	public static Quest Cities12Workers = new Quest("cities_12_workers")
	{
		OnCardCreate = (CardData card) => card is Worker && WorldManager.instance.GetCardCount<Worker>() >= 12,
		QuestGroup = QuestGroup.Cities_Worker
	};

	public static Quest Cities6EducatedWorkers = new Quest("cities_6_educated_workers")
	{
		OnCardCreate = (CardData card) => card.Id == "educated_worker" && WorldManager.instance.GetCardCount("educated_worker") >= 6,
		QuestGroup = QuestGroup.Cities_Worker
	};

	public static Quest CitiesStorageContainer = new Quest("cities_storage")
	{
		OnCardCreate = (CardData card) => card.Id == "storage_container",
		QuestGroup = QuestGroup.Cities_Automation
	};

	public static Quest CitiesConnectTransport = new Quest("cities_transport")
	{
		OnSpecialAction = (string action) => action == "cities_cable_connected" && WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id).Sum((GameCard x) => x.CardConnectorChildren.Count((CardConnector x) => x.ConnectionType == ConnectionType.Transport && x.ConnectedNode != null)) >= 1,
		QuestGroup = QuestGroup.Cities_Automation
	};

	public static Quest CitiesConnectTransport5 = new Quest("cities_transport_5")
	{
		OnSpecialAction = (string action) => action == "cities_cable_connected" && WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id).Sum((GameCard x) => x.CardConnectorChildren.Count((CardConnector x) => x.ConnectionType == ConnectionType.Transport && x.CardDirection == CardDirection.input && x.ConnectedNode != null)) >= 5,
		QuestGroup = QuestGroup.Cities_Automation
	};

	public static Quest CitiesGetBarrack = new Quest("cities_barrack")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_barrack" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_Freedom
	};

	public static Quest CitiesSoldier = new Quest("cities_soldier")
	{
		OnCardCreate = (CardData card) => card.Id == "soldier",
		QuestGroup = QuestGroup.Cities_Freedom
	};

	public static Quest CitiesRadarStation = new Quest("cities_radar_station")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_radar_station" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_Freedom
	};

	public static Quest CitiesTank = new Quest("cities_tank")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_tank" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_Freedom
	};

	public static Quest CitiesJet = new Quest("cities_jet")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_fighter_jet" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_Freedom
	};

	public static Quest CitiesFirstCable = new Quest("cities_first_cable")
	{
		OnSpecialAction = (string action) => action == "cities_cable_connected" && WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id).Sum((GameCard x) => x.CardConnectorChildren.Count((CardConnector x) => (x.ConnectionType == ConnectionType.LV || x.ConnectionType == ConnectionType.HV) && x.ConnectedNode != null)) >= 1,
		QuestGroup = QuestGroup.Cities_Power
	};

	public static Quest CitiesPowerSource = new Quest("cities_power_source")
	{
		OnActionComplete = (CardData card, string action) => card.Id == "blueprint_water_wheel" && action == "finish_blueprint",
		QuestGroup = QuestGroup.Cities_Power
	};

	public static Quest Cities5LowPower = new Quest("cities_5_low_power")
	{
		OnSpecialAction = (string action) => action == "cities_cable_connected" && WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id).Sum((GameCard x) => x.CardConnectorChildren.Count((CardConnector x) => x.ConnectionType == ConnectionType.LV && x.CardDirection == CardDirection.input && x.ConnectedNode != null)) >= 5,
		QuestGroup = QuestGroup.Cities_Power
	};

	public static Quest Cities10LowPower = new Quest("cities_10_low_power")
	{
		OnSpecialAction = (string action) => action == "cities_cable_connected" && WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id).Sum((GameCard x) => x.CardConnectorChildren.Count((CardConnector x) => x.ConnectionType == ConnectionType.LV && x.CardDirection == CardDirection.input && x.ConnectedNode != null)) >= 10,
		QuestGroup = QuestGroup.Cities_Power
	};

	public static Quest CitiesOilPower = new Quest("cities_oil_power")
	{
		OnCardCreate = (CardData card) => card.Id == "oil_power_plant",
		QuestGroup = QuestGroup.Cities_Power
	};

	public static Quest Cities5HighPower = new Quest("cities_high_power")
	{
		OnSpecialAction = (string action) => action == "cities_cable_connected" && WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id).Any((GameCard x) => x.CardConnectorChildren.Any((CardConnector x) => x.ConnectionType == ConnectionType.HV && x.ConnectedNode != null)),
		QuestGroup = QuestGroup.Cities_Power
	};

	public static Quest Cities10HighPower = new Quest("cities_5_high_power")
	{
		OnSpecialAction = (string action) => action == "cities_cable_connected" && WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id).Sum((GameCard x) => x.CardConnectorChildren.Count((CardConnector x) => x.ConnectionType == ConnectionType.HV && x.CardDirection == CardDirection.input && x.ConnectedNode != null)) >= 5,
		QuestGroup = QuestGroup.Cities_Power
	};

	public static Quest CitiesLoseWellbeing = new Quest("cities_lose_wellbeing")
	{
		OnSpecialAction = (string action) => action == "cities_wellbeing_changed" && CitiesManager.instance.PreviousWellbeing > CitiesManager.instance.Wellbeing,
		QuestGroup = QuestGroup.Cities_Wellbeing
	};

	public static Quest CitiesGainWellbeing = new Quest("cities_gain_wellbeing")
	{
		OnSpecialAction = (string action) => action == "cities_wellbeing_changed" && CitiesManager.instance.PreviousWellbeing < CitiesManager.instance.Wellbeing,
		QuestGroup = QuestGroup.Cities_Wellbeing
	};

	public static Quest Cities5WellbeingGained = new Quest("cities_gain_5_wellbeing")
	{
		OnSpecialAction = (string action) => action == "cities_wellbeing_gained_5",
		QuestGroup = QuestGroup.Cities_Wellbeing
	};

	public static Quest Cities5WellbeingLost = new Quest("cities_lose_5_wellbeing")
	{
		OnSpecialAction = (string action) => action == "cities_wellbeing_lost_5",
		QuestGroup = QuestGroup.Cities_Wellbeing
	};

	public static Quest CitiesMaxWellbeing = new Quest("cities_max_wellbeing")
	{
		OnSpecialAction = (string action) => action == "cities_wellbeing_changed" && CitiesManager.instance.Wellbeing >= 50,
		QuestGroup = QuestGroup.Cities_Wellbeing
	};

	public static Quest CitiesCardCap = new Quest("cities_card_cap")
	{
		OnSpecialAction = (string action) => action == "card_cap_increased",
		QuestGroup = QuestGroup.Cities_Misc
	};

	public static Quest CitiesGetOil = new Quest("cities_get_oil")
	{
		OnCardCreate = (CardData card) => card.Id == "oil",
		QuestGroup = QuestGroup.Cities_Misc
	};

	public static Quest CitiesGetUranium = new Quest("cities_get_uranium")
	{
		OnCardCreate = (CardData card) => card.Id == "uranium",
		QuestGroup = QuestGroup.Cities_Misc
	};

	public static Quest CitiesPets = new Quest("cities_pets")
	{
		OnCardCreate = (CardData card) => (card.Id == "cities_dog" || card.Id == "cities_cat") && WorldManager.instance.GetCardCount("cities_cat") + WorldManager.instance.GetCardCount("cities_dog") >= 3,
		QuestGroup = QuestGroup.Cities_Misc
	};

	public static Quest CitiesTrex = new Quest("cities_trex")
	{
		OnSpecialAction = (string action) => action == "dino_killed",
		QuestGroup = QuestGroup.Cities_Misc
	};

	public static Quest CitiesAliens = new Quest("cities_aliens")
	{
		OnCardCreate = (CardData card) => card.Id == "alien" && WorldManager.instance.GetCardCount("alien") >= 3,
		QuestGroup = QuestGroup.Cities_Misc
	};

	public static Quest CitiesRobotSociety = new Quest("cities_robot_society")
	{
		OnSpecialAction = (string action) => (action == "worker_created" || action == "worker_removed") && WorldManager.instance.GetCardCount("robot_worker") >= 10 && WorldManager.instance.GetCardCount("worker") <= 0,
		QuestGroup = QuestGroup.Cities_Misc
	};

	public static Quest CitiesGreenEnergy = new Quest("cities_green_energy")
	{
		OnSpecialAction = delegate(string action)
		{
			List<string> greenConnectors = new List<string> { "treadmill", "water_wheel", "solar_panel", "wind_turbine" };
			return action == "cities_cable_connected" && WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id).Sum((GameCard x) => x.CardConnectorChildren.Count((CardConnector x) => (x.ConnectionType == ConnectionType.LV || x.ConnectionType == ConnectionType.HV) && x.ConnectedNode != null && greenConnectors.Contains(x.ConnectedNode.Parent.CardData.Id))) >= 10;
		},
		QuestGroup = QuestGroup.Cities_Misc
	};
}
