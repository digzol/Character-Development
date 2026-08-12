using HarmonyLib;
using RimWorld;
using Verse;

namespace WantsAndQuirks
{
    [HarmonyPatch(typeof(CameraJumper), nameof(CameraJumper.TryJumpAndSelect))]
    public static class CameraJumper_TryJumpAndSelect_Patch
    {
        public static void Postfix()
        {
            if (Message_Draw_Patch.drawingMessage == null)
                return;
            if (!WantsAndQuirksUtility.wantMessages.TryGetValue(Message_Draw_Patch.drawingMessage, out var pawn))
                return;
            if (!pawn.CanHaveWants())
                return;
            Find.MainTabsRoot.SetCurrentTab(DefsOf.WQ_CharactersMenu);
            InspectPaneUtility.OpenTab(typeof(ITab_Pawn_WantsAndQuirks));
        }
    }
}
