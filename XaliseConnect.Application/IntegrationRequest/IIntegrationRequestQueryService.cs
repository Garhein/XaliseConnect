using XaliseConnect.Application.IntegrationRequest.Models;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Définition des opérations associées aux demandes d'intégration.
/// </summary>
public interface IIntegrationRequestQueryService
{
    /// <summary>
    /// Récupère la liste des demandes d'intégration à afficher sur le portail.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation de la demande.</param>
    /// <returns>Les demandes d'intégration à afficher.</returns>
    /// <exception cref="OperationCanceledException">Si la demande a été annulée.</exception>
    Task<IReadOnlyList<IntegrationRequestListItem>> GetIntegrationRequestListAsync(CancellationToken cancellationToken);
}
