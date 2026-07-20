using System.Linq;
using Verse;

namespace WantsAndQuirks
{
    public class WantWorker
    {
        public WantDef def;

        public bool CanHaveWant(Pawn pawn)
        {
            if (!def.CanGenerate())
                return false;

            if (def.minimumColonists > 0 && Find.Maps.Where(m => m.IsPlayerHome).SelectMany(m => m.mapPawns.FreeColonistsSpawned).Count() < def.minimumColonists)
                return false;

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

        public virtual bool IsCompleted(Pawn pawn, WantWorkerContext context)
        {
            return IsSatisfied(pawn);
        }

        public virtual Pawn GetRandomTargetPawn(Pawn pawn)
        {
            return null;
        }

        public virtual bool IsSatisfiedWithPawnTarget(Pawn pawn, Pawn targetPawn)
        {
            return false;
        }

        public virtual Def GetRandomTarget(Pawn pawn)
        {
            return null;
        }

        public virtual bool IsTargetDiscovered(Def target) => false;

        public virtual bool IsSatisfiedWithTarget(Pawn pawn, Def targetDef) => false;

        public virtual bool IsValid(Pawn pawn) => true;

        public virtual bool IsValidWithTarget(Pawn pawn, Def targetDef) => true;

        public virtual bool IsValidWithPawnTarget(Pawn pawn, Pawn targetPawn) => true;
    }
}
