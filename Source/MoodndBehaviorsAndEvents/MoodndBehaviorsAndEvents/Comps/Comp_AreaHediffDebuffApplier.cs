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
                // check that this pawn is able to apply the effect right now according to general and parameterized requirements
                thisPawn = this.parent as Pawn; 
                if (thisPawn != null && thisPawn.Map != null && !thisPawn.Dead && !thisPawn.Downed && (!Props.needsToBeTamed || (Props.needsToBeTamed && thisPawn.Faction != null && thisPawn.Faction.IsPlayer)))
                {
                    int targetsAffected = 0;
                    // check for potential targets in a radius around the broadcasters
                    foreach (Thing thing in GenRadial.RadialDistinctThingsAround(thisPawn.Position, thisPawn.Map, Props.radius, true))
                    {
                        Pawn pawn = thing as Pawn;
                        //Ensure that this thing is a valid target by checking a bunch of attributes of the target pawn, some based on the comp config.
                        if (pawn != null && !pawn.Dead && !pawn.Downed && // basics
                            (!Props.onlyAffectColonists || pawn.IsColonist) && // IsColonist check
                             (!Props.requiresPsychicSensitivity || pawn.GetStatValue(StatDefOf.PsychicSensitivity, true) > 0f) && // psychic check
                             (!Props.affectsSameFaction || this.parent.Faction == null || this.parent.Faction.Equals(pawn.Faction))) // faction check
                        {
                            // Roll to hit based on DGI params
                            if (DebuffLogicUtil.DoesDebuffHappen(pawn, Props.dgi))
                            {
                                // if hit add the hediff
                                pawn.health.AddHediff(Props.hediff);
                                // add/increase resistance if needed
                                if (Props.dgi.resistanceHediffDef != null)
                                {
                                    var resistanceHediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(Props.dgi.resistanceHediffDef);
                                    if (resistanceHediff != null)
                                    {
                                        resistanceHediff.Severity += Props.dgi.resistanceGrowth;
                                    }
                                    else
                                    {
                                        pawn.health.AddHediff(Props.dgi.resistanceHediffDef);
                                    }
                                }
                                // max targets logic, we only consider a target 'affected' if we rolled to hit them, so immune targets don't add to this count
                                targetsAffected++;
                                if (Props.maxTargetsPerBroadcast > 0 && targetsAffected >= Props.maxTargetsPerBroadcast)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                tickCounter = 0;
            }
        }
    }
}
