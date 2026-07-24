using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace WantsAndQuirks
{
    [StaticConstructorOnStartup]
    public static class VEFCompat
    {
        public static readonly bool Active = ModsConfig.IsActive("OskarPotocki.VanillaFactionsExpanded.Core");
        private static readonly HashSet<BuildableDef> hiddenDesignators;

        static VEFCompat()
        {
            if (Active)
            {
                var defType = AccessTools.TypeByName("VEF.Buildings.HiddenDesignatorsDef");
                var hiddenField = AccessTools.Field(defType, "hiddenDesignators");
                hiddenDesignators = new HashSet<BuildableDef>();
                foreach (Def def in GenDefDatabase.GetAllDefsInDatabaseForDef(defType))
                {
                    var list = (List<BuildableDef>)hiddenField.GetValue(def);
                    for (int i = 0; i < list.Count; i++)
                    {
                        hiddenDesignators.Add(list[i]);
                    }
                }
            }
        }

        public static bool IsHiddenDesignator(BuildableDef def)
        {
            if (!Active)
                return false;
            return hiddenDesignators.Contains(def);
        }
    }
}
