using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

/* Targeting for scrolls of animation */
namespace MoodndBehaviorsAndEvents
{
    class CompTargetable_AnimateableFurniture : CompTargetable
    {

        // Todo should move this into a def list instead of a hard-coded list
        private static HashSet<String> VALID_TARGETS = new HashSet<string>
        {
            "DiningChair",
            "ToolCabinet",
            "Stool"
        };

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
                    return base.BaseTargetValidator(x.Thing) && x.Thing != null && x.Thing.def != null && VALID_TARGETS.Contains(x.Thing.def.defName);
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
