using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoodndBehaviorsAndEvents
{
    public class HediffCompProperties_Decay : HediffCompProperties
    {
        public int decayRateTicks = 60;
        public int decayDmg = 1;
        public bool damageBodyPartIfUncovered = true;
        public bool removeHediffIfAnyClothingDestroyed = true;
        public bool targetMustBeMetallic = false;
        public DamageDef skinDamage = null;

        public HediffCompProperties_Decay()
        {
            this.compClass = typeof(HediffComp_Decay);
        } 
    }
}
