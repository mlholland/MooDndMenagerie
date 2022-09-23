using UnityEngine;
using Verse;
using System;
/* Settings object largely copied/modified from https://github.com/juanosarg/AlphaAnimals/blob/master/1.3/Source/AlphaBehavioursAndEvents/AlphaBehavioursAndEvents/Options%20and%20Utility/AlphaAnimals_Settings.cs
 * */
namespace MoodndBehaviorsAndEvents
{
    public class Moodnd_Settings : ModSettings

    {
        private static Vector2 scrollPosition = Vector2.zero; 

        public bool flagMoodndMonsterRaids = true;
        public bool flagRustMonsterInfestations = true;
        public bool flagDebug = false;

        public const float moodndAnimalSpawnMultiplierBase = 1;
        public float moodndAnimalSpawnMultiplier = moodndAnimalSpawnMultiplierBase;
        // TODO figure out if this is even possible to implement - need to dig though ThingSetMakerDef
        // and see if I can specifically patch the Generate function behavior for the Reward_ItemsStandard instance
        //public const float moodndQuestRewardMultiplierBase = 1;
        //public float moodndQuestRewardMultiplier = moodndQuestRewardMultiplierBase;


        public override void ExposeData()
        {
            
            base.ExposeData();
            Scribe_Values.Look(ref flagMoodndMonsterRaids, "flagmoodndMonsterRaids", true, true);
            Scribe_Values.Look(ref flagRustMonsterInfestations, "flagRustMonsterInfestations", true, true);
            Scribe_Values.Look(ref flagDebug, "flagDebug", false, true);
            Scribe_Values.Look(ref moodndAnimalSpawnMultiplier, "moodndAnimalSpawnMultiplier", moodndAnimalSpawnMultiplierBase, true);
            //Scribe_Values.Look(ref moodndQuestRewardMultiplier, "moodndQuestRewardMultiplier", moodndQuestRewardMultiplierBase, true); 
        }

        public void DoWindowContents(Rect inRect)
        {
            
            Listing_Standard ls = new Listing_Standard();

            ls.Begin(inRect);
            
            ls.Gap(3f);
            //ls.Label("DND_MoodndRewardMultiplier".Translate() + ": " + moodndQuestRewardMultiplier, -1, "DND_MoodndRewardMultiplierTooltip".Translate());
            //moodndQuestRewardMultiplier = (float)Math.Round(ls.Slider(moodndQuestRewardMultiplier, 0.1f, 5f), 2);
            ls.CheckboxLabeled("allowDungeonMonsterRaids".Translate(), ref flagMoodndMonsterRaids, null);
            ls.CheckboxLabeled("allowRustMonsterInfestations".Translate(), ref flagRustMonsterInfestations, null);
            // spawn multiplier and individual spawn controllers
            ls.Label("DND_MoodndAnimalSpawnMultiplier".Translate() + ": " + moodndAnimalSpawnMultiplier, -1, "DND_MoodndAnimalSpawnMultiplierTooltip".Translate());
            moodndAnimalSpawnMultiplier = (float)Math.Round(ls.Slider(moodndAnimalSpawnMultiplier, 0.1f, 5f), 2);
            ls.Label("DND_IndividualSpawnSettingsNote".Translate());
            ls.CheckboxLabeled("DND_PrintDebugLogs".Translate(), ref flagDebug, null);

            ls.End();
            base.Write();
        }
    }
}
