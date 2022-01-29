using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

/* A modified vanilla infestation hive spawner. Instead of a vanilla hive, this produces a group of rust monsters, and no hive.
*/
namespace MoodndBehaviorsAndEvents
{
    [StaticConstructorOnStartup]
    public class ThingWithComps_RustMonsterSpawner : ThingWithComps
    {
        private static int RUST_MONSTER_SPAWN_LIMIT = 300;


        // TODO figure out how to patch a call of this during startup like other static data resetters (see playDataLoader)
        public static void ResetStaticData()
        {
            ThingWithComps_RustMonsterSpawner.filthTypes.Clear();
            ThingWithComps_RustMonsterSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
            ThingWithComps_RustMonsterSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
            ThingWithComps_RustMonsterSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
            ThingWithComps_RustMonsterSpawner.filthTypes.Add(ThingDefOf.Filth_RubbleRock);
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.secondarySpawnTick, "secondarySpawnTick", 0, false);
            Scribe_Values.Look<float>(ref this.insectsPoints, "insectsPoints", 0f, false);
            Scribe_Values.Look<bool>(ref this.spawnedByInfestationThingComp, "spawnedByInfestationThingComp", false, false);
        }
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.secondarySpawnTick = Find.TickManager.TicksGame + this.ResultSpawnDelay.RandomInRange.SecondsToTicks();
            }
            this.CreateSustainer();
        }
        
        public override void Tick()
        {
            if (base.Spawned)
            {
                this.sustainer.Maintain();
                Vector3 vector = base.Position.ToVector3Shifted();
                IntVec3 c;
                if (Rand.MTBEventOccurs(ThingWithComps_RustMonsterSpawner.FilthSpawnMTB, 1f, 1.TicksToSeconds()) 
                    && CellFinder.TryFindRandomReachableCellNear(base.Position, base.Map, ThingWithComps_RustMonsterSpawner.FilthSpawnRadius, 
                    TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false, false, false), null, null, out c, 999999))
                {
                    FilthMaker.TryMakeFilth(c, base.Map, ThingDefOf.Filth_Dirt, 1, FilthSourceFlags.None);
                }
                if (Rand.MTBEventOccurs(ThingWithComps_RustMonsterSpawner.DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
                {
                    FleckMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    }, base.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
                }
                if (this.secondarySpawnTick <= Find.TickManager.TicksGame)
                {
                    this.sustainer.End();
                    Map map = base.Map;
                    IntVec3 position = base.Position;
                    this.Destroy(DestroyMode.Vanish);
                    Spawn(map, position);
                }
            }
        }

        protected virtual void Spawn(Map map, IntVec3 loc)
        {
            float pointsLeft = Math.Max(this.insectsPoints, 1); //min on 1 to ensure at least one rust monster spawning
            List<Pawn> list = new List<Pawn>();
            int num = 0; 
            while (pointsLeft > 0f)
            { 
                num++;
                if (num > RUST_MONSTER_SPAWN_LIMIT) 
                {
                    Log.Error(String.Format("MooDnd ThingWithComps_RustMonsterSpawner: Too many iterations. Tried to spawn over {0} rust mosnters", RUST_MONSTER_SPAWN_LIMIT));
                    break;
                }
                Pawn pawn = PawnGenerator.GeneratePawn(RustMonsterKindDef);
                GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(loc, map, 2, null), map, WipeMode.Vanish);
                list.Add(pawn);
                pointsLeft -= RustMonsterKindDef.combatPower;
            }
        }
        
        public override void Draw()
        {
            Rand.PushState();
            Rand.Seed = this.thingIDNumber;
            for (int i = 0; i < 6; i++)
            {
                this.DrawDustPart(Rand.Range(0f, 360f), Rand.Range(0.9f, 1.1f) * (float)Rand.Sign * 4f, Rand.Range(1f, 1.5f));
            }
            Rand.PopState();
        }

        // Token: 0x06006747 RID: 26439 RVA: 0x002310C0 File Offset: 0x0022F2C0
        private void DrawDustPart(float initialAngle, float speedMultiplier, float scale)
        {
            float num = (Find.TickManager.TicksGame - this.secondarySpawnTick).TicksToSeconds();
            Vector3 pos = base.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Filth);
            pos.y += 0.04054054f * Rand.Range(0f, 1f);
            Color value = new Color(0.47058824f, 0.38431373f, 0.3254902f, 0.7f);
            ThingWithComps_RustMonsterSpawner.matPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0f, initialAngle + speedMultiplier * num, 0f), Vector3.one * scale);
            Graphics.DrawMesh(MeshPool.plane10, matrix, ThingWithComps_RustMonsterSpawner.TunnelMaterial, 0, null, 0, ThingWithComps_RustMonsterSpawner.matPropertyBlock);
        }
        
        private void CreateSustainer()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                SoundDef tunnel = SoundDefOf.Tunnel;
                this.sustainer = tunnel.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
            });
        }
        
        private int secondarySpawnTick;
        
        public bool spawnHive = true;
        
        public float insectsPoints;
        
        public bool spawnedByInfestationThingComp;
        
        private Sustainer sustainer;
        
        private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();
        
        private readonly FloatRange ResultSpawnDelay = new FloatRange(26f, 30f);
        
        [TweakValue("Gameplay", 0f, 1f)]
        private static float DustMoteSpawnMTB = 0.2f;
        
        [TweakValue("Gameplay", 0f, 1f)]
        private static float FilthSpawnMTB = 0.3f;
        
        [TweakValue("Gameplay", 0f, 10f)]
        private static float FilthSpawnRadius = 3f;
        
        private static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);
        
        private static List<ThingDef> filthTypes = new List<ThingDef>();

        private static readonly PawnKindDef RustMonsterKindDef = PawnKindDef.Named("DND_RustMonster");
    }
}
