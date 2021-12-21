using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

// Removed the current element of a pawn's job queue, used in conjunction with forced behaviors to make sure they don't last too long.
namespace MoodndBehaviorsAndEvents
{
    public class HediffComp_EnrageAtHighPain : HediffComp
    {
        public int ticksSinceLastCheck = 0;
        private static HediffDef rageGrace = HediffDef.Named("DND_FleshGolemRageGrace");
        private static List<HediffDef> immunityDefs = new List<HediffDef>() { rageGrace };



        public HediffCompProperties_EnrageAtHighPain Props
        {
            get
            {
                return (HediffCompProperties_EnrageAtHighPain)this.props;
            }
        }
        

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            ticksSinceLastCheck++;
            if (ticksSinceLastCheck >= Props.painCheckFrequencyTicks)
            {
                ticksSinceLastCheck = 0;
                if(!HediffAddingUtil.PawnHasAnyHediff(this.Pawn, immunityDefs) && this.Pawn.health.hediffSet.PainTotal >= Props.painThreshold) 
                {

                    Hediff addedDebuff = this.Pawn.health.AddHediff(rageGrace);
                    if (base.Pawn.RaceProps.Humanlike)
                    {
                        Log.Error(string.Format("Moodnd: Tried to apply enrageAtHighPain comps to humanlike pawn ({0}). This should only ever apply to flesh golem animals", this.Pawn.Name));
                    }
                    else if (base.Pawn.RaceProps.Animal 
                        && base.Pawn.mindState.mentalStateHandler.CurStateDef != MentalStateDefOf.Manhunter 
                        && base.Pawn.Awake() 
                        && base.Pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, this.parent.def.LabelCap, false, false, null, true, false, false) 
                        && base.Pawn.Spawned)
                    {
                        this.SendLetter(MentalStateDefOf.Berserk);
                    }
                }
            }
        }

        private void SendLetter(MentalStateDef mentalStateDef)
        {
            Find.LetterStack.ReceiveLetter((mentalStateDef.beginLetterLabel ?? mentalStateDef.LabelCap).CapitalizeFirst() + ": " + base.Pawn.LabelShortCap, base.Pawn.mindState.mentalStateHandler.CurState.GetBeginLetterText() + "\n\n" + "CausedByHediff".Translate(this.parent.LabelCap), LetterDefOf.ThreatSmall, base.Pawn, null, null, null, null);
        }
    }
}
