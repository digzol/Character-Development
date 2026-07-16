using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    [HarmonyPatch(typeof(MemoryThoughtHandler), nameof(MemoryThoughtHandler.TryGainMemory), new Type[] { typeof(Thought_Memory), typeof(Pawn) })]
    public static class MemoryThoughtHandler_TryGainMemory_Patch
    {
        public static void Postfix(MemoryThoughtHandler __instance, Thought_Memory newThought)
        {
            var pawn = __instance.pawn;
            if (pawn.CanHaveWants())
            {
                var data = pawn.GetWantsData();
                for (int i = data.activeWants.Count - 1; i >= 0; i--)
                {
                    var want = data.activeWants[i];
                    if (want.def.completedByThought == newThought.def)
                    {
                        WantsAndQuirksUtility.CompleteWant(pawn, data, want);
                    }
                }
            }
        }
    }
}
