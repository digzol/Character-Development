using HarmonyLib;
using Verse;

namespace WantsAndQuirks
{
    [HarmonyPatch(typeof(Message), nameof(Message.Draw))]
    public static class Message_Draw_Patch
    {
        public static Message drawingMessage;

        public static void Prefix(Message __instance)
        {
            drawingMessage = __instance;
        }

        public static void Postfix()
        {
            drawingMessage = null;
        }
    }
}
