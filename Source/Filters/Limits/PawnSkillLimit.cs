using System;
using JetBrains.Annotations;
using LordKuper.Common.Cache;
using RimWorld;
using Verse;

namespace LordKuper.Common.Filters.Limits;

/// <summary>
///     Represents a skill limit filter for a pawn, based on a specific <see cref="SkillDef" />.
///     Stores a range for the allowed skill level and supports serialization.
/// </summary>
[UsedImplicitly]
public class PawnSkillLimit : DefCache<SkillDef>, IExposable
{
    /// <summary>
    ///     The maximum allowed skill level cap.
    /// </summary>
    internal const int LimitMaxCap = 20;

    /// <summary>
    ///     The minimum allowed skill level cap.
    /// </summary>
    internal const int LimitMinCap = 0;

    /// <summary>
    ///     The step value for skill increments.
    /// </summary>
    internal const int ValueStep = 1;

    /// <summary>
    ///     The range of allowed skill levels for the pawn.
    /// </summary>
    public IntRange Limit;

    /// <summary>
    ///     Default constructor for serialization.
    /// </summary>
    [UsedImplicitly]
    public PawnSkillLimit() { }

    /// <summary>
    ///     Initializes a new instance of <see cref="PawnSkillLimit" /> for the specified skill definition.
    /// </summary>
    /// <param name="def">The skill definition to apply the limit to.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="def" /> is null.</exception>
    public PawnSkillLimit([NotNull] SkillDef def) : base(def.defName)
    {
        if (def == null) throw new ArgumentNullException(nameof(def));
        Limit = new IntRange(LimitMinCap, LimitMaxCap);
    }

    /// <summary>
    ///     Serializes the skill limit data for saving/loading.
    /// </summary>
    public new void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref Limit, nameof(Limit));
    }
}