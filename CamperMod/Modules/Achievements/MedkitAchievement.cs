using CamperMod.SkillStates;
using RoR2;

namespace CamperMod.Modules.Achievements
{
    [RegisterAchievement("SLAD_CAMPER_BODY_MEDKITUNLOCKABLE_ACHIEVEMENT_ID", "SLAD_CAMPER_BODY_MEDKITUNLOCKABLE_REWARD_ID", null, typeof(MedkitAchievement.MedkitServerAchievement))]
    internal class MedkitAchievement : GenericModdedUnlockable
    {
        public override string AchievementTokenPrefix => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_MEDKIT";
        public override string AchievementSpriteName => "texMedkit";
        public override string PrerequisiteUnlockableIdentifier => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_MEDKITUNLOCKABLE_REWARD_ID";

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

        private class MedkitServerAchievement : RoR2.Achievements.BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
            }

            public override void OnUninstall()
            {
                On.RoR2.CharacterBody.OnBuffFinalStackLost -= CharacterBody_OnBuffFinalStackLost;
                base.OnUninstall();
            }

            private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
            {
                orig(self, buffDef);

                if(buffDef == RoR2Content.Buffs.MedkitHeal)
                {
                    int medkitCount = self.inventory.GetItemCount(RoR2Content.Items.Medkit);
                    int rejuvRackCount = self.inventory.GetItemCount(RoR2Content.Items.IncreaseHealing);
                    HealthComponent healthComponent = self.healthComponent;

                    float healAmount = 20f + (healthComponent.fullHealth * 0.05f * medkitCount);
                    healAmount += healAmount * rejuvRackCount;

                    if(healAmount >= healthComponent.fullHealth / 3)
                    {
                        Grant();
                    }
                }
            }
        }
    }
}
