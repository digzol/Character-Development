using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WantsAndQuirks
{
    public enum WantTriggerType
    {
        None,
        ApparelAdded,
        WeaponEquipped,
        DrugIngested,
        Resurrected,
        BondedWithAnimal,
        FoodEaten
    }

    public static class WantsAndQuirksUtility
    {
        private static readonly ConditionalWeakTable<Pawn, PawnWantsData> pawnData = new ConditionalWeakTable<Pawn, PawnWantsData>();
        public static PawnWantsData GetWantsData(this Pawn pawn)
        {
            if (!pawnData.TryGetValue(pawn, out var data))
            {
                data = new PawnWantsData();
                pawnData.Add(pawn, data);
            }
            return data;
        }

        public static bool CanHaveWants(this Pawn pawn)
        {
            return WantsAndQuirksMod.settings.enableWantsSystem && pawn.DestroyedOrNull() is false && pawn.RaceProps.Humanlike && pawn.IsColonist;
        }

        public static void CheckWants(Pawn pawn, WantTriggerType triggerType)
        {
            var data = pawn.GetWantsData();
            for (int i = data.activeWants.Count - 1; i >= 0; i--)
            {
                var want = data.activeWants[i];
                if (want.def.Worker.IsCompleted(pawn, triggerType))
                {
                    CompleteWant(pawn, data, want);
                }
            }
        }

        public static void CompleteWant(Pawn pawn, PawnWantsData data, ActiveWant want)
        {
            State.characterPoints += want.def.reward;
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                var text = !string.IsNullOrEmpty(want.def.fulfilledText) ? want.def.fulfilledText.Formatted(pawn.Named("PAWN"), want.def.LabelCap) : "WQ_WantCompleted".Translate(pawn.Named("PAWN"), want.def.LabelCap);
                Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent, false);
                DefsOf.WQ_WantCompleted.PlayOneShotOnCamera();
            }
            data.activeWants.Remove(want);
        }

        public static void GenerateGlobalRewardBubbles()
        {
            var list = new List<RewardNode>();
            for (int i = 0; i < WantsAndQuirksMod.settings.bubblesPerRoll; i++)
            {
                list.Add(GenerateSingleRewardBubble());
            }
            State.rewardNodes = list;
        }

        public static RewardNode GenerateSingleRewardBubble()
        {
            var validChoices = new List<Pair<RewardDef, ThingDef>>();
            var map = Find.AnyPlayerHomeMap ?? Find.CurrentMap;
            foreach (var rDef in DefDatabase<RewardDef>.AllDefsListForReading)
            {
                if (rDef.Worker.TryGenerateItem(map, out var item))
                {
                    validChoices.Add(new Pair<RewardDef, ThingDef>(rDef, item));
                }
            }

            var chosen = validChoices.RandomElementByWeight(r => GetRarityWeight(r.First.rarity));
            var node = new RewardNode
            {
                def = chosen.First,
                item = chosen.Second,
                pos = new Vector2(Rand.Range(-100f, 100f), Rand.Range(-100f, 100f))
            };
            node.drawPos = node.pos;
            return node;
        }

        private static float GetRarityWeight(RewardRarity rarity)
        {
            if (rarity == RewardRarity.Legendary)
            {
                return 0.02f;
            }
            if (rarity == RewardRarity.Rare)
            {
                return 0.1f;
            }
            if (rarity == RewardRarity.Uncommon)
            {
                return 0.4f;
            }
            return 1f;
        }

        public static void AddWant(Pawn pawn, PawnWantsData data, WantDef def, bool sendNotification = true)
        {
            data.activeWants.Add(new ActiveWant { def = def, assignedTick = Find.TickManager.TicksGame });
            if (sendNotification && PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("WQ_NewWantGenerated".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent, false);
            }
        }

        public static bool GenerateRandomWant(Pawn pawn, PawnWantsData data, bool sendNotification = true)
        {
            var availableDefs = DefDatabase<WantDef>.AllDefs.Where(x => !data.activeWants.Any(w => w.def == x) && x.Worker.CanGenerate(pawn)).ToList();
            if (availableDefs.TryRandomElementByWeight(x => x.commonality, out var chosenDef))
            {
                AddWant(pawn, data, chosenDef, sendNotification);
                return true;
            }
            return false;
        }

        public static void AddQuirk(Pawn pawn, PawnWantsData data, RewardDef def)
        {
            if (def.isQuirk)
            {
                if (data.quirks.Any(q => q.def == def))
                {
                    return;
                }
                var quirk = new Quirk(def);
                data.quirks.Add(quirk);
                def.Worker.OnAcquired(pawn, quirk);
            }
            else
                def.Worker.OnAcquired(pawn, null);
        }

        public static void TickWants(Pawn pawn)
        {
            var data = pawn.GetWantsData();

            if (data.nextWantTick == -1)
            {
                InitializePawnWants(pawn, data);
            }

            CheckWants(pawn, WantTriggerType.None);

            if (State.characterPoints >= WantsAndQuirksMod.settings.pointsNeededForReward)
            {
                State.characterPoints -= WantsAndQuirksMod.settings.pointsNeededForReward;
                State.rewardPoints++;
                if (PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    Messages.Message("WQ_RewardPointEarned".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent, false);
                }
            }

            if (data.activeWants.Count < 4 && Find.TickManager.TicksGame >= data.nextWantTick)
            {
                GenerateRandomWant(pawn, data);
                data.nextWantTick = Find.TickManager.TicksGame + GetNextWantInterval();
            }
        }

        public static void InitializePawnWants(Pawn pawn, PawnWantsData data)
        {
            data.nextWantTick = Find.TickManager.TicksGame + GetNextWantInterval();
            for (int i = 0; i < WantsAndQuirksMod.settings.startingWantsCount; i++)
            {
                GenerateRandomWant(pawn, data, false);
            }
        }

        private static int GetNextWantInterval()
        {
            var range = WantsAndQuirksMod.settings.wantGenerationFrequencyDays;
            return (int)(Rand.Range(range.min, (float)range.max) * GenDate.TicksPerDay);
        }
    }
}
