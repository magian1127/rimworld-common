using System;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace LordKuper.Common.UI;

/// <summary>
///     Provides utility methods for drawing icon buttons in the UI.
/// </summary>
public static class Buttons
{
    /// <summary>
    ///     The default height of an action button in pixels.
    /// </summary>
    [UsedImplicitly] public const float ActionButtonHeight = Layout.GridSize * 8;

    /// <summary>
    ///     The minimum width of an action button in pixels.
    /// </summary>
    private const float ActionButtonWidthMin = Layout.GridSize * 50;

    /// <summary>
    ///     The default size (in pixels) for field icon buttons.
    /// </summary>
    internal const float FieldIconButtonSize = Layout.GridSize * 6;

    /// <summary>
    ///     Draws a standard action button with a label and invokes the specified action when clicked.
    /// </summary>
    /// <param name="rect">The rectangle area where the button will be drawn.</param>
    /// <param name="label">The text label to display on the button.</param>
    /// <param name="action">The action to invoke when the button is clicked.</param>
    /// <param name="tooltip">The tooltip text to display when hovering over the button. Can be null.</param>
    /// <param name="isEnabled">Indicates whether the button is enabled and interactive. Default is true.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action" /> is null.</exception>
    [UsedImplicitly]
    public static void DoActionButton(Rect rect, string label, [NotNull] Action action,
        [CanBeNull] string tooltip = null, bool isEnabled = true)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var font = Text.Font;
        Text.Font = GameFont.Small;
        var buttonRect = Layout.GetCenterRowRect(rect, ActionButtonHeight, out _, out _);
        if (Verse.Widgets.ButtonText(buttonRect, label, active: isEnabled)) action.Invoke();
        if (!string.IsNullOrEmpty(tooltip)) TooltipHandler.TipRegion(buttonRect, tooltip);
        Text.Font = font;
    }

    /// <summary>
    ///     Draws a row of action buttons and returns the total height used.
    /// </summary>
    /// <param name="rect">The rectangle area where the button row will be drawn.</param>
    /// <param name="actionButtons">An array of <see cref="ActionButton" />s to display.</param>
    /// <param name="remRect">The remaining rectangle after the row is drawn.</param>
    /// <returns>The height of the button row.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionButtons" /> is null.</exception>
    [UsedImplicitly]
    public static float DoActionButtonGrid(Rect rect, [NotNull] ActionButton[] actionButtons, out Rect remRect)
    {
        if (actionButtons == null) throw new ArgumentNullException(nameof(actionButtons));
        if (actionButtons.Length == 0)
        {
            remRect = rect;
            return 0f;
        }
        var buttonRects = Layout.GetGridRects(rect, ActionButtonWidthMin, Layout.ElementGapSmall, ActionButtonHeight,
            Layout.ElementGapSmall, actionButtons.Length, out var height, out remRect);
        for (var i = 0; i < actionButtons.Length; i++)
        {
            var button = actionButtons[i];
            var buttonRect = buttonRects[i];
            DoActionButton(buttonRect, button.Label, button.Action, button.Tooltip, button.IsEnabled);
        }
        return height;
    }

    /// <summary>
    ///     Draws an icon button and handles its interaction.
    /// </summary>
    /// <param name="rect">The rectangle area where the button will be drawn.</param>
    /// <param name="iconButton">The <see cref="IconButton" /> to display and handle.</param>
    [UsedImplicitly]
    public static void DoIconButton(Rect rect, IconButton iconButton)
    {
        if (iconButton.IsEnabled)
        {
            if (Verse.Widgets.ButtonImageFitted(rect, iconButton.Icon)) iconButton.Action.Invoke();
        }
        else
        {
            Verse.Widgets.DrawTextureFitted(rect, iconButton.Icon, 1f, 0.25f);
        }
        if (!string.IsNullOrEmpty(iconButton.Tooltip)) TooltipHandler.TipRegion(rect, iconButton.Tooltip);
    }
}