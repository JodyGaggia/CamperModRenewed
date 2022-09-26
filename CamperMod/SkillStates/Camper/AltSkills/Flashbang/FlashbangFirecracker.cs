using EntityStates;
using RoR2;

namespace CamperMod.SkillStates.Camper.AltSkills.Flashbang
{
    public class FlashbangFirecracker : BaseFirecrackerState
    {
        public override void OnEnter()
        {
            Log.Debug("FlashbangFirecracker: OnEnter");
            damageType = DamageType.Stun1s;
            dropSound = "FlashbangDrop";
            explodeSound = "FlashbangExplode";
            explosionEffect = Modules.Assets.flashbangExplosion;
            firecrackerPrefab = Modules.Assets.flashbangMesh;

            enemyDamageCoefficient = Modules.StaticValues.flashbangFirecrackerDamageCoefficient;
            playerDamageCoefficient = 0f;
            procCoefficient = 1f;

            blastRadius = 10f;

            airForce = 5f;
            groundForce = 1f;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            Log.Debug("FlashbangFirecracker: OnExit");
            this.outer.SetNextState(new FlashbangStealthMode());
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}