namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des types de sources, desquelles proviennent les demandes d'intégration, utilisés par l'application.
    /// </summary>
    public sealed class SourceType : BaseEntity
    {
        /// <summary>
        /// Libellé du type de source.
        /// </summary>
        public string Label { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private SourceType() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="SourceType"/> avec un libellé.
        /// </summary>
        /// <param name="label">Libellé du type de source.</param>
        public SourceType(string label)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));

            this.Label = label;
        }
    }
}