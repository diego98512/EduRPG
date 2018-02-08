namespace Engine
{
    public class Spell
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int MinimumDamage { get; set; }

        public int MaximumDamage { get; set; }

        public int ManaCost { get; set; }

        public int CastingExperience { get; set; }

        public int Level { get; set; }

        public Spell(int id , string name , int minimumDamage , int maximumDamage , int manaCost , int castingExperience , int level)
        {
            ID = id;
            Name = name;
            MinimumDamage = minimumDamage;
            MaximumDamage = maximumDamage;
            ManaCost = manaCost;
            CastingExperience = castingExperience;
            Level = level;
        }
    }
}