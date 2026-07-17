using System.Collections.Generic;
using Verse;

namespace WantsAndQuirks
{
    public class ActiveWant : IExposable
    {
        public WantDef def;
        public int assignedTick;
        public bool isMentalBreak;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref assignedTick, "assignedTick");
            Scribe_Values.Look(ref isMentalBreak, "isMentalBreak", false);
        }
    }

    public class PawnWantsData : IExposable
    {
        public List<ActiveWant> activeWants;
        public List<Quirk> quirks;
        public int nextWantTick;

        public PawnWantsData()
        {
            activeWants = new List<ActiveWant>();
            quirks = new List<Quirk>();
            nextWantTick = -1;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref activeWants, "activeWants", LookMode.Deep);
            Scribe_Collections.Look(ref quirks, "quirks", LookMode.Deep);
            Scribe_Values.Look(ref nextWantTick, "nextWantTick", -1);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                activeWants ??= new List<ActiveWant>();
                quirks ??= new List<Quirk>();
                quirks.RemoveAll(q => q.def == null || (q.def.requiresItem && q.item == null));
            }
        }
    }
}
