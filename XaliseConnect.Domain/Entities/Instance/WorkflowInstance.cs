namespace XaliseConnect.Domain.Entities.Instance
{
    /// <summary>
    /// Instance d'un flux.
    /// </summary>
    public sealed class WorkflowInstance : BaseEntity
    {
        /// <summary>
        /// Date et heure de création de l'instance du flux.
        /// </summary>
        public DateTime CreatedAt { get; private set;  } = DateTime.UtcNow;

        /// <summary>
        /// Date et heure de la dernière réception d'un événement pour cette instance du flux.
        /// </summary>
        public DateTime LastEventReceivedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Valeur de la règle de corrélation utilisée pour identifier cette instance du flux.
        /// </summary>
        public string CorrelationRuleValue { get; private set; } = string.Empty;

        /// <summary>
        /// Identifiant du flux auquel cette instance appartient.
        /// </summary>
        public int WorkflowId { get; private set; }

        /// <summary>
        /// Flux auquel cette instance appartient.
        /// </summary>
        public Workflow.Workflow Workflow { get; private set; } = null!;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private WorkflowInstance() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="WorkflowInstance"/> avec les propriétés spécifiées.
        /// </summary>
        /// <param name="createdAt">Date et heure de création de l'instance du flux.</param>
        /// <param name="lastEventReceivedAt">Date et heure de la dernière réception d'un événement pour cette instance du flux.</param>
        /// <param name="correlationRuleValue">Valeur de la règle de corrélation utilisée pour identifier cette instance du flux.</param>
        /// <param name="workflow">Flux auquel cette instance appartient.</param>
        public WorkflowInstance(DateTime createdAt, DateTime lastEventReceivedAt, string correlationRuleValue, Workflow.Workflow workflow)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(correlationRuleValue, nameof(correlationRuleValue));
            ArgumentNullException.ThrowIfNull(workflow, nameof(workflow));

            this.CreatedAt = createdAt;
            this.LastEventReceivedAt = lastEventReceivedAt;
            this.CorrelationRuleValue = correlationRuleValue;
            this.WorkflowId = workflow.Id;
            this.Workflow = workflow;
        }
    }
}
