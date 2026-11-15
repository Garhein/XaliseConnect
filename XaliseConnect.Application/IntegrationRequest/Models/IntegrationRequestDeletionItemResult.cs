using XaliseConnect.Application.IntegrationRequest.Enums;

namespace XaliseConnect.Application.IntegrationRequest.Models;

/// <summary>
/// Résultat de suppression d'une demande dans une commande groupée.
/// </summary>
public sealed class IntegrationRequestDeletionItemResult
{
    /// <summary>
    /// Identifiant de la demande traitée.
    /// </summary>
    public int RecordId { get; }

    /// <summary>
    /// Résultat de la suppression de la demande.
    /// </summary>
    public IntegrationRequestDeletionOutcome Outcome { get; }

    /// <summary>
    /// Initialise le résultat de suppression d'une demande.
    /// </summary>
    /// <param name="recordId">Identifiant de la demande traitée.</param>
    /// <param name="outcome">Résultat de la suppression.</param>
    public IntegrationRequestDeletionItemResult(int recordId, IntegrationRequestDeletionOutcome outcome)
    {
        this.RecordId = recordId;
        this.Outcome  = outcome;
    }
}
