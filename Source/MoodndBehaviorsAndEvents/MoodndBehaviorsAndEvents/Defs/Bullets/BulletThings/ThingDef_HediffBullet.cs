using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

/* Def to describe the inputs to a hediff-adding projectile*/
namespace MoodndBehaviorsAndEvents
{
    public class ThingDef_HediffBullet : ThingDef
    {
        // mandatory
        public HediffDef hediffToAdd;
        // optional
        public DebuffLogicUtil.DebuffGiverInputs dgi; // Contains a bunch of options for what affects the likelihood that a hediff will be added.
        public float addHediffChance = 1f; // chance the hediff is applied before other values like capacity scaling or resistances are applied
        public bool modifyExistingHediff = true; // if false, create and add a new hediff every time
        public Verse.FloatRange severityToAdd = Verse.FloatRange.Zero; // If the max is non zero, then assume this hediff uses severity, and add a random amount from this range to the created hediff
        public bool addSeverityToExisting = true; // if false, replace severity with new value only if the newly rolled value is higher
        public bool addHediffToWholeBody = true; // If false, applies the hediff to the body part that was hit, and becomes incompatible with modifyExistingHediff=true. The hit part is calculated separately from normal damage.
       
        // TODO figure out if this is possible, not sure if I can really modify hediff durations when they're added
        // Was previously seeing strange glitches where the duration would NOT be set according to the below value
        // on the first application of a hediff, and possibly other problems that I don't recall now.
        //public Verse.IntRange hediffDurationRange = new Verse.IntRange(3600, 3600);
        //public bool addDurationToExisting = false;
    }
}
