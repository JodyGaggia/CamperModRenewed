using RoR2;

namespace CamperMod.Modules.Achievements
{
    [RegisterAchievement("SLAD_CAMPER_BODY_WINTERFIRECRACKERUNLOCKABLE_ACHIEVEMENT_ID", "SLAD_CAMPER_BODY_WINTERFIRECRACKERUNLOCKABLE_REWARD_ID", null, typeof(WinterFirecrackerAchievement.WinterFirecrackerServerAchievement))]
    internal class WinterFirecrackerAchievement : GenericModdedUnlockable
    {
        public override string AchievementTokenPrefix => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_WINTERFIRECRACKER";
        public override string AchievementSpriteName => "texWinterFirecracker";
        public override string PrerequisiteUnlockableIdentifier => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_WINTERFIRECRACKERUNLOCKABLE_REWARD_ID";

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

        private class WinterFirecrackerServerAchievement : RoR2.Achievements.BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                RoR2.BossGroup.onBossGroupDefeatedServer += BossGroup_onBossGroupDefeatedServer;
            }

            private void BossGroup_onBossGroupDefeatedServer(BossGroup obj)
            {
                DifficultyDef difficulty = DifficultyCatalog.GetDifficultyDef(RoR2.Run.instance.selectedDifficulty);
                if (SceneCatalog.GetSceneDefForCurrentScene().baseSceneName == "snowyforest" && difficulty.countsAsHardMode)
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
