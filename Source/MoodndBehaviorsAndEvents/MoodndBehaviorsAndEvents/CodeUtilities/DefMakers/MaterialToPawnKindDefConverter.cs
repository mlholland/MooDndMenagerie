using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

/* Helper functions which produce pawnkind defs based on furniture and stuff. Used for procedurally generating
 * animataed furniture defs 
 */
namespace MoodndBehaviorsAndEvents
{
    class MaterialToPawnKindDefConverter
    {
        // todo make functions
        public static void ModifyDef(PawnKindDef original, ThingDef furniture)
        {
            foreach (PawnKindLifeStage origStage in original.lifeStages)
            {
                origStage.bodyGraphicData.texPath = furniture.graphicData.texPath;
                origStage.bodyGraphicData.drawSize = furniture.graphicData.drawSize;
                origStage.bodyGraphicData.color = furniture.graphicData.color;
                origStage.bodyGraphicData.graphicClass = furniture.graphicData.graphicClass;
            }
        }

        // todo make functions
        public static PawnKindDef MakeDef(Def_AnimatedFurniture original, ThingDef animalDef, ThingDef furniture)
        {
            PawnKindDef copyKindDef = new PawnKindDef();
            copyKindDef.defName = original.defName;
            copyKindDef.label = original.label;
            copyKindDef.race = animalDef;
            copyKindDef.labelPlural = original.labelPlural;
            copyKindDef.ecoSystemWeight = original.ecoSystemWeight;
            copyKindDef.combatPower = original.combatPower;
            copyKindDef.wildGroupSize = original.wildGroupSize;
            copyKindDef.lifeStages = new List<PawnKindLifeStage>();
            foreach (PawnKindLifeStage origStage in original.lifeStages)
            {
                PawnKindLifeStage copyStage = new PawnKindLifeStage();
                copyStage.bodyGraphicData = new GraphicData();
                copyStage.bodyGraphicData.texPath = furniture.graphicData.texPath;
                copyStage.bodyGraphicData.drawSize = furniture.graphicData.drawSize;
                copyStage.bodyGraphicData.color = furniture.graphicData.color;
                copyStage.bodyGraphicData.graphicClass = furniture.graphicData.graphicClass;
                copyStage.bodyGraphicData.shadowData = origStage.bodyGraphicData.shadowData;
                copyStage.dessicatedBodyGraphicData = origStage.dessicatedBodyGraphicData;
                copyStage.label = origStage.label;
                copyKindDef.lifeStages.Add(copyStage);
                
            }
            return copyKindDef;
        }

        public static PawnKindDef MakeStuffBasedDef(Def_AnimatedFurniture original,  ThingDef animalDef, ThingDef furniture, ThingDef stuff)
        {
            // recycle normal def making code
            PawnKindDef copyKindDef = MakeDef(original, animalDef, furniture);
            // Then modify values that have stuff parameterization
            copyKindDef.defName = String.Format("{0}_{1}", original.defName, stuff.defName);
            copyKindDef.label = String.Format("{0} ({1})", original.label, stuff.label);
            copyKindDef.race = animalDef;
            copyKindDef.labelPlural = String.Format("{0} ({1})", original.labelPlural, stuff.label);
            foreach (PawnKindLifeStage stage in copyKindDef.lifeStages)
            {
                stage.bodyGraphicData.color = stuff.stuffProps.color;
            }
            return copyKindDef;
        }
    }
}

