namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des statuts d'intégration utilisés par l'application.
    /// </summary>
    public sealed class StatusIntegration : BaseEntity
    {
        /// <summary>
        /// Description du statut d'intégration.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private StatusIntegration() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="StatusIntegration"/> avec une description.
        /// </summary>
        /// <param name="description">Description du statut d'intégration.</param>
        public StatusIntegration(string description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description, nameof(description));

            this.Description = description;
        }
    }
}