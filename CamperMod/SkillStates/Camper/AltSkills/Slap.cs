using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using RoR2;
using UnityEngine;

namespace CamperMod.SkillStates
{
    public class Slap : BaseSkillState
    {
        public int swingIndex;
        public static string hitboxName = "Slap";
        public static float damageCoefficient = Modules.StaticValues.palletSmashDamageCoefficient;
        public static float procCoefficient = 0.8f;
        public static float baseDuration = 0.4f;
        public static float force = 10f;

        private Animator animator;
        private OverlapAttack attack;
        private float duration;
        private float fireTime;
        private float earlyExitTime;
        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();

            // Set variables
            this.duration = Slap.baseDuration / this.attackSpeedStat;
            this.fireTime = 1f * this.duration;
            this.animator = base.GetModelAnimator();
            this.animator.SetBool("attacking", true);
            this.earlyExitTime = this.duration * 0.8f;

            // Initiate aim timer
            base.characterBody.SetAimTimer(2f);
            //this.childLocator = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
            //this.childLocator.FindChild("PalletModel").gameObject.SetActive(true);

            // Set up attack
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = System.Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == DeadHard.hitboxName);
            }

            this.attack = new OverlapAttack();
            this.attack.damageType = DamageType.Generic;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = Slap.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = Slap.procCoefficient;
            this.attack.hitEffectPrefab = null;
            this.attack.forceVector = DeadHard.force;
            this.attack.pushAwayForce = Slap.force;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = new RoR2.Audio.NetworkSoundEventIndex();

            if (swingIndex == 0) base.PlayCrossfade("Gesture, Override", "SlapLeft", "Slap.playbackRate", this.duration, 0.1f);
            else base.PlayCrossfade("Gesture, Override", "SlapRight", "Slap.playbackRate", this.duration, 0.1f);
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

                if (this.attack.Fire()) Util.PlaySound("Slap", base.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime && base.isAuthority) this.Fire();

            if (base.fixedAge >= this.earlyExitTime && base.isAuthority && base.inputBank.skill1.down)
            {
                if (!this.hasFired) this.Fire();
                this.SetNextState();
                return;
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        protected virtual void SetNextState()
        {
            int index = this.swingIndex;
            if (index == 0) index = 1;
            else index = 0;

            this.outer.SetNextState(new Slap
            {
                swingIndex = index
            });
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}