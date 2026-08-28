namespace XaliseConnect.Domain.Entities.Configuration
{
    /// <summary>
    /// Règle de corrélation permettant de traiter un flux.
    /// </summary>
    public sealed class CorrelationRule : BaseEntity
    {
        /// <summary>
        /// Libellé de la règle de corrélation.
        /// </summary>
        public string Label { get; private set; } = string.Empty;

        /// <summary>
        /// Description de la règle de corrélation.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Expression de la règle de corrélation.
        /// </summary>
        public string Expression { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private CorrelationRule() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="CorrelationRule"/> avec un libellé, une description et une expression.
        /// </summary>
        /// <param name="label">Libellé de la règle de corrélation.</param>
        /// <param name="description">Description de la règle de corrélation.</param>
        /// <param name="expression">Expression de la règle de corrélation.</param>
        public CorrelationRule(string label, string description, string expression)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));
            ArgumentNullException.ThrowIfNull(description, nameof(description));
            ArgumentException.ThrowIfNullOrWhiteSpace(expression, nameof(expression));

            this.Label = label;
            this.Description = description;
            this.Expression = expression;
        }
    }
}