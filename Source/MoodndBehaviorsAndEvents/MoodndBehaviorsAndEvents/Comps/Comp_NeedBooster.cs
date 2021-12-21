using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

// A modifies the a need value of nearby pawns.
namespace MoodndBehaviorsAndEvents
{
    public class Comp_NeedBooster : ThingComp
    {
        public int tickCounter = 0;
        public List<Pawn> pawnList = new List<Pawn>();
        public Pawn thisPawn;

        public CompProperties_NeedBooster Props
        {
            get
            {
                return (CompProperties_NeedBooster)this.props;
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
                            {;
                                // todo: figure out why this was broken, something about finding 1670 elements named Mote_PsycastPsychicEffect
                                // MoteMaker.MakeAttachedOverlay(this.parent, ThingDef.Named("Mote_PsycastPsychicEffect"), Vector3.zero, 1f, -1f);

                                if(Props.needDef == null)
                                {
                                    Log.Error("Moodnd Comp_NeedBooster's needDef not set");
                                }
                                Need pawnNeed = pawn.needs.TryGetNeed(Props.needDef);
                                if (pawnNeed == null || (Props.maxTargetablePct >= 0 && pawnNeed.CurLevelPercentage > Props.maxTargetablePct))
                                {
                                    continue;
                                }
                                pawnNeed.CurLevelPercentage += Props.needBoostPct;
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
