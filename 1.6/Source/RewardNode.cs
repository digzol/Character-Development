using UnityEngine;
using Verse;

namespace WantsAndQuirks
{
    public class RewardNode : IExposable
    {
        public RewardDef def;
        public Vector2 pos;
        public Vector2 velocity;
        [Unsaved(false)]
        public Vector2 drawPos;
        [Unsaved(false)]
        public Vector2 dampVelocity;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref pos, "pos");
        }
    }
}
