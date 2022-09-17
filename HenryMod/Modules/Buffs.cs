using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace CamperMod.Modules
{
    public static class Buffs
    {
        internal static BuffDef spinBuff;
        internal static void RegisterBuffs()
        {
            spinBuff = AddNewBuff("CamperSpinBuff", RoR2.LegacyResourcesAPI.Load<Sprite>("texSpinBuff"), Color.white, false, false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}