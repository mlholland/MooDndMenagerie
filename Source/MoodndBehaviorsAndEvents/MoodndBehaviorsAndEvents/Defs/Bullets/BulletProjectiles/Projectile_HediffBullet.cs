using System.Collections.Generic;
using Verse;
using RimWorld;

/* This bullet tries to apply a hediff to the target. Has a lot of options described in the associated ThingDef_HediffBullet
 */
namespace MoodndBehaviorsAndEvents
{
    public class Projectile_HediffBullet : Bullet
    {

        public ThingDef_HediffBullet Def
        {
            get
            {
                return this.def as ThingDef_HediffBullet;
            }
        }
        
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            
            // sanity check that nothing important is null
            bool targetWasPawn = false;
            if (Def != null && hitThing != null && hitThing is Pawn hitPawn && hitPawn != null)
            { 
                targetWasPawn = true;
                if (DebuffLogicUtil.DoesDebuffHappen(hitPawn, Def.dgi)) // try to roll to 'hit'
                {
                    var hediffOnPawn = hitPawn.health?.hediffSet?.GetFirstHediffOfDef(Def.hediffToAdd);
                    Hediff hediff;
                    if (Def.modifyExistingHediff && hediffOnPawn != null) // if modify existing hediff if it's present and designated
                    {
                        hediff = hediffOnPawn;
                        if (Def.severityToAdd.max > 0) // add severity if this hediff uses it
                        {
                            float severity = Def.severityToAdd.RandomInRange;
                            if (Def.addSeverityToExisting) // increase existing hediff severity
                            {
                                hediff.Severity += severity;
                            }
                            else if (hediff.Severity < severity) // overwrite hediff severity with a re-rerolled value
                            { 
                                hediff.Severity = severity;
                            }
                        } 
                    }
                    else // Hediff adding logic that's independant of existing hediffs of the same type.
                    {

                        hediff = HediffMaker.MakeHediff(Def.hediffToAdd, hitPawn); // Make a new hediff 

                        if (Def.severityToAdd.max > 0f) // add severity if this hediff uses it
                        {
                            hediff.Severity = Def.severityToAdd.RandomInRange;
                        }

                        if (Def.addHediffToWholeBody) // just add the hediff as-is if it's a full-body effect
                        {
                            hitPawn.health.AddHediff(hediff);
                        }
                        else // Otherwise choose a body part to afflict. TODO: add logic for target specificity - would need to sync this with normal impact hit location if the bullet also does localized damage
                        {
                            IEnumerable<BodyPartRecord>  notMissingParts = hitPawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside, null, null);
                            BodyPartRecord result;
                            notMissingParts.TryRandomElementByWeight((BodyPartRecord x) => x.coverageAbs, out result);
                            if (result == null)
                            {
                                Log.Error("MooDND Projectile_HediffBullet - tried and failed to get a BodyPartRecord to apply a hediff do when evaluating an impact");
                                return;
                            }
                            hitPawn.health.AddHediff(hediff, result);
                        }
                    }

                    // Regardless of the exact details of what hediff we added or modified, the fact that we're in this if-statement means that it was probably applied, so run a resistance check
                    DebuffLogicUtil.AddOrModifyResistanceIfNeeded(hitPawn, Def.dgi);
                }
            }

            // extra check for dealing bonus damage to non-living things, mostly copied from normal bullet impact code, but with modified damage
            if (Def != null && hitThing != null && !targetWasPawn)
            {
                BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
                DamageInfo dinfo = new DamageInfo(this.def.projectile.damageDef, Def.bonusDamageToStructures, base.ArmorPenetration, this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing, true, true);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
            }
        }
    }
}
