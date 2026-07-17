using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class RewardWorker
    {
        public RewardDef def;

        public virtual bool TryGenerateItem(Map map, out ThingDef item)
        {
            item = null;
            return !def.requiresItem;
        }

        public virtual void OnAcquired(Pawn pawn, Quirk quirk)
        {
        }

        public virtual void OnRemoved(Pawn pawn, Quirk quirk)
        {
        }

        public virtual void Notify_ApparelAdded(Pawn pawn, Quirk quirk, Apparel apparel)
        {
        }

        public virtual void Notify_EquipmentAdded(Pawn pawn, Quirk quirk, ThingWithComps eq)
        {
        }

        public virtual void Notify_Ingested(Pawn pawn, Quirk quirk, Thing ingestible)
        {
        }

        protected void TryGainThought(Pawn pawn, Quirk quirk)
        {
            if (quirk.def.thought != null)
            {
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(quirk.def.thought);
            }
        }
    }
}
