using System; 
using Verse.Sound;
using Verse;
using RimWorld;

// Decay the given body part, starting with the outside layer of clothing. Can be configured to only target metal stuff (for rusting effect) or anything (for disintegration)
// Does not do any standard "should this hit logic" beyond a few unique calculations, standard logic from the DebuffAddingUtil should have been done prior to this hediff being added
namespace MoodndBehaviorsAndEvents
{
    public class HediffComp_Decay : HediffComp
    {

        public HediffCompProperties_Decay Props
        {
            get
            {
                return (HediffCompProperties_Decay)this.props;
            }
        }

        public override bool CompShouldRemove
        {
            get
            {
                return base.CompShouldRemove || shouldRemoveHediff;
            }
        }


        // figure out if I can cache these values to prevent recalculations
        bool shouldRemoveHediff = false;
        Apparel selectedApparel = null;
        bool ApparelIsMetal = false;
        bool PartIsMetal = false;

        // todo cache a lot of stuff
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.parent.ageTicks % Props.decayRateTicks == 0)
            {
                // Get apparel to decay
                if (selectedApparel == null)
                { 
                    selectedApparel = SelectApparel();
                }
                // Make sure apparel exists
                if (selectedApparel != null)
                { 
                    // Apparel exists on body part, try to decay it

                    // check that apparel is metal if needed
                    if (Props.targetMustBeMetallic && !IsApparelMetal(this.selectedApparel))
                    { 
                        shouldRemoveHediff = true;
                        return;
                    }
                    
                    selectedApparel.TakeDamage(new DamageInfo(DamageDefOf.Deterioration, Props.decayDmg, 2)); // todo cache this dInfo maybe?

                    SoundDef.Named("DeathAcidifier").PlayOneShot(SoundInfo.InMap(this.Pawn, MaintenanceType.None));
                    //MoteMaker.MakeStaticMote(loc, base.Pawn.Map, Mote, 1f);
                    if (Props.removeHediffIfAnyClothingDestroyed && selectedApparel.HitPoints <= 0)
                    {
                        shouldRemoveHediff = true;
                    } else
                    {
                        selectedApparel = null;
                    }
                }
                else if (Props.damageBodyPartIfUncovered) // If no clothing is covering this part, and this can damage the body directly, then apply damage to the pawn.
                {
                    // check that the part is metal if needed
                    if (Props.targetMustBeMetallic && !IsBodyPartMetal(this.parent.Part))
                    {
                        shouldRemoveHediff = true;
                        return;
                    } 
                    // TODO parametize this more, and attach a battle log (see impact code in vanilla for example)
                    this.Pawn.TakeDamage(new DamageInfo(Props.skinDamage, Props.decayDmg, 2, -1, null, this.parent.Part));
                }
                else
                {
                    shouldRemoveHediff = true;
                }
            }
        }
 
        private Apparel SelectApparel()
        {
            // don't waste effort on naked stuff like animals
            if (this.Pawn.apparel == null) {
                return null;
            } 

            // If we somehow hit a body part that's not grouped, pick random clothing, and log a warning
            // Since this probably indicates an improperly fletched out alien body plan.
            else if (this.parent.Part.groups.Count == 0) 
            {
                Log.Warning("Moodnd HediffComp_Decay: Tried to decay clothing but found no source body part group. Choosing random clothing");
                return this.Pawn.apparel.WornApparel.RandomElement();
            }
            // Otherwise, iterate through all body part groups associated with the hit part, and try to get clothing covering those groups.
            else if (this.parent.Part.groups != null && this.parent.Part.groups.Count > 0)
            {
                foreach (BodyPartGroupDef targetGroup in this.parent.Part.groups)
                { 
                    Apparel targetApparel = null;
                    // the following line does NOT work - 'first' means 'first in the sequential list of apparel the pawn is wearing as shown in the in-game UI'
                    // targetApparel = this.Pawn.apparel.FirstApparelOnBodyPartGroup(this.parent.Part.groups[0]);

                    // loop though worn apparel
                    foreach (Apparel app in this.Pawn.apparel.WornApparel)
                    { 
                        // short circuit to make sure we don't check apparel that's under the current best
                        if (targetApparel == null || app.def.apparel.LastLayer.drawOrder > targetApparel.def.apparel.LastLayer.drawOrder) {
                            foreach (BodyPartGroupDef appPart in app.def.apparel.bodyPartGroups)
                            { 
                                if (appPart.defName == targetGroup.defName)
                                { 
                                    targetApparel = app;
                                }
                            }
                        }
                    }
                    if (targetApparel != null)
                    {
                        return targetApparel;
                    }
                }
            }
            return null;
        }

        // todo if the item is uncraftable, maybe just default to high tech level? Chances are any high tech clothing is ganna be metal.
        private bool IsApparelMetal(Apparel apparel)
        {
            
            if (apparel == null)
            {
                return false;
            }
            // see if this apparel is made out of metal (ex: plate armor)

            // First check for metal stuff type if clothing can be made from multiple materials.
            if (apparel.Stuff != null && apparel.Stuff.stuffProps != null)
            {
                foreach (StuffCategoryDef stuffCatDef in apparel.Stuff.stuffProps.categories)
                {
                    if (stuffCatDef.Equals(StuffCategoryDefOf.Metallic))
                    {
                        return true;
                    }
                }
            }
            // Then check all parts from cost list for any metallics.
            if (apparel.def.costList != null)
            {
                foreach(ThingDefCountClass thingCount in apparel.def.costList)
                {
                    if (thingCount.thingDef.IsStuff && thingCount.thingDef.stuffProps != null && thingCount.thingDef.stuffProps.categories != null)
                    {
                        foreach (StuffCategoryDef stuffCatDef in thingCount.thingDef.stuffProps.categories)
                        {
                            if (stuffCatDef.Equals(StuffCategoryDefOf.Metallic))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
           
            return false;
        }

        private bool IsBodyPartMetal(BodyPartRecord part)
        {
            if (this.Pawn.RaceProps.IsMechanoid) // rust monsters mess up mechs
            {
                return true;
            }
            if (this.Pawn.def.defName == "DND_GolemSteel") // TODO: turn this into a read-in XML list of metal-but-not-mech creatures.
            {
                return true;
            }
            Hediff partHediff = HediffSetExtraLogic.TryGetAddedPartOrFirstAncestorDirectlyAddedParts(this.Pawn.health.hediffSet, part);
            if (partHediff != null)
            {
                return IsMetalPartHediff(partHediff);
            }
            return false;
        }

        // Currently works by trying to find the item behind an implant, then checking the tech level of that (so peg legs are ignored)
        private bool IsMetalPartHediff(Hediff partHediff)
        {
            // TODO maybe search for the recipe instead, since that should link physical parts to resulting hediffs.
            ThingDef partThing = DefDatabase<ThingDef>.GetNamed(partHediff.def.defName, false);
            if (partThing == null)
            {
                Log.Message(String.Format("no part ThingDef found for hediff: {0}", partHediff.def.defName));
                return false;
            } 

            if (partThing != null && partThing.techLevel.CompareTo(TechLevel.Industrial) >= 0)
            {
                return true;
            }
            return false;
        }
    }
}


/*gear repair logic
 
         [StaticConstructorOnStartup]
    class HediffComp_GearRepair : HediffComp
    {

        private bool initializing = true;

        public string labelCap
        {
            get
            {
                return base.Def.LabelCap;
            }
        }

        public string label
        {
            get
            {
                return base.Def.label;
            }
        }


        private void Initialize()
        {
            bool spawned = base.Pawn.Spawned;
            if (spawned)
            {
                FleckMaker.ThrowLightningGlow(base.Pawn.TrueCenter(), base.Pawn.Map, 3f);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            bool flag = base.Pawn != null;
            if (flag)
            {
                if (initializing)
                {
                    initializing = false;
                    this.Initialize();
                }
            }
            if(Find.TickManager.TicksGame % 1200 == 0)
            {
                TickAction();
            }
        }

        public void TickAction()
        {
            List<Apparel> gear = this.Pawn.apparel.WornApparel;
            for(int i = 0; i < gear.Count; i++)
            {
                if(Rand.Chance(.2f) && gear[i].HitPoints < gear[i].MaxHitPoints)
                {
                    gear[i].HitPoints++;
                }
                CompAbilityUserMight comp = this.Pawn.GetComp<CompAbilityUserMight>();
                if (comp != null && comp.MightData.MightPowerSkill_FieldTraining.FirstOrDefault((MightPowerSkill x) => x.label == "TM_FieldTraining_ver").level >= 5)
                {
                    if (gear[i].HitPoints >= gear[i].MaxHitPoints && gear[i].WornByCorpse)
                    {
                        gear[i].Notify_PawnResurrected();
                        Traverse.Create(root: gear[i]).Field(name: "wornByCorpseInt").SetValue(false);
                    }
                }
            }
            Thing weapon = this.Pawn.equipment.Primary;
            if (weapon != null && (weapon.def.IsRangedWeapon || weapon.def.IsMeleeWeapon))
            {
                if(Rand.Chance(.2f) && weapon.HitPoints < weapon.MaxHitPoints)
                {
                    weapon.HitPoints++;
                }
            }
        }
     */
