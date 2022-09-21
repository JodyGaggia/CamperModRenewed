using RoR2;

namespace CamperMod.SkillStates.Camper.AltSkills.Flashbang
{
    public class FlashbangFirecracker : BaseFirecrackerState
    {
        public override void OnEnter()
        {
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

            buffsToApply = new BuffDef[] { RoR2Content.Buffs.Cloak };
            buffsDuration = 3f;

            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            Log.Debug("test");
        }
    }
}