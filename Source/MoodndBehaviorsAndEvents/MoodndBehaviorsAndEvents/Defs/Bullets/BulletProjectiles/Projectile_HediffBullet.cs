using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

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
            if (Def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
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
                        // not needed since the hediff's already on the pawn
                        // hitPawn.health.AddHediff(hediff);

                        // See ThingDef_HediffBullet comment for why this is blocked out
                        /*if (Def.addDurationToExisting) 
                        {
                            // TODO fix the fact that the first hit always uses the time range specified in the hediff def, and not the one in the bullet def
                            HediffComp_Disappears disComp = hediff.TryGetComp<HediffComp_Disappears>();
                            if (disComp != null)
                            {
                                disComp.ticksToDisappear += Def.hediffDurationRange.RandomInRange;
                            }
                            else
                            {
                                Log.Error("Couldn't get disappear comp from hediff - this likely means that an invalid hediff was inputted into a TimedHediffBullet");
                            }
                        }*/
                    }
                    else // Hediff adding logic that's independant of existing hediffs of the same type.
                    {

                        hediff = HediffMaker.MakeHediff(Def.hediffToAdd, hitPawn); // Make a new hediff
                        // commented out for same reason as sibling block above
                        /* HediffComp_Disappears disComp = hediff.TryGetComp<HediffComp_Disappears>();
                        if (disComp != null)
                        {
                            HediffCompProperties_Disappears props = (HediffCompProperties_Disappears)disComp.props;
                            props.disappearsAfterTicks = Def.hediffDurationRange;
                        }
                        else
                        {
                            Log.Error("Couldn't get disappear comp from hediff - this likely means that an invalid hediff was inputted into a TimedHediffBullet");
                        }*/

                        if (Def.severityToAdd.max > 0f) // add severity if this hediff uses it
                        {
                            hediff.Severity = Def.severityToAdd.RandomInRange;
                        }

                        if (Def.addHediffToWholeBody) // just add the hediff as-is if it's a full-body effect
                        {
                            hitPawn.health.AddHediff(hediff);
                        }
                        else // Otherwise choose a body part to afflict. TODO: add logic for target specificity
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
        }
    }
}
