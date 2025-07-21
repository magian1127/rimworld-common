using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace LordKuper.Common
{
    /// <summary>
    ///     Represents a rule that associates a work type with a set of stat weights for evaluating things.
    /// </summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class WorkTypeThingRule : IExposable
    {
        private bool _isInitialized;
        private Dictionary<string, StatWeight> _statWeights = new Dictionary<string, StatWeight>();
        private WorkTypeDef _workTypeDef;
        private string _workTypeDefName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkTypeThingRule" /> class with the specified work type definition
        ///     name.
        /// </summary>
        /// <param name="workTypeDefName">The name of the work type definition.</param>
        public WorkTypeThingRule(string workTypeDefName)
        {
            _workTypeDefName = workTypeDefName;
        }

        /// <summary>
        ///     Gets the default rules for all work types defined in <see cref="WorkTypeStatMap.DefaultStatsMap" />.
        /// </summary>
        public static IEnumerable<WorkTypeThingRule> DefaultRules
        {
            get
            {
                var rules = new List<WorkTypeThingRule>();
                foreach (var map in WorkTypeStatMap.DefaultStatsMap)
                {
                    var rule = new WorkTypeThingRule(map.Key.defName);
                    foreach (var statWeight in map.Value.Values)
                        rule.SetStatWeight(statWeight.StatDef, statWeight.Weight);
                    rules.Add(rule);
                }
                return rules;
            }
        }

        /// <summary>
        ///     Gets the label for this rule, using the short label of the work type if available.
        /// </summary>
        public string Label =>
            WorkTypeDef != null
                ? WorkTypeDef.labelShort.NullOrEmpty() ? WorkTypeDefName : WorkTypeDef.labelShort.CapitalizeFirst()
                : WorkTypeDefName;

        /// <summary>
        ///     Gets the collection of <see cref="StatWeight" /> objects associated with this rule.
        /// </summary>
        public IEnumerable<StatWeight> StatWeights
        {
            get
            {
                Initialize();
                return _statWeights.Values;
            }
        }

        /// <summary>
        ///     Gets the <see cref="WorkTypeDef" /> associated with this rule.
        /// </summary>
        private WorkTypeDef WorkTypeDef
        {
            get
            {
                Initialize();
                return _workTypeDef;
            }
        }

        /// <summary>
        ///     Gets the name of the work type definition associated with this rule.
        /// </summary>
        public string WorkTypeDefName => _workTypeDefName;

        /// <summary>
        ///     Exposes data for serialization.
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look(ref _workTypeDefName, nameof(WorkTypeDefName));
            Scribe_Collections.Look(ref _statWeights, nameof(StatWeights), LookMode.Value, LookMode.Deep);
        }

        /// <summary>
        ///     Calculates a normalized [-1..1] score for the specified <see cref="Thing" /> based on the stat weights of this
        ///     rule.
        /// </summary>
        /// <param name="thing">The thing to score.</param>
        /// <returns>The calculated score.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thing" /> is null.</exception>
        public float GetThingScore([NotNull] Thing thing)
        {
            return thing == null
                ? throw new ArgumentNullException(nameof(thing))
                : _statWeights.Values.Where(sw => sw.StatDef != null).Sum(sw =>
                    StatRanges.NormalizeStatValue(sw.StatDef, StatHelper.GetStatValueDeviation(thing, sw.StatDef)) *
                    sw.Weight);
        }

        /// <summary>
        ///     Initializes the rule by loading the work type definition and default stat weights if not already initialized.
        /// </summary>
        private void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            _workTypeDef = DefDatabase<WorkTypeDef>.GetNamedSilentFail(_workTypeDefName);
            if (_workTypeDef == null) return;
            if (WorkTypeStatMap.DefaultStatsMap.TryGetValue(_workTypeDef, out var defaultStatWeights))
                foreach (var kvp in defaultStatWeights.Where(kvp => !_statWeights.ContainsKey(kvp.Key.defName)))
                    _statWeights.Add(kvp.Key.defName, new StatWeight(kvp.Key, kvp.Value.Weight, kvp.Value.Protected));
        }

        /// <summary>
        ///     Sets the weight for a specific stat in this rule.
        /// </summary>
        /// <param name="statDef">The stat definition.</param>
        /// <param name="weight">The weight to assign.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="statDef" /> is null.</exception>
        public void SetStatWeight([NotNull] StatDef statDef, float weight)
        {
            if (statDef == null) throw new ArgumentNullException(nameof(statDef));
            if (!_statWeights.TryGetValue(statDef.defName, out var statWeight))
            {
                statWeight = new StatWeight(statDef, false);
                _statWeights.Add(statDef.defName, statWeight);
            }
            statWeight.Weight = weight;
        }
    }
}