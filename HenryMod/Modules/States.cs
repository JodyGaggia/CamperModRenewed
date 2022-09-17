using CamperMod.SkillStates;
using System.Collections.Generic;
using System;

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
            Modules.Content.AddEntityState(typeof(Slap));

            // Secondary
            Modules.Content.AddEntityState(typeof(DropFirecracker));
            Modules.Content.AddEntityState(typeof(RemoteFirecracker));

            // Utility
            Modules.Content.AddEntityState(typeof(DeadHard));
            Modules.Content.AddEntityState(typeof(SprintBurst));
            Modules.Content.AddEntityState(typeof(BalancedLanding));

            // Special
            Modules.Content.AddEntityState(typeof(SelfCare));
            Modules.Content.AddEntityState(typeof(Medkit));
        }
    }
}