using HarmonyLib;
using UnityEngine;
using Verse;

namespace WantsAndQuirks
{
    public class WantsAndQuirksMod : Mod
    {
        public static WantsAndQuirksSettings settings;

        public WantsAndQuirksMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<WantsAndQuirksSettings>();
            new Harmony("WantsAndQuirksMod").PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }
}
