using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

/* Makes the target walk to the shooter.*/
namespace MoodndBehaviorsAndEvents
{
    public class Projectile_ForceApproachBullet : Projectile_ForceJobBullet
    {
        // Assumed hitPawn is non-null by now
        // Should return null if the job can't be created successfully in children classes
        protected override Job CreateForcedJob(Pawn hitPawn)
        {
            Job forcedJob = new Job(Def.forcedJob);
            forcedJob.SetTarget(TargetIndex.A, new LocalTargetInfo(this.launcher.Position));
            return forcedJob;
        }
    }
}
