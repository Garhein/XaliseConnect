namespace XaliseConnect.Domain.Entities.Reference
{
    /// <summary>
    /// Référentiel des types de dépendances de workflow utilisés par l'application.
    /// </summary>
    public sealed class WorkflowDependencyType : BaseEntity
    {
        /// <summary>
        /// Libellé du type de dépendance de workflow.
        /// </summary>
        public string Label { get; private set; } = string.Empty;

        /// <summary>
        /// Constructeur réservé à l'infrastructure (EF Core).
        /// </summary>
        private WorkflowDependencyType() { }

        /// <summary>
        /// Constructeur public pour créer une instance de <see cref="WorkflowDependencyType"/> avec un libellé.
        /// </summary>
        /// <param name="label">Libellé du type de dépendance de workflow.</param>
        public WorkflowDependencyType(string label)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));

            this.Label = label;
        }
    }
}