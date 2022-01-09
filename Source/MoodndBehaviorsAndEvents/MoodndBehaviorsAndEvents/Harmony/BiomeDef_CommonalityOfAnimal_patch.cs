using HarmonyLib;
using RimWorld;
using Verse; 


/*Based on the the code to scale a mod's animals' spawn frequency in alpha animals:  https://github.com/juanosarg/AlphaAnimals/blob/master/1.3/Source/AlphaBehavioursAndEvents/AlphaBehavioursAndEvents/Harmony/BiomeDef_CommonalityOfAnimal_patch.cs */
namespace MoodndBehaviorsAndEvents
{
     /*This Harmony Postfix multiplies commonality of animals in the biome */
    [HarmonyPatch(typeof(BiomeDef))]
    [HarmonyPatch("CommonalityOfAnimal")]
    public static class MooDND_Animals_BiomeDef_CommonalityOfAnimal_Patch
    {
        [HarmonyPostfix]
        public static void MultiplyDndAnimalCommonality(PawnKindDef animalDef, ref float __result)
        {
            if (animalDef.defName.StartsWith("DND_"))
            {
                float TotalMultiplier = MoodndManagerie_Mod.settings.moodndAnimalSpawnMultiplier; // original Alpha Animals code multiplied this value by 0.5f. Why? Was that just an ancient bug?
                __result *= TotalMultiplier; 
            }
        }
    }
}
