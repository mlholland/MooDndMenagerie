using Verse; 

/* This HediffComp works in tandem with a comp on pawns called MoodndBehaviorsAndEvents.Comp_RecolorableAnimal 
 * and a harmony patch defined in MoodndBehaviorsAndEvents.PawnGraphicSet_ResolveAllGraphics.
 * If an animal has Comp_RecolorableAnimal comp, it gets its color set to the color specified in this comp.
 * This hediffComp then forces a graphics refresh of this animal, which triggers the harmony patch to 
 * notice that this animal has a non-null Comp_RecolorableAnimal color, at which point is uses that to 
 * recolor the animal.
 */
namespace MoodndBehaviorsAndEvents
{
    public class HediffComp_ChangeColorOnAdd : HediffComp
    {
        public HediffCompProperties_ChangeColorOnAdd Props
        {
            get
            {
                return (HediffCompProperties_ChangeColorOnAdd)this.props;
            }
        }
        
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            Comp_RecolorableAnimal colorComp = this.Pawn.TryGetComp<Comp_RecolorableAnimal>();
            if (colorComp != null)
            {
                colorComp.newColor = Props.color;
                colorComp.colorSet = true;
                this.Pawn.Drawer.renderer.graphics.SetAllGraphicsDirty();
            } 
        } 
    }
}
