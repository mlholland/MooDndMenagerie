using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Linq;

/* This patch intercepts the save/load behavior of ThingWithComps and identifies if the thing in question is an inert mimic. If so, it saves or loads the associated 
 * instance of the Comp_InertMimic class, which isn't normally loaded because it's not part of any normal defs.
 * When saving, inert mimics are ID'd by the presense of a mythic comp.
 * When loading, they're identified by a flag that's saved to the ThingWithComps itself.
 * DIsabled until I work on it post 1.4 update
 */
namespace MoodndBehaviorsAndEvents
{
    public class MimicCasketPatch
    {
        private static AccessTools.FieldRef<object, List<ThingComp>> compsField = AccessTools.FieldRefAccess<List<ThingComp>>(typeof(ThingWithComps), "comps");
        private static AccessTools.FieldRef<object, bool> knownField = AccessTools.FieldRefAccess<bool>(typeof(Building_AncientCryptosleepCasket), "contentsKnown");

        private static readonly string isMimicLookString = "MooDND_InertMimicComp";

        private static readonly float MIMIC_CASKET_CHANCE = 0.5f;
        private static readonly float OOPS_ALL_MIMICS_CHANCE = 1f;

        //[HarmonyPatch(typeof(Building_AncientCryptosleepCasket), "EjectContents")]

        static class SignalAction_OpenCasket_Complete_Prefix_Patch
        {
            static bool Prefix(ref Building_AncientCryptosleepCasket __instance)
            {
                Debug.LogIfDebug("castket group opening, testing whether or not to replace caskets with mimics");
                // TODO replace with 'oops all mimics' check.
                bool contentsKnown = knownField.Invoke(__instance);
                Comp_InertMimic mimicComp = __instance.TryGetComp<Comp_InertMimic>();
                if (contentsKnown)
                {
                    if (mimicComp != null)
                    {
                        // Contents are known (which means other caskets are already being managed), and this
                        // one is a mimic. Replace it with a mimic and cancel normal opening code.
                        Debug.LogIfDebug("Replacing opening cryptosleep casket with mimic.");
                        PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named("Thrumbo"), Find.FactionManager.FirstFactionOfDef(FactionDef.Named("DND_DungeonMonsterFaction")), PawnGenerationContext.NonPlayer, -1, false, false, false, false, false, 0f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, false);
                        Pawn pawn = PawnGenerator.GeneratePawn(request);
                        GenSpawn.Spawn(pawn, __instance.PositionHeld, __instance.MapHeld, WipeMode.Vanish);
                        __instance.DeSpawn();
                        return false;
                    } else
                    {
                        // contents are known and this casket doesn't have a mimic comp
                        // Use normal logic
                        return true;
                    }
                }
                else
                {
                    List<Building_AncientCryptosleepCasket> caskets = UnopenedCasketsInGroup(__instance).ToList();
                    // This is a new casket group being opened - roll for mayhem
                    if (Rand.Chance(OOPS_ALL_MIMICS_CHANCE))
                    {
                        int count = 0;
                        Debug.LogIfDebug("OOPS ALL MIMICS.");
                        foreach (Building_AncientCryptosleepCasket casket in caskets)
                        {
                            count++;
                            AddMimicCompToCasket(casket);
                            knownField.Invoke(casket) = true;
                            casket.EjectContents();
                        }
                        Debug.LogIfDebug("count "+  count);
                    } else
                    {
                        foreach(Building_AncientCryptosleepCasket casket in caskets)
                        {
                            if(Rand.Chance(MIMIC_CASKET_CHANCE))
                            {
                                Debug.LogIfDebug("Adding mimic comp to a crypto casket.");
                                AddMimicCompToCasket(casket);
                            }
                            knownField.Invoke(casket) = true;
                            casket.EjectContents();
                        }
                    }
                    return false;
                }
            }
        }

        public static void AddMimicCompToCasket(Building_AncientCryptosleepCasket thing)
        {
            List<ThingComp> comps = compsField.Invoke(thing);
            Comp_InertMimic mimicComp = (Comp_InertMimic)Activator.CreateInstance(typeof(Comp_InertMimic));
            mimicComp.parent = thing;
            comps.Add(mimicComp);
            mimicComp.Initialize(new CompProperties_InertMimic());
        }

        // copied from the vanilla casket function of the same name because I"m too lazy to figure out
        // function fieldRefs
        private static IEnumerable<Building_AncientCryptosleepCasket> UnopenedCasketsInGroup(Building_AncientCryptosleepCasket casket)
        {
            Debug.LogIfDebug("id: " + casket.groupID);
            if (casket.groupID != -1)
            {
                foreach (Thing thing in casket.Map.listerThings.ThingsOfDef(ThingDefOf.AncientCryptosleepCasket))
                {
                    Building_AncientCryptosleepCasket building_AncientCryptosleepCasket = thing as Building_AncientCryptosleepCasket;
                    Debug.LogIfDebug("other id: " + building_AncientCryptosleepCasket.groupID);
                    if (building_AncientCryptosleepCasket.groupID == casket.groupID && !knownField.Invoke(casket))
                    {
                        yield return building_AncientCryptosleepCasket;
                    }
                }
                List<Thing>.Enumerator enumerator = default(List<Thing>.Enumerator);
            }
            yield break;
            yield break;
            yield return casket;
        }
    }
}