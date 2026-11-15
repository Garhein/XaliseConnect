using XaliseConnect.Application.IntegrationRequest.Enums;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Définition des opérations associées à la modification des demandes d'intégration.
/// </summary>
public interface IIntegrationRequestCommandService
{
    /// <summary>
    /// Supprime une demande d'intégration.
    /// </summary>
    /// <param name="recordId">Identifiant de la demande à supprimer.</param>
    /// <param name="cancellationToken">Jeton d'annulation de la demande.</param>
    /// <returns>Résultat indiquant si la demande a été supprimée, est introuvable ou si une erreur est survenue.</returns>
    /// <exception cref="OperationCanceledException">Si l'opération a été annulée.</exception>
    Task<IntegrationRequestDeletionOutcome> DeleteIntegrationRequestAsync(int recordId, CancellationToken cancellationToken);
}
