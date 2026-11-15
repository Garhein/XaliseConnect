using XaliseConnect.Application.IntegrationRequest.Enums;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Mock des opérations associées aux modifications des demandes d'intégration.
/// </summary>
public sealed class MockIntegrationRequestCommandService : IIntegrationRequestCommandService
{
    /// <inheritdoc />
    public Task<IntegrationRequestDeletionOutcome> DeleteIntegrationRequestAsync(int recordId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IntegrationRequestDeletionOutcome result = MockIntegrationRequestStore._TryRemove(recordId)
            ? IntegrationRequestDeletionOutcome.Deleted
            : IntegrationRequestDeletionOutcome.NotFound;

        return Task.FromResult(result);
    }
}
