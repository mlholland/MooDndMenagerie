using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

/* Used for rust monster and annihilator attacks. Maybe I'll eventually make this generic like the name suggests, but not now :\ 
 * .*/
namespace MoodndBehaviorsAndEvents
{
    class Verb_MeleeApplyHediff_MoreOptions : Verb_MeleeAttackDamage
    {
        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            // apply the normal melee attack first;
            DamageWorker.DamageResult damageResult =  base.ApplyMeleeDamageToTarget(target);

            // Todo can I delete the commented sections below from a previous iteration safely?
            //DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
            /*if (this.tool == null)
            {
                Log.ErrorOnce("Attempted to apply melee hediff without a tool", 383817350);
                return damageResult;
            }
            if (pawn == null)
            {
                Log.ErrorOnce("Attempted to apply melee hediff without pawn target", 783300530); //TODO figure out if this key is ok, just adding 0 to the original code
                return damageResult;
            }*/
            Pawn pawn = target.Thing as Pawn;
            // Cast the parent maneuver to a DebuffingManeuverDef. This verb can only be used with that def, and the extra data it contains.
            DebuffingManeuverDef dmd = DebuffingManeuverDef.GetDMDFromVerb(this);
            if (dmd == null )
            {
                Log.Error(String.Format("Moodnd Verb_MeleeApplyHediff_MoreOptions: custom verb expected maneuver subtype of DebuffingManeuverDef. But maneuver {0} could not be cast that way.", this.maneuver.defName));
                return damageResult;
            }
            if (dmd.hediffDef == null)
            {
                Log.Error(String.Format("Moodnd Verb_MeleeApplyHediff_MoreOptions: custom verb expected DebuffingManeuverDef {0} with a non-null hediff def.", this.maneuver.defName));
            }
            if (DebuffLogicUtil.DoesDebuffHappen(pawn, dmd.dgi)) {
                if (!dmd.onlyApplyHediffIfWounded || damageResult.wounded)
                { 
                    if (dmd.applyHediffToWholeBody)
                    {
                        damageResult.AddHediff(pawn.health.AddHediff(dmd.hediffDef, null, null, null));
                    } else if (damageResult.LastHitPart != null) {
                        damageResult.AddHediff(pawn.health.AddHediff(dmd.hediffDef, damageResult.LastHitPart, null, null));
                    } else
                    { 
                        damageResult.AddHediff(pawn.health.AddHediff(dmd.hediffDef, GetRandomVExternalPartWeighted(pawn), null, null));    
                    }

                }
            }
            return damageResult;
        }
        
        public override bool IsUsableOn(Thing target)
        {
            return target is Pawn;
        }

        public override bool IsMeleeAttack
        {
            get
            {
                return true;
            }
        }


        protected BodyPartRecord GetRandomVExternalPartWeighted(Pawn pawn)
        {
            HashSet<BodyPartRecord> validParts = new HashSet<BodyPartRecord>();
            IEnumerable<BodyPartRecord> notMissingParts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside, null, null);
            BodyPartRecord result;
            notMissingParts.TryRandomElementByWeight((BodyPartRecord x) => x.coverageAbs, out result);
            if (result == null)
            {
                Log.Error("MooDND Verb_MeleeApplyHediff_MoreOptions - tried and failed to get a BodyPartRecord to apply a hediff do when evaluating an impact");
                return null;
            }
            return result;
        }

    }
}
