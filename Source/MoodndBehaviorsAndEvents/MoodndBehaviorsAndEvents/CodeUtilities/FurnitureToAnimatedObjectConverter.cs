using System;
using System.Collections.Generic; 
using Verse; 

// Utility functions to keep track of furniture and their assocaited animated object defs
namespace MoodndBehaviorsAndEvents
{
    class FurnitureToAnimatedObjectConverter
    {
        private FurnitureToAnimatedObjectConverter() { }

        private static Dictionary<ThingDef, PawnKindDef> s_furnitureToAnimals = new Dictionary<ThingDef, PawnKindDef>(); 

        public static void Load(Def_AnimatableFurniture mappings)
        {
            if(mappings.targets == null || mappings.results == null || mappings.targets.Count != mappings.results.Count)
            {
                Log.Error(String.Format("MooDND FurnitureToAnimatedObjectConverter Load: Failed to load inputted Def_AnimatableFurniture - it was improperly formatted somehow. Skipping the def named {0}", mappings.defName));
                return;
            }
            for(int i = 0; i < mappings.targets.Count; i++)
            {
                s_furnitureToAnimals.Add(mappings.targets[i], mappings.results[i]);
            } 
        }

        public static PawnKindDef GetResultingPawnKindDef(ThingDef furniture)
        {
            if (s_furnitureToAnimals.ContainsKey(furniture))
            {
                return s_furnitureToAnimals[furniture];
            }
            Log.Error(String.Format("Tried to get animated animal associated with funiture named '{0}', but found nothing.", furniture.defName));
            return null;
        }  

        public static bool IsValidTarget(ThingDef target)
        {
            return s_furnitureToAnimals.ContainsKey(target);
        }
    }
}
