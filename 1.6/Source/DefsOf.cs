using RimWorld;

namespace WantsAndQuirks
{
    [DefOf]
    public static class DefsOf
    {
        public static RewardDef WQ_Quirk_EarlyBird;
        static DefsOf() => DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
    }
}
