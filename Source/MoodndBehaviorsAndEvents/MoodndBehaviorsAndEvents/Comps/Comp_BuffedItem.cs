using Verse;
using RimWorld;

// Modeled off the CompEnchantedItem from RoM, this comp is added to materials that cause things made from them to have non-standard buffs/effects 
// on their wielders/wearers.
// https://github.com/TorannD/TMagic/blob/f0895d1d296bf54184d5153ddbb307ac42b776dd/Source/TMagic/TMagic/Enchantment/CompEnchantedItem.cs#L231
namespace MoodndBehaviorsAndEvents
{
    public class Comp_BuffedItem : ThingComp
    {

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
                if (this.parent != null && this.parent.def.MadeFromStuff && this.parent.Stuff != null && this.parent.Stuff.GetCompProperties<CompProperties_BuffedStuff>() != null)
                {
                    return BuffedStuff != null;
                    // TODO change to this back to return BuffedStuff.isBuffed;
                }
                return false;
            }
        }

        public bool AppliedHediffOnEquip
        {
            get
            {
                if (MadeFromBuffedStuff)
                {
                    HediffDef appliedHediffDef = this.parent.Stuff.GetCompProperties<CompProperties_BuffedStuff>().appliedHediffWhenWorn;
                    if (appliedHediffDef != null)
                    {
                        return true;
                    }
                }
                return false;
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

        private bool shouldApplyHediffOnEquip = false;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            //Pawn pawn = this.parent as Pawn;
            shouldApplyHediffOnEquip = false;
            if (AppliedHediffOnEquip)
            {
                Apparel ap = this.parent as Apparel;
                if (BuffedStuff.requiredBodyPartGroup != null)
                {
                    bool meetsPartReq = false;
                    foreach (BodyPartGroupDef bpg in ap.def.apparel.bodyPartGroups)
                    {
                        if (bpg.Equals(BuffedStuff.requiredBodyPartGroup))
                        {
                            meetsPartReq = true;
                            break;
                        }
                    }
                    if (!meetsPartReq)
                    {
                        return; // apparel doesn't cover the right body parts - don't apply effects.
                    }
                }
                if (BuffedStuff.requiredLayer != null)
                {
                    bool meetsLayerReq = false;
                    foreach (ApparelLayerDef apd in ap.def.apparel.layers)
                    {
                        if (apd.Equals(BuffedStuff.requiredLayer))
                        {
                            meetsLayerReq = true;
                            break;
                        }
                    }
                    if (!meetsLayerReq)
                    {
                        return; // apparel doesn't include the right clothing layer - don't apply effects
                    }
                }
                //Log.Message(String.Format("MooDnD debug: initializing apparel made from special stuff: {0}, {1}", this.parent.def.defName, this.parent.Stuff.defName));
                shouldApplyHediffOnEquip = true;
            }
        }
        
        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
            if (shouldApplyHediffOnEquip)
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
    }
}
