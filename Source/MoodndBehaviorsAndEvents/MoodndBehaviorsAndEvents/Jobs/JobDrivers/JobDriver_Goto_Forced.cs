using System.Collections.Generic;  
using Verse.AI;
using Verse;
using RimWorld;

// Todo: maybe try to abstract this entire behavior out into an extension of JobDef which takes in a different Jobdef as input?
/* Wrapper around the standard FleeAndCower jobdriver with a hard-coded maximum duration.*/
namespace MoodndBehaviorsAndEvents
{
    class JobDriver_Goto_Forced : JobDriver_Goto
    {
        private int ticksPassed = 0; // todo validate the safety of this method, especially with rimthreaded.
        private int maxForcedJobTicks = 360; // todo make this configurable
        protected override IEnumerable<Toil> MakeNewToils()
        {
            base.AddEndCondition(() =>
            {
                ticksPassed++;
                if (ticksPassed >= maxForcedJobTicks)
                {
                    return JobCondition.Succeeded;
                }
                return JobCondition.Ongoing;
            });
            
            yield return new Toil
            {
                atomicWithPrevious = true,
                defaultCompleteMode = ToilCompleteMode.Instant,
                initAction = delegate ()
                {
                    // todo figure out how to make this spawn a heart, since that's technically not a mote?
                    MoteBubble moteBubble2 = (MoteBubble)ThingMaker.MakeThing(ThingDefOf.Mote_ThoughtGood, null); 
                    moteBubble2.Attach(this.pawn);
                    GenSpawn.Spawn(moteBubble2, pawn.Position, pawn.Map, WipeMode.Vanish);
                    //return moteBubble2;

                    // MoteMaker.MakeColonistActionOverlay(this.pawn, ThingDefOf.Mote_ThoughtGood);
                }
            };

            IEnumerable<Toil> toils = base.MakeNewToils();
            foreach (Toil toil in toils)
            {
                yield return toil;
            }
            yield break;

        }
    }
}
