using XaliseConnect.Application.IntegrationRequest.Models;

namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Modèle d'affichage de la liste des demandes d'intégration.
/// </summary>
public sealed class IntegrationRequestListViewModel
{
    /// <summary>
    /// Liste des demandes d'intégration à afficher.
    /// </summary>
    public IReadOnlyList<IntegrationRequestListItem> Requests { get; }

    /// <summary>
    /// Initialise le modèle d'affichage de la liste des demandes d'intégration.
    /// </summary>
    /// <param name="requests">Liste des demandes d'intégration à afficher.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="requests"/> est <see langword="null"/>.</exception>
    public IntegrationRequestListViewModel(IReadOnlyList<IntegrationRequestListItem> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);

        this.Requests = requests;
    }
}
