using System;
using System.Collections.Generic;  
using Verse.AI; 

// Todo: maybe try to abstract this entire behavior out into an extension of JobDef which takes in a different Jobdef as input?
/* Wrapper around the standard FleeAndCower jobdriver with a hard-coded maximum duration.*/
namespace MoodndBehaviorsAndEvents
{
    class JobDriver_Goto_Forced : JobDriver_Goto
    {
        private int ticksPassed = 0; // todo validate the safety of this method, especially with rimthreaded.
        private int maxForcedJobTicks = 300; // todo make this configurable
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
            IEnumerable<Toil> toils = base.MakeNewToils();
            return toils;
        }
    }
}
