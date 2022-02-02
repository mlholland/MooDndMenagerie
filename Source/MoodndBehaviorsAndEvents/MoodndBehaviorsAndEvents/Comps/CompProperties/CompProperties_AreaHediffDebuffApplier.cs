using Verse;
using System.Collections.Generic;

namespace MoodndBehaviorsAndEvents
{
    public class CompProperties_AreaHediffDebuffApplier : CompProperties
    {
        // The modified version of the CompProperties_HediffEffecter class from VEF with a few extra params, as well as a plugin into my own combat utilities

        public int radius = 1;
        public int tickInterval = 1000;
        public HediffDef hediff; 
        public bool onlyAffectColonists = false;
        public bool requiresPsychicSensitivity = false;
        public bool needsToBeTamed = true;
        public bool affectsSameFaction = true;
        public int maxTargetsPerBroadcast = 0; // infinite targets allowed if <= 0  
        public DebuffLogicUtil.DebuffGiverInputs dgi;
         
        public CompProperties_AreaHediffDebuffApplier()
        {
            this.compClass = typeof(Comp_AreaHediffDebuffApplier);
        }


    }
}
