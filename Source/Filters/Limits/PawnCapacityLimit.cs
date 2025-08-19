using System;
using JetBrains.Annotations;
using LordKuper.Common.Cache;
using LordKuper.Common.UI;
using Verse;

namespace LordKuper.Common.Filters.Limits;

/// <summary>
///     Represents a limit for a specific <see cref="PawnCapacityDef" />.
///     Provides range constraints and value step for UI sliders.
/// </summary>
[UsedImplicitly]
public class PawnCapacityLimit : DefCache<PawnCapacityDef>, IExposable
{
    /// <summary>
    ///     The maximum allowed value for the limit.
    /// </summary>
    internal const float LimitMaxCap = 5f;

    /// <summary>
    ///     The minimum allowed value for the limit.
    /// </summary>
    internal const float LimitMinCap = 0f;

    /// <summary>
    ///     The style used for displaying the value (percent, zero decimals).
    /// </summary>
    internal const ToStringStyle ValueStyle = ToStringStyle.PercentZero;

    private float _valueStep;

    /// <summary>
    ///     The range of allowed values for the capacity limit.
    /// </summary>
    public FloatRange Limit;

    /// <summary>
    ///     Default constructor for serialization.
    /// </summary>
    [UsedImplicitly]
    public PawnCapacityLimit() { }

    /// <summary>
    ///     Initializes a new instance of <see cref="PawnCapacityLimit" /> for the specified capacity definition.
    /// </summary>
    /// <param name="def">The pawn capacity definition.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="def" /> is null.</exception>
    public PawnCapacityLimit([NotNull] PawnCapacityDef def) : base(def.defName)
    {
        if (def == null) throw new ArgumentNullException(nameof(def));
        Limit = new FloatRange(LimitMinCap, LimitMaxCap);
    }

    /// <summary>
    ///     Gets the step value for the slider, based on the value style.
    /// </summary>
    internal float ValueStep
    {
        get
        {
            if (_valueStep == 0f) _valueStep = Fields.GetFloatSliderStepByValueStyle(ValueStyle);
            return _valueStep;
        }
    }

    /// <summary>
    ///     Serializes the limit data for saving/loading.
    /// </summary>
    public new void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref Limit, nameof(Limit));
    }
}