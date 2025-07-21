using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace LordKuper.Common
{
    /// <summary>
    ///     Provides a mapping between <see cref="WorkTypeDef" /> and relevant <see cref="StatDef" />s with associated weights.
    ///     Used to determine which stats are important for each work type in the game.
    /// </summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class WorkTypeStatMap
    {
        /// <summary>
        ///     Internal cache for the mapping between <see cref="WorkTypeDef" /> and the set of <see cref="StatDef" />s
        ///     required for auto-switching logic.
        /// </summary>
        private static Dictionary<WorkTypeDef, HashSet<StatDef>> _autoSwitchStatsMap;

        /// <summary>
        ///     Internal cache for the mapping between <see cref="WorkTypeDef" /> and their relevant <see cref="StatDef" />s
        ///     with weights.
        /// </summary>
        private static Dictionary<WorkTypeDef, Dictionary<StatDef, StatWeight>> _defaultStatsMap;

        /// <summary>
        ///     Default stat weights for specific work types, keyed by work type defName and stat defName.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, float>> DefaultWorkTypeStats =
            new Dictionary<string, Dictionary<string, float>>
            {
                {
                    "Cooking", new Dictionary<string, float>
                    {
                        { "FoodPoisonChance", 2f }, { "DrugCookingSpeed", 1f }, { "ButcheryFleshSpeed", 1f },
                        { "ButcheryFleshEfficiency", 1.5f }, { "CookSpeed", 1f }
                    }
                },
                { "Hunting", new Dictionary<string, float> { { "HuntingStealth", 2f } } },
                {
                    "Doctor",
                    new Dictionary<string, float> { { "MedicalTendQualityOffset", 2f }, { "MedicalPotency", 2f } }
                }
            };

        /// <summary>
        ///     Gets the mapping between <see cref="WorkTypeDef" /> and the set of <see cref="StatDef" />s that are required for
        ///     auto-switching logic.
        ///     The map is built on first access. If the map is not yet built, it will be constructed by calling
        ///     <see cref="BuildMap" />.
        /// </summary>
        public static Dictionary<WorkTypeDef, HashSet<StatDef>> AutoSwitchStatsMap
        {
            get
            {
                if (_autoSwitchStatsMap == null) BuildMap();
                return _autoSwitchStatsMap;
            }
        }

        /// <summary>
        ///     Gets the mapping between <see cref="WorkTypeDef" /> and their relevant <see cref="StatDef" />s with weights.
        ///     The map is built on first access. If the map is not yet built, it will be constructed by calling
        ///     <see cref="BuildMap" />.
        /// </summary>
        public static Dictionary<WorkTypeDef, Dictionary<StatDef, StatWeight>> DefaultStatsMap
        {
            get
            {
                if (_defaultStatsMap == null) BuildMap();
                return _defaultStatsMap;
            }
        }

        /// <summary>
        ///     Builds the mapping between work types and their relevant stats, including default weights,
        ///     skill-based stats, and recipe-based stats.
        ///     This method is called on first access to the <see cref="DefaultStatsMap" /> property.
        ///     It populates the internal <see cref="_defaultStatsMap" /> dictionary with stat weights for each
        ///     <see cref="WorkTypeDef" />.
        ///     The process includes:
        ///     <list type="number">
        ///         <item>Adding default stat weights from <see cref="DefaultWorkTypeStats" />.</item>
        ///         <item>Adding stats from relevant skills using <see cref="SkillStatMap" />.</item>
        ///         <item>Adding stats from recipes associated with the work type.</item>
        ///         <item>
        ///             Removing stats not considered relevant for work types (filtered by
        ///             <see cref="StatHelper.WorkTypeStatDefs" />).
        ///         </item>
        ///     </list>
        /// </summary>
        private static void BuildMap()
        {
#if DEBUG
            Logger.LogMessage($"Building {nameof(WorkTypeStatMap)}...");
#endif
            _defaultStatsMap = new Dictionary<WorkTypeDef, Dictionary<StatDef, StatWeight>>();
            _autoSwitchStatsMap = new Dictionary<WorkTypeDef, HashSet<StatDef>>();
            foreach (var workType in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder)
            {
                var statWeights = new Dictionary<StatDef, StatWeight>();
                _defaultStatsMap.Add(workType, statWeights);
                var requiredStats = new HashSet<StatDef>();
                _autoSwitchStatsMap.Add(workType, requiredStats);

                // Add default stat weights for this work type
                if (DefaultWorkTypeStats.TryGetValue(workType.defName, out var defaultStatWeights))
                    foreach (var kvp in defaultStatWeights)
                    {
                        var statDef = DefDatabase<StatDef>.GetNamedSilentFail(kvp.Key);
                        if (statDef != null) statWeights[statDef] = new StatWeight(statDef, kvp.Value, false);
                    }

                // Add stats from relevant skills
                foreach (var statDef in workType.relevantSkills.Where(skill => SkillStatMap.Map.ContainsKey(skill))
                             .Select(skill => SkillStatMap.Map[skill]).SelectMany(stats => stats))
                {
                    requiredStats.Add(statDef);
                    if (!statWeights.ContainsKey(statDef))
                        statWeights.Add(statDef, new StatWeight(statDef, 1f, true));
                }

                // Add stats from recipes associated with this work type
                foreach (var recipe in DefDatabase<RecipeDef>.AllDefs.Where(recipeDef =>
                             recipeDef.requiredGiverWorkType == workType))
                {
                    if (recipe.efficiencyStat != null && !statWeights.ContainsKey(recipe.efficiencyStat))
                        statWeights.Add(recipe.efficiencyStat, new StatWeight(recipe.efficiencyStat, 0.8f, false));
                    if (recipe.workSpeedStat != null)
                    {
                        requiredStats.Add(recipe.workSpeedStat);
                        if (!statWeights.ContainsKey(recipe.workSpeedStat))
                            statWeights.Add(recipe.workSpeedStat, new StatWeight(recipe.workSpeedStat, 0.5f, true));
                    }
                    if (recipe.workTableEfficiencyStat != null &&
                        !statWeights.ContainsKey(recipe.workTableEfficiencyStat))
                        statWeights.Add(recipe.workTableEfficiencyStat,
                            new StatWeight(recipe.workTableEfficiencyStat, 0.8f, false));
                    if (recipe.workTableSpeedStat != null && !statWeights.ContainsKey(recipe.workTableSpeedStat))
                        statWeights.Add(recipe.workTableSpeedStat,
                            new StatWeight(recipe.workTableSpeedStat, 0.5f, false));
                }
#if DEBUG
                Logger.LogMessage(
                    $"Stats for {workType.defName} before clearing: {string.Join(", ", statWeights.Select(kvp => kvp.Value.Protected ? $"*{kvp.Key.defName}" : kvp.Key.defName))}");
#endif
                // Remove stats that are not considered relevant for work types
                statWeights.RemoveAll(sw => !StatHelper.WorkTypeStatDefs.Contains(sw.Key));
#if DEBUG
                Logger.LogMessage(
                    $"Stats for {workType.defName} after clearing: {string.Join(", ", statWeights.Select(kvp => kvp.Value.Protected ? $"*{kvp.Key.defName}" : kvp.Key.defName))}");
#endif
            }
        }
    }
}