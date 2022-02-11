using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using Verse.AI;
using Verse.AI.Group;

/* Generates raids of dnd monsters. This simple implementation just copies a standard raid, but only allows the monster faction */
namespace MoodndBehaviorsAndEvents
{
    public class IncidentWorker_DungeonMonsterRaid : IncidentWorker_RaidEnemy
    {

        // temperature must be safe for beholders, and the raids must be enabled in settings
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return base.CanFireNowSub(parms) && map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(ThingDef.Named("DND_Beholder"))
                && MoodndManagerie_Mod.settings.flagMoodndMonsterRaids;
        }
        
        // Only allow the unique dungeon monster faction
        protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
        {
            return f.def.Equals(FactionDef.Named("DND_DungeonMonsterFaction"));
        }

        protected override bool TryResolveRaidFaction(IncidentParms parms)
        {
            parms.faction = Find.FactionManager.FirstFactionOfDef(FactionDef.Named("DND_DungeonMonsterFaction"));
            return true;
        }

        // todo: a lot of this has faction agnostic checks. It might be worth simplifying and restriction this worker since it's just meant for my custom faction
        /* This is just a copy of vanilla rimworlds's raid executor code, minus some tool/shield specific stuff that causes errors when applied to animals.*/
        protected override bool TryExecuteWorker(IncidentParms parms)
        { 
            List<Pawn> attackingAnimals; 
            if (!base.TryGenerateRaidInfo(parms, out attackingAnimals, false))
            {
                return false;
            }
            TaggedString baseLetterLabel = "DND_MonsterRaidLetterLabel".Translate();
            TaggedString baseLetterText = "DND_MonsterRaidLetterText".Translate();
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(attackingAnimals, ref baseLetterLabel, ref baseLetterText, this.GetRelatedPawnsInfoLetterText(parms), true, true);
            List<TargetInfo> list2 = new List<TargetInfo>(); 
            if (parms.pawnGroups != null)
            {
                List<List<Pawn>> list3 = IncidentParmsUtility.SplitIntoGroups(attackingAnimals, parms.pawnGroups);
                List<Pawn> list4 = list3.MaxBy((List<Pawn> x) => x.Count);
                if (list4.Any<Pawn>())
                { 
                    list2.Add(list4[0]);
                }
                for (int i = 0; i < list3.Count; i++)
                {
                    if (list3[i] != list4 && list3[i].Any<Pawn>())
                    { 
                        list2.Add(list3[i][0]);
                    }
                }
            }
            else if (attackingAnimals.Any<Pawn>())
            {
                foreach (Pawn t in attackingAnimals)
                { 
                    list2.Add(t);
                }
            } 
            base.SendStandardLetter(baseLetterLabel, baseLetterText, this.GetLetterDef(), parms, list2, Array.Empty<NamedArgument>());
            if (parms.controllerPawn == null || parms.controllerPawn.Faction != Faction.OfPlayer)
            {
                parms.raidStrategy.Worker.MakeLords(parms, attackingAnimals);
            }
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;
            parms.target.StoryState.lastRaidFaction = parms.faction;
            return true;
        }
        
        // todo roll for alternate raid strategies
        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("DND_AnimalAttackStrategy");
        }

        public override void ResolveRaidArriveMode(IncidentParms parms)
        {
            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
        }
        
    }
}
