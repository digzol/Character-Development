using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class WantWorker_Thought : WantWorker
    {
        public override bool CanGenerate(Pawn pawn)
        {
            return pawn.needs?.mood?.thoughts?.memories?.GetFirstMemoryOfDef(def.completedByThought) == null;
        }

        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return pawn.needs?.mood?.thoughts?.memories?.GetFirstMemoryOfDef(def.completedByThought) != null;
        }
    }

    public class WantWorker_RoomStat : WantWorker
    {
        public override bool CanGenerate(Pawn pawn)
        {
            var room = pawn.ownership?.OwnedRoom;
            return room == null || room.GetStat(def.roomStat) < def.roomStatThreshold;
        }

        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            var room = pawn.ownership?.OwnedRoom;
            return room != null && room.GetStat(def.roomStat) >= def.roomStatThreshold;
        }
    }

    public class WantWorker_PrettierRoom : WantWorker_RoomStat
    {
        public override bool CanGenerate(Pawn pawn)
        {
            return base.CanGenerate(pawn) && pawn.ownership?.OwnedBed != null;
        }
    }

    public class WantWorker_SeeAurora : WantWorker
    {
        private bool IsAuroraActive(Pawn pawn)
        {
            return pawn.Spawned && pawn.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Aurora);
        }

        public override bool CanGenerate(Pawn pawn)
        {
            return !IsAuroraActive(pawn);
        }

        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return IsAuroraActive(pawn) && pawn.Awake() && !pawn.Position.Roofed(pawn.Map);
        }
    }

    public class WantWorker_Bionic : WantWorker
    {
        public override bool CanGenerate(Pawn pawn) => pawn.health?.hediffSet?.CountAddedAndImplantedParts() == 0;

        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType) => pawn.health?.hediffSet?.CountAddedAndImplantedParts() > 0;
    }

    public class WantWorker_GetMarried : WantWorker_Thought
    {
        public override bool CanGenerate(Pawn pawn)
        {
            return base.CanGenerate(pawn) && pawn.GetFirstSpouse() == null && pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Fiance) != null;
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
        public override bool CanGenerate(Pawn pawn)
        {
            return base.CanGenerate(pawn) && !pawn.HasPsylink;
        }

        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return pawn.HasPsylink;
        }
    }

    public class WantWorker_BecomeParent : WantWorker
    {
        public override bool CanGenerate(Pawn pawn)
        {
            return base.CanGenerate(pawn) && pawn.relations.ChildrenCount == 0;
        }

        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return pawn.relations.ChildrenCount > 0;
        }
    }

    public class WantWorker_Propose : WantWorker
    {
        public override bool CanGenerate(Pawn pawn)
        {
            return base.CanGenerate(pawn) && pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Lover) != null && pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Fiance) == null && pawn.GetFirstSpouse() == null;
        }

        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Fiance) != null || pawn.GetFirstSpouse() != null;
        }
    }

    public class WantWorker_ColonyWealth : WantWorker
    {
        public override bool CanGenerate(Pawn pawn)
        {
            return base.CanGenerate(pawn) && pawn.MapHeld != null && pawn.MapHeld.wealthWatcher.WealthTotal < def.wealthThreshold;
        }

        public override bool IsCompleted(Pawn pawn, WantTriggerType triggerType)
        {
            return pawn.MapHeld != null && pawn.MapHeld.wealthWatcher.WealthTotal >= def.wealthThreshold;
        }
    }
}
