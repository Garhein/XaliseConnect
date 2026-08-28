using XaliseConnect.Domain.Entities.Reference;

namespace XaliseConnect.Domain.Entities.Configuration
{
    /// <summary>
    /// Source de laquelle provient une demande d'intégration.
    /// </summary>
    public sealed class Source : BaseEntity
    {
        /// <summary>
        /// Libellé de la source.
        /// </summary>
        public string Label { get; private set; } = string.Empty;

        /// <summary>
        /// Description de la source.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Date et heure d'archivage de la source.<br/>
        /// Si la source n'est pas archivée, cette valeur est <see langword="null"/>.
        /// </summary>
        public DateTime? ArchivedAt { get; private set; }

        /// <summary>
        /// Identifiant du type de source associé à cette source.
        /// </summary>
        public int SourceTypeId { get; private set; }

        /// <summary>
        /// Type de source associé à cette source.
        /// </summary>
        public SourceType SourceType { get; private set; } = null!;

        /// <summary>
        /// Indique <see langword="true"/> si la source est archivée, sinon <see langword="false"/>.
        /// </summary>
        public bool IsArchived => this.ArchivedAt.HasValue;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private Source() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="Source"/> avec un libellé, une description et un type de source.
        /// </summary>
        /// <param name="label">Libellé de la source.</param>
        /// <param name="description">Description de la source.</param>
        /// <param name="sourceType">Type de source associé à cette source.</param>
        public Source(string label, string description, SourceType sourceType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));
            ArgumentNullException.ThrowIfNull(description, nameof(description));
            ArgumentNullException.ThrowIfNull(sourceType, nameof(sourceType));

            this.Label = label;
            this.Description = description;
            this.SourceTypeId = sourceType.Id;
            this.SourceType = sourceType;
        }

        /// <summary>
        /// Archive la source en définissant la date et l'heure d'archivage à la date et l'heure actuelles (UTC).
        /// </summary>
        /// <exception cref="InvalidOperationException">Si <see cref="ArchivedAt"/> est déjà défini.</exception>
        public void Archive()
        {
            if (this.ArchivedAt.HasValue)
            {
                throw new InvalidOperationException("La source est déjà archivée.");
            }

            this.ArchivedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Désarchive la source en réinitialisant la date et l'heure d'archivage à <see langword="null"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si <see cref="ArchivedAt"/> n'est pas défini.</exception>
        public void Unarchive()
        {
            if (!this.ArchivedAt.HasValue)
            {
                throw new InvalidOperationException("La source n'est pas archivée.");
            }

            this.ArchivedAt = null;
        }
    }
}
