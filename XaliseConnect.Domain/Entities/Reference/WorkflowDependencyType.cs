using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Reference;

/// <summary>
/// Représente un type de dépendance utilisé par un workflow.
/// </summary>
public class WorkflowDependencyType
{
    /// <summary>
    /// Identifiant unique du type de dépendance.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Libellé du type de dépendance.
    /// </summary>
    [MaxLength(40)]
    public string Label { get; set; } = string.Empty;
}
