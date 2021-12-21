using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

/* Def to describe the inputs to a need drain projectile*/
namespace MoodndBehaviorsAndEvents
{
    public class ThingDef_NeedDrainBullet : ThingDef
    {
        public NeedDef needToDrain = NeedDefOf.Food;
        public HediffAddingUtil.DebuffGiverInputs dgi;
        public float drainChance = 0.05f;
        public float drainAmount = 0.05f; 
    }
}
