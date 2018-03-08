using System.Collections.Generic;

namespace Engine
{
    public class Player : LivingCreature
    {
        public int Gold { get; set; }

        public int ExperiencePoints { get; set; }

        public int Level { get; set; }

        public List<InventoryItem> Inventory { get; set; }

        public List<PlayerQuest> Quests { get; set; }

        public Location CurrentLocation { get; set; }

        public Player(int currentHitPoints , int maximumHitPoints , int currentMana , int maximumMana , int gold , int experiencePoints , int level , int statusEffect) : base(currentHitPoints , maximumHitPoints , currentMana , maximumMana , statusEffect)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Level = level;
            CurrentMana = currentMana;
            CurrentHitPoints = currentHitPoints;

            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }
    }
}