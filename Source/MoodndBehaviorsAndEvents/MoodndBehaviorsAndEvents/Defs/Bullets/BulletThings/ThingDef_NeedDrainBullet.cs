using Verse;
using RimWorld;

/* Def to describe the inputs to a need drain projectile*/
namespace MoodndBehaviorsAndEvents
{
    public class ThingDef_NeedDrainBullet : ThingDef
    {
        public NeedDef needToDrain = NeedDefOf.Food;
        public DebuffLogicUtil.DebuffGiverInputs dgi;
        public float drainChance = 0.05f;
        public float drainAmount = 0.05f; 
    }
}
