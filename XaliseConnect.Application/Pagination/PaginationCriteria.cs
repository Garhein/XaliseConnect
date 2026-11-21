namespace XaliseConnect.Application.Pagination;

/// <summary>
/// Critères génériques de tri et de pagination.
/// </summary>
public sealed class PaginationCriteria
{
    private int _pageSize = PaginationPolicy.CSTN_DEFAULT_PAGE_SIZE;

    /// <summary>
    /// Numéro de la page demandée, à partir de 1.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Nombre d'éléments affichés sur une page, normalisé selon la politique commune de pagination.
    /// </summary>
    public int PageSize
    {
        get => this._pageSize;
        init => this._pageSize = PaginationPolicy.NormalizePageSize(value);
    }

    /// <summary>
    /// Clé de la colonne de tri, ou <see langword="null"/> si aucun tri n'est demandé.
    /// </summary>
    public string? SortColumn { get; init; }

    /// <summary>
    /// Sens du tri, ou <see langword="null"/> si aucun sens n'est demandé.
    /// </summary>
    public string? SortDirection { get; init; }
}
