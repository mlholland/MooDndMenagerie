using Verse;
using Verse.AI;
using RimWorld;

/* Decribes the special behavior of a sleep drain bullet, a modified need drain bullet that has the additional effect of potentially putting the target to
 * sleep if their rest stat is fully drained.*/
namespace MoodndBehaviorsAndEvents
{
    public class Projectile_SleepDrainBullet : Bullet
    {

        public ThingDef_SleepDrainBullet Def
        {
            get
            {
                return this.def as ThingDef_SleepDrainBullet;
            }
        }
        
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);
            if (Def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                if (DebuffLogicUtil.DoesDebuffHappen(hitPawn, Def.dgi))
                {
                    var pawnNeed = hitPawn.needs.TryGetNeed(NeedDefOf.Rest);
                    if (pawnNeed != null)
                    {
                        pawnNeed.CurLevel -= Def.drainAmount;
                        var forceSleepRoll = Rand.Value;
                        if (forceSleepRoll <= Def.narcChanceAtZeroRest)
                        {
                            Job forcedSleepJob = JobMaker.MakeJob(JobDefOf.LayDown, hitPawn.Position);
                            forcedSleepJob.forceSleep = true;
                            hitPawn.jobs.ClearQueuedJobs();
                            hitPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                            hitPawn.jobs.TryTakeOrderedJob(forcedSleepJob);
                        }
                        DebuffLogicUtil.AddOrModifyResistanceIfNeeded(hitPawn, Def.dgi);
                    }
                }
            }
        }
    }
}
