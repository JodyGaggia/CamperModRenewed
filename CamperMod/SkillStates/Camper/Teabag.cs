using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using RoR2;
using UnityEngine;

namespace CamperMod.SkillStates
{
    public class Teabag : BaseSkillState
    {
        private static float baseDamageCoefficient = Modules.StaticValues.teabagDamageCoefficient;
        private static float extraDamageCoefficient = Modules.StaticValues.teabagRangeDamageCoefficient;
        private static float facingDamageCoefficient = Modules.StaticValues.teabagFacingDamageCoefficient;

        private static float procCoefficient = 0.5f;
        private static float baseDuration = 0.1f;
        private static float force = 10f;
        private static float range = 125f;

        private Animator animator;
        private float damageCoefficient;
        private float duration;
        private float fireTime;
        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();

            // Set variables
            this.duration = Teabag.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.2f * this.duration;
            this.animator = base.GetModelAnimator();
            this.animator.SetBool("attacking", true);

            // Initiate aim timer
            base.characterBody.SetAimTimer(1f);

            // Play sound and animation
            Util.PlaySound(FirePistol2.firePistolSoundString, base.gameObject);
            base.PlayAnimation("FullBody, Override", "Crouch", "Crouch.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            this.animator.SetBool("attacking", false);
            base.OnExit();
        }

        private void Fire()
        {
            if (!this.hasFired)
            {
                // Prevent further firing
                this.hasFired = true;

                // Add bloom and get aim ray
                base.characterBody.AddSpreadBloom(1.5f);
                Ray aimRay = base.GetAimRay();

                // Get damage coefficient
                this.damageCoefficient = GetDamageCoefficient(aimRay);

                // Fire bullet
                new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = this.damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = Teabag.range,
                    force = Teabag.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 0f,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = "",
                    smartCollision = false,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.75f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = null,
                    spreadPitchScale = 0f,
                    spreadYawScale = 0f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = null,
                }.Fire();

            }
        }

        private float GetDamageCoefficient(Ray ray)
        {
            // Default coefficient
            float coeff = baseDamageCoefficient;

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerIndex.entityPrecise.mask)) // Raycast using aim ray
            {
                // Attempt to grab enemy body
                HurtBox enemyHurtbox = hit.transform.gameObject.GetComponent<HurtBox>();
                CharacterBody enemyBody = enemyHurtbox.healthComponent.gameObject.GetComponent<CharacterBody>();

                // Weight damage based on distance
                float weightDistance = Mathf.Clamp(hit.distance / Teabag.range, 0f, 1f);

                if (enemyBody)
                {
                    // For backstab
                    Vector3 corePosToHit = base.characterBody.corePosition - hit.point;

                    // If enemy is facing player, deal more damage
                    if (BackstabManager.IsBackstab(-corePosToHit, enemyBody)) coeff = baseDamageCoefficient + (Teabag.extraDamageCoefficient * weightDistance);
                    else coeff = baseDamageCoefficient + facingDamageCoefficient + (Teabag.extraDamageCoefficient * weightDistance);

                }
            }

            return coeff;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime && base.isAuthority) this.Fire();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}