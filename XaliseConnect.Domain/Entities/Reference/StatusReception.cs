using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Reference;

/// <summary>
/// Représente un statut de réception d'une demande d'intégration.
/// </summary>
public class StatusReception
{
    /// <summary>
    /// Identifiant unique du statut de réception.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Libellé du statut de réception.
    /// </summary>
    [MaxLength(40)]
    public string Label { get; set; } = string.Empty;
}
