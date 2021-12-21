using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;


/* Utility code to help modders get from stuff categories to the ThingDefs associated with that category, since
 * AFAIK there's no build-in functions or fields to make this easy. 
 * Uses a basic singleton instance, and returns copied values to ensure that editing return values doesn't
 * corrupt the underlying data.
 * That said, these methods are probably a little slow, so it's recommended to avoid using them in situations
 * Note: where speed is important (like events that occur on every tick), so caching return values might be prudent.
 * Note: The returned values are not ThingDefs, but Strings of ThingDef defNames. Storing ThingDefs in HashSets creates
 * new thing defs with the same name as the originals, which apparently makes rimworld cataclysmically explorde, so you'll
 * need run ThingDef.Named(element-from-this-utility's-output) to actually get the Thingdefs you want.
 */
namespace MoodndBehaviorsAndEvents
{
    class StuffCategoryToThingUtil
    {
        private SortedDictionary<String, HashSet<String>> stuffSets;

        private static StuffCategoryToThingUtil instance;

        public static StuffCategoryToThingUtil GetInstance()
        {
            if (instance == null)
            {
                instance = new StuffCategoryToThingUtil();
            }
            return instance;
        }

        private StuffCategoryToThingUtil()
        {
            stuffSets = new SortedDictionary<String, HashSet<String>>();
            foreach(ThingDef thing in ThingCategoryDefOf.Root.DescendantThingDefs)
            {
                if(thing.IsStuff)
                {
                    foreach(StuffCategoryDef stuffCat in thing.stuffProps.categories)
                    {
                        if(!stuffSets.ContainsKey(stuffCat.defName)) {
                            stuffSets.Add(stuffCat.defName, new HashSet<String>());
                        }
                        stuffSets[stuffCat.defName].Add(thing.defName);
                    }
                }
            }
        }

        public HashSet<String> GetThingDefsOfStuffCategory(StuffCategoryDef stuffCat)
        {
            HashSet<String> ret = new HashSet<String>();
            if (!stuffSets.ContainsKey(stuffCat.defName))
            {
                return ret;
            }
            foreach (String defName in stuffSets[stuffCat.defName])
            {
                ret.Add(defName);
            }
            return ret;
        }

        public HashSet<String> GetThingDefsOfStuffCategories(List<StuffCategoryDef> stuffCats)
        {
            HashSet<String> ret = new HashSet<String>();
            if (stuffCats == null || stuffCats.Count == 0)
            {
                return ret;
            }
            foreach (StuffCategoryDef stuffCat in stuffCats)
            {
                if (!stuffSets.ContainsKey(stuffCat.defName))
                {
                    continue;
                }
                foreach (String defName in stuffSets[stuffCat.defName])
                {
                    ret.Add(defName);
                }
            }
            return ret;
        }
    }
}
