using EntityStates;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace CamperMod.SkillStates
{
    public class BaseFirecrackerState : BaseSkillState
    {
        public static float maxDropDistance = 3f;
        public static float maxDropDistanceOffset = 2f;
        public static float distanceAboveGround = 0.5f;

        protected float baseCharges = 1f;
        protected bool isRemote = false;

        protected float enemyDamageCoefficient = 10f;
        protected float playerDamageCoefficient = 0.2f;
        protected float baseDuration = 1f;
        protected float baseChargeTime = 0.5f;
        protected float radius = 20f;
        protected float procCoefficient = 1f;

        protected float airForce = 10f;
        protected float groundForce = 2f;

        private Vector3 dropPos;
        private List<GameObject> firecrackers = new List<GameObject>();
        private float stopwatch;
        private float damageToPlayer;
        private float chargeTime;
        private float duration;
        private bool hasThrown;
        private bool hasExploded;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = this.baseDuration / this.attackSpeedStat;
            this.damageToPlayer = base.healthComponent.fullHealth * this.playerDamageCoefficient;
            this.chargeTime = this.baseChargeTime / this.attackSpeedStat;
            this.dropPos = GetDropPos();

            GameObject firecracker = GameObject.Instantiate(Modules.Assets.firecrackerMesh, dropPos, this.transform.rotation * Quaternion.Euler(90f, 0f, 0f));
            firecrackers.Add(firecracker);

            this.baseCharges -= 1f;

            base.PlayAnimation("Gesture, Override", "DropFirecracker", "DropFirecracker.playbackRate", this.duration);

            if (!isRemote) Util.PlaySound("FirecrackerIgnite", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            stopwatch += Time.fixedDeltaTime;

            if (isRemote)
            {
                if (base.isAuthority && base.inputBank.skill2.down && this.baseCharges > 0 && stopwatch > Modules.StaticValues.keyLiftGrace)
                {
                    base.PlayAnimation("Gesture, Override", "DropFirecracker", "DropFirecracker.playbackRate", this.duration);
                    this.dropPos = GetDropPos();
                    GameObject firecrackerInstance = GameObject.Instantiate(Modules.Assets.firecrackerMesh, this.dropPos, this.transform.rotation * Quaternion.Euler(90f, 0f, 0f));
                    this.firecrackers.Add(firecrackerInstance);
                    stopwatch = 0f;
                    this.baseCharges -= 1f;
                }

                if (base.isAuthority && this.baseCharges == 0 && stopwatch > Modules.StaticValues.keyLiftGrace && base.inputBank.skill2.down)
                {
                    ExplodeFirecrackers();
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            else
            {
                this.chargeTime -= Time.fixedDeltaTime;

                if (this.chargeTime <= 0 && !hasThrown && base.isAuthority)
                {
                    this.hasThrown = true;
                    ExplodeFirecrackers();
                }

                if (base.isAuthority && base.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (!hasExploded) ExplodeFirecrackers();
            base.OnExit();
        }

        private void ExplodeFirecrackers()
        {
            foreach (GameObject firecracker in this.firecrackers)
            {
                // Damage enemies
                BlastAttack blast = new BlastAttack();
                blast.radius = this.radius;
                blast.procCoefficient = this.procCoefficient;
                blast.position = firecracker.transform.position;
                blast.attacker = base.gameObject;
                blast.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blast.baseDamage = base.characterBody.damage * this.enemyDamageCoefficient;
                blast.falloffModel = BlastAttack.FalloffModel.None;
                blast.damageType = DamageType.Shock5s;
                blast.baseForce = 800f;
                blast.teamIndex = TeamComponent.GetObjectTeam(blast.attacker);
                blast.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blast.Fire();

                // Effects
                Util.PlaySound("Firecracker", base.gameObject);
                EffectManager.SimpleEffect(Modules.Assets.firecrackerExplosion, firecracker.transform.position, Quaternion.identity, true);

                // Force and damage to player
                if (base.isAuthority && base.characterBody && base.characterBody.characterMotor)
                {
                    Vector3 pos = this.transform.position - firecracker.transform.position;
                    float distance = pos.magnitude;

                    if (distance < this.radius)
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
                            damageType = (DamageType.NonLethal | DamageType.BypassArmor),
                            damageColorIndex = DamageColorIndex.Default,
                            dotIndex = DotController.DotIndex.None
                        };

                        // Create force info
                        var direction = pos / distance;
                        float force = base.characterBody.characterMotor.isGrounded ? this.groundForce : this.airForce;
                        float mass = base.characterBody.characterMotor ? base.characterBody.characterMotor.mass : 1f;
                        float acceleration = base.characterBody.acceleration;
                        float ySpeed = Trajectory.CalculateInitialYSpeedForHeight(force, -acceleration);

                        // Take damage and apply force
                        base.healthComponent.TakeDamage(damageToTake);
                        base.characterBody.characterMotor.ApplyForce(ySpeed * mass * direction, false, false);

                        // Destroy firecracker gameobject
                        Destroy(firecracker);
                        //if (NetworkServer.active) NetworkServer.UnSpawn(firecracker);

                        this.hasExploded = true;
                    }
                }
            }
        }

        private Vector3 GetDropPos()
        {
            Vector3 forwardDirection = base.inputBank.moveVector.normalized;

            if (this.isRemote)
            {
                if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.down), out RaycastHit primaryHit)) return primaryHit.point + (Vector3.up * BaseFirecrackerState.distanceAboveGround);
                else return base.transform.position - (Vector3.down * 1000f); // default will basically never be hit
            }

            if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.down), out RaycastHit hit, BaseFirecrackerState.maxDropDistance)) return hit.point + (Vector3.up * BaseFirecrackerState.distanceAboveGround);

            this.chargeTime = 0.01f;
            return base.transform.position + (Vector3.down * BaseFirecrackerState.maxDropDistanceOffset) + (-forwardDirection * BaseFirecrackerState.maxDropDistanceOffset);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}