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
                    return $"{CommonMod.ModId}.Stats.{defName}.Description".Translate();
                }

                /// <summary>
                ///     Gets the localized label for a stat by its definition name.
                /// </summary>
                /// <param name="defName">The definition name of the stat.</param>
                /// <returns>The localized label string for the stat.</returns>
                public static string GetStatLabel(string defName)
                {
                    return $"{CommonMod.ModId}.Stats.{defName}.Label".Translate();
                }
            }
        }
    }
}