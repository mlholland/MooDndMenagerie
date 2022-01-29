using Verse;


/* Values related to capacity scaling in the context of determine the chance for effects to occur (ex: petrification application scaling with target sight). Generally used as an input
 * to the HediffAddingUtil.PawnCapacityScaling function and things that use said function in their calculations.
 */
namespace MoodndBehaviorsAndEvents
{
    public class CapacityScalingValues
    {
        public PawnCapacityDef capacityDef = null; // if non-null, scales the application chance by the targets capacity value by multiplying the two values together.
        public float capacityScaling = 1; // Changes how the capacity modifies the
        public float minCapacityRequired = 0; // If the target's scalingCapacity value is LEC this threshold, then the hediff can't be applied.
        public bool allowSuperCentennialCapScaling = true; // if false, then all capacity values above 100% are treated like 100% for the purposes of capacity scaling calculations
    }
}
