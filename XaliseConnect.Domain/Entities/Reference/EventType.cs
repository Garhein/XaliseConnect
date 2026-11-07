using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Reference;

/// <summary>
/// Représente un type d'événement utilisable par Xalise Connect.
/// </summary>
public class EventType
{
    /// <summary>
    /// Identifiant unique du type d'événement.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Code du type d'événement.
    /// </summary>
    [MaxLength(40)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Description du type d'événement.
    /// </summary>
    [MaxLength(254)]
    public string Description { get; set; } = string.Empty;
}
