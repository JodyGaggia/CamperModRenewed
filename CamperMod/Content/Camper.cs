using BepInEx.Configuration;
using CamperMod.Modules.Characters;
using CamperMod.SkillStates.Camper.AltSkills.Flashbang;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CamperMod.Modules.Survivors
{
    internal class Camper : SurvivorBase
    {
        public override string bodyName => "Camper";

        public const string CAMPER_PREFIX = CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_";
        public override string survivorTokenPrefix => CAMPER_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "CamperBody",
            bodyNameToken = CAMPER_PREFIX + "NAME",
            subtitleNameToken = CAMPER_PREFIX + "SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texCamperIcon"),
            bodyColor = Color.white,

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 80f,
            healthRegen = 2.5f,
            armor = 5f,
            damageGrowth = 3.7f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] 
        {
            new CustomRendererInfo
            {
                childName = "Model",
                material = Modules.Materials.CreateHopooMaterial("matClaudette"),
            },
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new CamperItemDisplays();

                                                                          //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;
        private static UnlockableDef winterFirecrackerUnlockableDef;
        private static UnlockableDef sprintBurstUnlockableDef;
        private static UnlockableDef flashbangUnlockableDef;
        private static UnlockableDef medkitUnlockableDef;
        private static UnlockableDef decisiveStrikeUnlockableDef;
        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            AddTrackerComponent();

            this.displayPrefab.AddComponent<SelectSound>();
        }

        private void AddTrackerComponent()
        {
            this.bodyPrefab.AddComponent<DSTracker>();
            DSTracker tracker = this.bodyPrefab.GetComponent<DSTracker>();
            tracker.maxTrackingDistance = 40f;
            tracker.maxTrackingAngle = 20f;
        }

        // there's definitely (probably) a better way of doing this since im using RegisterAchievement() but idk how to fix it
        // L
        public override void InitializeUnlockables()
        {
            masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>(true);
            winterFirecrackerUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.WinterFirecrackerAchievement>(true);
            sprintBurstUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.SprintBurstAchievement>(true);
            flashbangUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.FlashbangAchievement>(true);
            medkitUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MedkitAchievement>(true);
            decisiveStrikeUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.DecisiveStrikeAchievement>(true);
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform hitboxTransform = childLocator.FindChild("DeadHardHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "DeadHardHitbox");

            Transform hitboxTransform2 = childLocator.FindChild("SpinHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform2, "SpinHitbox");

            Transform hitboxTransform3 = childLocator.FindChild("DecisiveStrikeHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform3, "DecisiveStrikeHitbox");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_";

            #region Primary
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo(prefix + "PRIMARY_TEABAG_NAME",
                                                                          prefix + "PRIMARY_TEABAG_DESCRIPTION",
                                                                          Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texTeabag"),
                                                                          new string[] { },
                                                                          new EntityStates.SerializableEntityStateType(typeof(SkillStates.Teabag)),
                                                                          "Slide",
                                                                          false));


            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);
            #endregion

            #region Secondary
            SkillDef firecrackerDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_FIRECRACKER_NAME",
                skillNameToken = prefix + "SECONDARY_FIRECRACKER_NAME",
                skillDescriptionToken = prefix + "SECONDARY_FIRECRACKER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texFirecracker"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Firecracker)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = 7f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, firecrackerDef);
            
            SkillDef winterFirecrackerDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_WINTERFIRECRACKER_NAME",
                skillNameToken = prefix + "SECONDARY_WINTERFIRECRACKER_NAME",
                skillDescriptionToken = prefix + "SECONDARY_WINTERFIRECRACKER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texWinterFirecracker"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.WinterFirecracker)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = 9f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE", "KEYWORD_FREEZING" }
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, winterFirecrackerDef);

            SkillDef flashbangFirecrackerDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_FLASHBANGFIRECRACKER_NAME",
                skillNameToken = prefix + "SECONDARY_FLASHBANGFIRECRACKER_NAME",
                skillDescriptionToken = prefix + "SECONDARY_FLASHBANGFIRECRACKER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texFlashbang"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(FlashbangFirecracker)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE", "KEYWORD_STUNNING" }
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, flashbangFirecrackerDef);
            #endregion

            #region Utility
            SkillDef deadHardDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "UTILITY_DEADHARD_NAME",
                skillNameToken = prefix + "UTILITY_DEADHARD_NAME",
                skillDescriptionToken = prefix + "UTILITY_DEADHARD_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texDeadHard"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.DeadHard)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, deadHardDef);
            
            SkillDef sprintBurstDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "UTILITY_SPRINTBURST_NAME",
                skillNameToken = prefix + "UTILITY_SPRINTBURST_NAME",
                skillDescriptionToken = prefix + "UTILITY_SPRINTBURST_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSprintBurst"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SprintBurst)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, sprintBurstDef);
            
            #endregion

            #region Special
            SkillDef selfCareDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_SELFCARE_NAME",
                skillNameToken = prefix + "SPECIAL_SELFCARE_NAME",
                skillDescriptionToken = prefix + "SPECIAL_SELFCARE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSelfCare"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SelfCare)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = true,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Frozen,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_CHANNELLING" }
            });

            Modules.Skills.AddSpecialSkills(bodyPrefab, selfCareDef);

            
            SkillDef medkitDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_MEDKIT_NAME",
                skillNameToken = prefix + "SPECIAL_MEDKIT_NAME",
                skillDescriptionToken = prefix + "SPECIAL_MEDKIT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMedkit"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Medkit)),
                activationStateMachineName = "Body",
                baseMaxStock = 1, // 1
                baseRechargeInterval = 15f, // 15
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = true,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Frozen,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_FRAGILE", "KEYWORD_CHANNELLING" }
            });

            Modules.Skills.AddSpecialSkills(bodyPrefab, medkitDef);

            SkillDef decisiveStrikeDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_DECISIVESTRIKE_NAME",
                skillNameToken = prefix + "SPECIAL_DECISIVESTRIKE_NAME",
                skillDescriptionToken = prefix + "SPECIAL_DECISIVESTRIKE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texDecisiveStrike"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.DecisiveStrike)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Frozen,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { }
            });

            Modules.Skills.AddSpecialSkills(bodyPrefab, decisiveStrikeDef);
            #endregion

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            Modules.Skills.AddUnlockablesToFamily(skillLocator.secondary.skillFamily, 
                // Default Firecracker
                null, 
                // Winter Firecracker
                winterFirecrackerUnlockableDef, 
                // Flashbang
                flashbangUnlockableDef);

            Modules.Skills.AddUnlockablesToFamily(skillLocator.utility.skillFamily,
                // Dead Hard
                null,
                // Sprint Burst
                sprintBurstUnlockableDef);

            Modules.Skills.AddUnlockablesToFamily(skillLocator.special.skillFamily,
                // Self Care
                null,
                // Medkit
                medkitUnlockableDef,
                // Decisive Strike
                decisiveStrikeUnlockableDef);
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(CAMPER_PREFIX + "DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texClaudetteSkin"),
                defaultRendererinfos,
                model);

            defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos, "meshClaudette");

            skins.Add(defaultSkin);
            #endregion

            #region NeaSkin
            CharacterModel.RendererInfo[] neaRendererInfos = new CharacterModel.RendererInfo[defaultRendererinfos.Length];
            defaultRendererinfos.CopyTo(neaRendererInfos, 0);

            SkinDef neaSkin = Modules.Skins.CreateSkinDef(CAMPER_PREFIX + "NEA_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texNeaSkin"),
                neaRendererInfos,
                model);

            neaSkin.meshReplacements = Modules.Skins.getMeshReplacements(neaRendererInfos, "meshNea");
            neaSkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matNea");

            skins.Add(neaSkin);
            #endregion

            #region ClaudettePrestigeSkin
            CharacterModel.RendererInfo[] claudettePRendererInfos = new CharacterModel.RendererInfo[defaultRendererinfos.Length];
            defaultRendererinfos.CopyTo(claudettePRendererInfos, 0);

            SkinDef claudettePSkin = Modules.Skins.CreateSkinDef(CAMPER_PREFIX + "CLAUDETTEPRESTIGE_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texClaudettePrestigeSkin"),
                claudettePRendererInfos,
                model,
                masterySkinUnlockableDef);

            claudettePSkin.meshReplacements = Modules.Skins.getMeshReplacements(claudettePRendererInfos, "meshClaudette");
            claudettePSkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matClaudettePrestige");

            skins.Add(claudettePSkin);
            #endregion

            #region NeaPrestigeSkin
            CharacterModel.RendererInfo[] neaPRendererInfos = new CharacterModel.RendererInfo[defaultRendererinfos.Length];
            defaultRendererinfos.CopyTo(neaPRendererInfos, 0);

            SkinDef neaPSkin = Modules.Skins.CreateSkinDef(CAMPER_PREFIX + "NEAPRESTIGE_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texNeaPrestigeSkin"),
                neaPRendererInfos,
                model,
                masterySkinUnlockableDef);

            neaPSkin.meshReplacements = Modules.Skins.getMeshReplacements(neaPRendererInfos, "meshNea");
            neaPSkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matNeaPrestige");

            skins.Add(neaPSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}