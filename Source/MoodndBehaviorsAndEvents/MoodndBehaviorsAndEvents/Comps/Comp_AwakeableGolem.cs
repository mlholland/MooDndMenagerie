using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

// A modified variant of the ComHediffEffector from the VEF that parameterizes psy sensitivity, among other things
namespace MoodndBehaviorsAndEvents
{
    public class Comp_AwakeableGolem : ThingComp
    {
        public CompProperties_AwakeableGolem Props
        {
            get
            {
                return (CompProperties_AwakeableGolem)this.props;
            }
        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (this.parent.Faction == Faction.OfPlayer)
            {
                yield return new Command_Action
                {
                    action = delegate ()
                    {
                        SoundDefOf.Roof_Collapse.PlayOneShot(null);
                        PawnGenerationRequest request = new PawnGenerationRequest(Props.CreatureDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, false, 0f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, false);

                        Pawn pawn = PawnGenerator.GeneratePawn(request);
                        GenSpawn.Spawn(pawn, this.parent.PositionHeld, this.parent.MapHeld, WipeMode.Vanish);
                        Messages.Message(string.Format((Props.TranslationString + "_Notification").Translate(), this.parent.Label, pawn.Name), MessageTypeDefOf.PositiveEvent, true);
                        this.parent.DeSpawn();
                    },
                    hotKey = KeyBindingDefOf.Misc2,
                    defaultDesc = (Props.TranslationString+"_IconDesc").Translate(Props.CreatureDef.label),
                    icon = ContentFinder<Texture2D>.Get(Props.IconString, true),
                    defaultLabel = (Props.TranslationString+"_IconLabel").TranslateSimple()
                };
            }
            yield break;
        }
    }
}
