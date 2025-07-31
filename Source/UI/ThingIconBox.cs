using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LordKuper.Common.UI
{
    /// <summary>
    ///     Provides utility methods for rendering a scrollable box of thing icons with tooltips and click actions.
    /// </summary>
    public static class ThingIconBox
    {
        /// <summary>
        ///     The gap in pixels between thing icons.
        /// </summary>
        private const float ThingIconGap = 4f;

        /// <summary>
        ///     The size in pixels of each thing icon.
        /// </summary>
        private const float ThingIconSize = 32f;

        /// <summary>
        ///     Represents the background color used for rendering a thing icon box.
        /// </summary>
        private static readonly Color BackgroundColor = new Color(1f, 1f, 1f, 0.05f);

        /// <summary>
        ///     Represents the color used for outlining a thing icon box.
        /// </summary>
        private static readonly Color OutlineColor = new Color(1f, 1f, 1f, 0.4f);

        /// <summary>
        ///     Renders a scrollable grid of <see cref="ThingDef" /> icons within the specified rectangle.
        /// </summary>
        /// <remarks>
        ///     Each <see cref="ThingDef" /> is displayed as an icon in a grid layout. Clicking the
        ///     left mouse button on an icon opens an information dialog for the corresponding <see cref="ThingDef" />.
        ///     Right-clicking an icon triggers the specified <paramref name="rightClickAction" />.
        /// </remarks>
        /// <param name="rect">The area on the screen where the grid will be drawn.</param>
        /// <param name="scrollPosition">A reference to the current scroll position of the grid.</param>
        /// <param name="things">A read-only list of <see cref="ThingDef" /> objects to display in the grid.</param>
        /// <param name="rightClickAction">An action to execute when a <see cref="ThingDef" /> is right-clicked.</param>
        /// <param name="tooltipGetter">
        ///     A function that provides a tooltip string for a given <see cref="ThingDef" />.  The tooltip is displayed when
        ///     the user hovers over an icon.
        /// </param>
        public static void DoThingDefBox(Rect rect, ref Vector2 scrollPosition,
            [NotNull] IReadOnlyList<ThingDef> things, Action<ThingDef> rightClickAction,
            Func<ThingDef, string> tooltipGetter)
        {
            if (things == null) throw new ArgumentNullException(nameof(things));
            var horizontalMargin = GUI.skin.verticalScrollbar.fixedWidth + ThingIconGap * 2;
            var itemsPerRow = (int)Math.Floor((rect.width - horizontalMargin) / (ThingIconSize + ThingIconGap));
            var rowCount = (int)Math.Ceiling((double)things.Count / itemsPerRow);
            Verse.Widgets.DrawBoxSolidWithOutline(rect, BackgroundColor, OutlineColor);
            var outRect = new Rect(rect.x + ThingIconGap, rect.y + ThingIconGap, rect.width - ThingIconGap * 1.5f,
                rect.height - ThingIconGap * 2);
            var itemBoxRect = new Rect(outRect.x, outRect.y, rect.width - horizontalMargin,
                ThingIconSize * rowCount + ThingIconGap * (rowCount - 1));
            Verse.Widgets.BeginScrollView(outRect, ref scrollPosition, itemBoxRect);
            for (var i = 0; i < things.Count; i++)
            {
                var thingDef = things[i];
                var thingRect = GetThingRect(itemBoxRect, itemsPerRow, i);
                GUI.color = !Mouse.IsOver(thingRect) ? Color.white : GenUI.MouseoverColor;
                GUI.DrawTexture(thingRect, thingDef.uiIcon ?? Resources.Textures.BadTexture, ScaleMode.ScaleToFit);
                GUI.color = Color.white;
                MouseoverSounds.DoRegion(thingRect);
                if (tooltipGetter != null)
                    TooltipHandler.TipRegion(thingRect, tooltipGetter(thingDef));
                if (Event.current.type == EventType.MouseDown && Mouse.IsOver(thingRect))
                    switch (Event.current.button)
                    {
                        case 0:
                            if (Current.Game != null) Find.WindowStack.Add(new Dialog_InfoCard(thingDef));
                            break;
                        case 1:
                            rightClickAction?.Invoke(thingDef);
                            break;
                    }
            }
            Verse.Widgets.EndScrollView();
        }

        /// <summary>
        ///     Calculates the total height required for a box containing the specified number of rows of thing icons.
        /// </summary>
        /// <param name="rowCount">The number of rows.</param>
        /// <returns>The total height in pixels.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="rowCount" /> is less than or equal to zero.</exception>
        internal static float GetThingIconBoxHeight(int rowCount)
        {
            if (rowCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(rowCount), "Row count must be a positive number.");
            return rowCount * ThingIconSize + (rowCount + 1) * ThingIconGap;
        }

        /// <summary>
        ///     Calculates the rectangle for a thing icon at the specified index in a grid layout.
        /// </summary>
        /// <param name="rect">The bounding rectangle for the grid.</param>
        /// <param name="columnCount">The number of columns in the grid.</param>
        /// <param name="index">The zero-based index of the icon.</param>
        /// <returns>The rectangle for the icon.</returns>
        private static Rect GetThingRect(Rect rect, int columnCount, int index)
        {
            var rowIndex = Math.DivRem(index, columnCount, out var columnIndex);
            return new Rect(rect.x + (ThingIconSize + ThingIconGap) * columnIndex,
                rect.y + (ThingIconSize + ThingIconGap) * rowIndex, ThingIconSize, ThingIconSize);
        }
    }
}