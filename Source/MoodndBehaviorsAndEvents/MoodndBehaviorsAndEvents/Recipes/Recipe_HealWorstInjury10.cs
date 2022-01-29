using System.Collections.Generic;
using Verse;
using RimWorld;

// Does the same thing as a healer mech serum because I'm not sure if I can cleanly detatch the serum's functionality from the item itself.
namespace MoodndBehaviorsAndEvents
{
    class Recipe_HealWorstInjury10 : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            TaggedString retval;
            for (int i = 0; i < 10; i++)
            {
                retval = HealthUtility.FixWorstHealthCondition(pawn);
                // short circuit if there's nothing left to heal
                if (retval == null)
                {
                    return;
                }
            }
        }
    }
}
