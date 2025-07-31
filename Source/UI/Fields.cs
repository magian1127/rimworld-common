using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace LordKuper.Common.UI
{
    /// <summary>
    ///     Provides utility methods and constants for rendering input fields in the UI.
    /// </summary>
    [UsedImplicitly]
    public static class Fields
    {
        /// <summary>
        ///     The height of a float slider field row.
        /// </summary>
        private const float FloatSliderHeight = Layout.RowHeight;

        /// <summary>
        ///     Draws a labeled float slider with optional action buttons.
        /// </summary>
        /// <param name="rect">The rectangle in which to draw the field.</param>
        /// <param name="actionButtons">A collection of action buttons to display next to the label.</param>
        /// <param name="label">The label text.</param>
        /// <param name="labelTooltip">The tooltip for the label.</param>
        /// <param name="value">The float value to be modified by the slider.</param>
        /// <param name="minValue">The minimum value of the slider.</param>
        /// <param name="maxValue">The maximum value of the slider.</param>
        /// <param name="step">The step size for the slider.</param>
        /// <param name="remRect">The remaining rectangle after drawing the field.</param>
        /// <returns>The height of the row used for the field.</returns>
        [UsedImplicitly]
        public static float DoLabeledFloatSlider(Rect rect, IReadOnlyCollection<ActionButton> actionButtons,
            string label, string labelTooltip, ref float value, float minValue, float maxValue, float step,
            out Rect remRect)
        {
            var font = Text.Font;
            var anchor = Text.Anchor;
            Text.Font = GameFont.Small;
            var rowRect = Layout.GetTopRowRect(rect, FloatSliderHeight, out remRect).ContractedBy(4f);
            GetLabeledFieldRects(rowRect, 0.5f, out var labelRect, out var inputRect);
            if (actionButtons != null && actionButtons.Count > 0)
            {
                var buttonsRect = Layout.GetLeftColumnRect(labelRect,
                    (Buttons.FieldIconButtonSize + Layout.ElementGapSmall) * actionButtons.Count, out labelRect);
                foreach (var actionButton in actionButtons)
                {
                    var buttonRect =
                        Layout.GetLeftColumnRect(buttonsRect, Buttons.FieldIconButtonSize, out buttonsRect);
                    Buttons.DoIconButton(buttonRect, actionButton.Icon, actionButton.Action, actionButton.Tooltip,
                        actionButton.IsEnabled);
                    Layout.GetLeftColumnRect(buttonsRect, Layout.ElementGapSmall, out buttonsRect);
                }
            }
            if (!string.IsNullOrWhiteSpace(labelTooltip))
                TooltipHandler.TipRegion(labelRect, labelTooltip);
            Verse.Widgets.LabelFit(labelRect, label);
            Verse.Widgets.HorizontalSlider(inputRect, ref value, new FloatRange(minValue, maxValue), $"{value:N2}",
                step);
            Text.Font = font;
            Text.Anchor = anchor;
            return rowRect.height;
        }

        /// <summary>
        ///     Splits a row rectangle into label and input rectangles based on the label width factor.
        /// </summary>
        /// <param name="rowRect">The full row rectangle.</param>
        /// <param name="labelWidthFactor">The fraction of the row width to use for the label (between 0.25 and 0.75).</param>
        /// <param name="labelRect">The output rectangle for the label.</param>
        /// <param name="inputRect">The output rectangle for the input field.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="labelWidthFactor" /> is outside [0.25..0.75].</exception>
        private static void GetLabeledFieldRects(Rect rowRect, float labelWidthFactor, out Rect labelRect,
            out Rect inputRect)
        {
            if (labelWidthFactor < 0.25f || labelWidthFactor > 0.75f)
                throw new ArgumentOutOfRangeException(nameof(labelWidthFactor),
                    "Label width factor must be in [0.25..0.75] range.");
            labelRect = Layout.GetLeftColumnRect(rowRect, rowRect.width * labelWidthFactor, out inputRect);
            Layout.GetLeftColumnRect(inputRect, Layout.ElementGap, out inputRect);
        }
    }
}