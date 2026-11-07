using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Reference;

/// <summary>
/// Représente un type de source pouvant émettre des demandes d'intégration.
/// </summary>
public class SourceType
{
    /// <summary>
    /// Identifiant unique du type de source.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Libellé du type de source.
    /// </summary>
    [MaxLength(40)]
    public string Label { get; set; } = string.Empty;
}
