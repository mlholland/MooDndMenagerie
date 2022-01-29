using Verse;
using Verse.AI;
using RimWorld;

/* Makes the target run away as if fleeing.*/
namespace MoodndBehaviorsAndEvents
{
    public class Projectile_ForceFleeBullet : Projectile_ForceJobBullet
    { 
        // Assumed hitPawn is non-null by now
        // Should return null if the job can't be created successfully in children classes
        protected override Job CreateForcedJob(Pawn hitPawn)
        {
            IntVec3 targetDirection;
            if (RCellFinder.TryFindDirectFleeDestination(this.launcher.Position, 9f, hitPawn, out targetDirection))
            {
                Job forcedJob = new Job(Def.forcedJob);
                forcedJob.SetTarget(TargetIndex.A, new LocalTargetInfo(targetDirection));
                return forcedJob;
            }
            return null;
        }
    }
}
