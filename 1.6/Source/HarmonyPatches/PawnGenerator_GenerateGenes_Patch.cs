using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GenerateGenes))]
    public static class PawnGenerator_GenerateGenes_Patch
    {
        public static void Postfix(Pawn pawn)
        {
            if (pawn.genes == null || (!pawn.IsColonist && !pawn.IsSlaveOfColony))
                return;

            var mother = pawn.GetMother();
            var father = pawn.GetFather();
            if (mother == null && father == null)
                return;

            var data = pawn.GetWantsData();
            var parents = new List<Pawn> { mother, father }.Where(x => x != null).ToList();
            foreach (var gene in pawn.genes.GenesListForReading)
            {
                var quirkDef = GetParentQuirkDef(parents, gene.def);
                if (quirkDef != null && !data.HasQuirk(quirkDef, null, null))
                {
                    var quirk = new Quirk(quirkDef);
                    data.quirks.Add(quirk);
                    data.grantedGenes.Add(new GrantedGeneLink(gene, quirk));
                }
            }
        }

        private static RewardDef GetParentQuirkDef(List<Pawn> parents, GeneDef geneDef)
        {
            foreach (var parent in parents)
            {
                foreach (var link in parent.GetWantsData().grantedGenes)
                {
                    if (link.gene?.def == geneDef && link.quirk != null)
                        return link.quirk.def;
                }
            }
            return null;
        }
    }
}
