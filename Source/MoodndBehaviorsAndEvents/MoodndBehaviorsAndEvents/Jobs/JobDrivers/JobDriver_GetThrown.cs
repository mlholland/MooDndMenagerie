using System.Collections.Generic; 
using Verse;
using Verse.AI;

namespace MoodndBehaviorsAndEvents
{
    class JobDriver_GetThrown : JobDriver
    { 
        private Building building
        {
            get
            {
                return (Building)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }
        
        private Thing Item
        {
            get
            {
                return this.job.GetTarget(TargetIndex.B).Thing;
            }
        }
        
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
