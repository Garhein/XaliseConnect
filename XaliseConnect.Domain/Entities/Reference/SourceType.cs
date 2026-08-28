namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des types de sources, desquelles proviennent les demandes d'intégration, utilisés par l'application.
    /// </summary>
    public sealed class SourceType : BaseEntity
    {
        /// <summary>
        /// Description du type de source.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private SourceType() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="SourceType"/> avec une description.
        /// </summary>
        /// <param name="description">Description du type de source.</param>
        public SourceType(string description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description, nameof(description));

            this.Description = description;
        }
    }
}