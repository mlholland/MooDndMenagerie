 using Verse;

// Inspired by RoM's CompProperties_EnchantedStuff. This is added to stuffDefs (metals, leathers, etc) that will cause items made
// out of them to have special buffs.
// https://github.com/TorannD/TMagic/blob/f0895d1d296bf54184d5153ddbb307ac42b776dd/Source/TMagic/TMagic/Enchantment/CompProperties_EnchantedStuff.cs
namespace MoodndBehaviorsAndEvents
{
    public class CompProperties_BuffedStuff : CompProperties
    {
        public HediffDef appliedHediffWhenWorn = null;
        public ApparelLayerDef requiredLayer = null; // if nonnull, then clothing must cover this layer to cause special effects.
        public BodyPartGroupDef requiredBodyPartGroup = null; // if nonnull, then clothing must cover this body part group to cause special effects

        public CompProperties_BuffedStuff()
        {
            this.compClass = typeof(Comp_BuffedStuff);
        }

    }
}
