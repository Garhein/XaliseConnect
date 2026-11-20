using XaliseConnect.Application.IntegrationRequest.Enums;
using XaliseConnect.Application.IntegrationRequest.Models;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Mock des opérations associées aux modifications des demandes d'intégration.
/// </summary>
public sealed class MockIntegrationRequestCommandService : IIntegrationRequestCommandService
{
    /// <inheritdoc />
    public Task<EnumIntegrationRequestDeletionOutcome> DeleteIntegrationRequestAsync(int requestId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        EnumIntegrationRequestDeletionOutcome result = MockIntegrationRequestStore._TryRemove(requestId)
            ? EnumIntegrationRequestDeletionOutcome.Deleted
            : EnumIntegrationRequestDeletionOutcome.NotFound;

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<IntegrationRequestDeletionItemResult>> DeleteIntegrationRequestsAsync(IReadOnlyCollection<int> requestIds, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(requestIds);
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<IntegrationRequestDeletionItemResult> results = MockIntegrationRequestStore._TryRemoveMany(requestIds);

        return Task.FromResult(results);
    }
}
