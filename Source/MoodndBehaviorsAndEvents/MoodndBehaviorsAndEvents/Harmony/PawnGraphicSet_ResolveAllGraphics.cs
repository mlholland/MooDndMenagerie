using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HarmonyLib;
using Verse; 

/* Allows non-human animals with the Comp_StuffableAnimal comp to be recolored. Used in-house with the HediffComp_ChangeColorOnAdd hediff comp.
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
            Comp_StuffableAnimal stuffableComp = __instance.pawn.TryGetComp<Comp_StuffableAnimal>();
            if (stuffableComp != null && stuffableComp.stuff != null && stuffableComp.stuff.stuffProps != null)
            {
                // change the graphic to use to new color
                Type graphicClass = __instance.nakedGraphic.GetType();
                if (graphicClass == typeof(Graphic_Single))
                {
                    __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Single>(__instance.nakedGraphic.path, ShaderDatabase.CutoutSkinColorOverride, __instance.nakedGraphic.drawSize, stuffableComp.stuff.stuffProps.color);
                }
                if (graphicClass == typeof(Graphic_Multi))
                {
                    __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(__instance.nakedGraphic.path, ShaderDatabase.CutoutSkinColorOverride, __instance.nakedGraphic.drawSize, stuffableComp.stuff.stuffProps.color);
                }
            }
        }
          
    }
}
