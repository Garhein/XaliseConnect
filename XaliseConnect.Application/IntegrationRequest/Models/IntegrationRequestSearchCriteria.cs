using XaliseConnect.Application.Pagination;
using XaliseConnect.Domain.Enums;

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
        SortColumn      = IntegrationRequestSortColumn.CSTS_RECEIVED_AT,
        SortDirection   = PaginationSortDirection.CSTS_DESCENDING
    };

    /// <summary>
    /// Numéro de la demande d'intégration.
    /// </summary>
    public string? RequestNumber { get; init; }

    /// <summary>
    /// Date et heure minimales de réception.
    /// </summary>
    public DateTime? ReceivedFrom { get; init; }

    /// <summary>
    /// Date et heure maximales de réception.
    /// </summary>
    public DateTime? ReceivedTo { get; init; }

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
    /// <see langword="true"/> si la recherche porte sur les demandes sans statut de réception, sinon <see langword="false"/>.
    /// </summary>
    public bool SearchWithoutStatusReception { get; init; }

    /// <summary>
    /// Statut de transport.
    /// </summary>
    public EnumStatusTransport? StatusTransport { get; init; }

    /// <summary>
    /// <see langword="true"/> si la recherche porte sur les demandes sans statut de transport, sinon <see langword="false"/>.
    /// </summary>
    public bool SearchWithoutStatusTransport { get; init; }

    /// <summary>
    /// Statut d'intégration.
    /// </summary>
    public EnumStatusIntegration? StatusIntegration { get; init; }

    /// <summary>
    /// <see langword="true"/> si la recherche porte sur les demandes sans statut d'intégration, sinon <see langword="false"/>.
    /// </summary>
    public bool SearchWithoutStatusIntegration { get; init; }
}
