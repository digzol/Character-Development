using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class WantWorker_Thought : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            return pawn.needs?.mood?.thoughts?.memories?.GetFirstMemoryOfDef(def.completedByThought) != null;
        }
    }

    public class WantWorker_RoomStat : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            var room = pawn.ownership?.OwnedRoom;
            return room != null && room.GetStat(def.roomStat) >= def.roomStatThreshold;
        }
    }

    public class WantWorker_PrettierRoom : WantWorker_RoomStat
    {
        public override bool CanGenerate(Pawn pawn)
        {
            return pawn.ownership?.OwnedBed != null && !IsSatisfied(pawn);
        }
    }

    public class WantWorker_SeeAurora : WantWorker
    {
        private bool IsAuroraActive(Pawn pawn) => pawn.Spawned && pawn.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Aurora);

        public override bool IsSatisfied(Pawn pawn)
        {
            return IsAuroraActive(pawn) && pawn.Awake() && !pawn.Position.Roofed(pawn.Map);
        }

        public override bool CanGenerate(Pawn pawn)
        {
            return !IsAuroraActive(pawn);
        }
    }

    public class WantWorker_Bionic : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn) => pawn.health?.hediffSet?.CountAddedAndImplantedParts() >= def.countThreshold;
    }

    public class WantWorker_GetMarried : WantWorker_Thought
    {
        public override bool CanGenerate(Pawn pawn)
        {
            return pawn.GetFirstSpouse() == null && pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Fiance) != null;
        }
    }

    public class WantWorker_EquipWeapon : WantWorker
    {
        public override bool CanGenerate(Pawn pawn) => !pawn.WorkTagIsDisabled(WorkTags.Violent);

        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType) => triggerType == WantTriggerType.WeaponEquipped;
    }

    public class WantWorker_TakeDrug : WantWorker
    {
        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType) => triggerType == WantTriggerType.DrugIngested;
    }

    public class WantWorker_NewOutfit : WantWorker
    {
        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType) => triggerType == WantTriggerType.ApparelAdded;
    }

    public class WantWorker_Resurrection : WantWorker
    {
        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType) => triggerType == WantTriggerType.Resurrected;
    }

    public class WantWorker_BondWithAnimal : WantWorker
    {
        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType) => triggerType == WantTriggerType.BondedWithAnimal;
    }

    public class WantWorker_BecomePsycaster : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            return pawn.HasPsylink;
        }
    }

    public class WantWorker_BecomeParent : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            return pawn.relations.ChildrenCount > 0;
        }
    }

    public class WantWorker_Propose : WantWorker
    {
        public override bool CanGenerate(Pawn pawn)
        {
            return pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Lover) != null && pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Fiance) == null && pawn.GetFirstSpouse() == null;
        }

        public override bool IsSatisfied(Pawn pawn)
        {
            return pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Fiance) != null || pawn.GetFirstSpouse() != null;
        }
    }

    public class WantWorker_ColonyWealth : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            return pawn.MapHeld != null && pawn.MapHeld.wealthWatcher.WealthTotal >= def.wealthThreshold;
        }
    }

    public class WantWorker_BecomeNoble : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn) => pawn.royalty.HasAnyTitleIn(Faction.OfEmpire);
    }

    public class WantWorker_BecomeIdeologicalFigure : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn) => pawn.Ideo?.GetRole(pawn) != null;
    }

    public class WantWorker_BecomeLeader : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn) => pawn.Ideo?.GetRole(pawn)?.def == PreceptDefOf.IdeoRole_Leader;
    }

    public class WantWorker_HasHediff : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            var hd = pawn.health.hediffSet.GetFirstHediffOfDef(def.targetHediff);
            return hd != null && hd.Severity >= def.targetHediffSeverity;
        }
    }

    public class WantWorker_CureHediff : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn) => !pawn.health.hediffSet.HasHediff(def.targetHediff);
    }

    public class WantWorker_ImproveComposure : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn) => TherapyCompat.HasHighComposure(pawn);
    }

    public class WantWorker_ResolveTraumaticTrait : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn) => !TherapyCompat.HasTraumaticTrait(pawn);
    }

    public class WantWorker_ThoughtAny : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            if (def.targetThoughts.NullOrEmpty() || pawn.needs?.mood?.thoughts?.memories == null) return false;
            foreach (var t in def.targetThoughts)
            {
                if (pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(t) != null) return true;
            }
            return false;
        }
    }

    public class WantWorker_HasTrait : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            if (def.targetTraits.NullOrEmpty() || pawn.story?.traits == null) return false;
            foreach (var t in def.targetTraits)
            {
                if (pawn.story.traits.HasTrait(t)) return true;
            }
            return false;
        }
    }

    public class WantWorker_Bleeding : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn) => pawn.health?.hediffSet?.BleedRateTotal >= def.targetHediffSeverity;
    }

    public class WantWorker_EquipQuality : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            if (pawn.equipment?.Primary != null && pawn.equipment.Primary.TryGetQuality(out var q1) && q1 >= def.targetQuality) return true;
            if (pawn.apparel?.WornApparel != null)
            {
                var worn = pawn.apparel.WornApparel;
                for (int i = 0; i < worn.Count; i++)
                {
                    if (worn[i].TryGetQuality(out var q2) && q2 >= def.targetQuality) return true;
                }
            }
            return false;
        }
    }

    public class WantWorker_OpinionCount : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            int count = 0;
            var pawns = pawn.MapHeld?.mapPawns?.FreeColonistsSpawned;
            if (pawns == null) return false;
            for (int i = 0; i < pawns.Count; i++)
            {
                var other = pawns[i];
                if (other != pawn && other.relations != null)
                {
                    if (def.opinionThreshold > 0 && other.relations.OpinionOf(pawn) >= def.opinionThreshold) count++;
                    else if (def.opinionThreshold < 0 && other.relations.OpinionOf(pawn) <= def.opinionThreshold) count++;
                }
            }
            return count >= def.countThreshold;
        }
    }

    public class WantWorker_BeautifulLover : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn)
        {
            var partners = LovePartnerRelationUtility.ExistingLovePartners(pawn, allowDead: false);
            for (int i = 0; i < partners.Count; i++)
            {
                if (partners[i].otherPawn.GetStatValue(StatDefOf.PawnBeauty) >= 1f) return true;
            }
            return false;
        }
    }

    public class WantWorker_Inspired : WantWorker
    {
        public override bool IsSatisfied(Pawn pawn) => pawn.InspirationDef != null;
    }

    public class WantWorker_BecomeGrandparent : WantWorker
    {
        public override bool CanGenerate(Pawn pawn) => pawn.relations != null && pawn.relations.ChildrenCount > 0;
        public override bool IsSatisfied(Pawn pawn)
        {
            if (pawn.relations == null) return false;
            foreach (var child in pawn.relations.Children)
            {
                if (child.relations != null && child.relations.ChildrenCount > 0) return true;
            }
            return false;
        }
    }
}
