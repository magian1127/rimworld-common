using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LordKuper.Common
{
    /// <summary>
    ///     Provides a mapping between <see cref="SkillDef" /> and the set of <see cref="StatDef" />s
    ///     that are affected by skill need factors and offsets.
    /// </summary>
    public static class SkillStatMap
    {
        /// <summary>
        ///     Backing field for the <see cref="Map" /> property.
        /// </summary>
        private static Dictionary<SkillDef, HashSet<StatDef>> _map;

        /// <summary>
        ///     Gets the mapping of <see cref="SkillDef" /> to the set of <see cref="StatDef" />s
        ///     that are influenced by skill need factors and offsets.
        ///     The map is built on first access.
        /// </summary>
        public static Dictionary<SkillDef, HashSet<StatDef>> Map
        {
            get
            {
                if (_map == null) BuildMap();
                return _map;
            }
        }

        /// <summary>
        ///     Builds the mapping between <see cref="SkillDef" /> and <see cref="StatDef" />s
        ///     by analyzing all stat definitions for skill need factors and offsets.
        /// </summary>
        private static void BuildMap()
        {
#if DEBUG
            Logger.LogMessage($"Building  {nameof(SkillStatMap)}...");
#endif
            _map = new Dictionary<SkillDef, HashSet<StatDef>>();
            foreach (var key in DefDatabase<SkillDef>.AllDefsListForReading)
                _map[key] = new HashSet<StatDef>();
            foreach (var stat in DefDatabase<StatDef>.AllDefsListForReading)
            {
                if (stat.skillNeedFactors != null)
                    foreach (var stats in stat.skillNeedFactors.Select(needFactor => _map[needFactor.skill]))
                    {
                        stats.Add(stat);
                        if (stat.statFactors == null) continue;
                        foreach (var factor in stat.statFactors) stats.Add(factor);
                    }
                if (stat.skillNeedOffsets != null)
                    foreach (var stats in stat.skillNeedOffsets.Select(needOffset => _map[needOffset.skill]))
                    {
                        stats.Add(stat);
                        if (stat.statFactors == null) continue;
                        foreach (var factor in stat.statFactors) stats.Add(factor);
                    }
            }
#if DEBUG
            foreach (var kvp in _map) Logger.LogMessage($"{kvp.Key.defName}: {string.Join(", ", kvp.Value)}");
#endif
        }
    }
}