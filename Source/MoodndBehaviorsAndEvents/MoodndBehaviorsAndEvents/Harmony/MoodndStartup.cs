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
            Log.Message("MooDnd Generation: Creating and modifying defs");
            Log.Message("MooDnd Generation: Generating Material-based animal defs and material hediffs");
            StartupHelper.GenerateAnimatedAnimalDefs();
            Log.Message("MooDnd Generation: Modifying existing clothing defs to allow special material buffs");
            StartupHelper.ModifyExistingDefs();
            Log.Message("MooDnd Generation: Complete");
        }
    }

    static class StartupHelper
    {
        public static void ModifyExistingDefs()
        {
            foreach(ThingDef td in DefDatabase<ThingDef>.AllDefs)
            {
                if (td.IsApparel)
                {
                    //Log.Message(String.Format("MooDnd Generation: Adding CompProperties_BuffedItem to apparel {0}", td.defName));
                    td.comps.Add(new CompProperties_BuffedItem());
                }
            }
        }


        // Adds new Defs to the game - namely all the stuff related to animated-furnituire (besides tool cabinets)
        public static void GenerateAnimatedAnimalDefs()
        {
            // add material hediffs for all stuff defs
            //Log.Message("Moodnd Generation: Making Animated Animals Material Buffs...");
            foreach (ThingDef td in DefDatabase<ThingDef>.AllDefs)
            {
                if (td.IsStuff)
                {
                    HediffDef matDef = MaterialToHediffConverter.MakeHediffFromMetal(td);
                    DefGenerator.AddImpliedDef<HediffDef>(matDef);
                }

            }
            // add animal and animal kind defs for all animatedObjectDefs 
            //Log.Message("Moodnd Generation: Making Animated Animals Thing and Kind Defs...");
            // used for corpse creation later
            HashSet<String> newAnimalDefNames = new HashSet<String>();
            foreach (Def_AnimatedFurniture aniFurDef in DefDatabase<Def_AnimatedFurniture>.AllDefsListForReading)
            {
                //Log.Message(String.Format("generating {0}", aniFurDef.defName));

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
                if (furnitureDef.stuffCategories != null && furnitureDef.stuffCategories.Count > 0)
                {
                    foreach (String stuffDefName in StuffCategoryToThingUtil.GetInstance().GetThingDefsOfStuffCategories(furnitureDef.stuffCategories))
                    {
                        //Log.Message(String.Format("generating for stuff {0}", stuffDefName));
                        ThingDef stuffDef = ThingDef.Named(stuffDefName);
                        ThingDef newAnimalDef = MaterialToAnimalDefConverter.MakeAnimalDefWithStuff(templateThingDef, furnitureDef, stuffDef);
                        newAnimalDef.ResolveReferences();
                        newAnimalDefNames.Add(newAnimalDef.defName);
                        FurnitureToAnimatedObjectConverter.AddStuffable(furnitureDef, stuffDef, newAnimalDef);
                        DefGenerator.AddImpliedDef<ThingDef>(newAnimalDef); 
                        DefGenerator.AddImpliedDef<PawnKindDef>(MaterialToPawnKindDefConverter.MakeStuffBasedDef(aniFurDef, newAnimalDef, furnitureDef, stuffDef));
                        DefGenerator.AddImpliedDef<ThingDef>(ThingDefGenerator_CorpseFromDef.ImpliedCorpseDefFromAnimalDef(newAnimalDef)); 
                    }
                }
                else
                {
                    if (aniFurDef.makeProgrammaticDefs)
                    {
                        ThingDef newAnimalDef = MaterialToAnimalDefConverter.MakeAnimalDef(templateThingDef, furnitureDef);
                        //DefGenerator.AddImpliedDef<ThingDef>(newAnimalDef);
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
            }
            
            /*// Add corpses for everything
            foreach (ThingDef corpse in ThingDefGenerator_Corpses.ImpliedCorpseDefs())
            {
                // Todo is there a better way to do this check? How can I access the 'innerPawn' value I saw somewhere else a while ago?
                if (newAnimalDefNames.Contains(corpse.defName.Substring(7))) // Remove "Corpse_" from beginning of def name to get original animal def name
                {
                    //corpse.ResolveReferences();
                    //DefGenerator.AddImpliedDef<ThingDef>(corpse);
                } else if (corpse.defName.Contains("DND_"))
                {
                    //corpse.ResolveReferences();
                }
            }*/
            //DirectXmlCrossRefLoader.ResolveAllWantedCrossReferences(FailMode.Silent);
            /*foreach (PawnColumnDef column in PawnColumnDefgenerator.ImpliedPawnColumnDefs())
            {
                if (newAnimalDefNames.Contains(column.defName.Substring(10))) // Remove "Trainable_" from beginning of def name to get original animal def name
                {
                    DefGenerator.AddImpliedDef<PawnColumnDef>(column);
                }
            }*/
        }
    }


}
