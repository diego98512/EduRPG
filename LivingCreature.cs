namespace Engine
{
    public class LivingCreature
    {
        public int CurrentHitPoints { get; set; }

        public int MaximumHitPoints { get; set; }
        
        public int StatusEffect { get; set; }

        public LivingCreature(int currentHitPoints , int maximumHitPoints , int statusEffect)
        {
            CurrentHitPoints = currentHitPoints;
            MaximumHitPoints = maximumHitPoints;
            StatusEffect = statusEffect;
        }
    }
}