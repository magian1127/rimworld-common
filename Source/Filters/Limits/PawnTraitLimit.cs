using System;
using JetBrains.Annotations;
using LordKuper.Common.Cache;
using RimWorld;
using Verse;

namespace LordKuper.Common.Filters.Limits;

/// <summary>
///     Represents a limit for a pawn capacity stat.
///     Inherits from <see cref="DefCache{T}" /> for caching the <see cref="TraitDef" />.
/// </summary>
[UsedImplicitly]
public class PawnTraitLimit : DefCache<TraitDef>, IExposable
{
    public bool Limit;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PawnTraitLimit" /> class.
    /// </summary>
    [UsedImplicitly]
    public PawnTraitLimit() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PawnTraitLimit" /> class with the specified stat definition.
    /// </summary>
    /// <param name="def">The stat definition to use for limits and formatting.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="def" /> is null.</exception>
    public PawnTraitLimit([NotNull] TraitDef def) : base(def.defName)
    {
        if (def == null) throw new ArgumentNullException(nameof(def));
        Limit = true;
    }

    /// <summary>
    ///     Exposes the data for saving and loading.
    /// </summary>
    public new void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref Limit, nameof(Limit));
    }
}