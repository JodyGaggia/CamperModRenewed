using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CamperMod.SkillStates
{
    public class BalancedLanding : BaseSkillState
    {
        public static float damageCoefficient = 70f;
        public static float procCoefficient = 0.8f;
        public static float maxDuration = 0.4f;
        public static float startSpeedCoefficient = 3f;
        public static float endSpeedCoefficient = 5f;
        public static float maxPushForce = 600f;
        public static float maxFallDistance = 60f;

        private Vector3 landingPos;
        private NetworkSoundEventIndex impactSound;
        private OverlapAttack attack;
        private float pushForce;
        private float fallCoefficient;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            if (!base.isGrounded) GetFallCoefficient();
            else this.fallCoefficient = 0f;

            this.pushForce = maxPushForce * this.moveSpeedStat * fallCoefficient;
            this.duration = BalancedLanding.maxDuration * this.fallCoefficient;

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
            this.attack.damage = BalancedLanding.damageCoefficient * this.damageStat * this.fallCoefficient;
            this.attack.procCoefficient = BalancedLanding.procCoefficient;
            this.attack.hitEffectPrefab = null;
            this.attack.forceVector = Vector3.up;
            this.attack.pushAwayForce = this.pushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;
        }

        private void GetFallCoefficient()
        {
            if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit))
            {
                float distance = Vector3.Distance(this.transform.position, hit.point);
                if (distance > BalancedLanding.maxFallDistance) distance = BalancedLanding.maxFallDistance;

                this.fallCoefficient = (distance / maxFallDistance);
                if (this.fallCoefficient < 0.2f) this.fallCoefficient = 0.2f;

                this.landingPos = hit.point;
                return;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && !base.isGrounded)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, this.landingPos, base.age / this.duration);
            }

            if (base.isAuthority && base.isGrounded)
            {
                this.transform.position = this.landingPos;
                this.attack.Fire();
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.SmallHop(base.characterMotor, 3f);
            base.OnExit();
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
        }
    }
}