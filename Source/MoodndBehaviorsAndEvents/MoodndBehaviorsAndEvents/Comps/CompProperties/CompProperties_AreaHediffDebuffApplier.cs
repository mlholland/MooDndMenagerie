﻿using Verse;
using System.Collections.Generic;

namespace MoodndBehaviorsAndEvents
{
    public class CompProperties_AreaHediffDebuffApplier : CompProperties
    {
        // The modified version of the CompProperties_HediffEffecter class from VEF with a few extra params

        public int radius = 1;
        public int tickInterval = 1000;
        public HediffDef hediff; 
        public bool onlyAffectColonists = false;
        public bool requiresPsychicSensitivity = false;
        public bool needsToBeTamed = true;
        public bool affectsSameFaction = true;
        public float applicationChance = 1f;
        public int maxTargets = 0; // infinite targets allowed if <= 0 
        public HediffDef resistanceHediff; // if nonnull, then add this hediff everytime we try to add the main hediff, even if unsuccessful (but not if they're immune for any reason)
        public float resistanceScaling = 1f;
        public float resistanceImmunityFloor = 1f;
        public float resistancePerApplication;
        public List<HediffDef> immunityHediffs = new List<HediffDef>();
        public CapacityScalingValues capacityScalingValues = null;
        public DebuffLogicUtil.DebuffGiverInputs dgi;
         
        public CompProperties_AreaHediffDebuffApplier()
        {
            this.compClass = typeof(Comp_AreaHediffDebuffApplier);
        }


    }
}
