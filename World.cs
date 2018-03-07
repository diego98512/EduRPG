using System.Collections.Generic;

namespace Engine
{
    internal class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Monster> Monsters = new List<Monster>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();
        public static readonly List<Spell> Spells = new List<Spell>();

        public const int ITEM_ID_RUSTY_SWORD = 1;
        public const int ITEM_ID_RAT_TAIL = 2;
        public const int ITEM_ID_PIECE_OF_FUR = 3;
        public const int ITEM_ID_SNAKE_FANG = 4;
        public const int ITEM_ID_SNAKESKIN = 5;
        public const int ITEM_ID_CLUB = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_SPIDER_FANG = 8;
        public const int ITEM_ID_SPIDER_SILK = 9;
        public const int ITEM_ID_ADVENTURER_PASS = 10;

        public const int MONSTER_ID_RAT = 1;
        public const int MONSTER_ID_SNAKE = 2;
        public const int MONSTER_ID_GIANT_SPIDER = 3;

        public const int QUEST_ID_CLEAR_ALCHEMISTS_GARDEN = 1;
        public const int QUEST_ID_CLEAR_FARMERS_FIELD = 2;

        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMISTS_HUT = 4;
        public const int LOCATION_ID_ALCHEMISTS_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;
        
        public const int SPELL_ID_FIRE = 1;
        public const int SPELL_ID_FROST = 2;
        public const int SPELL_ID_LIGHTNING = 3;
        public const int SPELL_ID_LIGHT = 4;
        public const int SPELL_ID_DARK = 5;

        public const int STATUS_ID_NULL = 0;
		public const int STATUS_ID_POISON = 1;
		public const int STATUS_ID_PARALYZE = 2;
		public const int STATUS_ID_BURN = 3;
		public const int STATUS_ID_SLOW = 4;
		public const int STATUS_ID_INTIMIDATE = 5;

        static World()
        {
            PopulateItems();
            PopulateMonsters();
            PopulateQuests();
            PopulateLocations();
            PopulateSpells();
        }
				
		#region PopulateItems
        private static void PopulateItems()
        {
            Items.Add(new Weapon(ITEM_ID_RUSTY_SWORD , "Rusty Sword" , "Rusty Swords" , 0 , 5));
            Items.Add(new Item(ITEM_ID_RAT_TAIL , "Rat Tail" , "Rat Tails"));
            Items.Add(new Item(ITEM_ID_PIECE_OF_FUR , "Piece of Fur" , "Pieces of Fur"));
            Items.Add(new Item(ITEM_ID_SNAKE_FANG , "Snake Fang" , "Snake Fangs"));
            Items.Add(new Item(ITEM_ID_SNAKESKIN , "Snakeskin" , "Snakeskins"));
            Items.Add(new Weapon(ITEM_ID_CLUB , "Club" , "Clubs" , 3 , 10));
            Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION , "Healing Potion" , "Healing Potions" , 5));
            Items.Add(new Item(ITEM_ID_SPIDER_FANG , "Spider Fang" , "Spider Fangs"));
            Items.Add(new Item(ITEM_ID_SPIDER_SILK , "Spider Silk" , "Spider Silks"));
            Items.Add(new Item(ITEM_ID_ADVENTURER_PASS , "Adventurer Pass" , "Adventurer Passes"));
        }
        #endregion PopulateItems

        #region PopulateMonsters
        private static void PopulateMonsters()
        {
            Monster rat = new Monster(MONSTER_ID_RAT , "Rat" , 5 , 3 , 10 , 3 , 3, 0);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL) , 75 , false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECE_OF_FUR) , 75 , true));

            Monster snake = new Monster(MONSTER_ID_SNAKE , "Snake" , 5 , 3 , 10 , 3 , 3, 0);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_FANG) , 75 , false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN) , 75 , true));

            Monster giantSpider = new Monster(MONSTER_ID_GIANT_SPIDER , "Giant Spider" , 20 , 5 , 40 , 10 , 10, 0);
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_FANG) , 75 , true));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_SILK) , 25 , false));

            Monsters.Add(rat);
            Monsters.Add(snake);
            Monsters.Add(giantSpider);
        }
        #endregion PopulateMonsters

        #region PopulateQuests
        private static void PopulateQuests()
        {
            Quest clearAlchemistsGarden = new Quest(QUEST_ID_CLEAR_ALCHEMISTS_GARDEN , "Clear the Alchemist's Garden." , "Kill rats in the Alchemist's Garden and bring back 3 Rat Tails. You will receive a healing potion and 10 gold pieces." , 20 , 10);

            clearAlchemistsGarden.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_RAT_TAIL) , 3));

            clearAlchemistsGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);

            Quest clearFarmersField = new Quest(QUEST_ID_CLEAR_FARMERS_FIELD , "Clear the Farmer's Field" , "Kill snakes in the Farmer's Field and bring back 3 snake fangs. You will receive an Adventurer's Pass and 20 gold pieces." , 20 , 20);

            clearFarmersField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKE_FANG) , 3));

            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURER_PASS);

            Quests.Add(clearAlchemistsGarden);
            Quests.Add(clearFarmersField);
        }
        #endregion PopulateQuests

        #region PopulateLocations
        private static void PopulateLocations()
        {
            //Create each location
            Location home = new Location(LOCATION_ID_HOME , "Home" , "Your house. You really need to clean the place up a bit.");

            Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE , "Town Square" , "You see a fountain.");

            Location alchemistHut = new Location(LOCATION_ID_ALCHEMISTS_HUT , "Alchemist's Hut" , "There are many strange plants on the shelves.")
            {
                QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_ALCHEMISTS_GARDEN)
            };

            Location alchemistsGarden = new Location(LOCATION_ID_ALCHEMISTS_GARDEN , "Alchemist's Garden" , "Many plants are growing here.")
            {
                MonsterLivingHere = MonsterByID(MONSTER_ID_RAT)
            };

            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE , "Farmhouse" , "There is a small farmhouse with a farmer out front.")
            {
                QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_FARMERS_FIELD)
            };

            Location farmersField = new Location(LOCATION_ID_FARM_FIELD , "Farmer's Field" , "You see rows of crops lining the field.")
            {
                MonsterLivingHere = MonsterByID(MONSTER_ID_SNAKE)
            };

            Location guardPost = new Location(LOCATION_ID_GUARD_POST , "Guard Post" , "There is a large, tough looking guard here." , ItemByID(ITEM_ID_ADVENTURER_PASS));

            Location bridge = new Location(LOCATION_ID_BRIDGE , "Bridge" , "A cobblestone bridge crosses a wide, torrentous river.");

            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD , "Forest" , "You see spider webs suffocating the trees in this forest.")
            {
                MonsterLivingHere = MonsterByID(MONSTER_ID_GIANT_SPIDER)
            };

            //Link the locations together

            home.LocationToNorth = townSquare;

            townSquare.LocationToNorth = alchemistHut;
            townSquare.LocationToSouth = home;
            townSquare.LocationToEast = guardPost;
            townSquare.LocationToWest = farmhouse;

            farmhouse.LocationToEast = townSquare;
            farmhouse.LocationToWest = farmersField;

            farmersField.LocationToEast = farmhouse;

            alchemistHut.LocationToSouth = townSquare;
            alchemistHut.LocationToNorth = alchemistsGarden;

            alchemistsGarden.LocationToSouth = alchemistHut;

            guardPost.LocationToEast = bridge;
            guardPost.LocationToWest = townSquare;

            bridge.LocationToWest = guardPost;
            bridge.LocationToEast = spiderField;

            spiderField.LocationToWest = bridge;

            //Add the locations to the static list
            Locations.Add(home);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(alchemistsGarden);
            Locations.Add(farmhouse);
            Locations.Add(farmersField);
            Locations.Add(bridge);
            Locations.Add(spiderField);
        }
        #endregion PopulateLocations

        #region PopulateSpells
        private static void PopulateSpells()
				{
					Spells.Add(new Spell(SPELL_ID_FIRE, "Fire", 3, 7, 4, 1, 0));
					Spells.Add(new Spell(SPELL_ID_FROST, "Frost", 3, 7, 4, 1, 0));
					Spells.Add(new Spell(SPELL_ID_LIGHTNING, "Lightning", 6, 10, 7, 1, 0));
					Spells.Add(new Spell(SPELL_ID_LIGHT, "Light", 2, 8, 3, 1, 0));
					Spells.Add(new Spell(SPELL_ID_DARK, "Dark", 2, 8, 3, 1, 0));
				}
        #endregion PopulateSpells

        public static Item ItemByID(int id)
        {
            foreach (Item item in Items)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }

            return null;
        }

        public static Monster MonsterByID(int id)
        {
            foreach (Monster monster in Monsters)
            {
                if (monster.ID == id)
                {
                    return monster;
                }
            }

            return null;
        }

        public static Quest QuestByID(int id)
        {
            foreach (Quest quest in Quests)
            {
                if (quest.ID == id)
                {
                    return quest;
                }
            }

            return null;
        }

        public static Location LocationByID(int id)
        {
            foreach (Location location in Locations)
            {
                if (location.ID == id)
                {
                    return location;
                }
            }

            return null;
        }
        
        public static Spell SpellByID(int id)
        {
        		foreach (Spell spell in Spells)
        		{
        				if(spell.ID == id)
        				{
        						return spell;
        				}
        		}
        				
        		return null;
        }
    }
}