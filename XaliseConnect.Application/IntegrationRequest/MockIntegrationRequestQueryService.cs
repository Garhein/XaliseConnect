using XaliseConnect.Application.IntegrationRequest.Models;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Mock de mise à disposition d'une liste fictive de demandes d'intégration.
/// </summary>
public sealed class MockIntegrationRequestQueryService : IIntegrationRequestQueryService
{
    private static readonly IReadOnlyList<IntegrationRequestListItem> _requestIntegration = Array.AsReadOnly(
        new IntegrationRequestListItem[]
        {
            new()
            {
                Id                      = 2026110701,
                ReceivedAtUtc           = new DateTime(2026, 11, 7, 12, 42, 0, DateTimeKind.Utc),
                RequestNumber           = "INT-2026110701",
                SourceLabel             = "Dépôt interne",
                SourceTypeLabel         = "Fichier",
                StatusReceptionLabel    = "Reçue",
                SourceReference         = "\\\\serveur\\hl7\\in\\patient.hl7"
            },
            new()
            {
                Id                      = 2026110602,
                ReceivedAtUtc           = new DateTime(2026, 11, 6, 9, 21, 0, DateTimeKind.Utc),
                RequestNumber           = "INT-2026110602",
                SourceLabel             = "Transmission interne",
                SourceTypeLabel         = "API",
                SourceReference         = "POST /api/patients/",
                WorkflowLabel           = "Admission",
                CorrelationRuleLabel    = "PV1-19",
                CorrelationRuleValue    = "EVT-008422",
                InstanceNumber          = "WF-1085",
                StatusReceptionLabel    = "Validée",
                StatusTransportLabel    = "Erreur"
            },
            new()
            {
                Id                      = 2026110503,
                ReceivedAtUtc           = new DateTime(2026, 11, 5, 19, 49, 0, DateTimeKind.Utc),
                RequestNumber           = "INT-2026110503",
                SourceLabel             = "Transmission externe",
                SourceTypeLabel         = "MLLP",
                SourceReference         = "192.168.1.10:2575",
                WorkflowLabel           = "Identité patient",
                CorrelationRuleLabel    = "PID-3",
                CorrelationRuleValue    = "EVT-008392",
                InstanceNumber          = "WF-1051",
                StatusReceptionLabel    = "Validée",
                StatusTransportLabel    = "Envoyée",
                StatusIntegrationLabel  = "En attente"
            }
        });

    /// <inheritdoc />
    public Task<IReadOnlyList<IntegrationRequestListItem>> GetIntegrationRequestListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(MockIntegrationRequestQueryService._requestIntegration);
    }
}
