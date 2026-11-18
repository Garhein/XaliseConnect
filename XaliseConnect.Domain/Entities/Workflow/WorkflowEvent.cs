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
    public int MinOccurrences { get; private set; }

    /// <summary>
    /// Nombre maximal d'occurrences, ou <see langword="null"/> si aucune limite n'est définie.
    /// Toute valeur renseignée doit être supérieure ou égale à <see cref="MinOccurrences"/>.
    /// </summary>
    public int? MaxOccurrences { get; private set; }

    /// <summary>
    /// Identifiant du type d'événement.
    /// </summary>
    public int EventTypeId { get; set; }

    /// <summary>
    /// Identifiant du workflow auquel l'événement appartient.
    /// </summary>
    public int WorkflowId { get; set; }

    /// <summary>
    /// Affecte le nombre minimal d'occurrences de l'événement, en s'assurant qu'il est supérieur ou égal à zéro.
    /// </summary>
    /// <param name="minOccurrences">Nombre minimal d'occurrences.</param>
    /// <exception cref="ArgumentOutOfRangeException">Si le nombre minimal d'occurrences est négatif.</exception>
    public void SetMinOccurrences(int minOccurrences)
    {
        if (minOccurrences < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minOccurrences), "Le nombre minimal d'occurrences doit être supérieur ou égal à zéro.");
        }

        if (this.MaxOccurrences.HasValue && minOccurrences > this.MaxOccurrences.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(minOccurrences), "Le nombre minimal d'occurrences ne peut pas être supérieur au nombre maximal d'occurrences.");
        }

        this.MinOccurrences = minOccurrences;
    }

    /// <summary>
    /// Affecte le nombre maximal d'occurrences de l'événement, en s'assurant qu'il est supérieur ou égal au nombre minimal d'occurrences.
    /// </summary>
    /// <param name="maxOccurrences">Nombre maximal d'occurrences.</param>
    /// <exception cref="ArgumentOutOfRangeException">Si le nombre maximal d'occurrences est inférieur au nombre minimal d'occurrences.</exception>
    public void SetMaxOccurrences(int? maxOccurrences)
    {
        if (maxOccurrences.HasValue && maxOccurrences.Value < this.MinOccurrences)
        {
            throw new ArgumentOutOfRangeException(nameof(maxOccurrences), "Le nombre maximal d'occurrences doit être supérieur ou égal au nombre minimal d'occurrences.");
        }

        this.MaxOccurrences = maxOccurrences;
    }
}
