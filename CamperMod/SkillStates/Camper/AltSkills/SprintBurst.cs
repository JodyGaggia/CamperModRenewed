using EntityStates;
using CamperMod.Modules;
using UnityEngine;
using RoR2;
using RoR2.Audio;
using UnityEngine.Networking;

namespace CamperMod.SkillStates
{
    public class SprintBurst : BaseSkillState
    {
        public static string hitboxName = "SpinHitbox";
        public static float sprintMultiplier = 3f;
        public static float sprintFOV = 100f;
        public static float sprintTurnSpeed = 360f;
        public static float duration = 4f;
        public static float maxMoonwalkStacks = 4f;
        public static float damageCoefficient = 7f;
        public static float procCoefficient = 0.2f;
        public static float baseHitPauseTime = 0.12f;

        private OverlapAttack attack;
        private Animator animator;
        private HitStopCachedState hitStopCachedState;
        private float hitPauseTimer;
        private bool inHitPause;
        private float charYaw;
        private float lastYaw;
        private float moonwalkStopwatch;
        private float moonwalkInterval;
        public override void OnEnter()
        {
            base.OnEnter();

            // fuck you fall damage
            base.SmallHop(base.characterMotor, 2f);

            // Set variables
            this.lastYaw = Mathf.Abs(this.characterDirection.yaw);
            this.moonwalkInterval = SprintBurst.duration / (SprintBurst.maxMoonwalkStacks + 1f);
            this.animator = base.GetModelAnimator();

            // Increase turnspeed
            base.characterDirection.turnSpeed = SprintBurst.sprintTurnSpeed;

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
            this.attack.impactSound = new NetworkSoundEventIndex();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Total rotation since start of ability
            this.charYaw += this.characterDirection.yaw - this.lastYaw;
            this.lastYaw = this.characterDirection.yaw;

            // Fire an attack if character has 360d
            if (Mathf.Abs(this.charYaw) > 270f)
            {
                this.charYaw = 0f;
                if (this.attack.Fire()) 
                {
                    // If this attack hits anyone, hit pause
                    if (!inHitPause)
                    {
                        this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "DeadHard.playbackRate");
                        this.hitPauseTimer = SprintBurst.baseHitPauseTime / this.attackSpeedStat;
                        inHitPause = true;
                    }
                }
            }

            // Reduce hitpause timer
            this.hitPauseTimer -= Time.fixedDeltaTime;

            // Set velocity to 0 if in hitpause
            if (inHitPause)
            {
                base.characterMotor.velocity = Vector3.zero;
                this.animator.SetFloat("DeadHard.playbackRate", 0f);

                // Reset hitpause if enough time has passed
                if (this.hitPauseTimer <= 0)
                {
                    base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                    this.inHitPause = false;
                }
            }
            else
            {
                // Increase movespeed if not in hitpause
                if (base.isAuthority)
                {
                    if (base.characterMotor)
                    {
                        Vector3 velocity = this.inputBank.moveVector * this.moveSpeedStat * SprintBurst.sprintMultiplier;
                        velocity.y = this.characterMotor.velocity.y;
                        this.characterMotor.velocity = velocity;
                    }
                }
            }

            // Get angle between movement and character direction
            float movementAngle = Vector3.Angle(base.characterDirection.forward, base.characterMotor.moveDirection);
            if (movementAngle > 100f) // Approximately running backwards ;)
            {
                // Count how long moonwalk has lasted
                this.moonwalkStopwatch -= Time.fixedDeltaTime;
                if (this.moonwalkStopwatch <= 0)
                {
                    // Add buffs if moonwalking for long enough
                    if (base.characterBody)
                    {
                        if (NetworkServer.active)
                        {
                            base.characterBody.AddTimedBuff(Buffs.moonwalkBuff, 4f);
                            base.characterBody.RecalculateStats();
                        }
                    }

                    if (base.cameraTargetParams) base.cameraTargetParams.AddRecoil(2f, 5f, 2f, 5f); // Some feedback

                    this.moonwalkStopwatch = this.moonwalkInterval; // Reset the timer since the buff is stackable
                }
            } else this.moonwalkStopwatch = this.moonwalkInterval;

            // Exit after elapsed duration or stopped moving
            if (base.fixedAge >= SprintBurst.duration || base.inputBank.moveVector == Vector3.zero)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            // Reset turn speed
            if (base.isAuthority) base.characterDirection.turnSpeed = Modules.StaticValues.turnSpeed;

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}