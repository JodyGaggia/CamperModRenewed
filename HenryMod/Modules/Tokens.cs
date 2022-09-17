using R2API;
using System;

namespace CamperMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Camper
            string prefix = CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_";

            string desc = "Campers teabag to infuriate her enemies.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Teabag's main damage comes from it's infuriation aspect - target enemies that are also targeting you." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Firecrackers instantly explode when 5m above the ground, making it a great mobility tool." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Dead Hard can knock back and damage smaller enemies on collision, making it strong near map edges." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Self Care briefly provides an armour boost so it can be used to mitigate a strong blow." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, searching for a way back home.";
            string outroFailure = "..and so she vanished, forever missing.";

            LanguageAPI.Add(prefix + "NAME", "Camper");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Survivor");
            LanguageAPI.Add(prefix + "LORE", "lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "NEA_SKIN_NAME", "Nea");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_PLUNDERERS_NAME", "Plunderer's Instinct");
            LanguageAPI.Add(prefix + "PASSIVE_PLUNDERERS_DESCRIPTION", $"Common chests have a <style=cIsUtility>15%</style> chance to drop better loot.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_TEABAG_NAME", "Teabag");
            LanguageAPI.Add(prefix + "PRIMARY_TEABAG_DESCRIPTION", $"<style=cDeath>Infuriating.</style> Teabag at an enemy for up to <style=cIsDamage>{100f * (StaticValues.teabagDamageCoefficient + StaticValues.teabagFacingDamageCoefficient + StaticValues.teabagRangeDamageCoefficient)}% damage.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_SLAP_NAME", "Slap");
            LanguageAPI.Add(prefix + "PRIMARY_SLAP_DESCRIPTION", Helpers.agilePrefix + $"Slap, striking enemies for <style=cIsDamage>{100f * StaticValues.palletSmashDamageCoefficient}% damage.</style>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_FIRECRACKER_NAME", "Firecracker");
            LanguageAPI.Add(prefix + "SECONDARY_FIRECRACKER_DESCRIPTION", Helpers.agilePrefix + $"<style=cIsDamage>Shocking.</style> Drop a firecracker which explodes and deals <style=cIsDamage>{100f * StaticValues.firecrackerDamageCoefficient}% damage</style> to enemies.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DEADHARD_NAME", "Dead Hard");
            LanguageAPI.Add(prefix + "UTILITY_DEADHARD_DESCRIPTION", "Dash a short distance. <style=cIsUtility>You cannot be hit during the dash.</style>");

            LanguageAPI.Add(prefix + "UTILITY_SPRINTBURST_NAME", "Sprint Burst");
            LanguageAPI.Add(prefix + "UTILITY_SPRINTBURST_DESCRIPTION", "Run at <style=cIsUtility>300%</style> movespeed. <style=cIsUtility>Moonwalking grants a stacking movement speed and attack speed buff.</style> <style=cIsDamage>360s deal damage.</style>");

            LanguageAPI.Add(prefix + "UTILITY_BALANCEDLANDING_NAME", "BalancedLanding");
            LanguageAPI.Add(prefix + "UTILITY_BALANCEDLANDING_DESCRIPTION", "smash.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_SELFCARE_NAME", "Self Care");
            LanguageAPI.Add(prefix + "SPECIAL_SELFCARE_DESCRIPTION", $"<style=cArtifact>Channelling.</style> Heal yourself for <style=cIsHealing>at a constant rate </style> over time. <style=cIsUtility>Reactivate this skill to stop healing.</style>");

            LanguageAPI.Add(prefix + "SPECIAL_MEDKIT_NAME", "Medkit");
            LanguageAPI.Add(prefix + "SPECIAL_MEDKIT_DESCRIPTION", $"<style=cIsHealth>Fragile.</style> <style=cArtifact>Channelling.</style> Heal yourself for <style=cIsHealing>{100f * StaticValues.medkitHealCoefficient}% maximum HP</style> over time.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Camper: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Camper, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Camper: Mastery");
            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_INFURIATING", "<style=cKeywordName>Infuriating</style><style=cSub>Deal <style=cIsDamage>extra damage</style> to enemies facing you.</style>");
            LanguageAPI.Add("KEYWORD_CHANNELLING", "<style=cKeywordName>Channelling</style><style=cSub>Controls are <style=cDeath>disabled</style> for the duration of the ability.</style>");
            LanguageAPI.Add("KEYWORD_FRAGILE", "<style=cKeywordName>Fragile</style><style=cSub><style=cIsDamage>Taking damage</style> cancels the ability.</style>");
            #endregion
            #endregion
        }
    }
}