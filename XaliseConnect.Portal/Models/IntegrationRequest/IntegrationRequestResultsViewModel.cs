using XaliseConnect.Application.IntegrationRequest.Models;
using XaliseConnect.Application.Pagination;
using XaliseConnect.Portal.Models.Pagination;

namespace XaliseConnect.Portal.Models.IntegrationRequest;

/// <summary>
/// Modèle des résultats de recherche et de leur état de pagination.
/// </summary>
public sealed class IntegrationRequestResultsViewModel
{
    /// <summary>
    /// Page de demandes d'intégration à afficher.
    /// </summary>
    public PagedResult<IntegrationRequestListItem> Page { get; }

    /// <summary>
    /// État destiné aux contrôles partagés de pagination.
    /// </summary>
    public PaginationViewModel Pagination { get; }

    /// <summary>
    /// Initialise les résultats de recherche.
    /// </summary>
    /// <param name="page">Demandes d'intégration paginées.</param>
    /// <param name="pagination">Modèle des contrôles de pagination.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="page"/> ou <paramref name="pagination"/> est <see langword="null"/>.</exception>
    public IntegrationRequestResultsViewModel(PagedResult<IntegrationRequestListItem> page, PaginationViewModel pagination)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(pagination);

        this.Page       = page;
        this.Pagination = pagination;
    }
}
