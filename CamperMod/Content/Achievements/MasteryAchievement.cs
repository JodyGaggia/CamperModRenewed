namespace CamperMod.Modules.Achievements
{
    internal class MasteryAchievement : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_MASTERY";
        //the name of the sprite in your bundle
        public override string AchievementSpriteName => "texClaudetteSkin";
        //the token of your character's unlock achievement if you have one
        public override string PrerequisiteUnlockableIdentifier => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_UNLOCKABLE_REWARD_ID";

        public override string RequiredCharacterBody => "CamperBody";
        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}