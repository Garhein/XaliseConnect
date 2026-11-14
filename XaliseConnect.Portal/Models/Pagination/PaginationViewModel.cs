namespace XaliseConnect.Portal.Models.Pagination;

/// <summary>
/// Modèle partagé des commandes de pagination et de l'état du tri.
/// </summary>
public sealed class PaginationViewModel
{
    /// <summary>
    /// Numéro de la page affichée.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Nombre d'éléments affichés par page.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Nombre total d'éléments correspondant aux critères.
    /// </summary>
    public int TotalItemCount { get; init; }

    /// <summary>
    /// Nombre total de pages.
    /// </summary>
    public int TotalPageCount { get; init; } = 1;

    /// <summary>
    /// Colonne de tri active.
    /// </summary>
    public string? SortColumn { get; init; }

    /// <summary>
    /// Sens de tri actif.
    /// </summary>
    public string? SortDirection { get; init; }

    /// <summary>
    /// Sélecteur CSS de la table pilotée par ces commandes.
    /// </summary>
    public string TableSelector { get; init; } = string.Empty;

    /// <summary>
    /// Identifiant du sélecteur de taille de page.
    /// </summary>
    public string PageSizeSelectId { get; init; } = "xal-id-page-size";

    /// <summary>
    /// Identifiant du sélecteur de page courante.
    /// </summary>
    public string PageSelectId { get; init; } = "xal-id-pagination-jump";

    /// <summary>
    /// Libellé des éléments comptabilisés.
    /// </summary>
    public string ItemLabel { get; init; } = "élément(s)";

    /// <summary>
    /// Libellé accessible de la navigation de pagination.
    /// </summary>
    public string NavigationLabel { get; init; } = "Pagination";

    /// <summary>
    /// Tailles de page proposées à l'utilisateur.
    /// </summary>
    public IReadOnlyList<int> PageSizeOptions { get; init; } = new int[] { 10, 25, 50 };
}
