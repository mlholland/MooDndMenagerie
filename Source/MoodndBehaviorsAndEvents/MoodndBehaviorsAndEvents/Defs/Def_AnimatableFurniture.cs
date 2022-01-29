using System.Collections.Generic; 
using Verse; 

/* This def defines a list of pairs of furniture defs and animal defs that are then loaded
 * into the FurnitureToAnimatedObjectConverter during startup.
 * All of these defs are found an loaded by the startup function, So patches aren't needed,
 * although I have no idea how conflicts will resolve.*/ 
namespace MoodndBehaviorsAndEvents
{
    public class Def_AnimatableFurniture : Def
    {
        public List<ThingDef> targets;
        public List<PawnKindDef> results;
    } 
}
