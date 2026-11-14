namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Paramètres de la requête HTTP de recherche des demandes d'intégration.
/// </summary>
public sealed class IntegrationRequestSearchRequest
{
    /// <summary>
    /// Numéro de demande recherché.
    /// </summary>
    public string? RequestNumber { get; set; }

    /// <summary>
    /// Date et heure UTC minimales de réception.
    /// </summary>
    public DateTimeOffset? ReceivedFromUtc { get; set; }

    /// <summary>
    /// Date et heure UTC maximales de réception.
    /// </summary>
    public DateTimeOffset? ReceivedToUtc { get; set; }

    /// <summary>
    /// Identifiant de la source recherchée.
    /// </summary>
    public int? SourceId { get; set; }

    /// <summary>
    /// Valeur de corrélation recherchée.
    /// </summary>
    public string? CorrelationValue { get; set; }

    /// <summary>
    /// Valeur sélectionnée pour le statut de réception.
    /// </summary>
    public string? StatusReception { get; set; }

    /// <summary>
    /// Valeur sélectionnée pour le statut de transport.
    /// </summary>
    public string? StatusTransport { get; set; }

    /// <summary>
    /// Valeur sélectionnée pour le statut d'intégration.
    /// </summary>
    public string? StatusIntegration { get; set; }

    /// <summary>
    /// Numéro de page demandé.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Nombre maximal de demandes à afficher par page.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Colonne de tri demandée.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Sens de tri demandé.
    /// </summary>
    public string? Direction { get; set; }
}
