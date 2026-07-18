using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld.Planet;

namespace WantsAndQuirks
{
    [HarmonyPatch]
    public static class CaravanArrivalAction_Arrived_Patch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(CaravanArrivalAction_VisitSettlement), nameof(CaravanArrivalAction_VisitSettlement.Arrived));
            yield return AccessTools.Method(typeof(CaravanArrivalAction_Enter), nameof(CaravanArrivalAction_Enter.Arrived));
        }

        public static void Postfix(Caravan caravan)
        {
            foreach (var pawn in caravan.PawnsListForReading)
            {
                if (pawn.CanHaveWants())
                {
                    WantsAndQuirksUtility.CheckWants(pawn, new WantWorkerContext(triggerType: WantTriggerType.SawNewPlace));
                }
            }
        }
    }
}
