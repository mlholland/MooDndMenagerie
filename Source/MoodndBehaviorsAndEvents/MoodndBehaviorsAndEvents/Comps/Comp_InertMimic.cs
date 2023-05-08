using Verse;
using RimWorld; 
using Verse.AI;

/* This comp designates a building as a hidden mimic.
 * 
 * This class is a ghost comp. It can only be added to things programatically, and requires a special
 * harmony patch to save/load properly.
 */
namespace MoodndBehaviorsAndEvents
{
    class Comp_InertMimic : ThingComp
    {
        public Comp_InertMimic() { }
    }
}
