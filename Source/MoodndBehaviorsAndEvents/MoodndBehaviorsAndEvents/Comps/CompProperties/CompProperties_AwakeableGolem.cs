using Verse;

namespace MoodndBehaviorsAndEvents
{
    public class CompProperties_AwakeableGolem : CompProperties
    { 
        public PawnKindDef CreatureDef;
        public string IconString = "UI/Abilities/DND_Awaken";
        public string TranslationString = "DND_GolemAwoken";

        public CompProperties_AwakeableGolem()
        {
            this.compClass = typeof(Comp_AwakeableGolem);
        } 
    }
}
