using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Configuration;

/// <summary>
/// Représente une source de demandes d'intégration configurée dans l'application.
/// </summary>
public class Source
{
    /// <summary>
    /// Identifiant unique de la source.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Libellé de la source.
    /// </summary>
    [MaxLength(40)]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Description de la source, ou <see langword="null"/> si elle n'est pas renseignée.
    /// </summary>
    [MaxLength(254)]
    public string? Description { get; set; }

    /// <summary>
    /// Date et heure UTC d'archivage, ou <see langword="null"/> si la source est active.
    /// </summary>
    public DateTime? ArchivedAt { get; set; }

    /// <summary>
    /// Identifiant du type de source associé.
    /// </summary>
    public int SourceTypeId { get; set; }
}
