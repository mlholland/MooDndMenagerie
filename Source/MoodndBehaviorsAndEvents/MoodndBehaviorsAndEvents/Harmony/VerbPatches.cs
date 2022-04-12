using System.Collections.Generic; 
using HarmonyLib;
using RimWorld;
using Verse;
using MVCF.Utilities;
using MVCF;

// Other Harmony patches
namespace MoodndBehaviorsAndEvents
{
    // The Normal logic for animal ranged attacks from VEF doesn't work for some of my animals for 2 reasons:
    // - The verb-choosing logic only looks at expected damage when picking a verb, so attacks that only apply effects instead of damage are never selected
    // - If multiple verbs have the same weight, the first in the list is always selected.
    // The patch below is more or less a copy of the code found [here](https://github.com/AndroidQuazar/VanillaExpandedFramework/blob/master/Source/MVCF/Utilities/PawnVerbUtility.cs#L47)
    // but with the following changes:
    // - It only activates if the pawn in question is from my mod.
    // - It uses custom logic for determining verb scores that's much more lenient, and generally produces the same value for most viable verbs
    // - It selects randomly among viable verbs if multiple verbs have the same best sore, 
    // TODO: figure out if it's possible to make verb selection be weighted by commonality.
    //[HarmonyPatch(typeof(PawnVerbUtility), nameof(PawnVerbUtility.BestVerbForTarget))]
    static class PawnVerbUtility_BestVerbForTargets_Prefix_Patch
    {
        static bool Prefix(ref Verb __result, Pawn p, LocalTargetInfo target, IEnumerable<ManagedVerb> verbs, VerbManager man = null)
        { 
            if (p != null && p.ContentSource != null && p.ContentSource.PackageId == "mooloh.dndmenagerie") // only intercept animals in this mod
            { 
                var debug = man?.debugOpts != null && man.debugOpts.ScoreLogging;
                if (!target.IsValid || (p.Map != null && !target.Cell.InBounds(p.Map))) // not sure why the last condition is red according to VS; it compiles just fine.
                {
                    if (debug)
                        Log.Error("MooDnd BestVerbForTarget patch: (Current job is " + p.CurJob + " with verb " + p.CurJob?.verbToUse + " and target " +
                                  p.CurJob?.targetA + ")");
                    return false;
                }
                
                HashSet<Verb> bestVerbs = new HashSet<Verb>();
                float bestScore = 0;
                foreach (var verb in verbs)
                {
                    if (verb.Verb is IVerbScore verbScore && verbScore.ForceUse(p, target))
                    {
                        __result = verb.Verb;
                        return false;
                    } 
                    var score = VerbScore(p, verb.Verb, target, debug);
                    if (score < bestScore) continue;
                    else if (score == bestScore)
                    { 
                        bestVerbs.Add(verb.Verb);
                    }
                    else
                    {
                        bestScore = score;
                        bestVerbs.Clear();
                        bestVerbs.Add(verb.Verb);
                    }
                }
                Verb randomVerbAmongBest = bestVerbs.RandomElement();
                if (debug) Log.Message("MooDnd BestVerbForTarget patch returning " + randomVerbAmongBest);
                __result = randomVerbAmongBest;
                return false;
            }
            return true;
        }


        private static float VerbScore(Pawn p, Verb verb, LocalTargetInfo target, bool debug = false)
        {
            if (debug) Log.Message("MooDnd VerbScore Patch: Getting score of " + verb + " with target " + target);
            if (verb is IVerbScore score) return score.GetScore(p, target);
            var accuracy = 0f;
            if (p.Map != null)
                accuracy = ShotReport.HitReportFor(p, verb, target).TotalEstimatedHitChance;
            else if (verb.TryFindShootLineFromTo(p.Position, target, out var line))
                accuracy = verb.verbProps.GetHitChanceFactor(verb.EquipmentSource,
                    p.Position.DistanceTo(target.Cell));

            var damage = accuracy * verb.verbProps.burstShotCount * GetDamage(verb);
            var timeSpent = verb.verbProps.AdjustedCooldownTicks(verb, p) + verb.verbProps.warmupTime.SecondsToTicks();
            if (debug)
            {
                Log.Message("Accuracy: " + accuracy);
                Log.Message("Damage: " + damage);
                Log.Message("timeSpent: " + timeSpent);
                Log.Message("Original score of " + verb + " on target " + target + " is " + damage / timeSpent);
                Log.Message("Score Overwritten to 1 due to Mooloh's DnD patch");
            }

            return 1;
        }

        private static int GetDamage(Verb verb)
        {
            switch (verb)
            {
                case Verb_LaunchProjectile launch:
                    return launch.Projectile.projectile.GetDamageAmount(1f);
                case Verb_Bombardment _:
                case Verb_PowerBeam _:
                case Verb_MechCluster _:
                    return int.MaxValue;
                case Verb_CastAbility cast:
                    return cast.ability.EffectComps.Count * 100;
                default:
                    return 1;
            }
        }
    }
}
