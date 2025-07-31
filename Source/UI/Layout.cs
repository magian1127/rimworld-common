using JetBrains.Annotations;
using UnityEngine;

namespace LordKuper.Common.UI
{
    /// <summary>
    ///     Provides layout utilities for UI elements.
    /// </summary>
    [UsedImplicitly]
    public static class Layout
    {
        /// <summary>
        ///     The gap in pixels between UI elements.
        /// </summary>
        public const float ElementGap = 12f;

        /// <summary>
        ///     The smaller gap in pixels between UI elements.
        /// </summary>
        public const float ElementGapSmall = 8f;

        /// <summary>
        ///     The height of a horizontal line.
        /// </summary>
        public const float HorizontalLineHeight = ElementGap;

        /// <summary>
        ///     The default height in pixels for a row.
        /// </summary>
        public const float RowHeight = 32f;

        /// <summary>
        ///     The height in pixels for a section layout.
        /// </summary>
        private const float SectionLayoutHeight = ElementGap;

        /// <summary>
        ///     The height in pixels for a labeled section layout.
        /// </summary>
        public static readonly float LabeledSectionLayoutHeight = SectionLayoutHeight + Labels.SectionHeaderHeight;

        /// <summary>
        ///     Draws two horizontal gap lines and returns the remaining rectangle below the lines.
        /// </summary>
        /// <param name="rect">The rectangle in which to draw the lines.</param>
        /// <returns>The remaining rectangle below the lines.</returns>
        public static Rect DoDoubleGapLineHorizontal(Rect rect)
        {
            var color = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.4f);
            var lineRect = GetTopRowRect(rect, ElementGap, out var remRect);
            Verse.Widgets.DrawLineHorizontal(lineRect.x, lineRect.center.y - 2f, lineRect.width);
            Verse.Widgets.DrawLineHorizontal(lineRect.x, lineRect.center.y + 2f, lineRect.width);
            GUI.color = color;
            return remRect;
        }

        /// <summary>
        ///     Draws a horizontal gap line and returns the remaining rectangle below the line.
        /// </summary>
        /// <param name="rect">The rectangle in which to draw the line.</param>
        /// <returns>The remaining rectangle below the line.</returns>
        private static Rect DoGapLineHorizontal(Rect rect)
        {
            var color = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.4f);
            var lineRect = GetTopRowRect(rect, ElementGap, out var remRect);
            Verse.Widgets.DrawLineHorizontal(lineRect.x, lineRect.center.y, lineRect.width);
            GUI.color = color;
            return remRect;
        }

        /// <summary>
        ///     Draws a vertical gap line within the specified rectangle and returns the remaining rectangle.
        /// </summary>
        /// <param name="rect">The rectangle in which to draw the line.</param>
        /// <returns>The remaining rectangle after drawing the vertical line.</returns>
        public static Rect DoGapLineVertical(Rect rect)
        {
            var color = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.4f);
            var lineRect = GetLeftColumnRect(rect, ElementGap, out var remRect);
            Verse.Widgets.DrawLineVertical(lineRect.center.x, lineRect.y, lineRect.height);
            GUI.color = color;
            return remRect;
        }

        /// <summary>
        ///     Creates a vertical gap within the specified rectangle and returns the top gap-sized rectangle.
        /// </summary>
        /// <param name="rect">The original rectangle from which the top row and remaining area are calculated.</param>
        /// <param name="remRect">When this method returns, contains the remaining rectangle after the top row is extracted.</param>
        /// <returns>A <see cref="Rect" /> representing the top row rectangle, adjusted by the vertical gap.</returns>
        internal static Rect DoVerticalGap(Rect rect, out Rect remRect)
        {
            return GetTopRowRect(rect, ElementGap, out remRect);
        }

        /// <summary>
        ///     Returns the bottom row rectangle of the specified height from the given rectangle.
        /// </summary>
        /// <param name="rect">The original rectangle.</param>
        /// <param name="rowHeight">The height of the bottom row to extract.</param>
        /// <param name="remRect">The remaining rectangle above the bottom row.</param>
        /// <returns>The rectangle representing the bottom row.</returns>
        public static Rect GetBottomRowRect(Rect rect, float rowHeight, out Rect remRect)
        {
            remRect = rect;
            remRect.yMax -= rowHeight;
            return new Rect(rect.x, rect.yMax - rowHeight, rect.width, rowHeight);
        }

        /// <summary>
        ///     Calculates and returns the centered row rectangle within the given rectangle, splitting the remaining space
        ///     into top and bottom rectangles.
        /// </summary>
        /// <param name="rect">The source rectangle from which the centered row and remaining areas are calculated.</param>
        /// <param name="rowHeight">The height of the centered row to extract. Must be a positive value.</param>
        /// <param name="topRect">When this method returns, contains the rectangle representing the area above the centered row.</param>
        /// <param name="bottomRect">When this method returns, contains the rectangle representing the area below the centered row.</param>
        /// <returns>
        ///     A <see cref="Rect" /> representing the centered row within the given rectangle. If the height of the source
        ///     rectangle is less than or equal to <paramref name="rowHeight" />, the entire rectangle is returned, and both
        ///     <paramref name="topRect" /> and <paramref name="bottomRect" /> are set to <see cref="Rect.zero" />.
        /// </returns>
        public static Rect GetCenterRowRect(Rect rect, float rowHeight, out Rect topRect, out Rect bottomRect)
        {
            if (rect.height <= rowHeight)
            {
                topRect = bottomRect = Rect.zero;
                return rect;
            }
            var offset = (rect.height - rowHeight) / 2f;
            topRect = GetTopRowRect(rect, offset, out var remRect);
            var rowRect = GetTopRowRect(remRect, rowHeight, out bottomRect);
            return rowRect;
        }

        /// <summary>
        ///     Returns the left column rectangle of the specified width from the given rectangle.
        /// </summary>
        /// <param name="rect">The original rectangle.</param>
        /// <param name="columnWidth">The width of the left column to extract.</param>
        /// <param name="remRect">The remaining rectangle to the right of the left column.</param>
        /// <returns>The rectangle representing the left column.</returns>
        public static Rect GetLeftColumnRect(Rect rect, float columnWidth, out Rect remRect)
        {
            remRect = rect;
            remRect.xMin += columnWidth;
            return new Rect(rect.x, rect.y, columnWidth, rect.height);
        }

        /// <summary>
        ///     Returns the right column rectangle of the specified width from the given rectangle.
        /// </summary>
        /// <param name="rect">The original rectangle.</param>
        /// <param name="columnWidth">The width of the right column to extract.</param>
        /// <param name="remRect">The remaining rectangle to the left of the right column.</param>
        /// <returns>The rectangle representing the right column.</returns>
        public static Rect GetRightColumnRect(Rect rect, float columnWidth, out Rect remRect)
        {
            remRect = rect;
            remRect.xMax -= columnWidth;
            return new Rect(rect.xMax - columnWidth, rect.y, columnWidth, rect.height);
        }

        /// <summary>
        ///     Calculates the rectangle for a section header within the specified area.
        /// </summary>
        /// <param name="rect">The total area from which the section header rectangle is derived.</param>
        /// <param name="remRect">When this method returns, contains the remaining area after the section header is excluded.</param>
        /// <returns>
        ///     A <see cref="Rect" /> representing the area allocated for the section header. The height of the rectangle is
        ///     determined by the predefined section header height.
        /// </returns>
        public static Rect GetSectionHeaderRect(Rect rect, out Rect remRect)
        {
            remRect = rect;
            remRect.yMin += Labels.SectionHeaderHeight;
            return new Rect(rect.x, rect.y, rect.width, Labels.SectionHeaderHeight);
        }

        /// <summary>
        ///     Returns the top row rectangle of the specified height from the given rectangle.
        /// </summary>
        /// <param name="rect">The original rectangle.</param>
        /// <param name="rowHeight">The height of the top row to extract.</param>
        /// <param name="remRect">The remaining rectangle below the top row.</param>
        /// <returns>The rectangle representing the top row.</returns>
        public static Rect GetTopRowRect(Rect rect, float rowHeight, out Rect remRect)
        {
            remRect = rect;
            remRect.yMin += rowHeight;
            return new Rect(rect.x, rect.y, rect.width, rowHeight);
        }
    }
}