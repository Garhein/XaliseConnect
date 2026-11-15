using XaliseConnect.Application.IntegrationRequest.Enums;
using XaliseConnect.Application.IntegrationRequest.Models;

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

    /// <inheritdoc />
    public Task<IReadOnlyList<IntegrationRequestDeletionItemResult>> DeleteIntegrationRequestsAsync(IReadOnlyCollection<int> recordIds, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(recordIds);
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<IntegrationRequestDeletionItemResult> results = MockIntegrationRequestStore._TryRemoveMany(recordIds);

        return Task.FromResult(results);
    }
}
