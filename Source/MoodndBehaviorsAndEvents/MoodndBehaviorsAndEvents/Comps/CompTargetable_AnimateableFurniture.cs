using System.Collections.Generic;
using Verse;
using RimWorld;

/* Comp for determining targeting for scrolls of animation */
namespace MoodndBehaviorsAndEvents
{ 
    class CompTargetable_AnimateableFurniture : CompTargetable
    { 
        protected override bool PlayerChoosesTarget
        {
            get
            {
                return true;
            }
        }
        
        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = true,
                validator = ((TargetInfo x) => {
                    return base.BaseTargetValidator(x.Thing) && x.Thing != null && x.Thing.def != null && FurnitureToAnimatedObjectConverter.IsValidTarget(x.Thing.def);
                })
            };
        }
        
        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
            yield break;
        }
    }
}
