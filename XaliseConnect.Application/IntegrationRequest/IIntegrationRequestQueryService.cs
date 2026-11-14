using XaliseConnect.Application.IntegrationRequest.Models;
using XaliseConnect.Application.Pagination;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Définition des opérations associées aux demandes d'intégration.
/// </summary>
public interface IIntegrationRequestQueryService
{
    /// <summary>
    /// Récupère la liste des demandes d'intégration.
    /// </summary>
    /// <param name="searchCriteria">Critères à appliquer à la recherche.</param>
    /// <param name="cancellationToken">Jeton d'annulation de la demande.</param>
    /// <returns>Les demandes de la page demandée et les métadonnées de pagination.</returns>
    /// <exception cref="ArgumentNullException">Si <paramref name="searchCriteria"/> est <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Si la demande a été annulée.</exception>
    Task<PagedResult<IntegrationRequestListItem>> GetIntegrationRequestListAsync(IntegrationRequestSearchCriteria searchCriteria, CancellationToken cancellationToken);
}
