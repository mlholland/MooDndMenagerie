using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

// A modified variant of the ComHediffEffector from the VEF that parameterizes psy sensitivity, among other things


 // A collection of static functions and helper classes designed to take in a material, and produce a hediff that represents that
 // material's attributes when applied to a creature. Used for Animated Object creatures.
namespace MoodndBehaviorsAndEvents
{
    public class MaterialToHediffConverter
    {

        public static HediffDef MakeHediffFromMetal(ThingDef thing /*Hedriff to edit, material to apply, material multipliers, maybe list of attributes to add to hedriff*/)
        {
            return MakeHediffFromThing(new HediffDef(), thing);

        }

        private static HediffDef MakeHediffFromThing(HediffDef newHediff, ThingDef thing)
        {
            newHediff.defaultLabelColor = thing.stuffProps.color;
            newHediff.generated = true;
            newHediff.hediffClass = typeof(HediffWithComps);
            newHediff.scenarioCanAdd = false; 
            newHediff.initialSeverity = 1; 
            newHediff.maxSeverity = 1;
            newHediff.comps = new List<HediffCompProperties>();
            newHediff.stages = new List<HediffStage>();
            

            newHediff.label = String.Format("Living Material: {0}", thing.label);
            newHediff.defName = String.Format("DND_Material_Hediff_{0}", thing.defName);
            newHediff.description = String.Format("This creature is made of {0}, which has various effects on its physiology.", thing.label);
            

            HediffStage hediffStage = new HediffStage();
            hediffStage.statOffsets = new List<StatModifier>();
            hediffStage.statFactors = new List<StatModifier>();
            hediffStage.capMods = new List<PawnCapacityModifier>();
            newHediff.stages.Add(hediffStage);
            ApplyStuffTypeMods(newHediff, thing);
            
            return newHediff;
        }

        private static void ApplyStuffTypeMods(HediffDef newHediff, ThingDef stuff)
        {
            if (stuff.stuffProps != null && stuff.stuffProps.categories != null && stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Stony))
            {
                AddStoneMods(newHediff, stuff);
            } else if (stuff.stuffProps != null && stuff.stuffProps.categories != null && stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Woody))
            {
                AddWoodyMods(newHediff, stuff);
            }
            else
            {
                AddMetalMods(newHediff, stuff);
            }
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
        /*
        private static float ConvertMaterialValToHediffVal(float matVal, float multiplier, float flatMod, float min, float max)
        {
            return Math.Min(max, Math.Max(min, matVal * multiplier + flatMod));
        }


        private interface IModHediffStages
        {
            void ModifyHediffStage(float inputVal, HediffStage hediffDef);
        }

        // A stat offset is a flat change to an amount, with
        private class StatOffsetModder : IModHediffStages
        {
            private float multiplier, flatMod, min, max;
            private StatDef statDef;

            public StatOffsetModder(StatDef statDef, float multiplier, float flatMod, float min, float max)
            {
                this.statDef = statDef;
                this.multiplier = multiplier;
                this.flatMod = flatMod;
                this.min = min;
                this.max = max;
            }

            public void ModifyHediffStage(float inputVal, HediffStage stage)
            {
                StatModifier statMod = new StatModifier();
                statMod.stat = statDef;
                statMod.value = Math.Min(max, Math.Max(min, inputVal * multiplier + flatMod));
                stage.statOffsets.Add(statMod);
            }
        }

        // A factor is a % change to an amount
        private class StatFactorModder : IModHediffStages
        {
            private float multiplier, flatMod, min, max;
            private StatDef statDef;

            public StatFactorModder(StatDef statDef, float multiplier, float flatMod, float min, float max)
            {
                this.statDef = statDef;
                this.multiplier = multiplier;
                this.flatMod = flatMod;
                this.min = min;
                this.max = max;
            }

            public void ModifyHediffStage(float inputVal, HediffStage stage)
            {
                StatModifier statMod = new StatModifier();
                statMod.stat = statDef;
                statMod.value = Math.Min(max, Math.Max(min, inputVal * multiplier + flatMod));
                stage.statFactors.Add(statMod);
            }
        }

        private class PawnCapModder : IModHediffStages
        {
            private float multiplier, flatMod, min, max;
            private PawnCapacityDef capDef;

            public PawnCapModder(PawnCapacityDef capDef, float multiplier, float flatMod, float min, float max)
            {
                this.capDef = capDef;
                this.multiplier = multiplier;
                this.flatMod = flatMod;
                this.min = min;
                this.max = max;
            }

            public void ModifyHediffStage(float inputVal, HediffStage stage)
            {
                float newVal = Math.Min(max, Math.Max(min, inputVal * multiplier + flatMod));
                PawnCapacityModifier mod = new PawnCapacityModifier();
                mod.offset = newVal;
                mod.capacity = capDef;
                stage.capMods.Add(mod);
            }
        }

        private interface IGetAStat
        {
            float GetStat(ThingDef thing);
        }

        private class GetStatFromFactor : IGetAStat
        {

            private float defVal;
            private StatDef statDef;

            public GetStatFromFactor(StatDef statDef, float defVal)
            {
                this.statDef = statDef;
                this.defVal = defVal;
            }

            public float GetStat(ThingDef thing)
            {
                float newVal = thing.stuffProps.statFactors.GetStatValueFromList(StatDefOf.WorkToBuild, defVal);
                return thing.stuffProps.statFactors.GetStatValueFromList(StatDefOf.WorkToBuild, defVal);
            }
        }

        private class GetStatFromOffset : IGetAStat
        {

            private float defVal;
            private StatDef statDef;

            public GetStatFromOffset(StatDef statDef, float defVal)
            {
                this.statDef = statDef;
                this.defVal = defVal;
            }

            public float GetStat(ThingDef thing)
            {
                return thing.stuffProps.statOffsets.GetStatValueFromList(StatDefOf.WorkToBuild, defVal);
            }
        }

        private class StuffStatToHediffStageChanger
        {
            private IGetAStat statGetter;
            private IModHediffStages stageModder;

            public StuffStatToHediffStageChanger(IGetAStat statGetter, IModHediffStages stageModder)
            {
                this.statGetter = statGetter;
                this.stageModder = stageModder;
            }

            public void ModHediffStageGivenStuff(ThingDef stuffDef, HediffStage stage)
            {
                float newVal = statGetter.GetStat(stuffDef);
                stageModder.ModifyHediffStage(statGetter.GetStat(stuffDef), stage);
            }
        }*/
    }
}
