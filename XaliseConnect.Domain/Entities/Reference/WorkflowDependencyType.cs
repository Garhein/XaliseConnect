namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des types de dépendances de workflow utilisés par l'application.
    /// </summary>
    public sealed class WorkflowDependencyType : BaseEntity
    {
        /// <summary>
        /// Description du type de dépendance de workflow.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private WorkflowDependencyType() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="WorkflowDependencyType"/> avec une description.
        /// </summary>
        /// <param name="description">Description du type de dépendance de workflow.</param>
        public WorkflowDependencyType(string description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description, nameof(description));

            this.Description = description;
        }
    }
}