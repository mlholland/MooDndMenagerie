using System.Collections.Generic;
using Verse;
using RimWorld;

// A modified variant of the ComHediffEffector from the VEF that parameterizes psy sensitivity, among other things
namespace MoodndBehaviorsAndEvents
{
    public class Comp_AreaHediffDebuffApplier : ThingComp
    {
        public int tickCounter = 0;
        public List<Pawn> pawnList = new List<Pawn>();
        public Pawn thisPawn;


        public CompProperties_AreaHediffDebuffApplier Props
        {
            get
            {
                return (CompProperties_AreaHediffDebuffApplier)this.props;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            tickCounter++;
            //Only do anything every tickInterval
            if (tickCounter > Props.tickInterval)
            {
                thisPawn = this.parent as Pawn; 
                if (thisPawn != null && thisPawn.Map != null && !thisPawn.Dead && !thisPawn.Downed && (!Props.needsToBeTamed || (Props.needsToBeTamed && thisPawn.Faction != null && thisPawn.Faction.IsPlayer)))
                {
                    int targetsAffected = 0;
                    foreach (Thing thing in GenRadial.RadialDistinctThingsAround(thisPawn.Position, thisPawn.Map, Props.radius, true))
                    {
                        Pawn pawn = thing as Pawn; 
                        //Ensure that this is a valid target by checking a bunch of attributes of the target pawn, some based on the comp config.
                        if (pawn != null && !pawn.Dead && !pawn.Downed && // basics
                            (!Props.onlyAffectColonists || pawn.IsColonist) && // IsColonist check
                             (!Props.requiresPsychicSensitivity || pawn.GetStatValue(StatDefOf.PsychicSensitivity, true) > 0f) && // psychic check
                             (!Props.affectsSameFaction || this.parent.Faction == null || this.parent.Faction.Equals(pawn.Faction))) // faction check
                        {
                            

                            float applicationChance = Props.applicationChance;
                            //
                            foreach (HediffDef immunityHediffDef in Props.immunityHediffs)
                            {
                                var immunityHediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(immunityHediffDef);
                                if (immunityHediff != null)
                                {
                                    applicationChance = 0;
                                }
                            }

                            // calculate application chance reduction based on resistance hediff if present
                            if (applicationChance > 0 && Props.resistanceHediff != null)
                            {
                                var resistanceHediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(Props.resistanceHediff);
                                if (resistanceHediff != null)
                                {
                                    if (resistanceHediff.Severity >= Props.resistanceImmunityFloor)
                                    {
                                        applicationChance = 0;
                                    }
                                    else
                                    {
                                        float resistance = resistanceHediff.Severity * Props.resistanceScaling;
                                        if (resistance > 1f)
                                        {
                                            applicationChance = 0;
                                        }
                                        else
                                        {
                                            applicationChance *= 1f - resistanceHediff.Severity * Props.resistanceScaling;
                                        }
                                    }
                                }
                            } 

                            // apply capacity scaling if needed
                            if (Props.capacityScalingValues != null && applicationChance > 0)
                            {
                                applicationChance *= DebuffLogicUtil.PawnCapacityScaling(pawn, Props.capacityScalingValues);
                            } 

                            // done modifying the chance to hit
                            if (applicationChance == 0) // if effectively immune
                            {
                            // TODO consider adding this back in, but maybe conditionally based on settings or comp flags?
                                //MoteMaker.ThrowText(pawn.PositionHeld.ToVector3(), pawn.MapHeld, "Immune!", 12f);
                            }
                            else // otherwise roll to apply the hediff. Rolling LEC the applicationChance is a hit.
                            {
                                // TODO add visuals (probably based on comp settings
                                float hitRoll = Rand.Value;
                                if (hitRoll <= applicationChance)
                                {
                                    pawn.health.AddHediff(Props.hediff);
                                    if (Props.resistanceHediff != null)
                                    {
                                        var resistanceHediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(Props.resistanceHediff);
                                        if (resistanceHediff != null)
                                        {
                                            resistanceHediff.Severity += Props.resistancePerApplication;
                                        }
                                        else
                                        {
                                            pawn.health.AddHediff(Props.resistanceHediff);
                                        }
                                    }
                                } else if (hitRoll <= Props.applicationChance)
                                {
                                    //MoteMaker.ThrowText(pawn.PositionHeld.ToVector3(), pawn.MapHeld, "Resisted!", 12f);
                                }
                            } 
                            // max targets logic, we only consider a target 'affected' if we rolled to hit them, so immune targets don't add to this count
                            targetsAffected++;
                            if (Props.maxTargets > 0 && targetsAffected >= Props.maxTargets)
                            {
                                break;
                            }
                        }
                    }
                }
                tickCounter = 0;
            }
        }
    }
}
