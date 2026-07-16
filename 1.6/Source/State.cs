using System.Collections.Generic;
using Verse;

namespace WantsAndQuirks
{
    public static class State
    {
        public static int characterPoints;
        public static int rewardPoints;
        public static List<RewardNode> rewardNodes = new List<RewardNode>();

        public static void ExposeData()
        {
            Scribe_Values.Look(ref characterPoints, "WQ_characterPoints", 0);
            Scribe_Values.Look(ref rewardPoints, "WQ_rewardPoints", 0);
            Scribe_Collections.Look(ref rewardNodes, "WQ_rewardNodes", LookMode.Deep);
            rewardNodes ??= new List<RewardNode>();

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                for (int i = 0; i < rewardNodes.Count; i++)
                {
                    rewardNodes[i].drawPos = rewardNodes[i].pos;
                }
            }
        }
    }
}
