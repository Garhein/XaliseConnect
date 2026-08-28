using XaliseConnect.Domain.Entities.Configuration;

namespace XaliseConnect.Domain.Entities.Workflow
{
    /// <summary>
    /// Définition d'un flux.
    /// </summary>
    public sealed class Workflow : BaseEntity
    {
        /// <summary>
        /// Libellé du flux.
        /// </summary>
        public string Label { get; private set; } = string.Empty;

        /// <summary>
        /// Description du flux.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Date et heure d'archivage du flux.<br/>
        /// Si le flux n'est pas archivé, cette valeur est <see langword="null"/>.
        /// </summary>
        public DateTime? ArchivedAt { get; private set; }

        /// <summary>
        /// Numéro de version du flux.<br/>
        /// Doit être supérieur à 0 et est incrémenté à chaque modification du flux.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Identifiant de la règle de corrélation associée au flux.
        /// </summary>
        public int CorrelationRuleId { get; private set; }

        /// <summary>
        /// Règle de corrélation associée au flux.
        /// </summary>
        public CorrelationRule CorrelationRule { get; private set; } = null!;

        /// <summary>
        /// Indique <see langword="true"/> si le flux est archivé, sinon <see langword="false"/>.
        /// </summary>
        public bool IsArchived => this.ArchivedAt.HasValue;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private Workflow() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="Workflow"/> avec un libellé, une description, un numéro de version et une règle de corrélation.
        /// </summary>
        /// <param name="label">Libellé du flux.</param>
        /// <param name="description">Description du flux.</param>
        /// <param name="version">Numéro de version du flux.</param>
        /// <param name="correlationRule">Règle de corrélation associée au flux.</param>
        public Workflow(string label, string description, int version, CorrelationRule correlationRule)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));
            ArgumentNullException.ThrowIfNull(description, nameof(description));
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(version, 0, nameof(version));
            ArgumentNullException.ThrowIfNull(correlationRule, nameof(correlationRule));

            this.Label = label;
            this.Description = description;
            this.Version = version;
            this.CorrelationRuleId = correlationRule.Id;
            this.CorrelationRule = correlationRule;
        }

        /// <summary>
        /// Archive le flux en définissant la date et l'heure d'archivage à la date et l'heure actuelles (UTC).
        /// </summary>
        /// <exception cref="InvalidOperationException">Si <see cref="ArchivedAt"/> est déjà défini.</exception>
        public void Archive()
        {
            if (this.ArchivedAt.HasValue)
            {
                throw new InvalidOperationException("Le flux est déjà archivé.");
            }

            this.ArchivedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Désarchive le flux en réinitialisant la date et l'heure d'archivage à <see langword="null"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si <see cref="ArchivedAt"/> n'est pas défini.</exception>
        public void Unarchive()
        {
            if (!this.ArchivedAt.HasValue)
            {
                throw new InvalidOperationException("Le flux n'est pas archivé.");
            }

            this.ArchivedAt = null;
        }
    }
}
