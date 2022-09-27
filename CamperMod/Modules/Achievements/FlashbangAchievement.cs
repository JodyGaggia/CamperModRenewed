using RoR2;

namespace CamperMod.Modules.Achievements
{
    [RegisterAchievement("SLAD_CAMPER_BODY_FLASHBANGUNLOCKABLE_ACHIEVEMENT_ID", "SLAD_CAMPER_BODY_FLASHBANGUNLOCKABLE_REWARD_ID", null)]
    internal class FlashbangAchievement : GenericModdedUnlockable
    {
        public override string AchievementTokenPrefix => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_FLASHBANG";
        public override string AchievementSpriteName => "texFlashbang";
        public override string PrerequisiteUnlockableIdentifier => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_FLASHBANGUNLOCKABLE_REWARD_ID";

        private string RequiredCharacterBody => "CamperBody";

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        public override void OnBodyRequirementBroken()
        {
            GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
            base.OnBodyRequirementBroken();
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport obj)
        {
            CharacterMaster playerMaster = PlayerCharacterMasterController.instances[0].master;
            if (obj.attacker == playerMaster.GetBodyObject())
            {
                if (playerMaster.GetBody().HasBuff(RoR2Content.Buffs.Cloak)) Grant();
            }
        }

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(RequiredCharacterBody);
        }
    }
}
