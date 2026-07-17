using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WantsAndQuirks
{
    [HotSwappable]
    public class ITab_Pawn_WantsAndQuirks : ITab
    {
        private Vector2 wantsScrollPos;
        private Vector2 quirksScrollPos;

        private static Color BgColor => new ColorInt(28, 30, 31).ToColor;
        private static Color WantBgColor => new ColorInt(79, 82, 84).ToColor;
        private static Color QuirkContainerColor => new ColorInt(123, 121, 118).ToColor;
        private static Color QuirkBgColor => new ColorInt(70, 68, 66).ToColor;
        private static Color PointsColor => new ColorInt(166, 187, 194).ToColor;
        private static Color MentalBreakTextColor => new ColorInt(184, 133, 134).ToColor;

        public ITab_Pawn_WantsAndQuirks()
        {
            labelKey = "WQ_Wants";
            size = new Vector2(600f, 390f);
        }

        public override bool IsVisible
        {
            get
            {
                if (!WantsAndQuirksMod.settings.enableCharactersMenu)
                    return false;
                var pawn = SelPawn;
                return pawn != null && pawn.CanHaveWants() && pawn.Faction == Faction.OfPlayer;
            }
        }

        public override void FillTab()
        {
            var pawn = SelPawn;
            size = new Vector2(600f, 400f);
            var data = pawn.GetWantsData();
            var rect = new Rect(0f, 0f, size.x, size.y);

            Widgets.DrawBoxSolid(rect, BgColor);
            rect = rect.ContractedBy(10f);

            var leftRect = new Rect(rect.x, rect.y, rect.width * 0.70f, rect.height);
            var rightRect = new Rect(leftRect.xMax, rect.y, rect.width * 0.30f, rect.height);

            DrawWants(leftRect, pawn, data);
            DrawQuirks(rightRect, pawn, data);
        }

        private void DrawWants(Rect rect, Pawn pawn, PawnWantsData data)
        {
            var curY = rect.y;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(rect.x, curY, rect.width, 30f), "WQ_PawnWants".Translate(pawn));
            curY += 35f;

            if (data.activeWants.Count == 0)
            {
                Text.Font = GameFont.Small;
                GUI.color = Color.gray;
                Widgets.Label(new Rect(rect.x, curY, rect.width, 30f), "WQ_NoActiveWants".Translate());
                GUI.color = Color.white;
                return;
            }

            var outRect = new Rect(rect.x, curY, rect.width, rect.height - (curY - rect.y));
            var viewRect = new Rect(0f, 0f, outRect.width - 16f, data.activeWants.Count * 85f);
            Widgets.BeginScrollView(outRect, ref wantsScrollPos, viewRect);

            var listY = 0f;
            for (int i = 0; i < data.activeWants.Count; i++)
            {
                var want = data.activeWants[i];
                var wantRect = new Rect(0f, listY, viewRect.width, 80f);

                Widgets.DrawBoxSolid(wantRect, WantBgColor);

                var iconRect = new Rect(wantRect.x + 10f, wantRect.y + 15f, 50f, 50f);
                GUI.color = new Color(1f, 1f, 1f, 0.8f);
                GUI.DrawTexture(iconRect, want.def.Icon);
                GUI.color = Color.white;

                var textRect = new Rect(iconRect.xMax + 15f, wantRect.y + 10, wantRect.width - 70f - 140f, wantRect.height - 10f);
                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(textRect.x, textRect.y, textRect.width, 32f), $"<i>{want.def.LabelCap}</i>");

                Text.Font = GameFont.Tiny;
                GUI.color = new Color(0.9f, 0.9f, 0.9f);
                Widgets.Label(new Rect(textRect.x, textRect.y + 20f, textRect.width, textRect.height - 20f), want.def.description);
                GUI.color = Color.white;

                var infoRect = new Rect(wantRect.xMax - 140f, wantRect.y + 15f, 110f, 50f);
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleCenter;
                if (want.isMentalBreak)
                {
                    GUI.color = MentalBreakTextColor;
                    Widgets.Label(infoRect, "WQ_CausedByMentalBreak".Translate());
                    GUI.color = Color.white;
                }
                else
                {
                    Text.Font = GameFont.Small;
                    Widgets.Label(new Rect(infoRect.x, infoRect.y, infoRect.width, 20f), "WQ_OnCompletion".Translate());
                    GUI.color = PointsColor;
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(new Rect(infoRect.x, infoRect.y + 20f, infoRect.width, 30f), "WQ_CharacterPointsReward".Translate(want.def.reward));
                    GUI.color = Color.white;
                }
                Text.Font = GameFont.Small;

                Text.Anchor = TextAnchor.UpperLeft;

                var btnRect = new Rect(wantRect.xMax - 25f, wantRect.y + 30f, 20f, 20f);
                if (!want.isMentalBreak)
                {
                    Text.Font = GameFont.Medium;
                    GUI.color = Color.gray;
                    if (Widgets.ButtonText(btnRect, "X", drawBackground: false))
                    {
                        data.activeWants.RemoveAt(i);
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        break;
                    }
                    GUI.color = Color.white;
                    Text.Font = GameFont.Small;
                }

                listY += 85f;
            }

            Widgets.EndScrollView();
        }

        private void DrawQuirks(Rect rect, Pawn pawn, PawnWantsData data)
        {
            var curY = rect.y;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(rect.x, curY, rect.width, 30f), "WQ_Quirks".Translate());
            curY += 30f;

            var listRect = new Rect(rect.x, curY, rect.width, rect.height - (curY - rect.y));
            Widgets.DrawBoxSolid(listRect, QuirkContainerColor);

            var viewRect = new Rect(0f, 0f, listRect.width - 16f, data.quirks.Count * 30f);
            Widgets.BeginScrollView(listRect, ref quirksScrollPos, viewRect);

            var listY = 0f;
            for (int i = 0; i < data.quirks.Count; i++)
            {
                var quirk = data.quirks[i];
                var quirkRect = new Rect(5f, listY + 5f, viewRect.width, 25f);

                Widgets.DrawBoxSolid(quirkRect, QuirkBgColor);

                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(new Rect(quirkRect.x + 5f, quirkRect.y, quirkRect.width - 30f, quirkRect.height), quirk.LabelCap);
                Text.Anchor = TextAnchor.UpperLeft;
                if (Mouse.IsOver(quirkRect))
                {
                    TooltipHandler.TipRegion(quirkRect, quirk.Description);
                }

                var btnRect = new Rect(quirkRect.xMax - 20f, quirkRect.y + 2f, 20f, 20f);
                Text.Font = GameFont.Medium;
                GUI.color = Color.gray;
                if (Widgets.ButtonText(btnRect, "x", drawBackground: false))
                {
                    data.quirks[i].def.Worker.OnRemoved(pawn, quirk);
                    data.quirks.RemoveAt(i);
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    break;
                }
                GUI.color = Color.white;
                Text.Font = GameFont.Small;

                listY += 30f;
            }

            Widgets.EndScrollView();
        }
    }
}
