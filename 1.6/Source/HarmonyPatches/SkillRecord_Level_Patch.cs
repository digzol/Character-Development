using HarmonyLib;
using RimWorld;

namespace WantsAndQuirks
{
    [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Level), MethodType.Setter)]
    public static class SkillRecord_Level_Patch
    {
        public static void Postfix(SkillRecord __instance)
        {
            var pawn = __instance.Pawn;
            if (pawn.CanHaveWants())
            {
                WantsAndQuirksUtility.CheckWants(pawn, new WantWorkerContext(triggerType: WantTriggerType.SkillIncreased, contextDef: __instance.def, contextAmount: __instance.Level));
            }
        }
    }
}
