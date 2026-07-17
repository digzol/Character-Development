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
            if (def.invalidNonViolent && pawn.WorkTagIsDisabled(WorkTags.Violent)) return false;
            if (def.invalidTraits != null)
            {
                for (int i = 0; i < def.invalidTraits.Count; i++)
                    if (pawn.story.traits.HasTrait(def.invalidTraits[i])) return false;
            }
            if (ModsConfig.BiotechActive && pawn.genes != null)
            {
                if (def.invalidGenes != null)
                {
                    for (int i = 0; i < def.invalidGenes.Count; i++)
                        if (pawn.genes.HasActiveGene(def.invalidGenes[i])) return false;
                }
                if (def.invalidXenotypes != null && def.invalidXenotypes.Contains(pawn.genes.Xenotype)) return false;
                if (def.requiredXenotypes != null && !def.requiredXenotypes.Contains(pawn.genes.Xenotype)) return false;
            }
            return true;
        }

        public virtual bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return false;
        }
    }
}
