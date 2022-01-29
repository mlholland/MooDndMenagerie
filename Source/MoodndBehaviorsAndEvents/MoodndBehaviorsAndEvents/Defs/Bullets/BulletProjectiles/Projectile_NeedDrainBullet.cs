using Verse;
using RimWorld;

/* A generalized need draining bullet. Assuming the target has no immunity hediffs, roll under the drain chance to reduce a need.
   Has no effect if the target doesn't have the desired need. */
namespace MoodndBehaviorsAndEvents
{
    public class Projectile_NeedDrainBullet : Bullet
    {

        public ThingDef_NeedDrainBullet Def
        {
            get
            {
                return this.def as ThingDef_NeedDrainBullet;
            }
        }
        
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            if (Def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                if (DebuffLogicUtil.DoesDebuffHappen(hitPawn, Def.dgi))
                {
                    var pawnNeed = hitPawn.needs.TryGetNeed(Def.needToDrain);
                    if (pawnNeed != null)
                    {
                        pawnNeed.CurLevel -= Def.drainAmount;
                        DebuffLogicUtil.AddOrModifyResistanceIfNeeded(hitPawn, Def.dgi);
                    }
                }
            }
        }
    }
}
