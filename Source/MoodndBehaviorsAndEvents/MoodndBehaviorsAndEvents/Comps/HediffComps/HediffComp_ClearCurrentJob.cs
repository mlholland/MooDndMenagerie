using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

// Removed the current element of a pawn's job queue, used in conjunction with forced behaviors to make sure they don't last too long.
namespace MoodndBehaviorsAndEvents
{
    public class HediffComp_ClearCurrentJob : HediffComp
    {
        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (Pawn.jobs.curJob != null && !Pawn.jobs.curJob.def.playerInterruptible)
            {
                this.Pawn.jobs.EndCurrentJob(Verse.AI.JobCondition.InterruptForced); // todo add more logic to make sure we only removed forced jobs
            }
        }
    }
}
