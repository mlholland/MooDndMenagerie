using Verse;
using RimWorld; 
using Verse.AI;

/* A comp to link the scroll of animation object to the animating job.
 */
namespace MoodndBehaviorsAndEvents
{
    class CompUseEffect_AnimateObject : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        { 
            if (!user.IsColonistPlayerControlled)
            {
                return;
            }
            if (!user.CanReserveAndReach(target, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
            {
                return;
            }
            Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("DND_AnimateObject"), target, this.parent);

            job.count = 1;
            user.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
        }
    }
}
