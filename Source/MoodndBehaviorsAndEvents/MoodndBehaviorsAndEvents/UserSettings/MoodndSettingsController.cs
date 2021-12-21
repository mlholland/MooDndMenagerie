using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;


/* Code for adding a settings window which contains toggles for animal spawning and events. Behavior was largely copied from my favorite teacher: 
 https://github.com/juanosarg/AlphaAnimals/blob/master/1.3/Source/AlphaBehavioursAndEvents/AlphaBehavioursAndEvents/Options%20and%20Utility/AlphaAnimals_SettingsController.cs 
 */
namespace MoodndBehaviorsAndEvents
{
    public class MoodndManagerie_Mod : Mod
    {

        public static Moodnd_Settings settings;

        public MoodndManagerie_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<Moodnd_Settings>();
        }
        public override string SettingsCategory() => "DND Managerie: Event and Spawning Options";

        public override void DoSettingsWindowContents(Rect inRect) 
        {
            base.DoSettingsWindowContents(inRect);

            ToggleableSpawnDef toggleablespawndef = (from k in DefDatabase<ToggleableSpawnDef>.AllDefsListForReading
                                                     where k.defName == "DND_ToggleableAnimals"
                                                     select k).RandomElement();


            if (settings.pawnSpawnStates == null) settings.pawnSpawnStates = new Dictionary<string, bool>();
            foreach (string defName in toggleablespawndef.toggleablePawns)
            {
                if (!settings.pawnSpawnStates.ContainsKey(defName))
                {
                    settings.pawnSpawnStates[defName] = false;
                }
            }



            settings.DoWindowContents(inRect);


        }
    }
}