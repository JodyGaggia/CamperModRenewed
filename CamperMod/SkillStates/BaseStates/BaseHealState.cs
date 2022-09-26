using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CamperMod.SkillStates
{
    public class BaseHealState : BaseSkillState
    {
        protected float hpsCoefficient = 0.003f;
        protected float soundHealInterval = 1f;

        private Animator animator;
        private float healAmount;
        private float stopwatch;

        public override void OnEnter()
        {
            base.OnEnter();

            base.characterBody.isSprinting = false;
            this.healAmount = base.healthComponent.fullHealth * this.hpsCoefficient;

            this.animator = base.GetModelAnimator();
            this.animator.SetBool("isHealing", true);
            base.PlayAnimation("FullBody, Override", "Heal");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= this.soundHealInterval)
            {
                AkSoundEngine.PostEvent("Heal", base.gameObject); // good enough

                if (NetworkServer.active && base.healthComponent) base.healthComponent.Heal(this.healAmount, default(ProcChainMask), true);

                this.stopwatch = 0f;
            }
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