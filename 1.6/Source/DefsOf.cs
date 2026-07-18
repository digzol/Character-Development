using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    [DefOf]
    public static class DefsOf
    {
        public static RewardDef WQ_Quirk_EarlyBird;
        public static RewardDef WQ_Quirk_NightOwl;
        public static RewardDef WQ_Quirk_LikesClothing;
        public static RewardDef WQ_Quirk_LikesWeapon;
        public static RewardDef WQ_Quirk_LikesPerson;
        public static RewardDef WQ_Quirk_DislikesPerson;
        public static RewardDef WQ_Quirk_LovesPerson;
        public static RewardDef WQ_Quirk_LikesRecreationBuilding;
        public static ThoughtDef WQ_Thought_LikesRecreationBuilding;
        public static SoundDef WQ_WantCompleted;
        static DefsOf() => DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
    }
}
