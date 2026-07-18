using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class Thought_Memory_LikesFood : Thought_Memory
    {
        public ThingDef foodDef;

        public override string LabelCap
        {
            get
            {
                if (foodDef != null)
                    return def.stages[0].label.Formatted(foodDef.label).CapitalizeFirst();
                return base.LabelCap;
            }
        }

        public override string Description
        {
            get
            {
                if (foodDef != null)
                    return def.stages[0].description.Formatted(foodDef.label);
                return base.Description;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref foodDef, "foodDef");
        }
    }
}
