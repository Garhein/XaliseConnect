using XaliseConnect.Domain.Entities.Reference;

namespace XaliseConnect.Domain.Entities.Workflow
{
    /// <summary>
    /// Événement associé à un flux.
    /// </summary>
    public sealed class WorkflowEvent : BaseEntity
    {
        /// <summary>
        /// Indique <see langword="true"/> si l'événement peut être rejoué, sinon <see langword="false"/>.
        /// </summary>
        public bool CanReplay { get; private set; }

        /// <summary>
        /// Indique l'ordre d'exécution de l'événement dans le flux.
        /// </summary>
        public int ExecutionOrder { get; private set; }

        /// <summary>
        /// Indique le nombre minimum d'occurrences de l'événement dans le flux.<br/>
        /// Si la valeur est 0, l'événement est optionnel.
        /// </summary>
        public int MinOccurrences { get; private set; }

        /// <summary>
        /// Indique le nombre maximum d'occurrences de l'événement dans le flux.<br/>
        /// Si la valeur est <see langword="null"/>, il n'y a pas de limite.
        /// </summary>
        /// <remarks>
        /// Doit être <see langword="null"/> ou >= <see cref="MinOccurrences"/>.
        /// </remarks>
        public int? MaxOccurrences { get; private set; }

        /// <summary>
        /// Identifiant du flux associé à l'événement.
        /// </summary>
        public int WorkflowId { get; private set; }

        /// <summary>
        /// Flux associé à l'événement.
        /// </summary>
        public Workflow Workflow { get; private set; } = null!;

        /// <summary>
        /// Identifiant du type d'événement associé à l'événement.
        /// </summary>
        public int EventTypeId { get; private set; }

        /// <summary>
        /// Type d'événement associé à l'événement.
        /// </summary>
        public EventType EventType { get; private set; } = null!;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private WorkflowEvent() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="WorkflowEvent"/> avec les propriétés spécifiées.
        /// </summary>
        /// <param name="canReplay"><see langword="true"/> si l'événement peut être rejoué, sinon <see langword="false"/>.</param>
        /// <param name="executionOrder">Ordre d'exécution de l'événement dans le flux.</param>
        /// <param name="minOccurrences">Nombre minimum d'occurrences de l'événement dans le flux.</param>
        /// <param name="maxOccurrences">Nombre maximum d'occurrences de l'événement dans le flux.</param>
        /// <param name="workflow">Flux associé à l'événement.</param>
        /// <param name="eventType">Type d'événement associé à l'événement.</param>
        public WorkflowEvent(bool canReplay, int executionOrder, int minOccurrences, int? maxOccurrences, Workflow workflow, EventType eventType)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(executionOrder, 0, nameof(executionOrder));
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(minOccurrences, 0, nameof(minOccurrences));
            ArgumentNullException.ThrowIfNull(workflow, nameof(workflow));
            ArgumentNullException.ThrowIfNull(eventType, nameof(eventType));

            if (maxOccurrences.HasValue && maxOccurrences.Value < minOccurrences)
            {
                throw new ArgumentOutOfRangeException(nameof(maxOccurrences), "Le nombre maximum d'occurrences doit être supérieur ou égal au nombre minimum d'occurrences.");
            }  
            
            this.CanReplay = canReplay;
            this.ExecutionOrder = executionOrder;
            this.MinOccurrences = minOccurrences;
            this.MaxOccurrences = maxOccurrences;
            this.Workflow = workflow;
            this.WorkflowId = workflow.Id;
            this.EventType = eventType;
            this.EventTypeId = eventType.Id;
        }
    }
}
