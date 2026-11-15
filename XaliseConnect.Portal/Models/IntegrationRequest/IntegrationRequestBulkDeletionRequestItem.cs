namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Demande de suppression individuelle transmise dans une commande groupée.
/// </summary>
public sealed class IntegrationRequestBulkDeletionRequestItem
{
    /// <summary>
    /// Identifiant interne de la demande.
    /// </summary>
    public int RecordId { get; set; }

    /// <summary>
    /// Numéro lisible de la demande, utilisé dans le récapitulatif de suppression.
    /// </summary>
    public string RequestNumber { get; set; } = string.Empty;
}
