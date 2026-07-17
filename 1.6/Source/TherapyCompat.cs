using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    [StaticConstructorOnStartup]
    public static class TherapyCompat
    {
        private static readonly bool therapyActive;
        private static readonly bool traumaActive;

        private static readonly NeedDef stabilityNeedDef;
        private static readonly MethodInfo getTraumaticTraitMethod;

        static TherapyCompat()
        {
            therapyActive = ModsConfig.IsActive("ferny.ProgressionTherapy");
            traumaActive = ModsConfig.IsActive("ferny.TraumaAndIntegrity");

            if (therapyActive)
            {
                stabilityNeedDef = DefDatabase<NeedDef>.GetNamed("PT_MentalStability");
            }
            if (traumaActive)
            {
                getTraumaticTraitMethod = AccessTools.Method(AccessTools.TypeByName("TraumaAndIntegrity.TraumaAndIntegrityCompat"), "GetTraumaticTrait");
            }
        }

        public static void ImproveComposure(Pawn pawn)
        {
            if (!therapyActive) return;
            pawn.needs.TryGetNeed(stabilityNeedDef).CurLevelPercentage += 0.05f;
        }

        public static bool HasLowComposure(Pawn pawn)
        {
            if (!therapyActive) return false;
            return pawn.needs.TryGetNeed(stabilityNeedDef).CurLevelPercentage < 0.6f;
        }

        public static bool HasHighComposure(Pawn pawn)
        {
            if (!therapyActive) return false;
            return pawn.needs.TryGetNeed(stabilityNeedDef).CurLevelPercentage >= 0.6f;
        }

        public static bool HasTraumaticTrait(Pawn pawn)
        {
            if (!traumaActive) return false;
            return getTraumaticTraitMethod.Invoke(null, new object[] { pawn }) != null;
        }
    }
}
