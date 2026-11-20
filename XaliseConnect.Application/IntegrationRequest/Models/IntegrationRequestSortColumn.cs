namespace XaliseConnect.Application.IntegrationRequest.Models;

/// <summary>
/// Définition des colonnes de tri disponibles pour les demandes d'intégration.
/// </summary>
public static class IntegrationRequestSortColumn
{
    /// <summary>
    /// Date de réception.
    /// </summary>
    public const string CSTS_RECEIVED_AT = "receivedAt";

    /// <summary>
    /// Numéro de demande.
    /// </summary>
    public const string CSTS_REQUEST_NUMBER = "requestNumber";

    /// <summary>
    /// Source.
    /// </summary>
    public const string CSTS_SOURCE = "source";
}
