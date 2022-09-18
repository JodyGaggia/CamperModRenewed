using EntityStates;
using RoR2;
using UnityEngine;

namespace CamperMod.SkillStates
{
    public class BaseHealState : BaseSkillState
    {
        protected float hptCoefficient = 0.003f;
        protected float duration = 3f;
        protected float soundInterval = 1f;
        protected bool isFragile = false;

        private Animator animator;
        private float healAmount;
        private float timeSinceLastHit;
        private float stopwatch;

        public override void OnEnter()
        {
            base.OnEnter();

            base.characterBody.isSprinting = false;
            this.healAmount = base.healthComponent.fullHealth * this.hptCoefficient;

            this.animator = base.GetModelAnimator();
            this.animator.SetBool("isHealing", true);
            base.PlayAnimation("FullBody, Override", "Heal");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= this.soundInterval)
            {
                Util.PlaySound("Heal", base.gameObject);
                this.stopwatch = 0f;
            }

            if (base.isAuthority && this.isFragile)
            {
                this.timeSinceLastHit = this.healthComponent.timeSinceLastHit;

                if (this.timeSinceLastHit <= base.fixedAge || base.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            else if (base.isAuthority && base.inputBank.skill4.down && base.fixedAge > Modules.StaticValues.keyLiftGrace)
            {
                this.outer.SetNextStateToMain();
                return;
            }

            base.healthComponent.Heal(this.healAmount, default(ProcChainMask), true);
        }

        public override void OnExit()
        {
            base.PlayAnimation("Fullbody, Override", "Heal", "Heal.playbackRate", 0.05f);
            this.animator.SetBool("isHealing", false);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}