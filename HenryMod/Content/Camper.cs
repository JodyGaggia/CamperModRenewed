using BepInEx.Configuration;
using CamperMod.Modules.Characters;
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
        //used when registering your survivor's language tokens
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
            armor = 15f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] 
        {
            new CustomRendererInfo
            {
                childName = "CamperModel",
                material = Materials.CreateHopooMaterial("matClaudetteDefault"),
            },
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new CamperItemDisplays();

                                                                          //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
        }

        public override void InitializeUnlockables()
        {
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform hitboxTransform = childLocator.FindChild("DeadHardHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Bash");

            Transform hitboxTransform2 = childLocator.FindChild("SpinHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform2, "SpinBox");

            Transform hitboxTransform3 = childLocator.FindChild("SlapHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform3, "Slap");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_";

            #region Primary
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo(prefix + "PRIMARY_TEABAG_NAME",
                                                                          prefix + "PRIMARY_TEABAG_DESCRIPTION",
                                                                          Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texTeabag"),
                                                                          new string[] { "KEYWORD_INFURIATING" },
                                                                          new EntityStates.SerializableEntityStateType(typeof(SkillStates.Teabag)),
                                                                          "Weapon",
                                                                          false));


            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);
            
            SkillDef primarySkillDef2 = Modules.Skills.CreateSkillDef(new SkillDefInfo(prefix + "PRIMARY_SLAP_NAME",
                                                                                      prefix + "PRIMARY_SLAP_DESCRIPTION",
                                                                                      Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texTeabag"),
                                                                                      new string[] { "KEYWORD_AGILE"},
                                                                                      new EntityStates.SerializableEntityStateType(typeof(SkillStates.Slap)),
                                                                                      "Weapon",
                                                                                      true));

            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef2);
            #endregion

            #region Secondary
            SkillDef firecrackerDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_FIRECRACKER_NAME",
                skillNameToken = prefix + "SECONDARY_FIRECRACKER_NAME",
                skillDescriptionToken = prefix + "SECONDARY_FIRECRACKER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texFirecracker"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.DropFirecracker)),
                activationStateMachineName = "Slide",
                baseMaxStock = 2,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE", "KEYWORD_SHOCKING" }
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, firecrackerDef);

            SkillDef remoteFirecrackerDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_FIRECRACKER_NAME",
                skillNameToken = prefix + "SECONDARY_FIRECRACKER_NAME",
                skillDescriptionToken = prefix + "SECONDARY_FIRECRACKER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texFirecracker"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.RemoteFirecracker)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 14f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_SHOCKING" }
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, remoteFirecrackerDef);
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
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
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

            SkillDef balancedLandingDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "UTILITY_BALANCEDLANDING_NAME",
                skillNameToken = prefix + "UTILITY_BALANCEDLANDING_NAME",
                skillDescriptionToken = prefix + "UTILITY_BALANCEDLANDING_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBalancedLanding"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.BalancedLanding)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
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

            Modules.Skills.AddUtilitySkills(bodyPrefab, balancedLandingDef);
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
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = true,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Frozen,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
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
                baseMaxStock = 1,
                baseRechargeInterval = 25f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = true,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Frozen,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_FRAGILE", "KEYWORD_CHANNELLING" }
            });

            Modules.Skills.AddSpecialSkills(bodyPrefab, medkitDef);
            #endregion
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

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos, "meshClaudetteDefault");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}