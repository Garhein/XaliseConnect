namespace XaliseConnect.Domain.Entities.Workflow;

/// <summary>
/// Représente un groupe de dépendances associé à un événement de workflow.
/// </summary>
public class WorkflowDependencyGroup
{
    /// <summary>
    /// Identifiant unique du groupe de dépendances.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant du type de dépendance du groupe.
    /// </summary>
    public int WorkflowDependencyTypeId { get; set; }

    /// <summary>
    /// Identifiant de l'événement auquel le groupe est associé.
    /// </summary>
    public int WorkflowEventId { get; set; }
}
