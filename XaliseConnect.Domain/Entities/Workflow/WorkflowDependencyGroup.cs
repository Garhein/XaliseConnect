using XaliseConnect.Domain.Entities.Reference;

namespace XaliseConnect.Domain.Entities.Workflow
{
    /// <summary>
    /// Groupe de dépendances d'un flux.
    /// </summary>
    public sealed class WorkflowDependencyGroup : BaseEntity
    {
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
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private WorkflowDependencyGroup() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="WorkflowDependencyGroup"/> avec un événement du flux et un type de dépendance du flux.
        /// </summary>
        /// <param name="workflowEvent">Événement du flux associé au groupe de dépendances.</param>
        /// <param name="workflowDependencyType">Type de dépendance du flux associé au groupe de dépendances.</param>
        public WorkflowDependencyGroup(WorkflowEvent workflowEvent, WorkflowDependencyType workflowDependencyType)
        {
            ArgumentNullException.ThrowIfNull(workflowEvent, nameof(workflowEvent));
            ArgumentNullException.ThrowIfNull(workflowDependencyType, nameof(workflowDependencyType));

            this.WorkflowEvent = workflowEvent;
            this.WorkflowEventId = workflowEvent.Id;
            this.WorkflowDependencyType = workflowDependencyType;
            this.WorkflowDependencyTypeId = workflowDependencyType.Id;
        }
    }
}