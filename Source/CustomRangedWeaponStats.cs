using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using RimWorld;

namespace LordKuper.Common
{
    /// <summary>
    ///     Enumerates all custom ranged weapon statistics.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum CustomRangedWeaponStat
    {
        /// <summary>
        ///     Accuracy-accounted damage per second.
        /// </summary>
        Dpsa,

        /// <summary>
        ///     Accuracy-accounted damage per second at close range.
        /// </summary>
        DpsaClose,

        /// <summary>
        ///     Accuracy-accounted damage per second at short range.
        /// </summary>
        DpsaShort,

        /// <summary>
        ///     Accuracy-accounted damage per second at medium range.
        /// </summary>
        DpsaMedium,

        /// <summary>
        ///     Accuracy-accounted damage per second at long range.
        /// </summary>
        DpsaLong,

        /// <summary>
        ///     The maximum range of the weapon.
        /// </summary>
        Range,

        /// <summary>
        ///     The time required to warm up the weapon before firing.
        /// </summary>
        Warmup,

        /// <summary>
        ///     The number of shots fired per burst.
        /// </summary>
        BurstShotCount,

        /// <summary>
        ///     The number of ticks between shots in a burst.
        /// </summary>
        TicksBetweenBurstShots,

        /// <summary>
        ///     The armor penetration value of the weapon.
        /// </summary>
        ArmorPenetration,

        /// <summary>
        ///     The stopping power of the weapon.
        /// </summary>
        StoppingPower,

        /// <summary>
        ///     The damage dealt by the weapon.
        /// </summary>
        Damage,

        /// <summary>
        ///     The technological level of the weapon.
        /// </summary>
        TechLevel
    }

    /// <summary>
    ///     Provides utilities for working with custom ranged weapon statistics.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class CustomRangedWeaponStats
    {
        /// <summary>
        ///     The category name for custom ranged weapon stats.
        /// </summary>
        private const string Category = "RangedWeapons";

        /// <summary>
        ///     The <see cref="StatCategoryDef" /> for custom ranged weapon stats.
        /// </summary>
        private static StatCategoryDef CategoryDef { get; } = new StatCategoryDef
        {
            defName = $"{StatHelper.CustomStatPrefix}_{Category}",
            label = $"{StatHelper.CustomStatPrefix}_{Category}"
        };

        /// <summary>
        ///     Gets the collection of all custom stat definition names.
        /// </summary>
        private static IEnumerable<string> StatDefNames =>
            Enum.GetValues(typeof(CustomRangedWeaponStat)).OfType<CustomRangedWeaponStat>().Select(GetStatDefName);

        /// <summary>
        ///     Gets the collection of all <see cref="StatDef" />s for custom ranged weapon stats.
        /// </summary>
        public static IEnumerable<StatDef> StatDefs { get; } = StatDefNames.Select(defName => new StatDef
        {
            defName = defName,
            label = Resources.Strings.Stats.GetStatLabel(defName),
            description = Resources.Strings.Stats.GetStatDescription(defName),
            category = CategoryDef
        });

        /// <summary>
        ///     Gets the stat definition name for a given <see cref="CustomRangedWeaponStat" />.
        /// </summary>
        /// <param name="stat">The custom ranged weapon stat.</param>
        /// <returns>The stat definition name.</returns>
        public static string GetStatDefName(CustomRangedWeaponStat stat)
        {
            return $"{StatHelper.CustomStatPrefix}_{Category}_{stat}";
        }

        /// <summary>
        ///     Gets the stat name from a stat definition name.
        /// </summary>
        /// <param name="defName">The stat definition name.</param>
        /// <returns>The stat name, or <c>null</c> if not a custom stat.</returns>
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