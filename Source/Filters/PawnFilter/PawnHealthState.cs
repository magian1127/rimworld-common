namespace LordKuper.Common.Filters;

/// <summary>
///     Represents the health state of a pawn.
/// </summary>
public enum PawnHealthState
{
    /// <summary>
    ///     The pawn is in good health.
    /// </summary>
    Healthy,

    /// <summary>
    ///     The pawn is resting, recovering from injuries or illnesses.
    /// </summary>
    Resting,

    /// <summary>
    ///     The pawn requires medical attention or tending.
    /// </summary>
    NeedsTending,

    /// <summary>
    ///     The pawn is downed and unable to move or act.
    /// </summary>
    Downed,

    /// <summary>
    ///     The pawn is experiencing a mental break.
    /// </summary>
    Mental,

    /// <summary>
    ///     The pawn is deceased.
    /// </summary>
    Dead
}