namespace XaliseConnect.Domain.Entities.Workflow;

/// <summary>
/// Représente un événement configuré dans un workflow.
/// </summary>
public class WorkflowEvent
{
    /// <summary>
    /// Identifiant unique de l'événement du workflow.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Valeur indiquant si l'événement peut être rejoué.
    /// </summary>
    public bool CanReplay { get; set; }

    /// <summary>
    /// Ordre d'exécution de l'événement dans le workflow.
    /// </summary>
    public int ExecutionOrder { get; set; }

    /// <summary>
    /// Nombre minimal d'occurrences, qui doit être supérieur ou égal à zéro.
    /// </summary>
    public int MinOccurrences { get; set; }

    /// <summary>
    /// Nombre maximal d'occurrences, ou <see langword="null"/> si aucune limite n'est définie.
    /// Toute valeur renseignée doit être supérieure ou égale à <see cref="MinOccurrences"/>.
    /// </summary>
    public int? MaxOccurrences { get; set; }

    /// <summary>
    /// Identifiant du type d'événement.
    /// </summary>
    public int EventTypeId { get; set; }

    /// <summary>
    /// Identifiant du workflow auquel l'événement appartient.
    /// </summary>
    public int WorkflowId { get; set; }
}
