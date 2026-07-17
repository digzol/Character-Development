using HarmonyLib;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_ApparelAdded))]
    public static class Pawn_ApparelTracker_Notify_ApparelAdded_Patch
    {
        public static void Postfix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            var pawn = __instance.pawn;
            if (pawn.CanHaveWants())
            {
                WantsAndQuirksUtility.CheckWants(pawn, WantTriggerType.ApparelAdded);
                var data = pawn.GetWantsData();
                foreach (var quirk in data.quirks)
                {
                    quirk.def.Worker.Notify_ApparelAdded(pawn, quirk, apparel);
                }
            }
        }
    }
}
