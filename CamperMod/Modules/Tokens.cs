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
            string achievementPrefix = "ACHIEVEMENT_" + prefix;
            string achievementAffix = "UNLOCKABLE_ACHIEVEMENT_ID_";

            string desc = "Survivors teabags to infuriate their enemies.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Teabag's main damage comes from its infuriation aspect - target enemies that are also targetting you." + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > Firecrackers only explode upon releasing the activation key." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Dead Hard can knock back and damage smaller enemies on collision, making it strong near map edges." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Self Care briefly provides an armour boost so it can be used to mitigate a strong blow." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, searching for a way back home.";
            string outroFailure = "..and so she vanished, forever missing.";

            LanguageAPI.Add(prefix + "NAME", "The Survivors");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Entity's Plaything");
            LanguageAPI.Add(prefix + "LORE", StaticValues.lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "NEA_SKIN_NAME", "Nea");
            LanguageAPI.Add(prefix + "CLAUDETTEPRESTIGE_SKIN_NAME", "Default (Mastery)");
            LanguageAPI.Add(prefix + "NEAPRESTIGE_SKIN_NAME", "Nea (Mastery)");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_OBJECTOFOBSESSION_NAME", "Object of Obsession");
            LanguageAPI.Add(prefix + "PASSIVE_OBJECTOFOBSESSION_DESCRIPTION", $"Teabagging deals <style=cIsDamage>bonus damage</style> to enemies facing you.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_TEABAG_NAME", "Teabag");
            LanguageAPI.Add(prefix + "PRIMARY_TEABAG_DESCRIPTION", $"Teabag at an enemy for up to <style=cIsDamage>{Math.Round(100f * (StaticValues.teabagDamageCoefficient + StaticValues.passiveFacingDamageMultiplier + StaticValues.teabagRangeDamageCoefficient))}% damage.</style>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_FIRECRACKER_NAME", "Firecracker");
            LanguageAPI.Add(prefix + "SECONDARY_FIRECRACKER_DESCRIPTION", Helpers.agilePrefix + $"Drop a firecracker which explodes and deals <style=cIsDamage>{100f * StaticValues.firecrackerDamageCoefficient}% damage</style> to enemies.");

            LanguageAPI.Add(prefix + "SECONDARY_WINTERFIRECRACKER_NAME", "Winter Party Starter");
            LanguageAPI.Add(prefix + "SECONDARY_WINTERFIRECRACKER_DESCRIPTION", Helpers.agilePrefix + $"<style=cIsDamage>Freezing.</style> Drop a wintery firecracker, dealing <style=cIsDamage>{100f * StaticValues.winterFirecrackerDamageCoefficient}% damage</style> to enemies.");
            
            LanguageAPI.Add(prefix + "SECONDARY_FLASHBANGFIRECRACKER_NAME", "Flashbang");
            LanguageAPI.Add(prefix + "SECONDARY_FLASHBANGFIRECRACKER_DESCRIPTION", Helpers.agilePrefix + $"<style=cIsDamage>Stunning.</style> Drop a flashbang, dealing <style=cIsDamage>{100f * StaticValues.flashbangFirecrackerDamageCoefficient}% damage</style> to enemies. You go <style=cIsUtility>invisible</style> once the flashbang has exploded.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DEADHARD_NAME", "Dead Hard");
            LanguageAPI.Add(prefix + "UTILITY_DEADHARD_DESCRIPTION", "Dash a short distance. <style=cIsUtility>You cannot be hit during the dash.</style>");

            LanguageAPI.Add(prefix + "UTILITY_SPRINTBURST_NAME", "Sprint Burst");
            LanguageAPI.Add(prefix + "UTILITY_SPRINTBURST_DESCRIPTION", "Run at <style=cIsUtility>300%</style> movespeed. Moonwalking grants a <style=cIsUtility>stacking movement speed and attack speed buff</style>. <style=cIsDamage>360s deal damage.</style>");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_SELFCARE_NAME", "Self Care");
            LanguageAPI.Add(prefix + "SPECIAL_SELFCARE_DESCRIPTION", $"<style=cArtifact>Channelling.</style> Heal yourself for <style=cIsHealing>{100f * Modules.StaticValues.selfCareHPSCoefficient}% maximum HP</style> every second. <style=cIsUtility>Reactivate this skill to stop healing.</style>");

            LanguageAPI.Add(prefix + "SPECIAL_MEDKIT_NAME", "Medkit");
            LanguageAPI.Add(prefix + "SPECIAL_MEDKIT_DESCRIPTION", $"<style=cIsHealth>Fragile.</style> <style=cArtifact>Channelling.</style> Heal yourself for <style=cIsHealing>{200f * StaticValues.medkitHPSCoefficient}% maximum HP</style> every second for <style=cIsUtility>{StaticValues.medkitDuration} seconds.</style>");

            LanguageAPI.Add(prefix + "SPECIAL_DECISIVESTRIKE_NAME", "Decisive Strike");
            LanguageAPI.Add(prefix + "SPECIAL_DECISIVESTRIKE_DESCRIPTION", $"Dash to a targetted enemy, dealing <style=cIsDamage>{100f * StaticValues.decisiveStrikeDamageCoefficient}% damage</style>. <style=cIsHealing>Heal for a portion of the damage dealt</style>. Kills <style=cIsUtility>reset this cooldown</style>.");
            #endregion

            #region Achievements
            LanguageAPI.Add(achievementPrefix + "MASTERY" + achievementAffix + "NAME", "Survivor: Mastery");
            LanguageAPI.Add(achievementPrefix + "MASTERY" + achievementAffix + "DESCRIPTION", "As a survivor, beat the game or obliterate on Monsoon.");

            LanguageAPI.Add(achievementPrefix + "WINTERFIRECRACKER" + achievementAffix + "NAME", "Survivor: Cold As Ice");
            LanguageAPI.Add(achievementPrefix + "WINTERFIRECRACKER" + achievementAffix + "DESCRIPTION", "As a survivor, defeat the teleporter boss on the Siphoned Forest on Monsoon.");

            LanguageAPI.Add(achievementPrefix + "SPRINTBURST" + achievementAffix + "NAME", "Survivor: Athlete");
            LanguageAPI.Add(achievementPrefix + "SPRINTBURST" + achievementAffix + "DESCRIPTION", "As a survivor, carry 5 Paul's Goat Hoofs at once.");

            LanguageAPI.Add(achievementPrefix + "FLASHBANG" + achievementAffix + "NAME", "Survivor: Escape Artist");
            LanguageAPI.Add(achievementPrefix + "FLASHBANG" + achievementAffix + "DESCRIPTION", "As a survivor, kill an enemy while cloaked.");

            LanguageAPI.Add(achievementPrefix + "MEDKIT" + achievementAffix + "NAME", "Survivor: Self Taught");
            LanguageAPI.Add(achievementPrefix + "MEDKIT" + achievementAffix + "DESCRIPTION", "As a survivor, heal more than a third of your health using medkits at once.");

            LanguageAPI.Add(achievementPrefix + "DECISIVESTRIKE" + achievementAffix + "NAME", "Survivor: Fight Back");
            LanguageAPI.Add(achievementPrefix + "DECISIVESTRIKE" + achievementAffix + "DESCRIPTION", "As a survivor, defeat the teleporter boss while below 20% health.");
            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_CHANNELLING", "<style=cKeywordName>Channelling</style><style=cSub>Controls are <style=cDeath>disabled</style> for the duration of the ability.</style>");
            LanguageAPI.Add("KEYWORD_FRAGILE", "<style=cKeywordName>Fragile</style><style=cSub><style=cIsDamage>Taking damage</style> cancels the ability.</style>");
            #endregion
            #endregion
        }
    }
}