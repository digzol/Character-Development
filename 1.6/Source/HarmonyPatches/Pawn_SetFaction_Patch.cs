using HarmonyLib;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.SetFaction))]
    public static class Pawn_SetFaction_Patch
    {
        public static void Prefix(Pawn __instance, Faction newFaction)
        {
            if (__instance.Faction == Faction.OfPlayer && newFaction != Faction.OfPlayer)
            {
                if (__instance.CanHaveWants())
                {
                    WantsAndQuirksUtility.CheckWants(__instance, new WantWorkerContext(triggerType: WantTriggerType.LeftFaction));
                }
            }
        }
    }
}
