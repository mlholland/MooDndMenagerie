using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

/* This comp marks a holding animal as potentially recolorable.
 * If the newColor value is non-null the graphics patch at MoodndBehaviorsAndEvents.PawnGraphicSet_ResolveAllGraphics
 * will use that color to produce a new shader for the animal.
 */
namespace MoodndBehaviorsAndEvents
{
    public class Comp_RecolorableAnimal : ThingComp
    {
        public Color newColor;
        public bool colorSet = false;
    }
}
