using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

/* A generalized need draining bullet. Assuming the target has no immunity hediffs, roll under the drain chance to reduce a need.
 * Has no effect if the target doesn't have the desired need. */
namespace MoodndBehaviorsAndEvents
{
    public class InjestionOutcomeDoer_RemoveHediffs : IngestionOutcomeDoer
    {

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            int hediffsRemoved = 0;
            foreach (HediffDef hediffDef in this.hediffDefs)
            {

                var hediffOnPawn = pawn.health?.hediffSet?.GetFirstHediffOfDef(hediffDef);
                if (hediffOnPawn != null)
                {
                    HealthUtility.Cure(hediffOnPawn);
                    hediffsRemoved++;
                    if (this.maxHediffsToRemove > 0 && hediffsRemoved >= this.maxHediffsToRemove)
                    {
                        return;
                    }
                }
            }
        }

        // TODO what does this do? copied from  IngestionOutcomeDoer_GiveHediff in base game code
        /*public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            if (parentDef.IsDrug && this.chance >= 1f)
            {
                foreach (StatDrawEntry statDrawEntry in this.hediffDef.SpecialDisplayStats(StatRequest.ForEmpty()))
                {
                    yield return statDrawEntry;
                }
                IEnumerator<StatDrawEntry> enumerator = null;
            }
            yield break;
            yield break;
        }*/
        
        public List<HediffDef> hediffDefs;

        public int maxHediffsToRemove = -1;
    }
}
