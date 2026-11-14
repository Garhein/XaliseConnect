namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Modèle de données pour l'affichage de la liste des demandes d'intégration.
/// </summary>
public sealed class IntegrationRequestListViewModel
{
    /// <summary>
    /// Résultats paginés des demandes d'intégration.
    /// </summary>
    public IntegrationRequestResultsViewModel Results { get; }

    /// <summary>
    /// Critères de recherche applicables à la liste des demandes d'intégration.
    /// </summary>
    public IntegrationRequestSearchCriteriaViewModel SearchCriteria { get; }

    /// <summary>
    /// Initialise le modèle d'affichage de la liste des demandes d'intégration.
    /// </summary>
    /// <param name="results">Résultats paginés des demandes d'intégration.</param>
    /// <param name="searchCriteria">Critères de recherche applicables à la liste des demandes d'intégration.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="results"/> ou <paramref name="searchCriteria"/> est <see langword="null"/>.</exception>
    public IntegrationRequestListViewModel(IntegrationRequestResultsViewModel results, IntegrationRequestSearchCriteriaViewModel searchCriteria)
    {
        ArgumentNullException.ThrowIfNull(results);
        ArgumentNullException.ThrowIfNull(searchCriteria);

        this.Results        = results;
        this.SearchCriteria = searchCriteria;
    }
}
