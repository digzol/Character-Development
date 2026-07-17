using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class WantWorker
    {
        public WantDef def;

        public virtual bool CanGenerate(Pawn pawn)
        {
            if (def.minimumTechLevel != TechLevel.Undefined && (int)Faction.OfPlayer.def.techLevel < (int)def.minimumTechLevel) return false;
            if (def.maximumTechLevel != TechLevel.Undefined && (int)Faction.OfPlayer.def.techLevel > (int)def.maximumTechLevel) return false;
            return def.PassesRecipientFilter(pawn);
        }

        public virtual bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return false;
        }
    }
}
