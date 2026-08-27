namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des statuts de réception utilisés par l'application.
    /// </summary>
    public sealed class StatusReception : BaseEntity
    {
        /// <summary>
        /// Description du statut de réception.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private StatusReception() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="StatusReception"/> avec une description.
        /// </summary>
        /// <param name="description">Description du statut de réception.</param>
        public StatusReception(string description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description, nameof(description));

            this.Description = description;
        }
    }
}