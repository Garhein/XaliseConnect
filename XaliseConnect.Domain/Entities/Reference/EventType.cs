namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des types d'événement utilisés par l'application.
    /// </summary>
    public sealed class EventType : BaseEntity
    {
        /// <summary>
        /// Code du type d'événement.
        /// </summary>
        public string Code { get; private set; } = string.Empty;

        /// <summary>
        /// Description du type d'événement.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private EventType() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="EventType"/> avec un code et une description.
        /// </summary>
        /// <param name="code">Code du type d'événement.</param>
        /// <param name="description">Description du type d'événement.</param>
        public EventType(string code, string description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code, nameof(code));
            ArgumentException.ThrowIfNullOrWhiteSpace(description, nameof(description));

            this.Code = code;
            this.Description = description;
        }
    }
}