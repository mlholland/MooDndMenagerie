using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// A modified version of the CompProperties_HediffEffecter class from VEF with a few extra params that... works? IDK I couldn't make the original function for some reason.
namespace MoodndBehaviorsAndEvents
{
    public class CompProperties_HediffEffecterRework : CompProperties
    {
        // if maxTargets < 1, then apply to all valid targets in range.
        public int maxTargets = 0;
        public int radius = 1;
        public float severity = 1.0f;
        public int tickInterval = 1000;
        public string hediff = "Plague";
        public bool onlyAffectColonists = true;
        public bool requiresPsychicSensitivity = false;
        public bool needsToBeTamed = true;

        public CompProperties_HediffEffecterRework()
        {
            this.compClass = typeof(CompHediffEffecterRework);
        }


    }
}
