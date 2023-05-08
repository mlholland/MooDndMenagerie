using Verse; 

// Ghost CompProps to be help construct a Comp_InertMimic instance.
namespace MoodndBehaviorsAndEvents
{
    public class CompProperties_InertMimic : CompProperties
    { 
        public CompProperties_InertMimic()
        {
            this.compClass = typeof(Comp_InertMimic);
        } 
    }
}
