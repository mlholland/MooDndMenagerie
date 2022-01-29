using Verse;

/* Utility functions that are effectively extensions or modifications to HediffSet behaviors.
*/
namespace MoodndBehaviorsAndEvents
{
    class HediffSetExtraLogic
    {
        
        public static Hediff TryGetDirectlyAddedPartFor(HediffSet hediffSet, BodyPartRecord part)
        {
            for (int i = 0; i < hediffSet.hediffs.Count; i++)
            {
                if (hediffSet.hediffs[i].Part == part && hediffSet.hediffs[i] is Hediff_AddedPart)
                {
                    return hediffSet.hediffs[i];
                }
            }
            return null;
        }
        
        public static Hediff TryGetAddedPartOrFirstAncestorDirectlyAddedParts(HediffSet hediffSet, BodyPartRecord part)
        {
            Hediff retHediff = TryGetDirectlyAddedPartFor(hediffSet, part);
            if (retHediff == null)
            {
                return TryGetAncestorFirstDirectlyAddedPart(hediffSet, part);
            }
            return retHediff;
        }

       
        public static Hediff TryGetAncestorFirstDirectlyAddedPart(HediffSet hediffSet, BodyPartRecord part)
        {
            if (part.parent == null)
            {
                return null;
            }
            return TryGetAddedPartOrFirstAncestorDirectlyAddedParts(hediffSet, part.parent);
        }
    }
}
