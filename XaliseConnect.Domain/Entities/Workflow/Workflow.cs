using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Workflow;

/// <summary>
/// Représente la définition versionnée d'un flux de traitement.
/// </summary>
public class Workflow
{
    /// <summary>
    /// Identifiant unique du workflow.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Libellé du workflow.
    /// </summary>
    [MaxLength(40)]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Description du workflow, ou <see langword="null"/> si elle n'est pas renseignée.
    /// </summary>
    [MaxLength(254)]
    public string? Description { get; set; }

    /// <summary>
    /// Date et heure UTC d'archivage, ou <see langword="null"/> si le workflow est actif.
    /// </summary>
    public DateTime? ArchivedAt { get; set; }

    /// <summary>
    /// Numéro de version du workflow.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Identifiant de la règle de corrélation associée.
    /// </summary>
    public int CorrelationRuleId { get; set; }
}
