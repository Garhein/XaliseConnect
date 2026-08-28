namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des statuts d'intégration utilisés par l'application.
    /// </summary>
    public sealed class StatusIntegration : BaseEntity
    {
        /// <summary>
        /// Libellé du statut d'intégration.
        /// </summary>
        public string Label { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private StatusIntegration() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="StatusIntegration"/> avec un libellé.
        /// </summary>
        /// <param name="label">Libellé du statut d'intégration.</param>
        public StatusIntegration(string label)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));

            this.Label = label;
        }
    }
}