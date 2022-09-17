using BepInEx;
using CamperMod.Modules.Survivors;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
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
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.sladoinki.CamperMod";
        public const string MODNAME = "CamperMod";
        public const string MODVERSION = "1.0.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "SLAD";

        public static CamperPlugin instance;

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

            // survivor initialization
            new Camper().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Hook();
        }

        // Passive (better chest items)
        private void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            string chestType = self.gameObject.name.ToLower();
            if (!chestType.Contains("chest")) orig.Invoke(self);

            if (chestType.Contains("chest1"))
            {
                PickupIndex dropItem = this.RollItem();

                if (dropItem != PickupIndex.none) self.SetFieldValue("dropPickup", dropItem);
            }
            orig.Invoke(self);
        }

        private PickupIndex RollItem()
        {
            System.Random random = new System.Random(System.DateTime.UtcNow.Second);

            List<PickupIndex> availableItems;
            int itemIndex = 0;

            int randomChance = random.Next(0, 101);

            if (randomChance <= 65) availableItems = Run.instance.availableTier1DropList;
            else if (randomChance <= 99) availableItems = Run.instance.availableTier2DropList;
            else availableItems = Run.instance.availableTier3DropList;

            if (availableItems.Count != 0)
            {
                itemIndex = random.Next(0, availableItems.Count);
                return availableItems[itemIndex];
            }

            return PickupIndex.none;
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            bool flag = self;

            if (flag)
            {
                bool flag2 = self.HasBuff(Modules.Buffs.spinBuff);

                if (flag2)
                {
                    self.moveSpeed += self.moveSpeed / 4f;
                    self.attackSpeed += 0.2f;
                }
            }
        }
    }
}