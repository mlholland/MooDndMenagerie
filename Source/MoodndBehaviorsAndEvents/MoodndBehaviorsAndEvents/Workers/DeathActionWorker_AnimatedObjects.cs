using Verse;
using RimWorld;
using System;

/* Makes animated furniture animals automatically turn into their component materials on death.
 * This is done for 2 reasons
 * - It prevents possible material duplication with >100% butchery efficiency
 * - It prevents animated creature resurrection, which is an (admittedly arbitrary) balance concern for very powerful creatures
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
            Thing droppedItem;
            // this is the calculation for the displayed meat amount, we do it this way to avoid player confusion.
            int dropAmount = (int)(corpse.InnerPawn.def.statBases.GetStatValueFromList(StatDefOf.MeatAmount, StatDefOf.MeatAmount.defaultBaseValue) * corpse.InnerPawn.RaceProps.baseBodySize); 
            Comp_StuffableAnimal stuffComp = corpse.InnerPawn.TryGetComp<Comp_StuffableAnimal>();
            if (stuffComp != null && stuffComp.stuff != null)
            {
                droppedItem = ThingMaker.MakeThing(stuffComp.stuff, null);
                if (stuffComp.stuff.smallVolume)
                {
                    dropAmount *= dropAmount * 10; // TODO for some reason stuffComp.stuff.VolumePerUnit was 4 for gold, not sure where real conversion is stored. Should find it.
                } 
            } else
            {
                droppedItem = ThingMaker.MakeThing(corpse.InnerPawn.def.race.meatDef, null);
            } 
            dropAmount = Math.Max(1, Rand.Range((int)(dropAmount * 0.8f), dropAmount)); 
            droppedItem.stackCount = dropAmount;
            GenPlace.TryPlaceThing(droppedItem, corpse.Position, corpse.Map, ThingPlaceMode.Near); 
            corpse.Destroy();
        }
    }
}