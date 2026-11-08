namespace XaliseConnect.Application.IntegrationRequest.Models;

/// <summary>
/// Représente les données d'une demande d'intégration destinées à l'affichage
/// de la liste des demandes d'intégration.
/// </summary>
public sealed class IntegrationRequestListItem
{
    /// <summary>
    /// Identifiant unique de la demande.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Date et heure de réception de la demande.
    /// </summary>
    public required DateTime ReceivedAtUtc { get; init; }

    /// <summary>
    /// Numéro de la demande.
    /// </summary>
    public required string RequestNumber { get; init; }

    /// <summary>
    /// Libellé de la source ayant émis la demande.
    /// </summary>
    public required string SourceLabel { get; init; }

    /// <summary>
    /// Libellé du type de la source.
    /// </summary>
    public required string SourceTypeLabel { get; init; }

    /// <summary>
    /// Référence de la demande fournie par sa source.
    /// </summary>
    public string? SourceReference { get; init; }

    /// <summary>
    /// Libellé du workflow associé à la demande.
    /// </summary>
    public string? WorkflowLabel { get; init; }

    /// <summary>
    /// Règle de corrélation associée à la demande.
    /// </summary>
    public string? CorrelationRuleLabel { get; init; }

    /// <summary>
    /// Numéro d'instance du workflow.
    /// </summary>
    public string? InstanceNumber { get; init; }

    /// <summary>
    /// Libellé du statut de réception, ou <see langword="null"/> si aucun statut n'est associé.
    /// </summary>
    public string? StatusReceptionLabel { get; init; }

    /// <summary>
    /// Libellé du statut de transport, ou <see langword="null"/> si aucun statut n'est associé.
    /// </summary>
    public string? StatusTransportLabel { get; init; }

    /// <summary>
    /// Libellé du statut d'intégration, ou <see langword="null"/> si aucun statut n'est associé.
    /// </summary>
    public string? StatusIntegrationLabel { get; init; }
}
