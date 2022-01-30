using System.Collections.Generic;
using Verse.AI; 
using RimWorld; 

/* Wrapper around the standard FleeAndCower jobdriver with a hard-coded maximum duration.*/
namespace MoodndBehaviorsAndEvents
{
    class JobDriver_FleeAndCower_Forced : JobDriver_FleeAndCower
    {
        // More observations from Niilo: There is also a small possibility that RimThreaded destructivly patches a method that you also patch, causing something to break. But thats very rare. And if it happens. Let us know, here or on the RT discord, so we can change the way we patch the problematic method
        private int ticksPassed = 0; // eventual todo: validate the safety of this method, especially with rimthreaded. (Check RT GH wiki for notes on compatability - Thanks Niilo007)
        private int maxForcedJobTicks = 300; // todo: make this configurable
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
