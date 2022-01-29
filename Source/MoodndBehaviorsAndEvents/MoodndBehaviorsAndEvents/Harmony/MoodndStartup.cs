using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HarmonyLib;
using RimWorld;
using Verse;

// Startup patches to create material hediffs for animated objects, modify existing defs, and maybe more in the future
namespace MoodndBehaviorsAndEvents
{
    class MoodndStartup : Mod
    {
        public MoodndStartup(ModContentPack content) : base(content)
        {
            //todo move harmony instance to mod file?
            new Harmony("rimworld.moodnd").PatchAll(); 
        }
    }

    [HarmonyPatch(typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PostResolve))]
    static class DefGenerator_GenerateImpliedDefs_PostResolve_Patch
    { 
        static void Postfix()
        {
            Log.Message("MooDnd Generation: Interrupting GenerateImpliedDefs to Create and modify defs");
            bool foundAny = false;
            //

            Log.Message("MooDnd Generation: Loading animation targets for scroll of animation");
            foreach (Def_AnimatableFurniture animatableFurniture in DefDatabase<Def_AnimatableFurniture>.AllDefs)
            {
                foundAny = true;
                FurnitureToAnimatedObjectConverter.Load(animatableFurniture); 
            }
            if (!foundAny)
            {
                Log.Error("MooDND: Could not find support def that lists valid animation targets for the scroll of animation. The scroll won't work now.");
            }

            Log.Message("MooDnd Generation: Generating material-based hediffs");
            StartupHelper.GenerateAnimatedAnimalDefs();
            Log.Message("MooDnd Generation: Modifying existing clothing defs to allow special material buffs");
            StartupHelper.ModifyExistingDefs();
            Log.Message("MooDnd Generation: Complete");
        }
    }

    static class StartupHelper
    {
        // should probably move this, among many other things into a separate utility mod if I keep making stuff.
        public static void ModifyExistingDefs()
        {
            foreach(ThingDef td in DefDatabase<ThingDef>.AllDefs)
            {
                if (td.IsApparel)
                { 
                    td.comps.Add(new CompProperties_BuffedItem());
                }
            }
        }

        // Adds new Defs to the game - namely all the stuff related to animated furnituire (besides tool cabinets)
        public static void GenerateAnimatedAnimalDefs()
        {
            // add material hediffs for all stuff defs 
            foreach (ThingDef td in DefDatabase<ThingDef>.AllDefs)
            {
                if (td.IsStuff)
                {
                    HediffDef matDef = MaterialToHediffConverter.MakeHediffFromMetal(td);
                    DefGenerator.AddImpliedDef<HediffDef>(matDef);
                }

            }
            /*
            // Create both thingDefs and pawnKindDefs for each Def_AnimatedFurniture.
            HashSet<String> newAnimalDefNames = new HashSet<String>();
            foreach (Def_AnimatedFurniture aniFurDef in DefDatabase<Def_AnimatedFurniture>.AllDefsListForReading)
            {
                ThingDef furnitureDef = aniFurDef.furnitureDef;
                if (furnitureDef == null)
                {
                    Log.Error(String.Format("Moodnd Generation: Animated furniture failed to find (mundane) furniture def for {0}", aniFurDef.defName));
                    continue;
                }
                ThingDef templateThingDef = ThingDef.Named(aniFurDef.baseDefName);
                if (templateThingDef == null)
                {
                    Log.Error(String.Format("Moodnd Generation: Animated furniture failed to find template thing def named {0} for {1} ", aniFurDef.baseDefName, aniFurDef.defName));
                    continue;
                }
                // Iterate over all valid stuff types for the furnitureDef if it's stuffable, and create a pair of defs for each.
                if (furnitureDef.stuffCategories != null && furnitureDef.stuffCategories.Count > 0)
                {
                    foreach (String stuffDefName in StuffCategoryToThingUtil.GetInstance().GetThingDefsOfStuffCategories(furnitureDef.stuffCategories))
                    {
                        ThingDef stuffDef = ThingDef.Named(stuffDefName);
                        ThingDef newAnimalDef = MaterialToAnimalDefConverter.MakeAnimalDefWithStuff(templateThingDef, furnitureDef, stuffDef);
                        newAnimalDef.ResolveReferences();
                        newAnimalDefNames.Add(newAnimalDef.defName);
                        FurnitureToAnimatedObjectConverter.AddStuffable(furnitureDef, stuffDef, newAnimalDef);
                        DefGenerator.AddImpliedDef<ThingDef>(newAnimalDef);
                        PawnKindDef newPawnKindDef = MaterialToPawnKindDefConverter.MakeStuffBasedDef(aniFurDef, newAnimalDef, furnitureDef, stuffDef);
                        DefGenerator.AddImpliedDef<PawnKindDef>(newPawnKindDef);
                        
                        ThingDef newCorpseDef = ThingDefGenerator_CorpseFromDef.ImpliedCorpseDefFromAnimalDef(newAnimalDef);
                        DefGenerator.AddImpliedDef<ThingDef>(newCorpseDef);
                        
                        newAnimalDef.ResolveReferences();
                        newPawnKindDef.ResolveReferences();
                        newCorpseDef.ResolveReferences();
                    }
                }
                else
                {
                    // the 'true' condition is no longer being executed as of v1.0
                    if (aniFurDef.makeProgrammaticDefs)
                    {
                        ThingDef newAnimalDef = MaterialToAnimalDefConverter.MakeAnimalDef(templateThingDef, furnitureDef);
                        newAnimalDefNames.Add(newAnimalDef.defName);
                        FurnitureToAnimatedObjectConverter.Add(furnitureDef, newAnimalDef);
                        DefGenerator.AddImpliedDef<PawnKindDef>(MaterialToPawnKindDefConverter.MakeDef(aniFurDef, newAnimalDef, furnitureDef));
                        DefGenerator.AddImpliedDef<ThingDef>(ThingDefGenerator_CorpseFromDef.ImpliedCorpseDefFromAnimalDef(newAnimalDef));
                    }
                    else
                    {
                        // if we aren't making the defs in code, then make the following assumptions:
                        // - both the ThingDef and PawnKindDef for the animal exist, and both have a def name of aniFurDef.baseDefName
                        // - The defs are fully formed, and don't need programatic modification, and there aren't multiple stuff-based variants
                        // - All we need to do here is add the defs to the converter so the scroll knows how to link valid furniture targets to the corresponding animals
                        ThingDef xmlAnimalDef = ThingDef.Named(aniFurDef.baseDefName);
                        if (xmlAnimalDef == null)
                        {
                            Log.Error(String.Format("MooDnD: animated furniture generation failed due to missing ThingDef for non-code-generated thingDef for {0}", aniFurDef.defName));
                        }
                        PawnKindDef xmlKindDef = PawnKindDef.Named(aniFurDef.baseDefName);
                        if (xmlKindDef == null)
                        {
                            Log.Error(String.Format("MooDnD: animated furniture generation failed due to missing ThingDef for non-code-generated pawnKinddef for {0}", aniFurDef.defName));
                        }
                        FurnitureToAnimatedObjectConverter.Add(furnitureDef, xmlAnimalDef);
                    }
                }
            }*/
        }
    }
}
