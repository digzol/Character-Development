using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    [DefOf]
    public static class DefsOf
    {
        public static RewardDef WQ_Quirk_EarlyBird;
        public static RewardDef WQ_Quirk_NightOwl;
        public static SoundDef WQ_WantCompleted;
        static DefsOf() => DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
    }
}
