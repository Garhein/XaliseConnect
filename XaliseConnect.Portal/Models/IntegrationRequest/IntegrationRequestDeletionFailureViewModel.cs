namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Informations affichées pour une demande dont la suppression a échoué.
/// </summary>
public sealed class IntegrationRequestDeletionFailureViewModel
{
    /// <summary>
    /// Numéro lisible de la demande.
    /// </summary>
    public string RequestNumber { get; }

    /// <summary>
    /// Explication de l'échec de suppression.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initialise les informations d'échec de suppression.
    /// </summary>
    /// <param name="requestNumber">Numéro lisible de la demande.</param>
    /// <param name="message">Explication de l'échec.</param>
    /// <exception cref="ArgumentException">Si le numéro de demande ou le message est vide.</exception>
    public IntegrationRequestDeletionFailureViewModel(string requestNumber, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        this.RequestNumber  = requestNumber;
        this.Message        = message;
    }
}
