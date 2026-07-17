using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WantsAndQuirks
{
    public class WantDef : Def
    {
        public Type workerClass = typeof(WantWorker);
        public string iconPath;
        public int reward = 1000;
        public float commonality = 1f;
        public ThoughtDef completedByThought;
        public RoomStatDef roomStat;
        public float roomStatThreshold;
        public List<XenotypeDef> invalidXenotypes;
        public List<XenotypeDef> requiredXenotypes;
        public List<TraitDef> invalidTraits;
        public List<GeneDef> invalidGenes;
        public bool invalidNonViolent;
        public TechLevel minimumTechLevel = TechLevel.Undefined;
        public TechLevel maximumTechLevel = TechLevel.Undefined;
        public string fulfilledText;

        [Unsaved(false)]
        private WantWorker workerInt;
        [Unsaved(false)]
        private Texture2D iconInt;

        public WantWorker Worker
        {
            get
            {
                if (workerInt == null)
                {
                    workerInt = (WantWorker)Activator.CreateInstance(workerClass);
                    workerInt.def = this;
                }
                return workerInt;
            }
        }

        public Texture2D Icon => iconInt ??= ContentFinder<Texture2D>.Get(iconPath);
    }
}
