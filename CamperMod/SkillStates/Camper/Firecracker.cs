using RoR2;
using UnityEngine;

namespace CamperMod.SkillStates
{
    public class Firecracker : BaseFirecrackerState
    {
        public override void OnEnter()
        {
            this.damageType = RoR2.DamageType.PercentIgniteOnHit;
            this.dropSound = "FirecrackerDrop";
            this.explodeSound = "FirecrackerExplode";
            this.explosionEffect = Modules.Assets.firecrackerExplosion;
            this.firecrackerPrefab = Modules.Assets.firecrackerMesh;

            this.enemyDamageCoefficient = Modules.StaticValues.firecrackerDamageCoefficient;
            this.playerDamageCoefficient = 0.2f;
            this.procCoefficient = 0.8f;

            this.blastRadius = 20f;

            this.airForce = 12.5f;
            this.groundForce = 2f;

            this.buffsToApply = new RoR2.BuffDef[] { };
            this.buffsDuration = 3f;

            base.OnEnter();
        }
    }
}