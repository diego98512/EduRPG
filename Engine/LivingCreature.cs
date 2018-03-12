namespace Engine
{
    public class LivingCreature
    {
        public int CurrentHitPoints { get; set; }

        public int MaximumHitPoints { get; set; }

        public int StatusEffect { get; set; }

        public int CurrentMana { get; set; }

        public int MaximumMana { get; set; }

        public LivingCreature(int currentHitPoints , int maximumHitPoints , int currentMana , int maximumMana , int statusEffect)
        {
            CurrentHitPoints = currentHitPoints;
            MaximumHitPoints = maximumHitPoints;
            CurrentMana = currentMana;
            MaximumMana = maximumMana;
            StatusEffect = statusEffect;
        }
    }
}