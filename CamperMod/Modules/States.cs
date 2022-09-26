using CamperMod.SkillStates;
using CamperMod.SkillStates.Camper.AltSkills;
using CamperMod.SkillStates.Camper.AltSkills.Flashbang;

namespace CamperMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            // Base states
            Modules.Content.AddEntityState(typeof(BaseHealState));
            Modules.Content.AddEntityState(typeof(BaseFirecrackerState));

            // Primary
            Modules.Content.AddEntityState(typeof(Teabag));

            // Secondary
            Modules.Content.AddEntityState(typeof(Firecracker));
            Modules.Content.AddEntityState(typeof(WinterFirecracker));
            Modules.Content.AddEntityState(typeof(FlashbangFirecracker));
            Modules.Content.AddEntityState(typeof(FlashbangStealthMode));

            // Utility
            Modules.Content.AddEntityState(typeof(DeadHard));
            Modules.Content.AddEntityState(typeof(SprintBurst));

            // Special
            Modules.Content.AddEntityState(typeof(SelfCare));
            Modules.Content.AddEntityState(typeof(Medkit));
            Modules.Content.AddEntityState(typeof(DecisiveStrike));
        }
    }
}