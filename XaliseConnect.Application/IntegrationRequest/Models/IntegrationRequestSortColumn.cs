namespace XaliseConnect.Application.IntegrationRequest.Models;

/// <summary>
/// Définit les colonnes de tri disponibles pour les demandes d'intégration.
/// </summary>
public static class IntegrationRequestSortColumn
{
    /// <summary>
    /// Colonne de date de réception.
    /// </summary>
    public const string CSTS_RECEIVED_AT = "receivedAt";

    /// <summary>
    /// Colonne du numéro de demande.
    /// </summary>
    public const string CSTS_REQUEST_NUMBER = "requestNumber";

    /// <summary>
    /// Colonne de la source.
    /// </summary>
    public const string CSTS_SOURCE = "source";
}
