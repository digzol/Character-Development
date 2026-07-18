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
        FoodEaten,
        Traded,
        RecipeCompleted,
        SkillIncreased,
        XenotypeChanged,
        RitualCompleted
    }

    public struct WantWorkerContext
    {
        public WantTriggerType triggerType;
        public Def contextDef;
        public Pawn contextPawn;
        public int contextAmount;
        public string contextString;

        public WantWorkerContext(WantTriggerType triggerType, Def contextDef = null, Pawn contextPawn = null, int contextAmount = 0, string contextString = null)
        {
            this.triggerType = triggerType;
            this.contextDef = contextDef;
            this.contextPawn = contextPawn;
            this.contextAmount = contextAmount;
            this.contextString = contextString;
        }
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

        public static void CheckWants(Pawn pawn, WantWorkerContext context)
        {
            var data = pawn.GetWantsData();
            for (int i = data.activeWants.Count - 1; i >= 0; i--)
            {
                var want = data.activeWants[i];
                if (want.IsCompleted(pawn, context))
                {
                    CompleteWant(pawn, data, want);
                }
            }
        }

        public static void AddCharacterPoints(int amount)
        {
            State.characterPoints = Mathf.Max(State.characterPoints + amount, 0);
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
                var text = !string.IsNullOrEmpty(want.def.fulfilledText) ? want.def.fulfilledText.Formatted(pawn.Named("PAWN"), want.LabelCap) : "WQ_WantCompleted".Translate(pawn.Named("PAWN"), want.LabelCap);
                Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent, false);
                DefsOf.WQ_WantCompleted.PlayOneShotOnCamera();
            }
            data.activeWants.Remove(want);
        }

        public static void GenerateGlobalRewardBubbles()
        {
            var list = new List<RewardNode>();
            var total = WantsAndQuirksMod.settings.bubblesPerRoll;
            var legendaryCount = 0;
            var rareCount = 0;
            var uncommonCount = 0;
            var commonCount = 0;

            for (int i = 0; i < total; i++)
            {
                var roll = Rand.Value;
                if (roll < 0.05f)
                {
                    legendaryCount++;
                }
                else if (roll < 0.20f)
                {
                    rareCount++;
                }
                else if (roll < 0.50f)
                {
                    uncommonCount++;
                }
                else
                {
                    commonCount++;
                }
            }

            FillRewardSlots(list, RewardRarity.Legendary, ref legendaryCount);
            FillRewardSlots(list, RewardRarity.Rare, ref rareCount);
            FillRewardSlots(list, RewardRarity.Uncommon, ref uncommonCount);
            FillRewardSlots(list, RewardRarity.Common, ref commonCount);

            while (list.Count < total)
            {
                var node = GenerateNodeForRarity(list, RewardRarity.Common) ??
                           GenerateNodeForRarity(list, RewardRarity.Uncommon) ??
                           GenerateNodeForRarity(list, RewardRarity.Rare) ??
                           GenerateNodeForRarity(list, RewardRarity.Legendary);
                if (node == null)
                    break;
                list.Add(node);
            }

            State.rewardNodes = list;
        }

        private static void FillRewardSlots(List<RewardNode> list, RewardRarity rarity, ref int count)
        {
            var limit = count;
            for (int i = 0; i < limit; i++)
            {
                var node = GenerateNodeForRarity(list, rarity);
                if (node != null)
                {
                    list.Add(node);
                }
                else
                {
                    count--;
                }
            }
        }

        public static RewardNode GenerateSingleRewardBubble(List<RewardNode> existingNodes)
        {
            var roll = Rand.Value;
            var rarity = RewardRarity.Common;
            if (roll < 0.05f)
            {
                rarity = RewardRarity.Legendary;
            }
            else if (roll < 0.20f)
            {
                rarity = RewardRarity.Rare;
            }
            else if (roll < 0.50f)
            {
                rarity = RewardRarity.Uncommon;
            }

            return GenerateNodeForRarity(existingNodes, rarity) ??
            GenerateNodeForRarity(existingNodes, RewardRarity.Common) ??
            GenerateNodeForRarity(existingNodes, RewardRarity.Uncommon) ??
            GenerateNodeForRarity(existingNodes, RewardRarity.Rare) ??
            GenerateNodeForRarity(existingNodes, RewardRarity.Legendary);
        }

        private static RewardNode GenerateNodeForRarity(List<RewardNode> existingNodes, RewardRarity rarity)
        {
            var map = Find.AnyPlayerHomeMap ?? Find.CurrentMap;
            if (map == null)
                return null;

            var validDefs = DefDatabase<RewardDef>.AllDefsListForReading.Where(rDef =>
            {
                if (rDef.rarity != rarity)
                    return false;
                if (rDef.requiresItem)
                {
                    var items = rDef.Worker.GetValidItems(map);
                    return items.Any(item => !existingNodes.Any(n => n.def == rDef && n.item == item));
                }
                return !existingNodes.Any(n => n.def == rDef);
            }).ToList();

            if (validDefs.TryRandomElement(out var chosenDef))
            {
                ThingDef chosenItem = null;
                if (chosenDef.requiresItem)
                {
                    var validItems = chosenDef.Worker.GetValidItems(map).Where(item => !existingNodes.Any(n => n.def == chosenDef && n.item == item));
                    chosenItem = validItems.RandomElement();
                }

                var node = new RewardNode
                {
                    def = chosenDef,
                    item = chosenItem,
                    pos = new Vector2(Rand.Range(-100f, 100f), Rand.Range(-100f, 100f))
                };
                node.drawPos = node.pos;
                return node;
            }
            return null;
        }

        public static void AddWant(Pawn pawn, PawnWantsData data, WantDef def, Def targetDef = null, bool sendNotification = true)
        {
            var want = targetDef != null ? new ActiveWantWithTarget { def = def, targetDef = targetDef, assignedTick = Find.TickManager.TicksGame } : new ActiveWant { def = def, assignedTick = Find.TickManager.TicksGame };
            data.activeWants.Add(want);
            if (sendNotification && PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("WQ_NewWantGenerated".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent, false);
            }
        }

        public static void AddWantWithPawnTarget(Pawn pawn, PawnWantsData data, WantDef def, Pawn targetPawn, bool sendNotification = true)
        {
            var want = new ActiveWantWithPawnTarget { def = def, targetPawn = targetPawn, assignedTick = Find.TickManager.TicksGame };
            data.activeWants.Add(want);
            if (sendNotification && PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("WQ_NewWantGenerated".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent, false);
            }
        }

        public static bool GenerateRandomWant(Pawn pawn, PawnWantsData data, bool sendNotification = true)
        {
            var availableDefs = DefDatabase<WantDef>.AllDefsListForReading.Where(x => !x.isMentalBreakWant && !data.activeWants.Any(w => w.def == x) && x.Worker.CanHaveWant(pawn) && x.Worker.CanGenerate(pawn)).ToList();
            if (availableDefs.TryRandomElementByWeight(x => x.commonality, out var chosenDef))
            {
                var targetPawn = chosenDef.Worker.GetRandomTargetPawn(pawn);
                if (targetPawn != null)
                {
                    AddWantWithPawnTarget(pawn, data, chosenDef, targetPawn, sendNotification);
                    return true;
                }

                var targetDef = chosenDef.Worker.GetRandomTarget(pawn);
                AddWant(pawn, data, chosenDef, targetDef, sendNotification);
                return true;
            }
            return false;
        }

        public static void AddQuirk(Pawn pawn, RewardDef def, ThingDef item)
        {
            var data = pawn.GetWantsData();
            if (data.quirks.Any(q => q.def == def && q.item == item))
            {
                return;
            }
            var quirk = new Quirk(def, item);
            if (def.isQuirk)
            {
                data.quirks.Add(quirk);
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

            CheckWants(pawn, new WantWorkerContext(WantTriggerType.None));

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
