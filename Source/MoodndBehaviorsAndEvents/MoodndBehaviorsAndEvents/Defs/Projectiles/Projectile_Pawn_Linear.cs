﻿using System;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

/* A pawn as a projectile. Used for abilities that involved rapid forced movement (like beholder telepathy) 
 * Based on the FlyingObject_DragonStrike code from RoM: https://github.com/TorannD/TMagic/blob/419cac105fe6f363f2ae60a7ad82657f01e2688e/Source/TMagic/TMagic/FlyingObject_DragonStrike.cs
 * Todo: Simplify this to be just a pawn-thrower and be less... damage-y like the original.
   */
namespace MoodndBehaviorsAndEvents
{
    [StaticConstructorOnStartup]
    public class Projectile_Pawn_Linear : Projectile
    {

        protected new Vector3 origin; 
        protected new Vector3 destination;

        protected float ProjPawnAlt = 11f; //TODO go back to using a normal altitude layer once I figure out what the top-level layer is.

        protected float speed = 15f;
        private bool drafted = false;
        private int verVal = 1;

        protected new int ticksToImpact;

        protected Thing assignedTarget = null;
        protected Thing flyingThing;

        private float distanceToTarget = 0;
        private float damageMultiplier = 1;
        public DamageInfo? impactDamage;

        public bool damageLaunched = true; 
        public bool explosion = false; 
        public int weaponDmg = 0;

        Pawn pawn;

        protected new int StartingTicksToImpact
        {
            get
            {
                int num = Mathf.RoundToInt((this.origin - this.destination).magnitude / (this.speed / 100f));
                bool flag = num < 1;
                if (flag)
                {
                    num = 1;
                }
                return num;
            }
        }

        protected new IntVec3 DestinationCell
        {
            get
            {
                return new IntVec3(this.destination);
            }
        }

        public new virtual Vector3 ExactPosition
        {
            get
            {
                Vector3 b = (this.destination - this.origin) * (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact);
                return this.origin + b + Vector3.up * this.def.Altitude;
            }
        }

        public new virtual Quaternion ExactRotation
        {
            get
            {
                return Quaternion.LookRotation(this.destination - this.origin);
            }
        }

        public override Vector3 DrawPos
        {
            get
            {
                return this.ExactPosition;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector3>(ref this.origin, "origin", default(Vector3), false);
            Scribe_Values.Look<Vector3>(ref this.destination, "destination", default(Vector3), false);
            Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
            Scribe_Values.Look<int>(ref this.verVal, "verVal", 0, false);
            Scribe_Values.Look<float>(ref this.distanceToTarget, "distanceToTarget", 0, false);
            Scribe_Values.Look<float>(ref this.damageMultiplier, "damageMultiplier", 1, false);
            Scribe_Values.Look<bool>(ref this.damageLaunched, "damageLaunched", true, false);
            Scribe_Values.Look<bool>(ref this.explosion, "explosion", false, false);
            Scribe_References.Look<Thing>(ref this.assignedTarget, "assignedTarget", false);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawn", false);
            Scribe_Deep.Look<Thing>(ref this.flyingThing, "flyingThing", new object[0]);
        }

        protected virtual void Initialize()
        {
            if (pawn != null)
            {
                FleckMaker.Static(pawn.TrueCenter(), base.Map, FleckDefOf.ExplosionFlash, 12f);
                SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(pawn.Position, base.Map, false));
                FleckMaker.ThrowDustPuff(pawn.Position, base.Map, Rand.Range(1.2f, 1.8f));
            }
            if (distanceToTarget > 12)
            {
                damageMultiplier = .5f;
            }
            else if (distanceToTarget > 8)
            {
                damageMultiplier = .8f;
            }
            //flyingThing.ThingID += Rand.Range(0, 2147).ToString();
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), targ, flyingThing, null);
        }

        public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing, DamageInfo? newDamageInfo = null)
        {
            bool spawned = flyingThing.Spawned;
            pawn = launcher as Pawn;
            drafted = pawn.Drafted;
            verVal = 1; // todo what is this used for. Can we abstract it out? In RoM it was based on user skill.
            if (spawned)
            {
                flyingThing.DeSpawn();
            }
            // Todo determine if we still need this.
            //ModOptions.Constants.SetPawnInFlight(true);
            //
            this.origin = origin;
            this.impactDamage = newDamageInfo;
            this.flyingThing = flyingThing;
            this.distanceToTarget = (targ.Cell - origin.ToIntVec3()).LengthHorizontal;
            bool flag = targ.Thing != null;
            if (flag)
            {
                this.assignedTarget = targ.Thing;
            }
            this.destination = targ.Cell.ToVector3();
            this.ticksToImpact = this.StartingTicksToImpact;
            this.Initialize();
        }

        public override void Tick()
        {
            //base.Tick();
            Vector3 exactPosition = this.ExactPosition;
            this.ticksToImpact--;
            bool flag = !this.ExactPosition.InBounds(base.Map);
            if (flag)
            {
                this.ticksToImpact++;
                base.Position = this.ExactPosition.ToIntVec3();
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                base.Position = this.ExactPosition.ToIntVec3();
                if (Find.TickManager.TicksGame % 2 == 0)
                {
                    FleckMaker.ThrowDustPuff(base.Position, base.Map, Rand.Range(0.8f, 1.2f));
                }

                //if(Find.TickManager.TicksGame % 10 == 0 && this.assignedTarget != null)
                //{
                //    this.origin = this.ExactPosition;
                //    this.destination = this.assignedTarget.DrawPos;
                //    this.ticksToImpact = this.StartingTicksToImpact;
                //}

                bool flag2 = this.ticksToImpact <= 0;
                if (flag2)
                {
                    bool flag3 = this.DestinationCell.InBounds(base.Map);
                    if (flag3)
                    {
                        base.Position = this.DestinationCell;
                    }
                    this.ImpactSomething();
                }

            }
        }

        public override void Draw()
        {
            bool flag = this.flyingThing != null;
            if (flag)
            {
                bool flag2 = this.flyingThing is Pawn;
                if (flag2)
                {
                    Vector3 arg_2B_0 = this.DrawPos;
                    bool flag4 = !this.DrawPos.ToIntVec3().IsValid;
                    if (flag4)
                    {
                        return;
                    }
                    Pawn pawn = this.flyingThing as Pawn;
                    pawn.Drawer.DrawAt(this.DrawPos);

                }
                else
                {
                    Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.DrawMatSingle, 0);
                }
            }
            else
            {
                Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.DrawMatSingle, 0);
            }
            base.Comps_PostDraw();
        }

        private void DrawEffects(Vector3 pawnVec, Pawn flyingPawn, int magnitude)
        {
            bool flag = !pawn.Dead && !pawn.Downed;
            if (flag)
            {

            }
        }

        protected void ImpactSomething()
        {
            bool flag = this.assignedTarget != null;
            if (flag)
            {
                Pawn targetPawn = this.assignedTarget as Pawn;
                bool flag2 = targetPawn != null && targetPawn.GetPosture() != PawnPosture.Standing && (this.origin - this.destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.1f;
                if (flag2)
                {
                    this.Impact(null);
                }
                else
                {
                    this.Impact(this.assignedTarget);
                }
            }
            else
            {
                this.Impact(null);
            }
        }

        protected void Impact(Thing hitThing)
        {
            bool flag = hitThing == null;
            if (flag)
            {
                Pawn hitPawn;
                bool flag2 = (hitPawn = (base.Position.GetThingList(base.Map).FirstOrDefault((Thing x) => x == this.assignedTarget) as Pawn)) != null;
                if (flag2)
                {
                    hitThing = hitPawn;
                }
            }
            bool hasValue = this.impactDamage.HasValue && hitThing != null && hitThing is Pawn;
            if (hasValue)
            {
                Pawn hitPawn = hitThing as Pawn;
                this.impactDamage.Value.SetAmount(this.impactDamage.Value.Amount * this.damageMultiplier);
                try
                {
                    hitPawn.TakeDamage(this.impactDamage.Value);
                    if (distanceToTarget <= 6f && hitThing is Pawn)
                    {
                        if (!hitPawn.DestroyedOrNull() && !hitPawn.Downed && !hitPawn.Dead)
                        {
                            IntVec3 projectedPosition = hitThing.Position + ((10f - distanceToTarget) * (1 + (.15f * verVal)) * (hitThing.Position.ToVector3() - this.origin).normalized).ToIntVec3();
                            if (projectedPosition.IsValid && projectedPosition.InBounds(hitThing.Map))
                            {
                                LaunchFlyingObect(projectedPosition, hitPawn, Mathf.RoundToInt(10f - distanceToTarget));
                            }
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    // ignore?
                }
            }
            try
            {
                SoundDefOf.Roof_Collapse.PlayOneShot(new TargetInfo(this.Position, this.Map, false));

                GenSpawn.Spawn(this.flyingThing, base.Position, base.Map);
                //ModOptions.Constants.SetPawnInFlight(false);
                Pawn p = this.flyingThing as Pawn;
                // TODO parameterize this behavior
                /*if (p.IsColonist)
                {
                    CameraJumper.TryJumpAndSelect(p);
                    p.drafter.Drafted = this.drafted;
                }*/
                this.Destroy(DestroyMode.Vanish);
            }
            catch
            {
                GenSpawn.Spawn(this.flyingThing, base.Position, base.Map);
                //ModOptions.Constants.SetPawnInFlight(false);
                // TODO parameterize this behavior
                /*Pawn p = this.flyingThing as Pawn;
                if (p.IsColonist)
                {
                    p.drafter.Drafted = this.drafted;
                }*/
                this.Destroy(DestroyMode.Vanish);
            }
        }

        public void LaunchFlyingObect(IntVec3 targetCell, Pawn pawn, int force)
        {
            bool flag = targetCell != null && targetCell != default(IntVec3);
            if (flag)
            {
                if (pawn != null && pawn.Position.IsValid && pawn.Spawned && pawn.Map != null && !pawn.Downed && !pawn.Dead)
                {
                    // todo determine if this is core logic, or flavor/visuals.
                    //FlyingObject_Spinning flyingObject = (FlyingObject_Spinning)GenSpawn.Spawn(ThingDef.Named("FlyingObject_Spinning"), pawn.Position, pawn.Map);
                    //flyingObject.speed = 25 + force;
                    //flyingObject.Launch(pawn, targetCell, pawn);
                }
            }
        }
    }
}
