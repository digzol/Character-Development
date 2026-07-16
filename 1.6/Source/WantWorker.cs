using Verse;

namespace WantsAndQuirks
{
    public class WantWorker
    {
        public WantDef def;

        public virtual bool CanGenerate(Pawn pawn)
        {
            return true;
        }

        public virtual bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return false;
        }
    }
}
