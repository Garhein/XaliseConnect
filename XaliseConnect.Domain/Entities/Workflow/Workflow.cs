using XaliseConnect.Domain.Entities.Configuration;
using XaliseConnect.Domain.Entities.Reference;

namespace XaliseConnect.Domain.Entities.Workflow
{
    /// <summary>
    /// Définition d'un flux.
    /// </summary>
    public sealed class Workflow : BaseEntity
    {
        /// <summary>
        /// Collection des événements associés au flux.
        /// </summary>
        private readonly List<WorkflowEvent> _events = [];

        /// <summary>
        /// Libellé du flux.
        /// </summary>
        public string Label { get; private set; } = string.Empty;

        /// <summary>
        /// Description du flux.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Date et heure d'archivage du flux.<br/>
        /// Si le flux n'est pas archivé, cette valeur est <see langword="null"/>.
        /// </summary>
        public DateTime? ArchivedAt { get; private set; }

        /// <summary>
        /// Numéro de version du flux.<br/>
        /// Doit être supérieur à 0.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Identifiant de la règle de corrélation associée au flux.
        /// </summary>
        public int CorrelationRuleId { get; private set; }

        /// <summary>
        /// Règle de corrélation associée au flux.
        /// </summary>
        public CorrelationRule CorrelationRule { get; private set; } = null!;

        /// <summary>
        /// Indique <see langword="true"/> si le flux est archivé, sinon <see langword="false"/>.
        /// </summary>
        public bool IsArchived => this.ArchivedAt.HasValue;

        /// <summary>
        /// Collection des événements associés au flux.
        /// </summary>
        public IReadOnlyCollection<WorkflowEvent> Events => this._events.AsReadOnly();

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private Workflow() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="Workflow"/> avec un libellé, une description, un numéro de version et une règle de corrélation.
        /// </summary>
        /// <param name="label">Libellé du flux.</param>
        /// <param name="description">Description du flux.</param>
        /// <param name="version">Numéro de version du flux.</param>
        /// <param name="correlationRule">Règle de corrélation associée au flux.</param>
        public Workflow(string label, string description, int version, CorrelationRule correlationRule)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));
            ArgumentNullException.ThrowIfNull(description, nameof(description));
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(version, 0, nameof(version));
            ArgumentNullException.ThrowIfNull(correlationRule, nameof(correlationRule));

            this.Label = label;
            this.Description = description;
            this.Version = version;
            this.CorrelationRuleId = correlationRule.Id;
            this.CorrelationRule = correlationRule;
        }

        /// <summary>
        /// Archive le flux en définissant la date et l'heure d'archivage à la date et l'heure actuelles (UTC).
        /// </summary>
        /// <exception cref="InvalidOperationException">Si <see cref="ArchivedAt"/> est déjà défini.</exception>
        public void Archive()
        {
            if (this.ArchivedAt.HasValue)
            {
                throw new InvalidOperationException("Le flux est déjà archivé.");
            }

            this.ArchivedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Désarchive le flux en réinitialisant la date et l'heure d'archivage à <see langword="null"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si <see cref="ArchivedAt"/> n'est pas défini.</exception>
        public void Unarchive()
        {
            if (!this.ArchivedAt.HasValue)
            {
                throw new InvalidOperationException("Le flux n'est pas archivé.");
            }

            this.ArchivedAt = null;
        }

        /// <summary>
        /// Ajoute un événement au flux avec les paramètres spécifiés.<br/>
        /// </summary>
        /// <param name="canReplay">Indique si l'événement peut être rejoué.</param>
        /// <param name="executionOrder">Ordre d'exécution de l'événement.</param>
        /// <param name="minOccurrences">Nombre minimum d'occurrences de l'événement.</param>
        /// <param name="maxOccurrences">Nombre maximum d'occurrences de l'événement.</param>
        /// <param name="eventType">Type de l'événement.</param>
        /// <returns>L'événement ajouté au flux.</returns>
        /// <exception cref="ArgumentNullException">Levée lorsque <paramref name="eventType"/> est <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Levée lorsque l'ordre d'exécution ou le nombre d'occurrences est invalide.</exception>
        /// <exception cref="InvalidOperationException">Levée lorsqu'un événement du workflow possède déjà le même ordre d'exécution.</exception>
        public WorkflowEvent AddEvent(bool canReplay, int executionOrder, int minOccurrences, int? maxOccurrences, EventType eventType)
        {
            if (this._events.Any(x => x.ExecutionOrder == executionOrder))
            {
                throw new InvalidOperationException("Un flux de travail ne peut pas contenir deux événements avec le même ordre d'exécution.");
            }

            WorkflowEvent workflowEvent = new WorkflowEvent(canReplay, executionOrder, minOccurrences, maxOccurrences, this, eventType);

            this._events.Add(workflowEvent);

            return workflowEvent;
        }

        /// <summary>
        /// Vérifie que l'ajout d'une dépendance ne crée pas de cycle dans le workflow.
        /// </summary>
        /// <remarks>
        /// Après l'ajout de A → B, la tentative d'ajout de B → A est refusée.<br/>
        /// Le test parcourt aussi les cycles indirects : A → B → C, puis C → A.
        /// </remarks>
        /// <param name="dependentWorkflowEvent">Événement qui dépend de l'événement requis.</param>
        /// <param name="requiredWorkflowEvent">Événement requis par l'événement dépendant.</param>
        /// <exception cref="InvalidOperationException">Levée lorsque l'ajout crée un cycle de dépendances.</exception>
        internal void EnsureDependencyDoesNotCreateCycle(WorkflowEvent dependentWorkflowEvent, WorkflowEvent requiredWorkflowEvent)
        {
            HashSet<WorkflowEvent> visitedWorkflowEvents = [];

            if (this.HasDependencyPath(requiredWorkflowEvent, dependentWorkflowEvent, visitedWorkflowEvents))
            {
                throw new InvalidOperationException("L'ajout de cette dépendance crée un cycle dans le flux de travail.");
            }
        }

        /// <summary>
        /// Recherche un chemin de dépendances entre deux événements.
        /// </summary>
        /// <param name="currentWorkflowEvent">Événement actuellement analysé.</param>
        /// <param name="targetWorkflowEvent">Événement recherché.</param>
        /// <param name="visitedWorkflowEvents">Événements déjà analysés.</param>
        /// <returns><see langword="true"/> lorsqu'un chemin mène à l'événement recherché, sinon <see langword="false"/>.</returns>
        private bool HasDependencyPath(WorkflowEvent currentWorkflowEvent, WorkflowEvent targetWorkflowEvent, ISet<WorkflowEvent> visitedWorkflowEvents)
        {
            if (ReferenceEquals(currentWorkflowEvent, targetWorkflowEvent))
            {
                return true;
            }

            if (!visitedWorkflowEvents.Add(currentWorkflowEvent))
            {
                return false;
            }

            foreach (WorkflowDependencyGroup workflowDependencyGroup in currentWorkflowEvent.DependencyGroups)
            {
                foreach (WorkflowDependencyItem workflowDependencyItem in workflowDependencyGroup.Items)
                {
                    if (this.HasDependencyPath(workflowDependencyItem.WorkflowEvent, targetWorkflowEvent, visitedWorkflowEvents))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
