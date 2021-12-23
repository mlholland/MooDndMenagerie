using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using AnimalBehaviours;


/* Painstakingly manual code that combines pseudo animalDefs, furniture defs, and sometimes a stuff def to produce
   a new animal def that represents a piece of furniture that's been animated to life.
   This needs to be done in code because animals aren't paramaterizeable by stuff categories. */
namespace MoodndBehaviorsAndEvents
{
    class MaterialToAnimalDefConverter
    {

        private static string BASE = "_Base";

        public static ThingDef MakeAnimalDef(ThingDef aniDef, ThingDef furnitureThingDef)
        {
            ThingDef newThing = new ThingDef();
            //Values set in basepawn def
            newThing.thingClass = typeof(Pawn); // this and the category value must be reset directly, since animated furniture defs
            newThing.category = ThingCategory.Pawn; // set these wierdly to avoid be processed like normal animals.

            newThing.selectable = aniDef.selectable;
            newThing.useHitPoints = aniDef.useHitPoints;
            newThing.hasTooltip = aniDef.hasTooltip;
            newThing.tickerType = aniDef.tickerType;
            newThing.altitudeLayer = aniDef.altitudeLayer;
            newThing.soundImpactDefault = furnitureThingDef.soundImpactDefault;

            newThing.inspectorTabs = new List<Type>();
            foreach (Type t in aniDef.inspectorTabs)
            {
                newThing.inspectorTabs.Add(t);
            }
            newThing.comps = new List<CompProperties>();
            if (aniDef.comps != null)
            {
                foreach (CompProperties origComp in aniDef.comps)
                {
                    newThing.comps.Add(origComp); // naive copy doesn't work due to subtyping, just add whole thing
                }
            }
            // This doesn't work for some reason
            // probably need to carefully examine how the VEF animal ranged attack framework works, and see if there are any
            // extra values it sets (probably related to widget creation).
            /*if (aniDef.Verbs != null && newThing.Verbs != null)
            {
                newThing.Verbs.Clear();
                foreach (VerbProperties verb in aniDef.Verbs)
                {
                    VerbProperties newVerb = new VerbProperties();
                    newVerb.verbClass = verb.verbClass;
                    newVerb.defaultProjectile = verb.defaultProjectile;
                    newVerb.warmupTime = verb.warmupTime;
                    newVerb.range = verb.range;
                    newVerb.burstShotCount = verb.burstShotCount;
                    newVerb.ticksBetweenBurstShots = verb.ticksBetweenBurstShots;
                    newVerb.soundCast = verb.soundCast;
                    newVerb.soundCastTail = verb.soundCastTail;
                    newVerb.muzzleFlashScale = verb.muzzleFlashScale;
                    newVerb.accuracyTouch = verb.accuracyTouch;
                    newVerb.accuracyShort = verb.accuracyShort;
                    newVerb.accuracyMedium = verb.accuracyMedium;
                    newVerb.accuracyLong = verb.accuracyLong;
                    newVerb.minRange = verb.minRange;
                    newVerb.label = verb.label;
                    newVerb.commonality = verb.commonality;
                    newThing.Verbs.Add(newVerb); 
                }
            }*/
            newThing.drawGUIOverlay = aniDef.drawGUIOverlay;

            newThing.race = new RaceProperties();
            newThing.race.nuzzleMtbHours = aniDef.race.nuzzleMtbHours;
            newThing.race.deathActionWorkerClass = aniDef.race.deathActionWorkerClass;
            newThing.race.thinkTreeMain = aniDef.race.thinkTreeMain;
            newThing.race.thinkTreeConstant = aniDef.race.thinkTreeConstant;
            newThing.race.hasGenders = aniDef.race.hasGenders;
            newThing.race.trainability = aniDef.race.trainability;
            // todo figure out how to set name generator?
            newThing.race.hediffGiverSets = new List<HediffGiverSetDef>();
            foreach (HediffGiverSetDef hgiv in aniDef.race.hediffGiverSets)
            {
                newThing.race.hediffGiverSets.Add(hgiv);
            }

            newThing.thingCategories = new List<ThingCategoryDef>();
            foreach (ThingCategoryDef tcat in aniDef.thingCategories)
            {
                newThing.thingCategories.Add(tcat);
            }

            newThing.recipes = new List<RecipeDef>();
            foreach (RecipeDef recipeDef in aniDef.recipes)
            {
                newThing.recipes.Add(recipeDef);
            }

            // values from BaseAnimatedFurniture

            newThing.defName = aniDef.defName.Substring(0, aniDef.defName.IndexOf(BASE));
            newThing.description = aniDef.description;
            newThing.label = aniDef.label;
            newThing.statBases = new List<StatModifier>();
            foreach (StatModifier origStatMod in aniDef.statBases)
            {
                StatModifier newMod = new StatModifier();
                newMod.stat = origStatMod.stat;
                newMod.value = origStatMod.value;
                // Overwrite values that exist in BaseAnimalDef but need to be overridden by furniture
                if (newMod.stat == StatDefOf.LeatherAmount)
                {
                    if (furnitureThingDef.costList != null && furnitureThingDef.costList.Count > 0)
                    {
                        newMod.value = furnitureThingDef.costList[0].count;
                    }
                    else
                    {
                        newMod.value = 50; // ensure this is always set to non-zero value
                    }
                }
                if (newMod.stat == StatDefOf.Mass)
                {
                    newMod.value = furnitureThingDef.statBases.GetStatFactorFromList(StatDefOf.Mass);
                }
                if (newMod.stat == StatDefOf.Flammability)
                {
                    newMod.value = furnitureThingDef.BaseFlammability;
                }
                newThing.statBases.Add(newMod);
            }

            newThing.race.wildBiomes = new List<AnimalBiomeRecord>(); // todo maybe iterate through values anyway
            newThing.race.petness = aniDef.race.petness;
            newThing.race.herdAnimal = aniDef.race.herdAnimal;
            newThing.race.predator = aniDef.race.predator;
            newThing.race.body = aniDef.race.body;
            newThing.race.baseBodySize = aniDef.race.baseBodySize;
            newThing.race.baseHealthScale = aniDef.race.baseHealthScale;
            newThing.race.baseHungerRate = aniDef.race.baseHungerRate;
            newThing.race.foodType = aniDef.race.foodType;
            newThing.race.wildness = aniDef.race.wildness;
            newThing.race.manhunterOnDamageChance = aniDef.race.manhunterOnDamageChance;
            if (furnitureThingDef.costList != null && furnitureThingDef.costList.Count > 0)
            {
                newThing.race.leatherDef = furnitureThingDef.costList[0].thingDef;
            } else
            {
                newThing.race.leatherDef = ThingDefOf.Steel;
            }
            
            newThing.race.lifeExpectancy = aniDef.race.lifeExpectancy;
            newThing.race.manhunterOnTameFailChance = aniDef.race.manhunterOnTameFailChance;
            newThing.race.soundMeleeHitPawn = aniDef.race.soundMeleeHitPawn;
            newThing.race.soundMeleeHitBuilding = aniDef.race.soundMeleeHitBuilding;
            newThing.race.soundMeleeMiss = aniDef.race.soundMeleeMiss;


            // values from actual animal def 
            // todo check and remeove duplicately set values
            newThing.race.nameCategory = aniDef.race.nameCategory;
            newThing.race.baseBodySize = aniDef.race.baseBodySize;
            newThing.race.baseHealthScale = aniDef.race.baseHealthScale;
            newThing.race.baseHungerRate = aniDef.race.baseHungerRate;
            newThing.race.body = aniDef.race.body;
            newThing.race.canReleaseToWild = aniDef.race.canReleaseToWild;
            newThing.race.hasGenders = aniDef.race.hasGenders;
            newThing.race.intelligence = aniDef.race.intelligence;
            newThing.race.ageGenerationCurve = aniDef.race.ageGenerationCurve;
            newThing.race.allowedOnCaravan = aniDef.race.allowedOnCaravan;
            newThing.race.animalType = aniDef.race.animalType;
            newThing.race.corpseDef = aniDef.race.corpseDef;
            newThing.race.herdAnimal = aniDef.race.herdAnimal;
            newThing.race.lifeExpectancy = aniDef.race.lifeExpectancy;
            newThing.race.lifeStageAges = aniDef.race.lifeStageAges;
            newThing.race.meatDef = newThing.race.leatherDef; 
            newThing.race.needsRest = aniDef.race.needsRest;
            newThing.race.soundMeleeDodge = aniDef.race.soundMeleeDodge;
            newThing.race.soundMeleeHitBuilding = aniDef.race.soundMeleeHitBuilding;
            newThing.race.soundMeleeHitPawn = aniDef.race.soundMeleeHitPawn;
            newThing.race.soundMeleeMiss = aniDef.race.soundMeleeMiss;
            newThing.race.thinkTreeMain = aniDef.race.thinkTreeMain;
            newThing.race.trainability = aniDef.race.trainability;
            newThing.race.wildBiomes = aniDef.race.wildBiomes;
            newThing.race.wildness = aniDef.race.wildness;
            newThing.race.willNeverEat = aniDef.race.willNeverEat;
            
            // todo check if set elsewhere
            newThing.tradeTags = aniDef.tradeTags;
            newThing.tools = aniDef.tools;

            return newThing;
        }


        public static ThingDef MakeAnimalDefWithStuff(ThingDef aniDef, ThingDef furnitureThingDef, ThingDef stuff)
        {
            // Start with non-stuff-based def, then modify stuff-parameterized values.
            ThingDef newThing = MakeAnimalDef(aniDef, furnitureThingDef);

            newThing.defName = String.Format("{0}_{1}", aniDef.defName.Substring(0, aniDef.defName.IndexOf(BASE)), stuff.defName);
            newThing.description = aniDef.description + "DND_MadeFromMaterialAddon".Translate(stuff.label);
            newThing.label = "DND_AnimatedCreatureLabel".Translate(aniDef.label, stuff.label);
            foreach (StatModifier statMod in newThing.statBases)
            {
                if (statMod.stat == StatDefOf.LeatherAmount)
                {
                    statMod.value = furnitureThingDef.costStuffCount * (stuff.smallVolume ? stuff.VolumePerUnit : 1);
                }
                if (statMod.stat == StatDefOf.Flammability)
                {
                    statMod.value *= stuff.BaseFlammability;
                }
                if (statMod.stat == StatDefOf.MarketValue)
                {
                    statMod.value += stuff.BaseMarketValue * furnitureThingDef.costStuffCount * (stuff.smallVolume ? stuff.VolumePerUnit : 1);
                }
            }
            newThing.race.leatherDef = stuff;
            newThing.race.meatDef = newThing.race.leatherDef;

            // Add stuff based buff
            CompProperties_InitialHediff newProp = new CompProperties_InitialHediff();
            newProp.hediffname = HediffDef.Named(String.Format("DND_Material_Hediff_{0}", stuff.defName)).defName; // Done this way to guarantee during load that hediff defs exist.
            newProp.hediffseverity = 1.0f;
            newProp.applyToAGivenBodypart = false;
            newProp.part = newThing.race.body.corePart.def;
            newProp.addRandomHediffs = false;
            newThing.comps.Add(newProp);
            // Todo add quality buff
            
            return newThing;
        }
    }
}


