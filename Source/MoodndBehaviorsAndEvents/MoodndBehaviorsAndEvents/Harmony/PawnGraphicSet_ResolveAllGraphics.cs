using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HarmonyLib;
using RimWorld;
using Verse;
using MVCF.Utilities;
using MVCF;


/* Allows non-human animals with the Comp_RecolorableAnimal comp to be recolored. Used in-house with the HediffComp_ChangeColorOnAdd hediff comp.
 */
namespace MoodndBehaviorsAndEvents
{ 
    [HarmonyPatch(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics))]
    static class PawnGraphicSet_ResolveAllGraphics
    {
        static void Postfix(PawnGraphicSet __instance)
        { 
            // only works on non-humanoid animals
            if (!__instance.pawn.NonHumanlikeOrWildMan())
            {
                return;
            }
            // check for the relevant comp and see if it has had its values set
            Comp_RecolorableAnimal colorComp = __instance.pawn.TryGetComp<Comp_RecolorableAnimal>();
            if (colorComp != null && colorComp.colorSet)
            {
                // change the graphic to use to new color
                Type graphicClass = __instance.nakedGraphic.GetType();
                if (graphicClass == typeof(Graphic_Single))
                {
                    __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Single>(__instance.nakedGraphic.path, ShaderDatabase.CutoutSkinColorOverride, __instance.nakedGraphic.drawSize, colorComp.newColor);
                }
                if (graphicClass == typeof(Graphic_Multi))
                {
                    __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(__instance.nakedGraphic.path, ShaderDatabase.CutoutSkinColorOverride, __instance.nakedGraphic.drawSize, colorComp.newColor);
                }
            }
        }
          
    }
}
