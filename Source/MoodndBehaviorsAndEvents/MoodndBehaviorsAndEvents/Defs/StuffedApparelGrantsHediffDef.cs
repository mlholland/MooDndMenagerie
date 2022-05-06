using System.Collections.Generic; 
using Verse;
using RimWorld;
using System.Linq;

/* This def defines a relationship between a material and hediff. The specified hediff is added
 * to pawns who wear apparel that is made from the specified material. Additional layer and body part requirements can also be specified. 
 * NOTE: actual logic that uses and applies hediffs based on this def is caused by a harmony patch in Pawn_ApparelTracker_Wear.cs
 */
namespace MoodndBehaviorsAndEvents
{
    public class StuffedApparelGrantsHediffDef : Def
    {

        ThingDef stuff = null; // The material the watch for.
        public HediffDef appliedHediffDef = null; // The hediff to apply.
        public List<ApparelLayerDef> validLayers = null; // if nonnull, then clothing must cover one of these layers to cause special effects.
        public List<BodyPartGroupDef> validBodyPartGroups = null; // if nonnull, then clothing must cover one of these body part groups to cause special effects.

        private static Dictionary<ThingDef, List<StuffedApparelGrantsHediffDef>> stuffToBuffsMap = new Dictionary<ThingDef, List<StuffedApparelGrantsHediffDef>>();

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (var error in base.ConfigErrors()) yield return error;
            if (stuff == null) yield return "stuff cannot be null";
            if (!stuff.IsStuff) yield return "stuff must have true IsStuff value";
            if (appliedHediffDef == null) yield return "appliedHediffDef cannot be null";
            if (!appliedHediffDef.HasComp(typeof(HediffComp_RemoveIfApparelDropped))) yield return "appliedHediffDef must be a hediff which has the HediffComp_RemoveIfApparelDropped comp";
        }
        
        public override void ResolveReferences()
        {
            base.ResolveReferences();
            if (!stuffToBuffsMap.ContainsKey(stuff))
            {
                stuffToBuffsMap[stuff] = new List<StuffedApparelGrantsHediffDef>();
            }
            stuffToBuffsMap[stuff].Add(this);
        }

        public static List<HediffDef> GetAllHediffsToApply(Apparel app)
        {
            if (app.Stuff == null || !stuffToBuffsMap.ContainsKey(app.Stuff)) return new List<HediffDef>();

            return (from def in stuffToBuffsMap[app.Stuff] where def.ShouldApplyHediffWhenWorn(app) select def.appliedHediffDef).ToList();
        }

        public bool ShouldApplyHediffWhenWorn(Apparel app)
        {
            if (app.Stuff == null || app.Stuff != this.stuff) return false;

            if (this.validBodyPartGroups != null)
            {
                bool passedPartCheck = false;
                List<BodyPartGroupDef> appGroups = app.def.apparel.bodyPartGroups;
                foreach (BodyPartGroupDef bpg in this.validBodyPartGroups)
                {
                    if(appGroups.Contains(bpg)) {
                        passedPartCheck = true;
                        break;
                    }
                }
                if (!passedPartCheck) return false;
            }

            if (this.validLayers != null)
            {
                bool passedLayerCheck = false;
                List<ApparelLayerDef> appLayers = app.def.apparel.layers;
                foreach (ApparelLayerDef layer in this.validLayers)
                {
                    if (appLayers.Contains(layer))
                    {
                        passedLayerCheck = true;
                        break;
                    }
                }
                if (!passedLayerCheck) return false;
            }
            return true;
        }


    } 
}
