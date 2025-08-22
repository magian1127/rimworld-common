﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using LordKuper.Common.Filters.Limits;
using LordKuper.Common.Helpers;
using RimWorld;
using Verse;

namespace LordKuper.Common.Filters;

/// <summary>
///     Provides filtering logic for pawns.
/// </summary>
public class PawnFilter : IExposable
{
    /// <summary>
    ///     The set of allowed pawn health states for filtering.
    /// </summary>
    public HashSet<PawnHealthState> AllowedPawnHealthStates = [];

    /// <summary>
    ///     Defines the set of allowed primary weapon types for pawns.
    /// </summary>
    public HashSet<PawnPrimaryWeaponType> AllowedPawnPrimaryWeaponTypes = [];

    /// <summary>
    ///     The set of allowed pawn types for filtering.
    /// </summary>
    public HashSet<PawnType> AllowedPawnTypes = [];

    /// <summary>
    ///     The set of allowed work passions for filtering.
    /// </summary>
    public HashSet<Passion> AllowedWorkPassions = [];

    /// <summary>
    ///     Whether to filter pawns by their capacities.
    /// </summary>
    public bool? FilterPawnCapacities;

    /// <summary>
    ///     Whether to filter pawns by their health states.
    /// </summary>
    public bool? FilterPawnHealthStates;

    /// <summary>
    ///     Gets or sets a value indicating whether to filter pawn primary weapon types.
    /// </summary>
    public bool? FilterPawnPrimaryWeaponTypes;

    /// <summary>
    ///     Indicates whether to filter pawns by their skills.
    /// </summary>
    public bool? FilterPawnSkills;

    /// <summary>
    ///     Indicates whether to filter pawns by their stats.
    /// </summary>
    public bool? FilterPawnStats;

    /// <summary>
    ///     Whether to filter pawns by their traits.
    /// </summary>
    public bool? FilterPawnTraits;

    /// <summary>
    ///     Whether to filter pawns by their types.
    /// </summary>
    public bool? FilterPawnTypes;

    /// <summary>
    ///     Whether to filter pawns by their work capacities.
    /// </summary>
    public bool? FilterWorkCapacities;

    /// <summary>
    ///     Whether to filter pawns by their work passions.
    /// </summary>
    public bool? FilterWorkPassions;

    /// <summary>
    ///     The set of forbidden pawn health states for filtering.
    /// </summary>
    public HashSet<PawnHealthState> ForbiddenPawnHealthStates = [];

    /// <summary>
    ///     The set of forbidden pawn types for filtering.
    /// </summary>
    public HashSet<PawnType> ForbiddenPawnTypes = [];

    /// <summary>
    ///     The dictionary of pawn capacity limits for filtering.
    ///     The key is the capacity name, and the value is the allowed range.
    /// </summary>
    public List<PawnCapacityLimit> PawnCapacityLimits = [];

    /// <summary>
    ///     The dictionary of pawn skill limits for filtering.
    ///     The key is the skill name, and the value is the allowed range.
    /// </summary>
    public List<PawnSkillLimit> PawnSkillLimits = [];

    /// <summary>
    ///     The list of stat limits for filtering pawns.
    /// </summary>
    public List<StatLimit> PawnStatLimits = [];

    /// <summary>
    ///     The dictionary of pawn trait limits for filtering.
    ///     The key is the trait name, and the value indicates if the trait is required (true) or forbidden (false).
    /// </summary>
    public List<PawnTraitLimit> PawnTraitLimits = [];

    /// <summary>
    ///     Indicates whether the control is in tri-state mode.
    /// </summary>
    public bool TriStateMode;

    /// <summary>
    ///     The dictionary of work capacity limits for filtering.
    ///     The key is the work capacity name, and the value indicates if the capacity is required (true) or forbidden (false).
    /// </summary>
    public Dictionary<WorkTags, bool> WorkCapacityLimits = [];

    /// <summary>
    ///     Exposes the filter data for saving and loading.
    /// </summary>
    public void ExposeData()
    {
        if (Scribe.mode == LoadSaveMode.Saving) Validate();
        Scribe_Values.Look(ref TriStateMode, nameof(TriStateMode));
        Scribe_Values.Look(ref FilterPawnTypes, nameof(FilterPawnTypes));
        Scribe_Collections.Look(ref AllowedPawnTypes, nameof(AllowedPawnTypes), LookMode.Value);
        Scribe_Collections.Look(ref ForbiddenPawnTypes, nameof(ForbiddenPawnTypes), LookMode.Value);
        Scribe_Values.Look(ref FilterPawnHealthStates, nameof(FilterPawnHealthStates));
        Scribe_Collections.Look(ref AllowedPawnHealthStates, nameof(AllowedPawnHealthStates), LookMode.Value);
        Scribe_Collections.Look(ref ForbiddenPawnHealthStates, nameof(ForbiddenPawnHealthStates), LookMode.Value);
        Scribe_Values.Look(ref FilterWorkPassions, nameof(FilterWorkPassions));
        Scribe_Collections.Look(ref AllowedWorkPassions, nameof(AllowedWorkPassions), LookMode.Value);
        Scribe_Values.Look(ref FilterPawnTraits, nameof(FilterPawnTraits));
        Scribe_Collections.Look(ref PawnTraitLimits, nameof(PawnTraitLimits), LookMode.Deep);
        Scribe_Values.Look(ref FilterPawnCapacities, nameof(FilterPawnCapacities));
        Scribe_Collections.Look(ref PawnCapacityLimits, nameof(PawnCapacityLimits), LookMode.Deep);
        Scribe_Values.Look(ref FilterWorkCapacities, nameof(FilterWorkCapacities));
        Scribe_Collections.Look(ref WorkCapacityLimits, nameof(WorkCapacityLimits), LookMode.Value, LookMode.Value);
        Scribe_Values.Look(ref FilterPawnSkills, nameof(FilterPawnSkills));
        Scribe_Collections.Look(ref PawnSkillLimits, nameof(PawnSkillLimits), LookMode.Deep);
        Scribe_Values.Look(ref FilterPawnStats, nameof(FilterPawnStats));
        Scribe_Collections.Look(ref PawnStatLimits, nameof(PawnStatLimits), LookMode.Deep);
    }

    /// <summary>
    ///     Combines two <see cref="PawnFilter" /> instances, preferring values from <paramref name="main" /> when available,
    ///     and falling back to <paramref name="fallback" /> otherwise.
    /// </summary>
    /// <param name="main">
    ///     The primary <see cref="PawnFilter" /> whose values take precedence if set.
    /// </param>
    /// <param name="fallback">
    ///     The fallback <see cref="PawnFilter" /> whose values are used if <paramref name="main" /> does not specify them.
    /// </param>
    /// <returns>
    ///     A new <see cref="PawnFilter" /> instance containing the combined filter settings.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if either <paramref name="main" /> or <paramref name="fallback" /> is <c>null</c>.
    /// </exception>
    [UsedImplicitly]
    public static PawnFilter Combine([NotNull] PawnFilter main, [NotNull] PawnFilter fallback)
    {
        if (main == null) throw new ArgumentNullException(nameof(main));
        if (fallback == null) throw new ArgumentNullException(nameof(fallback));
        var combined = new PawnFilter
        {
            TriStateMode = false
        };
        if (main.FilterPawnTypes.HasValue)
        {
            combined.FilterPawnTypes = main.FilterPawnTypes;
            combined.AllowedPawnTypes = new HashSet<PawnType>(main.AllowedPawnTypes);
            combined.ForbiddenPawnTypes = new HashSet<PawnType>(main.ForbiddenPawnTypes);
        }
        else
        {
            combined.FilterPawnTypes = fallback.FilterPawnTypes;
            combined.AllowedPawnTypes = new HashSet<PawnType>(fallback.AllowedPawnTypes);
            combined.ForbiddenPawnTypes = new HashSet<PawnType>(fallback.ForbiddenPawnTypes);
        }
        if (main.FilterPawnHealthStates.HasValue)
        {
            combined.FilterPawnHealthStates = main.FilterPawnHealthStates;
            combined.AllowedPawnHealthStates = new HashSet<PawnHealthState>(main.AllowedPawnHealthStates);
            combined.ForbiddenPawnHealthStates = new HashSet<PawnHealthState>(main.ForbiddenPawnHealthStates);
        }
        else
        {
            combined.FilterPawnHealthStates = fallback.FilterPawnHealthStates;
            combined.AllowedPawnHealthStates = new HashSet<PawnHealthState>(fallback.AllowedPawnHealthStates);
            combined.ForbiddenPawnHealthStates = new HashSet<PawnHealthState>(fallback.ForbiddenPawnHealthStates);
        }
        if (main.FilterWorkPassions.HasValue)
        {
            combined.FilterWorkPassions = main.FilterWorkPassions;
            combined.AllowedWorkPassions = new HashSet<Passion>(main.AllowedWorkPassions);
        }
        else
        {
            combined.FilterWorkPassions = fallback.FilterWorkPassions;
            combined.AllowedWorkPassions = new HashSet<Passion>(fallback.AllowedWorkPassions);
        }
        if (main.FilterPawnTraits.HasValue)
        {
            combined.FilterPawnTraits = main.FilterPawnTraits;
            combined.PawnTraitLimits = new List<PawnTraitLimit>(main.PawnTraitLimits);
        }
        else
        {
            combined.FilterPawnTraits = fallback.FilterPawnTraits;
            combined.PawnTraitLimits = new List<PawnTraitLimit>(fallback.PawnTraitLimits);
        }
        if (main.FilterPawnCapacities.HasValue)
        {
            combined.FilterPawnCapacities = main.FilterPawnCapacities;
            combined.PawnCapacityLimits = new List<PawnCapacityLimit>(main.PawnCapacityLimits);
        }
        else
        {
            combined.FilterPawnCapacities = fallback.FilterPawnCapacities;
            combined.PawnCapacityLimits = new List<PawnCapacityLimit>(fallback.PawnCapacityLimits);
        }
        if (main.FilterWorkCapacities.HasValue)
        {
            combined.FilterWorkCapacities = main.FilterWorkCapacities;
            combined.WorkCapacityLimits = new Dictionary<WorkTags, bool>(main.WorkCapacityLimits);
        }
        else
        {
            combined.FilterWorkCapacities = fallback.FilterWorkCapacities;
            combined.WorkCapacityLimits = new Dictionary<WorkTags, bool>(fallback.WorkCapacityLimits);
        }
        if (main.FilterPawnSkills.HasValue)
        {
            combined.FilterPawnSkills = main.FilterPawnSkills;
            combined.PawnSkillLimits = new List<PawnSkillLimit>(main.PawnSkillLimits);
        }
        else
        {
            combined.FilterPawnSkills = fallback.FilterPawnSkills;
            combined.PawnSkillLimits = new List<PawnSkillLimit>(fallback.PawnSkillLimits);
        }
        if (main.FilterPawnStats.HasValue)
        {
            combined.FilterPawnStats = main.FilterPawnStats;
            combined.PawnStatLimits = new List<StatLimit>(main.PawnStatLimits);
        }
        else
        {
            combined.FilterPawnStats = fallback.FilterPawnStats;
            combined.PawnStatLimits = new List<StatLimit>(fallback.PawnStatLimits);
        }
        if (main.FilterPawnPrimaryWeaponTypes.HasValue)
        {
            combined.FilterPawnPrimaryWeaponTypes = main.FilterPawnPrimaryWeaponTypes;
            combined.AllowedPawnPrimaryWeaponTypes =
                new HashSet<PawnPrimaryWeaponType>(main.AllowedPawnPrimaryWeaponTypes);
        }
        else
        {
            combined.FilterPawnPrimaryWeaponTypes = fallback.FilterPawnPrimaryWeaponTypes;
            combined.AllowedPawnPrimaryWeaponTypes =
                new HashSet<PawnPrimaryWeaponType>(fallback.AllowedPawnPrimaryWeaponTypes);
        }
        combined.Validate();
        return combined;
    }

    /// <summary>
    ///     Creates a deep copy of the current <see cref="PawnFilter" /> instance.
    /// </summary>
    /// <remarks>
    ///     The returned copy includes deep copies of collections and complex objects, ensuring that
    ///     changes to the copy do not affect the original instance, and vice versa.
    /// </remarks>
    /// <returns>A new <see cref="PawnFilter" /> instance with the same configuration and state as the current instance.</returns>
    [UsedImplicitly]
    [NotNull]
    public PawnFilter Copy()
    {
        return new PawnFilter
        {
            AllowedPawnHealthStates = [..AllowedPawnHealthStates],
            AllowedPawnTypes = [..AllowedPawnTypes],
            AllowedWorkPassions = [..AllowedWorkPassions],
            FilterPawnCapacities = FilterPawnCapacities,
            FilterPawnHealthStates = FilterPawnHealthStates,
            FilterPawnSkills = FilterPawnSkills,
            FilterPawnStats = FilterPawnStats,
            FilterPawnTraits = FilterPawnTraits,
            FilterPawnTypes = FilterPawnTypes,
            FilterWorkCapacities = FilterWorkCapacities,
            FilterWorkPassions = FilterWorkPassions,
            ForbiddenPawnHealthStates = [..ForbiddenPawnHealthStates],
            ForbiddenPawnTypes = [..ForbiddenPawnTypes],
            PawnCapacityLimits = PawnCapacityLimits.Select(l => new PawnCapacityLimit(l.Def) { Limit = l.Limit })
                .ToList(),
            PawnSkillLimits = PawnSkillLimits.Select(l => new PawnSkillLimit(l.Def) { Limit = l.Limit }).ToList(),
            PawnStatLimits = PawnStatLimits.Select(l => new StatLimit(l.Def)
            {
                Limit = l.Limit, LimitMaxCap = l.LimitMaxCap, LimitMinCap = l.LimitMinCap, ValueStyle = l.ValueStyle
            }).ToList(),
            PawnTraitLimits = PawnTraitLimits.Select(l => new PawnTraitLimit(l.Def) { Limit = l.Limit }).ToList(),
            TriStateMode = TriStateMode,
            WorkCapacityLimits = new Dictionary<WorkTags, bool>(WorkCapacityLimits)
        };
    }

    /// <summary>
    ///     Returns a set of pawns from the given maps that match the pawn filter settings.
    /// </summary>
    /// <param name="maps">The maps to search for pawns.</param>
    /// <param name="workType">The work type to filter by passion, or null to ignore passion filtering.</param>
    /// <returns>A set of filtered pawns.</returns>
    [UsedImplicitly]
    [NotNull]
    public HashSet<Pawn> GetFilteredPawns([NotNull] IEnumerable<Map> maps, [CanBeNull] WorkTypeDef workType)
    {
        if (maps == null) throw new ArgumentNullException(nameof(maps));
        var pawns = new HashSet<Pawn>();
        foreach (var map in maps)
        {
            var allPawns = map.mapPawns.AllPawnsSpawned;
            foreach (var pawn in allPawns)
            {
                if (!SatisfiesFilter(pawn, workType)) continue;
                pawns.Add(pawn);
            }
        }
        return pawns;
    }

    /// <summary>
    ///     Generates a detailed summary of the current pawn filter configuration, formatted with the specified indentation
    ///     level.
    /// </summary>
    /// <remarks>
    ///     The summary includes only the filter criteria that are currently active. Each section is indented
    ///     according to the specified <paramref name="indentationLevel" /> and formatted for readability. This method is
    ///     useful
    ///     for displaying the filter configuration in a user interface.
    /// </remarks>
    /// <param name="indentationLevel">
    ///     The number of indentation levels to apply to the formatted summary. Must be a
    ///     non-negative integer.
    /// </param>
    /// <returns>A string containing the formatted summary of the pawn filter configuration.</returns>
    [NotNull]
    [UsedImplicitly]
    public string GetSummary(int indentationLevel)
    {
        var stringBuilder = new StringBuilder();
        var anyValue = false;
        if (FilterPawnTypes.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.AllowedPawnTypesLabel}: ".Colorize(ColoredText.ExpectationsColor),
                indentationLevel);
            stringBuilder.AppendLine(FilterPawnTypes.Value
                ? string.Join(", ", AllowedPawnTypes.Select(Resources.Strings.PawnType.GetLabel))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (FilterPawnHealthStates.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.AllowedPawnHealthStatesLabel}: "
                    .Colorize(ColoredText.ExpectationsColor), indentationLevel);
            stringBuilder.AppendLine(FilterPawnHealthStates.Value
                ? string.Join(", ", AllowedPawnHealthStates.Select(Resources.Strings.PawnHealthState.GetLabel))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (FilterPawnPrimaryWeaponTypes.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.AllowedPawnPrimaryWeaponTypesLabel}: ".Colorize(ColoredText
                    .ExpectationsColor), indentationLevel);
            stringBuilder.AppendLine(FilterPawnPrimaryWeaponTypes.Value
                ? string.Join(", ",
                    AllowedPawnPrimaryWeaponTypes.Select(Resources.Strings.PawnPrimaryWeaponType.GetLabel))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (FilterPawnSkills.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.PawnSkillLimitsLabel}: ".Colorize(ColoredText.ExpectationsColor),
                indentationLevel);
            stringBuilder.AppendLine(FilterPawnSkills.Value
                ? string.Join(", ",
                    PawnSkillLimits.Select(l => $"{l.Label} [{l.Limit.TrueMin:N0}..{l.Limit.TrueMax:N0}]"))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (FilterWorkPassions.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.AllowedWorkPassionsLabel}: ".Colorize(ColoredText.ExpectationsColor),
                indentationLevel);
            stringBuilder.AppendLine(FilterWorkPassions.Value
                ? string.Join(", ",
                    AllowedWorkPassions.Select(PassionHelper.GetPassionCache).Where(p => p != null)
                        .Select(p => p.Label))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (FilterWorkCapacities.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.WorkCapacityLimitsLabel}: ".Colorize(ColoredText.ExpectationsColor),
                indentationLevel);
            stringBuilder.AppendLine(FilterWorkCapacities.Value
                ? string.Join(", ", WorkCapacityLimits.Select(l => $"{(l.Value ? "+" : "-")}{l.Key.LabelTranslated()}"))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (FilterPawnTraits.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.PawnTraitLimitsLabel}: ".Colorize(ColoredText.ExpectationsColor),
                indentationLevel);
            stringBuilder.AppendLine(FilterPawnTraits.Value
                ? string.Join(", ", PawnTraitLimits.Select(l => $"{(l.Limit ? "+" : "-")}{l.Label}"))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (FilterPawnStats.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.PawnStatLimitsLabel}: ".Colorize(ColoredText.ExpectationsColor),
                indentationLevel);
            stringBuilder.AppendLine(FilterPawnStats.Value
                ? string.Join(", ",
                    PawnStatLimits.Select(l =>
                        $"{l.Label} [{l.Limit.TrueMin.ToStringByStyle(l.ValueStyle)}..{l.Limit.TrueMax.ToStringByStyle(l.ValueStyle)}]"))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (FilterPawnCapacities.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.PawnCapacityLimitsLabel}: ".Colorize(ColoredText.ExpectationsColor),
                indentationLevel);
            stringBuilder.AppendLine(FilterPawnCapacities.Value
                ? string.Join(", ",
                    PawnCapacityLimits.Select(l =>
                        $"{l.Label} [{l.Limit.TrueMin.ToStringByStyle(PawnCapacityLimit.ValueStyle)}..{l.Limit.TrueMax.ToStringByStyle(PawnCapacityLimit.ValueStyle)}]"))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (!anyValue)
            stringBuilder.AppendIndented(Resources.Strings.PawnFilter.UndefinedFilterTooltip, indentationLevel);
        return stringBuilder.ToString();
    }

    /// <summary>
    ///     Determines whether the specified <paramref name="pawn" /> satisfies the defined filters and constraints.
    /// </summary>
    /// <remarks>
    ///     This method evaluates the <paramref name="pawn" /> against multiple filters, including pawn
    ///     type, health state, work passions, traits, capacities, skills, and stats. Each filter is applied only if it is
    ///     enabled and configured. If any filter is not satisfied, the method returns <see langword="false" />. If all
    ///     applicable filters are satisfied, the method returns <see langword="true" />.
    /// </remarks>
    /// <param name="pawn">The pawn to evaluate. Cannot be <see langword="null" />.</param>
    /// <param name="workType">
    ///     The work type to consider when filtering by work passions. Can be <see langword="null" /> if work passion
    ///     filtering is not required.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the <paramref name="pawn" /> meets all the specified filters and constraints;
    ///     otherwise, <see langword="false" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="pawn" /> is <see langword="null" />.</exception>
    [UsedImplicitly]
    public bool SatisfiesFilter([NotNull] Pawn pawn, [CanBeNull] WorkTypeDef workType)
    {
        if (pawn == null) throw new ArgumentNullException(nameof(pawn));
        if (FilterPawnTypes == true)
        {
            var pawnType = PawnHelper.GetPawnType(pawn);
            if (!AllowedPawnTypes.Contains(pawnType)) return false;
        }
        if (FilterPawnHealthStates == true)
        {
            var healthState = PawnHelper.GetPawnHealthState(pawn);
            if (!AllowedPawnHealthStates.Any(s => healthState.HasFlag(s))) return false;
        }
        if (FilterPawnPrimaryWeaponTypes == true)
        {
            var weaponType = PawnHelper.GetPrimaryWeaponType(pawn);
            if (!AllowedPawnPrimaryWeaponTypes.Contains(weaponType)) return false;
        }
        if (FilterWorkPassions == true && workType != null)
        {
            var passion = PawnHelper.GetWorkPassion(pawn, workType);
            if (!AllowedWorkPassions.Contains(passion)) return false;
        }
        if (FilterPawnTraits == true && PawnTraitLimits.Count > 0)
        {
            var traits = pawn.story?.traits;
            if (traits == null) return false;
            bool enabledSatisfied = true, disabledSatisfied = true, hasEnabled = false;
            foreach (var limit in PawnTraitLimits)
            {
                if (limit.Def == null) return false;
                if (limit.Limit)
                {
                    if (!hasEnabled)
                    {
                        enabledSatisfied = false;
                        hasEnabled = true;
                    }
                    if (!enabledSatisfied && traits.HasTrait(limit.Def))
                        enabledSatisfied = true;
                }
                else
                {
                    if (traits.HasTrait(limit.Def))
                    {
                        disabledSatisfied = false;
                        break;
                    }
                }
            }
            if (!enabledSatisfied || !disabledSatisfied) return false;
        }
        if (FilterPawnCapacities == true && PawnCapacityLimits.Count > 0)
        {
            var satisfied = true;
            foreach (var limit in PawnCapacityLimits)
            {
                if (limit.Def == null) return false;
                if (!limit.Limit.Includes(pawn.health.capacities.GetLevel(limit.Def)))
                {
                    satisfied = false;
                    break;
                }
            }
            if (!satisfied) return false;
        }
        if (FilterWorkCapacities == true && WorkCapacityLimits.Count > 0)
        {
            var satisfied = true;
            foreach (var limit in WorkCapacityLimits)
            {
                if (pawn.WorkTagIsDisabled(limit.Key) != limit.Value)
                {
                    satisfied = false;
                    break;
                }
            }
            if (!satisfied) return false;
        }
        if (FilterPawnSkills == true && PawnSkillLimits.Count > 0)
        {
            var satisfied = true;
            foreach (var limit in PawnSkillLimits)
            {
                if (limit.Def == null) return false;
                var value = pawn.skills.GetSkill(limit.Def).Level;
                if (value < limit.Limit.TrueMin || value > limit.Limit.TrueMax)
                {
                    satisfied = false;
                    break;
                }
            }
            if (!satisfied) return false;
        }
        if (FilterPawnStats == true && PawnStatLimits.Count > 0)
        {
            var satisfied = true;
            foreach (var limit in PawnStatLimits)
            {
                if (limit.Def == null) return false;
                var statValue = pawn.GetStatValue(limit.Def);
                if (statValue < limit.Limit.TrueMin || statValue > limit.Limit.TrueMax)
                {
                    satisfied = false;
                    break;
                }
            }
            if (!satisfied) return false;
        }
        return true;
    }

    /// <summary>
    ///     Ensures that all filter-related properties are set to valid default values  when <see cref="TriStateMode" /> is
    ///     disabled.
    /// </summary>
    /// <remarks>
    ///     This method initializes filter properties to <see langword="false" /> if they are null  and
    ///     <see cref="TriStateMode" /> is not enabled. It is intended to maintain consistent  state for filter-related
    ///     settings.
    /// </remarks>
    private void Validate()
    {
        if (!TriStateMode)
        {
            FilterPawnTypes ??= false;
            FilterWorkPassions ??= false;
            FilterPawnCapacities ??= false;
            FilterPawnHealthStates ??= false;
            FilterPawnSkills ??= false;
            FilterPawnStats ??= false;
            FilterPawnTraits ??= false;
            FilterWorkCapacities ??= false;
            FilterPawnPrimaryWeaponTypes ??= false;
        }
    }
}