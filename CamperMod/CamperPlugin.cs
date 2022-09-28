using BepInEx;
using BepInEx.Configuration;
using CamperMod.Modules.Survivors;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace CamperMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI"
    })]

    public class CamperPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.sladoinki.CamperMod";
        public const string MODNAME = "CamperMod";
        public const string MODVERSION = "1.0.0";

        public const string DEVELOPER_PREFIX = "SLAD";

        public static CamperPlugin instance;
        protected ConfigFile ConfigFile { get; }
        public static ConfigEntry<bool> FirecrackersAffectOthers { get; set; }

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            new Camper().Initialize();

            new Modules.ContentPacks().Initialize();

            SetUpConfig();
            Hook();
        }

        private void SetUpConfig()
        {
            FirecrackersAffectOthers = Config.Bind<bool>(
                "Multiplayer",
                "FirecrackersAffectOthers",
                true,
                "Determines whether firecracker explosions should damage and apply force to others in multiplayer."
                );
        }

        private void Hook()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(Modules.Buffs.moonwalkBuff))
                {
                    self.moveSpeed *= 1.2f;
                    self.attackSpeed *= 1.2f;
                }
            }
        }
    }
}