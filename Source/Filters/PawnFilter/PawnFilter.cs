using System;
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
    public static PawnFilter CombineFilters([NotNull] PawnFilter main, [NotNull] PawnFilter fallback)
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
        combined.Validate();
        return combined;
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
                if (pawn == null) continue;
                var pawnType = GetPawnType(pawn);
                if (!AllowedPawnTypes.Contains(pawnType)) continue;
                if (FilterPawnHealthStates == true)
                {
                    var healthState = GetPawnHealthState(pawn);
                    if (!AllowedPawnHealthStates.Contains(healthState)) continue;
                }
                if (FilterWorkPassions == true && workType != null)
                {
                    var passion = GetWorkPassion(pawn, workType);
                    if (!AllowedWorkPassions.Contains(passion)) continue;
                }
                if (FilterPawnTraits == true && PawnTraitLimits.Count > 0)
                {
                    var traits = pawn.story?.traits;
                    if (traits == null) continue;
                    bool enabledSatisfied = true, disabledSatisfied = true, hasEnabled = false;
                    foreach (var limit in PawnTraitLimits)
                    {
                        if (limit.Def == null) continue;
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
                    if (!enabledSatisfied || !disabledSatisfied) continue;
                }
                if (FilterPawnCapacities == true && PawnCapacityLimits.Count > 0)
                {
                    var satisfied = true;
                    foreach (var limit in PawnCapacityLimits)
                    {
                        if (limit.Def == null) continue;
                        if (!limit.Limit.Includes(pawn.health.capacities.GetLevel(limit.Def)))
                        {
                            satisfied = false;
                            break;
                        }
                    }
                    if (!satisfied) continue;
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
                    if (!satisfied) continue;
                }
                if (FilterPawnSkills == true && PawnSkillLimits.Count > 0)
                {
                    var satisfied = true;
                    foreach (var limit in PawnSkillLimits)
                    {
                        if (limit.Def == null) continue;
                        var value = pawn.skills.GetSkill(limit.Def).Level;
                        if (value < limit.Limit.TrueMin || value > limit.Limit.TrueMax)
                        {
                            satisfied = false;
                            break;
                        }
                    }
                    if (!satisfied) continue;
                }
                if (FilterPawnStats == true && PawnStatLimits.Count > 0)
                {
                    var satisfied = true;
                    foreach (var limit in PawnStatLimits)
                    {
                        if (limit.Def == null) continue;
                        var statValue = pawn.GetStatValue(limit.Def);
                        if (statValue < limit.Limit.TrueMin || statValue > limit.Limit.TrueMax)
                        {
                            satisfied = false;
                            break;
                        }
                    }
                    if (!satisfied) continue;
                }
                pawns.Add(pawn);
            }
        }
        return pawns;
    }

    /// <summary>
    ///     Determines the health state of the specified pawn.
    /// </summary>
    /// <param name="pawn">The pawn whose health state is to be determined.</param>
    /// <returns>The <see cref="PawnHealthState" /> of the pawn.</returns>
    private static PawnHealthState GetPawnHealthState([NotNull] Pawn pawn)
    {
        if (pawn.Dead) return PawnHealthState.Dead;
        if (pawn.Downed) return PawnHealthState.Downed;
        if (pawn.InMentalState) return PawnHealthState.Mental;
        var health = pawn.health;
        if (health.HasHediffsNeedingTend() || health.hediffSet.HasTendableHediff())
            return PawnHealthState.NeedsTending;
        if (HealthAIUtility.ShouldSeekMedicalRest(pawn)) return PawnHealthState.Resting;
        return PawnHealthState.Healthy;
    }

    /// <summary>
    ///     Determines the type of the specified pawn.
    /// </summary>
    /// <param name="pawn">The pawn whose type is to be determined.</param>
    /// <returns>The <see cref="PawnType" /> of the pawn.</returns>
    private static PawnType GetPawnType([NotNull] Pawn pawn)
    {
        if (pawn.IsFreeNonSlaveColonist) return PawnType.Colonist;
        if (pawn.IsSlaveOfColony) return PawnType.Slave;
        if (pawn.IsPrisonerOfColony) return PawnType.Prisoner;
        if (pawn is { IsColonist: true, GuestStatus: GuestStatus.Guest } || pawn.HasExtraHomeFaction() ||
            pawn.HasExtraMiniFaction())
            return PawnType.Guest;
        if (pawn.IsAnimal && pawn.Faction == Faction.OfPlayer) return PawnType.Animal;
        return PawnType.Undefined;
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
                ? string.Join(", ", AllowedPawnTypes.Select(Resources.Strings.PawnType.GetPawnTypeLabel))
                : Resources.Strings.PawnFilter.IgnoreFilter);
        }
        if (FilterPawnHealthStates.HasValue)
        {
            anyValue = true;
            stringBuilder.AppendIndented(
                $"{Resources.Strings.PawnFilter.AllowedPawnHealthStatesLabel}: "
                    .Colorize(ColoredText.ExpectationsColor), indentationLevel);
            stringBuilder.AppendLine(FilterPawnHealthStates.Value
                ? string.Join(", ",
                    AllowedPawnHealthStates.Select(Resources.Strings.PawnHealthState.GetPawnHealthStateLabel))
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
    ///     Gets the highest work passion for the specified pawn and work type.
    /// </summary>
    /// <param name="pawn">The pawn whose work passion is to be determined.</param>
    /// <param name="workType">The work type to check for passion.</param>
    /// <returns>The highest <see cref="Passion" /> for the given work type, or <see cref="Passion.None" /> if unavailable.</returns>
    private static Passion GetWorkPassion([NotNull] Pawn pawn, [NotNull] WorkTypeDef workType)
    {
        if (pawn == null) throw new ArgumentNullException(nameof(pawn));
        if (workType == null) throw new ArgumentNullException(nameof(workType));
        return pawn.skills?.MaxPassionOfRelevantSkillsFor(workType) ?? Passion.None;
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
        }
    }
}