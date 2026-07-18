using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WantsAndQuirks
{
    public class ActiveWant : IExposable
    {
        public WantDef def;
        public int assignedTick;
        public bool isMentalBreak;

        public virtual string LabelCap => def.LabelCap;
        public virtual string Description => def.description;

        public virtual Texture Icon
        {
            get
            {
                if (def.discoveryRequirementThing != null)
                    return def.discoveryRequirementThing.uiIcon;
                return def.Icon;
            }
        }

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref assignedTick, "assignedTick");
            Scribe_Values.Look(ref isMentalBreak, "isMentalBreak", false);
        }

        public virtual bool IsCompleted(Pawn pawn, WantWorkerContext context)
        {
            return def.Worker.IsCompleted(pawn, context);
        }
    }

    public class ActiveWantWithTarget : ActiveWant
    {
        public Def targetDef;
        private string targetDefName;
        private string targetDefTypeName;

        public override string LabelCap => def.label.Formatted(targetDef.label).CapitalizeFirst();
        public override string Description => def.description.Formatted(targetDef.label);

        public override Texture Icon
        {
            get
            {
                if (targetDef is ThingDef tDef)
                    return tDef.uiIcon;
                if (targetDef is XenotypeDef xDef)
                    return xDef.Icon;
                return base.Icon;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                targetDefName = targetDef.defName;
                targetDefTypeName = targetDef.GetType().Name;
            }
            Scribe_Values.Look(ref targetDefName, "targetDef");
            Scribe_Values.Look(ref targetDefTypeName, "targetDefType");
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                var type = GenTypes.GetTypeInAnyAssembly(targetDefTypeName);
                targetDef = GenDefDatabase.GetDefSilentFail(type, targetDefName);
            }
        }

        public override bool IsCompleted(Pawn pawn, WantWorkerContext context)
        {
            if (def.Worker.IsTargetDiscovered(targetDef))
            {
                return true;
            }
            if (targetDef == context.contextDef)
            {
                return def.Worker.IsCompleted(pawn, context);
            }
            if (def.Worker.IsSatisfiedWithTarget(pawn, targetDef))
            {
                return true;
            }
            if (context.triggerType != WantTriggerType.None)
            {
                return false;
            }
            return base.IsCompleted(pawn, context);
        }
    }

    public class ActiveWantWithPawnTarget : ActiveWant
    {
        public Pawn targetPawn;

        public override string LabelCap => def.label.Formatted(targetPawn.LabelShort).CapitalizeFirst();
        public override string Description => def.description.Formatted(targetPawn.LabelShort);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref targetPawn, "targetPawn");
        }

        public override bool IsCompleted(Pawn pawn, WantWorkerContext context)
        {
            if (targetPawn == context.contextPawn)
            {
                return def.Worker.IsCompleted(pawn, context);
            }
            if (def.Worker.IsSatisfiedWithPawnTarget(pawn, targetPawn))
            {
                return true;
            }
            if (context.triggerType != WantTriggerType.None)
            {
                return false;
            }
            return base.IsCompleted(pawn, context);
        }
    }

    public class PawnWantsData : IExposable
    {
        public List<ActiveWant> activeWants;
        public List<Quirk> quirks;
        public int nextWantTick;

        public PawnWantsData()
        {
            activeWants = new List<ActiveWant>();
            quirks = new List<Quirk>();
            nextWantTick = -1;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref activeWants, "activeWants", LookMode.Deep);
            Scribe_Collections.Look(ref quirks, "quirks", LookMode.Deep);
            Scribe_Values.Look(ref nextWantTick, "nextWantTick", -1);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                activeWants ??= new List<ActiveWant>();
                quirks ??= new List<Quirk>();
                activeWants.RemoveAll(w => w.def == null || (w is ActiveWantWithTarget t && t.targetDef == null) || (w is ActiveWantWithPawnTarget tp && tp.targetPawn == null));
                quirks.RemoveAll(q => q.def == null || (q.def.requiresItem && q.item == null) || (q.def.requiresPawn && q.pawnTarget == null));
            }
        }
    }
}
