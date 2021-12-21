using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace MoodndBehaviorsAndEvents
{

    class Recipe_FullHealInjuries : Recipe_Surgery
    {
        private static int maxCount = 100;

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
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
