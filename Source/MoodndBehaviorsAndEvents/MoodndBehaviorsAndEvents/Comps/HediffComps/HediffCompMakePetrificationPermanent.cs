using Verse;

// Replace the disease-like version of petrification with an untendable, permanent version at max severity.
namespace MoodndBehaviorsAndEvents
{
    public class HediffCompMakePetrificationPermanent : HediffComp
    {
        public int tickCounter = 0;
        private static string PERMANENT_HEDIFF_NAME = "DND_Petrification_Complete";
        private static string PERMANENT_HEDIFF_NAME_MECHS = "DND_Petrification_Complete_Mechs";


        public HediffCompProperties_MakePetrificationPermanent Props
        {
            get
            {
                return (HediffCompProperties_MakePetrificationPermanent)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.parent.Severity >= 1) {
                HediffDef perm = (this.Pawn.RaceProps.IsMechanoid || this.Pawn.RaceProps.IsWorkMech) ? HediffDef.Named(PERMANENT_HEDIFF_NAME_MECHS) : HediffDef.Named(PERMANENT_HEDIFF_NAME);
                this.Pawn.health.AddHediff(perm);
                this.Pawn.health.RemoveHediff(this.parent);
            }
        }
    }
}
