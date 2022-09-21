using UnityEngine;

namespace CamperMod.SkillStates
{
    public class WinterFirecracker : BaseFirecrackerState
    {
        public override void OnEnter()
        {
            this.damageType = RoR2.DamageType.Freeze2s;
            this.dropSound = "FirecrackerDrop";
            this.explodeSound = "WinterExplode";
            this.explosionEffect = Modules.Assets.winterExplosion;
            this.firecrackerPrefab = Modules.Assets.winterFirecrackerMesh;

            this.enemyDamageCoefficient = Modules.StaticValues.winterFirecrackerDamageCoefficient;
            this.playerDamageCoefficient = 0.1f;
            this.procCoefficient = 1f;

            this.blastRadius = 15f;

            this.airForce = 8f;
            this.groundForce = 1f;

            this.buffsToApply = new RoR2.BuffDef[] { };
            this.buffsDuration = 3f;

            base.OnEnter();
        }
    }
}