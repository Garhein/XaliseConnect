using XaliseConnect.Domain.Entities.Reference;

namespace XaliseConnect.Domain.Entities.Workflow
{
    /// <summary>
    /// Groupe de dépendances d'un flux.
    /// </summary>
    public sealed class WorkflowDependencyGroup : BaseEntity
    {
        /// <summary>
        /// Éléments de dépendance composant le groupe.
        /// </summary>
        private readonly List<WorkflowDependencyItem> _items = [];

        /// <summary>
        /// Identifiant de l'événement du flux associé au groupe de dépendances.
        /// </summary>
        public int WorkflowEventId { get; private set; }

        /// <summary>
        /// Événement du flux associé au groupe de dépendances.
        /// </summary>
        public WorkflowEvent WorkflowEvent { get; private set; } = null!;

        /// <summary>
        /// Identifiant du type de dépendance du flux associé au groupe de dépendances.
        /// </summary>
        public int WorkflowDependencyTypeId { get; private set; }

        /// <summary>
        /// Type de dépendance du flux associé au groupe de dépendances.
        /// </summary>
        public WorkflowDependencyType WorkflowDependencyType { get; private set; } = null!;

        /// <summary>
        /// Collection en lecture seule des éléments de dépendance du groupe.
        /// </summary>
        public IReadOnlyCollection<WorkflowDependencyItem> Items => this._items.AsReadOnly();

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private WorkflowDependencyGroup() { }

        /// <summary>
        /// Constructeur pour créer une instance de <see cref="WorkflowDependencyGroup"/> avec un événement du flux et un type de dépendance du flux.
        /// </summary>
        /// <param name="workflowEvent">Événement du flux associé au groupe de dépendances.</param>
        /// <param name="workflowDependencyType">Type de dépendance du flux associé au groupe de dépendances.</param>
        internal WorkflowDependencyGroup(WorkflowEvent workflowEvent, WorkflowDependencyType workflowDependencyType)
        {
            ArgumentNullException.ThrowIfNull(workflowEvent, nameof(workflowEvent));
            ArgumentNullException.ThrowIfNull(workflowDependencyType, nameof(workflowDependencyType));

            this.WorkflowEvent = workflowEvent;
            this.WorkflowEventId = workflowEvent.Id;
            this.WorkflowDependencyType = workflowDependencyType;
            this.WorkflowDependencyTypeId = workflowDependencyType.Id;
        }

        /// <summary>
        /// Ajoute un événement requis au groupe de dépendances.
        /// </summary>
        /// <param name="workflowEvent">Événement requis.</param>
        /// <returns>L'élément de dépendance ajouté.</returns>
        /// <exception cref="ArgumentNullException">Levée lorsque <paramref name="workflowEvent"/> est <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Levée lorsque l'événement appartient à un autre workflow, dépend de lui-même, est déjà présent dans le groupe ou crée un cycle de dépendances.</exception>
        public WorkflowDependencyItem AddDependencyItem(WorkflowEvent workflowEvent)
        {
            ArgumentNullException.ThrowIfNull(workflowEvent, nameof(workflowEvent));

            if (!ReferenceEquals(workflowEvent.Workflow, this.WorkflowEvent.Workflow))
            {
                throw new InvalidOperationException("Un événement de dépendance doit appartenir au même flux de travail.");
            }

            if (ReferenceEquals(workflowEvent, this.WorkflowEvent))
            {
                throw new InvalidOperationException("Un événement ne peut pas dépendre de lui-même.");
            }

            if (this._items.Any(item => ReferenceEquals(item.WorkflowEvent, workflowEvent)))
            {
                throw new InvalidOperationException("Un événement ne peut apparaître qu'une fois dans un groupe de dépendances.");
            }

            this.WorkflowEvent.Workflow.EnsureDependencyDoesNotCreateCycle(this.WorkflowEvent, workflowEvent);

            WorkflowDependencyItem workflowDependencyItem = new WorkflowDependencyItem(this, workflowEvent);

            this._items.Add(workflowDependencyItem);

            return workflowDependencyItem;
        }
    }
}