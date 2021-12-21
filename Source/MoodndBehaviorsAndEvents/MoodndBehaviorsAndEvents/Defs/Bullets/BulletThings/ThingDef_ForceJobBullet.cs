﻿using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

/* Def to describe the inputs to a sleep drain projectile*/
namespace MoodndBehaviorsAndEvents
{
    public class ThingDef_ForceJobBullet : ThingDef
    {
        public JobDef forcedJob;
        public HediffAddingUtil.DebuffGiverInputs dgi;
        public float applicationChance = 1f;
        public int minJobDurationTicks = 100;
        public int jobDurationVarianceTicks = 0;
    }
}
