using System;
using JetBrains.Annotations;
using UnityEngine;

namespace LordKuper.Common.UI
{
    /// <summary>
    ///     Represents an action button to be displayed next to a field label.
    /// </summary>
    public struct ActionButton
    {
        /// <summary>
        ///     Gets the icon to display on the button.
        /// </summary>
        public Texture2D Icon { get; }

        /// <summary>
        ///     Gets the action to execute when the button is clicked.
        /// </summary>
        public Action Action { get; }

        /// <summary>
        ///     Gets a value indicating whether the button is enabled.
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        ///     Gets the tooltip to display for the button.
        /// </summary>
        public string Tooltip { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActionButton" /> struct.
        /// </summary>
        /// <param name="icon">The icon to display on the button.</param>
        /// <param name="action">The action to execute when the button is clicked.</param>
        /// <param name="tooltip">The tooltip to display for the button.</param>
        /// <param name="isEnabled">Whether the button is enabled.</param>
        [UsedImplicitly]
        public ActionButton(Texture2D icon, Action action, string tooltip = null, bool isEnabled = true)
        {
            Icon = icon;
            Action = action;
            Tooltip = tooltip;
            IsEnabled = isEnabled;
        }
    }
}