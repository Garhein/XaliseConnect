using XaliseConnect.Application.IntegrationRequest.Enums;

namespace XaliseConnect.Application.IntegrationRequest.Models;

/// <summary>
/// Résultat d'une demande de suppression de plusieurs demandes d'intégration.
/// </summary>
public sealed class IntegrationRequestDeletionItemResult
{
    /// <summary>
    /// Identifiant de la demande d'intégration traitée.
    /// </summary>
    public int RecordId { get; }

    /// <summary>
    /// Résultat de la suppression de la demande d'intégration.
    /// </summary>
    public EnumIntegrationRequestDeletionResult Outcome { get; }

    /// <summary>
    /// Initialise le résultat de suppression d'une demande d'intégration.
    /// </summary>
    /// <param name="recordId">Identifiant de la demande d'intégration traitée.</param>
    /// <param name="outcome">Résultat de la suppression.</param>
    public IntegrationRequestDeletionItemResult(int recordId, EnumIntegrationRequestDeletionResult outcome)
    {
        this.RecordId = recordId;
        this.Outcome  = outcome;
    }
}
