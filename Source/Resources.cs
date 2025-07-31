using UnityEngine;
using Verse;

namespace LordKuper.Common
{
    /// <summary>
    ///     Provides access to common resources used throughout the mod.
    /// </summary>
    public static class Resources
    {
        /// <summary>
        ///     Contains string resources for the mod.
        /// </summary>
        public static class Strings
        {
            /// <summary>
            ///     Provides localized strings for common actions.
            /// </summary>
            public static class Actions
            {
                /// <summary>
                ///     Gets the localized string for the "Add" action.
                /// </summary>
                public static string Add => $"{CommonMod.ModId}.{nameof(Actions)}.{nameof(Add)}".Translate();

                /// <summary>
                ///     Gets the localized string for the "Refresh" action.
                /// </summary>
                public static string Refresh => $"{CommonMod.ModId}.{nameof(Actions)}.{nameof(Refresh)}".Translate();
            }

            /// <summary>
            ///     Provides methods for retrieving stat-related string resources.
            /// </summary>
            public static class Stats
            {
                /// <summary>
                ///     Gets the localized description for a stat by its definition name.
                /// </summary>
                /// <param name="defName">The definition name of the stat.</param>
                /// <returns>The localized description string for the stat.</returns>
                public static string GetStatDescription(string defName)
                {
                    return $"{CommonMod.ModId}.{nameof(Stats)}.{defName}.Description".Translate();
                }

                /// <summary>
                ///     Gets the localized label for a stat by its definition name.
                /// </summary>
                /// <param name="defName">The definition name of the stat.</param>
                /// <returns>The localized label string for the stat.</returns>
                public static string GetStatLabel(string defName)
                {
                    return $"{CommonMod.ModId}.{nameof(Stats)}.{defName}.Label".Translate();
                }
            }

            /// <summary>
            ///     Provides string resources for the WorkTypeThingRuleWidget UI component.
            /// </summary>
            public static class WorkTypeThingRuleWidget
            {
                public static string AvailableItemsLabel =>
                    $"{CommonMod.ModId}.{nameof(WorkTypeThingRuleWidget)}.{nameof(AvailableItemsLabel)}".Translate();

                public static string AvailableItemsTooltip =>
                    $"{CommonMod.ModId}.{nameof(WorkTypeThingRuleWidget)}.{nameof(AvailableItemsTooltip)}".Translate();

                /// <summary>
                ///     Gets the localized string indicating that no rule is selected.
                /// </summary>
                public static string NoRuleSelected =>
                    $"{CommonMod.ModId}.{nameof(WorkTypeThingRuleWidget)}.{nameof(NoRuleSelected)}".Translate();

                /// <summary>
                ///     Gets the localized string for selecting a work type.
                /// </summary>
                public static string SelectWorkType =>
                    $"{CommonMod.ModId}.{nameof(WorkTypeThingRuleWidget)}.{nameof(SelectWorkType)}".Translate();

                /// <summary>
                ///     Gets the localized label for stat weights.
                /// </summary>
                public static string StatWeightsLabel =>
                    $"{CommonMod.ModId}.{nameof(WorkTypeThingRuleWidget)}.{nameof(StatWeightsLabel)}".Translate();

                /// <summary>
                ///     Gets the localized tooltip for stat weights.
                /// </summary>
                public static string StatWeightsTooltip =>
                    $"{CommonMod.ModId}.{nameof(WorkTypeThingRuleWidget)}.{nameof(StatWeightsTooltip)}".Translate();
            }
        }

        [StaticConstructorOnStartup]
        public static class Textures
        {
            public static readonly Texture2D BadTexture = ContentFinder<Texture2D>.Get("UI/Misc/BadTexture");
        }
    }
}