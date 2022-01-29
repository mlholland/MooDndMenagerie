using Verse; 
using UnityEngine;
using RimWorld;

/* This comp marks a holding animal as potentially stuffable.
 * If the stuff value is non-null the graphics patch at MoodndBehaviorsAndEvents.PawnGraphicSet_ResolveAllGraphics
 * will use that stuff's color to produce a new shader for the animal.
 * 
 * Also modifies the animals label and description when the material is set.
 */
namespace MoodndBehaviorsAndEvents
{
    public class Comp_StuffableAnimal : ThingComp
    {
        private static readonly string s_descriptionTranslationString = "DND_MadeFromMaterialAddon";
        private static readonly string s_stuffableLabel = "DND_AnimatedCreatureLabel";

        public ThingDef stuff = null;
        private string cachedDescription = null; 

        private string Description
        {
            get
            {
                if (cachedDescription == null && stuff != null)
                {
                    cachedDescription = string.Format(s_descriptionTranslationString.Translate(), stuff.label);
                }
                return cachedDescription;
            }
        }
 
        // Need to persist animal stuff across saves
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look<ThingDef>(ref stuff, "animalStuff");
        }

        public override string GetDescriptionPart()
        {
            if (stuff != null)
            {
                return base.GetDescriptionPart() + this.Description;
            }
            return base.GetDescriptionPart();
        }
        
        public override string TransformLabel(string label)
        {
            if (stuff != null)
            {
                return string.Format(s_stuffableLabel.Translate(), label, stuff.label);
            }
            return base.TransformLabel(label);
        }

    }
}
