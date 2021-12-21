using HarmonyLib;
using RimWorld;
using Verse;


/* Original: https://github.com/juanosarg/AlphaAnimals/blob/master/1.3/Source/AlphaBehavioursAndEvents/AlphaBehavioursAndEvents/Harmony/SappersUtility_IsGoodSapper.cs */
namespace AlphaBehavioursAndEvents
{
    /*This Harmony Postfix allows appropriate monsters to use the sapper AI
    */
    [HarmonyPatch(typeof(SappersUtility))]
    [HarmonyPatch("IsGoodSapper")]
    public static class Moodnd_SappersUtility_IsGoodSapper_Patch
    {
        [HarmonyPostfix]
        public static void DemolisherIsAGoodSapper(Pawn p, ref bool __result)

        {
           if (p.def.defName== "DND_Anihilator" || p.def.defName == "DND_Beholder") {
                __result = true;
            } 
        }
    }
}