 using System.Collections.Generic; 
using Verse;
using RimWorld; 

// A modified version of the CompHediffEffector class from VEF. The original lacked a bunch of features I needed, so I bootstapped off that to produce this.
// When added to creature, this comp periodically checks for nearby creatures that meet any of a variety of checks, and if they pass, applies a specific
// hediff to them
namespace MoodndBehaviorsAndEvents
{
    public class CompHediffEffecterRework : ThingComp
    {
        public int tickCounter = 0;
        public List<Pawn> pawnList = new List<Pawn>();
        public Pawn thisPawn;

        public CompProperties_HediffEffecterRework Props
        {
            get
            {
                return (CompProperties_HediffEffecterRework)this.props;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            //todo add this back in when I add my own settings?
            //if (AnimalBehaviours_Settings.flagEffecters)
            //{
            tickCounter++;
            //Only do anything every tickInterval
            if (tickCounter > Props.tickInterval)
            { 
                thisPawn = this.parent as Pawn;
                //Null map check. Also will only work if pawn is not dead or downed
                if (thisPawn != null && thisPawn.Map != null && !thisPawn.Dead && !thisPawn.Downed && (!Props.needsToBeTamed || (Props.needsToBeTamed && thisPawn.Faction != null && thisPawn.Faction.IsPlayer)))
                {
                    int targetsAffected = 0;
                    foreach (Thing thing in GenRadial.RadialDistinctThingsAround(thisPawn.Position, thisPawn.Map, Props.radius, true))
                    { 
                        Pawn pawn = thing as Pawn;
                        //Only work on colonists, unless not-OnlyAffectColonists
                        if (pawn != null &&(pawn.IsColonist || !Props.onlyAffectColonists))
                        {
                            //Only work on not dead, not downed, not psychically immune colonists
                            if (!pawn.Dead && !pawn.Downed && (!Props.requiresPsychicSensitivity || pawn.GetStatValue(StatDefOf.PsychicSensitivity, true) > 0f))
                            {
                                // check if the current pawn is the applier's master if necessary
                                if (Props.onlyAffectsMaster)
                                {
                                    if (thisPawn.playerSettings.Master != null && thisPawn.playerSettings.Master.Equals(pawn)) {
                                        HediffDef mastersHediff = HediffDef.Named(Props.hediff);
                                        pawn.health.AddHediff(mastersHediff);
                                        return;
                                    }
                                    continue;
                                }
                                HediffDef diff = HediffDef.Named(Props.hediff);
                                pawn.health.AddHediff(diff);
                                targetsAffected++;
                                if (Props.maxTargets > 0 && targetsAffected >= Props.maxTargets)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                tickCounter = 0;
                }
            //} 
        }
    }
}
