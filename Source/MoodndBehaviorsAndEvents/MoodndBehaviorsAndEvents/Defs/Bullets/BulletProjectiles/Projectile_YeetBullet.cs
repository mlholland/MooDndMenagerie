using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

/* A bullet that throws the hit creature, then does damage to their lower body. */
namespace MoodndBehaviorsAndEvents
{
    public class Projectile_YeetBullet : Bullet
    {

        public ThingDef_YeetBullet Def
        {
            get
            {
                return this.def as ThingDef_YeetBullet;
            }
        }
        
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);

            if (Def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                if (HediffAddingUtil.DoesDebuffHappen(hitPawn, Def.dgi))
                {
                    // todo this first before the pawn in cached to avoid possible issues
                    HediffAddingUtil.AddOrModifyResistanceIfNeeded(hitPawn, Def.dgi);
                    IntVec3 targetDirection;
                    if (RCellFinder.TryFindDirectFleeDestination(this.launcher.Position, 9f, hitPawn, out targetDirection))
                    {
                        Projectile_Pawn_Linear flyingPawn = (Projectile_Pawn_Linear)GenSpawn.Spawn(ThingDef.Named("DND_Projectile_Pawn_Suspend"), hitPawn.Position, hitPawn.Map);
                        flyingPawn.Launch(hitPawn, hitPawn.Position.ToVector3(), targetDirection, hitPawn, null);
                    }
                }
            }
        }
    }
}
