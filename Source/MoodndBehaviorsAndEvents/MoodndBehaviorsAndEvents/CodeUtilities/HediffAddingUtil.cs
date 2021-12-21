using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

/* Utility functions related to pawn logic. Generally used when trying to apply hediffs to determine whether or not to try adding
 * the hediff, and how likely that application is to work.
*/
namespace MoodndBehaviorsAndEvents
{
    public class HediffAddingUtil
    {

        public class DebuffGiverInputs
        {
            public float baseApplicationChance = 1f;
            public List<HediffDef> immunityHediffDefs = null;
            public HediffDef resistanceHediffDef = null;
            public float resistanceScaling = 1;
            public float resistanceImmunityFloor = 1;
            public float resistanceGrowth = 0.2f;
            public float shieldPierceChance = 0f;
            public float shieldDamage = 0f;
            public CapacityScalingValues csv = null;
            public bool fireImmunityMote = false;
            public bool affectedByShields = true;
        }

        /* A generalized function that applies all possible hit modifiers based on the inputted DebuffGiverInputs
         * This means that it can possibly consider any combination of a worn shield belt, immunity and resistance hediffs, and a relevant capacity.
         * For the most part, all null DebuffGiverInput fields are either checked then ignored, or assumed to not be null.
         * Returns false if the inputted pawn is null, but true if the inputs are null while the pawn isn't.
         */
        public static bool DoesDebuffHappen(Pawn hitPawn, DebuffGiverInputs inputs)
        {
            if (hitPawn == null)
            {
                return false;
            }
            if (inputs == null)
            {
                return true;
            }

            float adjustedApplicationChance = inputs.baseApplicationChance;

            // shield belt effect
            if (inputs.affectedByShields)
            {
                adjustedApplicationChance *= PawnShieldResistance(hitPawn, inputs.shieldDamage, inputs.shieldPierceChance);
            }

            // immunity effect
            if (PawnHasAnyHediff(hitPawn, inputs.immunityHediffDefs))
            {
                adjustedApplicationChance = 0;
            }
            // resistance hediff effect
            adjustedApplicationChance *= PawnHediffResistanceScale(hitPawn, inputs.resistanceHediffDef, inputs.resistanceScaling, inputs.resistanceImmunityFloor);
            // capacity scaling effect
            adjustedApplicationChance *= PawnCapacityScaling(hitPawn, inputs.csv);

            if (adjustedApplicationChance == 0)
            {
                if (inputs.fireImmunityMote)
                {
                    MoteMaker.ThrowText(hitPawn.PositionHeld.ToVector3(), hitPawn.MapHeld, "Immune!", 12f);
                }
            }
            else
            {
                float hitRoll = Rand.Value;
                if (hitRoll <= adjustedApplicationChance)
                {
                    return true;
                }
            }
            return false;
        }
        

        /* Returns true if both inputs are non-null and the pawn contains at least one instance of any of the listed hediff types*/
        public static bool PawnHasAnyHediff(Pawn pawn, List<HediffDef> hediffDefs)
        {
            if (hediffDefs == null || pawn == null)
            {
                return false;
            }
            foreach (HediffDef hediffDef in hediffDefs)
            {
                var hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(hediffDef);
                if (hediff != null)
                {
                    return true;
                }
            }
            return false;
        }

        /* Returns 1  if any inputs are null, or if this pawn doesn't have the hediff in question.
         * Otherwise returns a value from 0 to 1 based on the hediff's severity, with high severities resulting in a lower return value.
         * The idea is that this value will then be multiplied by the probability that something will occur, effectively adding 'resistance' to that occurence.
         */
        public static float PawnHediffResistanceScale(Pawn pawn, HediffDef resistanceHediffDef, float resistanceScaling = 1f, float immunityFloor = 1f)
        {
            if (pawn == null || resistanceHediffDef == null)
            {
                return 1;
            }
            var resistanceHediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(resistanceHediffDef);
            if (resistanceHediff != null)
            {
                if (immunityFloor > 0 && resistanceHediff.Severity >= immunityFloor)
                {
                    return 0; // immunity
                } else
                {
                    return (1 - Mathf.Min(1, resistanceHediff.Severity * resistanceScaling));
                }
            }
            return 1; // no resistance
        }

        /* Returns a scaling value based on inputted pawn's capacity.
         * Returns 1 (normal scaling) if any inputs are null.
         * Returns 0 if this pawn doesn't have the capacity in question, or if it's LEC minimumCapcacityRequired 
         * Returns (capacity value * capacityScaling) otherwise. */
        public static float PawnCapacityScaling(Pawn pawn, CapacityScalingValues csv)
        {
            if (pawn == null || csv == null ||  csv.capacityDef == null)
            {
                return 1; // inputs null, something went wrong, so return the value least likely to make things break;
            }
            bool capableOf = pawn.health.capacities.CapableOf(csv.capacityDef);
            if (capableOf)
            {
                float capVal = pawn.health.capacities.GetLevel(csv.capacityDef);
                if (capVal <= csv.minCapacityRequired)
                { 
                    return 0; // capacity is low enough that scaling is zero'd
                }
                else
                { 
                    return (csv.allowSuperCentennialCapScaling ? capVal : Mathf.Min(1, capVal)) * csv.capacityScaling; // apply normal scaling
                }
            }
            else
            {
                return 0; // Pawn doesn't have this capacity, return zero scaling.
            }
        }

        // returns 1 if the inputted pawn is not wearing an active shield.
        // If they do have an active shield, then return the shieldPiercing value.
        // Has a possible side-effect: If the inputted pawn is wearing a shield, and the shieldDamage input is positive,
        // then the shield will be dealt shieldDamage points of damage.
        public static float PawnShieldResistance(Pawn pawn, float shieldDamage = 0f, float shieldPiercing = .5f)
        {
            if (pawn != null && pawn.apparel != null)
            {
                foreach(Apparel app in pawn.apparel.WornApparel)
                {
                    if(app.def.Equals(ThingDefOf.Apparel_ShieldBelt))
                    {
                        ShieldBelt belt = (ShieldBelt)app;
                        if(belt.ShieldState == ShieldState.Active)
                        {
                            if (shieldDamage > 0)
                            {
                                DamageInfo shieldDInfo = new DamageInfo(DamageDefOf.Bullet, shieldDamage / 3);
                                belt.CheckPreAbsorbDamage(shieldDInfo);
                            }
                            return shieldPiercing;
                        }
                    }
                }
            } 
            return 1;
        }

        // If the inputted dgi has a non-null resistanceHediffDef, then this function
        // will either add an instance of said hediffDef to the inputted pawn, or increase
        // the hediff's severity by the dgi's specified amount.
        // This function assumes that pawn and dig themselves are non-null.
        public static void AddOrModifyResistanceIfNeeded(Pawn pawn, DebuffGiverInputs dgi)
        {
            if (dgi.resistanceHediffDef != null)
            {
                var resistanceHediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(dgi.resistanceHediffDef);
                if (resistanceHediff != null)
                {
                    resistanceHediff.Severity += dgi.resistanceGrowth;
                }
                else
                {
                    pawn.health.AddHediff(dgi.resistanceHediffDef);
                }
            }
        }
    }
}
