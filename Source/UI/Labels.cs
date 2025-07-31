using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace LordKuper.Common.UI
{
    /// <summary>
    ///     Provides utility methods for drawing labels in the UI.
    /// </summary>
    [UsedImplicitly]
    public static class Labels
    {
        internal static readonly float LabelHeight = Text.LineHeightOf(GameFont.Medium);

        /// <summary>
        ///     The height of a section header.
        /// </summary>
        internal static readonly float SectionHeaderHeight = Text.LineHeightOf(GameFont.Medium) + 4f;

        public static void DoLabel(Rect rect, string label, TextAnchor anchor)
        {
            var font = Text.Font;
            var textAnchor = Text.Anchor;
            Text.Font = GameFont.Medium;
            Text.Anchor = anchor;
            Verse.Widgets.Label(rect, label);
            Text.Font = font;
            Text.Anchor = textAnchor;
        }

        internal static void DoSectionHeader(Rect rect, string header, string tooltip = null)
        {
            var font = Text.Font;
            var anchor = Text.Anchor;
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleLeft;
            Verse.Widgets.Label(rect, header);
            if (!string.IsNullOrWhiteSpace(tooltip))
            {
                var headerWidth = GetTextWidth(header, Text.Font);
                Layout.GetLeftColumnRect(rect, headerWidth + Layout.ElementGapSmall, out var tooltipRect);
                var buttonRect = Layout.GetLeftColumnRect(tooltipRect, Icons.InfoIconSize, out _);
                Icons.DoIcon(buttonRect, TexButton.Info, tooltip);
            }
            Text.Font = font;
            Text.Anchor = anchor;
        }

        /// <summary>
        ///     Returns the rendered width of the specified text using the given font.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font to use for measurement.</param>
        /// <returns>The width in pixels.</returns>
        private static float GetTextWidth(string text, GameFont font)
        {
            var prevFont = Text.Font;
            Text.Font = font;
            var width = Text.CalcSize(text).x;
            Text.Font = prevFont;
            return width;
        }
    }
}