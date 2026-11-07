using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Reference;

/// <summary>
/// Représente un statut de transport d'une demande d'intégration.
/// </summary>
public class StatusTransport
{
    /// <summary>
    /// Identifiant unique du statut de transport.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Libellé du statut de transport.
    /// </summary>
    [MaxLength(40)]
    public string Label { get; set; } = string.Empty;
}
