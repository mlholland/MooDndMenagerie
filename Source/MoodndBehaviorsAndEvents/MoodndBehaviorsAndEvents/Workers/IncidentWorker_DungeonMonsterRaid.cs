using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

/* Generates raids of dnd monsters. This simple implementation just copies a standard raid, but only allows the monster faction */
namespace MoodndBehaviorsAndEvents
{
    public class IncidentWorker_DungeonMonsterRaid : IncidentWorker_RaidEnemy
    {

        // temperature must be safe for beholders, and the raids must be enabled in settings
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
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

        // todo roll for alternate raid strategies
        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
        }

        public override void ResolveRaidArriveMode(IncidentParms parms)
        {
            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
        }
        
    }
}
