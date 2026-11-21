using XaliseConnect.Application.IntegrationRequest.Models;
using XaliseConnect.Domain.Enums;
using XaliseConnect.Domain.Helpers;
using XaliseConnect.Application.Pagination;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Mock des opérations associées à la lectures des demandes d'intégration.
/// </summary>
public sealed class MockIntegrationRequestQueryService : IIntegrationRequestQueryService
{
    /// <inheritdoc />
    public Task<PagedResult<IntegrationRequestListItem>> GetIntegrationRequestListAsync(IntegrationRequestSearchCriteria searchCriteria, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(searchCriteria);
        ArgumentNullException.ThrowIfNull(searchCriteria.Pagination);
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<IntegrationRequestListItem> requests = MockIntegrationRequestStore._GetSnapshot();

        if (!string.IsNullOrWhiteSpace(searchCriteria.RequestNumber))
        {
            requests = requests.Where(req => req.RequestNumber == searchCriteria.RequestNumber);
        }

        if (searchCriteria.ReceivedFrom.HasValue)
        {
            requests = requests.Where(req => req.ReceivedAt >= searchCriteria.ReceivedFrom.Value);
        }

        if (searchCriteria.ReceivedTo.HasValue)
        {
            requests = requests.Where(req => req.ReceivedAt <= searchCriteria.ReceivedTo.Value);
        }

        if (searchCriteria.SourceId.HasValue)
        {
            requests = requests.Where(req => req.SourceId == searchCriteria.SourceId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.CorrelationValue))
        {
            requests = requests.Where(req => req.CorrelationRuleValue?.Equals(searchCriteria.CorrelationValue) == true);
        }

        if (searchCriteria.StatusReception.HasValue)
        {
            requests = requests.Where(req => req.StatusReception == searchCriteria.StatusReception.Value);
        }
        else if (searchCriteria.SearchWithoutStatusReception)
        {
            requests = requests.Where(req => req.StatusReception is null);
        }

        if (searchCriteria.StatusTransport.HasValue)
        {
            requests = requests.Where(req => req.StatusTransport == searchCriteria.StatusTransport.Value);
        }
        else if (searchCriteria.SearchWithoutStatusTransport)
        {
            requests = requests.Where(req => req.StatusTransport is null);
        }

        if (searchCriteria.StatusIntegration.HasValue)
        {
            requests = requests.Where(req => req.StatusIntegration == searchCriteria.StatusIntegration.Value);
        }
        else if (searchCriteria.SearchWithoutStatusIntegration)
        {
            requests = requests.Where(req => req.StatusIntegration is null);
        }

        (string sortColumn, string sortDirection) = this._NormalizeSortCriteria(searchCriteria.Pagination);

        IOrderedEnumerable<IntegrationRequestListItem> orderedRequests = (sortColumn, sortDirection) switch
        {
            (IntegrationRequestSortColumn.CSTS_REQUEST_NUMBER, PaginationSortDirection.CSTS_ASCENDING)  => requests.OrderBy(req => req.RequestNumber),
            (IntegrationRequestSortColumn.CSTS_REQUEST_NUMBER, PaginationSortDirection.CSTS_DESCENDING) => requests.OrderByDescending(req => req.RequestNumber),
            (IntegrationRequestSortColumn.CSTS_SOURCE, PaginationSortDirection.CSTS_ASCENDING)          => requests.OrderBy(req => req.SourceLabel),
            (IntegrationRequestSortColumn.CSTS_SOURCE, PaginationSortDirection.CSTS_DESCENDING)         => requests.OrderByDescending(req => req.SourceLabel),
            (IntegrationRequestSortColumn.CSTS_RECEIVED_AT, PaginationSortDirection.CSTS_ASCENDING)     => requests.OrderBy(req => req.ReceivedAt),
            (IntegrationRequestSortColumn.CSTS_RECEIVED_AT, PaginationSortDirection.CSTS_DESCENDING)    => requests.OrderByDescending(req => req.ReceivedAt),
            _ => throw new InvalidOperationException("Les critères de tri normalisés ne correspondent à aucune combinaison prise en charge.")
        };

        int totalItemCount  = requests.Count();
        orderedRequests     = orderedRequests.ThenBy(req => req.Id);

        int pageSize        = searchCriteria.Pagination.PageSize;
        int totalPageCount  = Math.Max(1, (int)Math.Ceiling(totalItemCount / (double)pageSize));
        int pageNumber      = Math.Clamp(searchCriteria.Pagination.PageNumber, 1, totalPageCount);

        IReadOnlyList<IntegrationRequestListItem> pageItems = orderedRequests
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        PagedResult<IntegrationRequestListItem> result = new PagedResult<IntegrationRequestListItem>(
            pageItems,
            pageNumber,
            pageSize,
            totalItemCount,
            sortColumn,
            sortDirection
        );

        return Task.FromResult(result);
    }

    /// <summary>
    /// Normalise les critères de tri et retourne les clés canoniques qui seront appliquées.
    /// Si une clé non vide est inconnue, l'ensemble des critères revient au tri par défaut (date de réception décroissante).
    /// </summary>
    /// <param name="paginationCriteria">Critères de tri et de pagination à normaliser.</param>
    /// <returns>Clé de colonne et direction canoniques à appliquer et à retourner dans les métadonnées.</returns>
    /// <exception cref="ArgumentNullException">Si <paramref name="paginationCriteria"/> est <see langword="null"/>.</exception>
    private (string SortColumn, string SortDirection) _NormalizeSortCriteria(PaginationCriteria paginationCriteria)
    {
        ArgumentNullException.ThrowIfNull(paginationCriteria);

        string? requestedSortColumn     = paginationCriteria.SortColumn;
        string? requestedSortDirection  = paginationCriteria.SortDirection?.ToLowerInvariant();

        bool isSortColumnSupported = requestedSortColumn is null
            or IntegrationRequestSortColumn.CSTS_RECEIVED_AT
            or IntegrationRequestSortColumn.CSTS_REQUEST_NUMBER
            or IntegrationRequestSortColumn.CSTS_SOURCE;

        bool isSortDirectionSupported = requestedSortDirection is null
            or PaginationSortDirection.CSTS_ASCENDING
            or PaginationSortDirection.CSTS_DESCENDING;

        if (!isSortColumnSupported || !isSortDirectionSupported)
        {
            return (
                IntegrationRequestSortColumn.CSTS_RECEIVED_AT,
                PaginationSortDirection.CSTS_DESCENDING
            );
        }

        return (
            requestedSortColumn ?? IntegrationRequestSortColumn.CSTS_RECEIVED_AT,
            requestedSortDirection ?? PaginationSortDirection.CSTS_DESCENDING
        );
    }

    /// <summary>
    /// Crée les données initiales de démonstration partagées par les mocks.
    /// </summary>
    /// <returns>Collection de 39 demandes d'intégration de démonstration.</returns>
    internal static IntegrationRequestListItem[] _CreateInitialData()
    {
        List<IntegrationRequestListItem> requests = new List<IntegrationRequestListItem>
        {
            new IntegrationRequestListItem()
            {
                Id                      = 2026110701,
                ReceivedAt              = new DateTime(2026, 11, 7, 12, 42, 0),
                RequestNumber           = "INT-2026110701",
                SourceLabel             = "Dépôt interne",
                SourceId                = 1,
                SourceTypeLabel         = "Fichier",
                WorkflowLabel           = "Admission",
                CorrelationRuleLabel    = "PV1-19",
                CorrelationRuleValue    = "EVT-008401",
                InstanceNumber          = "WF-1049",
                StatusReceptionLabel    = EnumStatusReception.RECUE.GetDescription(),
                StatusReception         = EnumStatusReception.RECUE,
                SourceReference         = "\\\\serveur\\hl7\\in\\patient.hl7"
            },
            new IntegrationRequestListItem()
            {
                Id                      = 2026110602,
                ReceivedAt              = new DateTime(2026, 11, 6, 9, 21, 0),
                RequestNumber           = "INT-2026110602",
                SourceLabel             = "Transmission interne",
                SourceId                = 2,
                SourceTypeLabel         = "API",
                SourceReference         = "POST /api/patients/",
                WorkflowLabel           = "Admission",
                CorrelationRuleLabel    = "PV1-19",
                CorrelationRuleValue    = "EVT-008422",
                InstanceNumber          = "WF-1085",
                StatusReceptionLabel    = EnumStatusReception.VALIDEE.GetDescription(),
                StatusReception         = EnumStatusReception.VALIDEE,
                StatusTransportLabel    = EnumStatusTransport.ECHEC_TRANSPORT.GetDescription(),
                StatusTransport         = EnumStatusTransport.ECHEC_TRANSPORT
            },
            new IntegrationRequestListItem()
            {
                Id                      = 2026110503,
                ReceivedAt              = new DateTime(2026, 11, 5, 19, 49, 0),
                RequestNumber           = "INT-2026110503",
                SourceLabel             = "Transmission externe",
                SourceId                = 3,
                SourceTypeLabel         = "MLLP",
                SourceReference         = "192.168.1.10:2575",
                WorkflowLabel           = "Identité patient",
                CorrelationRuleLabel    = "PID-3",
                CorrelationRuleValue    = "EVT-008392",
                InstanceNumber          = "WF-1051",
                StatusReceptionLabel    = EnumStatusReception.VALIDEE.GetDescription(),
                StatusReception         = EnumStatusReception.VALIDEE,
                StatusTransportLabel    = EnumStatusTransport.ENVOYEE.GetDescription(),
                StatusTransport         = EnumStatusTransport.ENVOYEE,
                StatusIntegrationLabel  = EnumStatusIntegration.EN_ATTENTE.GetDescription(),
                StatusIntegration       = EnumStatusIntegration.EN_ATTENTE
            },
            new IntegrationRequestListItem()
            {
                Id                      = 2026110404,
                ReceivedAt              = new DateTime(2026, 11, 4, 8, 15, 0),
                RequestNumber           = "INT-2026110404",
                SourceLabel             = "Dépôt interne",
                SourceId                = 1,
                SourceTypeLabel         = "Fichier",
                SourceReference         = "\\\\serveur\\hl7\\in\\patient-2.hl7"
            }
        };

        for (int requestIndex = 5; requestIndex <= 39; requestIndex++)
        {
            int statusCase  = (requestIndex - 5) % 16;
            int sourceId    = (requestIndex % 3) + 1;

            EnumStatusReception? statusReception = statusCase switch
            {
                0 => null,
                1 => EnumStatusReception.EN_ATTENTE,
                2 => EnumStatusReception.RECUE,
                3 => EnumStatusReception.ECHEC_RECEPTION,
                4 => EnumStatusReception.ECHEC_VALIDATION,
                _ => EnumStatusReception.VALIDEE
            };

            EnumStatusTransport? statusTransport = statusCase switch
            {
                6 => EnumStatusTransport.EN_ATTENTE,
                7 => EnumStatusTransport.EN_COURS,
                8 => EnumStatusTransport.ECHEC_TRANSPORT,
                9 => EnumStatusTransport.DELAI_ACCUSE_RECEPTION_DEPASSE,
                >= 10 => EnumStatusTransport.ENVOYEE,
                _ => null
            };

            EnumStatusIntegration? statusIntegration = statusCase switch
            {
                11 => EnumStatusIntegration.EN_ATTENTE,
                12 => EnumStatusIntegration.EN_COURS,
                13 => EnumStatusIntegration.REUSSIE,
                14 => EnumStatusIntegration.ECHEC_INTEGRATION,
                15 => EnumStatusIntegration.REJETEE,
                _ => null
            };

            string? correlationRuleValue = statusReception.HasValue
                ? $"EVT-MOCK-{requestIndex:D4}"
                : null;

            string? instanceNumber = statusReception.HasValue
                ? $"WF-MOCK-{requestIndex:D4}"
                : null;

            string? workflowLabel = correlationRuleValue is null
                ? null
                : sourceId switch
                {
                    1 => "Admission",
                    2 => "Identité patient",
                    _ => "Mise à jour du dossier"
                };

            string? correlationRuleLabel = correlationRuleValue is null
                ? null
                : sourceId switch
                {
                    1 => "PV1-19",
                    2 => "PID-3",
                    _ => "MRG-1"
                };

            requests.Add(new IntegrationRequestListItem()
            {
                Id                      = 2026110000 + requestIndex,
                ReceivedAt              = new DateTime(2026, 11, 4, 8, 0, 0).AddDays(5 - requestIndex).AddMinutes(requestIndex),
                RequestNumber           = $"INT-202611-{requestIndex:D4}",
                SourceLabel             = sourceId switch
                {
                    1 => "Dépôt interne",
                    2 => "Transmission interne",
                    _ => "Transmission externe"
                },
                SourceId                = sourceId,
                SourceTypeLabel         = sourceId switch
                {
                    1 => "Fichier",
                    2 => "API",
                    _ => "MLLP"
                },
                SourceReference         = sourceId switch
                {
                    1 => $"\\\\serveur\\hl7\\in\\message-{requestIndex:D4}.hl7",
                    2 => $"POST /api/patients/{requestIndex:D4}",
                    _ => $"192.168.1.{10 + requestIndex}:2575"
                },
                WorkflowLabel           = workflowLabel,
                CorrelationRuleLabel    = correlationRuleLabel,
                CorrelationRuleValue    = correlationRuleValue,
                InstanceNumber          = instanceNumber,
                StatusReceptionLabel    = statusReception.HasValue
                    ? statusReception.Value.GetDescription()
                    : null,
                StatusReception         = statusReception,
                StatusTransportLabel    = statusTransport.HasValue
                    ? statusTransport.Value.GetDescription()
                    : null,
                StatusTransport         = statusTransport,
                StatusIntegrationLabel  = statusIntegration.HasValue
                    ? statusIntegration.Value.GetDescription()
                    : null,
                StatusIntegration       = statusIntegration
            });
        }

        return requests.ToArray();
    }
}
