using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

/* Def to describe the inputs to a sleep drain projectile*/
namespace MoodndBehaviorsAndEvents
{
    public class ThingDef_SleepDrainBullet : ThingDef
    {
        public HediffAddingUtil.DebuffGiverInputs dgi;
        public float narcChanceAtZeroRest = 0.5f;
        public float drainChance = 0.05f;
        public float drainAmount = 0.05f; 
    }
}
