namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Données de contexte de la vue récapitulative d'une suppression groupée.
/// </summary>
public sealed class IntegrationRequestDeletionSummaryViewModel
{
    /// <summary>
    /// Numéros des demandes supprimées.
    /// </summary>
    public IReadOnlyList<string> DeletedRequestNumbers { get; }

    /// <summary>
    /// Demandes dont la suppression a échoué.
    /// </summary>
    public IReadOnlyList<IntegrationRequestDeletionFailureViewModel> FailedRequests { get; }

    /// <summary>
    /// Nombre de demandes supprimées.
    /// </summary>
    public int DeletedCount => this.DeletedRequestNumbers.Count;

    /// <summary>
    /// Nombre de demandes dont la suppression a échoué.
    /// </summary>
    public int FailedCount => this.FailedRequests.Count;

    /// <summary>
    /// Nombre total de demandes pour lesquelles la suppression a été demandée.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Initialise le contexte de la vue récapitulative.
    /// </summary>
    /// <param name="deletedRequestNumbers">Numéros des demandes supprimées.</param>
    /// <param name="failedRequests">Demandes dont la suppression a échoué.</param>
    /// <param name="totalCount">Nombre de demandes distinctes traitées.</param>
    /// <exception cref="ArgumentNullException">Si une collection est <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Si <paramref name="totalCount"/> est négatif ou inférieur au nombre de résultats reçus.</exception>
    public IntegrationRequestDeletionSummaryViewModel(
        IReadOnlyList<string> deletedRequestNumbers,
        IReadOnlyList<IntegrationRequestDeletionFailureViewModel> failedRequests,
        int totalCount)
    {
        ArgumentNullException.ThrowIfNull(deletedRequestNumbers);
        ArgumentNullException.ThrowIfNull(failedRequests);

        if (totalCount < 0 || totalCount < deletedRequestNumbers.Count + failedRequests.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(totalCount));
        }

        this.DeletedRequestNumbers  = deletedRequestNumbers;
        this.FailedRequests         = failedRequests;
        this.TotalCount             = totalCount;
    }
}
