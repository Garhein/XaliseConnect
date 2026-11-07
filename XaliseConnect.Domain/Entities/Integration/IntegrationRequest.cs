using System.ComponentModel.DataAnnotations;

namespace XaliseConnect.Domain.Entities.Integration;

/// <summary>
/// Représente un message reçu et pris en charge comme demande d'intégration.
/// </summary>
public class IntegrationRequest
{
    /// <summary>
    /// Identifiant unique de la demande d'intégration.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Date et heure UTC de réception de la demande.
    /// </summary>
    public DateTime ReceivedAt { get; set; }

    /// <summary>
    /// Contenu brut du message reçu.
    /// </summary>
    public string RawMessage { get; set; } = string.Empty;

    /// <summary>
    /// Référence de la demande fournie par sa source.
    /// </summary>
    [MaxLength(80)]
    public string SourceReference { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant du statut de réception, ou <see langword="null"/> si aucun statut n'est encore associé.
    /// </summary>
    public int? StatusReceptionId { get; set; }

    /// <summary>
    /// Identifiant du statut de transport, ou <see langword="null"/> si aucun statut n'est encore associé.
    /// </summary>
    public int? StatusTransportId { get; set; }

    /// <summary>
    /// Identifiant du statut d'intégration, ou <see langword="null"/> si aucun statut n'est encore associé.
    /// </summary>
    public int? StatusIntegrationId { get; set; }

    /// <summary>
    /// Identifiant du type d'événement du message.
    /// </summary>
    public int EventTypeId { get; set; }

    /// <summary>
    /// Identifiant de la source de la demande.
    /// </summary>
    public int SourceId { get; set; }

    /// <summary>
    /// Identifiant de l'instance de workflow associée, ou <see langword="null"/> si aucune instance n'est affectée.
    /// </summary>
    public int? WorkflowInstanceId { get; set; }
}
