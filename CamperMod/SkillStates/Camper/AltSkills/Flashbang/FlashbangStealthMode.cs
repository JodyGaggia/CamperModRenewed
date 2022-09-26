using EntityStates;
using RoR2;
using UnityEngine.Networking;

namespace CamperMod.SkillStates.Camper.AltSkills.Flashbang
{
    public class FlashbangStealthMode : BaseState
    {
        private static float stealthDuration = 3f;
        public override void OnEnter()
        {
            base.OnEnter();

            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    base.characterBody.AddBuff(RoR2Content.Buffs.Cloak);
                }
                base.characterBody.onSkillActivatedAuthority += CharacterBody_onSkillActivatedAuthority;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= FlashbangStealthMode.stealthDuration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
                }
                base.characterBody.onSkillActivatedAuthority -= CharacterBody_onSkillActivatedAuthority;
            }

            base.OnExit();
        }

        private void CharacterBody_onSkillActivatedAuthority(GenericSkill obj)
        {
            if (obj.skillDef.isCombatSkill)
            {
                // this makes me sad
                // without this, a new flashbang just doesnt get thrown but goes on cooldown
                if(obj.skillDef.skillIndex == 2) this.outer.SetNextState(new FlashbangFirecracker());
                else this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
