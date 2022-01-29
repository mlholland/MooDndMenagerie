using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

 /* A collection of static functions and helper classes designed to take in a material, and produce a hediff that represents that
  * material's attributes when applied to a creature. Used for Animated Object creatures. */
namespace MoodndBehaviorsAndEvents
{
    public class MaterialToHediffConverter
    {

        public static readonly string s_materialHediffDefNameBase = "DND_Material_Hediff_";

        // todo refactor this function out of existence?
        public static HediffDef MakeHediffFromMetal(ThingDef thing)
        {
            return MakeHediffFromThing(new HediffDef(), thing);

        }

        private static HediffDef MakeHediffFromThing(HediffDef newHediff, ThingDef stuff)
        {
            newHediff.defaultLabelColor = stuff.stuffProps.color;
            newHediff.generated = true;
            newHediff.hediffClass = typeof(HediffWithComps);
            newHediff.scenarioCanAdd = false; 
            newHediff.initialSeverity = 1; 
            newHediff.maxSeverity = 1;
            newHediff.comps = new List<HediffCompProperties>(); 
            newHediff.stages = new List<HediffStage>(); 
            newHediff.label = String.Format("DND_MaterialHediffLabel".Translate(), stuff.label);
            newHediff.defName = s_materialHediffDefNameBase + stuff.defName;
            newHediff.description = String.Format("DND_MaterialHediffDescription".Translate(), stuff.label);
            
             

            HediffStage hediffStage = new HediffStage();
            hediffStage.statOffsets = new List<StatModifier>();
            hediffStage.statFactors = new List<StatModifier>();
            hediffStage.capMods = new List<PawnCapacityModifier>();
            newHediff.stages.Add(hediffStage);
            ApplyStuffTypeMods(newHediff, stuff);
            
            return newHediff;
        }

        private static void ApplyStuffTypeMods(HediffDef newHediff, ThingDef stuff)
        {
            StuffProperties stuffProps = stuff.stuffProps;
            if (stuffProps != null) {
                // stat changes that are always added and calculated the same way
                // This mod assumes 0% flammability of creature it's applied to
                StatModifier flamMod = new StatModifier();
                flamMod.stat = StatDefOf.Flammability;
                flamMod.value = stuff.BaseFlammability;
                newHediff.stages[0].statOffsets.Add(flamMod);
                
                if (  stuffProps.categories != null && stuffProps.categories.Contains(StuffCategoryDefOf.Stony))
                {
                    AddStoneMods(newHediff, stuff);
                } else if (stuffProps != null && stuffProps.categories != null && stuffProps.categories.Contains(StuffCategoryDefOf.Woody))
                {
                    AddWoodyMods(newHediff, stuff);
                }
                else
                {
                    AddMetalMods(newHediff, stuff);
                }
                return;
            }
            AddMetalMods(newHediff, stuff);
        }

        private static void AddWoodyMods(HediffDef newHediff, ThingDef wood)
        {
            AddFlatStatMod(newHediff, StatDefOf.ArmorRating_Sharp, .2f, 0, 1f);
            AddFlatStatMod(newHediff, StatDefOf.ArmorRating_Blunt, .2f, 0, .5f);
            AddFlatStatMod(newHediff, StatDefOf.StuffPower_Armor_Heat, (1 - wood.BaseFlammability) * 2, 0, 2);

            // Some values are stored under thing.stuffProps.statFactors/Offsets instead
            AddStatMultiplier(newHediff, StatDefOf.IncomingDamageFactor, Mathf.Min(3, Mathf.Max(.3f, 1f / wood.stuffProps.statFactors.GetStatFactorFromList(StatDefOf.MaxHitPoints))), .3f, 3f);

            AddFlatCapacityModifier(newHediff, PawnCapacityDefOf.Manipulation, 1.15f - wood.stuffProps.statFactors.GetStatValueFromList(StatDefOf.WorkToMake, 1), 0f, 1f); 
        } 

        private static void AddMetalMods(HediffDef newHediff, ThingDef metal)
        {
            AddFlatStatMod(newHediff, StatDefOf.ArmorRating_Sharp, metal.statBases.GetStatFactorFromList(StatDefOf.StuffPower_Armor_Sharp) / 1.5f, 0, 1.25f);
            AddFlatStatMod(newHediff, StatDefOf.ArmorRating_Blunt,metal.statBases.GetStatFactorFromList(StatDefOf.StuffPower_Armor_Blunt) / 1.5f, 0, .75f);
            AddFlatStatMod(newHediff, StatDefOf.StuffPower_Armor_Heat, (1 - metal.BaseFlammability) * 2, 0, 2);

            // Some values are stored under thing.stuffProps.statFactors/Offsets instead
            AddStatMultiplier(newHediff, StatDefOf.IncomingDamageFactor, Mathf.Min(3, Mathf.Max(.3f, 1f / metal.stuffProps.statFactors.GetStatFactorFromList(StatDefOf.MaxHitPoints))), .3f, 3f);
            
            AddFlatCapacityModifier(newHediff, PawnCapacityDefOf.Manipulation, 1f - metal.stuffProps.statFactors.GetStatValueFromList(StatDefOf.WorkToMake, 1), -.3f, .5f); 
        }
         
        private static void AddStoneMods(HediffDef newHediff, ThingDef stone)
        {
            // Some values are stored under thing.stuffProps.statFactors/Offsets instead
            AddStatMultiplier(newHediff, StatDefOf.IncomingDamageFactor, 1f / stone.stuffProps.statFactors.GetStatFactorFromList(StatDefOf.MaxHitPoints), .1f, 3f);

            AddFlatStatMod(newHediff, StatDefOf.ArmorRating_Sharp, stone.stuffProps.statFactors.GetStatValueFromList(StatDefOf.WorkToMake, 1) * .7f, 0, 2f);
            AddFlatStatMod(newHediff, StatDefOf.ArmorRating_Blunt, stone.stuffProps.statFactors.GetStatValueFromList(StatDefOf.WorkToMake, 1), 0, 1.5f);
            AddFlatStatMod(newHediff, StatDefOf.StuffPower_Armor_Heat, (1 - stone.BaseFlammability) * 2, 0, 2);
            AddFlatCapacityModifier(newHediff, PawnCapacityDefOf.Manipulation, -.35f, -.35f, -.35f);
            
        }
         
        private static void AddLeatherMods(HediffDef newHediff, ThingDef leather)
        {
            throw new NotImplementedException("No leather-based creatures exist yet, so why is the AddLeatherMods function being called?");
        }

        private static void AddFlatStatMod(HediffDef newHediff, StatDef stat, float mod, float min=0f, float max=5f)
        {
            StatModifier statMod = new StatModifier();
            statMod.stat = stat;
            statMod.value = Math.Min(max, Math.Max(min, mod));
            newHediff.stages[0].statOffsets.Add(statMod);
        }

        private static void AddStatMultiplier(HediffDef newHediff, StatDef stat, float mod, float min = 0f, float max = 5f)
        {
            StatModifier statMod = new StatModifier();
            statMod.stat = stat;
            statMod.value = Math.Min(max, Math.Max(min, mod));
            newHediff.stages[0].statFactors.Add(statMod);
        }

        private static void AddCapacityMultipler(HediffDef newHediff, PawnCapacityDef cap, float mod, float min = 0f, float max = 5f)
        {
            PawnCapacityModifier capMod = new PawnCapacityModifier();
            capMod.capacity = cap;
            capMod.postFactor = Math.Min(max, Math.Max(min, mod));
            newHediff.stages[0].capMods.Add(capMod);
        }

        private static void AddFlatCapacityModifier(HediffDef newHediff, PawnCapacityDef cap, float mod, float min = 0f, float max = 5f)
        {
            PawnCapacityModifier capMod = new PawnCapacityModifier();
            capMod.capacity = cap;
            capMod.offset = Math.Min(max, Math.Max(min, mod));
            newHediff.stages[0].capMods.Add(capMod);
        }
    }
}
