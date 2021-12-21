using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

// Utility functions to keep track of furniture and their assocaited animated object defs
namespace MoodndBehaviorsAndEvents
{
    class FurnitureToAnimatedObjectConverter
    {
        private FurnitureToAnimatedObjectConverter() { }

        private static Dictionary<String, String> FURNITURE_TO_ANIMALS = new Dictionary<string, string>();
        private static Dictionary<String, Dictionary<String,String>> FURNITURE_TO_STUFFABLE_ANIMALS = new Dictionary<string, Dictionary<String, String>>();

        public static bool Add(ThingDef furniture, ThingDef animal)
        {
            if (FURNITURE_TO_ANIMALS.ContainsKey(furniture.defName))
            {
                Log.Error(String.Format("Tried to add animatad animal def to same furniture ({0}) multiple times", furniture.defName));
                return false;
            }
            //Log.Message(String.Format("Adding def to dictionary: {0}/{1}", furniture.defName, animal.defName));
            FURNITURE_TO_ANIMALS.Add(furniture.defName, animal.defName);
            return true;
        }

        public static bool AddStuffable(ThingDef furniture, ThingDef stuff, ThingDef animal)
        {
            if (!FURNITURE_TO_STUFFABLE_ANIMALS.ContainsKey(furniture.defName))
            {
                FURNITURE_TO_STUFFABLE_ANIMALS.Add(furniture.defName, new Dictionary<string, string>());
            }

            Dictionary<String, String> stuffDict = FURNITURE_TO_STUFFABLE_ANIMALS[furniture.defName];
            if (stuffDict.ContainsKey(stuff.defName))
            {
                Log.Error(String.Format("Tried to add animatad animal def to same furniture/stuff combo ({0},{1}) multiple times", furniture.defName, stuff.defName));
                return false;
            }
            //Log.Message(String.Format("Adding stuffable def to dictionary: {0}/{1}/{2}", furniture.defName, stuff.defName, animal.defName));
            stuffDict.Add(stuff.defName, animal.defName);
            return true;
        }

        public static ThingDef Get(ThingDef furniture)
        {
            if (FURNITURE_TO_ANIMALS.ContainsKey(furniture.defName))
            {
                return ThingDef.Named(FURNITURE_TO_ANIMALS[furniture.defName]);
            }
            Log.Error(String.Format("Tried to get animated animal associated with funiture named '{0}', but found nothing.", furniture.defName));
            return null;
        }


        public static ThingDef GetStuffable(ThingDef furniture, ThingDef stuff)
        {
            if (FURNITURE_TO_STUFFABLE_ANIMALS.ContainsKey(furniture.defName))
            {
                Dictionary<String, String> stuffDict = FURNITURE_TO_STUFFABLE_ANIMALS[furniture.defName];
                if (stuffDict.ContainsKey(stuff.defName))
                {
                    return ThingDef.Named(stuffDict[stuff.defName]);
                }
                Log.Error(String.Format("Tried to get stuffable animated animal associated with funiture '{0}' and stuff '{1}', but found nothing assocaited with this stuff.", furniture.defName, stuff.defName));

            }
            Log.Error(String.Format("Tried to get stuffable animated animal associated with funiture '{0}' and stuff '{1}', but found nothing assocaited with this furniture.", furniture.defName, stuff.defName));
            return null;
        }
    }
}
