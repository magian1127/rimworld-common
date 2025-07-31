using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace LordKuper.Common
{
    /// <summary>
    ///     Enumerates the custom melee weapon stats available for use.
    /// </summary>
    public enum CustomMeleeWeaponStat
    {
        /// <summary>
        ///     The armor penetration stat.
        /// </summary>
        ArmorPenetration,

        /// <summary>
        ///     The sharp damage per second stat.
        /// </summary>
        DpsSharp,

        /// <summary>
        ///     The blunt damage per second stat.
        /// </summary>
        DpsBlunt,

        /// <summary>
        ///     The tech level stat.
        /// </summary>
        TechLevel
    }

    /// <summary>
    ///     Provides helper methods and properties for custom melee weapon stats.
    /// </summary>
    public static class CustomMeleeWeaponStats
    {
        /// <summary>
        ///     The category name for custom melee weapon stats.
        /// </summary>
        private const string Category = "MeleeWeapons";

        /// <summary>
        ///     The stat category definition for custom melee weapon stats.
        /// </summary>
        private static StatCategoryDef CategoryDef { get; } = new StatCategoryDef
        {
            defName = $"{StatHelper.CustomStatPrefix}_{Category}",
            label = $"{StatHelper.CustomStatPrefix}_{Category}"
        };

        /// <summary>
        ///     Gets the collection of custom stat definition names.
        /// </summary>
        private static IEnumerable<string> StatDefNames =>
            Enum.GetValues(typeof(CustomMeleeWeaponStat)).OfType<CustomMeleeWeaponStat>().Select(GetStatDefName);

        /// <summary>
        ///     Gets the collection of custom stat definitions.
        /// </summary>
        public static IEnumerable<StatDef> StatDefs { get; } = StatDefNames.Select(defName => new StatDef
        {
            defName = defName,
            label = Resources.Strings.Stats.GetStatLabel(defName),
            description = Resources.Strings.Stats.GetStatDescription(defName),
            category = CategoryDef
        });

        /// <summary>
        ///     Gets the stat definition name for a given <see cref="CustomMeleeWeaponStat" />.
        /// </summary>
        /// <param name="stat">The custom melee weapon stat.</param>
        /// <returns>The stat definition name.</returns>
        public static string GetStatDefName(CustomMeleeWeaponStat stat)
        {
            return $"{StatHelper.CustomStatPrefix}_{Category}_{stat}";
        }

        /// <summary>
        ///     Gets the stat name from a stat definition name.
        /// </summary>
        /// <param name="defName">The stat definition name.</param>
        /// <returns>The stat name if the definition name matches the custom stat pattern; otherwise, <c>null</c>.</returns>
        public static string GetStatName(string defName)
        {
            var categoryPrefix = $"{StatHelper.CustomStatPrefix}_{Category}_";
            return defName.StartsWith(categoryPrefix, StringComparison.OrdinalIgnoreCase)
                ? defName.Substring(categoryPrefix.Length)
                : null;
        }

        /// <summary>
        ///     Determines whether the specified definition name is a custom stat.
        /// </summary>
        /// <param name="defName">The stat definition name.</param>
        /// <returns><c>true</c> if the definition name is a custom stat; otherwise, <c>false</c>.</returns>
        public static bool IsCustomStat(string defName)
        {
            return StatDefNames.Contains(defName);
        }
    }
}