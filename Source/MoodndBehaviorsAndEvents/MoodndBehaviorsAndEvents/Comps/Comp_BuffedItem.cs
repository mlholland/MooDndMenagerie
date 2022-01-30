using Verse;
using RimWorld;

// Modeled off the CompEnchantedItem from RoM, this comp is added to materials that cause things made from them to have non-standard buffs/effects 
// on their wielders/wearers.
// https://github.com/TorannD/TMagic/blob/f0895d1d296bf54184d5153ddbb307ac42b776dd/Source/TMagic/TMagic/Enchantment/CompEnchantedItem.cs#L231
namespace MoodndBehaviorsAndEvents
{
    public class Comp_BuffedItem : ThingComp
    {

        private static readonly string s_descriptionAddition = "DND_madeFromSpecialMaterialNote";

        public CompProperties_BuffedItem Props
        {
            get
            {
                return (CompProperties_BuffedItem)this.props;
            }
        }

        public CompProperties_BuffedStuff BuffedStuff
        {
            get
            {
                return this.parent.Stuff.GetCompProperties<CompProperties_BuffedStuff>();
            }
        }

        public bool MadeFromBuffedStuff
        {
            get
            {
                if (this.parent != null && this.parent.def.MadeFromStuff && this.parent.Stuff != null)
                {
                    return this.BuffedStuff != null;
                    // TODO change to this back to return BuffedStuff.isBuffed;
                }
                return false;
            }
        }


        // whether or not the item associated with this comp should apply a hediff when it's equipped
        private bool cachedAppliedHediffOnEquip = false;
        private bool cachedAppliedCalculated = false;
        public bool AppliesHediffOnEquip
        {
            get
            {
                if (!cachedAppliedCalculated)  {
                    cachedAppliedHediffOnEquip = false;
                    cachedAppliedCalculated = true;
                    if (MadeFromBuffedStuff)
                    {
                        HediffDef appliedHediffDef = BuffedStuff.appliedHediffWhenWorn;
                        if (appliedHediffDef != null)
                        { 
                            Apparel ap = this.parent as Apparel;
                            bool meetsPartReq = false; 
                            if (BuffedStuff.requiredBodyPartGroup != null)
                            {
                                foreach (BodyPartGroupDef bpg in ap.def.apparel.bodyPartGroups)
                                {
                                    if (bpg.Equals(BuffedStuff.requiredBodyPartGroup))
                                    { 
                                        meetsPartReq = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {

                                meetsPartReq = true;
                            }
                                
                            bool meetsLayerReq = false;
                            if ( BuffedStuff.requiredLayer != null)
                            { 
                                foreach (ApparelLayerDef apd in ap.def.apparel.layers)
                                {
                                    if (apd.Equals(BuffedStuff.requiredLayer))
                                    { 
                                        meetsLayerReq = true;
                                        break;
                                    }
                                }
                            } 
                            else 
                            {
                                meetsLayerReq = true;
                            }
                            // if this is true, then this item is made from a material that applies special effects,
                            // AND it covers the correct layers and body parts required by the material (if any)
                            if (meetsPartReq && meetsPartReq)
                            { 
                                cachedAppliedHediffOnEquip = true;
                            }  
                        } 
                    }
                } 
                return cachedAppliedHediffOnEquip;
            }
        }

        private string cachedDescriptionAddon;
        public string DescriptionAddon
        {
            get
            {
                if (cachedDescriptionAddon == null)
                {
                    cachedDescriptionAddon = "";
                    if(this.AppliesHediffOnEquip)
                    {
                        cachedDescriptionAddon = string.Format(s_descriptionAddition.Translate(), this.parent.Stuff == null ? "???" : this.parent.Stuff.label);
                    }
                }
                return cachedDescriptionAddon;
            }
        }

        public Pawn WearingPawn
        {
            get
            {
                Apparel ap = this.parent as Apparel;
                if (ap != null)
                {
                    if (ap.Wearer != null)
                    {
                        return ap.Wearer;
                    }
                }
                ThingWithComps twc = this.parent as ThingWithComps;
                if (twc != null)
                {
                    Pawn_EquipmentTracker p_et = twc.ParentHolder as Pawn_EquipmentTracker;
                    if (p_et != null && p_et.pawn != null)
                    {
                        return p_et.pawn;
                    }
                }
                return null;
            }
        }
        
        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
            if (this.AppliesHediffOnEquip)
            {
                HediffDef appliedHediffDef = this.parent.Stuff.GetCompProperties<CompProperties_BuffedStuff>().appliedHediffWhenWorn;
                if (pawn.health.hediffSet.GetFirstHediffOfDef(appliedHediffDef, false) == null)
                {
                    HediffComp_RemoveIfApparelDropped hediffComp_RemoveIfApparelDropped = pawn.health.AddHediff(appliedHediffDef).TryGetComp<HediffComp_RemoveIfApparelDropped>();
                    if (hediffComp_RemoveIfApparelDropped != null)
                    {
                        hediffComp_RemoveIfApparelDropped.wornApparel = (Apparel)this.parent;
                    }
                }
            }
        }

        public override string GetDescriptionPart()
        { 
            return base.GetDescriptionPart() + this.DescriptionAddon; 
        }
    }
}
