using Verse;

namespace MoodndBehaviorsAndEvents
{
    public class HediffCompProperties_EnrageAtHighPain : HediffCompProperties
    {
        public float painThreshold = 0.5f;
        public int painCheckFrequencyTicks = 1250;

        public HediffCompProperties_EnrageAtHighPain()
        {
            this.compClass = typeof(HediffComp_EnrageAtHighPain);
        } 
    }
}
