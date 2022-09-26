using RoR2;

namespace CamperMod.Modules.Achievements
{
    [RegisterAchievement("SLAD_CAMPER_BODY_SPRINTBURSTUNLOCKABLE_ACHIEVEMENT_ID", "SLAD_CAMPER_BODY_SPRINTBURSTUNLOCKABLE_REWARD_ID", null)]
    internal class SprintBurstAchievement : GenericModdedUnlockable
    {
        public override string AchievementTokenPrefix => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_SPRINTBURST";
        public override string AchievementSpriteName => "texSprintBurst";
        public override string PrerequisiteUnlockableIdentifier => CamperPlugin.DEVELOPER_PREFIX + "_CAMPER_BODY_SPRINTBURSTUNLOCKABLE_REWARD_ID";

        private string RequiredCharacterBody => "CamperBody";
        private Inventory currentInventory;

        private ItemDef RequiredItem => RoR2Content.Items.Hoof;
        private int AmountRequired = 5;

        // Basically a copy-paste of the huntress achievement
        // good enough
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.localUser.onMasterChanged += GrabUserInventory;
        }

        public override void OnBodyRequirementBroken()
        {
            base.localUser.onMasterChanged += GrabUserInventory;
            SetCurrentInventory(null);
            base.OnBodyRequirementBroken();
        }

        private void GrabUserInventory()
        {
            Inventory inventory = null;
            PlayerCharacterMasterController masterController = base.localUser.cachedMasterController;

            if (masterController) inventory = masterController.master.inventory;

            SetCurrentInventory(inventory);
        }

        private void SetCurrentInventory(Inventory inventory)
        {
            if (currentInventory == inventory) return;
            if (currentInventory != null) currentInventory.onInventoryChanged -= OnInventoryChanged;
            currentInventory = inventory;
            if (currentInventory != null)
            {
                currentInventory.onInventoryChanged += OnInventoryChanged;
                OnInventoryChanged();
            }
        }

        private void OnInventoryChanged()
        {
            if (AmountRequired <= currentInventory.GetItemCount(RequiredItem))
            {
                Grant();
            }
        }

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(RequiredCharacterBody);
        }
    }
}
