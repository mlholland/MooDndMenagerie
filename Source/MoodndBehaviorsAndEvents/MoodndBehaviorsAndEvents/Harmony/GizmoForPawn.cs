using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using MVCF.Utilities;

/* This patch prevents the MVCF GetMainAttackGizmoForPawn function from trying to access humanoind-only aspects of animals with
 * multiple ranged attacks. TL;DR the code that calls the original function only does so if there are multiple ranged verbs available,
 * so virtually all other animals with ranged attacks don't need to worry about this problem. Spectators - and if you cheat to tame them, Beholders -
 * have multiple ranged attacks, so the MVCF code was trying to call original function on them, which threw and NRE, then destroyed all gizmos
 * for the pawn in question.
 * 
 * This patch is effectively a copy of the original function minus some humanoid-only data access, 
 * but it only runs if the pawn is identified as one of my animals.
 * 
 * Original function: https://github.com/AndroidQuazar/VanillaExpandedFramework/blob/aec5a1dda0b6fcda38b5cab7ec15e2525f8f451b/Source/MVCF/Utilities/PawnVerbGizmoUtility.cs#L111
 */
namespace MoodndBehaviorsAndEvents
{

    [HarmonyPatch(typeof(PawnVerbGizmoUtility), nameof(PawnVerbGizmoUtility.GetMainAttackGizmoForPawn))]
    static class PawnVerbGizmoUtility_GetMainAttackGizmoForPawn
    {
        static bool Prefix(ref Gizmo __result, Pawn pawn)
        { 
            if (pawn.def.defName.StartsWith("DND_"))
            {
                var verbs = pawn.Manager().ManagedVerbs;
                var gizmo = new Command_Action
                {
                    defaultDesc = "Attack",
                    hotKey = KeyBindingDefOf.Misc1,
                    icon = TexCommand.SquadAttack,
                    action = () =>
                    {
                        Find.Targeter.BeginTargeting(TargetingParameters.ForAttackAny(), target =>
                        {
                            var manager = pawn.Manager();
                            manager.CurrentVerb = null;
                            var verb = pawn.BestVerbForTarget(target, verbs.Where(v => v.Enabled && !v.Verb.IsMeleeAttack),
                                manager);
                            verb.OrderForceTarget(target);
                        }, pawn, null, TexCommand.Attack);
                    }
                };

                if (pawn.Faction != Faction.OfPlayer)
                {
                    gizmo.Disable("CannotOrderNonControlled".Translate());
                }
                __result = gizmo;
                return false;
            }
            return true;
        }
    }
}
