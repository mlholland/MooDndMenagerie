using Verse;

/* Def to describe the inputs to a sleep drain projectile*/
namespace MoodndBehaviorsAndEvents
{
    public class ThingDef_SleepDrainBullet : ThingDef
    {
        public DebuffLogicUtil.DebuffGiverInputs dgi;
        public float narcChanceAtZeroRest = 0.5f; 
        public float drainAmount = 0.05f; 
    }
}
