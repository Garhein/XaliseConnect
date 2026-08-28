namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des statuts de transport utilisés par l'application.
    /// </summary>
    public sealed class StatusTransport : BaseEntity
    {
        /// <summary>
        /// Libellé du statut de transport.
        /// </summary>
        public string Label { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private StatusTransport() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="StatusTransport"/> avec un libellé.
        /// </summary>
        /// <param name="label">Libellé du statut de transport.</param>
        public StatusTransport(string label)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));

            this.Label = label;
        }
    }
}