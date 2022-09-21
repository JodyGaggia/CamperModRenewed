using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace CamperMod.SkillStates
{
    public class BaseFirecrackerState : BaseSkillState
    {
        private static float distanceAboveGround = 0.5f;
        private static float maxDropDistance = 3f;
        private static float distanceBelowPlayerWhenAirborne = 2f;

        protected DamageType damageType = RoR2.DamageType.Generic;
        protected string dropSound = "FirecrackerDrop";
        protected string explodeSound = "FirecrackerExplode";
        protected GameObject explosionEffect = Modules.Assets.firecrackerExplosion;
        protected GameObject firecrackerPrefab = Modules.Assets.firecrackerMesh;

        protected float enemyDamageCoefficient = 10f;
        protected float playerDamageCoefficient = 0.2f;
        protected float procCoefficient = 1f;

        protected float baseDuration = 0.8f;
        protected float blastRadius = 20f;
        
        protected float airForce = 10f;
        protected float groundForce = 2f;

        protected RoR2.BuffDef[] buffsToApply;
        protected float buffsDuration = 3f;

        private Vector3 dropPos;
        protected GameObject firecracker;
        protected float damageToPlayer;
        protected float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = this.baseDuration / this.attackSpeedStat;
            this.damageToPlayer = base.healthComponent.fullHealth * this.playerDamageCoefficient;
            this.dropPos = GetDropPos();

            if(this.dropPos == Vector3.zero) this.outer.SetNextStateToMain();

            ClientScene.RegisterPrefab(firecracker);

            // Spawn a firecracker GameObject
            this.firecracker = GameObject.Instantiate(firecrackerPrefab, dropPos, this.transform.rotation * Quaternion.Euler(90f, 0f, 0f));
            AkSoundEngine.PostEvent(dropSound, firecracker);

            base.PlayAnimation("Gesture, Override", "DropFirecracker", "DropFirecracker.playbackRate", this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(this.fixedAge >= this.duration)
            {
                ExplodeFirecracker(damageType);
                ApplyDamageAndForceToPlayer();
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            ApplyBuffs(buffsToApply, buffsDuration);
            base.OnExit();
        }

        private void ExplodeFirecracker(DamageType damageType)
        {
            if (base.isAuthority)
            {
                // Damage enemies
                BlastAttack blast = new BlastAttack();
                blast.radius = this.blastRadius;
                blast.procCoefficient = this.procCoefficient;
                blast.position = firecracker.transform.position;
                blast.attacker = base.gameObject;
                blast.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blast.baseDamage = base.characterBody.damage * this.enemyDamageCoefficient;
                blast.falloffModel = BlastAttack.FalloffModel.None;
                blast.damageType = damageType;
                blast.baseForce = 800f;
                blast.teamIndex = TeamComponent.GetObjectTeam(blast.attacker);
                blast.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blast.Fire();
            }

            // Effects
            AkSoundEngine.PostEvent(explodeSound, firecracker);
            EffectManager.SimpleEffect(explosionEffect, firecracker.transform.position, Quaternion.identity, true);

            // Destroy firecracker after explosion
            Destroy(this.firecracker);
        }

        private void ApplyDamageAndForceToPlayer()
        {
            Vector3 relativePosToFirecracker = this.transform.position - firecracker.transform.position;
            float distanceToFirecracker = relativePosToFirecracker.magnitude;

            // If player is in blast radius, apply force and take damage
            if (distanceToFirecracker < this.blastRadius)
            {
                // Create force info
                var direction = relativePosToFirecracker / distanceToFirecracker;
                float force = base.characterBody.characterMotor.isGrounded ? this.groundForce : this.airForce;
                float mass = base.characterBody.characterMotor ? base.characterBody.characterMotor.mass : 1f;
                float acceleration = base.characterBody.acceleration;
                float ySpeed = Trajectory.CalculateInitialYSpeedForHeight(force, -acceleration);

                if (base.healthComponent)
                {
                    if (NetworkServer.active)
                    {
                        // Create damage info
                        DamageInfo damageToTake = new DamageInfo()
                        {
                            damage = this.damageToPlayer,
                            crit = false,
                            inflictor = null,
                            attacker = null,
                            position = base.transform.position,
                            force = Vector3.zero,
                            rejected = false,
                            procChainMask = default(ProcChainMask),
                            procCoefficient = 0f,
                            damageType = DamageType.NonLethal,
                            damageColorIndex = DamageColorIndex.Default,
                            dotIndex = DotController.DotIndex.None
                        };
                        base.healthComponent.TakeDamage(damageToTake);
                    }
                }

                // Apply force to player
                if (base.isAuthority)
                {
                    if (base.characterBody.characterMotor) base.characterBody.characterMotor.ApplyForce(ySpeed * mass * direction, false, false);
                }
            }
        }

        private void ApplyBuffs(RoR2.BuffDef[] buffs, float duration)
        {
            if (buffs.Length == 0) return;

            foreach(RoR2.BuffDef buff in buffs)
            {
                if(base.characterBody && NetworkServer.active) base.characterBody.AddTimedBuff(buff, duration);
            }
        }

        private Vector3 GetDropPos()
        {
            Vector3 forwardDirection = base.inputBank.moveVector.normalized;

            if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.down), out RaycastHit hit, maxDropDistance)) return hit.point + (Vector3.up * BaseFirecrackerState.distanceAboveGround);

            // Return a position right beneath the player if outside of the max drop distance
            // Also set the duration to 0.05 so that it explodes instantly
            this.duration = 0.05f;
            return base.transform.position + (Vector3.down * distanceBelowPlayerWhenAirborne) + (-forwardDirection * distanceBelowPlayerWhenAirborne);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}