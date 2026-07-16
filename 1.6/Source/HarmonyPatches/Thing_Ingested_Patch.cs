using HarmonyLib;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.Ingested))]
    public static class Thing_Ingested_Patch
    {
        public static void Postfix(Thing __instance, Pawn ingester)
        {
            if (ingester.CanHaveWants())
            {
                if (__instance.def.IsDrug)
                {
                    WantsAndQuirksUtility.CheckWants(ingester, WantTriggerType.DrugIngested);
                }
                var data = ingester.GetWantsData();
                foreach (var quirk in data.quirks)
                {
                    quirk.Worker.Notify_Ingested(ingester, __instance);
                }
            }
        }
    }
}
