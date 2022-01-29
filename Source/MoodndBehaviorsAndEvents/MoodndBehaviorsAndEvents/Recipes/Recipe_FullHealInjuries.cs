using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MoodndBehaviorsAndEvents
{

    class Recipe_FullHealInjuries : Recipe_Surgery
    {
        private static readonly int maxCount = 100;

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            // TODO use something not deprecated?
            HealthUtility.HealNonPermanentInjuriesAndFreshWounds(pawn);
            Hediff healed = HealthUtility.HealRandomPermanentInjury(pawn);
            for (int safetyCount = 0; safetyCount < maxCount; safetyCount++)
            {
                if (healed == null)
                {
                    break;
                }
                healed = HealthUtility.HealRandomPermanentInjury(pawn);
            }
        }
    }
}
