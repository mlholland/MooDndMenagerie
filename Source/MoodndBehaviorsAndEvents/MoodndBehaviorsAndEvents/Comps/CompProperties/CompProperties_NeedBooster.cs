using Verse;
using RimWorld;

// A makes the given animal occasionally boost a specific need of nearby colonists.
namespace MoodndBehaviorsAndEvents
{
    public class CompProperties_NeedBooster : CompProperties
    {
        // if maxTargets < 1, then apply to all valid targets in range.
        public int maxTargets = 0;
        public int radius = 1;
        public float needBoostPct = 1.0f;
        // if minimumNeedPct < 0, then apply regardless of current Need value.
        public float maxTargetablePct = -1;
        public int tickInterval = 1000;
        public NeedDef needDef;
        public bool onlyAffectColonists = true;
        public bool requiresPsychicSensitivity = false;
        public bool needsToBeTamed = true;
        
        public CompProperties_NeedBooster()
        {
            this.compClass = typeof(Comp_NeedBooster);
        }


    }
}
