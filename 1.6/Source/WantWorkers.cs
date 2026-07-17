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
        public override bool IsSatisfied(Pawn pawn) => pawn.health?.hediffSet?.CountAddedAndImplantedParts() > 0;
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
        public override bool IsSatisfied(Pawn pawn) => pawn.health.hediffSet.HasHediff(def.targetHediff);
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
}
