using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace WantsAndQuirks
{
    public enum RewardRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public class RewardDef : Def
    {
        public Type workerClass = typeof(RewardWorker);
        public RewardRarity rarity = RewardRarity.Common;
        public string iconPath;
        public HediffDef hediff;
        public bool isQuirk;
        public bool requiresItem;
        public ThoughtDef thought;
        public SkillDef skill;

        [Unsaved(false)]
        private RewardWorker workerInt;
        [Unsaved(false)]
        private Texture2D iconInt;

        public RewardWorker Worker
        {
            get
            {
                if (workerInt == null)
                {
                    workerInt = (RewardWorker)Activator.CreateInstance(workerClass);
                    workerInt.def = this;
                }
                return workerInt;
            }
        }

        public Texture2D Icon => iconInt ??= ContentFinder<Texture2D>.Get(iconPath, true);
    }
}
