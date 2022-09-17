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
            bodyNameToken = CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_NAME",
            subtitleNameToken = CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_SUBTITLE",

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
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
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
            string prefix = CamperPlugin.DEVELOPER_PREFIX;

            #region Primary
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_CAMPER_BODY_PRIMARY_TEABAG_NAME",
                skillDescriptionToken = prefix + "_CAMPER_BODY_PRIMARY_TEABAG_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texTeabag"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Teabag)),
                activationStateMachineName = "Weapon",
                canceledFromSprinting = true
            });

            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);

            SkillDef primary2SkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_CAMPER_BODY_PRIMARY_SLAP_NAME",
                skillDescriptionToken = prefix + "_CAMPER_BODY_PRIMARY_SLAP_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texTeabag"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Slap)),
                activationStateMachineName = "Weapon",
                canceledFromSprinting = false
            });

            Modules.Skills.AddPrimarySkills(bodyPrefab, primary2SkillDef);
            #endregion

            #region Secondary
            SkillDef firecrackerDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_CAMPER_BODY_SECONDARY_FIRECRACKER_NAME",
                skillNameToken = prefix + "_CAMPER_BODY_SECONDARY_FIRECRACKER_NAME",
                skillDescriptionToken = prefix + "_CAMPER_BODY_SECONDARY_FIRECRACKER_DESCRIPTION",
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
                skillName = prefix + "_CAMPER_BODY_SECONDARY_FIRECRACKER_NAME",
                skillNameToken = prefix + "_CAMPER_BODY_SECONDARY_FIRECRACKER_NAME",
                skillDescriptionToken = prefix + "_CAMPER_BODY_SECONDARY_FIRECRACKER_DESCRIPTION",
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
                skillName = prefix + "_CAMPER_BODY_UTILITY_DEADHARD_NAME",
                skillNameToken = prefix + "_CAMPER_BODY_UTILITY_DEADHARD_NAME",
                skillDescriptionToken = prefix + "_CAMPER_BODY_UTILITY_DEADHARD_DESCRIPTION",
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
                skillName = prefix + "_CAMPER_BODY_UTILITY_SPRINTBURST_NAME",
                skillNameToken = prefix + "_CAMPER_BODY_UTILITY_SPRINTBURST_NAME",
                skillDescriptionToken = prefix + "_CAMPER_BODY_UTILITY_SPRINTBURST_DESCRIPTION",
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
                skillName = prefix + "_CAMPER_BODY_UTILITY_BALANCEDLANDING_NAME",
                skillNameToken = prefix + "_CAMPER_BODY_UTILITY_BALANCEDLANDING_NAME",
                skillDescriptionToken = prefix + "_CAMPER_BODY_UTILITY_BALANCEDLANDING_DESCRIPTION",
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
                skillName = prefix + "_CAMPER_BODY_SPECIAL_SELFCARE_NAME",
                skillNameToken = prefix + "_CAMPER_BODY_SPECIAL_SELFCARE_NAME",
                skillDescriptionToken = prefix + "_CAMPER_BODY_SPECIAL_SELFCARE_DESCRIPTION",
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
                skillName = prefix + "_CAMPER_BODY_SPECIAL_MEDKIT_NAME",
                skillNameToken = prefix + "_CAMPER_BODY_SPECIAL_MEDKIT_NAME",
                skillDescriptionToken = prefix + "_CAMPER_BODY_SPECIAL_MEDKIT_DESCRIPTION",
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

            #region NeaSkin
            SkinDef neaSkin = Modules.Skins.CreateSkinDef(CAMPER_PREFIX + "NEA_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                model);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            neaSkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos, "meshNeaDefault");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(neaSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            /*
            //creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(HenryPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                model,
                masterySkinUnlockableDef);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "meshHenrySwordAlt",
                null,//no gun mesh replacement. use same gun mesh
                "meshHenryAlt");

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos defaultMaterials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            masterySkin.rendererInfos[1].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            masterySkin.rendererInfos[2].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("GunModel"),
                    shouldActivate = false,
                }
            };
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);
            */
            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}