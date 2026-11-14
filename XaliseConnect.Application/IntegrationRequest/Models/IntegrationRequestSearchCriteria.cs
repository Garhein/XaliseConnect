using XaliseConnect.Domain.Enums;

using XaliseConnect.Application.Pagination;

namespace XaliseConnect.Application.IntegrationRequest.Models;

/// <summary>
/// Critères de recherche des demandes d'intégration.
/// </summary>
public sealed class IntegrationRequestSearchCriteria
{
    /// <summary>
    /// Critères de tri et de pagination.
    /// </summary>
    public PaginationCriteria Pagination { get; init; } = new PaginationCriteria()
    {
        SortColumn = "receivedAt",
        SortDirection = "descending"
    };

    /// <summary>
    /// Numéro de demande.
    /// </summary>
    public string? RequestNumber { get; init; }

    /// <summary>
    /// Date et heure UTC minimales de réception.
    /// </summary>
    public DateTimeOffset? ReceivedFromUtc { get; init; }

    /// <summary>
    /// Date et heure UTC maximales de réception.
    /// </summary>
    public DateTimeOffset? ReceivedToUtc { get; init; }

    /// <summary>
    /// Identifiant de la source.
    /// </summary>
    public int? SourceId { get; init; }

    /// <summary>
    /// Valeur de corrélation.
    /// </summary>
    public string? CorrelationValue { get; init; }

    /// <summary>
    /// Statut de réception.
    /// </summary>
    public EnumStatusReception? StatusReception { get; init; }

    /// <summary>
    /// Indique si la recherche porte sur les demandes sans statut de réception.
    /// </summary>
    public bool SearchWithoutStatusReception { get; init; }

    /// <summary>
    /// Statut de transport.
    /// </summary>
    public EnumStatusTransport? StatusTransport { get; init; }

    /// <summary>
    /// Indique si la recherche porte sur les demandes sans statut de transport.
    /// </summary>
    public bool SearchWithoutStatusTransport { get; init; }

    /// <summary>
    /// Statut d'intégration.
    /// </summary>
    public EnumStatusIntegration? StatusIntegration { get; init; }

    /// <summary>
    /// Indique si la recherche porte sur les demandes sans statut d'intégration.
    /// </summary>
    public bool SearchWithoutStatusIntegration { get; init; }
}
