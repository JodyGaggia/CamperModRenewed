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
        public static float damageCoefficient = 4f;
        public static float procCoefficient = 0.2f;

        private Animator animator;
        private OverlapAttack attack;
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

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.charYaw += this.characterDirection.yaw - this.lastYaw;
            this.lastYaw = this.characterDirection.yaw;

            // Increase speed
            Vector3 velocity = this.inputBank.moveVector * this.moveSpeedStat * SprintBurst.sprintMultiplier;
            velocity.y = this.characterMotor.velocity.y;

            if (base.isAuthority) this.characterMotor.velocity = velocity;

            if (Mathf.Abs(this.charYaw) > 270f)
            {
                this.charYaw = 0f;
                FireAttack();
            }
            

            // Get angle between movement and character direction
            float movementAngle = Vector3.Angle(base.characterDirection.forward, base.characterMotor.velocity);
            Log.Debug("Movement: " + (movementAngle > 100f));
            if (movementAngle > 100f) // Approximately running backwards ;)
            {
                // Count how long moonwalk has lasted
                this.stopwatch += Time.fixedDeltaTime;
                Log.Debug("Stopwatch: " + (this.stopwatch >= this.moonwalkInterval));
                if (this.stopwatch >= this.moonwalkInterval)
                {
                    Log.Debug("CharBody: " + base.characterBody);
                    // Add buffs if moonwalking for long enough
                    if (base.characterBody)
                    {
                        Log.Debug("Network: " + NetworkServer.active);
                        if (NetworkServer.active)
                        {
                            base.characterBody.AddTimedBuff(Buffs.moonwalkBuff, 4f);
                            base.characterBody.RecalculateStats();
                        }
                    }

                    if (base.cameraTargetParams) base.cameraTargetParams.AddRecoil(2f, 5f, 2f, 5f); // Some feedback

                    this.stopwatch = 0f; // Reset the timer since the buff is stackable
                }
            }
            else this.stopwatch = 0f;
            

            // Exit if attacking, util button pressed or no movement input (ugly)
            if (animator.GetBool("attacking") || base.inputBank.skill3.down && base.fixedAge >= Modules.StaticValues.keyLiftGrace || base.inputBank.moveVector == Vector3.zero || base.fixedAge >= SprintBurst.duration)
            {
                this.outer.SetNextStateToMain();
            }
            
        }

        public override void OnExit()
        {
            // Reset camera and turnspeed
            if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = -1f;
            if (base.isAuthority) base.characterDirection.turnSpeed = Modules.StaticValues.turnSpeed;

            base.OnExit();
        }

        private void FireAttack()
        {
            HitBoxGroup hitboxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitboxGroup = System.Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == SprintBurst.hitboxName);
            }

            if (base.isAuthority)
            {
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
                this.attack.Fire();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

    }
}