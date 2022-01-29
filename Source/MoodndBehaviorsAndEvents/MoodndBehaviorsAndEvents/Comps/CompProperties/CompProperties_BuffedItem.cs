using Verse;

// Based off of RoM's CompProperties_EnchantedItem. This is added to ThingDefs of weapons/apparel to give them special buffs.
// https://github.com/TorannD/TMagic/blob/f0895d1d296bf54184d5153ddbb307ac42b776dd/Source/TMagic/TMagic/Enchantment/CompProperties_EnchantedItem.cs
namespace MoodndBehaviorsAndEvents
{
    public class CompProperties_BuffedItem : CompProperties
    {
        public CompProperties_BuffedItem()
        {
            this.compClass = typeof(Comp_BuffedItem);
        }
    }
}
