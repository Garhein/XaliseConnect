using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Instance;

/// <summary>
/// Représente une exécution concrète d'un workflow.
/// </summary>
public class WorkflowInstance
{
    /// <summary>
    /// Identifiant unique de l'instance de workflow.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Date et heure UTC de création de l'instance.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure UTC de réception du dernier événement.
    /// </summary>
    public DateTime LastEventReceivedAt { get; set; }

    /// <summary>
    /// Valeur extraite pour corréler les demandes à cette instance.
    /// </summary>
    [MaxLength(80)]
    public string CorrelationRuleValue { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant du workflow exécuté par l'instance.
    /// </summary>
    public int WorkflowId { get; set; }
}
