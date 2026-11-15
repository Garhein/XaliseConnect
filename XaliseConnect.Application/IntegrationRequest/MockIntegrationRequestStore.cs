using XaliseConnect.Application.IntegrationRequest.Enums;
using XaliseConnect.Application.IntegrationRequest.Models;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Stockage en mémoire partagé par les mocks associés aux demandes d'intégration.
/// </summary>
internal static class MockIntegrationRequestStore
{
    private static readonly object _syncRoot = new object();

    private static readonly List<IntegrationRequestListItem> _requests = new List<IntegrationRequestListItem>(MockIntegrationRequestQueryService._CreateInitialData());

    /// <summary>
    /// Récupère une copie des demandes actuellement stockées.
    /// </summary>
    /// <returns>Copie des demandes disponibles.</returns>
    internal static IReadOnlyList<IntegrationRequestListItem> _GetSnapshot()
    {
        lock (MockIntegrationRequestStore._syncRoot)
        {
            return MockIntegrationRequestStore._requests.ToArray();
        }
    }

    /// <summary>
    /// Supprime une demande du stockage simulé.
    /// </summary>
    /// <param name="recordId">Identifiant de la demande à supprimer.</param>
    /// <returns><see langword="true"/> si une demande a été supprimée, sinon <see langword="false"/>.</returns>
    internal static bool _TryRemove(int recordId)
    {
        lock (MockIntegrationRequestStore._syncRoot)
        {
            int requestIndex = MockIntegrationRequestStore._requests.FindIndex(request => request.Id == recordId);

            if (requestIndex < 0)
            {
                return false;
            }

            MockIntegrationRequestStore._requests.RemoveAt(requestIndex);

            return true;
        }
    }

    /// <summary>
    /// Supprime plusieurs demandes et conserve un résultat pour chaque identifiant distinct.
    /// </summary>
    /// <param name="recordIds">Identifiants des demandes à supprimer.</param>
    /// <returns>Résultats correspondant aux identifiants traités.</returns>
    internal static IReadOnlyList<IntegrationRequestDeletionItemResult> _TryRemoveMany(IReadOnlyCollection<int> recordIds)
    {
        lock (MockIntegrationRequestStore._syncRoot)
        {
            List<IntegrationRequestDeletionItemResult> results = new List<IntegrationRequestDeletionItemResult>();

            foreach (int recordId in recordIds.Distinct())
            {
                int requestIndex = MockIntegrationRequestStore._requests.FindIndex(request => request.Id == recordId);

                IntegrationRequestDeletionOutcome outcome = requestIndex < 0
                    ? IntegrationRequestDeletionOutcome.NotFound
                    : IntegrationRequestDeletionOutcome.Deleted;

                if (requestIndex >= 0)
                {
                    MockIntegrationRequestStore._requests.RemoveAt(requestIndex);
                }

                results.Add(new IntegrationRequestDeletionItemResult(recordId, outcome));
            }

            return results;
        }
    }
}
