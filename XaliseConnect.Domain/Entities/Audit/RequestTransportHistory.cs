using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Audit;

/// <summary>
/// Représente une entrée d'historique du statut de transport d'une demande.
/// </summary>
public class RequestTransportHistory
{
    /// <summary>
    /// Identifiant unique de l'entrée d'historique.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Date et heure UTC de création de l'entrée.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Détail de l'événement d'historique, ou <see langword="null"/> si aucun détail n'est disponible.
    /// </summary>
    [MaxLength(254)]
    public string? Details { get; set; }

    /// <summary>
    /// Identifiant de la demande d'intégration concernée.
    /// </summary>
    public int IntegrationRequestId { get; set; }

    /// <summary>
    /// Identifiant du statut de transport enregistré.
    /// </summary>
    public int StatusTransportId { get; set; }
}
