namespace XaliseConnect.Domain.Entities.Workflow;

/// <summary>
/// Représente un événement membre d'un groupe de dépendances de workflow.
/// </summary>
public class WorkflowDependencyItem
{
    /// <summary>
    /// Identifiant unique de l'élément de dépendance.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l'événement utilisé comme élément de dépendance.
    /// </summary>
    public int WorkflowEventId { get; set; }

    /// <summary>
    /// Identifiant du groupe de dépendances contenant cet élément.
    /// </summary>
    public int WorkflowDependencyGroupId { get; set; }
}
