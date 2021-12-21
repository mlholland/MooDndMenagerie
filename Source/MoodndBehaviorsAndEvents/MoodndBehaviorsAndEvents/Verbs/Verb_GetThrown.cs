using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

/* Modified version of the Verb_Jump verb used by jump packs, but with less restrictive logic. Requires royalty.
    TODO: probably remove this since I replaced this with the yeet-bullet
     */
namespace MoodndBehaviorsAndEvents
{
    class Verb_GetThrown : Verb_Jump
    {
        protected override float EffectiveRange
        {
            get
            { 
                return 100f; // TODO don't hardcode this
            }
        } 
 
        protected override bool TryCastShot()
        {
            if (!ModLister.CheckRoyalty("Jumping"))
            {
                return false;
            }
            Pawn casterPawn = this.CasterPawn;
            IntVec3 cell = this.currentTarget.Cell;
            Map map = casterPawn.Map;
            PawnFlyer pawnFlyer = PawnFlyer.MakeFlyer(ThingDefOf.PawnJumper, casterPawn, cell);
            if (pawnFlyer != null)
            {
                GenSpawn.Spawn(pawnFlyer, cell, map, WipeMode.Vanish);
                return true;
            }
            return false;
        }

        /* TODO is this function needed?
        // Token: 0x060080F3 RID: 33011 RVA: 0x002DC0A0 File Offset: 0x002DA2A0
        public override void OrderForceTarget(LocalTargetInfo target)
        {
            Verb_Jump.<> c__DisplayClass6_0 CS$<> 8__locals1 = new Verb_Jump.<> c__DisplayClass6_0();
            CS$<> 8__locals1.<> 4__this = this;
            CS$<> 8__locals1.map = this.CasterPawn.Map;
            IntVec3 intVec = RCellFinder.BestOrderedGotoDestNear(target.Cell, this.CasterPawn, new Predicate<IntVec3>(CS$<> 8__locals1.< OrderForceTarget > g__AcceptableDestination | 0));
            Job job = JobMaker.MakeJob(JobDefOf.CastJump, intVec);
            job.verbToUse = this;
            if (this.CasterPawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false))
            {
                FleckMaker.Static(intVec, CS$<> 8__locals1.map, FleckDefOf.FeedbackGoto, 1f);
            }
        }*/


    }
}
