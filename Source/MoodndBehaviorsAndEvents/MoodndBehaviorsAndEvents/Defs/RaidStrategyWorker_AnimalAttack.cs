using System.Collections.Generic; 
using Verse;
using Verse.AI.Group; 
using RimWorld;

/* Simple modified version of the vanilla RaidStrategyWorker_ImmediateAttack that has a slightly varied lord
 * to ensure that animals attack properly. Can only be hostile. */
namespace MoodndBehaviorsAndEvents
{
    public class RaidStrategyWorker_AnimalAttack : RaidStrategyWorker
    {
        protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
        { 
            return new LordJob_AssaultColony(parms.faction, false, false, true, false, false, false, false);
        }
    }
}
