using Verse;

/* A maneuverDef with extra fields. Consumed and expected as input by custom verbs.
 * The idea is that the fields present allow for more logic on the verb's end when determining what happens based on the
 * target pawn's characteristics, and to avoid relying on existing fields in the toolDef or normal ManeuverDef that might
 * already be consumed
 */
namespace MoodndBehaviorsAndEvents
{
    public class DebuffingManeuverDef : ManeuverDef
    {
        public DebuffLogicUtil.DebuffGiverInputs dgi; // DG-inputs to be used by the verb, if needed.
        public HediffDef hediffDef; // A hediff to be used by the verb, if needed
        public bool applyHediffToWholeBody = false; // if dealing with an extra hediff, apply it to the whole body if true, and the last hit part according to normal calculations otherwise.
        // ... if there is no last part to hit, then a random part is selected and added to the DamageResult.
        public bool onlyApplyHediffIfWounded = false; // if true, then normal melee verb calculations must have determined that this effect wounded the target.

        public static DebuffingManeuverDef GetDMDFromVerb(Verb verb)
        {
            ManeuverDef maneuver = verb.maneuver;
            if (maneuver is DebuffingManeuverDef dmd)
            {
                return dmd;
            }
            return null;
        }
    }
}
