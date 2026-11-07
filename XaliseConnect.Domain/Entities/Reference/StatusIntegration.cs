using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Reference;

/// <summary>
/// Représente un statut d'intégration d'une demande.
/// </summary>
public class StatusIntegration
{
    /// <summary>
    /// Identifiant unique du statut d'intégration.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Libellé du statut d'intégration.
    /// </summary>
    [MaxLength(40)]
    public string Label { get; set; } = string.Empty;
}
