using XaliseConnect.Application.IntegrationRequest.Enums;
using XaliseConnect.Application.IntegrationRequest.Models;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Stockage en mémoire partagé des données de démonstration utilisées par les mocks
/// associés aux demandes d'intégration.
/// </summary>
internal static class MockIntegrationRequestStore
{
    private static readonly object _syncRoot = new object();

    private static readonly List<IntegrationRequestListItem> _requests = new List<IntegrationRequestListItem>(MockIntegrationRequestQueryService._CreateInitialData());

    /// <summary>
    /// Récupère une copie des demandes d'intégration actuellement stockées.
    /// </summary>
    /// <returns>Une copie des demandes d'intégration disponibles.</returns>
    internal static IReadOnlyList<IntegrationRequestListItem> _GetSnapshot()
    {
        lock (MockIntegrationRequestStore._syncRoot)
        {
            return MockIntegrationRequestStore._requests.ToArray();
        }
    }

    /// <summary>
    /// Supprime une demande d'intégration du stockage.
    /// </summary>
    /// <param name="requestId">Identifiant de la demande d'intégration à supprimer.</param>
    /// <returns><see langword="true"/> si la demande d'intégration a été supprimée, sinon <see langword="false"/>.</returns>
    internal static bool _TryRemove(int requestId)
    {
        lock (MockIntegrationRequestStore._syncRoot)
        {
            int requestIndex = MockIntegrationRequestStore._requests.FindIndex(request => request.Id == requestId);

            if (requestIndex < 0)
            {
                return false;
            }

            MockIntegrationRequestStore._requests.RemoveAt(requestIndex);

            return true;
        }
    }
    
    /// <summary>
    /// Supprime plusieurs demandes d'intégration et conserve un résultat pour chaque identifiant distinct.
    /// </summary>
    /// <param name="requestIds">Identifiants des demandes d'intégration à supprimer.</param>
    /// <returns>Résultats correspondant aux identifiants traités.</returns>
    internal static IReadOnlyList<IntegrationRequestDeletionItemResult> _TryRemoveMany(IReadOnlyCollection<int> requestIds)
    {
        lock (MockIntegrationRequestStore._syncRoot)
        {
            List<IntegrationRequestDeletionItemResult> results = new List<IntegrationRequestDeletionItemResult>();

            foreach (int requestId in requestIds.Distinct())
            {
                int requestIndex = MockIntegrationRequestStore._requests.FindIndex(request => request.Id == requestId);

                EnumIntegrationRequestDeletionResult outcome = requestIndex < 0
                    ? EnumIntegrationRequestDeletionResult.NotFound
                    : EnumIntegrationRequestDeletionResult.Deleted;

                if (requestIndex >= 0)
                {
                    MockIntegrationRequestStore._requests.RemoveAt(requestIndex);
                }

                results.Add(new IntegrationRequestDeletionItemResult(requestId, outcome));
            }

            return results;
        }
    }
}
