using Verse;
using Verse.Sound;
using RimWorld;
using UnityEngine;

/* A pawn as a projectile. Used for abilities that involved rapid forced movement (like beholder telepathy) 
   Based on the FlyingObject_DragonStrike code from RoM: https://github.com/TorannD/TMagic/blob/419cac105fe6f363f2ae60a7ad82657f01e2688e/Source/TMagic/TMagic/FlyingObject_DragonStrike.cs*/
namespace MoodndBehaviorsAndEvents
{
    [StaticConstructorOnStartup]
    public class Projectile_Pawn_Suspend : Projectile_Pawn_Linear
    {
        public int riseTicksRemaining = 60;
        public int hoverTicksRemaining = 300;
        public float hoverHeight = 3;
        public float hoverSwayVerticalAmount = .2f;
        public int hoverSwayVerticalInterval = 75;
        public float hoverSwayHorizontalAmount = .4f;
        public int hoverSwayHorizonalInterval = 180;

        private int riseTicksStart;



        protected override void Initialize()
        {
            riseTicksStart = riseTicksRemaining;
            base.Initialize();
        }

        public override Vector3 ExactPosition
        {
            get
            {
                if (this.riseTicksRemaining > 0)
                {
                    return this.origin + (Vector3.forward * hoverHeight * (1f - (float)this.riseTicksRemaining / (float)this.riseTicksStart)) + Vector3.up * this.ProjPawnAlt;
                }
                else if (hoverTicksRemaining > 0)
                {
                    return this.origin 
                        + (Vector3.forward * hoverHeight) 
                        + (Vector3.right * hoverSwayHorizontalAmount * Mathf.Cos((float)(this.hoverTicksRemaining % this.hoverSwayHorizonalInterval) / (this.hoverSwayHorizonalInterval / (2 * Mathf.PI))))
                        + (Vector3.forward * hoverSwayVerticalAmount * Mathf.Cos((float)(this.hoverTicksRemaining % this.hoverSwayVerticalInterval) / (this.hoverSwayVerticalInterval / (2 * Mathf.PI))))
                        + Vector3.up * this.ProjPawnAlt;
                }
                else
                {
                    Vector3 b = (this.destination - (this.origin + Vector3.forward * hoverHeight)) * (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact);
                    return this.origin + b + (Vector3.forward * hoverHeight) + Vector3.up * this.ProjPawnAlt;
                }
            }
        }

        public override Quaternion ExactRotation
        {
            get
            {
                return Quaternion.LookRotation(this.destination - this.origin);
            }
        }

        public override void Tick()
        {
            //base.Tick();

            if (riseTicksRemaining > 0)
            {
                riseTicksRemaining--;
            } else if (hoverTicksRemaining > 0)
            {
                hoverTicksRemaining--;
                if (hoverTicksRemaining == 0)
                {
                    SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(this.Position, this.Map, false));
                } else if (hoverTicksRemaining % 60 == 0 )
                {
                    SoundDefOf.Ambient_AltitudeWind.sustainFadeoutTime.Equals(30.0f);
                }
            } else
            {
                ticksToImpact--;
            }
            Vector3 exactPosition = this.ExactPosition;
            bool flag = !exactPosition.InBounds(base.Map);
            if (flag)
            {
                this.ticksToImpact++;
                base.Position = exactPosition.ToIntVec3();
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                base.Position = exactPosition.ToIntVec3();
                if (Find.TickManager.TicksGame % 5 == 0)
                {
                    FleckMaker.ThrowDustPuff(base.Position, base.Map, Rand.Range(0.8f, 1.2f));
                }

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
    }
}
