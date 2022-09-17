using EntityStates;
using CamperMod.Modules;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
using RoR2.Audio;

namespace CamperMod.SkillStates
{
    public class SprintBurst : BaseSkillState
    {
        public static string hitboxName = "SpinBox";
        public static float sprintMultiplier = 3f;
        public static float sprintFOV = 100f;
        public static float sprintTurnSpeed = 360f;
        public static float duration = 4f;
        public static float maxMoonwalkStacks = 4f;
        public static float damageCoefficient = 4f;
        public static float procCoefficient = 0.2f;

        private Animator animator;
        private List<GameObject> effectList = new List<GameObject> { Assets.goodEffect, Assets.greatEffect };
        private OverlapAttack attack;
        private NetworkSoundEventIndex impactSound;
        private float charYaw;
        private float lastYaw;
        private float stopwatch;
        private float moonwalkInterval;

        public override void OnEnter()
        {
            base.OnEnter();

            // Set variables
            this.animator = base.GetModelAnimator();
            this.lastYaw = this.characterDirection.yaw;
            this.moonwalkInterval = SprintBurst.duration / (SprintBurst.maxMoonwalkStacks + 1f);

            // Increase turnspeed
            this.characterDirection.turnSpeed = SprintBurst.sprintTurnSpeed;

            // FOV
            if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = Mathf.Lerp(60f, SprintBurst.sprintFOV, 0.1f);
        }

        public override void OnExit()
        {
            // Reset camera and turnspeed
            if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = -1f;
            base.characterDirection.turnSpeed = Modules.StaticValues.turnSpeed;

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                this.charYaw += this.characterDirection.yaw - this.lastYaw;
                this.lastYaw = this.characterDirection.yaw;

                // Increase speed
                Vector3 velocity = this.inputBank.moveVector * this.moveSpeedStat * SprintBurst.sprintMultiplier;
                velocity.y = this.characterMotor.velocity.y;
                this.characterMotor.velocity = velocity;

                if (Mathf.Abs(this.charYaw) > 270f)
                {
                    EffectManager.SimpleEffect(Assets.spinEffect, this.transform.position, Quaternion.identity, true);
                    this.charYaw = 0f;
                    FireAttack();
                }

                float movementAngle = Vector3.Angle(this.characterDirection.forward, this.characterMotor.velocity);
                if (movementAngle > 100f)
                {
                    this.stopwatch += Time.fixedDeltaTime;
                    if (this.stopwatch >= this.moonwalkInterval)
                    {
                        base.characterBody.AddTimedBuff(Buffs.spinBuff, 4f);
                        base.characterBody.RecalculateStats();

                        if (base.cameraTargetParams) base.cameraTargetParams.AddRecoil(2f, 5f, 2f, 5f);

                        EffectManager.SimpleEffect(this.effectList[Random.Range(0, this.effectList.Count)], this.transform.position, Quaternion.identity, true);
                        this.stopwatch = 0f;
                    }
                }
                else this.stopwatch = 0f;

                // Exit if attacking, util button pressed or no movement input (ugly)
                if ((animator.GetBool("attacking") || base.inputBank.skill3.down && base.fixedAge >= Modules.StaticValues.keyLiftGrace || base.inputBank.moveVector == Vector3.zero || base.fixedAge >= SprintBurst.duration))
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        private void FireAttack()
        {
            HitBoxGroup hitboxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitboxGroup = System.Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == SprintBurst.hitboxName);
            }

            this.attack = new OverlapAttack();
            this.attack.damageType = DamageType.Stun1s;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = SprintBurst.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = SprintBurst.procCoefficient;
            this.attack.hitEffectPrefab = null;
            this.attack.forceVector = Vector3.up;
            this.attack.pushAwayForce = 800f;
            this.attack.hitBoxGroup = hitboxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;
            this.attack.Fire();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

    }
}