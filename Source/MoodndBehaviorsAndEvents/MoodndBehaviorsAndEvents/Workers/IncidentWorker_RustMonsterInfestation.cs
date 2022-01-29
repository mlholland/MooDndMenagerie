using System;
using Verse;
using RimWorld;

/* A rust monster infestation is a cross between an alphabeaver migration and a vanilla infestation. A group of rust monsters
 * will spawn in your base in an area beneath overhead mountain. The spawning rust monsters will be hungrier than usual, but not agressive
 * unless provoked. Of course, being rust monsters, they'll be problematic to have inside your base.
 */
namespace MoodndBehaviorsAndEvents
{
    public class IncidentWorker_RustMonsterInfestation : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
            return base.CanFireNowSub(parms) && Faction.OfInsects != null && InfestationCellFinder.TryFindCell(out intVec, map)
                && MoodndManagerie_Mod.settings.flagRustMonsterInfestations;
        }
        
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Thing t = SpawnTunnel(map, parms.points);
            base.SendStandardLetter(parms, t, Array.Empty<NamedArgument>());
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }

        public static Thing SpawnTunnel(Map map, float insectsPoints)
        {
            IntVec3 loc = FindRootTunnelLoc(map, false, false);
            if (!loc.IsValid)
            {
                return null;
            }
            ThingWithComps_RustMonsterSpawner rustMonsterSpawner = (ThingWithComps_RustMonsterSpawner)ThingMaker.MakeThing(ThingDef.Named("DND_RustMonsterSpawner"), null);
            rustMonsterSpawner.insectsPoints = insectsPoints;
            Thing thing = GenSpawn.Spawn(rustMonsterSpawner, loc, map, WipeMode.FullRefund);
            return thing;

        }
        
        private static IntVec3 FindRootTunnelLoc(Map map, bool spawnAnywhereIfNoGoodCell = false, bool ignoreRoofIfNoGoodCell = false)
        {
            IntVec3 result;
            if (InfestationCellFinder.TryFindCell(out result, map))
            {
                return result;
            }
            if (!spawnAnywhereIfNoGoodCell)
            {
                return IntVec3.Invalid;
            }
            Func<IntVec3, bool, bool> validator = delegate (IntVec3 x, bool canIgnoreRoof)
            {
                if (!x.Standable(map) || x.Fogged(map))
                {
                    return false;
                }
                if (!canIgnoreRoof)
                {
                    bool flag = false;
                    int num = GenRadial.NumCellsInRadius(3f);
                    for (int i = 0; i < num; i++)
                    {
                        IntVec3 c = x + GenRadial.RadialPattern[i];
                        if (c.InBounds(map))
                        {
                            RoofDef roof = c.GetRoof(map);
                            if (roof != null && roof.isThickRoof)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        return false;
                    }
                }
                return true;
            };
            if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => validator(x, false), map, out result))
            {
                return result;
            }
            if (ignoreRoofIfNoGoodCell && RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => validator(x, true), map, out result))
            {
                return result;
            }
            return IntVec3.Invalid;
        }
    }
}