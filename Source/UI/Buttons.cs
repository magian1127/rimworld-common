using System;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace LordKuper.Common.UI
{
    /// <summary>
    ///     Provides utility methods for drawing icon buttons in the UI.
    /// </summary>
    [UsedImplicitly]
    public static class Buttons
    {
        /// <summary>
        ///     The default height of an action button in pixels.
        /// </summary>
        internal const float ActionButtonHeight = 32f;

        /// <summary>
        ///     The default size (in pixels) for field icon buttons.
        /// </summary>
        internal const float FieldIconButtonSize = 24f;

        public static void DoActionButton(Rect rect, string label, Action action, bool isEnabled = true)
        {
            var font = Text.Font;
            Text.Font = GameFont.Small;
            var buttonRect = Layout.GetCenterRowRect(rect, ActionButtonHeight, out _, out _);
            if (Verse.Widgets.ButtonText(buttonRect, label, active: isEnabled)) action.Invoke();
            Text.Font = font;
        }

        /// <summary>
        ///     Draws an icon button and handles its interaction.
        /// </summary>
        /// <param name="rect">The rectangle area where the button will be drawn.</param>
        /// <param name="icon">The icon texture to display on the button.</param>
        /// <param name="action">The action to invoke when the button is clicked.</param>
        /// <param name="tooltip">The tooltip text to display when hovering over the button.</param>
        /// <param name="isEnabled">Indicates whether the button is enabled and interactive.</param>
        public static void DoIconButton(Rect rect, Texture2D icon, Action action, string tooltip, bool isEnabled)
        {
            if (isEnabled)
            {
                if (Verse.Widgets.ButtonImageFitted(rect, icon)) action?.Invoke();
            }
            else
            {
                Verse.Widgets.DrawTextureFitted(rect, icon, 1f, 0.25f);
            }
            if (!string.IsNullOrEmpty(tooltip)) TooltipHandler.TipRegion(rect, tooltip);
        }
    }
}