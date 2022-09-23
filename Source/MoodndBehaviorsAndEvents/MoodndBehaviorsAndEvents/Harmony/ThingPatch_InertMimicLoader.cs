using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;

/* This patch intercepts the save/load behavior of ThingWithComps and identifies if the thing in question is an inert mimic. If so, it saves or loads the associated 
 * instance of the Comp_InertMimic class, which isn't normally loaded because it's not part of any normal defs.
 * When saving, inert mimics are ID'd by the presense of a mythic comp.
 * When loading, they're identified by a flag that's saved to the ThingWithComps itself.
 */
namespace MoodndBehaviorsAndEvents
{
    public class ThingPatch_InertMimicLoader
    {
        private static readonly string isMimicLookString = "MooDND_InertMimicComp";

        [HarmonyPatch(typeof(ThingWithComps), nameof(ThingWithComps.ExposeData))]
        static class ThingWithComps_InitializeComps_PostResolve_Patch
        {
            static void Postfix(ref ThingWithComps __instance, List<ThingComp> ___comps)
            {
                if (Scribe.mode == LoadSaveMode.Saving)
                {
                    Comp_InertMimic comp = __instance.TryGetComp<Comp_InertMimic>();
                    if (comp != null)
                    {
                        Debug.LogIfDebug("Saving a mimic item disguised as a {0}.", __instance.def.label);
                        bool isMimic = true;
                        Scribe_Values.Look(ref isMimic, isMimicLookString);
                    }
                }
                else
                {
                    bool isMimic = false;
                    Scribe_Values.Look(ref isMimic, isMimicLookString);
                    if (isMimic)
                    {
                        Debug.LogIfDebug("Found a mimic comp on a {0}.", __instance.def.label);
                        Comp_InertMimic newComp = Activator.CreateInstance<Comp_InertMimic>();
                        newComp.parent = __instance;
                        ___comps.Add(newComp);
                        newComp.Initialize(new CompProperties_InertMimic());
                        newComp.PostExposeData();
                    }
                }
            }
        }
    }
}