using HarmonyLib;
using RimWorld;
using Verse;

/* For some reason, animals won't fight properly in a normal raid strategy without having the return value of PawnUtility.IsFighting be forced to true for that pawn.
 * This simple patch just sets that value for all animals from the dungeon monsters faction.  */
namespace MoodndBehaviorsAndEvents
{

    [HarmonyPatch(typeof(PawnUtility))]
    [HarmonyPatch("IsFighting")] 
    public static class DND_PawnUtility_IsFighting_Patch
    {
        
        [HarmonyPostfix]
        public static void DisableMonsters(Pawn pawn, ref bool __result)

        {
            if (pawn != null && pawn.Faction.Equals(Find.FactionManager.FirstFactionOfDef(FactionDef.Named("DND_DungeonMonsterFaction"))) && pawn.CurJob != null) {
                __result = true;
            }
        }
    }
}