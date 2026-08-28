namespace XaliseConnect.Domain.Entities.Workflow
{
    /// <summary>
    /// Représente un élément de dépendance dans un flux de travail.
    /// </summary>
    public sealed class WorkflowDependencyItem : BaseEntity
    {
        /// <summary>
        /// Identifiant du groupe de dépendances auquel appartient l'élément de dépendance.
        /// </summary>
        public int WorkflowDependencyGroupId { get; private set; }

        /// <summary>
        /// Groupe de dépendances auquel appartient l'élément de dépendance.
        /// </summary>
        public WorkflowDependencyGroup WorkflowDependencyGroup { get; private set; } = null!;

        /// <summary>
        /// Événement du flux auquel appartient l'élément de dépendance.
        /// </summary>
        /// <remarks>
        /// Désigne un événement requis.
        /// </remarks>
        public int WorkflowEventId { get; private set; }

        /// <summary>
        /// Événement du flux auquel appartient l'élément de dépendance.
        /// </summary>
        /// <remarks>
        /// Désigne un événement requis.
        /// </remarks>
        public WorkflowEvent WorkflowEvent { get; private set; } = null!;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private WorkflowDependencyItem() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="WorkflowDependencyItem"/> avec un groupe de dépendances et un événement du flux.
        /// </summary>
        /// <param name="workflowDependencyGroup">Groupe de dépendances auquel appartient l'élément de dépendance.</param>
        /// <param name="workflowEvent">Événement du flux auquel appartient l'élément de dépendance.</param>
        public WorkflowDependencyItem(WorkflowDependencyGroup workflowDependencyGroup, WorkflowEvent workflowEvent)
        {
            ArgumentNullException.ThrowIfNull(workflowDependencyGroup, nameof(workflowDependencyGroup));
            ArgumentNullException.ThrowIfNull(workflowEvent, nameof(workflowEvent));

            this.WorkflowDependencyGroup = workflowDependencyGroup;
            this.WorkflowDependencyGroupId = workflowDependencyGroup.Id;
            this.WorkflowEvent = workflowEvent;
            this.WorkflowEventId = workflowEvent.Id;
        }
    }
}
