using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    public class RewardWorker_Skill : RewardWorker
    {
        public override void OnAcquired(Pawn pawn, Quirk quirk)
        {
            var record = pawn.skills.GetSkill(def.skill);
            if (!record.TotallyDisabled && record.Level < 20)
            {
                record.Level++;
            }
        }
    }

    public class RewardWorker_RandomSkill : RewardWorker
    {
        public override void OnAcquired(Pawn pawn, Quirk quirk)
        {
            if (pawn.skills.skills.Where(s => !s.TotallyDisabled && s.Level < 20).TryRandomElement(out var skill))
            {
                skill.Level++;
            }
        }
    }

    public class RewardWorker_BoostImmuneSystem : RewardWorker
    {
        public override void OnAcquired(Pawn pawn, Quirk quirk)
        {
            pawn.health.AddHediff(quirk.def.hediff);
        }
    }

    public class RewardWorker_RandomInspiration : RewardWorker
    {
        public override void OnAcquired(Pawn pawn, Quirk quirk)
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
        public override void OnAcquired(Pawn pawn, Quirk quirk)
        {
            if (quirk.def.hediff != null && pawn.health.hediffSet.GetFirstHediffOfDef(quirk.def.hediff) == null)
            {
                pawn.health.AddHediff(quirk.def.hediff);
            }
        }

        public override void OnRemoved(Pawn pawn, Quirk quirk)
        {
            if (quirk.def.hediff != null)
            {
                var hd = pawn.health.hediffSet.GetFirstHediffOfDef(quirk.def.hediff);
                if (hd != null)
                {
                    pawn.health.RemoveHediff(hd);
                }
            }
        }
    }

    public class QuirkWorker_LikesFood : RewardWorker
    {
        public override bool TryGenerateItem(Map map, out ThingDef item)
        {
            return map.listerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree).Select(t => t.def).Where(d => d.IsNutritionGivingIngestible).Distinct().TryRandomElement(out item);
        }

        public override void Notify_Ingested(Pawn pawn, Quirk quirk, Thing ingestible)
        {
            if (ingestible.def == quirk.item)
            {
                TryGainThought(pawn, quirk);
            }
        }
    }

    public class QuirkWorker_LikesClothing : RewardWorker
    {
        public override bool TryGenerateItem(Map map, out ThingDef item)
        {
            return map.listerThings.ThingsInGroup(ThingRequestGroup.Apparel).Select(t => t.def).Distinct().TryRandomElement(out item);
        }

        public override void Notify_ApparelAdded(Pawn pawn, Quirk quirk, Apparel apparel)
        {
            if (apparel.def == quirk.item)
            {
                TryGainThought(pawn, quirk);
            }
        }
    }

    public class QuirkWorker_LikesWeapon : RewardWorker
    {
        public override bool TryGenerateItem(Map map, out ThingDef item)
        {
            var weapons = map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon).Select(t => t.def);
            var eq = map.mapPawns.FreeColonists.SelectMany(p => p.equipment?.AllEquipmentListForReading ?? new List<ThingWithComps>()).Select(t => t.def);
            return weapons.Concat(eq).Distinct().TryRandomElement(out item);
        }

        public override void Notify_EquipmentAdded(Pawn pawn, Quirk quirk, ThingWithComps eq)
        {
            if (eq.def == quirk.item)
            {
                TryGainThought(pawn, quirk);
            }
        }
    }
}
