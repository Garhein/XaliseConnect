using XaliseConnect.Application.IntegrationRequest.Models;
using XaliseConnect.Domain.Enums;
using XaliseConnect.Domain.Helpers;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Mock des opérations associées aux demandes d'intégration.
/// </summary>
public sealed class MockIntegrationRequestQueryService : IIntegrationRequestQueryService
{
    private static readonly IReadOnlyList<IntegrationRequestListItem> _requestIntegration = Array.AsReadOnly(MockIntegrationRequestQueryService._CreateMockIntegrationRequestList());

    /// <inheritdoc />
    public Task<IReadOnlyList<IntegrationRequestListItem>> GetIntegrationRequestListAsync(IntegrationRequestSearchCriteria searchCriteria, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(searchCriteria);
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<IntegrationRequestListItem> requests = MockIntegrationRequestQueryService._requestIntegration;

        if (searchCriteria.RequestNumber is string requestNumberFilter && !string.IsNullOrWhiteSpace(requestNumberFilter))
        {
            requests = requests.Where(req => req.RequestNumber == requestNumberFilter);
        }

        if (searchCriteria.ReceivedFromUtc is DateTimeOffset receivedFromUtc)
        {
            requests = requests.Where(req => new DateTimeOffset(DateTime.SpecifyKind(req.ReceivedAtUtc, DateTimeKind.Utc)) >= receivedFromUtc.ToUniversalTime());
        }

        if (searchCriteria.ReceivedToUtc is DateTimeOffset receivedToUtc)
        {
            requests = requests.Where(req => new DateTimeOffset(DateTime.SpecifyKind(req.ReceivedAtUtc, DateTimeKind.Utc)) <= receivedToUtc.ToUniversalTime());
        }

        if (searchCriteria.SourceId is int sourceId)
        {
            requests = requests.Where(req => req.SourceId == sourceId);
        }

        if (searchCriteria.CorrelationValue is string correlationValueFilter && !string.IsNullOrWhiteSpace(correlationValueFilter))
        {
            requests = requests.Where(req => req.CorrelationRuleValue?.Equals(correlationValueFilter) == true);
        }

        if (searchCriteria.StatusReception is EnumStatusReception statusReception)
        {
            requests = requests.Where(req => req.StatusReception == statusReception);
        }
        else if (searchCriteria.SearchWithoutStatusReception)
        {
            requests = requests.Where(req => req.StatusReception is null);
        }

        if (searchCriteria.StatusTransport is EnumStatusTransport statusTransport)
        {
            requests = requests.Where(req => req.StatusTransport == statusTransport);
        }
        else if (searchCriteria.SearchWithoutStatusTransport)
        {
            requests = requests.Where(req => req.StatusTransport is null);
        }

        if (searchCriteria.StatusIntegration is EnumStatusIntegration statusIntegration)
        {
            requests = requests.Where(req => req.StatusIntegration == statusIntegration);
        }
        else if (searchCriteria.SearchWithoutStatusIntegration)
        {
            requests = requests.Where(req => req.StatusIntegration is null);
        }

        IReadOnlyList<IntegrationRequestListItem> result = requests.ToArray();

        return Task.FromResult(result);
    }

    /// <summary>
    /// Crée les données de démonstration des demandes d'intégration.
    /// </summary>
    /// <returns>Collection de 39 demandes aux statuts cohérents et aux identifiants uniques.</returns>
    private static IntegrationRequestListItem[] _CreateMockIntegrationRequestList()
    {
        List<IntegrationRequestListItem> requests = new List<IntegrationRequestListItem>
        {
            new IntegrationRequestListItem()
            {
                Id                      = 2026110701,
                ReceivedAtUtc           = new DateTime(2026, 11, 7, 12, 42, 0, DateTimeKind.Utc),
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
                ReceivedAtUtc           = new DateTime(2026, 11, 6, 9, 21, 0, DateTimeKind.Utc),
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
                ReceivedAtUtc           = new DateTime(2026, 11, 5, 19, 49, 0, DateTimeKind.Utc),
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
                ReceivedAtUtc           = new DateTime(2026, 11, 4, 8, 15, 0, DateTimeKind.Utc),
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
                ReceivedAtUtc           = new DateTime(2026, 11, 4, 8, 0, 0, DateTimeKind.Utc).AddDays(5 - requestIndex).AddMinutes(requestIndex),
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
