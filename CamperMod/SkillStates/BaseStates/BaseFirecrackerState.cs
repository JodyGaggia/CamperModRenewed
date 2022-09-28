using EntityStates;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace CamperMod.SkillStates
{
    public class BaseFirecrackerState : BaseSkillState
    {
        public static float distanceAboveGroundOffset = 0.5f;
        public static float maxDropDistance = Modules.StaticValues.firecrackerInstantExplodeHeight;
        public static float distanceAwayFromPlayerOffset = 2f;

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

        private Vector3 spawnPos;
        private Vector3 immediateGroundBeneath;
        private GameObject firecracker;
        private float damageToPlayer;
        private float minimumDuration;
        private bool hasExploded;
        private bool permanentPositionSet;

        public override void OnEnter()
        {
            base.OnEnter();

            this.minimumDuration = this.baseDuration / this.attackSpeedStat;
            this.damageToPlayer = base.healthComponent.fullHealth * this.playerDamageCoefficient;
            this.spawnPos = GetDropPos();
            this.hasExploded = false;

            this.firecracker = GameObject.Instantiate(firecrackerPrefab, spawnPos, this.transform.rotation * Quaternion.Euler(90f, 0f, 0f));
            PrefabAPI.RegisterNetworkPrefab(this.firecracker);
            NetworkServer.Spawn(this.firecracker);

            AkSoundEngine.PostEvent(dropSound, firecracker);

            base.PlayAnimation("Gesture, Override", "DropFirecracker", "DropFirecracker.playbackRate", this.minimumDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // This is here to prevent the firecracker from clipping beneath the ground
            // For some reason this happens on slopes (mainly) but some other places too
            // If there's a better solution I'd love to know...
            if (!permanentPositionSet && this.immediateGroundBeneath != default(Vector3))
            {
                if(this.firecracker.transform.position.y < this.immediateGroundBeneath.y)
                {
                    this.firecracker.transform.position = this.immediateGroundBeneath;
                    this.firecracker.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    this.permanentPositionSet = true;
                }
            }

            if (this.fixedAge >= this.minimumDuration)
            {
                if (base.inputBank && !base.inputBank.skill2.down)
                {
                    if (!this.hasExploded)
                    {
                        ApplyDamageAndForceToPlayers(CamperPlugin.FirecrackersAffectOthers.Value);
                        ExplodeFirecracker(damageType);
                    }
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            if (!this.hasExploded)
            {
                ApplyDamageAndForceToPlayers(CamperPlugin.FirecrackersAffectOthers.Value);
                ExplodeFirecracker(damageType);
            }

            base.OnExit();
        }

        private void ApplyDamageAndForceToPlayers(bool affectOtherPlayers)
        {
            foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
            {
                if (player)
                {
                    CharacterBody playerBody = player.master.GetBody();
                    if (playerBody)
                    {
                        HealthComponent playerHealthComponent = playerBody.GetComponent<HealthComponent>();
                        Vector3 relativePosToFirecracker = playerBody.transform.position - firecracker.transform.position;
                        float distanceToFirecracker = relativePosToFirecracker.magnitude;

                        // If player is in blast radius, apply force and take damage
                        if (distanceToFirecracker < this.blastRadius)
                        {
                            if (playerHealthComponent)
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
                                    playerHealthComponent.TakeDamage(damageToTake);
                                }
                            }

                            if (playerBody.characterMotor)
                            {
                                // Apply force to character
                                var direction = relativePosToFirecracker / distanceToFirecracker;
                                float force = playerBody.characterMotor.isGrounded ? this.groundForce : this.airForce;
                                float mass = playerBody.characterMotor.mass;
                                float acceleration = playerBody.acceleration;
                                float ySpeed = Trajectory.CalculateInitialYSpeedForHeight(force, -acceleration);

                                if (playerBody.hasAuthority) playerBody.characterMotor.ApplyForce(ySpeed * mass * direction, false, false);
                            }
                        }
                    }
                    if (!affectOtherPlayers) return;
                } 
            }
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
            NetworkServer.Destroy(this.firecracker);
            Destroy(this.firecracker);
            this.hasExploded = true;
        }

        private Vector3 GetDropPos()
        {
            Vector3 moveDirection = base.inputBank.moveVector.normalized;
            Vector3 currentCharacterPosition = base.transform.position;

            // Grab ground beneath player (if existing)
            if (Physics.Raycast(currentCharacterPosition, base.transform.TransformDirection(Vector3.down), out RaycastHit hit))
            {
                this.immediateGroundBeneath = hit.point + (BaseFirecrackerState.distanceAboveGroundOffset * Vector3.up);

                // Check if character is within 5 meters of the ground
                if (Vector3.Distance(immediateGroundBeneath, currentCharacterPosition) <= maxDropDistance)
                    return currentCharacterPosition;
            }
            
            // Return a position right beneath the player if not within 5m of ground
            // Also set the duration to 0.05 so that it can explode instantly
            this.minimumDuration = 0.05f;
            return currentCharacterPosition + (Vector3.down * distanceAwayFromPlayerOffset) + (-moveDirection * distanceAwayFromPlayerOffset);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}