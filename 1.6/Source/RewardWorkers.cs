using System.Linq;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class RewardWorker_RandomSkill : RewardWorker
    {
        public override void OnAcquired(Pawn pawn)
        {
            if (pawn.skills.skills.Where(s => !s.TotallyDisabled && s.Level < 20).TryRandomElement(out var skill))
            {
                skill.Level++;
            }
        }
    }

    public class RewardWorker_BoostImmuneSystem : RewardWorker
    {
        public override void OnAcquired(Pawn pawn)
        {
            pawn.health.AddHediff(def.hediff);
        }
    }

    public class RewardWorker_RandomInspiration : RewardWorker
    {
        public override void OnAcquired(Pawn pawn)
        {
            var inspirationDef = pawn.mindState.inspirationHandler.GetRandomAvailableInspirationDef();
            if (inspirationDef != null)
            {
                pawn.mindState.inspirationHandler.TryStartInspiration(inspirationDef);
            }
        }
    }

    public class RewardWorker_HediffQuirk : RewardWorker
    {
        public override void OnAcquired(Pawn pawn)
        {
            if (def.hediff != null && pawn.health.hediffSet.GetFirstHediffOfDef(def.hediff) == null)
            {
                pawn.health.AddHediff(def.hediff);
            }
        }

        public override void OnRemoved(Pawn pawn)
        {
            if (def.hediff != null)
            {
                var hd = pawn.health.hediffSet.GetFirstHediffOfDef(def.hediff);
                if (hd != null)
                {
                    pawn.health.RemoveHediff(hd);
                }
            }
        }
    }

    public class QuirkWorker_LikesFood : RewardWorker
    {
        public override void Notify_Ingested(Pawn pawn, Thing ingestible)
        {
            if (ingestible.def.IsNutritionGivingIngestible)
            {
                TryGainThought(pawn);
            }
        }
    }

    public class QuirkWorker_LikesClothing : RewardWorker
    {
        public override void Notify_ApparelAdded(Pawn pawn, Apparel apparel)
        {
            TryGainThought(pawn);
        }
    }

    public class QuirkWorker_LikesWeapon : RewardWorker
    {
        public override void Notify_EquipmentAdded(Pawn pawn, ThingWithComps eq)
        {
            TryGainThought(pawn);
        }
    }
}
