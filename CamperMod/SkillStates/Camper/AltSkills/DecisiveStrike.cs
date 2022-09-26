using CamperMod.Modules;
using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CamperMod.SkillStates
{
    public class DecisiveStrike : BaseSkillState
    {
        public static string hitboxName = "DecisiveStrikeHitbox";
        public static float baseDuration = 0.8f;
        public static float damageCoefficient = StaticValues.decisiveStrikeDamageCoefficient;
        public static float basePushForce = 350f;
        public static float healPercentage = 0.15f;

        private GameObject targetEnemy;
        private Vector3 moveDirection;
        private Vector3 enemyPosition;
        private float duration;
        private float pushForce;
        private float stopwatch;
        private float damage;
        private float cachedEnemyHealth;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = DecisiveStrike.baseDuration / this.attackSpeedStat;
            this.pushForce = DecisiveStrike.basePushForce * this.damageStat;
            this.damage = damageCoefficient * this.damageStat;

            HurtBox targetedHurtbox = base.GetComponent<DSTracker>().GetTrackingTarget();

            if (targetedHurtbox)
            {
                // Small stun at start
                targetedHurtbox.healthComponent.TakeDamage(new DamageInfo
                {
                    damageType = DamageType.Stun1s,
                    damage = 0f,
                });

                base.gameObject.layer = LayerIndex.fakeActor.intVal;
                base.characterMotor.Motor.RebuildCollidableLayers();
                base.characterMotor.Motor.ForceUnground();

                this.cachedEnemyHealth = targetedHurtbox.healthComponent.health;
                this.targetEnemy = targetedHurtbox.healthComponent.gameObject;
                this.moveDirection = (this.targetEnemy.transform.position - base.transform.position).normalized;
                this.enemyPosition = targetEnemy.transform.position - new Vector3(this.moveDirection.x * 3f, this.moveDirection.y, this.moveDirection.z * 3f);
            }

            base.PlayAnimation("FullBody, Override", "Kick", "Kick.playbackRate", this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.isGrounded || targetEnemy != null)
            {
                base.characterDirection.forward = this.moveDirection;
                base.characterMotor.velocity = Vector3.zero;
            }

            if (targetEnemy != null)
            {
                stopwatch += Time.fixedDeltaTime * 2f;

                if (base.isAuthority)
                {
                    // Freeze enemy
                    CharacterMotor enemyMotor = this.targetEnemy.GetComponent<CharacterMotor>();
                    if (enemyMotor) enemyMotor.velocity = Vector3.zero;

                    // Teleport to enemy
                    Vector3 newPosition = Vector3.Lerp(base.transform.position, this.enemyPosition, this.stopwatch / this.duration);
                    base.characterMotor.Motor.SetPosition(newPosition);
                }
            }

            // Fire attack after elapsed time
            if (base.fixedAge >= this.duration)
            {
                if (FireAttack())
                {
                    base.AddRecoil(5f, 8f, 5f, 8f);
                    AkSoundEngine.PostEvent("DecisiveStrike", base.gameObject);

                    if (NetworkServer.active)
                    {
                        if (base.healthComponent) base.healthComponent.Heal(this.damage * DecisiveStrike.healPercentage, new ProcChainMask());
                    }
                }

                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            if (this.damage >= this.cachedEnemyHealth) base.characterBody.skillLocator.special.AddOneStock();

            base.OnExit();
        }

        private bool FireAttack()
        {
            // Set up attack
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == DecisiveStrike.hitboxName);
            }

            OverlapAttack attack = new OverlapAttack();
            attack.damageType = DamageType.Generic;
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = base.GetTeam();
            attack.damage = this.damage;
            attack.procCoefficient = 1f;
            attack.hitEffectPrefab = null;
            attack.forceVector = base.characterDirection.forward;
            attack.pushAwayForce = this.pushForce;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = base.RollCrit();
            attack.impactSound = new NetworkSoundEventIndex();
            return attack.Fire();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(base.characterDirection.forward);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            base.characterDirection.forward = reader.ReadVector3();
        }
    }
}
