using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class WQ_BaseDef : Def
    {
        public List<XenotypeDef> invalidXenotypes;
        public List<XenotypeDef> requiredXenotypes;
        public List<TraitRequirement> invalidTraits;
        public List<TraitRequirement> requiredTraits;
        public bool requiredTraitsAny;
        public List<GeneDef> invalidGenes;
        public bool invalidNonViolent;

        public bool PassesRecipientFilter(Pawn pawn)
        {
            if (invalidNonViolent && pawn.WorkTagIsDisabled(WorkTags.Violent))
                return false;
            if (invalidTraits != null)
            {
                for (int i = 0; i < invalidTraits.Count; i++)
                    if (invalidTraits[i].HasTrait(pawn))
                        return false;
            }
            if (requiredTraits != null && requiredTraits.Count > 0)
            {
                if (requiredTraitsAny)
                {
                    var any = false;
                    for (int i = 0; i < requiredTraits.Count; i++)
                        if (requiredTraits[i].HasTrait(pawn))
                        { any = true; break; }
                    if (!any)
                        return false;
                }
                else
                {
                    for (int i = 0; i < requiredTraits.Count; i++)
                        if (!requiredTraits[i].HasTrait(pawn))
                            return false;
                }
            }
            if (ModsConfig.BiotechActive && pawn.genes != null)
            {
                if (invalidGenes != null)
                {
                    for (int i = 0; i < invalidGenes.Count; i++)
                        if (pawn.genes.HasActiveGene(invalidGenes[i]))
                            return false;
                }
                if (invalidXenotypes != null && invalidXenotypes.Contains(pawn.genes.Xenotype))
                    return false;
                if (requiredXenotypes != null && !requiredXenotypes.Contains(pawn.genes.Xenotype))
                    return false;
            }
            return true;
        }
    }
}
