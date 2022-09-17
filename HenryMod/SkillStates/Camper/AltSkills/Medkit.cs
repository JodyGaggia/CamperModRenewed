using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace CamperMod.SkillStates
{
    public class Medkit : BaseHealState
    {
        public override void OnEnter()
        {
            if (NetworkServer.active) base.characterBody.AddTimedBuff(RoR2Content.Buffs.ArmorBoost, 0.2f);
            if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = 50f;

            this.duration = 3f;
            this.hptCoefficient = 0.002f;
            this.isFragile = true;

            base.OnEnter();
        }

        public override void OnExit()
        {
            if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = -1f;
            base.OnExit();
        }
    }
}