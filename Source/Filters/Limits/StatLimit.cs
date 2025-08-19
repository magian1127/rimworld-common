using System;
using System.Globalization;
using JetBrains.Annotations;
using LordKuper.Common.Cache;
using LordKuper.Common.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace LordKuper.Common.Filters.Limits;

/// <summary>
///     Represents a limit for a stat, including minimum and maximum values and their buffers for UI input.
/// </summary>
[UsedImplicitly]
public class StatLimit : DefCache<StatDef>, IExposable
{
    /// <summary>
    ///     The maximum allowed value for percent-based stats.
    /// </summary>
    private const float PercentStatCap = 5f;

    /// <summary>
    ///     Buffer for the maximum value input in the UI.
    /// </summary>
    private string _maxValueBuffer;

    /// <summary>
    ///     Buffer for the minimum value input in the UI.
    /// </summary>
    private string _minValueBuffer;

    /// <summary>
    ///     Represents the incremental step value for a floating-point operation or calculation.
    /// </summary>
    private float _valueStep;

    /// <summary>
    ///     The current range limit for the stat.
    /// </summary>
    public FloatRange Limit;

    /// <summary>
    ///     The maximum cap for the stat limit.
    /// </summary>
    public float LimitMaxCap;

    /// <summary>
    ///     The minimum cap for the stat limit.
    /// </summary>
    public float LimitMinCap;

    /// <summary>
    ///     The style used for converting float values to string.
    /// </summary>
    public ToStringStyle ValueStyle;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StatLimit" /> class.
    /// </summary>
    [UsedImplicitly]
    public StatLimit() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="StatLimit" /> class with the specified stat definition.
    /// </summary>
    /// <param name="def">The stat definition to use for limits and formatting.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="def" /> is null.</exception>
    public StatLimit([NotNull] StatDef def) : base(def.defName)
    {
        if (def == null) throw new ArgumentNullException(nameof(def));
        var style = def.toStringStyle;
        if (style is ToStringStyle.PercentZero or ToStringStyle.PercentOne or ToStringStyle.PercentTwo)
        {
            LimitMinCap = Mathf.Max(-1 * PercentStatCap, def.minValue);
            LimitMaxCap = Mathf.Min(PercentStatCap, def.maxValue);
            ValueStyle = style;
        }
        else
        {
            LimitMinCap = def.minValue;
            LimitMaxCap = def.maxValue;
            ValueStyle = ToStringStyle.FloatTwo;
        }
        Limit = new FloatRange(LimitMinCap, LimitMaxCap);
    }

    /// <summary>
    ///     Gets or sets the buffer string for the maximum value input.
    ///     When set, parses and clamps the value; when got, returns the formatted value if available.
    /// </summary>
    [NotNull]
    internal string MaxValueBuffer
    {
        get
        {
            if (string.IsNullOrEmpty(_maxValueBuffer))
                _maxValueBuffer = Limit.TrueMax.ToString("F2");
            return _maxValueBuffer;
        }
        set
        {
            if (ReferenceEquals(value, _maxValueBuffer) || value == _maxValueBuffer) return;
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var maxValue))
            {
                var clamped = Mathf.Clamp(maxValue, LimitMinCap, LimitMaxCap);
                if (!Mathf.Approximately(Limit.max, clamped))
                    Limit.max = clamped;
                _maxValueBuffer = clamped.ToString("F2");
            }
            else
            {
                _maxValueBuffer = value;
            }
        }
    }

    /// <summary>
    ///     Gets or sets the buffer string for the minimum value input.
    ///     When set, parses and clamps the value; when got, returns the formatted value if available.
    /// </summary>
    [NotNull]
    internal string MinValueBuffer
    {
        get
        {
            if (string.IsNullOrEmpty(_minValueBuffer))
                _minValueBuffer = Limit.TrueMin.ToString("F2");
            return _minValueBuffer;
        }
        set
        {
            if (ReferenceEquals(value, _minValueBuffer) || value == _minValueBuffer) return;
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var minValue))
            {
                var clamped = Mathf.Clamp(minValue, LimitMinCap, LimitMaxCap);
                if (!Mathf.Approximately(Limit.min, clamped))
                    Limit.min = clamped;
                _minValueBuffer = clamped.ToString("F2");
            }
            else
            {
                _minValueBuffer = value;
            }
        }
    }

    /// <summary>
    ///     Gets the incremental step value used for adjusting a float slider based on the specified value style.
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
    ///     Exposes the data for saving and loading.
    /// </summary>
    public new void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref Limit, nameof(Limit));
        Scribe_Values.Look(ref LimitMinCap, nameof(LimitMinCap));
        Scribe_Values.Look(ref LimitMaxCap, nameof(LimitMaxCap));
        Scribe_Values.Look(ref ValueStyle, nameof(ValueStyle));
    }
}