using Verse;

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
        public bool onlyAffectColonists = true; // if true, the hediff is only applied to colonists (humanoids in the player faction)
        public bool requiresPsychicSensitivity = false; // if true, then requires non-zero psychic sensitivity.
        public bool needsToBeTamed = true; // only try to apply the hediff nearby if the animal is tamed and part of the player faction
        public bool onlyAffectsMaster = false; // if true, only apply the hediff to the animal's master. Do nothing if it has no master.

        public CompProperties_HediffEffecterRework()
        {
            this.compClass = typeof(CompHediffEffecterRework);
        } 
    }
}
