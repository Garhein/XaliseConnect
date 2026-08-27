namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des statuts de transport utilisés par l'application.
    /// </summary>
    public sealed class StatusTransport : BaseEntity
    {
        /// <summary>
        /// Description du statut de transport.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private StatusTransport() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="StatusTransport"/> avec une description.
        /// </summary>
        /// <param name="description">Description du statut de transport.</param>
        public StatusTransport(string description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description, nameof(description));

            this.Description = description;
        }
    }
}