using Verse;

/* The propery associated with HediffComp_ChangeColorOnAdd*/
namespace MoodndBehaviorsAndEvents
{
    public class HediffCompProperties_ChangeColorOnAdd : HediffCompProperties
    {
        public UnityEngine.Color color;

        public HediffCompProperties_ChangeColorOnAdd()
        {
            this.compClass = typeof(HediffComp_ChangeColorOnAdd);
        }
    }
}
