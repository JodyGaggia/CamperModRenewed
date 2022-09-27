using RoR2;

namespace CamperMod.Modules.Achievements
{
    [RegisterAchievement("SLAD_CAMPER_BODY_DECISIVESTRIKEUNLOCKABLE_ACHIEVEMENT_ID", "SLAD_CAMPER_BODY_DECISIVESTRIKEUNLOCKABLE_REWARD_ID", null, typeof(DecisiveStrikeAchievement.DecisiveStrikeServerAchievement))]
    internal class DecisiveStrikeAchievement : GenericModdedUnlockable
    {
        public override string AchievementTokenPrefix => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_DECISIVESTRIKE";
        public override string AchievementSpriteName => "texDecisiveStrike";
        public override string PrerequisiteUnlockableIdentifier => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_DECISIVESTRIKEUNLOCKABLE_REWARD_ID";

        private string RequiredCharacterBody => "CamperBody";

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(RequiredCharacterBody);
        }

        public override void OnInstall()
        {
            base.OnInstall();
        }
        public override void OnUninstall()
        {
            base.OnUninstall();
        }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.SetServerTracked(true);
        }

        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }

        private class DecisiveStrikeServerAchievement : RoR2.Achievements.BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                RoR2.BossGroup.onBossGroupDefeatedServer += BossGroup_onBossGroupDefeatedServer;
            }

            private void BossGroup_onBossGroupDefeatedServer(BossGroup obj)
            {
                HealthComponent playerHealthComponent = PlayerCharacterMasterController.instances[0].master.GetBody().healthComponent;
                if (playerHealthComponent.health < playerHealthComponent.fullHealth * 0.2f)
                {
                    Grant();
                }
            }

            public override void OnUninstall()
            {
                RoR2.BossGroup.onBossGroupDefeatedServer -= BossGroup_onBossGroupDefeatedServer;
                base.OnUninstall();
            }
        }
    }
}
