using System;
using Verse;
using Verse.AI;
using RimWorld;

/* A generalized job-forcing bullet. Clears the afflicted pawns work queue, and replaces their current job with the inputted job.
 * By default this just creates the specified forced job with no inputs, but it can be extended by overriding the CreateForcedJob method
 * to allow for more complex forced behaviors.
 */
namespace MoodndBehaviorsAndEvents
{
    public class Projectile_ForceJobBullet : Bullet
    {

        public ThingDef_ForceJobBullet Def
        {
            get
            {
                return this.def as ThingDef_ForceJobBullet;
            }
        }
        
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            if (Def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                if (DebuffLogicUtil.DoesDebuffHappen(hitPawn, Def.dgi))
                {
                    Job forcedJob = CreateForcedJob(hitPawn); 
                    if (forcedJob != null) {
                        hitPawn.jobs.ClearQueuedJobs();
                        hitPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                        if(hitPawn.jobs.TryTakeOrderedJob(forcedJob))
                        {
                            DebuffLogicUtil.AddOrModifyResistanceIfNeeded(hitPawn, Def.dgi);
                        } else
                        {
                            Log.Error(String.Format("MooDnd managerie - Projectile_ForceJobBullet: failed to make pawn take job as part of forced job bullet impact. Jobdef was {0}", Def.forcedJob.defName));
                        }

                    } else
                    {
                        Log.Error(String.Format("MooDnd managerie - Projectile_ForceJobBullet: failed to create job as part of forced job bullet impact. Jobdef was {0}", Def.forcedJob.defName));
                    }
                }
            }
        }
        
        // Assumed hitPawn is non-null by now
        // Should return null if the job can't be created successfully in children classes
        protected virtual Job CreateForcedJob(Pawn hitPawn)
        {
            return new Job(Def.forcedJob);
        }
    }
}
