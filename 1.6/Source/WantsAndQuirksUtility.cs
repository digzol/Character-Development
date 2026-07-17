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

        public const int CharacterPointsMax = 1000;

        public static void AddCharacterPoints(int amount)
        {
            State.characterPoints = Mathf.Clamp(State.characterPoints + amount, 0, CharacterPointsMax);
            while (State.characterPoints >= WantsAndQuirksMod.settings.pointsNeededForReward)
            {
                State.characterPoints -= WantsAndQuirksMod.settings.pointsNeededForReward;
                State.rewardPoints++;
                Messages.Message("WQ_RewardPointEarned".Translate(), null, MessageTypeDefOf.PositiveEvent, false);
            }
        }

        public static void CompleteWant(Pawn pawn, PawnWantsData data, ActiveWant want)
        {
            AddCharacterPoints(want.def.reward);
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
                var node = GenerateSingleRewardBubble(list);
                if (node != null)
                {
                    list.Add(node);
                }
            }
            State.rewardNodes = list;
        }

        public static RewardNode GenerateSingleRewardBubble(List<RewardNode> existingNodes)
        {
            var map = Find.AnyPlayerHomeMap ?? Find.CurrentMap;
            if (map is null)
                return null;
            var validDefs = DefDatabase<RewardDef>.AllDefsListForReading.Where(rDef => 
            {
                var items = rDef.Worker.GetValidItems(map);
                return items.Any(item => !existingNodes.Any(n => n.def == rDef && n.item == item));
            });

            if (!validDefs.TryRandomElementByWeight(r => GetRarityWeight(r.rarity), out var chosenDef))
                return null;

            var validItems = chosenDef.Worker.GetValidItems(map).Where(item => !existingNodes.Any(n => n.def == chosenDef && n.item == item));
            var chosenItem = validItems.RandomElement();

            var node = new RewardNode
            {
                def = chosenDef,
                item = chosenItem,
                pos = new Vector2(Rand.Range(-100f, 100f), Rand.Range(-100f, 100f))
            };
            node.drawPos = node.pos;
            return node;
        }

        private static float GetRarityWeight(RewardRarity rarity)
        {
            if (rarity == RewardRarity.Legendary)
            {
                return 0.05f;
            }
            if (rarity == RewardRarity.Rare)
            {
                return 0.2f;
            }
            if (rarity == RewardRarity.Uncommon)
            {
                return 0.5f;
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
            var availableDefs = DefDatabase<WantDef>.AllDefs.Where(x => !data.activeWants.Any(w => w.def == x) && x.Worker.CanHaveWant(pawn) && x.Worker.CanGenerate(pawn)).ToList();
            if (availableDefs.TryRandomElementByWeight(x => x.commonality, out var chosenDef))
            {
                AddWant(pawn, data, chosenDef, sendNotification);
                return true;
            }
            return false;
        }

        public static void AddQuirk(Pawn pawn, RewardDef def, ThingDef item)
        {
            var data = pawn.GetWantsData();
            if (data.quirks.Any(q => q.def == def))
            {
                return;
            }
            var quirk = new Quirk(def, item);
            if (def.isQuirk)
            {
                pawn.GetWantsData().quirks.Add(quirk);
            }
            def.Worker.OnAcquired(pawn, quirk);
        }

        public static void TickWants(Pawn pawn)
        {
            var data = pawn.GetWantsData();

            if (data.nextWantTick == -1)
            {
                InitializePawnWants(pawn, data);
            }

            CheckWants(pawn, WantTriggerType.None);

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
