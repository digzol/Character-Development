using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class WantWorker
    {
        public WantDef def;

        public bool CanHaveWant(Pawn pawn)
        {
            if (def.minimumTechLevel != TechLevel.Undefined && (int)Faction.OfPlayer.def.techLevel < (int)def.minimumTechLevel) return false;
            if (def.maximumTechLevel != TechLevel.Undefined && (int)Faction.OfPlayer.def.techLevel > (int)def.maximumTechLevel) return false;
            return def.PassesRecipientFilter(pawn);
        }
        public virtual bool IsSatisfied(Pawn pawn)
        {
            return false;
        }
        public virtual bool CanGenerate(Pawn pawn)
        {
            return !IsSatisfied(pawn);
        }

        public virtual bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return IsSatisfied(pawn);
        }
    }
}
