using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

/* Used for rust monster and annihilator attacks. Maybe I'll eventually make this generic like the name suggests, but not now :\ 
 * TODO refactor this to just be an override of DamageInfosToApply*/
namespace MoodndBehaviorsAndEvents
{
    class Verb_MeleeApplyHediff_MoreOptions : Verb_MeleeAttackDamage
    {
        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        { 
            // apply the normal melee attack first;
            DamageWorker.DamageResult damageResult =  base.ApplyMeleeDamageToTarget(target); 
            // Cast the parent maneuver to a DebuffingManeuverDef. This verb can only be used with that def, and the extra data it contains.
            DebuffingManeuverDef dmd = DebuffingManeuverDef.GetDMDFromVerb(this);
            if (dmd == null)
            {
                Log.Error(String.Format("Moodnd Verb_MeleeApplyHediff_MoreOptions: custom verb expected maneuver subtype of DebuffingManeuverDef. But maneuver {0} could not be cast that way.", this.maneuver.defName));
                return damageResult;
            }
            if (dmd.hediffDef == null)
            {
                Log.Error(String.Format("Moodnd Verb_MeleeApplyHediff_MoreOptions: custom verb expected DebuffingManeuverDef {0} with a non-null hediff def.", this.maneuver.defName));
            }

            // Pawn-hitting logic
            Pawn pawn = target.Thing as Pawn;
            if (pawn != null && DebuffLogicUtil.DoesDebuffHappen(pawn, dmd.dgi)) {
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
            // extra check for dealing bonus damage to non-living things, mostly copied from normal bullet impact code, but with modified damage
            else if (pawn == null)
            {  
                ThingDef source;
                if (base.EquipmentSource != null)
                {
                    source = base.EquipmentSource.def;
                }
                else
                {
                    source = this.CasterPawn.def;
                }
                DamageInfo damageInfo = new DamageInfo(DamageDefOf.Blunt, dmd.structureBonusDamage, 0, -1f, this.caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true);
                damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                target.Thing.TakeDamage(damageInfo); 
            }

            return damageResult;
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
