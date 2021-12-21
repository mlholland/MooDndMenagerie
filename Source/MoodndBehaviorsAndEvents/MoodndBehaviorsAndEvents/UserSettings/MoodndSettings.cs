using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System;
/* Settings object largely copied/modified from https://github.com/juanosarg/AlphaAnimals/blob/master/1.3/Source/AlphaBehavioursAndEvents/AlphaBehavioursAndEvents/Options%20and%20Utility/AlphaAnimals_Settings.cs
 * */
namespace MoodndBehaviorsAndEvents
{
    public class Moodnd_Settings : ModSettings

    {
        private static Vector2 scrollPosition = Vector2.zero;
        public Dictionary<string, bool> pawnSpawnStates = new Dictionary<string, bool>(); 

        public bool flagMoodndMonsterRaids = true;
        public bool flagRustMonsterInfestations = true;

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
            Scribe_Values.Look(ref moodndAnimalSpawnMultiplier, "moodndAnimalSpawnMultiplier", moodndAnimalSpawnMultiplierBase, true);
            //Scribe_Values.Look(ref moodndQuestRewardMultiplier, "moodndQuestRewardMultiplier", moodndQuestRewardMultiplierBase, true);
            Scribe_Collections.Look(ref pawnSpawnStates, "pawnSpawnStates", LookMode.Value, LookMode.Value, ref pawnKeys, ref boolValues);


        }
        private List<string> pawnKeys;
        private List<bool> boolValues;

        public void DoWindowContents(Rect inRect)
        {

            List<string> keys = pawnSpawnStates.Keys.ToList().OrderByDescending(x => x).ToList();
            Listing_Standard ls = new Listing_Standard();

            ls.Begin(inRect);


            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Rect rect2 = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Widgets.BeginScrollView(rect, ref scrollPosition, rect2, true);
            //ls.ColumnWidth = rect2.width / 2.2f;
            ls.Begin(rect2);
            //ls.Label("DND_MoodndRewardMultiplier".Translate() + ": " + moodndQuestRewardMultiplier, -1, "DND_MoodndRewardMultiplierTooltip".Translate());
            //moodndQuestRewardMultiplier = (float)Math.Round(ls.Slider(moodndQuestRewardMultiplier, 0.1f, 5f), 2);

            ls.Label("DND_EventSettingsTitle".Translate());
            ls.Gap(3f);
            ls.CheckboxLabeled("allowDungeonMonsterRaids".Translate(), ref flagMoodndMonsterRaids, null);
            ls.CheckboxLabeled("allowRustMonsterInfestations".Translate(), ref flagRustMonsterInfestations, null);
            ls.Gap(10f);
            ls.Label("DND_SpawningSettingsTitle".Translate());
            ls.Gap(3f);

            // spawn multiplier and individual spawn controllers
            ls.Label("DND_MoodndAnimalSpawnMultiplier".Translate() + ": " + moodndAnimalSpawnMultiplier, -1, "DND_MoodndAnimalSpawnMultiplierTooltip".Translate());
            moodndAnimalSpawnMultiplier = (float)Math.Round(ls.Slider(moodndAnimalSpawnMultiplier, 0.1f, 5f), 2);
            for (int num = keys.Count - 1; num >= 0; num--)
            {
                if (num == keys.Count / 2) { ls.NewColumn(); }
                bool test = pawnSpawnStates[keys[num]];
                ls.CheckboxLabeled("DND_DisableAnimal".Translate(PawnKindDef.Named(keys[num]).LabelCap), ref test);
                pawnSpawnStates[keys[num]] = test;
            }

            ls.End();
            Widgets.EndScrollView();
            base.Write();
        }
    }
}
