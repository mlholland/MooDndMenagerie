using HarmonyLib;
using RimWorld;
using Verse;

/* Identifies when something is made from a stuff type that adds a comp to the resulting thing, and if so, add said comp */
namespace MoodndBehaviorsAndEvents
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear))]
    public static class Pawn_ApparelTracker_Wear
    {
        [HarmonyPostfix]
        public static void PostFix(Pawn_ApparelTracker __instance, Apparel newApparel)
        {
            if (__instance == null || newApparel == null) return;

            Pawn pawn = __instance.pawn;
            if (pawn == null) return; 
            foreach (HediffDef hediffDef in StuffedApparelGrantsHediffDef.GetAllHediffsToApply(newApparel))
            {
                if (hediffDef == null)
                {
                    //TODO when logging done - complain since this shouldn't happen
                    continue;
                } 
                if (pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef, false) == null)
                {
                    HediffComp_RemoveIfApparelDropped hediffComp_RemoveIfApparelDropped = pawn.health.AddHediff(hediffDef).TryGetComp<HediffComp_RemoveIfApparelDropped>();
                    if (hediffComp_RemoveIfApparelDropped != null)
                    {
                        hediffComp_RemoveIfApparelDropped.wornApparel = newApparel;
                    } 
                }
            } 
        }
    }
}