using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CamperMod.SkillStates
{
    public class DeadHard : BaseSkillState
    {
        public static string hitboxName = "Bash";
        public static float basePushForce = 285f;
        public static float maxPushForce = 8000f;
        public static float baseDuration = 0.5f;
        public static float speedUpperCoefficient = 6f;
        public static float speedLowerCoefficient = 2f;
        public static float damageCoefficient = 10f;
        public static float baseHitPauseTimer = 0.12f;
        public static Vector3 force = Vector3.up;

        private Vector3 forwardDirection;
        private HitStopCachedState hitStopCachedState;
        private OverlapAttack attack;
        private Animator animator;
        private NetworkSoundEventIndex impactSound;
        private float duration;
        private float antiBounceDuration;
        private float hitPauseTimer;
        private float pushForce;
        private bool inHitPause = false;

        public override void OnEnter()
        {
            base.OnEnter();

            // Get forward direction
            if (base.isAuthority && base.inputBank && base.characterDirection)
            {
                this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            }

            // Set variables
            this.animator = base.GetModelAnimator();
            this.duration = DeadHard.baseDuration / this.attackSpeedStat;
            this.antiBounceDuration = this.duration / 3f;
            this.pushForce = DeadHard.basePushForce * this.damageStat;
            if (this.pushForce > DeadHard.maxPushForce) this.pushForce = DeadHard.maxPushForce;
            base.characterMotor.velocity.y = 0f;

            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.characterMotor.Motor.ForceUnground();

            // Animations
            base.PlayAnimation("FullBody, Override", "DeadHard", "DeadHard.playbackRate", this.duration);

            // Invincibility
            if (NetworkServer.active) base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, this.duration);

            // Set up attack
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == DeadHard.hitboxName);
            }

            this.attack = new OverlapAttack();
            this.attack.damageType = DamageType.Stun1s;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = DeadHard.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = 1f;
            this.attack.hitEffectPrefab = null;
            this.attack.forceVector = DeadHard.force;
            this.attack.pushAwayForce = this.pushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Reduce timers
            this.antiBounceDuration -= Time.fixedDeltaTime;
            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (base.isAuthority)
            {
                // Force character to face initial direction
                base.characterDirection.forward = this.forwardDirection;

                // Create and consume hitpauses
                if (this.attack.Fire())
                {
                    if (!inHitPause)
                    {
                        EffectManager.SimpleMuzzleFlash(Modules.Assets.deadHardHit, base.gameObject, "DeadHardEffect", true);
                        this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "DeadHard.playbackRate");
                        this.hitPauseTimer = DeadHard.baseHitPauseTimer / this.attackSpeedStat;
                        this.inHitPause = true;
                    }
                }

                if (this.hitPauseTimer <= 0 && this.inHitPause)
                {
                    base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                    this.inHitPause = false;
                }

                // Move character forward
                if (!this.inHitPause)
                {
                    if (base.characterMotor && base.characterDirection)
                    {
                        Vector3 velocity = this.forwardDirection * this.moveSpeedStat * Mathf.Lerp(DeadHard.speedUpperCoefficient, DeadHard.speedLowerCoefficient, base.age / this.duration);
                        if (antiBounceDuration > 0) velocity.y = 0f;
                        else velocity.y = base.characterMotor.velocity.y;
                        base.characterMotor.velocity = velocity;
                    }
                }
                else // If in hitpause freeze character
                {
                    base.characterMotor.velocity = Vector3.zero;
                    this.animator.SetFloat("DeadHard.playbackRate", 0f);
                }
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }

        }

        public override void OnExit()
        {
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            base.OnExit();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.forwardDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.forwardDirection = reader.ReadVector3();
        }
    }
}