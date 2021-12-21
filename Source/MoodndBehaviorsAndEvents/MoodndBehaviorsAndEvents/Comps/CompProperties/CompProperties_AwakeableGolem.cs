using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoodndBehaviorsAndEvents
{
    public class CompProperties_AwakeableGolem : CompProperties
    {

        public PawnKindDef CreatureDef;
        // public ~ todo add dirt type that spawns
        public string TranslationString = "DND_GolemAwoken";

        public CompProperties_AwakeableGolem()
        {
            this.compClass = typeof(Comp_AwakeableGolem);
        }


    }
}
