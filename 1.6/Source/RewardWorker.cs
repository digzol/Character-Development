using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class RewardWorker
    {
        public RewardDef def;

        public virtual void OnAcquired(Pawn pawn)
        {
        }

        public virtual void OnRemoved(Pawn pawn)
        {
        }

        public virtual void Notify_ApparelAdded(Pawn pawn, Apparel apparel)
        {
        }

        public virtual void Notify_EquipmentAdded(Pawn pawn, ThingWithComps eq)
        {
        }

        public virtual void Notify_Ingested(Pawn pawn, Thing ingestible)
        {
        }

        protected void TryGainThought(Pawn pawn)
        {
            if (def.thought != null)
            {
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(def.thought);
            }
        }
    }
}
