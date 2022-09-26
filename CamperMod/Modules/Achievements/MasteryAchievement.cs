using R2API;
using RoR2;

namespace CamperMod.Modules.Achievements
{
    [RegisterAchievement("SLAD_CAMPER_BODY_MASTERYUNLOCKABLE_ACHIEVEMENT_ID", "SLAD_CAMPER_BODY_MASTERYUNLOCKABLE_REWARD_ID", null)]
    internal class MasteryAchievement : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_MASTERY";
        public override string AchievementSpriteName => "texClaudetteSkin";
        public override string PrerequisiteUnlockableIdentifier => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_UNLOCKABLE_REWARD_ID";

        public override string RequiredCharacterBody => "CamperBody";
        public override float RequiredDifficultyCoefficient => 3;
    }
}