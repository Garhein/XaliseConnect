namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des statuts de réception utilisés par l'application.
    /// </summary>
    public sealed class StatusReception : BaseEntity
    {
        /// <summary>
        /// Libellé du statut de réception.
        /// </summary>
        public string Label { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private StatusReception() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="StatusReception"/> avec un libellé.
        /// </summary>
        /// <param name="label">Libellé du statut de réception.</param>
        public StatusReception(string label)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));

            this.Label = label;
        }
    }
}