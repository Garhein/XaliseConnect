using XaliseConnect.Application.IntegrationRequest.Enums;
using XaliseConnect.Application.IntegrationRequest.Models;

namespace XaliseConnect.Application.IntegrationRequest;

/// <summary>
/// Définition des opérations associées à la modification des demandes d'intégration.
/// </summary>
public interface IIntegrationRequestCommandService
{
    /// <summary>
    /// Supprime une demande d'intégration.
    /// </summary>
    /// <param name="requestId">Identifiant de la demande d'intégration à supprimer.</param>
    /// <param name="cancellationToken">Jeton d'annulation de la demande.</param>
    /// <returns>Résultat indiquant si la demande d'intégration a été supprimée, est introuvable ou si une erreur est survenue.</returns>
    /// <exception cref="OperationCanceledException">Si l'opération a été annulée.</exception>
    Task<EnumIntegrationRequestDeletionResult> DeleteIntegrationRequestAsync(int requestId, CancellationToken cancellationToken);

    /// <summary>
    /// Supprime plusieurs demandes d'intégration.
    /// </summary>
    /// <param name="requestIds">Identifiants des demandes d'intégration à supprimer.</param>
    /// <param name="cancellationToken">Jeton d'annulation de la demande.</param>
    /// <returns>Résultat de suppression associé à chaque identifiant distinct fourni.</returns>
    /// <exception cref="ArgumentNullException">Si <paramref name="requestIds"/> est <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Si l'opération a été annulée.</exception>
    Task<IReadOnlyList<IntegrationRequestDeletionItemResult>> DeleteIntegrationRequestsAsync(IReadOnlyCollection<int> requestIds, CancellationToken cancellationToken);
}
