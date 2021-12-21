using Verse;
using RimWorld;

/* Makes animated furniture animals automatically turn into their component materials on death.
 * This is done for 2 reasons
 * - It prevents possible material duplication with 100% butchery efficiency
 * - Resolving corpse references in my harmony patch seems to break the game lol.
 */
namespace MoodndBehaviorsAndEvents
{
    class DeathActionWorker_AnimatedObjects : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            for (int i = 0; i < 3; i++)
            {
                FleckMaker.ThrowSmoke(corpse.DrawPos, corpse.Map, Rand.Range(.5f, 1.1f));
            } 
            Thing droppedItem = ThingMaker.MakeThing(corpse.InnerPawn.def.race.leatherDef, null);
            int baseDropAmount = (int)corpse.InnerPawn.def.statBases.GetStatValueFromList(StatDefOf.LeatherAmount, StatDefOf.LeatherAmount.defaultBaseValue);
            droppedItem.stackCount = Rand.Range((int)(baseDropAmount * 0.8f), baseDropAmount);
            GenPlace.TryPlaceThing(droppedItem, corpse.Position, corpse.Map, ThingPlaceMode.Near); 
            corpse.Destroy();
        }
    }
}