using XaliseConnect.Application.IntegrationRequest.Models;

namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Modèle de données pour l'affichage de la liste des demandes d'intégration.
/// </summary>
public sealed class IntegrationRequestListViewModel
{
    /// <summary>
    /// Liste des demandes d'intégration à afficher.
    /// </summary>
    public IReadOnlyList<IntegrationRequestListItem> Requests { get; }

    /// <summary>
    /// Critères de recherche applicables à la liste des demandes d'intégration.
    /// </summary>
    public IntegrationRequestSearchCriteriaViewModel SearchCriteria { get; }

    /// <summary>
    /// Initialise le modèle d'affichage de la liste des demandes d'intégration.
    /// </summary>
    /// <param name="requests">Liste des demandes d'intégration à afficher.</param>
    /// <param name="searchCriteria">Critères de recherche applicables à la liste des demandes d'intégration.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="requests"/> ou <paramref name="searchCriteria"/> est <see langword="null"/>.</exception>
    public IntegrationRequestListViewModel(IReadOnlyList<IntegrationRequestListItem> requests, IntegrationRequestSearchCriteriaViewModel searchCriteria)
    {
        ArgumentNullException.ThrowIfNull(requests);
        ArgumentNullException.ThrowIfNull(searchCriteria);

        this.Requests       = requests;
        this.SearchCriteria = searchCriteria;
    }
}
