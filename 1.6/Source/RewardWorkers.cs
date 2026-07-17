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
            var inspirationDef = def.inspirationDef ?? pawn.mindState.inspirationHandler.GetRandomAvailableInspirationDef();
            if (inspirationDef != null)
            {
                pawn.mindState.inspirationHandler.TryStartInspiration(inspirationDef);
            }
        }
    }

    public class RewardWorker_ImproveComposure : RewardWorker
    {
        public override void OnAcquired(Pawn pawn, Quirk quirk)
        {
            TherapyCompat.ImproveComposure(pawn);
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

    public class RewardWorker_HealScar : RewardWorker
    {
        public override bool CanBestowOn(Pawn pawn)
        {
            return base.CanBestowOn(pawn) && pawn.health.hediffSet.hediffs.Any(h => h.IsPermanent());
        }

        public override void OnAcquired(Pawn pawn, Quirk quirk)
        {
            if (pawn.health.hediffSet.hediffs.Where(h => h.IsPermanent()).TryRandomElement(out var scar))
            {
                pawn.health.RemoveHediff(scar);
                Messages.Message("WQ_ScarHealed".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
    }

    public class RewardWorker_RestoreBodyPart : RewardWorker
    {
        public override bool CanBestowOn(Pawn pawn)
        {
            return base.CanBestowOn(pawn) && pawn.health.hediffSet.GetMissingPartsCommonAncestors().Any();
        }

        public override void OnAcquired(Pawn pawn, Quirk quirk)
        {
            if (pawn.health.hediffSet.GetMissingPartsCommonAncestors().TryRandomElement(out var missing))
            {
                pawn.health.RestorePart(missing.Part, null, true);
                Messages.Message("WQ_PartRestored".Translate(pawn.Named("PAWN"), missing.Part.def.label), pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
    }

    public class RewardWorker_RandomPassion : RewardWorker
    {
        public override bool CanBestowOn(Pawn pawn)
        {
            return base.CanBestowOn(pawn) && pawn.skills.skills.Any(s => s.passion != Passion.Major && !s.TotallyDisabled);
        }

        public override void OnAcquired(Pawn pawn, Quirk quirk)
        {
            if (pawn.skills.skills.Where(s => s.passion != Passion.Major && !s.TotallyDisabled).TryRandomElement(out var skill))
            {
                skill.passion = skill.passion == Passion.None ? Passion.Minor : Passion.Major;
                Messages.Message("WQ_PassionGained".Translate(pawn.Named("PAWN"), skill.def.label), pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
    }

    public class QuirkWorker_LikesFood : RewardWorker
    {
        public override IEnumerable<ThingDef> GetValidItems(Map map)
        {
            var things = map.listerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
            return things.Select(t => t.def)
                .Where(d => d.IsNutritionGivingIngestible && !d.IsCorpse)
                .Distinct();
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
        public override IEnumerable<ThingDef> GetValidItems(Map map)
        {
            var things = map.listerThings.ThingsInGroup(ThingRequestGroup.Apparel);
            return things.Select(t => t.def).Distinct();
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
        public override IEnumerable<ThingDef> GetValidItems(Map map)
        {
            var weapons = map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon).Select(t => t.def);
            var eq = map.mapPawns.FreeColonists.SelectMany(p => p.equipment.AllEquipmentListForReading).Select(t => t.def);
            return weapons.Concat(eq).Distinct();
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
