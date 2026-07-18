using Verse;

namespace WantsAndQuirks
{
    public class Quirk : IExposable
    {
        public RewardDef def;
        public ThingDef item;
        public Pawn pawnTarget;

        public Quirk() { }

        public Quirk(RewardDef def, ThingDef item = null, Pawn pawnTarget = null)
        {
            this.def = def;
            this.item = item;
            this.pawnTarget = pawnTarget;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Defs.Look(ref item, "item");
            Scribe_References.Look(ref pawnTarget, "pawnTarget");
        }

        public string LabelCap => def.requiresItem ? string.Format(def.LabelCap, item.label).CapitalizeFirst() : (def.requiresPawn && pawnTarget != null ? string.Format(def.LabelCap, pawnTarget.LabelShort).CapitalizeFirst() : def.LabelCap);
        public string Description => def.requiresItem ? string.Format(def.description, item.label) : (def.requiresPawn && pawnTarget != null ? string.Format(def.description, pawnTarget.LabelShort) : def.description);
    }
}
