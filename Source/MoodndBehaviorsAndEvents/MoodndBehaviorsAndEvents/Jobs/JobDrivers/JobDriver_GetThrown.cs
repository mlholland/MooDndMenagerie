using System;
using System.Collections.Generic; 
using Verse;
using Verse.AI;
using RimWorld; 

namespace MoodndBehaviorsAndEvents
{
    class JobDriver_GetThrown : JobDriver
    {
        // Token: 0x17000988 RID: 2440
        // (get) Token: 0x06003327 RID: 13095 RVA: 0x00125210 File Offset: 0x00123410
        private Building building
        {
            get
            {
                return (Building)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        // Token: 0x17000989 RID: 2441
        // (get) Token: 0x06003328 RID: 13096 RVA: 0x00125238 File Offset: 0x00123438
        private Thing Item
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Thing;
            }
        }
        
        // Token: 0x06003329 RID: 13097 RVA: 0x0012525C File Offset: 0x0012345C
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            this.pawn.Map.pawnDestinationReservationManager.Reserve(this.pawn, this.job, this.job.targetA.Cell);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_General.StopDead();
            Toil jumpTarget = Toils_Goto.GotoCell(TargetA.Cell, PathEndMode.OnCell);
            yield return Toils_Jump.Jump(jumpTarget); 
            yield break;
        }
        
    }
}
