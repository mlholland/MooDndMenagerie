using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

/* Def to describe the inputs to a projectile that throws the victim*/
namespace MoodndBehaviorsAndEvents
{
    public class ThingDef_YeetBullet : ThingDef
    {
        public HediffAddingUtil.DebuffGiverInputs dgi;
    }
}
