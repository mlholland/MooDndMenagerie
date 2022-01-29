using Verse;
using RimWorld; 

// Note doesn't work lol
// Intent is to make cretures that die while having the Disintegration dust actually turn to dust, and make annihilators eat that.
namespace MoodndBehaviorsAndEvents
{
    public class HediffComp_TurnToDustOnDeath : HediffComp
    {
        public HediffCompProperties_TurnToDustOnDeath Props
        {
            get
            {
                return (HediffCompProperties_TurnToDustOnDeath)this.props;
            }
        }

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            if (!base.Pawn.Spawned)
            {
                return;
            }
            for (int i = 0; i < 3; i++)
            {
                FleckMaker.ThrowSmoke(base.Pawn.DrawPos, base.Pawn.Map, Rand.Range(.5f, 1.1f));
            }
            Thing droppedItem = ThingMaker.MakeThing(ThingDef.Named("DND_OrganicDust"), null);
            int baseDropAmount = 20 * (int)base.Pawn.def.statBases.GetStatValueFromList(StatDefOf.Nutrition, 1);
            droppedItem.stackCount = baseDropAmount;
            GenPlace.TryPlaceThing(droppedItem, base.Pawn.Position, base.Pawn.Map, ThingPlaceMode.Near);
            base.Pawn.Corpse.Destroy();
        }
    }
}
