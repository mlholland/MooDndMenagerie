using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace MoodndBehaviorsAndEvents
{
    class JobDriver_AnimateObject : JobDriver
    {

        private static string BASE_QUALITY_HEDIFF_STRING = "DND_AnimatedQuality";

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
            return this.pawn.Reserve(this.building, this.job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(this.Item, this.job, 1, -1, null, errorOnFailed);
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = Toils_General.Wait(50, TargetIndex.None);
            toil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return toil;
            yield return Toils_General.Do(new Action(this.Animate));
            yield break;
        }
        
        private void Animate()
        {
            //get the animal thing corresponding to this furniture
            ThingDef furnitureDef = this.building.def;
            // went with a pawnKindDef in the mapping just to make it harder to screw with an inproper inputted def
            PawnKindDef pawnKindDef = FurnitureToAnimatedObjectConverter.GetResultingPawnKindDef(furnitureDef);
            if (pawnKindDef == null)
            {
                Log.Error("Moodnd animate: Something went terribly wrong - no pawnKindDef could be found for a mid-animation piece of furniture. Aborting animation attempt.");
                return;
            } 
            ThingDef livingObjDef = pawnKindDef.race;
            //create the pawn
            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named(livingObjDef.defName), this.pawn.Faction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            
            // add quality hediff
            QualityCategory qc;
            bool useQuality = this.building.TryGetQuality(out qc);
            if (useQuality)
            {
                pawn.health.AddHediff(HediffDef.Named(BASE_QUALITY_HEDIFF_STRING + ((int)qc)));
            }
            // add material hediff
            if (this.building.Stuff != null && furnitureDef.stuffCategories != null && furnitureDef.stuffCategories.Count > 0)
            {
                pawn.health.AddHediff(HediffDef.Named(MaterialToHediffConverter.s_materialHediffDefNameBase + this.building.Stuff.defName));
            }
            // spen the creature and remove the furniture
            GenSpawn.Spawn(pawn, building.PositionHeld, building.MapHeld, WipeMode.Vanish);
            String buildingLabel = this.building.Label;
            this.building.DeSpawn();
            // announce success
            Messages.Message(String.Format("DND_FurnitureAnimated".Translate(), buildingLabel, pawn.Name), MessageTypeDefOf.PositiveEvent, true); 
        }
        
        private const TargetIndex CorpseInd = TargetIndex.A;
        
        private const TargetIndex ItemInd = TargetIndex.B;
        
        private const int DurationTicks = 600;
    }
}
