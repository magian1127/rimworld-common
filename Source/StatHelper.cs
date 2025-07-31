using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace LordKuper.Common
{
    /// <summary>
    ///     Provides helper methods and properties for working with StatDefs and stat values.
    /// </summary>
    [UsedImplicitly]
    public static class StatHelper
    {
        /// <summary>
        ///     Prefix used for custom stat definitions.
        /// </summary>
        internal const string CustomStatPrefix = "LK";

        /// <summary>
        ///     Gets all stat definitions, including both default and custom stats.
        /// </summary>
        public static IEnumerable<StatDef> AllStatsDefs => DefaultStatDefs.Union(CustomStatsDefs);

        /// <summary>
        ///     Gets all custom stat definitions from melee, ranged, and tool stats.
        /// </summary>
        public static IEnumerable<StatDef> CustomStatsDefs =>
            CustomMeleeWeaponStats.StatDefs.Union(CustomRangedWeaponStats.StatDefs).Union(CustomToolStats.StatDefs);

        /// <summary>
        ///     Gets all default pawn stat definitions, ordered by category and label.
        /// </summary>
        public static IEnumerable<StatDef> DefaultPawnStatDefs =>
            DefaultStatDefs.Where(def => PawnCategories.Contains(def.category?.defName ?? string.Empty))
                .OrderBy(def => def.category?.defName ?? string.Empty).ThenBy(def => def.label);

        /// <summary>
        ///     Gets all default stat definitions from the database.
        /// </summary>
        public static IEnumerable<StatDef> DefaultStatDefs => DefDatabase<StatDef>.AllDefs;

        /// <summary>
        ///     Gets all default weapon stat definitions.
        /// </summary>
        public static IEnumerable<StatDef> DefaultWeaponStatDefs =>
            DefaultStatDefs.Where(def => WeaponCategories.Contains(def.category?.defName ?? string.Empty));

        /// <summary>
        ///     Gets all melee weapon stat definitions, including custom and default, ordered by category and label.
        /// </summary>
        public static IReadOnlyList<StatDef> MeleeWeaponStatDefs { get; } = new List<StatDef>(CustomMeleeWeaponStats
            .StatDefs.Union(DefaultWeaponStatDefs).OrderBy(def => def.category?.defName ?? string.Empty)
            .ThenBy(def => def.label));

        /// <summary>
        ///     Gets the list of pawn stat categories.
        /// </summary>
        private static IEnumerable<string> PawnCategories =>
            new[]
            {
                "Basics", "BasicsImportant", "BasicsPawnImportant", "BasicsPawn", "PawnCombat", "PawnSocial",
                "PawnMisc", "PawnWork"
            };

        /// <summary>
        ///     Gets all ranged weapon stat definitions, including custom and default, ordered by category and label.
        /// </summary>
        public static IReadOnlyList<StatDef> RangedWeaponStatDefs { get; } = new List<StatDef>(CustomRangedWeaponStats
            .StatDefs.Union(DefaultWeaponStatDefs).OrderBy(def => def.category?.defName ?? string.Empty)
            .ThenBy(def => def.label));

        /// <summary>
        ///     Gets all tool stat definitions, including custom and default, ordered by category and label.
        /// </summary>
        public static IReadOnlyList<StatDef> ToolStatDefs { get; } = new List<StatDef>(CustomToolStats.StatDefs
            .Union(DefaultWeaponStatDefs).OrderBy(def => def.category?.defName ?? string.Empty)
            .ThenBy(def => def.label));

        /// <summary>
        ///     Gets the list of weapon stat categories.
        /// </summary>
        private static IEnumerable<string> WeaponCategories =>
            new[]
            {
                "Basics", "BasicsImportant", "BasicsNonPawnImportant", "BasicsNonPawn", "Weapon", "Weapon_Ranged",
                "Weapon_Melee", "PawnWork"
            };

        /// <summary>
        ///     Gets the list of work stat categories.
        /// </summary>
        private static IEnumerable<string> WorkCategories => new[] { "PawnWork", "PawnSocial" };

        /// <summary>
        ///     Gets all stat definitions related to work types, ordered by category and label.
        /// </summary>
        public static IEnumerable<StatDef> WorkTypeStatDefs { get; } = DefaultStatDefs
            .Where(def => WorkCategories.Contains(def.category?.defName ?? string.Empty))
            .OrderBy(def => def.category?.defName ?? string.Empty).ThenBy(def => def.label);

        /// <summary>
        ///     Gets a <see cref="StatDef" /> by its defName, case-insensitive.
        /// </summary>
        /// <param name="defName">The defName of the stat.</param>
        /// <returns>The matching <see cref="StatDef" />, or null if not found.</returns>
        public static StatDef GetStatDef(string defName)
        {
            return AllStatsDefs.FirstOrDefault(def => def.defName.Equals(defName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Gets the value of a stat for a specific <see cref="Thing" />.
        /// </summary>
        /// <param name="thing">The thing to evaluate.</param>
        /// <param name="statDef">The stat definition.</param>
        /// <returns>The stat value, or 0 if evaluation fails.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thing" /> or <paramref name="statDef" /> is null.</exception>
        public static float GetStatValue([NotNull] Thing thing, [NotNull] StatDef statDef)
        {
            if (thing == null) throw new ArgumentNullException(nameof(thing));
            if (statDef == null) throw new ArgumentNullException(nameof(statDef));
            try
            {
                return thing.GetStatValue(statDef) +
                       (thing.def.equippedStatOffsets?.Find(modifier => modifier.stat == statDef)?.value ?? 0f);
            }
            catch (Exception exception)
            {
                Logger.LogWarning($"Could not evaluate stat '{statDef.LabelCap}' of {thing.LabelCapNoCount}.",
                    exception);
                return 0f;
            }
        }

        /// <summary>
        ///     Gets the value of a stat for a specific <see cref="ThingDef" />.
        /// </summary>
        /// <param name="def">The thing definition.</param>
        /// <param name="statDef">The stat definition.</param>
        /// <returns>The stat value, or 0 if evaluation fails.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="def" /> or <paramref name="statDef" /> is null.</exception>
        public static float GetStatValue([NotNull] ThingDef def, [NotNull] StatDef statDef)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            if (statDef == null) throw new ArgumentNullException(nameof(statDef));
            try
            {
                return (def.statBases?.Find(modifier => modifier.stat == statDef)?.value ?? 0f) +
                       (def.equippedStatOffsets?.Find(modifier => modifier.stat == statDef)?.value ?? 0f);
            }
            catch (Exception exception)
            {
                Logger.LogWarning($"Could not evaluate stat '{statDef.LabelCap}' of {def.LabelCap}", exception);
                return 0f;
            }
        }

        /// <summary>
        ///     Gets the deviation of a stat value from its default base value for a <see cref="ThingDef" />.
        /// </summary>
        /// <param name="def">The thing definition.</param>
        /// <param name="statDef">The stat definition.</param>
        /// <returns>The deviation from the default base value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="def" /> or <paramref name="statDef" /> is null.</exception>
        public static float GetStatValueDeviation([NotNull] ThingDef def, [NotNull] StatDef statDef)
        {
            return def == null ? throw new ArgumentNullException(nameof(def)) :
                statDef == null ? throw new ArgumentNullException(nameof(statDef)) :
                GetStatValue(def, statDef) - statDef.defaultBaseValue;
        }

        /// <summary>
        ///     Gets the deviation of a stat value from its default base value for a <see cref="Thing" />.
        /// </summary>
        /// <param name="thing">The thing to evaluate.</param>
        /// <param name="statDef">The stat definition.</param>
        /// <returns>The deviation from the default base value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thing" /> or <paramref name="statDef" /> is null.</exception>
        public static float GetStatValueDeviation([NotNull] Thing thing, [NotNull] StatDef statDef)
        {
            return thing == null ? throw new ArgumentNullException(nameof(thing)) :
                statDef == null ? throw new ArgumentNullException(nameof(statDef)) :
                GetStatValue(thing, statDef) - statDef.defaultBaseValue;
        }

        /// <summary>
        ///     Normalizes a value within a specified range to a value between -1 and 1 or 0 and 1, depending on the range.
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <param name="range">The range to normalize within.</param>
        /// <returns>The normalized value.</returns>
        public static float NormalizeValue(float value, FloatRange range)
        {
            value = Mathf.Clamp(value, range.min, range.max);
            var valueRange = range.max - range.min;
            if (Math.Abs(valueRange) < 0.001f) return 0f;
            var normalizedValue = (value - range.min) / valueRange;
            return range.min < 0 && range.max < 0 ? -1 + normalizedValue :
                range.min < 0 && range.max > 0 ? -1 + 2 * normalizedValue : normalizedValue;
        }
    }
}