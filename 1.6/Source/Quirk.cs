using Verse;

namespace WantsAndQuirks
{
    public class Quirk : IExposable
    {
        public RewardDef def;
        public ThingDef item;

        public Quirk() { }

        public Quirk(RewardDef def, ThingDef item = null)
        {
            this.def = def;
            this.item = item;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Defs.Look(ref item, "item");
        }

        public string LabelCap => def.requiresItem ? string.Format(def.LabelCap, item.label).CapitalizeFirst() : def.LabelCap;
        public string Description => def.requiresItem ? string.Format(def.description, item.label) : def.description;
    }
}
