using UnityEngine.Networking;
using RoR2;
using CamperMod.Modules;

namespace CamperMod.SkillStates
{
    public class SelfCare : BaseHealState
    {
        public override void OnEnter()
        {
            if (base.characterBody)
            {
                if (NetworkServer.active) base.characterBody.AddTimedBuff(RoR2Content.Buffs.ArmorBoost, 1f);
            }

            this.hpsCoefficient = StaticValues.selfCareHPSCoefficient;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.inputBank.skill4.down && this.fixedAge > Modules.StaticValues.keyLiftGrace)
            {
                base.outer.SetNextStateToMain();
                return;
            }
        }
    }
}