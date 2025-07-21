using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace LordKuper.Common
{
    /// <summary>
    ///     Represents a weighted stat definition, with optional protection and serialization support.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [UsedImplicitly]
    public class StatWeight : IExposable
    {
        /// <summary>
        ///     The maximum allowed weight value.
        /// </summary>
        public const float WeightCap = 2f;

        private bool _isInitialized;
        private bool _protected;
        private StatDef _statDef;
        private string _statDefName;

        /// <summary>
        ///     The weight assigned to the stat.
        /// </summary>
        public float Weight;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatWeight" /> class using a <see cref="StatDef" /> and protection
        ///     flag.
        /// </summary>
        /// <param name="statDef">The stat definition.</param>
        /// <param name="isProtected">Whether the stat is protected.</param>
        public StatWeight(StatDef statDef, bool isProtected) : this(statDef.defName, isProtected)
        {
            _statDef = statDef;
            _isInitialized = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatWeight" /> class using a <see cref="StatDef" />, weight, and
        ///     protection flag.
        /// </summary>
        /// <param name="statDef">The stat definition.</param>
        /// <param name="weight">The weight value.</param>
        /// <param name="isProtected">Whether the stat is protected.</param>
        public StatWeight(StatDef statDef, float weight, bool isProtected) : this(statDef, isProtected)
        {
            Weight = weight;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatWeight" /> class.
        /// </summary>
        public StatWeight()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatWeight" /> class using a stat definition name and protection flag.
        /// </summary>
        /// <param name="statDefName">The name of the stat definition.</param>
        /// <param name="isProtected">Whether the stat is protected.</param>
        public StatWeight(string statDefName, bool isProtected)
        {
            _statDefName = statDefName;
            _protected = isProtected;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatWeight" /> class using a stat definition name, weight, and
        ///     protection flag.
        /// </summary>
        /// <param name="statDefName">The name of the stat definition.</param>
        /// <param name="weight">The weight value.</param>
        /// <param name="isProtected">Whether the stat is protected.</param>
        public StatWeight(string statDefName, float weight, bool isProtected) : this(statDefName, isProtected)
        {
            Weight = weight;
        }

        /// <summary>
        ///     Gets a value indicating whether this stat is protected.
        /// </summary>
        public bool Protected => _protected;

        /// <summary>
        ///     Gets the <see cref="StatDef" /> associated with this instance.
        /// </summary>
        public StatDef StatDef
        {
            get
            {
                Initialize();
                return _statDef;
            }
        }

        /// <summary>
        ///     Gets the name of the stat definition.
        /// </summary>
        public string StatDefName => _statDefName;

        /// <summary>
        ///     Serializes and deserializes the data for this instance.
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look(ref _statDefName, nameof(StatDefName));
            Scribe_Values.Look(ref _protected, nameof(Protected));
            Scribe_Values.Look(ref Weight, nameof(Weight));
        }

        /// <summary>
        ///     Initializes the <see cref="StatDef" /> from its name if not already initialized.
        /// </summary>
        private void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            _statDef = StatHelper.GetStatDef(_statDefName);
        }
    }
}